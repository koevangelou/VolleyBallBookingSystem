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
    public class TournamentController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        
        private readonly IUserService _userService;

        public TournamentController(AppDbContext dbContext,  IUserService userService)
        {
            _dbContext = dbContext;
            
            _userService = userService;
        }

        // Method: Create a new Tournament 
        [Authorize(Roles = "Admin,Coach")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTournament([FromBody] TournamentDto tournamentDto)
        {
            // Ensure the Organizer exists
            var organizer = await _userService.GetUserById(tournamentDto.OrganizerID);
            if (organizer == null)
            {
                return BadRequest("Coachnot found.");
            }

            // Create a new Tournament
            var tournament = new Tournament
            {
                TournamentName = tournamentDto.TournamentName,
                OrganizerID = tournamentDto.OrganizerID,
                StartDate = tournamentDto.StartDate,
                EndDate = tournamentDto.EndDate,
                Location = tournamentDto.Location,
                MaxTeams = tournamentDto.MaxTeams,
                Status = "Upcoming",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save the tournament to the database
            await _dbContext.Tournaments.AddAsync(tournament);
            await _dbContext.SaveChangesAsync();

            return Ok($"Tournament '{tournament.TournamentName}' has been successfully created.");
        }

        // Method: Retrieve all tournaments (Accessible to all users)
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllTournaments()
        {
            var tournaments = await _dbContext.Tournaments.ToListAsync();
            return Ok(tournaments);
        }

        // Method: Retrieve a specific tournament by ID (Accessible to all users)
        [Authorize]
        [HttpGet("{tournamentId}")]
        public async Task<IActionResult> GetTournamentById(int tournamentId)
        {
            var tournament = await _dbContext.Tournaments.FindAsync(tournamentId);
            if (tournament == null)
            {
                return NotFound("Tournament not found.");
            }

            return Ok(tournament);
        }

        // Method: Update a tournament 
        [Authorize(Roles = "Admin,Coach")]
        [HttpPut("update/{tournamentId}")]
        public async Task<IActionResult> UpdateTournament(int tournamentId, [FromBody] UpdateTournamentDto updateTournamentDto)
        {
            var tournament = await _dbContext.Tournaments.FindAsync(tournamentId);
            if (tournament == null)
            {
                return NotFound("Tournament not found.");
            }

            // Update the tournament details
            tournament.TournamentName = updateTournamentDto.TournamentName;
            tournament.StartDate = updateTournamentDto.StartDate;
            tournament.EndDate = updateTournamentDto.EndDate;
            tournament.Location = updateTournamentDto.Location;
            tournament.MaxTeams = updateTournamentDto.MaxTeams;
            tournament.Status = updateTournamentDto.Status;
            tournament.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return Ok($"Tournament '{tournament.TournamentName}' has been successfully updated.");
        }

        // Method: Delete a tournament (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{tournamentId}")]
        public async Task<IActionResult> DeleteTournament(int tournamentId)
        {
            var tournament = await _dbContext.Tournaments.FindAsync(tournamentId);
            if (tournament == null)
            {
                return NotFound("Tournament not found.");
            }

            _dbContext.Tournaments.Remove(tournament);
            await _dbContext.SaveChangesAsync();

            return Ok("Tournament has been successfully deleted.");
        }
    }

    // DTO class for creating a tournament
    public class TournamentDto
    {
        public string TournamentName { get; set; }
        public string OrganizerID { get; set; } // ID of the organizer (user)
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int MaxTeams { get; set; }
    }

    // DTO class for updating a tournament
    public class UpdateTournamentDto
    {
        public string TournamentName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public int MaxTeams { get; set; }
        public string Status { get; set; } = "Upcoming";
    }
}
