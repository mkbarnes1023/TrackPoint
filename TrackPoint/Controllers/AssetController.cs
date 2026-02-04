using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Security.Cryptography.Xml;
using TrackPoint.Data;
using TrackPoint.Models;
using TrackPoint.Views.Asset;

namespace TrackPoint.Controllers
{
    [Authorize(Roles = "Admin,Borrower")]
	public class AssetController : Controller
	{
		/*
         *  Return the view for the Asset Browser with the sample data as the model
         */

        private readonly ApplicationDbContext _context;
        private IEnumerable<Asset> assets => _context.Asset;
        private IEnumerable<Category> categories => _context.Category;
        private IEnumerable<Location> locations => _context.Location;
        public AssetController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AssetBrowser()
        {
            // Pack the needed information into a ViewModel
            AssetBrowserViewModel model = new AssetBrowserViewModel();
            model._assets = assets.ToList();
            model._categories = categories.ToList();
            model._locations = locations.ToList();
            return View(model);
        }


        /*
         *  Return the view for the Location Add Form
         */
        public IActionResult LocationAdd()
        {
            return View();
        }

        /*
         *  Add the new location to the database and redirect to the index
         */
        public IActionResult NewLocation(Location l)
        {
            // Make sure the Abbreviation is captialized
            l.Abbreviation = l.Abbreviation.ToUpper();
            // Add the new Location to database
            _context.Location.Add(l);
            _context.SaveChanges();

            // Log the Location to the console for debugging purposes
            Console.WriteLine($"New Locatoin Added: {l.Name}, {l.Abbreviation}");
            return View("../Home/Index");
        }
        /*
		 *  Return the view for the Category Add Form
		 */
        public IActionResult CategoryAdd()
        {
            return View();
        }

        /*
		 *  Return the view for the Asset Add Form
		 */
        public IActionResult AssetAdd()
        {
            // Pass the locations and categories to the view via the AssetAddModel
            AssetAddViewModel model = new AssetAddViewModel();
            model._locations = locations.ToList();
            model._categories = categories.ToList();
            return View(model);
        }

        /*
	    *  Add the new category to the database and redirect to the index
	    */
	    public IActionResult NewCategory(Category c)
	    {
            // Make sure the Abbreviation is captialized
            c.Abbreviation = c.Abbreviation.ToUpper();
            // Add the new Category to database and redirect the user to the AssetBrowser
            _context.Category.Add(c);
            _context.SaveChanges();
            // Log the category to the console for debugging purposes
            Console.WriteLine($"New Category Added: {c.Name}, {c.Abbreviation}");
            return View("../Home/Index");
        }

        /* 
		 *  Add the new asset to the database and redirect to the Asset Browser
		 */
		public IActionResult NewAsset(Asset asset)
		{
            // Assign the Asset a asset tag based on the Category's abbreviation and a unique number
            asset.AssetTag = $"{_context.Category.Find(asset.CategoryId)?.Abbreviation}-{_context.Asset.Count(a => a.CategoryId == asset.CategoryId) + 1}";
            
            // Add the new Asset to database and redirect the user to the AssetBrowser
            _context.Asset.Add(asset);
            _context.SaveChanges();

            // Log the asset to the console for debugging purposes
            Console.WriteLine($"New Asset Added: {asset.AssetTag}, {asset.Make}, {asset.Model}, {asset.Category}, {asset.Location}, {asset.IssuedToUser}, {asset.AssetStatus}, {asset.Notes}");

            // Pack the information for the AssetBrowser
            AssetBrowserViewModel model = new AssetBrowserViewModel();
            model._assets = assets.ToList();
            model._categories = categories.ToList();
            model._locations = locations.ToList();
            return View("AssetBrowser", model);
		}

