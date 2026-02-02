using TrackPoint.Models;

namespace TrackPoint.Views.Asset
{
    public class AssetBrowserViewModel
    {
        public IEnumerable<Location> _locations;
        public IEnumerable<Category> _categories;
        public IEnumerable<Models.Asset> _assets;
    }
}
