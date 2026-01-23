using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
    [Authorize(Roles = "Admin")] // admin-only section
    public class AdminController : Controller
    {
        public IActionResult AllocateDemo() => View();

        [HttpPost]
        public IActionResult Allocate(string assetTag, string issuedTo)
        {
            // Admin allocation logic
            TempData["Failure"] = null;
            return RedirectToAction("AllocateDemo");
        }
    }
}
