using System.Text.Json.Serialization;

namespace CoolVolleyBallBookingSystem.Models
{
    public class Training
    {
        public int TrainingId { get; set; }
        public int CourtID { get; set; }  // Court for the training
        public string UserId { get; set; }  // User assigned to the training
        public DateTime TrainingDate { get; set; }  // Training date

        // Navigation properties (optional)
        public Court Court { get; set; }
        public User User { get; set; }
    }
}
