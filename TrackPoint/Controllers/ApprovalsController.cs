using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrackPoint.Data;
using TrackPoint.Models;
using TrackPoint.Views.Approvals;

namespace TrackPoint.Controllers
{
    public class ApprovalsController : Controller
    {
        ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ApprovalsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult ApprovalsTest()
        {
            var approvalsModel = new ApprovalsTestViewModel();
            {
                approvalsModel._approvals = _context.Approvals.ToList();
            }
            return View(approvalsModel);
        }

        public IActionResult ApproveAsset(int ApprovalId)
        {
            // Find the Approval given the ID
            var Approval = _context.Approvals.FirstOrDefault(a => a.ApprovalId == ApprovalId);
            if (Approval == null)
            {
                Console.WriteLine($"\n\n\nERROR: Approval {ApprovalId} not found\n\n\n");
                return RedirectToAction("ApprovalsTest");
            }

            // Check Out Asset to borrower
            var asset = _context.Asset.FirstOrDefault(a => a.AssetId == Approval.AssetId);
            if (asset == null)
            {
                Console.WriteLine($"\n\n\nERROR: Asset {Approval.AssetId} not found\n\n\n");
                return RedirectToAction("ApprovalsTest");
            }

            // Update Asset properties
            asset.IssuedToUserId = Approval.RequestorId;
            asset.StatusDate = DateTime.Now;
            asset.AssetStatus = "InUse";

            // Update AssetLoan for Check Out
            var currentUserId = _userManager.GetUserId(User);
            var currentUser = _userManager.Users.FirstOrDefault(u => u.Id == currentUserId);
            _context.Assetloan.Add(new AssetLoan
            {
                    AssetId = asset.AssetId,
                    BorrowerId = Approval.RequestorId,
                    CheckedoutDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(_context.Category.Find(asset.CategoryId)?.DefaultLoanPeriodDays ?? 14),
                    ReturnedDate = null,
                    ExtendedByAdminId = null,
                    ExtendedBy = null,
                    ApprovedByUserId = Approval.RequestorId,
                    ApprovedBy = currentUser
            });

            // Update TransferLog for Check Out
            _context.TransferLog.Add(new TransferLog
            {
                AssetId = asset.AssetId,
                OldBorrowerId = null,
                NewBorrowerId = asset.IssuedToUserId,
                NewStatus = "InUse",
                OldStatus = null,
                eventType = Enums.eventType.BorrowerTransfer,
                TransferDate = DateTime.Now
            });

            // Remove the approval request
            _context.Approvals.Remove(Approval); // TODO: I don't think this should remove the entry entirely, but the approval is "done" at this point
            _context.SaveChanges();
            TempData["Success"] = $"Request {ApprovalId} has been approved.";
            return RedirectToAction("ApprovalsTest");
        }

        public IActionResult RejectAsset(int ApprovalId)
        {
            var Approval = _context.Approvals.FirstOrDefault(a => a.ApprovalId == ApprovalId);
            if (Approval == null)
            {
                Console.WriteLine($"\n\n\nERROR: Approval {ApprovalId} not found\n\n\n");
                return RedirectToAction("ApprovalsTest");
            }
            // Remove the approval without modifying any data in the Asset table
            _context.Approvals.Remove(Approval);
            _context.SaveChanges();

            TempData["Success"] = $"Request {ApprovalId} has been rejected.";
            return RedirectToAction("ApprovalsTest");
        }
    }
}
