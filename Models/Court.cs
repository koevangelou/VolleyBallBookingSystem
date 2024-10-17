using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CoolVolleyBallBookingSystem.Models
{
    public class Court
    {
        public int CourtID { get; set; }
        public string CourtName { get; set; }
        public string Location { get; set; }
        public string CourtType { get; set; } = "Sand";
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; }

        // Add this navigation property for Trainings
        [JsonIgnore]
        public ICollection<Training> Trainings { get; set; }
    }
}
