using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
    [Authorize(Roles = "Admin")] // admin-only section
    public class AdminController : Controller
    {
        public IActionResult AllocateDemo() => View();

        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Admin allocation logic
        public IActionResult Allocate(string assetTag, string issuedTo)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetTag == assetTag);
        
            // Verify if user entered input in all required fields
            if ((string.IsNullOrEmpty(issuedTo)) || string.IsNullOrEmpty(assetTag))
            {
                TempData["Failure"] = $"Error: Fields cannot be empty.";
                return RedirectToAction("AllocateDemo");
            }    
            
            // Verify if asset exists
            if (asset == null)
            {
                TempData["Failure"] = $"Error: Asset not found.";
                return RedirectToAction("AllocateDemo");
            }

            // Verify if user exists based on their username
            // TODO: Add proper input validation for this and search by NormalizedUserName instead for consistency purposes.
            var user = _context.Users.FirstOrDefault(u => u.UserName == issuedTo);

            if (user == null)
            {
                TempData["Failure"] = $"Error: User '{issuedTo}' not found.";
                return RedirectToAction("AllocateDemo");
            }

            // Store previous issued to and transfer date
            string previousIssuedTo = asset.IssuedToUserId ?? "Unassigned";
            DateTime previousTransferDate = asset.StatusDate;

            // Update asset information
            asset.IssuedToUserId = user.Id;
            asset.StatusDate = DateTime.Now; // TODO: This should be an Enum, but in the Asset model it's a String.
            asset.AssetStatus = "InUse";

            // Update the asset's audit trail
            // TODO: Reimplement the audit trail.
            //asset.AuditTrail.Add(new AuditTrail
            //{
            //    AssetTag = asset.AssetTag,
            //    IssuedTo = previousIssuedTo,
            //    TransferDate = previousTransferDate,
            //    //Asset = asset
            //});
            
            // Prevent duplicate form submissions on page refresh
            if (ModelState.IsValid)
            {
                TempData["Success"] = $"Asset {assetTag} successfully allocated to {issuedTo}!";
                return RedirectToAction("AssetBrowser", "Asset");
            }

            
            return RedirectToAction("AllocateDemo");
        }
    }
}
