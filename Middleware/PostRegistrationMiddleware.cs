using CoolVolleyBallBookingSystem.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoolVolleyBallBookingSystem.Middleware
{
    public class PostRegistrationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHubContext<BookingHub> _hubContext;
        private readonly ILogger<PostRegistrationMiddleware> _logger;

        public PostRegistrationMiddleware(
            RequestDelegate next,
            IHubContext<BookingHub> hubContext,
            ILogger<PostRegistrationMiddleware> logger)
        {
            _next = next;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await _next(context);

                if (context.Request.Path.StartsWithSegments("/register") &&
                    context.Response.StatusCode == 200)
                {
                    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveAdminNotification",
                        "New user has registered!");
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
            }
            finally
            {
                // Ensure that the response body is set back to the original stream
                context.Response.Body = originalBodyStream;
            }
        }

    }


}
