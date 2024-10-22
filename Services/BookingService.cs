using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.dto;
using CoolVolleyBallBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoolVolleyBallBookingSystem.Services
{
    public class BookingService
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public BookingService(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // Method to check for booking conflicts
        public async Task<bool> IsBookingConflict(int courtID, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            return await _dbContext.Bookings
                .AnyAsync(b => b.CourtID == courtID &&
                               b.BookingDate == bookingDate &&
                               ((startTime >= b.StartTime && startTime < b.EndTime) ||  // Starts within an existing booking
                                (endTime > b.StartTime && endTime <= b.EndTime) ||     // Ends within an existing booking
                                (startTime <= b.StartTime && endTime >= b.EndTime)));  // Encompasses an existing booking
        }

        // Method to create a new booking
        public async Task<Booking> CreateBooking(User user, Court court, BookingRequestDto requestDto)
        {
            // Check if too many players are being added
            if (requestDto.Players.Length != 3)
            {
                throw new Exception("Please add 3 players");
            }

            // Create a new booking
            Booking booking = new Booking
            {
                CourtID = requestDto.CourtID,
                User = user,
                Court = court,
                UserID = user.Id,
                BookingDate = requestDto.BookingDate,
                StartTime = requestDto.StartTime,
                EndTime = requestDto.StartTime.Add(TimeSpan.FromHours(1)) // Assuming 1-hour default booking
            };

            // Add the players
            var players = new List<BookingPlayer>();

            // Add the creator of the booking first
            players.Add(new BookingPlayer
            {
                UserId = user.Id,
                Booking = booking,
                BookingId = booking.BookingID
            });

            // Add other players from the request
            foreach (var playerMail in requestDto.Players)
            {
                var player = await _userManager.FindByEmailAsync(playerMail);
                if (player == null)
                {
                    throw new Exception($"Player with mail {playerMail} not found.");
                }

                if (players.Any(p => p.UserId == player.Id))
                {
                    throw new Exception($"Player with mail {playerMail} is already part of the booking.");
                }

                players.Add(new BookingPlayer
                {
                    UserId = player.Id,
                    Booking = booking,
                    BookingId = booking.BookingID
                });
            }

            booking.BookingPlayers = players;


            // Save the booking to the database
            booking.Status = "Completed"; // Mark the booking as completed
            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();

            return booking;
        }
    }
}
