using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.Models;
using CoolVolleyBallBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CoolVolleyBallBookingSystem.Controllers
{
    [Authorize(Roles = "Coach")]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly BookingService _bookingService;

        public TrainingController(AppDbContext dbContext, UserManager<User> userManager, BookingService bookingService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _bookingService = bookingService;
        }

        // Method to assign a user to training on a specific court
        [HttpPost("assign")]
        public async Task<IActionResult> AssignUserToTraining([FromBody] AssignTrainingDto assignTrainingDto)
        {
            // Ensure the Court exists
            var court = await _dbContext.Courts.FindAsync(assignTrainingDto.CourtID); // Updated here
            if (court == null)
            {
                return NotFound("Court not found.");
            }

            // Ensure the User exists
            var user = await _userManager.FindByIdAsync(assignTrainingDto.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Calculate start and end time (assuming training lasts 1 hour)
            var startTime = assignTrainingDto.TrainingDate.TimeOfDay; // Getting the time part of the TrainingDate
            var endTime = startTime.Add(TimeSpan.FromHours(1)); // Assuming 1-hour training sessions

            // Check for booking conflicts
            bool isConflict = await _bookingService.IsBookingConflict(assignTrainingDto.CourtID, assignTrainingDto.TrainingDate.Date, startTime, endTime); // Updated here
            if (isConflict)
            {
                return BadRequest("The selected time slot is already booked for this court.");
            }

            // Create a new Training session
            var training = new Training
            {
                CourtID = assignTrainingDto.CourtID, // Updated here
                UserId = assignTrainingDto.UserId,
                TrainingDate = assignTrainingDto.TrainingDate
            };

            // Save the training to the database
            await _dbContext.Trainings.AddAsync(training);

            // Create a booking entry for the training session
            var booking = new Booking
            {
                UserID = user.Id,
                CourtID = court.CourtID, // Updated here
                BookingDate = assignTrainingDto.TrainingDate.Date,
                StartTime = startTime,
                EndTime = endTime,
                Status = "Confirmed", // or other status based on your logic
                isTraining = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save the booking to the database
            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();

            // Return success response
            return Ok($"User {user.UserName} has been successfully assigned to training on court {court.CourtName}.");
        }
    }

    // DTO class for assigning a user to training
    public class AssignTrainingDto
    {
        public int CourtID { get; set; } // ID of the court (updated here)
        public string UserId { get; set; } // ID of the user
        public DateTime TrainingDate { get; set; } // Training date and time
    }
}
