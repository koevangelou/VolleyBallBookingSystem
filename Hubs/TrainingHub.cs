using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CoolVolleyBallBookingSystem.Hubs
{
    public class TrainingHub : Hub
    {
        // Method to broadcast a notification when a training is created
        public async Task SendTrainingCreatedNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveTrainingCreatedNotification", message);
        }
    }
}
