using CoolVolleyBallBookingSystem.Models;

namespace CoolVolleyBallBookingSystem.Services
{
    public interface IUserService
    {
        Task DeleteUserById(string userId);
        Task<List<User>> GetAllUsers();
        Task<User> GetCurrentUser();
        Task<User> GetUserById(string userId);
        Task<User> GetUserByName(string name);
        Task RemoveUserByEmail(string email);
        Task<string> RemoveUserRole(string email, string[] roles);
        Task<string> SetUserRole(string email, string[] roles);
    }
}