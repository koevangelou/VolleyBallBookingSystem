using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoolVolleyBallBookingSystem.Models;
using CoolVolleyBallBookingSystem.Services;
using CoolVolleyBallBookingSystem.dto;

namespace CoolVolleyBallBookingSystem.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // Setup UserManager mock
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStoreMock.Object,
                null, null, null, null, null, null, null, null
            );

            // Setup HttpContextAccessor mock
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new Mock<HttpContext>().Object);

            _userService = new UserService(_mockUserManager.Object, _mockHttpContextAccessor.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "1", UserName = "user1", Email = "user1@example.com" },
                new User { Id = "2", UserName = "user2", Email = "user2@example.com" }
            };

            _mockUserManager.Setup(x => x.Users)
                .Returns(users.AsQueryable<User>);

            // Act
            List<User> result = await _userService.GetAllUsers();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("user1", result[0].UserName);
            Assert.Equal("user2", result[1].UserName);
        }

        [Fact]
        public async Task GetCurrentUser_UserExists_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "currentUser" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetCurrentUser();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("currentUser", result.UserName);
        }

        [Fact]
        public async Task GetCurrentUser_UserNotFound_ReturnsNull()
        {
            // Arrange
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetCurrentUser();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserById_ExistingUser_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "testUser" };
            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserById("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testUser", result.UserName);
        }

        [Fact]
        public async Task GetUserById_NonExistentUser_ReturnsNull()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserById("999");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByName_ExistingUser_ReturnsUser()
        {
            // Arrange
            var user = new User { UserName = "testUser" };
            _mockUserManager.Setup(x => x.FindByNameAsync("testUser"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByName("testUser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testUser", result.UserName);
        }

        [Fact]
        public async Task SetUserRole_ValidEmail_AddsRoles()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var roles = new[] { "Admin", "User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.AddToRolesAsync(user, roles))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.SetUserRole("test@example.com", roles);

            // Assert
            Assert.Contains("successfully added", result);
            _mockUserManager.Verify(x => x.AddToRolesAsync(user, roles), Times.Once);
        }

        [Fact]
        public async Task SetUserRole_UserNotFound_ThrowsException()
        {
            // Arrange
            var roles = new[] { "Admin" };
            _mockUserManager.Setup(x => x.FindByEmailAsync("nonexistent@example.com"))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _userService.SetUserRole("nonexistent@example.com", roles));
        }

        [Fact]
        public async Task RemoveUserRole_ValidEmail_RemovesRoles()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            var roles = new[] { "Admin", "User" };

            _mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.RemoveFromRolesAsync(user, roles))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.RemoveUserRole("test@example.com", roles);

            // Assert
            Assert.Contains("successfully removed", result);
            _mockUserManager.Verify(x => x.RemoveFromRolesAsync(user, roles), Times.Once);
        }

        [Fact]
        public async Task RemoveUserRole_UserNotFound_ThrowsException()
        {
            // Arrange
            var roles = new[] { "Admin" };
            _mockUserManager.Setup(x => x.FindByEmailAsync("nonexistent@example.com"))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _userService.RemoveUserRole("nonexistent@example.com", roles));
        }

        [Fact]
        public async Task RemoveUserByEmail_ValidEmail_DeletesUser()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act & Assert
            await _userService.RemoveUserByEmail("test@example.com");
            _mockUserManager.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task RemoveUserByEmail_UserNotFound_ThrowsException()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByEmailAsync("nonexistent@example.com"))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _userService.RemoveUserByEmail("nonexistent@example.com"));
        }

        [Fact]
        public async Task DeleteUserById_ValidId_DeletesUser()
        {
            // Arrange
            var user = new User { Id = "1" };
            _mockUserManager.Setup(x => x.FindByIdAsync("1"))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.DeleteUserById("1");

            // Assert
            Assert.True(result.Succeeded);
            _mockUserManager.Verify(x => x.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserById_UserNotFound_ThrowsException()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByIdAsync("999"))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _userService.DeleteUserById("999"));
        }

        [Fact]
        public async Task ChangeCurrentProfile_ValidData_UpdatesProfile()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "oldName", Email = "old@example.com" };
            var changeProfileDto = new ChangeProfileDto
            {
                userName = "newName",
                email = "new@example.com",
                phoneNumber = "1234567890"
            };

            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.ChangeCurrentProfile(changeProfileDto);

            // Assert
            Assert.Contains("updated successfully", result);
            Assert.Equal("newName", user.UserName);
            Assert.Equal("new@example.com", user.Email);
            Assert.Equal("1234567890", user.PhoneNumber);
        }

        [Fact]
        public async Task ChangeCurrentProfile_UpdateFails_ThrowsException()
        {
            // Arrange
            var user = new User { Id = "1" };
            var changeProfileDto = new ChangeProfileDto { userName = "newName" };

            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Setup UpdateAsync to throw an exception
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("Update failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _userService.ChangeCurrentProfile(changeProfileDto));

            Assert.Contains("There was an error with profile change", exception.Message);
        }

        [Fact]
        public async Task LogoutCurrentUser_Success()
        {
            // Arrange
            var user = new User { Id = "1" };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.RemoveAuthenticationTokenAsync(
                user,
                IdentityConstants.BearerScheme,
                "RefreshToken"))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.logoutCurentUser();

            // Assert
            Assert.Contains("Logged out successfully", result);
            _mockUserManager.Verify(x => x.RemoveAuthenticationTokenAsync(
                user,
                IdentityConstants.BearerScheme,
                "RefreshToken"),
                Times.Once);
        }

        [Fact]
        public async Task LogoutCurrentUser_UserNotFound_ThrowsException()
        {
            // Arrange
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _userService.logoutCurentUser());
        }

        [Fact]
        public async Task UpdateUser_ValidData_UpdatesUser()
        {
            // Arrange
            var userId = "1";
            var user = new User { Id = userId };
            var updatedUser = new User
            {
                Id = userId,
                Email = "new@example.com",
                PasswordHash = "newHash"
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.UpdateUser(userId, updatedUser);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(updatedUser.Email, user.Email);
            Assert.Equal(updatedUser.PasswordHash, user.PasswordHash);
        }

        [Fact]
        public async Task UpdateUser_IdMismatch_ThrowsException()
        {
            // Arrange
            var userId = "1";
            var updatedUser = new User { Id = "2" };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _userService.UpdateUser(userId, updatedUser));
        }

        [Fact]
        public async Task GetUserByEmail_ExistingEmail_ReturnsUser()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync("test@example.com"))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmail("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task IsInRole_UserInRole_ReturnsTrue()
        {
            // Arrange
            var user = new User { Id = "1" };
            _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(true);

            // Act
            var result = await _userService.IsInRole(user, "Admin");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsInRole_UserNotInRole_ReturnsFalse()
        {
            // Arrange
            var user = new User { Id = "1" };
            _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Admin"))
                .ReturnsAsync(false);

            // Act
            var result = await _userService.IsInRole(user, "Admin");

            // Assert
            Assert.False(result);
        }
    }
}