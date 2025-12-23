using EnterpriseTicketing.Domain.Entities;

namespace EnterpriseTicketing.Application.Interfaces;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<IEnumerable<Ticket>> GetTicketsByUserAsync(Guid userId);
    Task<IEnumerable<Ticket>> GetTicketsByDepartmentAsync(string department);
    Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus status);
    Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(TicketPriority priority);
    Task<IEnumerable<Ticket>> GetUnassignedTicketsAsync();
    Task<IEnumerable<Ticket>> GetOverdueTicketsAsync();
}