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

        public BookingService(AppDbContext dbContext,UserManager<User> userManager)
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

            if (requestDto.Players.Length > 3)
            {
                throw new Exception("Max players 3 ");
            }

            User[] players = []; 
            foreach (var item in requestDto.Players)
            {
                if (await _userManager.FindByIdAsync(item.ToString()) != null)
                {
                    players.Append(await _userManager.FindByIdAsync(item.ToString()));
                }
                
                
            }

            Booking booking = new Booking
            {
                CourtID = requestDto.CourtID,
                User = user,
                Court = court,
                UserID = requestDto.UserID,
                BookingDate = requestDto.BookingDate,
                StartTime = requestDto.StartTime,
                EndTime = requestDto.StartTime.Add(TimeSpan.FromHours(1)),
                Players= players
            };

            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();

            return booking;
        }
    }
}
