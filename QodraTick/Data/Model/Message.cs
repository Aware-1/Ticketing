using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    [Table("Messages")]
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; } = string.Empty; // HTML from TinyMCE

        public DateTime CreatedAt { get; set; }

        public bool IsFromSupport { get; set; } // true = پشتیبان، false = کاربر

        // Foreign Keys با DataAnnotation
        [ForeignKey(nameof(Ticket))]
        public int TicketId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        // Navigation Properties
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
