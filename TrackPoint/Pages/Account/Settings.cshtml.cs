using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrackPoint.Pages.Account;

[Authorize]
public class SettingsModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public SettingsModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public string? UserName { get; private set; }
    public string? Email { get; private set; }
    public bool IsEntraUser { get; private set; }
    public IList<string> Roles { get; private set; } = new List<string>();

    public async Task<IActionResult> OnGetAsync()
    {
        // Get user info from claims (Entra provides these)
        Email = User.FindFirstValue(ClaimTypes.Email) 
                ?? User.FindFirstValue("preferred_username");
        UserName = User.FindFirstValue(ClaimTypes.Name) ?? Email;
        
        // Check if this is an Entra user (has oid claim)
        IsEntraUser = User.FindFirstValue("oid") != null 
                      || User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier") != null;

        // Get roles from claims (Entra provides these in "roles" claim)
        Roles = User.FindAll("roles").Select(c => c.Value).ToList();
        if (!Roles.Any())
        {
            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        }

        return Page();
    }
}