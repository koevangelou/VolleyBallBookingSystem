using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CoolVolleyBallBookingSystem.Hubs
{
    public class CourtHub : Hub
    {
        private readonly ILogger<CourtHub> _logger;

        public CourtHub(ILogger<CourtHub> logger)
        {
            _logger = logger;
        }

        public async Task SendCourtNotification(string message)
        {
            _logger.LogInformation("Notification: " + message);
            await Clients.All.SendAsync("ReceiveCourtNotification", message);
        }

        public async Task SendCourtCreatedNotification(string message)
        {
            _logger.LogInformation("Court Created: " + message);
            await Clients.All.SendAsync("ReceiveCourtCreatedNotification", message);
        }

        public async Task SendCourtsRetrievedNotification(int count)
        {
            var message = $"Retrieved {count} courts.";
            _logger.LogInformation(message);
            await Clients.All.SendAsync("ReceiveCourtsRetrievedNotification", message);
        }
    }
}
