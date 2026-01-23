using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrackPoint.Controllers
{
    [Authorize(Roles = "Admin,Borrower")] // both roles can access controller by default
    public class DashboardController : Controller
    {
        // Borrower + Admin
        public IActionResult Index() => View();

        // Admin-only view
        [Authorize(Roles = "Admin")]
        public IActionResult adminDashboard() => View();
    }
}
