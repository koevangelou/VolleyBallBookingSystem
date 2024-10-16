using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolVolleyBallBookingSystem.dto;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CoolVolleyBallBookingSystem.Controllers
{
    //[Authorize(Roles = "Coach","Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CourtController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Existing code for GetCourtById
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

        // Existing code to get a list of all courts
        [HttpGet("list")]
        public async Task<IActionResult> GetCourtsList()
        {
            var courtsList = await _dbContext.Courts.ToListAsync();
            return Ok(courtsList);
        }

        // Existing code to create a new court
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

            return CreatedAtAction(nameof(GetCourtById), new { id = court.CourtID }, court);
        }

        // New method to update an existing court
        [Authorize(Roles = "Admin")] // Restricting access to Admins
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourt(int id, [FromBody] Courtdto courtDto)
        {
            // Find the court by ID
            var court = await _dbContext.Courts.FindAsync(id);
            if (court == null)
            {
                return NotFound("Court not found.");
            }

            // Update court properties from the dto
            court.CourtName = courtDto.CourtName ?? court.CourtName;
            court.Location = courtDto.Location ?? court.Location;
            court.CourtType = courtDto.CourtType ?? court.CourtType;

            // Save changes to the database
            try
            {
                await _dbContext.SaveChangesAsync();
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

        private bool CourtExists(int id)
        {
            return _dbContext.Courts.Any(e => e.CourtID == id);
        }
    }
}
