using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
    // [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        public IActionResult AllocateDemo()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Allocate(string AssetTag, string IssuedTo)
        {
            var asset = Asset.SampleAssets.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                TempData["Failure"] = $"Error: Asset not found.";
                return RedirectToAction("AllocateDemo");
            }

            if (IssuedTo == null)
            {
                TempData["Failure"] = $"Error: Invalid user.";
                return RedirectToAction("AllocateDemo");
            }

            // Store previous issued to and transfer date
            string previousIssuedTo = asset.IssuedTo ?? "Unassigned";
            DateTime previousTransferDate = asset.TransferDate;

            // Update asset information
            asset.IssuedTo = IssuedTo;
            asset.TransferDate = DateTime.Now;
            asset.Status = Asset.AssetStatus.InUse;

            // Update the asset's audit trail
            asset.AuditTrail.Add(new AuditTrail
            {
                AssetTag = asset.AssetTag,
                IssuedTo = previousIssuedTo,
                TransferDate = previousTransferDate,
                //Asset = asset
            });
            
            // Prevent duplicate form submissions on page refresh
            if (ModelState.IsValid)
            {
                TempData["Success"] = $"Asset {AssetTag} successfully allocated to {IssuedTo}!";
                return RedirectToAction("AssetBrowser", "Asset");
            }
            
            return View(); // TODO: This leads to nowhere, redirect back to form with error
        }
    }
}
