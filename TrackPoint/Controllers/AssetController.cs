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
		 *  Return the view for the Location Add Form
		 */
		public IActionResult LocationAdd()
		{
			return View();
		}

		/**
		 *  Add the new location to the database and redirect to the index
		 */
		public IActionResult NewLocation(Location l)
		{
			// Add the new Location to database and redirect the user to the index

			// Log the Location to the console for debugging purposes
			Console.WriteLine($"New Locatoin Added: {l.Name}, {l.Abbreviation}");
			return View("../Home/Index");
		}
	}
}
