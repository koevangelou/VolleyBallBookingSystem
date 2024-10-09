namespace CoolVolleyBallBookingSystem.Models
{
    public class Tournament
    {
        public int TournamentID { get; set; }
        public string TournamentName { get; set; }
        public int? OrganizerID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int MaxTeams { get; set; }
        public string Participants { get; set; }  // JSON string
        public string Status { get; set; } = "Upcoming";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User Organizer { get; set; }
    }
}