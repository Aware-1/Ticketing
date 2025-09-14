using Data.Context;
using Data.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Service.IService;

namespace QodraTick.Hubs;


[Authorize]
public class TicketHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public TicketHub(ApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        var user = await _userService.GetCurrentUserAsync();
        if (user != null)
        {
            // Add user to their role group for targeted notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{user.Role.Name}");

            // Add user to their personal group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{user.Id}");

            // If support, add to support group
            if (user.Role.Name == "Support")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "SupportTeam");
            }

            // If admin, add to admin group
            if (user.Role.Name == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "AdminTeam");
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _userService.GetCurrentUserAsync();
        if (user != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Role_{user.Role.Name}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{user.Id}");

            if (user.Role.Name == "Support")
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SupportTeam");
            }

            if (user.Role.Name == "Admin")
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminTeam");
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Join ticket conversation
    public async Task JoinTicket(string ticketId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Ticket_{ticketId}");
    }

    // Leave ticket conversation
    public async Task LeaveTicket(string ticketId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Ticket_{ticketId}");
    }

    // Send message in ticket
    public async Task SendMessage(int ticketId, string message)
    {
        try
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user == null) return;

            // Get ticket and verify user has access
            var ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return;

            // Verify user can send message (owner, assigned support, or admin)
            bool canSendMessage = ticket.CreatedByUserId == user.Id ||
                                ticket.AssignedToUserId == user.Id ||
                                user.Role.Name == "Admin";

            if (!canSendMessage) return;

            // Create message
            var newMessage = new Message
            {
                Content = message,
                TicketId = ticketId,
                UserId = user.Id,
                IsFromSupport = user.Role.Name == "Support" || user.Role.Name == "Admin",
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(newMessage);

            // Update ticket last activity
            ticket.LastActivityAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Load the message with user info
            newMessage = await _context.Messages
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == newMessage.Id);

            // Send message to all users in this ticket
            await Clients.Group($"Ticket_{ticketId}").SendAsync("ReceiveMessage", new
            {
                id = newMessage.Id,
                content = newMessage.Content,
                createdAt = newMessage.CreatedAt.ToString("HH:mm"),
                userName = newMessage.User.DisplayName,
                isFromSupport = newMessage.IsFromSupport,
                userId = newMessage.UserId
            });

            // Send notification to the other party
            if (newMessage.IsFromSupport)
            {
                // Notify ticket creator
                await Clients.Group($"User_{ticket.CreatedByUserId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        type = "message",
                        title = "پاسخ جدید از پشتیبانی",
                        message = $"تیکت «{ticket.Subject}» پاسخ جدید دریافت کرد",
                        ticketId = ticketId,
                        url = $"/tickets/{ticketId}"
                    });
            }
            else if (ticket.AssignedToUserId.HasValue)
            {
                // Notify assigned support
                await Clients.Group($"User_{ticket.AssignedToUserId.Value}")
                    .SendAsync("ReceiveNotification", new
                    {
                        type = "message",
                        title = "پیام جدید از کاربر",
                        message = $"تیکت «{ticket.Subject}» پیام جدید دریافت کرد",
                        ticketId = ticketId,
                        url = $"/tickets/{ticketId}"
                    });
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"خطا در ارسال پیام: {ex.Message}");
        }
    }

    // Accept ticket (for support)
    public async Task AcceptTicket(int ticketId)
    {
        try
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user?.Role.Name != "Support") return;

            var ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket?.Status != TicketStatus.Open) return;

            ticket.Status = TicketStatus.InProgress;
            ticket.AssignedToUserId = user.Id;
            ticket.AssignedAt = DateTime.UtcNow;
            ticket.LastActivityAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Notify ticket creator
            await Clients.Group($"User_{ticket.CreatedByUserId}")
                .SendAsync("ReceiveNotification", new
                {
                    type = "accepted",
                    title = "تیکت پذیرفته شد",
                    message = $"تیکت «{ticket.Subject}» توسط {user.DisplayName} پذیرفته شد",
                    ticketId = ticketId,
                    url = $"/tickets/{ticketId}"
                });

            // Notify support team about ticket assignment
            await Clients.Group("SupportTeam").SendAsync("TicketAssigned", new
            {
                ticketId = ticketId,
                assignedTo = user.DisplayName,
                subject = ticket.Subject
            });

            // Refresh support tickets list
            await Clients.Group("SupportTeam").SendAsync("RefreshTicketList");
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"خطا در پذیرش تیکت: {ex.Message}");
        }
    }

    // Close ticket
    public async Task CloseTicket(int ticketId)
    {
        try
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user?.Role.Name != "Support" && user?.Role.Name != "Admin") return;

            var ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null) return;

            ticket.Status = TicketStatus.Closed;
            ticket.ClosedByUserId = user.Id;
            ticket.ClosedAt = DateTime.UtcNow;
            ticket.LastActivityAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Update statistics
            await UpdateSupportStatistics(user.Id);

            // Notify ticket creator
            await Clients.Group($"User_{ticket.CreatedByUserId}")
                .SendAsync("ReceiveNotification", new
                {
                    type = "closed",
                    title = "تیکت بسته شد",
                    message = $"تیکت «{ticket.Subject}» بسته شد",
                    ticketId = ticketId,
                    url = $"/tickets/{ticketId}"
                });

            // Refresh lists
            await Clients.Group("SupportTeam").SendAsync("RefreshTicketList");
            await Clients.Group("AdminTeam").SendAsync("RefreshDashboard");
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"خطا در بستن تیکت: {ex.Message}");
        }
    }

    // Reassign ticket
    public async Task ReassignTicket(int ticketId, int newSupportUserId)
    {
        try
        {
            var user = await _userService.GetCurrentUserAsync();
            if (user?.Role.Name != "Support" && user?.Role.Name != "Admin") return;

            var ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket?.Status != TicketStatus.InProgress) return;

            var newSupport = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == newSupportUserId && u.Role.Name == "Support");

            if (newSupport == null) return;

            ticket.AssignedToUserId = newSupportUserId;
            ticket.AssignedAt = DateTime.UtcNow;
            ticket.LastActivityAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Notify new assignee
            await Clients.Group($"User_{newSupportUserId}")
                .SendAsync("ReceiveNotification", new
                {
                    type = "reassigned",
                    title = "تیکت جدید اختصاص یافت",
                    message = $"تیکت «{ticket.Subject}» به شما اختصاص یافت",
                    ticketId = ticketId,
                    url = $"/tickets/{ticketId}"
                });

            // Notify ticket creator
            await Clients.Group($"User_{ticket.CreatedByUserId}")
                .SendAsync("ReceiveNotification", new
                {
                    type = "reassigned",
                    title = "تیکت به پشتیبان جدید اختصاص یافت",
                    message = $"تیکت «{ticket.Subject}» به {newSupport.DisplayName} اختصاص یافت",
                    ticketId = ticketId,
                    url = $"/tickets/{ticketId}"
                });

            // Refresh support lists
            await Clients.Group("SupportTeam").SendAsync("RefreshTicketList");
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", $"خطا در انتقال تیکت: {ex.Message}");
        }
    }

    // Notify new ticket created
    public async Task NotifyNewTicket(int ticketId, string subject, string priority)
    {
        // Notify support team
        await Clients.Group("SupportTeam").SendAsync("ReceiveNotification", new
        {
            type = "new_ticket",
            title = "تیکت جدید",
            message = $"تیکت جدید با موضوع «{subject}» ثبت شد - اولویت: {priority}",
            ticketId = ticketId,
            url = $"/support/tickets"
        });

        // Refresh support tickets list
        await Clients.Group("SupportTeam").SendAsync("RefreshTicketList");

        // Notify admins
        await Clients.Group("AdminTeam").SendAsync("RefreshDashboard");
    }

    private async Task UpdateSupportStatistics(int supportUserId)
    {
        var stats = await _context.TicketStatistics
            .FirstOrDefaultAsync(s => s.UserId == supportUserId);

        if (stats == null)
        {
            stats = new TicketStatistic
            {
                UserId = supportUserId,
                TicketsResolved = 1,
                LastUpdated = DateTime.UtcNow
            };
            _context.TicketStatistics.Add(stats);
        }
        else
        {
            stats.TicketsResolved++;
            stats.LastUpdated = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}

