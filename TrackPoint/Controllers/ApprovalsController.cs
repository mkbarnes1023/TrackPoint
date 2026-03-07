using Microsoft.AspNetCore.Mvc;
using TrackPoint.Data;
using TrackPoint.Views.Approvals;

namespace TrackPoint.Controllers
{
    public class ApprovalsController : Controller
    {
        ApplicationDbContext _context;
        public ApprovalsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var approvalsModel = new ApprovalsTestViewModel();
            {
                approvalsModel._approvals = _context.Approvals.ToList();
            }
            return View(approvalsModel);
        }
    }
}
