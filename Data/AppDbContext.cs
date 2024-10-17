using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using CoolVolleyBallBookingSystem.Models;
using System;
using System.Collections.Generic;

namespace CoolVolleyBallBookingSystem.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        // DbSet for courts
        public DbSet<Court> Courts { get; set; }

        // DbSet for bookings
        public DbSet<Booking> Bookings { get; set; }

        // DbSet for tournaments
        public DbSet<Tournament> Tournaments { get; set; }

        // New: DbSet for trainings
        public DbSet<Training> Trainings { get; set; }  // Add this line for Training sessions

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationships for Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Court)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CourtID)
                .OnDelete(DeleteBehavior.Cascade);

            // Define relationships for Tournament
            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Organizer)
                .WithMany(u => u.Tournaments)
                .HasForeignKey(t => t.OrganizerID)
                .OnDelete(DeleteBehavior.SetNull);

            // Define relationships for Training
            modelBuilder.Entity<Training>()
                .HasOne(t => t.Court)
                .WithMany(c => c.Trainings)
                .HasForeignKey(t => t.CourtID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Training>()
                .HasOne(t => t.User)
                .WithMany(u => u.Trainings)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed roles (Player, Coach, Admin)
            var readerRoleId = Guid.NewGuid().ToString();
            var writerRoleId = Guid.NewGuid().ToString();
            var adminRoleId = Guid.NewGuid().ToString();

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Player",
                    NormalizedName = "Player".ToUpper()
                },
                new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp = writerRoleId,
                    Name = "Coach",
                    NormalizedName = "Coach".ToUpper()
                },
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
