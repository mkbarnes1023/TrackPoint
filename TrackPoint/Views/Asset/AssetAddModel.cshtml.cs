using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrackPoint.Models;

namespace TrackPoint.Views.Asset
{
    public class AssetAddModel : PageModel
    {
        public readonly IEnumerable<Location> _locations;
        public readonly IEnumerable<Category> _categories;
        public Models.Asset asset;
        public AssetAddModel(IEnumerable<Location> locations, IEnumerable<Category> categories)
        {
            _locations = locations;
            _categories = categories;
        }
    }
}
