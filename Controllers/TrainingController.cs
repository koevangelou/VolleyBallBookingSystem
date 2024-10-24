using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.Models;
using CoolVolleyBallBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoolVolleyBallBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly BookingService _bookingService;

        public TrainingController(AppDbContext dbContext, IUserService userService, BookingService bookingService)
        {
            _dbContext = dbContext;
            _userService = userService;
            _bookingService = bookingService;
        }

        // Method to assign a user to training on a specific court (accessible to Coaches only)
        [Authorize(Roles = "Coach")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignUserToTraining([FromBody] AssignTrainingDto assignTrainingDto)
        {
            // Ensure the Court exists
            var court = await _dbContext.Courts.FindAsync(assignTrainingDto.CourtID);
            if (court == null)
            {
                return NotFound("Court not found.");
            }

            // Ensure the User exists
            var user = await _userService.GetUserById(assignTrainingDto.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Calculate start and end time (assuming training lasts 1 hour)
            var startTime = assignTrainingDto.TrainingDate.TimeOfDay;
            var endTime = startTime.Add(TimeSpan.FromHours(1));

            // Check for booking conflicts
            bool isConflict = await _bookingService.IsBookingConflict(assignTrainingDto.CourtID, assignTrainingDto.TrainingDate.Date, startTime, endTime);
            if (isConflict)
            {
                return BadRequest("The selected time slot is already booked for this court.");
            }

            // Create a new Training session
            var training = new Training
            {
                CourtID = assignTrainingDto.CourtID,
                UserId = assignTrainingDto.UserId,
                TrainingDate = assignTrainingDto.TrainingDate
            };

            // Save the training to the database
            await _dbContext.Trainings.AddAsync(training);

            // Create a booking entry for the training session
            var booking = new Booking
            {
                UserID = user.Id,
                CourtID = court.CourtID,
                BookingDate = assignTrainingDto.TrainingDate.Date,
                StartTime = startTime,
                EndTime = endTime,
                Status = "Confirmed",
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

        // Method: Retrieve all trainings (Accessible to all users)
        [Authorize] // All authenticated users can access
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTrainings()
        {
            var trainings = await _dbContext.Trainings.ToListAsync();
            return Ok(trainings);
        }

        // Method: Retrieve a specific training by ID (Accessible to all users)
        [Authorize] // All authenticated users can access
        [HttpGet("{trainingId}")]
        public async Task<IActionResult> GetTrainingById(int trainingId)
        {
            var training = await _dbContext.Trainings.FindAsync(trainingId);
            if (training == null)
            {
                return NotFound("Training not found.");
            }

            return Ok(training);
        }

        // Method: Delete a training and its associated booking (Accessible only to Admins)
        [Authorize(Roles = "Admin")] // Only admins can delete
        [HttpDelete("{trainingId}")]
        public async Task<IActionResult> DeleteTraining(int trainingId)
        {
            var training = await _dbContext.Trainings.FindAsync(trainingId);
            if (training == null)
            {
                return NotFound("Training not found.");
            }

            // Find the associated booking
            var booking = await _dbContext.Bookings
                .FirstOrDefaultAsync(b => b.CourtID == training.CourtID && b.UserID == training.UserId && b.BookingDate == training.TrainingDate.Date);

            // If a booking exists, delete it
            if (booking != null)
            {
                _dbContext.Bookings.Remove(booking);
            }

            // Delete the training itself
            _dbContext.Trainings.Remove(training);
            await _dbContext.SaveChangesAsync();

            return Ok("Training and associated booking (if any) have been successfully deleted.");
        }

        // Method: Update an existing training session
        [Authorize(Roles = "Coach")] // Only coaches can update
        [HttpPut("update/{trainingId}")]
        public async Task<IActionResult> UpdateTraining(int trainingId, [FromBody] UpdateTrainingDto updateTrainingDto)
        {
            var training = await _dbContext.Trainings.FindAsync(trainingId);
            if (training == null)
            {
                return NotFound("Training not found.");
            }

            // Update the training details
            training.CourtID = updateTrainingDto.CourtID;
            training.TrainingDate = updateTrainingDto.TrainingDate;

            // Update the associated booking, if any
            var booking = await _dbContext.Bookings
                .FirstOrDefaultAsync(b => b.CourtID == training.CourtID && b.UserID == training.UserId && b.BookingDate == training.TrainingDate.Date);
            if (booking != null)
            {
                booking.CourtID = updateTrainingDto.CourtID;
                booking.BookingDate = updateTrainingDto.TrainingDate.Date;
                booking.StartTime = updateTrainingDto.TrainingDate.TimeOfDay;
                booking.EndTime = booking.StartTime.Add(TimeSpan.FromHours(1));
                booking.UpdatedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            return Ok("Training session has been successfully updated.");
        }
    }

    // DTO class for updating a training session
    public class UpdateTrainingDto
    {
        public int CourtID { get; set; }
        public DateTime TrainingDate { get; set; }
    }

    // DTO class for assigning a user to training
    public class AssignTrainingDto
    {
        public int CourtID { get; set; }
        public string UserId { get; set; }
        public DateTime TrainingDate { get; set; }
    }
}
