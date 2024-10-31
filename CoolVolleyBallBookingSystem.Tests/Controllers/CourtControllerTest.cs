using CoolVolleyBallBookingSystem.Controllers;
using CoolVolleyBallBookingSystem.Models;
using CoolVolleyBallBookingSystem.dto;
using CoolVolleyBallBookingSystem.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using CoolVolleyBallBookingSystem.Data;

namespace CoolVolleyBallBookingSystem.Tests
{
    public class CourtControllerTests
    {
        private readonly Mock<IHubContext<CourtHub>> _hubContextMock;
        private readonly Mock<IHubClients> _hubClientsMock;
        private readonly Mock<IClientProxy> _clientProxyMock;
        private readonly CourtController _controller;
        
        // In-memory list to simulate data storage
        private readonly List<Court> _courts;
        private readonly DbContextOptions <AppDbContext> _options;
        public CourtControllerTests()
        {
            _hubContextMock = new Mock<IHubContext<CourtHub>>();
            _hubClientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();

            // Setting up Hub mock to send notifications
            _hubClientsMock.Setup(clients => clients.All).Returns(_clientProxyMock.Object);
            _hubContextMock.Setup(hub => hub.Clients).Returns(_hubClientsMock.Object);

            // Initialize in-memory list to simulate courts
            _courts = new List<Court>
            {
                new Court { CourtID = 1, CourtName = "Court 1", Location = "Location 1" },
                new Court { CourtID = 2, CourtName = "Court 2", Location = "Location 2" }
            };

            _controller = new CourtController(_hubContextMock.Object);
                _options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
        }


        [Fact]
        public async Task GetCourtById_ReturnsNotFound_WhenCourtDoesNotExist()
        {
            // Arrange
            int courtId = 3; // An ID that does not exist in the list

            // Act
            var result = await _controller.GetCourtById(courtId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCourtById_ReturnsOk_WhenCourtExists()
        {
            // Arrange
            int courtId = 1;

            // Act
            var result = await _controller.GetCourtById(courtId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(_courts.First(c => c.CourtID == courtId), okResult.Value);
        }

        [Fact]
        public async Task GetCourtsList_ReturnsListOfCourts()
        {
            // Act
            var result = await _controller.GetCourtsList();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(_courts, okResult.Value);
            _clientProxyMock.Verify(client => client.SendCoreAsync("ReceiveCourtsRetrievedNotification", It.Is<object[]>(o => (int)o[0] == _courts.Count), default), Times.Once);
        }

        [Fact]
        public async Task CreateCourt_ReturnsCreatedAtActionResult_WhenCourtIsCreated()
        {
            // Arrange
            var courtDto = new Courtdto { CourtName = "Court A", Location = "Location A" };
            var court = new Court { CourtID = 3, CourtName = courtDto.CourtName, Location = courtDto.Location };

            // Act
            var result = await _controller.CreateCourt(courtDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetCourtById", createdResult.ActionName);
            _clientProxyMock.Verify(client => client.SendCoreAsync("ReceiveCourtCreatedNotification", It.Is<object[]>(o => o[0].ToString().Contains(courtDto.CourtName)), default), Times.Once);
        }

        [Fact]
        public async Task UpdateCourt_ReturnsNoContent_WhenCourtIsUpdated()
        {
            // Arrange
            int courtId = 1;
            var courtDto = new Courtdto { CourtName = "Updated Court", Location = "Updated Location" };
            var court = _courts.First(c => c.CourtID == courtId);

            // Act
            var result = await _controller.UpdateCourt(courtId, courtDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _clientProxyMock.Verify(client => client.SendCoreAsync("ReceiveCourtUpdatedNotification", It.Is<object[]>(o => o[0].ToString().Contains(courtDto.CourtName)), default), Times.Once);
        }

        [Fact]
        public async Task DeleteCourt_ReturnsNoContent_WhenCourtIsDeleted()
        {
            // Arrange
            int courtId = 1;
            var court = _courts.First(c => c.CourtID == courtId);

            // Act
            var result = await _controller.DeleteCourt(courtId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _clientProxyMock.Verify(client => client.SendCoreAsync("ReceiveCourtDeletedNotification", It.Is<object[]>(o => o[0].ToString().Contains(court.CourtName)), default), Times.Once);
        }
    }
}
