using Microsoft.EntityFrameworkCore;
using EnterpriseTicketing.Application.Interfaces;
using EnterpriseTicketing.Domain.Entities;
using EnterpriseTicketing.Infrastructure.Persistence;

namespace EnterpriseTicketing.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;

    public TicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket?> GetTicketByIdAsync(Guid id)
    {
        return await _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments)
            .Include(t => t.Attachments)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(Guid userId)
    {
        return await _context.Tickets
            .Where(t => t.CreatedById == userId || t.AssignedToId == userId)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetDepartmentTicketsAsync(string department)
    {
        return await _context.Tickets
            .Where(t => t.AssignedTo.Department == department)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Ticket> CreateTicketAsync(Ticket ticket)
    {
        ticket.CreatedAt = DateTime.UtcNow;
        ticket.Status = TicketStatus.New;
        
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task UpdateTicketAsync(Ticket ticket)
    {
        ticket.UpdatedAt = DateTime.UtcNow;
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTicketAsync(Guid id)
    {
        var ticket = await GetTicketByIdAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> AssignTicketAsync(Guid ticketId, Guid assignedToId)
    {
        var ticket = await GetTicketByIdAsync(ticketId);
        if (ticket == null) return false;

        ticket.AssignedToId = assignedToId;
        ticket.Status = TicketStatus.Assigned;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CloseTicketAsync(Guid ticketId, string resolution)
    {
        var ticket = await GetTicketByIdAsync(ticketId);
        if (ticket == null) return false;

        ticket.Status = TicketStatus.Closed;
        ticket.ResolvedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReopenTicketAsync(Guid ticketId, string reason)
    {
        var ticket = await GetTicketByIdAsync(ticketId);
        if (ticket == null) return false;

        ticket.Status = TicketStatus.Reopened;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.ResolvedAt = null;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Ticket>> SearchTicketsAsync(string searchTerm)
    {
        return await _context.Tickets
            .Where(t => t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm))
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();
    }

    public async Task<bool> AddCommentAsync(Guid ticketId, TicketComment comment)
    {
        var ticket = await GetTicketByIdAsync(ticketId);
        if (ticket == null) return false;

        comment.TicketId = ticketId;
        comment.CreatedAt = DateTime.UtcNow;
        
        await _context.TicketComments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddAttachmentAsync(Guid ticketId, TicketAttachment attachment)
    {
        var ticket = await GetTicketByIdAsync(ticketId);
        if (ticket == null) return false;

        attachment.TicketId = ticketId;
        attachment.UploadedAt = DateTime.UtcNow;
        
        await _context.TicketAttachments.AddAsync(attachment);
        await _context.SaveChangesAsync();
        return true;
    }
}