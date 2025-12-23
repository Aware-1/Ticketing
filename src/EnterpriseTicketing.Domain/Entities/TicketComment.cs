namespace EnterpriseTicketing.Domain.Entities;

public class TicketComment
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid TicketId { get; set; }
    public virtual Ticket Ticket { get; set; } = null!;
    public Guid CreatedById { get; set; }
    public virtual User CreatedBy { get; set; } = null!;
    public bool IsInternal { get; set; }
}