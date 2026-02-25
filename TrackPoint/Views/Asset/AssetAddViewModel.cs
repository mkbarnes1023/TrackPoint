using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrackPoint.Models;

namespace TrackPoint.Views.Asset
{
    public class AssetAddViewModel
    {
        public IEnumerable<Location> _locations;
        public IEnumerable<Category> _categories;
        public IEnumerable<IdentityUser> _users;
        public Models.Asset asset { get; set; } = new Models.Asset();
    }
}
