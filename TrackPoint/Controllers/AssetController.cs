using Microsoft.AspNetCore.Mvc;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
	public class AssetController : Controller
	{
		public IActionResult AssetBrowser()
		{
			return View(Asset.SampleAssets);
		}
	}
}