        /**
         * Delete the asset from the database and redirect to the Asset Browser
         * 
         * We may want to make it clear that this function is for when a mistake was made,
         * rather than for when they are done with an asset. Assets they are finished with should
         * have their status changed to "Retired", to preserve their history in the logs.
         */
		public IActionResult DeleteAsset(string AssetTag)
		{
            Asset asset = _context.Asset.First(a => a.AssetTag == AssetTag);

            _context.Asset.Remove(asset);
            _context.SaveChanges();

			// Log deleted asset
			Console.WriteLine($"Asset Deleted: {AssetTag}");
            // Pack the information for the AssetBrowser
            AssetBrowserViewModel model = new AssetBrowserViewModel();
            model._assets = assets.ToList();
            model._categories = categories.ToList();
            model._locations = locations.ToList();
            return View("AssetBrowser", model);
		}

        /**
         * Return the view for editing assets with the selected asset passed as the model
         */
		public IActionResult AssetEdit(string AssetTag)
        {
            Asset asset = _context.Asset.First(a => a.AssetTag == AssetTag);
			if (asset == null)
			{
				return NotFound();
			}
            AssetAddViewModel model = new AssetAddViewModel();
            model._categories = categories.ToList();
            model._locations = locations.ToList();
            model.asset = asset;
            return View(model);
        }

        /**
         * Return the view for editing assets with the selected asset passed as the model
         */

		public IActionResult UpdateAsset(Asset asset)
		{
            // Update the asset in the database
            _context.Asset.Update(asset);
            _context.SaveChanges();

            // Log updated asset
            Console.WriteLine($"Asset Updated: {asset.AssetTag}, {asset.Make}, {asset.Model}, {asset.Category}, {asset.Location}, {asset.IssuedToUser}, {asset.AssetStatus}, {asset.Notes}");
            // Pack the information for the AssetBrowser
            AssetBrowserViewModel model = new AssetBrowserViewModel();
            model._assets = assets.ToList();
            model._categories = categories.ToList();
            model._locations = locations.ToList();
            return View("AssetBrowser", model);
		}

        /**
         * Return the view for the Transfer Log with the sample data sorted by TransferDate descending as the 
         */
        // TODO: This is not a real transfer log since it's just sorting the assets by transfer date, it does not give a detailed history.
        // Create a transfer log model in the future to properly track asset transfers.
        // TODO: Remove SampleAssets and replace with database calls
        /*
		public IActionResult TransferLog()
        {
            var sorted = Asset.SampleAssets.OrderByDescending(a => a.TransferDate).ToList();
            return View(sorted);
        }
        */

        /**
         * Redirects to the Audit Trail view using the selected asset. In the future, this will be 
         * replaced with a more compact menu instead of a separate page just to view it.
         */
        // TODO: Remove SampleAssets and replace with database calls
        /*
		public IActionResult AuditTrail(string AssetTag)
        {
            var asset = Asset.SampleAssets.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                return NotFound();
            }
            return View(asset);
        }
        */

        // TODO: Remove SampleAssets and replace with database calls
        /*
		public IActionResult AssetView(string AssetTag)
        {
            var asset = Asset.SampleAssets.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }
        */

        // TODO: Remove SampleAssets and replace with database calls
        /*
		public IActionResult checkOut(string AssetTag)
        {
            var asset = Asset.SampleAssets.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                TempData["Failure"] = $"Error: Asset not found.";
                return RedirectToAction("AllocateDemo");
            }

            // Store previous issued to and transfer date
            string previousIssuedTo = asset.IssuedTo ?? "Unassigned";
            DateTime previousTransferDate = asset.TransferDate;

            // Update asset information
            asset.IssuedTo = @User.Identity?.Name;
            asset.TransferDate = DateTime.Now;
            asset.Status = Enums.AssetStatus.InUse;

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
                TempData["Success"] = $"Asset {AssetTag} successfully allocated to {asset.IssuedTo}!";
                return RedirectToAction("AssetBrowser", "Asset");
            }
            
            return View(); // TODO: This leads to nowhere, redirect back to form with error
        }
        */
    }
}
