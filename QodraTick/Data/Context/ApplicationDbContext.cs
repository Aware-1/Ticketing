using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<TicketStatistic> TicketStatistics { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();

                entity.Property(e => e.Role)
                    .HasConversion<int>();
            });

            // Ticket Configuration
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Status)
                    .HasConversion<int>();

                entity.Property(e => e.Priority)
                    .HasConversion<int>();

                entity.Property(e => e.Category)
                    .HasConversion<int>();

                // Relations
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(u => u.CreatedTickets)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssignedToUser)
                    .WithMany(u => u.AssignedTickets)
                    .HasForeignKey(e => e.AssignedToUserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.ClosedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.ClosedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Message Configuration
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Ticket)
                    .WithMany(t => t.Messages)
                    .HasForeignKey(e => e.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Attachment Configuration
            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Ticket)
                    .WithMany(t => t.Attachments)
                    .HasForeignKey(e => e.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
                //Error
               /* entity.HasOne(e => e.Message)
                    .WithMany(m => m.Attachments)
                    .HasForeignKey(e => e.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);*/

                entity.HasOne(e => e.UploadedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.UploadedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TicketStatistic Configuration
            modelBuilder.Entity<TicketStatistic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
