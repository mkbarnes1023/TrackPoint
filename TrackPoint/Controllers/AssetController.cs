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
         * Return the view for the Transfer Log with the sample data sorted by TransferDate descending as the model
         */
        public IActionResult TransferLog()
        {
            var sorted = Asset.SampleAssets.OrderByDescending(a => a.TransferDate).ToList();
            return View(sorted);
        }
    }
}
