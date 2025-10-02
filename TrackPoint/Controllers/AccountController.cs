using Microsoft.AspNetCore.Mvc;
using TrackPoint.Models;

namespace TrackPoint.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Placeholder for authentication logic
            if (model.Username == "admin" && model.Password == "password")
            {
                // TODO: Set authentication cookie/session
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: Save user to database or user store
            // For now, just redirect to login or home
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: Implement password reset email logic here.
            // For now, just show a confirmation message.
            ViewBag.Message = "If an account with that email exists, a password reset link has been sent.";
            return View();
        }
    }
}
