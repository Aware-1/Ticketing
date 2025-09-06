using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    [Table("Message")]

    public class Message
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsFromSupport { get; set; } // true = پشتیبان، false = کاربر

        // Foreign Keys
        public int TicketId { get; set; }
        public int UserId { get; set; }

        // Navigation Properties
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
