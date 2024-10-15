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

        // Add new method to retrieve all courts
        [HttpGet("list")]
        public async Task<IActionResult> GetCourtsList()
        {
            // Fetch all courts from the database
            var courtsList = await _dbContext.Courts.ToListAsync();

            // Return the list of courts
            return Ok(courtsList);
        }

        // Existing code for CreateCourt
        [HttpPost("create")]
        public async Task<IActionResult> CreateCourt([FromBody] Courtdto courtDto)
        {
            if (courtDto == null)
            {
                return BadRequest("Invalid court data");
            }

            // Create a new Court entity and map properties from the Courtdto
            var court = new Court
            {
                CourtName = courtDto.CourtName,
                Location = courtDto.Location,
                CourtType = courtDto.CourtType ?? "GRASISI"
            };

            // Add the new court to the database
            await _dbContext.Courts.AddAsync(court);

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();

            // Return the created court with a 201 Created status
            return CreatedAtAction(nameof(GetCourtById), new { id = court.CourtID }, court);
        }
    }
}
