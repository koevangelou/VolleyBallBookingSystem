using CoolVolleyBallBookingSystem.Models;

namespace CoolVolleyBallBookingSystem.dto
{
    public class BookingRequestDto
    {
        public int CourtID { get; set; }

        public string UserMail { get; set; }

        public DateTime BookingDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public string [] Players { get; set; }  = [];
    }
}
