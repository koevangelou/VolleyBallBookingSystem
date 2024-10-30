using Microsoft.AspNetCore.Authorization;
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


       // [Authorize(Roles ="Admin")]
        public async Task JoinAdminGroup()
        {
           
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        // Add this method to send notifications specifically to admins
        public async Task SendAdminNotification(string message)
        {
            await Clients.Group("Admins").SendAsync("ReceiveAdminNotification", message);
        }


        public async Task JoinBookingGroup(string bookingId)
        {
            
            await Groups.AddToGroupAsync(Context.ConnectionId, bookingId);
        }

        public async Task LeaveBookingGroup(string bookingId)
        {
            
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, bookingId);
        }

        
        public async Task SendBookingNotification(string bookingId, string message)
        {
            await Clients.Group(bookingId).SendAsync("ReceiveBookingNotification", message);
        }


    }
}