using Microsoft.AspNetCore.Mvc;
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
	}
}
