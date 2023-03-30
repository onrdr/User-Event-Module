using Microsoft.EntityFrameworkCore;
using Models.Concrete.Entities;

namespace DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(user => user.CreatedEvents)
            .WithOne(evt => evt.Creator)
            .HasForeignKey(evt => evt.CreatorId);
         
        modelBuilder.Entity<Participant>()
            .HasKey(p => new { p.UserId, p.EventId });
        modelBuilder.Entity<Participant>()
            .HasOne(p => p.User)
            .WithMany(user => user.ParticipatedEvents)
            .HasForeignKey(p => p.UserId);
        modelBuilder.Entity<Participant>()
            .HasOne(p => p.Event)
            .WithMany(evt => evt.Participants)
            .HasForeignKey(p => p.EventId);
         
        modelBuilder.Entity<Invitation>()
        .HasOne(i => i.Inviter)
        .WithMany(u => u.InvitationsSent)
        .HasForeignKey(i => i.InviterId);

        modelBuilder.Entity<Invitation>()
            .HasOne(i => i.Invitee)
            .WithMany(u => u.InvitationsReceived)
            .HasForeignKey(i => i.InviteeId);
    }

    public DbSet<User> Users { get; set; }
    public virtual DbSet<Event> Events { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
}
