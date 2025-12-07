using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrackPoint.Pages.Account;

[Authorize]
public class SettingsModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public SettingsModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Display-only account info
    public string? UserName { get; private set; }
    public string? Email { get; private set; }

    [BindProperty]
    public ChangePasswordInput Input { get; set; } = new();

    public class ChangePasswordInput
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge(); // Not logged in

        UserName = await _userManager.GetUserNameAsync(user);
        Email = await _userManager.GetEmailAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadUserInfoAsync();
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var result = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            await LoadUserInfoAsync();
            return Page();
        }

        // Re-sign in to refresh security stamp
        await _signInManager.RefreshSignInAsync(user);
        TempData["StatusMessage"] = "Your password has been changed.";
        return RedirectToPage();
    }

    private async Task LoadUserInfoAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        UserName = user != null ? await _userManager.GetUserNameAsync(user) : null;
        Email = user != null ? await _userManager.GetEmailAsync(user) : null;
    }
}