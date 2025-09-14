using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TicketStatistic> TicketStatistics { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "User", Description = "کاربر عادی", CreatedAt = DateTime.UtcNow },
                new Role { Id = 2, Name = "Support", Description = "پشتیبان", CreatedAt = DateTime.UtcNow },
                new Role { Id = 3, Name = "Admin", Description = "مدیر", CreatedAt = DateTime.UtcNow }
            );

            // Seed Users (بدون رمزنگاری فعلاً)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    DisplayName = "مدیر سیستم",
                    Email = "admin@company.com",
                    Password = "admin123", // Plain text فعلاً
                    RoleId = 3, // Admin
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "support1",
                    DisplayName = "پشتیبان اول",
                    Email = "support1@company.com",
                    Password = "support123",
                    RoleId = 2, // Support
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 3,
                    Username = "support2",
                    DisplayName = "پشتیبان دوم",
                    Email = "support2@company.com",
                    Password = "support123",
                    RoleId = 2, // Support
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 4,
                    Username = "user1",
                    DisplayName = "کاربر تست اول",
                    Email = "user1@company.com",
                    Password = "user123",
                    RoleId = 1, // User
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User
                {
                    Id = 5,
                    Username = "user2",
                    DisplayName = "کاربر تست دوم",
                    Email = "user2@company.com",
                    Password = "user123",
                    RoleId = 1, // User
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed Sample Tickets
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    Id = 1,
                    Subject = "مشکل نصب نرم‌افزار",
                    Description = "<p>نمی‌توانم نرم‌افزار جدید را نصب کنم. لطفاً کمک کنید.</p>",
                    Priority = TicketPriority.Normal,
                    Category = TicketCategory.Software,
                    Status = TicketStatus.Open,
                    CreatedByUserId = 4, // user1
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    LastActivityAt = DateTime.UtcNow.AddDays(-2)
                },
                new Ticket
                {
                    Id = 2,
                    Subject = "خرابی پرینتر",
                    Description = "<p>پرینتر طبقه سوم کار نمی‌کند.</p>",
                    Priority = TicketPriority.High,
                    Category = TicketCategory.Hardware,
                    Status = TicketStatus.InProgress,
                    CreatedByUserId = 5, // user2
                    AssignedToUserId = 2, // support1
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    AssignedAt = DateTime.UtcNow.AddHours(-2),
                    LastActivityAt = DateTime.UtcNow.AddHours(-1)
                }
            );
        }
    }
}
