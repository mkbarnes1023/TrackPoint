using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Notifications()
        {
            return View();
        }

        [HttpGet]
        [Route("api/notifications/list")]
        public async Task<IActionResult> GetUserNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var notifications = await _context.Notification
                .Where(n => n.userId == user.Id)
                .OrderByDescending(n => n.createdAt)
                .ToListAsync();

            return Json(notifications);
        }

        [HttpPost]
        [Route("api/notifications/{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var notification = await _context.Notification
                .FirstOrDefaultAsync(n => n.notificationId == id && n.userId == user.Id);

            if (notification == null)
            {
                return NotFound();
            }

            notification.readAt = DateTime.UtcNow;
            _context.Notification.Update(notification);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("api/notifications/{id}/delete")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var notification = await _context.Notification
                .FirstOrDefaultAsync(n => n.notificationId == id && n.userId == user.Id);

            if (notification == null)
            {
                return NotFound();
            }

            _context.Notification.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("api/notifications/mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var notifications = await _context.Notification
                .Where(n => n.userId == user.Id)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.readAt = DateTime.UtcNow;
                _context.Notification.Update(notification);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("api/notifications/delete-all")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var notifications = await _context.Notification
                .Where(n => n.userId == user.Id)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                _context.Notification.Remove(notification);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
