using Azure.Core;
using CoolVolleyBallBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoolVolleyBallBookingSystem.Services
{
    public class UserService : IUserService
    {
        private UserManager<User> _userManager;
        private HttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<User> userManager, HttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _userManager.Users.ToListAsync();
        }
        public async Task<User> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }

        public async Task<User> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User> GetUserByName(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async Task<string> SetUserRole(string email, string[] roles)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user != null)
            {
                // Add roles to the user
                var result = await _userManager.AddToRolesAsync(user, roles);

                // Check if the operation was successful
                if (result.Succeeded)
                {
                    return ($"Roles: {string.Join(", ", roles)} were successfully added to {email}");
                }
                else
                {
                    // If it failed, return the errors
                    return (result.Errors.ToString());
                }
            }

            // Return error if user is not found
            return ($"User with email {email} not found");
        }
        public async Task<string> RemoveUserRole(string email, string[] roles)
        {
            User user = await _userManager.FindByEmailAsync(email);

            // Check if the user exists
            if (user != null)
            {
                // Remove roles to the user
                var result = await _userManager.RemoveFromRolesAsync(user, roles);

                // Check if the operation was successful
                if (result.Succeeded)
                {
                    return ($"Roles: {string.Join(", ", roles)} were successfully removed from {email}");
                }
                else
                {
                    // If it failed, return the errors
                    throw new Exception(result.Errors.ToString());
                }
            }

            // Return error if user is not found
            throw new Exception($"User with email {email} not found");
        }
        public async Task RemoveUserByEmail(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            await _userManager.DeleteAsync(user);
        }
        public async Task DeleteUserById(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            await _userManager.DeleteAsync(user);
        }
    }
}
