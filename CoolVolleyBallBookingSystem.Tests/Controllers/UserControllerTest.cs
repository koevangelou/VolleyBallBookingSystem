using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoolVolleyBallBookingSystem.Controllers;
using CoolVolleyBallBookingSystem.Data;
using CoolVolleyBallBookingSystem.Models;
using CoolVolleyBallBookingSystem.Services;
using CoolVolleyBallBookingSystem.dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoolVolleyBallBookingSystem.Tests.Controllers
{
    public class UserControllerTests
    {
        
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            
            _mockUserService = new Mock<IUserService>();
            _userController = new UserController( _mockUserService.Object);
        }

        [Fact]
        public async Task GetCurrentUserProfile_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var testUser = new User { Id = "1", UserName = "testuser" };
            _mockUserService
                .Setup(x => x.GetCurrentUser())
                .ReturnsAsync(testUser);

            // Act
            var result = await _userController.GetCurrentUserProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(testUser.Id, returnedUser.Id);
        }

        [Fact]
        public async Task GetCurrentUserProfile_NoUser_ReturnsBadRequest()
        {
            // Arrange
            _mockUserService
                .Setup(x => x.GetCurrentUser())
                .ReturnsAsync((User)null);

            // Act
            var result = await _userController.GetCurrentUserProfile();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Please log in", badRequestResult.Value);
        }

        [Fact]
        public async Task SetUserRole_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var request = new SetUserRoleRequest
            {
                UserMail = "test@example.com",
                Roles = new[] { "Admin" }
            };
            _mockUserService
                .Setup(x => x.SetUserRole(request.UserMail, request.Roles))
                .ReturnsAsync("Roles added successfully");

            // Act
            var result = await _userController.SetUserRole(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Roles added successfully", okResult.Value);
        }

        [Fact]
        public async Task SetUserRole_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            var request = new SetUserRoleRequest
            {
                UserMail = "test@example.com",
                Roles = new[] { "Admin" }
            };
            _mockUserService
                .Setup(x => x.SetUserRole(request.UserMail, request.Roles))
                .ThrowsAsync(new Exception("User not found"));

            // Act
            var result = await _userController.SetUserRole(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User not found", badRequestResult.Value);
        }

        [Fact]
        public async Task RemoveUserRole_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var request = new SetUserRoleRequest
            {
                UserMail = "test@example.com",
                Roles = new[] { "Admin" }
            };
            _mockUserService
                .Setup(x => x.RemoveUserRole(request.UserMail, request.Roles))
                .ReturnsAsync("Roles removed successfully");

            // Act
            var result = await _userController.RemoveUserRole(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Roles removed successfully", okResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var userId = "1";
            var userToUpdate = new User
            {
                Id = userId,
                Email = "updated@example.com"
            };
            _mockUserService
                .Setup(x => x.UpdateUser(userId, userToUpdate))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userController.UpdateUser(userId, userToUpdate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(IdentityResult.Success, okResult.Value);
        }

        [Fact]
        public async Task DeleteUser_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var userId = "1";
            _mockUserService
                .Setup(x => x.DeleteUserById(userId))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userController.DeleteUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ChangeProfile_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var changeProfileDto = new ChangeProfileDto
            {
                userName = "newusername",
                email = "new@example.com",
                phoneNumber = "1234567890"
            };
            _mockUserService
                .Setup(x => x.ChangeCurrentProfile(changeProfileDto))
                .ReturnsAsync("Profile updated successfully");

            // Act
            var result = await _userController.ChangeProfile(changeProfileDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Profile updated successfully", okResult.Value);
        }

        [Fact]
        public async Task Logout_ValidInput_ReturnsOkResult()
        {
            // Arrange
            _mockUserService
                .Setup(x => x.logoutCurentUser())
                .ReturnsAsync("Logged out successfully");

            // Act
            var result = await _userController.logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Logged out successfully", okResult.Value);
        }
    }

   
    
}