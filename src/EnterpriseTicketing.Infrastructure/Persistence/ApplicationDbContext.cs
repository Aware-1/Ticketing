using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EnterpriseTicketing.Domain.Entities;

namespace EnterpriseTicketing.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<TicketAttachment> TicketAttachments => Set<TicketAttachment>();
    public DbSet<SLA> SLAs => Set<SLA>();
    public DbSet<SLABreachNotification> SLABreachNotifications => Set<SLABreachNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configurations
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.UserName).IsUnique();
            
            entity.HasMany(e => e.CreatedTickets)
                .WithOne(e => e.CreatedBy)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.AssignedTickets)
                .WithOne(e => e.AssignedTo)
                .HasForeignKey(e => e.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Ticket configurations
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.CreatedAt);
            
            entity.HasMany(e => e.Comments)
                .WithOne(e => e.Ticket)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Attachments)
                .WithOne(e => e.Ticket)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SLA configurations
        modelBuilder.Entity<SLA>(entity =>
        {
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.Department);
            
            entity.HasMany(e => e.BreachNotifications)
                .WithOne(e => e.SLA)
                .HasForeignKey(e => e.SLAId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}