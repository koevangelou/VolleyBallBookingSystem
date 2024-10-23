using Azure.Core;
using CoolVolleyBallBookingSystem.dto;
using CoolVolleyBallBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoolVolleyBallBookingSystem.Services
{
    public class UserService : IUserService
    {
        private UserManager<User> _userManager;
        private IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
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
                    throw new Exception (result.ToString());
                }
            }

            // Return error if user is not found
            throw new Exception ($"User with email {email} not found");
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
        public async Task<IdentityResult> DeleteUserById(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            return await _userManager.DeleteAsync(user);
        }

        public async Task<string> ChangeCurrentProfile(ChangeProfileDto changeProfileDto)
        {
            User user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (changeProfileDto.phoneNumber != null)
            {
                user.PhoneNumber = changeProfileDto.phoneNumber;
            }
            if (changeProfileDto.userName != null)
            {
                user.UserName = changeProfileDto.userName;
            }
            if (changeProfileDto.email != null)
            {
                user.Email = changeProfileDto.email;
            }
            try
            {
                var result = await _userManager.UpdateAsync(user);
                return ("Profile updated successfully");
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error with profile change" + ex.Message);
            }
        }

        public async Task<string> logoutCurentUser()
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Remove the refresh token
            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                IdentityConstants.BearerScheme,
                "RefreshToken"
            );

            return ( "Logged out successfully" );
        }
        public async Task<IdentityResult> UpdateUser(string id,User updatedUser)
        {
            if (id != updatedUser.Id)
            {
                throw new Exception("User ID mismatch");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new Exception ("User not found");
            }


            //user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.PasswordHash = updatedUser.PasswordHash;
            user.UpdatedAt = DateTime.UtcNow;

            
              return  await _userManager.UpdateAsync(user);
            
            
        }
    }
}
