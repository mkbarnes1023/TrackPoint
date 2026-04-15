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
using System.Threading.Tasks;

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
            var attentionStatuses = new[] { "UnderMaintenance", "PendingDeployment", "Lost", "NeedsReplacement" };
            var attentionAssets = await _context.Asset
                .Where(a => a.AssetStatus != null && attentionStatuses.Contains(a.AssetStatus.ToLower()))
                .ToListAsync();

            var needsAttention = attentionAssets.Count;

            var overdue = await _context.Asset
                .CountAsync(a => a.WarrantyExpirationDate != null && a.WarrantyExpirationDate < DateTime.Now);

            // Include related entities so views can access ApprovalReason and Asset properties
            var approvals = await _context.Approvals
                .Include(a => a.ApprovalReason)
                .Include(a => a.Asset)
                .ToListAsync();

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
                Overdue = overdue,
                Attention = attentionAssets,
                _approvals = approvals,
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
            var approvals = new List<Approvals>();
            var dueSoon = 0;
            var warrantyExpiringSoon = 0;


            if (!string.IsNullOrEmpty(userId))
            {
                // Retrieve all assets assigned to the current user and set the count
                assigned = await _context.Asset
                    .Where(a => a.IssuedToUserId == userId)
                    .ToListAsync();

                assignedToUser = assigned.Count;



                // Load approvals for this user and include related Asset and ApprovalReason for display
                approvals = await _context.Approvals
                    .Where(a => a.RequestorId == userId)
                    .Include(a => a.Asset)
                    .Include(a => a.ApprovalReason)
                    .ToListAsync();

                
            }

            // Calculate loans that are due soon for the current user (e.g., within 30 days)
            var today = DateTime.Today;
            var dueLimit = today.AddDays(2);
            if (!string.IsNullOrEmpty(userId))
            {
                dueSoon = await _context.Assetloan
                    .Where(l => l.BorrowerId == userId
                                && l.DueDate.HasValue
                                && l.DueDate.Value.Date >= today
                                && l.DueDate.Value.Date <= dueLimit
                                && l.ReturnedDate == null)
                    .CountAsync();

                // Count assets assigned to the user whose warranty is expiring soon (within 30 days)
                warrantyExpiringSoon = await _context.Asset
                    .Where(a => a.IssuedToUserId == userId
                                && a.WarrantyExpirationDate.HasValue
                                && a.WarrantyExpirationDate.Value.Date >= today
                                && a.WarrantyExpirationDate.Value.Date <= dueLimit)
                    .CountAsync();
            }

            var viewModel = new BorrowerDashboardViewModel
            {
                AssignedToUser = assignedToUser,
                Assigned = assigned,
                DueSoon = dueSoon,
                WarrantyExpiringSoon = warrantyExpiringSoon,
                _approvals = approvals
            };
            return View(viewModel);
        }
        
        
    }
}
