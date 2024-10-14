using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.dto;
using CoolVolleyBallBookingSystem.Models;
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
        public BookingController(AppDbContext dbContext,UserManager<User> userManager) {
            _dbContext = dbContext;
            _userManager = userManager;
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
        public async Task<IActionResult> createBooking(BookingRequestDto requestDto)
        {
            User user= await _userManager.FindByIdAsync(requestDto.UserID);


            if (user == null)
            {
                return BadRequest("User with Id not found");

            }

            Court court = await _dbContext.Courts.FindAsync(requestDto.CourtID);


            Booking booking = new Booking{
                CourtID=requestDto.CourtID,
                User =user,
                Court=court,
                UserID=requestDto.UserID,
                BookingDate=requestDto.BookingDate,
                StartTime =requestDto.StartTime,
                EndTime =requestDto.EndTime
            };
            try
            {

                var result = await _dbContext.Bookings.AddAsync(booking);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
