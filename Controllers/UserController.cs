using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolVolleyBallBookingSystem.dto;
using CoolVolleyBallBookingSystem.Services;

namespace CoolVolleyBallBookingSystem.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {

        private readonly AppDbContext _dbContext;
        
        
        private readonly IUserService _userService;

        public UserController(AppDbContext dbContext,IUserService userService)
        {
            _dbContext = dbContext;
            
            
            _userService = userService;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllUserProfiles")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            return await _dbContext.Users.ToListAsync();

        }
        [HttpGet]
        [Route("GetCurrentUserProfile")]
        public async Task<ActionResult> GetCurrentUserProfile()
        {
            User user = await _userService.GetCurrentUser();
            if (user!= null) { 
            return Ok(user);
            }
            else
            {
                return BadRequest("Please log in");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userService.GetUserById(id);
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
            try
            {
                return Ok( await _userService.SetUserRole(request.UserMail, request.Roles));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("removeRole")]
        public async Task<ActionResult> RemoveUserRole([FromBody] SetUserRoleRequest request)
        {
            try
            {
                return Ok (await _userService.RemoveUserRole(request.UserMail, request.Roles));
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            try
            {
                return Ok(await _userService.UpdateUser(id, updatedUser));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                return Ok( _userService.DeleteUserById(id));
            }catch(Exception e)
            {
                return BadRequest(e.Message);            }
        }

        [HttpPut]
        [Route("ChangeProfile")]
        public async Task<IActionResult> ChangeProfile([FromBody]ChangeProfileDto changeProfileDto)
        {
            try
            {
                return Ok(await _userService.ChangeCurrentProfile(changeProfileDto));
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> logout()
        {
            try
            {
                return Ok(await _userService.logoutCurentUser());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Logout failed", message = ex.Message });
            }
        }

    }
    }
