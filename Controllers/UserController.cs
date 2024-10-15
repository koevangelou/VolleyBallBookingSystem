using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolVolleyBallBookingSystem.dto;

namespace CoolVolleyBallBookingSystem.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(AppDbContext dbContext, UserManager<User> userManager,IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            this.userManager = userManager;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _dbContext.Users.ToListAsync();

        }
        [HttpGet]
        [Route("GetCurrentUserProfile")]
        public async Task<ActionResult> GetCurrentUserProfile()
        {
            User user = await userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return Ok(user);
        }

        [Authorize(Roles ="Admin")]
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
        

        //[Authorize(Roles ="Admin")]
        [HttpPost]
        [Route("setRole")]
        public async Task<ActionResult> SetUserRole([FromBody] SetUserRoleRequest request)
        {
            // Find user by email
            User user = await userManager.FindByEmailAsync(request.UserMail);
            
            

            // Check if the user exists
            if (user != null)
            {
                // Add roles to the user
                var result = await userManager.AddToRolesAsync(user, request.Roles);

                // Check if the operation was successful
                if (result.Succeeded)
                {
                    return Ok($"Roles: {string.Join(", ", request.Roles)} were successfully added to {request.UserMail}");
                }
                else
                {
                    // If it failed, return the errors
                    return BadRequest(result.Errors);
                }
            }

            // Return error if user is not found
            return NotFound($"User with email {request.UserMail} not found");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("removeRole")]
        public async Task<ActionResult> RemoveUserRole([FromBody] SetUserRoleRequest request)
        {
            // Find user by email
            User user = await userManager.FindByEmailAsync(request.UserMail);
            
            // Check if the user exists
            if (user != null)
            {
                // Remove roles to the user
                var result = await userManager.RemoveFromRolesAsync(user, request.Roles);

                // Check if the operation was successful
                if (result.Succeeded)
                {
                    return Ok($"Roles: {string.Join(", ", request.Roles)} were successfully removed from {request.UserMail}");
                }
                else
                {
                    // If it failed, return the errors
                    return BadRequest(result.Errors);
                }
            }

            // Return error if user is not found
            return NotFound($"User with email {request.UserMail} not found");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest("User ID mismatch");
            }

            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }


            //user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.PasswordHash = updatedUser.PasswordHash;
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Users.Any(u => u.Id == id))
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
