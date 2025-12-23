namespace EnterpriseTicketing.Domain.Entities;

public class SLA
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public int ResponseTimeHours { get; set; }
    public int ResolutionTimeHours { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? Department { get; set; }
    public virtual ICollection<SLABreachNotification> BreachNotifications { get; set; } = new List<SLABreachNotification>();
}

public class SLABreachNotification
{
    public Guid Id { get; set; }
    public Guid SLAId { get; set; }
    public virtual SLA SLA { get; set; } = null!;
    public Guid TicketId { get; set; }
    public virtual Ticket Ticket { get; set; } = null!;
    public DateTime BreachTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
}