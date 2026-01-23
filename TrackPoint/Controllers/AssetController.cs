using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Security.Cryptography.Xml;
using TrackPoint.Models;
using TrackPoint.Models.RepositoryInterfaces;

namespace TrackPoint.Controllers
{
	public class AssetController : Controller
	{

        private IAssetRepository _assetRepository;
        private ICategoryRepository _categoryRepository;
        private ILocationRepository _locationRepository;
        public AssetController(IAssetRepository assetRepository, ICategoryRepository categoryRepository, ILocationRepository locationRepository)
        {
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;
            _locationRepository = locationRepository;
		}

		/*
         *  Return the view for the Asset Browser with the sample data as the model
         */
		public IActionResult AssetBrowser()
        {
            return View(_assetRepository.Assets);
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
        public IActionResult NewLocation(LocationModel l)
        {
            // Add the new Location to database and redirect the user to the index
            _locationRepository.AddLocation(l);
            _locationRepository.Save();

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
	    public IActionResult NewCategory(CategoryModel c)
	    {
		    // Add the new Category to database and redirect the user to the AssetBrowser
            _categoryRepository.AddCategory(c);
            _categoryRepository.Save();

			// Log the category to the console for debugging purposes
			Console.WriteLine($"New Category Added: {c.Name}, {c.Abbreviation}");
			return View("../Home/Index");
        }

		/* 
		 *  Add the new asset to the database and redirect to the Asset Browser
		 */
		public IActionResult NewAsset(AssetModel a)
		{
			// Assign the Asset a asset tag based on the Category's abbreviation and a unique number
            // Placeholder currently
			a.AssetTag = "Something";

			//Copilot assisted
			// Making the Asset Tag:
			// Get the category abbreviation the asset belongs to. If the category is not found, default to "GEN" (General)
			String catAbbv = _categoryRepository.Categories
                .FirstOrDefault(c => c.CategoryId == a.CategoryId)?.Abbreviation ?? "GEN";
			// Count how many assets are in that category already
            int catCount = _assetRepository.Assets
                .Count(asset => asset.CategoryId == a.CategoryId);
			// Increment by 1 to get the new asset's number
            catCount += 1;
			// Combine to make the asset tag
            a.AssetTag = $"{catAbbv}-{catCount:D4}";

			// Add the new Asset to database and redirect the user to the AssetBrowser
			_assetRepository.AddAsset(a);
            _assetRepository.Save();

			// Log the asset to the console for debugging purposes
			Console.WriteLine($"New Asset Added: {a.AssetTag}, {a.Make}, {a.Model}, {a.Category}, {a.Location}, {a.Notes}");
			return View("AssetBrowser", _assetRepository.Assets);
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
            _assetRepository.RemoveAssetByID(AssetTag);
            _assetRepository.Save();

			// TODO: Delete any other data that references this asset to prevent any null reference problems

			// Log updated asset
			Console.WriteLine($"Asset Deleted: {AssetTag}");
			return View("AssetBrowser", _assetRepository.Assets);
		}

		/**
         * Return the view for editing assets with the selected asset passed as the model
         */
		public IActionResult AssetEdit(string AssetTag)
        {
			var asset = _assetRepository.Assets.FirstOrDefault(a => a.AssetTag == AssetTag);
			if (asset == null)
			{
				return NotFound();
			}
			return View(asset);
        }

		/**
         * Return the view for editing assets with the selected asset passed as the model
         */
		public IActionResult UpdateAsset(AssetModel a)
		{
            _assetRepository.UpdateAsset(a);
            _assetRepository.Save();

			// Log updated asset
			Console.WriteLine($"Asset Updated: {a.AssetTag}, {a.Make}, {a.Model}, {a.Category}, {a.Location}, {a.Notes}");

			return View("AssetBrowser", _assetRepository.Assets);
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
