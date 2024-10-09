namespace CoolVolleyBallBookingSystem.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "Player";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Tournament>? Tournaments { get; set; }
    }
}
