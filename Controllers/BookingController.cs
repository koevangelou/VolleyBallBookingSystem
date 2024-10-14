using Microsoft.AspNetCore.Mvc;

namespace CoolVolleyBallBookingSystem.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
