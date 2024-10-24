using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CoolVolleyBallBookingSystem.Hubs
{
    public class BookingHub : Hub
    {
        public async Task SendBookingCompletedNotification(string message)
        {
            // Broadcast a message to all connected clients
            await Clients.All.SendAsync("ReceiveBookingNotification", message);
        }
    }
}