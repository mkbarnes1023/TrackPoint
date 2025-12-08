using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Security.Cryptography.Xml;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
	public class AssetController : Controller
	{
        /*
         *  Return the view for the Asset Browser with the sample data as the model
         */
        public IActionResult AssetBrowser()
        {
            return View(Asset.SampleAssets);
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
            // Add the new Location to database and redirect the user to the index

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
			return View();
		}

	    /*
	    *  Add the new category to the database and redirect to the index
	    */
	    public IActionResult NewCategory(Category c)
	    {
		    // Add the new Category to database and redirect the user to the AssetBrowser

			// Log the category to the console for debugging purposes
			Console.WriteLine($"New Category Added: {c.Name}, {c.Abbreviation}");
			return View("../Home/Index");
        }
     
        /* 
		 *  Add the new asset to the database and redirect to the Asset Browser
		 */
		public IActionResult NewAsset(Asset a)
		{
			// Assign the Asset a asset tag based on the Category's abbreviation and a unique number
			a.AssetTag = "Something";

			// Add the new Asset to database and redirect the user to the AssetBrowser
			Asset.SampleAssets = Asset.SampleAssets.Append(a);

			// Log the asset to the console for debugging purposes
			Console.WriteLine($"New Asset Added: {a.AssetTag}, {a.Make}, {a.Model}, {a.Category}, {a.Location}, {a.IssuedTo}, {a.Status}, {a.Notes}");
			return View("AssetBrowser", Asset.SampleAssets);
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
       // TODO: Replace with database functions
			// Convert IEnumerable to List
			List<Asset> assets = Asset.SampleAssets.ToList();
      // Remove the asset with the given asset tag
      assets.Remove(assets.FirstOrDefault(a => a.AssetTag == AssetTag));
			// Convert the asset List back to a IEnumerable and reassign it to Sample Assets
			Asset.SampleAssets = assets;

      // TODO: Delete any other data that references this asset to prevent any null reference problems

			// Log updated asset
			Console.WriteLine($"Asset Deleted: {AssetTag}");
      return View(asset);
    }
        /**
         * Return the view for editing assets with the selected asset passed as the model
         */
    public IActionResult AssetEdit(string AssetTag)
    {
			var asset = Asset.SampleAssets.FirstOrDefault(a => a.AssetTag == AssetTag);
			if (asset == null)
			{
				return NotFound();
			}
			return View(asset);
    }

		/**
         * Return the view for editing assets with the selected asset passed as the model
         */
		public IActionResult UpdateAsset(Asset a)
		{
            // TODO: Replace with database functions and stop using SampleAssets

            // Convert IEnumerable to List
            List<Asset> assets = Asset.SampleAssets.ToList();
            // Get the index of the asset that shares the tag of the one we are updating
            int assetIndex = assets.IndexOf(assets.FirstOrDefault(asset => asset.AssetTag == a.AssetTag));
            // Replace the asset with the updated one
            assets[assetIndex] = a;
            // Convert the asset List back to a IEnumerable and reassign it to Sample Assets
            Asset.SampleAssets = assets;

			// Log updated asset
			Console.WriteLine($"Asset Updated: {a.AssetTag}, {a.Make}, {a.Model}, {a.Category}, {a.Location}, {a.IssuedTo}, {a.Status}, {a.Notes}");

			return View("AssetBrowser", Asset.SampleAssets);
		}

		/**
         * Return the view for the Transfer Log with the sample data sorted by TransferDate descending as the 
         */
		// TODO: This is not a real transfer log since it's just sorting the assets by transfer date, it does not give a detailed history.
		// Create a transfer log model in the future to properly track asset transfers.
		public IActionResult TransferLog()
        {
            var sorted = Asset.SampleAssets.OrderByDescending(a => a.TransferDate).ToList();
            return View(sorted);
        }

        /**
         * Redirects to the Audit Trail view using the selected asset. In the future, this will be 
         * replaced with a more compact menu instead of a separate page just to view it.
         */
        public IActionResult AuditTrail(string AssetTag)
        {
            var asset = Asset.SampleAssets.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                return NotFound();
            }
            return View(asset);
        }

        public IActionResult AssetView(string AssetTag)
        {
            var asset = Asset.SampleAssets.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

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
                TempData["Success"] = $"Asset {AssetTag} successfully allocated to {asset.IssuedTo}!";
                return RedirectToAction("AssetBrowser", "Asset");
            }
            
            return View(); // TODO: This leads to nowhere, redirect back to form with error
        }
    }
}
