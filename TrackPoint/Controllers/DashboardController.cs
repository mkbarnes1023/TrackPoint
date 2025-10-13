using Microsoft.AspNetCore.Mvc;

namespace TrackPoint.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
