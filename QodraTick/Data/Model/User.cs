using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
   

    public enum UserRole
    {
        User = 0,        // کاربر عادی
        Support = 1,     // پشتیبان
        Admin = 2        // ادمین (گزارش‌گیر)
    }

    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        //[Index(IsUnique = true)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        //[Index(IsUnique = true)]
        //Index نداریم راهی داری بگو نداری حذف کنیم
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty; // بدون Hash فعلاً

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Foreign Key با DataAnnotation
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; } = 1; // Default User role

        // Navigation Properties
        public virtual Role Role { get; set; } = null!;

        [InverseProperty(nameof(Ticket.CreatedByUser))]
        public virtual ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();

        [InverseProperty(nameof(Ticket.AssignedToUser))]
        public virtual ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
