using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TrackPoint.Controllers
{
    public class ReportController : Controller
    {

        public IActionResult ReportBuilder() => View();
    }
}
