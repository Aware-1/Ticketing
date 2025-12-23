using EnterpriseTicketing.Domain.Entities;

namespace EnterpriseTicketing.Application.Interfaces;

public interface ITicketService
{
    Task<Ticket?> GetTicketByIdAsync(Guid id);
    Task<IEnumerable<Ticket>> GetUserTicketsAsync(Guid userId);
    Task<IEnumerable<Ticket>> GetDepartmentTicketsAsync(string department);
    Task<Ticket> CreateTicketAsync(Ticket ticket);
    Task UpdateTicketAsync(Ticket ticket);
    Task DeleteTicketAsync(Guid id);
    Task<bool> AssignTicketAsync(Guid ticketId, Guid assignedToId);
    Task<bool> CloseTicketAsync(Guid ticketId, string resolution);
    Task<bool> ReopenTicketAsync(Guid ticketId, string reason);
    Task<IEnumerable<Ticket>> SearchTicketsAsync(string searchTerm);
    Task<bool> AddCommentAsync(Guid ticketId, TicketComment comment);
    Task<bool> AddAttachmentAsync(Guid ticketId, TicketAttachment attachment);
}