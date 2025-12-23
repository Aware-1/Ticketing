namespace EnterpriseTicketing.Domain.Entities;

public class TicketAttachment
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public Guid TicketId { get; set; }
    public virtual Ticket Ticket { get; set; } = null!;
    public Guid UploadedById { get; set; }
    public virtual User UploadedBy { get; set; } = null!;
}