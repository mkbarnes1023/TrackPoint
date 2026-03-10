using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;
using TrackPoint.Models;
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

            var limit = DateTime.Now.AddMonths(6);

            var expiringSoonCount = await _context.Asset
                .CountAsync(a => a.WarrantyExpirationDate.HasValue && a.WarrantyExpirationDate >= DateTime.Now && a.WarrantyExpirationDate <= limit);

            var today = DateTime.Today;

            var expiredCount = _context.Asset
                .Count(a => a.WarrantyExpirationDate != null
                            && a.WarrantyExpirationDate < today);

            var zeroToThirty = _context.Asset
                .Count(a => a.WarrantyExpirationDate != null
                            && a.WarrantyExpirationDate >= today
                            && a.WarrantyExpirationDate <= today.AddDays(30));

            var thirtyOneToNinety = _context.Asset
                .Count(a => a.WarrantyExpirationDate != null
                            && a.WarrantyExpirationDate > today.AddDays(30)
                            && a.WarrantyExpirationDate <= today.AddDays(90));

            var ninetyPlus = _context.Asset
                .Count(a => a.WarrantyExpirationDate != null
                            && a.WarrantyExpirationDate > today.AddDays(90));

            // Retrieve assets whose AssetStatus indicates they need attention.
            // Use lowercase list and compare against the column converted to lower to avoid case-sensitivity issues.
            var attentionStatuses = new[] { "maintenance", "pendingdeployment", "lost", "needsreplacement" };
            var attentionAssets = await _context.Asset
                .Where(a => a.AssetStatus != null && attentionStatuses.Contains(a.AssetStatus.ToLower()))
                .ToListAsync();

            var needsAttention = attentionAssets.Count;

            var viewModel = new AdminDashboardViewModel
            {
                StatusCounts = statusCounts,
                UnassignedCount = unassignedCount,
                ExpiringSoonCount = expiringSoonCount,
                ExpiredCount = expiredCount,
                ZeroToThirty = zeroToThirty,
                ThirtyOneToNinety = thirtyOneToNinety,
                NinetyPlus = ninetyPlus,
                NeedsAttention = needsAttention,
                Attention = attentionAssets
            };

            return View(viewModel);
        }



        // Borrower + Admin
        public async Task<IActionResult> Index()
        {
            // Get current signed-in user's ID (NameIdentifier)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Prepare defaults
            var assignedToUser = 0;
            var assigned = new List<Asset>();

            if (!string.IsNullOrEmpty(userId))
            {
                // Retrieve all assets assigned to the current user and set the count
                assigned = await _context.Asset
                    .Where(a => a.IssuedToUserId == userId)
                    .ToListAsync();

                assignedToUser = assigned.Count;
            }

            var viewModel = new BorrowerDashboardViewModel
            {
                AssignedToUser = assignedToUser,
                Assigned = assigned
            };
            return View(viewModel);
        }

        
    }
}
