using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Threading;
using TrackPoint.Data;
using TrackPoint.Models;
using TrackPoint.Views.Asset;

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
         *  Return the view for the Location Edit form
         */
        public IActionResult LocationEdit(int locationId)
        {
            LocationEditViewModel model = new LocationEditViewModel();
            model.location = _context.Location.Find(locationId);
            return View(model);
        }

        /*
		 *  Try to delete a category
		 */
        public IActionResult DeleteLocation(int locationId)
        {
            // Check that the location specified corresponds to a real location
            if (!_context.Location.Any(l => l.LocationId == locationId))
            {
                Console.WriteLine($"Couldn't Find location: {locationId}");
                return RedirectToAction("ManageLocations");
            }
            // Set each asset in this location to a "null" location
            IEnumerable<Asset> assets = _context.Asset.Where(l => l.LocationId == locationId);
            foreach (Asset a in assets)
            {
                // Replace with null location's ID in the future. "Unasigned" is seeded with id 1 by default.
                a.LocationId = 1;
                _context.Update(a);
            }

            // Remove the category
            Location l = _context.Location.Find(locationId);
            _context.Location.Remove(l);
            _context.SaveChanges();

            return RedirectToAction("ManageLocations");
        }

        /*
        *  Add the new location to the database and redirect to the index
        */
        public IActionResult EditLocation(LocationEditViewModel l)
        {
            Location location = l.location;
            // Validate the category. If the string returned isnt empty, return a error message back to the view.
            string ErrorString = ValidateLocation(location);
            if (!ErrorString.Equals(""))
            {
                TempData["InputError"] = ErrorString;
                LocationEditViewModel model = new LocationEditViewModel();
                model.location = location;
                return View("LocationEdit", model);
            }

            // Prepare the data for entry into the database:
            // Make sure the Abbreviation is captialized
            location.Abbreviation = location.Abbreviation.ToUpper();
            // Add the new Location to database
            _context.Location.Update(location);
            // If the user selected "Update Asset Tags", find all the assets in this category and update their tags
            if (l.updateAssetTags)
            {
                IEnumerable<Asset> assets = _context.Asset.Where(a => a.LocationId == location.LocationId).ToList();
                foreach (Asset a in assets)
                {
                    // Replace with null location's ID in the future. "Unasigned" is seeded with id 1 by default.
                    // Assign the Asset an asset tag based on the Category's abbreviation, Location abbreviation and a unique number, padded to 4 digits with leading zeros.
                    a.AssetTag = $"{_context.Category.Find(a.CategoryId)?.Abbreviation}-{_context.Location.Find(a.LocationId)?.Abbreviation}-{a.AssetTag.Substring(a.AssetTag.Length - 4)}";
                    _context.Asset.Update(a);
                    Console.WriteLine($"Asset Tag Updated: {a.AssetTag}");
                }
            }
            _context.SaveChanges();

            // Log the Location to the console for debugging purposes
            Console.WriteLine($"Location Edited: {location.Name}, {location.Abbreviation}");
            return RedirectToAction("ManageLocations");
        }

        /*
         *  Add the new location to the database and redirect to the index
         */
        public IActionResult NewLocation(Location l)
        {
            // Validate the category. If the string returned isnt empty, return a error message back to the view.
            string ErrorString = ValidateLocation(l);
            if (!ErrorString.Equals(""))
            {
                TempData["InputError"] = ErrorString;
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
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ManageLocations()
        {
            IEnumerable<Location> locations = _context.Location.ToList();
            return View(locations);
        }

        /*
		 *  Return the view for the Category Add Form
		 */
        public IActionResult CategoryAdd()
        {
            return View(new Category());
        }

        /*
         *  Return the view for the Categroy Edit form
         */
        public IActionResult CategoryEdit(int categoryId)
        {
            CategoryEditViewModel model = new CategoryEditViewModel();
            model.category = _context.Category.Find(categoryId);
            return View(model);
        }

        public IActionResult ManageCategories()
        {
            IEnumerable<Category> categories = _context.Category.ToList();
            return View(categories);
        }

        /*
         *  Update the category and return to the Managament page
         */
        public IActionResult EditCategory(CategoryEditViewModel c)
        {
            Category category = c.category;
            // Validate the category. If the string returned isnt empty, return a error message back to the view.
            string ErrorString = ValidateCategory(category);
            if (!ErrorString.Equals(""))
            {
                TempData["InputError"] = ErrorString;
                CategoryEditViewModel model = new CategoryEditViewModel();
                model.category = category;
                return View("CategoryEdit", model);
            }

            // Prepare the data for entry into the database:
            // Make sure the Abbreviation is captialized
            category.Abbreviation = category.Abbreviation.ToUpper();
            // Add the new Category to database
            _context.Category.Update(category);
            // If the user selected "Update Asset Tags", find all the assets in this category and update their tags
            if(c.updateAssetTags)
            {
                IEnumerable<Asset> assets = _context.Asset.Where(a => a.CategoryId == category.CategoryId).ToList();
                foreach (Asset a in assets)
                {
                    // Replace with null location's ID in the future. "Unasigned" is seeded with id 1 by default.
                    // Assign the Asset an asset tag based on the Category's abbreviation, Location abbreviation and the number it had previously
                    a.AssetTag = $"{_context.Category.Find(a.CategoryId)?.Abbreviation}-{_context.Location.Find(a.LocationId)?.Abbreviation}-{a.AssetTag.Substring(a.AssetTag.Length - 4)}";
                    _context.Asset.Update(a);
                    Console.WriteLine($"Asset Tag Updated: {a.AssetTag}");
                }
            }
            _context.SaveChanges();
            // Log the category to the console for debugging purposes
            Console.WriteLine($"Category Updated: {category.Name}, {category.Abbreviation}");
            return RedirectToAction("ManageCategories");
        }

        /*
		 *  Try to delete a category
		 */
        public IActionResult DeleteCategory(int categoryId)
        {
            // Check that the category specified corresponds to a real category
            if (!_context.Category.Any(c => c.CategoryId == categoryId))
            {
                Console.WriteLine($"Couldn't Find category: {categoryId}");
                return RedirectToAction("ManageCategories");
            }
            // Set each asset in this category to a "null" category
            IEnumerable<Asset> assets = _context.Asset.Where(a => a.CategoryId == categoryId);
            foreach (Asset a in assets)
            {
                // Replace with null category's ID. "Unasigned" is seeded as 1 by default
                a.CategoryId = 1;
                _context.Update(a);
            }

            // Remove the category
            Category c = _context.Category.Find(categoryId);
            _context.Category.Remove(c);
            _context.SaveChanges();

            return RedirectToAction("ManageCategories");
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
            // Validate the category. If the string returned isnt empty, return a error message back to the view.
            string ErrorString = ValidateCategory(c);
            if (!ErrorString.Equals(""))
            {
                TempData["InputError"] = ErrorString;
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
            return RedirectToAction("Index", "Home");
        }

        /* 
		 *  Add the new asset to the database and redirect to the Asset Browser
		 */
        public IActionResult NewAsset(Asset asset)
        {
            // Validate the asset. If the string returned isnt empty, return a error message back to the view.
            string ErrorString = ValidateAsset(asset);
            if (!ErrorString.Equals(""))
            {
                TempData["InputError"] = ErrorString;
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
            // Validate the asset. If the string returned isnt empty, return a error message back to the view.
            string ErrorString = ValidateAsset(asset);
            if (!ErrorString.Equals(""))
            {
                TempData["InputError"] = ErrorString;
                return RedirectToAction("AssetEditFromModel", asset);
            }

            // Update the asset in the database
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

            _context.SaveChanges();

            // Prevent duplicate form submissions on page refresh
            if (ModelState.IsValid)
            {
                TempData["Success"] = $"Asset {AssetTag} successfully allocated to {User.Identity?.Name}!";
                return RedirectToAction("AssetBrowser", "Asset");
            }

            return View(); // TODO: This leads to nowhere, redirect back to form with error or remove this if we will never reach it
        }

        public IActionResult checkIn(string AssetTag)
        {
            var asset = _context.Asset.FirstOrDefault(a => a.AssetTag == AssetTag);
            if (asset == null)
            {
                TempData["Failure"] = $"Error: Asset not found.";
                return RedirectToAction("AssetBrowser");
            }
            asset.IssuedToUserId = null;
            asset.StatusDate = DateTime.Now;
            asset.AssetStatus = "InStorage";
            _context.SaveChanges();

            // Prevent duplicate form submissions on page refresh
            if (ModelState.IsValid)
            {
                TempData["Success"] = $"Asset {AssetTag} successfully checked back in!";
                return RedirectToAction("AssetBrowser", "Asset");
            }

            return View();
        }

        /*
         * Validate a given asset. 
         * Returns an empty string if all checks passed
         * Return an error string if a check fails
         */
        public string ValidateAsset(Asset a)
        {
            // Perform input validation. Redirect back to the form with an error message if the input is invalid.
            // Copilot:
            // Empty or whitespace make/model
            if (string.IsNullOrWhiteSpace(a.Make) || string.IsNullOrWhiteSpace(a.Model))
            {
                return "Error: Make and Model cannot be empty.";
            }
            // Category is empty or invalid
            if (!_context.Category.Any(c => c.CategoryId == a.CategoryId))
            {
                return "Error: Invalid category selected.";

            }
            // Location is empty or invalid
            if (!_context.Location.Any(l => l.LocationId == a.LocationId))
            {

                return "Error: Invalid location selected.";
            }
            // Check if IssuedToUser is valid if not null (TODO: Currently broken.)
            if (a.IssuedToUser != null && !_userManager.Users.Any(u => u.Id == a.IssuedToUserId))
            {
                return $"Error: User '{a.IssuedToUser}' not found.";
            }

            // If all checks pass, return empty string
            return "";
        }

        /*
         * Validate a given location. 
         * Returns an empty string if all checks passed
         * Return an error string if a check fails
         */
        public string ValidateLocation(Location l)
        {
            // Perform input validation. Redirect back to the form with an error message if the input is invalid.
            // Copilot:
            // Empty or whitespace name/abbreviation
            if (string.IsNullOrWhiteSpace(l.Name) || string.IsNullOrWhiteSpace(l.Abbreviation))
            {
                return "Error: Name and Abbreviation cannot be empty.";
            }
            // Abbreviation too long
            if (l.Abbreviation.Length > 10)
            {
                return "Error: Abbreviation cannot be longer than 10 characters.";
            }
            // Location uses reserved name
            if (l.Name.Equals("Unassigned"))
            {
                return "Error: The name \"Unassigned\" is reserved. Please choose a different name.";
            }
            // Location uses reserved abbreviation
            if (l.Abbreviation.ToUpper().Equals("UN"))
            {
                return "Error: The abbreviation \"UN\" is reserved. Please choose a different abbreviation.";
            }
            // Location with same name already exists
            if (_context.Location.Any(loc => loc.Name == l.Name && loc.LocationId != l.LocationId))
            {
                Location existingLocation = _context.Location.First(loc => loc.Name == l.Name);
                return "Error: A location with this name already exists: " + existingLocation.Name + " (" + existingLocation.Abbreviation + ")";
            }
            // Location with same abbreviation already exists
            if (_context.Location.Any(loc => loc.Abbreviation == l.Abbreviation.ToUpper() && loc.LocationId != l.LocationId))
            {
                Location existingLocation = _context.Location.First(loc => loc.Name == l.Name);
                return "Error: A location with this abbreviation already exists: " + existingLocation.Name + " (" + existingLocation.Abbreviation + ")";
            }

            return "";
        }
        /*
         * Validate a given category. 
         * Returns an empty string if all checks passed
         * Return an error string if a check fails
         */
        public string ValidateCategory(Category c)
        {
            // Perform input validation. Redirect back to the form with an error message if the input is invalid.
            // Copilot:
            // Empty or whitespace name/abbreviation
            if (string.IsNullOrWhiteSpace(c.Name) || string.IsNullOrWhiteSpace(c.Abbreviation))
            {
                return "Error: Name and Abbreviation cannot be empty.";
            }
            // Abbreviation too long
            if (c.Abbreviation.Length > 10)
            {
                return "Error: Abbreviation cannot be longer than 10 characters.";
            }
            // Abbreviation contains whitespace
            if (c.Abbreviation.Any(char.IsWhiteSpace))
            {
                return "Error: Abbreviation cannot contain whitespace.";
            }
            // Category uses reserved name
            if (c.Name.Equals("Unassigned"))
            {
                return "Error: The name \"Unassigned\" is reserved. Please choose a different name.";
            }
            // Category uses reserved abbreviation
            if (c.Abbreviation.ToUpper().Equals("UN"))
            {
                return "Error: The abbreviation \"UN\" is reserved. Please choose a different abbreviation.";
            }
            // Category with same name already exists
            if (_context.Category.Any(cat => cat.Name == c.Name && cat.CategoryId != c.CategoryId))
            {
                Category existingCategory = _context.Category.First(cat => cat.Name == c.Name);
                return "Error: A category with this name already exists: " + existingCategory.Name + " (" + existingCategory.Abbreviation + ")";
            }
            // Category with same abbreviation already exists
            if (_context.Category.Any(cat => cat.Abbreviation == c.Abbreviation.ToUpper() && cat.CategoryId != c.CategoryId))
            {
                Category existingCategory = _context.Category.First(cat => cat.Name == c.Name && cat.CategoryId != c.CategoryId);
                return "Error: A category with this abbreviation already exists: " + existingCategory.Name + " (" + existingCategory.Abbreviation + ")";
            }
            // Negative default loan period
            if (c.DefaultLoanPeriodDays < 0)
            {
                return "Error: Default Loan Period cannot be negative.";
            }

            return "";
        }
    }
}
