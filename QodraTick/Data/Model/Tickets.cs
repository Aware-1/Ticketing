using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace Data.Model;
public enum TicketStatus
{
    Open = 0,           // باز (هنوز اکسپت نشده)
    InProgress = 1,     // در حال بررسی (اکسپت شده)
    Closed = 2,         // بسته شده
    Rejected = 3        // پس داده شده (نتونست جواب بده)
}

public enum TicketPriority
{
    Low = 0,        // کم
    Normal = 1,     // عادی  
    High = 2,       // بالا
    Critical = 3    // اورژانس
}

public enum TicketCategory
{
    Hardware = 0,   // سخت‌افزار
    Software = 1    // نرم‌افزار
}
[Table("Ticket")]
public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty; // Rich Text

    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public TicketPriority Priority { get; set; } = TicketPriority.Normal;

    public TicketCategory Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? AssignedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    // Foreign Keys
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public int? ClosedByUserId { get; set; }

    // Navigation Properties
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? AssignedToUser { get; set; }
    public virtual User? ClosedByUser { get; set; }
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}