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
		 *  Return the view for the Category Add Form
		 */
		public IActionResult CategoryAdd()
		{
			return View();
		}

		/**
		 *  Add the new category to the database and redirect to the index
		 */
		public IActionResult NewCategory(Categroy c)
		{
			// Add the new Category to database and redirect the user to the AssetBrowser

			// Log the category to the console for debugging purposes
			Console.WriteLine($"New Category Added: {c.Name}, {c.Abbreviation}");
			return View("../Home/Index");
		}
	}
}
