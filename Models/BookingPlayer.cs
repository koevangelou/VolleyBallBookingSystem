namespace CoolVolleyBallBookingSystem.Models
{
    public class BookingPlayer
    {
        public int BookingId { get; set; }
        public string UserId { get; set; }

        public Booking Booking { get; set; }
        public User User { get; set; }
    }
}
