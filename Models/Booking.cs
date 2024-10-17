using System.Text.Json.Serialization;

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

        public bool isTraining { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Court Court { get; set; }

    }
}