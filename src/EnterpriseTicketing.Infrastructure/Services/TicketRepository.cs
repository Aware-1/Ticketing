using Microsoft.EntityFrameworkCore;
using EnterpriseTicketing.Application.Interfaces;
using EnterpriseTicketing.Domain.Entities;
using EnterpriseTicketing.Infrastructure.Persistence;

namespace EnterpriseTicketing.Infrastructure.Services;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.CreatedById == userId || t.AssignedToId == userId)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByDepartmentAsync(string department)
    {
        return await _dbSet
            .Where(t => t.AssignedTo != null && t.AssignedTo.Department == department)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(TicketStatus status)
    {
        return await _dbSet
            .Where(t => t.Status == status)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(TicketPriority priority)
    {
        return await _dbSet
            .Where(t => t.Priority == priority)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetUnassignedTicketsAsync()
    {
        return await _dbSet
            .Where(t => t.AssignedToId == null)
            .Include(t => t.CreatedBy)
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetOverdueTicketsAsync()
    {
        // اینجا منطق بررسی تیکت‌های تاخیری براساس SLA باید پیاده‌سازی شود
        return await _dbSet
            .Where(t => t.Status != TicketStatus.Resolved && t.Status != TicketStatus.Closed)
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}