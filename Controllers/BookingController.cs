using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.dto;
using CoolVolleyBallBookingSystem.Models;
using CoolVolleyBallBookingSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoolVolleyBallBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : Controller
    {

        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly BookingService _bookingService;
        public BookingController(AppDbContext dbContext, UserManager<User> userManager, BookingService bookingService) {
            _dbContext = dbContext;
            _userManager = userManager;
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetAllBookings()
        {
            return await _dbContext.Bookings.ToListAsync();

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _dbContext.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);

        }
        [HttpPost]
        public async Task<IActionResult> CreateBooking(BookingRequestDto requestDto)
        {
            User user = await _userManager.FindByIdAsync(requestDto.UserID);
            if (user == null)
            {
                return BadRequest("User with Id not found");
            }

            Court court = await _dbContext.Courts.FindAsync(requestDto.CourtID);
            if (court == null)
            {
                return BadRequest("Court not found");
            }

            // Use the service to check for booking conflicts
            bool isConflict = await _bookingService.IsBookingConflict(requestDto.CourtID, requestDto.BookingDate, requestDto.StartTime, requestDto.StartTime.Add(TimeSpan.FromHours(1)));
            if (isConflict)
            {
                return BadRequest("The selected time slot is already booked for this court.");
            }

            // Use the service to create the booking
            Booking booking = await _bookingService.CreateBooking(user, court, requestDto);

            return Ok("Booked successfully on " + requestDto.BookingDate.ToString("yyyy-MM-dd") +
                      " from " + requestDto.StartTime + " to " + requestDto.StartTime.Add(TimeSpan.FromHours(1)) +
                      " in " + court.CourtName);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateBooking(int id, BookingRequestDto requestDto)
        {
            Booking booking = await _dbContext.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound(new { Message = $"Booking with ID {id} not found" });
            }

            var user = await _userManager.FindByIdAsync(requestDto.UserID);

            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {requestDto.UserID} not found" });
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && booking.UserID != requestDto.UserID)
            {
                return Forbid("You can only update your own bookings.");
            }

            Court court = await _dbContext.Courts.FindAsync(requestDto.CourtID);
            if (court == null)
            {
                return NotFound(new { Message = $"Court with ID {requestDto.CourtID} not found" });
            }

            // Use the service to create the booking
            Booking bookingUpdated = await _bookingService.UpdateBooking(booking, requestDto);

            return Ok($"Booked changed succesfully on {requestDto.BookingDate.ToString("yyyy-MM-dd")} from {requestDto.StartTime} " +
                $"to { requestDto.StartTime.Add(TimeSpan.FromHours(1))} in { court.CourtName}");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id, string userId)
        {
            // Trova la prenotazione esistente
            Booking booking = await _dbContext.Bookings.FindAsync(id);
            
            if (booking == null)
            {
                return NotFound(new { Message = $"Booking with ID {id} not found" });
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { Message = $"User with ID {userId} not found" });
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && booking.UserID != userId)
            {
                return Forbid("You can only update your own bookings.");
            }

            // Remove the booking from the DB
            await _bookingService.DeleteBooking(booking);

            return Ok($"Booking removed succeffuly");
        }
    }
}
