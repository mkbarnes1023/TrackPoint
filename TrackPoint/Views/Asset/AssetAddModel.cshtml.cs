using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrackPoint.Models;

namespace TrackPoint.Views.Asset
{
    public class AssetAddModel : PageModel
    {
        public IEnumerable<Location> _locations;
        public IEnumerable<Category> _categories;
        public Models.Asset asset { get; set; }
    }
}
