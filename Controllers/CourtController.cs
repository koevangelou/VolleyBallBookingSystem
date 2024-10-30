using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.Models;
using CoolVolleyBallBookingSystem.dto;
using CoolVolleyBallBookingSystem.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CoolVolleyBallBookingSystem.Controllers
{
    //[Authorize(Roles = "Coach", "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IHubContext<CourtHub> _courtHubContext;

        public CourtController(AppDbContext dbContext, IHubContext<CourtHub> courtHubContext)
        {
            _dbContext = dbContext;
            _courtHubContext = courtHubContext;
        }

        // Get a court by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourtById(int id)
        {
            var court = await _dbContext.Courts.FindAsync(id);
            if (court == null)
            {
                return NotFound();
            }

            return Ok(court);
        }

        // Get a list of all courts
        [HttpGet("list")]
        public async Task<IActionResult> GetCourtsList()
        {
            var courtsList = await _dbContext.Courts.ToListAsync();

            // Notify clients about the retrieval
            await _courtHubContext.Clients.All.SendAsync("ReceiveCourtsRetrievedNotification", courtsList.Count);

            return Ok(courtsList);
        }

        // Create a new court
        [HttpPost("create")]
        public async Task<IActionResult> CreateCourt([FromBody] Courtdto courtDto)
        {
            if (courtDto == null)
            {
                return BadRequest("Invalid court data");
            }

            var court = new Court
            {
                CourtName = courtDto.CourtName,
                Location = courtDto.Location,
                CourtType = courtDto.CourtType ?? "GRASISI"
            };

            await _dbContext.Courts.AddAsync(court);
            await _dbContext.SaveChangesAsync();

            // Notify clients about the new court creation
            await _courtHubContext.Clients.All.SendAsync("ReceiveCourtCreatedNotification", $"Court '{court.CourtName}' at '{court.Location}' has been created.");

            return CreatedAtAction(nameof(GetCourtById), new { id = court.CourtID }, court);
        }

        // Update an existing court
        [Authorize(Roles = "Admin")] // Restricting access to Admin users
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourt(int id, [FromBody] Courtdto courtDto)
        {
            var court = await _dbContext.Courts.FindAsync(id);
            if (court == null)
            {
                return NotFound("Court not found.");
            }

            court.CourtName = courtDto.CourtName ?? court.CourtName;
            court.Location = courtDto.Location ?? court.Location;
            court.CourtType = courtDto.CourtType ?? court.CourtType;

            try
            {
                await _dbContext.SaveChangesAsync();

                // Notify clients about the court update
                await _courtHubContext.Clients.All.SendAsync("ReceiveCourtUpdatedNotification", $"Court '{court.CourtName}' updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourtExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Delete a court - Admin only
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourt(int id)
        {
            // Find the court by ID
            var court = await _dbContext.Courts.FindAsync(id);
            if (court == null)
            {
                return NotFound("Court not found.");
            }

            // Remove the court from the database
            _dbContext.Courts.Remove(court);
            await _dbContext.SaveChangesAsync();

            // Notify clients about the court deletion
            await _courtHubContext.Clients.All.SendAsync("ReceiveCourtDeletedNotification", $"Court '{court.CourtName}' at '{court.Location}' has been deleted.");

            // Return a 204 No Content status
            return NoContent();
        }

        // Helper method to check if a court exists
        private bool CourtExists(int id)
        {
            return _dbContext.Courts.Any(e => e.CourtID == id);
        }
    }
}
