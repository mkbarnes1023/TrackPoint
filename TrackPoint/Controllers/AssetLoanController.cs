using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrackPoint.Data;
using TrackPoint.Models;
using TrackPoint.Views.Asset;

namespace TrackPoint.Controllers
{
    public class AssetLoanController : Controller
    {
        ApplicationDbContext _context;
        public AssetLoanController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult AssetLoanTest()
        {
            var loanModel = new AssetLoanTestViewModel();
            {
                    loanModel._assetLoans = _context.Assetloan.ToList();
            }
            return View(loanModel);
        }
    }
}
