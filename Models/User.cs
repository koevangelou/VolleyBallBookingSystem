using Microsoft.AspNetCore.Identity;

namespace CoolVolleyBallBookingSystem.Models
{
    public class User : IdentityUser
    {
        //public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Tournament>? Tournaments { get; set; }
    }
}
