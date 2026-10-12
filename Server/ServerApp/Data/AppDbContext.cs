using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Attendant> Attendants => Set<Attendant>();
    public DbSet<ParkingSpot> ParkingSpots => Set<ParkingSpot>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Зв'язок Attendant -> ParkingSpots (1 : N, SET NULL)
        modelBuilder.Entity<ParkingSpot>()
            .HasOne(p => p.User)
            .WithMany(a => a.ParkingSpots)
            .HasForeignKey(p => p.UserID)
            .OnDelete(DeleteBehavior.SetNull);

        // Зв'язок Attendant -> Reservations (1 : N, CASCADE)
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(a => a.Reservations)
            .HasForeignKey(r => r.UserID)
            .OnDelete(DeleteBehavior.Cascade);

        // Зв'язок ParkingSpot -> Reservations (1 : N, CASCADE)
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Spot)
            .WithMany(p => p.Reservations)
            .HasForeignKey(r => r.SpotID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}