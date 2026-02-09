using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;
using TrackPoint.Models.DTOs;

namespace TrackPoint.Controllers
{
    [Authorize(Roles = "Admin,Borrower")] // both roles can access controller by default
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Admin-only dashboard
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var statusCounts = await _context.Asset
                .GroupBy(a => a.AssetStatus)
                .Select(g => new CountByLabelDto
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Count assets that are not assigned to any user (null or empty IssuedToUserId)
            var unassignedCount = await _context.Asset
                .CountAsync(a => string.IsNullOrEmpty(a.IssuedToUserId));

            var viewModel = new AdminDashboardViewModel
            {
                StatusCounts = statusCounts,
                UnassignedCount = unassignedCount
            };

            return View(viewModel);
        }

        // Borrower + Admin
        public IActionResult Index() => View();

        
    }
}
