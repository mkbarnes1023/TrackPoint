using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace TrackPoint.Services
{
    public interface IEntraUserProvisioningService
    {
        Task<IdentityUser> GetOrCreateUserFromEntraAsync(ClaimsPrincipal principal);
    }

    public class EntraUserProvisioningService : IEntraUserProvisioningService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<EntraUserProvisioningService> _logger;

        public EntraUserProvisioningService(
            UserManager<IdentityUser> userManager,
            ILogger<EntraUserProvisioningService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IdentityUser> GetOrCreateUserFromEntraAsync(ClaimsPrincipal principal)
        {
            var oid = principal.FindFirstValue("oid")
                      ?? principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
            var email = principal.FindFirstValue(ClaimTypes.Email)
                        ?? principal.FindFirstValue("preferred_username")
                        ?? principal.FindFirstValue("upn");
            var name = principal.FindFirstValue(ClaimTypes.Name)
                       ?? principal.FindFirstValue("name");

            if (string.IsNullOrEmpty(oid))
            {
                _logger.LogError("Entra OID claim not found in principal");
                throw new InvalidOperationException("Entra OID claim not found. Cannot provision user.");
            }

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("Email claim not found for user with OID: {Oid}", oid);
                throw new InvalidOperationException("Email claim not found. Cannot provision user.");
            }

            _logger.LogInformation("Processing Entra user: OID={Oid}, Email={Email}, Name={Name}", oid, email, name);

            // Check if user already exists by Entra OID (via AspNetUserLogins)
            var user = await _userManager.FindByLoginAsync("AzureAD", oid);

            if (user == null)
            {
                _logger.LogInformation("User with OID {Oid} not found in UserLogins. Checking by email...", oid);

                // Check if user exists by email (for migration scenario)
                user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogInformation("Creating new user for email: {Email}", email);

                    // Create new user
                    user = new IdentityUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to create user {Email}: {Errors}", email, errors);
                        throw new InvalidOperationException($"Failed to create user: {errors}");
                    }

                    _logger.LogInformation("Successfully created user {Email} with Id {UserId}", email, user.Id);
                }
                else
                {
                    _logger.LogInformation("Found existing user by email: {Email}, Id: {UserId}", email, user.Id);
                }

                // Link Entra OID to this user via AspNetUserLogins
                var loginInfo = new UserLoginInfo("AzureAD", oid, "Microsoft Entra ID");
                var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);

                if (!addLoginResult.Succeeded)
                {
                    var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to link Entra OID {Oid} to user {UserId}: {Errors}", oid, user.Id, errors);
                    throw new InvalidOperationException($"Failed to link Entra account: {errors}");
                }

                _logger.LogInformation("Successfully linked Entra OID {Oid} to user {UserId}", oid, user.Id);
            }
            else
            {
                _logger.LogInformation("Found existing user via Entra OID {Oid}: UserId={UserId}", oid, user.Id);
            }

            return user;
        }
    }
}
