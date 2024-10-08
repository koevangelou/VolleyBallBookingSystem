using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoolVolleyBallBookingSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        private readonly AppDbContext _dbContext;
        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _dbContext.Users.ToListAsync();

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);

        }


        [HttpPost]
        public async Task<ActionResult> CreateUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = user.UserID }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserID)
            {
                return BadRequest("User ID mismatch");
            }

            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }


            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.PasswordHash = updatedUser.PasswordHash;
            user.Role = updatedUser.Role;
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Users.Any(u => u.UserID == id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

    }
    }
