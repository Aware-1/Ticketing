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

            // تنظیم Foreign Key Relationships - همه NoAction برای جلوگیری از Multiple Cascade Paths

            // User -> Role relationship (NoAction)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);

            // Ticket -> CreatedByUser (NoAction)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Ticket -> AssignedToUser (NoAction)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTickets)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Ticket -> ClosedByUser (NoAction)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.ClosedByUser)
                .WithMany()
                .HasForeignKey(t => t.ClosedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Message -> Ticket (Cascade)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Ticket)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Message -> User (NoAction)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Attachment -> Ticket (Cascade)
            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attachment -> UploadedByUser (NoAction)
            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.UploadedByUser)
                .WithMany()
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // TicketStatistic -> User (NoAction)
            modelBuilder.Entity<TicketStatistic>()
                .HasOne(ts => ts.User)
                .WithMany()
                .HasForeignKey(ts => ts.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // تاریخ ثابت برای Seeding (بدون Dynamic values)
            var seedDate = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "User", Description = "کاربر عادی", CreatedAt = seedDate },
                new Role { Id = 2, Name = "Support", Description = "پشتیبان", CreatedAt = seedDate },
                new Role { Id = 3, Name = "Admin", Description = "مدیر", CreatedAt = seedDate }
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                // Admins
                new User
                {
                    Id = 1,
                    Username = "admin1",
                    DisplayName = "مدیر اول",
                    Email = "admin1@company.com",
                    Password = "admin123",
                    RoleId = 3,
                    CreatedAt = seedDate,
                    IsActive = true
                },
                new User
                {
                    Id = 2,
                    Username = "admin2",
                    DisplayName = "مدیر دوم",
                    Email = "admin2@company.com",
                    Password = "admin123",
                    RoleId = 3,
                    CreatedAt = seedDate.AddMinutes(5),
                    IsActive = true
                },
                // Support Team
                new User
                {
                    Id = 3,
                    Username = "support1",
                    DisplayName = "پشتیبان اول",
                    Email = "support1@company.com",
                    Password = "support123",
                    RoleId = 2,
                    CreatedAt = seedDate.AddMinutes(10),
                    IsActive = true
                },
                new User
                {
                    Id = 4,
                    Username = "support2",
                    DisplayName = "پشتیبان دوم",
                    Email = "support2@company.com",
                    Password = "support123",
                    RoleId = 2,
                    CreatedAt = seedDate.AddMinutes(15),
                    IsActive = true
                },
                new User
                {
                    Id = 5,
                    Username = "support3",
                    DisplayName = "پشتیبان سوم",
                    Email = "support3@company.com",
                    Password = "support123",
                    RoleId = 2,
                    CreatedAt = seedDate.AddMinutes(20),
                    IsActive = true
                },
                new User
                {
                    Id = 6,
                    Username = "support4",
                    DisplayName = "پشتیبان چهارم",
                    Email = "support4@company.com",
                    Password = "support123",
                    RoleId = 2,
                    CreatedAt = seedDate.AddMinutes(25),
                    IsActive = true
                },
                // Regular Users
                new User
                {
                    Id = 7,
                    Username = "user1",
                    DisplayName = "کاربر تست اول",
                    Email = "user1@company.com",
                    Password = "user123",
                    RoleId = 1,
                    CreatedAt = seedDate.AddMinutes(30),
                    IsActive = true
                },
                new User
                {
                    Id = 8,
                    Username = "user2",
                    DisplayName = "کاربر تست دوم",
                    Email = "user2@company.com",
                    Password = "user123",
                    RoleId = 1,
                    CreatedAt = seedDate.AddMinutes(35),
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
                    CreatedByUserId = 7, // user1
                    CreatedAt = seedDate.AddDays(1),
                    LastActivityAt = seedDate.AddDays(1)
                },
                new Ticket
                {
                    Id = 2,
                    Subject = "خرابی پرینتر",
                    Description = "<p>پرینتر طبقه سوم کار نمی‌کند.</p>",
                    Priority = TicketPriority.High,
                    Category = TicketCategory.Hardware,
                    Status = TicketStatus.InProgress,
                    CreatedByUserId = 8, // user2
                    AssignedToUserId = 3, // support1
                    CreatedAt = seedDate.AddDays(1).AddHours(2),
                    AssignedAt = seedDate.AddDays(1).AddHours(3),
                    LastActivityAt = seedDate.AddDays(1).AddHours(4)
                },
                new Ticket
                {
                    Id = 3,
                    Subject = "درخواست بروزرسانی سیستم عامل",
                    Description = "<p>نیاز به بروزرسانی سیستم عامل ویندوز داریم.</p>",
                    Priority = TicketPriority.Low,
                    Category = TicketCategory.Software,
                    Status = TicketStatus.Open,
                    CreatedByUserId = 7, // user1
                    CreatedAt = seedDate.AddDays(1).AddHours(6),
                    LastActivityAt = seedDate.AddDays(1).AddHours(6)
                }
            );
        }
    }
}
