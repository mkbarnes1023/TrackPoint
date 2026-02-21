using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using TrackPoint.Data;
using TrackPoint.Models;
using TrackPoint.Views.Asset;
using QRCoder;
using System.Collections.Immutable;

namespace TrackPoint.Controllers
{
    [Authorize(Roles = "Admin,Borrower")]
	public class AssetController : Controller
	{
		/*
         *  Return the view for the Asset Browser with the sample data as the model
         */

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private IEnumerable<Asset> assets => _context.Asset;
        private IEnumerable<Category> categories => _context.Category;
        private IEnumerable<Location> locations => _context.Location;
        public AssetController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            return View(new Location());
        }

        /*
         *  Add the new location to the database and redirect to the index
         */
        public IActionResult NewLocation(Location l)
        {
            // Perform input validation. Redirect back to the form with an error message if the input is invalid.
            // Copilot:
            // Empty or whitespace name/abbreviation
            if (string.IsNullOrWhiteSpace(l.Name) || string.IsNullOrWhiteSpace(l.Abbreviation))
            {
                TempData["InputError"] = "Error: Name and Abbreviation cannot be empty.";
                return View("LocationAdd", l);
            }
            // Abbreviation too long
            if (l.Abbreviation.Length > 10)
            {
                TempData["InputError"] = "Error: Abbreviation cannot be longer than 10 characters.";
                return View("LocationAdd", l);
            }
            // Location with same name already exists
            if (_context.Location.Any(loc => loc.Name == l.Name))
            {
                Location existingLocation = _context.Location.First(loc => loc.Name == l.Name);
                TempData["InputError"] = "Error: A location with this name already exists: " + existingLocation.Name + " (" + existingLocation.Abbreviation + ")";
                return View("LocationAdd", l);
            }
            // Location with same abbreviation already exists
            if (_context.Location.Any(loc => loc.Abbreviation == l.Abbreviation.ToUpper()))
            {
                Location existingLocation = _context.Location.First(loc => loc.Name == l.Name);
                TempData["InputError"] = "Error: A location with this abbreviation already exists: " + existingLocation.Name + " (" + existingLocation.Abbreviation + ")";
                return View("LocationAdd", l);
            }

            // Prepare the data for entry into the database:
            // Make sure the Abbreviation is captialized
            l.Abbreviation = l.Abbreviation.ToUpper();
            // Add the new Location to database
            _context.Location.Add(l);
            _context.SaveChanges();

            // Log the Location to the console for debugging purposes
            Console.WriteLine($"New Location Added: {l.Name}, {l.Abbreviation}");
            return View("../Home/Index");
        }
        /*
		 *  Return the view for the Category Add Form
		 */
        public IActionResult CategoryAdd()
        {
            return View(new Category());
        }

        /*
		 *  Return the view for the Asset Add Form
		 */
        public IActionResult AssetAdd(Asset a)
        {
            // Pass the locations and categories to the view via the AssetAddModel
            AssetAddViewModel model = new AssetAddViewModel();
            model._locations = locations.ToList();
            model._categories = categories.ToList();
            model.asset = a;
            return View(model);
        }

