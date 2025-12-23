namespace EnterpriseTicketing.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid CreatedById { get; set; }
    public virtual User CreatedBy { get; set; } = null!;
    public Guid? AssignedToId { get; set; }
    public virtual User? AssignedTo { get; set; }
    public virtual ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public virtual ICollection<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
}

public enum TicketPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum TicketStatus
{
    New = 0,
    Assigned = 1,
    InProgress = 2,
    OnHold = 3,
    Resolved = 4,
    Closed = 5,
    Reopened = 6
}