namespace CoolVolleyBallBookingSystem.dto
{
    public class BookingRequestDto
    {
        public int CourtID { get; set; }

        public string UserID { get; set; }

        public DateTime BookingDate { get; set; }

        public TimeSpan StartTime { get; set; }

    }
}
