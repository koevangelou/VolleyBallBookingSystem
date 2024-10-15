using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.dto;
using CoolVolleyBallBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CoolVolleyBallBookingSystem.Services
{
    public class BookingService
    {
        private readonly AppDbContext _dbContext;

        public BookingService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
            Booking booking = new Booking
            {
                CourtID = requestDto.CourtID,
                User = user,
                Court = court,
                UserID = requestDto.UserID,
                BookingDate = requestDto.BookingDate,
                StartTime = requestDto.StartTime,
                EndTime = requestDto.StartTime.Add(TimeSpan.FromHours(1))
            };

            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();

            return booking;
        }
    }
}