        /*
	    *  Add the new category to the database and redirect to the index
	    */
        public IActionResult NewCategory(Category c)
	    {
            // Perform input validation. Redirect back to the form with an error message if the input is invalid.
            // Copilot:
            // Empty or whitespace name/abbreviation
            if (string.IsNullOrWhiteSpace(c.Name) || string.IsNullOrWhiteSpace(c.Abbreviation))
            {
                TempData["InputError"] = "Error: Name and Abbreviation cannot be empty.";
                return View("CategoryAdd", c);
            }
            // Abbreviation too long
            if (c.Abbreviation.Length > 10)
            {
                TempData["InputError"] = "Error: Abbreviation cannot be longer than 10 characters.";
                return View("CategoryAdd", c);
            }
            // Abbreviation contains whitespace
            if (c.Abbreviation.Any(char.IsWhiteSpace))
            {
                TempData["InputError"] = "Error: Abbreviation cannot contain whitespace.";
                return View("CategoryAdd", c);
            }
            // Category with same name already exists
            if (_context.Category.Any(cat => cat.Name == c.Name))
            {
                Category existingCategory = _context.Category.First(cat => cat.Name == c.Name);
                TempData["InputError"] = "Error: A category with this name already exists: " + existingCategory.Name + " (" + existingCategory.Abbreviation + ")";
                return View("CategoryAdd", c);
            }
            // Category with same abbreviation already exists
            if (_context.Category.Any(cat => cat.Abbreviation == c.Abbreviation.ToUpper()))
            {
                Category existingCategory = _context.Category.First(cat => cat.Name == c.Name);
                TempData["InputError"] = "Error: A category with this abbreviation already exists: " + existingCategory.Name + " (" + existingCategory.Abbreviation + ")";
                return View("CategoryAdd", c);
            }
            // Negative default loan period
            if (c.DefaultLoanPeriodDays < 0)
            {
                TempData["InputError"] = "Error: Default Loan Period cannot be negative.";
                return View("CategoryAdd", c);
            }

            // Prepare the data for entry into the database:
            // Make sure the Abbreviation is captialized
            c.Abbreviation = c.Abbreviation.ToUpper();
            // Add the new Category to database and redirect the user to the Index
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
            // Perform input validation. Redirect back to the form with an error message if the input is invalid.
            // Copilot:
            // Empty or whitespace make/model
            if (string.IsNullOrWhiteSpace(asset.Make) || string.IsNullOrWhiteSpace(asset.Model))
            {
                TempData["InputError"] = "Error: Make and Model cannot be empty.";
                return RedirectToAction("AssetAdd", asset);
            }
            // Category is empty or invalid
            if (!_context.Category.Any(c => c.CategoryId == asset.CategoryId))
            {
                TempData["InputError"] = "Error: Invalid category selected.";
                return RedirectToAction("AssetAdd", asset);
            }
            // Location is empty or invalid
            if (!_context.Location.Any(l => l.LocationId == asset.LocationId))
            {
                TempData["InputError"] = "Error: Invalid location selected.";
                return RedirectToAction("AssetAdd", asset);
            }

            // Check if IssuedToUser is valid if not null (TODO: Currently broken.)
            if (asset.IssuedToUser != null && !_userManager.Users.Any(u => u.Id == asset.IssuedToUserId))
            {
                TempData["InputError"] = $"Error: User '{asset.IssuedToUser}' not found.";
                return RedirectToAction("AssetAdd", asset);
            }

            // Assign the Asset an asset tag based on the Category's abbreviation, Location abbreviation and a unique number, padded to 4 digits with leading zeros.
            asset.AssetTag = $"{_context.Category.Find(asset.CategoryId)?.Abbreviation}-{_context.Location.Find(asset.LocationId)?.Abbreviation}-{(_context.Asset.Count(a => a.CategoryId == asset.CategoryId && a.LocationId == asset.LocationId) + 1).ToString().PadLeft(4, '0')}";


            //asset.AssetTag = $"{_context.Category.Find(asset.CategoryId)?.Abbreviation}-{_context.Asset.Count(a => a.CategoryId == asset.CategoryId) + 1}";

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
        // TODO: Update AssetLoan for this as well, likely by deleting the loan
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
        // TODO: This may need another update for AssetLoan
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
        public IActionResult AssetEditFromModel(Asset a)
        {
            AssetAddViewModel model = new AssetAddViewModel();
            model._categories = categories.ToList();
            model._locations = locations.ToList();
            model.asset = a;
            return View("AssetEdit", model);
        }

        /**
         * Return the view for editing assets with the selected asset passed as the model
         */

        public IActionResult UpdateAsset(Asset asset)
		{
            // Perform input validation. Redirect back to the form with an error message if the input is invalid.
            // Copilot:
            // Empty or whitespace make/model
            if (string.IsNullOrWhiteSpace(asset.Make) || string.IsNullOrWhiteSpace(asset.Model))
            {
                TempData["InputError"] = "Error: Make and Model cannot be empty.";
                return RedirectToAction("AssetEditFromModel", asset);
            }
            // Category is empty or invalid
            if (!_context.Category.Any(c => c.CategoryId == asset.CategoryId))
            {
                TempData["InputError"] = "Error: Invalid category selected.";
                return RedirectToAction("AssetEditFromModel", asset);
            }
            // Location is empty or invalid
            if (!_context.Location.Any(l => l.LocationId == asset.LocationId))
            {
                TempData["InputError"] = "Error: Invalid location selected.";
                return RedirectToAction("AssetEditFromModel", asset);
            }
            // Update the asset in the database
            // TODO: Update AssetLoan here if necessary
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

		public IActionResult TransferLog()
        {
            var sorted = _context.Asset.OrderByDescending(a => a.StatusDate).ToList();
            return View(sorted);
        }

        /**
         * Redirects to the Audit Trail view using the selected asset. In the future, this will be 
         * replaced with a more compact menu instead of a separate page just to view it.
         */
		public IActionResult AuditTrail(string AssetTag)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                return NotFound();
            }
            return View(asset);
        }

