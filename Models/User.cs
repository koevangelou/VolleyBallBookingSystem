using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace CoolVolleyBallBookingSystem.Models
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Tournament>? Tournaments { get; set; }

        // Add this navigation property for Trainings
        public ICollection<Training>? Trainings { get; set; }  // Nullable ICollection
    }
}
