using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model
{
    [Table("TicketStatistics")]
    public class TicketStatistic
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        [Index(IsUnique = true)]
        public int UserId { get; set; }

        public int TicketsResolved { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
    }
}
