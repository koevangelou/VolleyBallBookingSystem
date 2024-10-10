using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using CoolVolleyBallBookingSystem.Models;

namespace CoolVolleyBallBookingSystem.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        //public DbSet<User> Users { get; set; }
        public DbSet<Court> Courts { get; set; }    

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Tournament> Tournaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Organizer)
                .WithMany(u => u.Tournaments)
                .HasForeignKey(t => t.OrganizerID).OnDelete(DeleteBehavior.SetNull);

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



    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);

    //    modelBuilder.Entity<TodoItem>()
    //        .HasOne(t => t.User)
    //        .WithMany(u => u.TodoItems)
    //        .HasForeignKey(t => t.UserId)
    //        .OnDelete(DeleteBehavior.Cascade); // Set the delete behavior as needed



    //    base.OnModelCreating(modelBuilder);

        
    //}


    // DbSet for TodoItems - you already have Users through IdentityDbContext<User>
    //public DbSet<TodoItem> TodoItems { get; set; }




