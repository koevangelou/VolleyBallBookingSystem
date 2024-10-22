using System;
using System.Text.Json.Serialization;

namespace CoolVolleyBallBookingSystem.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public string UserID { get; set; } // Foreign key to the User
        public int CourtID { get; set; } // Foreign key to the Court
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = "Pending"; // Default status set to "Booked"
        public bool isTraining { get; set; } = false; // Indicates if this booking is a training session
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties (optional)
        [JsonIgnore] // Prevents circular reference during serialization
        public User User { get; set; }
        [JsonIgnore] // Prevents circular reference during serialization
        public Court Court { get; set; }

        [JsonIgnore]
        public ICollection<BookingPlayer> BookingPlayers { get; set; }

    }
}
