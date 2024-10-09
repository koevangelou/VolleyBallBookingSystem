namespace CoolVolleyBallBookingSystem.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public string UserID { get; set; }
        public int CourtID { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = "Booked";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; }
        public Court Court { get; set; }

    }
}