		public IActionResult AssetView(string AssetTag)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }
		// TODO: Upgrade this to the new checkout process.
        public IActionResult checkOut(string AssetTag)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetTag == AssetTag);
            
            if (asset == null)
            {
                TempData["Failure"] = $"Error: Asset not found.";
                return RedirectToAction("AssetBrowser");
            }

            // Get the current user's Id
            string userId = _userManager.GetUserId(User);
            // alternatively: string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            asset.IssuedToUserId = userId;
            asset.StatusDate = DateTime.Now;
            asset.AssetStatus = "InUse";
            
            // Update the asset's audit trail
            // TODO: Properly re-implement this functionality.
            //asset.AuditTrail.Add(new AuditTrail
            //{
            //    AssetTag = asset.AssetTag,
            //    IssuedTo = previousIssuedTo,
            //    TransferDate = previousTransferDate,
            //    //Asset = asset
            //});
            
            // Update AssetLoan for Check Out
            if (asset != null)
            {
                // TODO: Update this for full Check Out process. This should work for now, but just be wary
                // of whether ApprovedByUserId should be set by the Borrower or Admin depending on context.
                var assetLoan = UpdateLoanStatus(asset.AssetId, asset.IssuedToUserId, "InUse", 0);
                _context.Assetloan.Update(assetLoan);
            }
            _context.SaveChanges();

            // Prevent duplicate form submissions on page refresh
            if (ModelState.IsValid)
            {
                TempData["Success"] = $"Asset {AssetTag} successfully allocated to {User.Identity?.Name}!";
                return RedirectToAction("AssetBrowser", "Asset");
            }
            
            return View(); // TODO: This leads to nowhere, redirect back to form with error or remove this if we will never reach it
        }

        // Check In an Asset
        public IActionResult checkIn(string AssetTag)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetTag == AssetTag);
            var assetLoan = _context.Assetloan.Where(al => al.AssetId == asset.AssetId).OrderByDescending(al => al.CheckedoutDate).FirstOrDefault();
            
            if (asset == null)
            {
                TempData["Failure"] = $"Error: Asset not found.";
                return RedirectToAction("AssetBrowser");
            }
            asset.IssuedToUserId = null;
            asset.StatusDate = DateTime.Now;
            asset.AssetStatus = "InStorage";
            
            // Update AssetLoan for Check In
            if (assetLoan != null)
            {
                UpdateLoanStatus(asset.AssetId, null, null, 2);
                _context.Assetloan.Update(assetLoan);
            }
            _context.SaveChanges();

            // Prevent duplicate form submissions on page refresh
            if (ModelState.IsValid)
            {
                TempData["Success"] = $"Asset {AssetTag} successfully checked back in!";
                return RedirectToAction("AssetBrowser", "Asset");
            }
            
            return View();
        }

        // Generate QR Code for Asset
        // TODO: Fix the URL and integrate this into the Check Out flow later
        [HttpPost]
        public IActionResult GenerateQRCode(string assetTag)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(assetTag, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (var qrCodeImage = qrCode.GetGraphic(20))
                    {
                        using (var ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // TODO: Test compatibility of this, CA1416 claims this won't work on non-Windows browsers
                            var qrCodeBytes = ms.ToArray();
                            return File(qrCodeBytes, "image/png", $"{assetTag}_QRCode.png");
                        }
                    }
                }
            }
        }

        // Report an Asset for Maintenance
        public IActionResult ReportForMaintenance(string AssetTag)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                TempData["Failure"] = $"Error: Asset not found.";
                return RedirectToAction("AssetBrowser");
            }
            // TODO: Check for errors in maintenance request
            // TODO: Change asset status and send information to approvals queue
            // Redirect user to Asset Browser/Dashboard on success
            if (ModelState.IsValid)
            {
                TempData["Success"] = $"Asset {AssetTag} successfully reported for maintenance.";
                return RedirectToAction("AssetBrowser", "Asset");
            }
            return View();
        }

        // Update the AssetLoan status of an Asset
        public AssetLoan UpdateLoanStatus(int AssetId, string borrowerId, string newStatus, int updateType)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetId == AssetId);
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = _userManager.Users.FirstOrDefault(u => u.Id == currentUserId);

            if (asset == null)
            {
                Console.WriteLine($"Error: Asset with ID {AssetId} not found.");
                return null;
            }

            // Create a new Asset Loan for this asset and save it to the AssetLoan table
            // TODO: Perform validation/error handling and don't create this if we're checking in an asset (unnecessary since it's unused in that case)
            AssetLoan assetLoan = new AssetLoan();

            // Update the Asset Loan based on the update type, such as initial AssetLoan creation, check in/check out, retirement, etc.
            // TODO: Avoid magic numbers by making an enum for update types
            switch(updateType)
            {
                case 0: // Asset Transfer (Check Out)
                    // TODO: Check Out can potentially become desynchronized with Asset Status, such as a loan existing even if the asset is InStorage. 
                    assetLoan.AssetId = asset.AssetId;
                    assetLoan.BorrowerId = borrowerId;
                    assetLoan.CheckedoutDate = DateTime.Now; // TODO: Verify that this time is consistent with StatusDate in Asset, maybe we make StatusDate a FK for CheckedoutDate?
                    assetLoan.DueDate = DateTime.Now.AddDays(_context.Category.Find(asset.CategoryId)?.DefaultLoanPeriodDays ?? 14); // Default to 2 week loan period if category not found for some reason
                    assetLoan.ReturnedDate = null; // This should not be set on creation
                    assetLoan.ExtendedByAdminId = null;
                    assetLoan.ExtendedBy = null; // This is ExtendedById in the table, but different here. May cause issues.
                    assetLoan.ApprovedByUserId = _userManager.GetUserId(User) ?? string.Empty; // This should be set by an admin when they approve the loan, not on creation. TODO: This cannot currently be properly nulled since it is required.
                    assetLoan.ApprovedBy = currentUser; // This is also different from the table
                    break;
                
                case 1: // Asset Transfer (Borrower switched between two people)
                    // TODO: Write this
                    break;

                case 2: // Asset Transfer (Check In)
                    // TODO: Currently I'm just deleting the AssetLoan, may way to keep it (see commented code below).
                    var removeLoan = _context.Assetloan.Where(al => al.AssetId == asset.AssetId).FirstOrDefault();
                    if (removeLoan != null)
			        {
				        assetLoan = removeLoan;
				        _context.Assetloan.Remove(removeLoan);
			        }
                    
                    /* assetLoan.AssetId = asset.AssetId;
                    assetLoan.BorrowerId = null; // Claims to not be nullable, yet it starts as null in the database
                    // assetLoan.CheckedoutDate = DateTime.Now; // TODO: This doesn't change, not sure whether we should null it
                    assetLoan.DueDate = null; // Not relevant since the asset is returned
                    assetLoan.ReturnedDate = DateTime.Now;
                    assetLoan.ExtendedByAdminId = null;
                    assetLoan.ExtendedBy = null;
                    assetLoan.ApprovedByUserId = _userManager.GetUserName(User);
                    assetLoan.ApprovedBy = currentUser; */
                    break;

                default:
                    Console.WriteLine($"Error: Invalid update type {updateType}.");
                    return null;

            }
            //if (assetLoan != null) // TODO: Verify if this check works correctly, probably rewrite this later
            //{
            //    _context.Assetloan.Add(assetLoan); // Save changes to DB
            //    //_context.SaveChanges();
            //}
            Console.WriteLine("assetLoan AssetId: " + assetLoan.AssetId);
            return assetLoan;
        }
    }
}