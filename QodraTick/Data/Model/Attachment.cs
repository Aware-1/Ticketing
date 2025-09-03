using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    [Table("Attachment")]
    public class Attachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys - فقط برای تیکت
        public int TicketId { get; set; }
        public int UploadedByUserId { get; set; }

        // Navigation Properties
        public virtual Ticket Ticket { get; set; } = null!;
        public virtual User UploadedByUser { get; set; } = null!;
    }
}
