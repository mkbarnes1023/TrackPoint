using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
	public class AssetController : Controller
	{
		/**
		 *  Return the view for the Asset Browser with the sample data as the model
		 */
		public IActionResult AssetBrowser()
		{
			return View(Asset.SampleAssets);
		}

		/**
		 *  Return the view for the Asset Add Form
		 */
		public IActionResult AssetAdd()
		{
			return View();
		}

		/**
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
            if (asset == null) // TODO: What if asset is null? Check that first to prevent errors on line 32.
            {
                return NotFound();
            }
            return View(asset);
        }
    }
}
