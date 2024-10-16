﻿using CoolVolleyBallBookingSystem.Data;
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
        public BookingController(AppDbContext dbContext,UserManager<User> userManager, BookingService bookingService) {
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
    }
}
