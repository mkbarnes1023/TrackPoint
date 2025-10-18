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

        /**
         * Redirects to the Audit Trail view using the selected asset. In the future, this will be 
         * replaced with a more compact menu instead of a separate page just to view it.
         */
        public IActionResult AuditTrail(string AssetTag)
        {
            var asset = Asset.SampleAssets.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null) // What if asset is null? We should check that first to prevent errors on line 32.
            {
                return NotFound();
            }
            return View(asset);
        }
    }
}
