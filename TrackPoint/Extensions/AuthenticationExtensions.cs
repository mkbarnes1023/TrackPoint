using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using TrackPoint.Data;
using TrackPoint.Services;

namespace TrackPoint.Extensions;

public static class AuthenticationExtensions
{
    // Non-static marker used as the ILogger<T> category for this extension.
    private sealed class TrackPointAuth { }
    /// <summary>
    /// Registers all TrackPoint authentication and authorization services:
    ///   1. Entra ID (OIDC) via Microsoft.Identity.Web
    ///   2. ASP.NET Core Identity Core (user/role storage only — no scheme conflicts)
    ///   3. Entra user provisioning service
    ///   4. Authorization
    /// </summary>
    public static IServiceCollection AddTrackPointAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ?? 1. Entra ID / OpenID Connect ?????????????????????????????????????
        // AddMicrosoftIdentityWebApp sets the following defaults so nothing else should override them:
        //   DefaultScheme             = "Cookies"   ? reads the session cookie on every request
        //   DefaultChallengeScheme    = "OpenIdConnect"
        //   DefaultSignInScheme       = "Cookies"
        //   DefaultAuthenticateScheme = "Cookies"
        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(options =>
            {
                configuration.Bind("AzureAd", options);

                // ?? 1a. Authorization Code Flow (PKCE) ????????????????????????
                // Prevents implicit/hybrid flow, which would require "ID tokens"
                // to be enabled on the App Registration. Code flow is more secure
                // and avoids the SameSite cookie issue (see 1b).
                options.ResponseType = OpenIdConnectResponseType.Code;

                // ?? 1b. SameSite Cookie Fix ???????????????????????????????????
                // Microsoft.Identity.Web forces response_mode=form_post, which is
                // a cross-site POST from login.microsoftonline.com back to the app.
                // SameSite=Lax (the default) blocks cookies on cross-site POSTs,
                // causing the OIDC correlation check to fail and creating a
                // redirect loop. SameSite=None;Secure allows the callback to work.
                options.CorrelationCookie.SameSite = SameSiteMode.None;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                options.NonceCookie.SameSite = SameSiteMode.None;
                options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;

                // ?? 1c. Role Claim Mapping ????????????????????????????????????
                // Microsoft.Identity.Web maps Entra app role claims to ClaimTypes.Role
                // (http://schemas.microsoft.com/ws/2008/06/identity/claims/role).
                // Setting RoleClaimType here ensures [Authorize(Roles = "Admin")]
                // resolves correctly against that claim type.
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;

                // ?? 1d. OIDC Event Handlers ???????????????????????????????????
                options.Events = BuildOidcEvents();
            });

        // ?? 2. ASP.NET Core Identity (user/role storage only) ?????????????????
        // AddIdentityCore does NOT register or override any authentication schemes.
        // ? Do NOT use AddDefaultIdentity here — it overrides DefaultAuthenticateScheme
        //   to "Identity.Application", which conflicts with the "Cookies" scheme that
        //   AddMicrosoftIdentityWebApp writes after a successful OIDC sign-in.
        //   That conflict causes every request to fail authentication ? redirect loop.
        services.AddIdentityCore<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            // Passwords are not used for Entra-authenticated users; these are relaxed
            // in case local accounts are ever needed for tooling or seeding.
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            
            // ?? Explicitly set claim type for user ID resolution ??????????????????
            // Ensures GetUserId() returns the local AspNetUsers ID, not the Entra OID
            options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        // ?? 3. Entra User Provisioning ????????????????????????????????????????
        // Creates or retrieves a local AspNetUsers record for each Entra sign-in.
        // Only runs for users who pass the role gate in OnTokenValidated (step 1d).
        services.AddScoped<IEntraUserProvisioningService, EntraUserProvisioningService>();

        // ?? 4. Authorization ??????????????????????????????????????????????????
        // Default policy: authenticated users only.
        // Fine-grained access is controlled via [Authorize(Roles = "Admin,Borrower")]
        // on individual controllers and pages.
        services.AddAuthorization();

        return services;
    }

    // ?? OIDC Event Handlers ???????????????????????????????????????????????????

    private static OpenIdConnectEvents BuildOidcEvents() => new()
    {
        // Fires after the ID token has been validated. Used to:
        //   a) enforce that the user has a pre-assigned TrackPoint app role, and
        //   b) provision (or look up) the corresponding local Identity user record.
        OnTokenValidated = async context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<TrackPointAuth>>();

            // ?? Role gate ?????????????????????????????????????????????????????
            // App roles are assigned in the Entra Enterprise Application.
            // Users with only "Default Access" (no custom role) have an empty
            // ClaimTypes.Role collection and are blocked here before any local
            // record is created. "Assignment required = Yes" in the Enterprise
            // Application properties blocks unassigned users at the Entra level.
            var roleClaims = context.Principal!
                .FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            string[] allowedRoles = ["Admin", "Borrower"];

            if (!roleClaims.Any(r => allowedRoles.Contains(r, StringComparer.OrdinalIgnoreCase)))
            {
                logger.LogWarning(
                    "Sign-in denied: {Email} has no valid TrackPoint role. Present: [{Roles}]",
                    context.Principal.FindFirstValue("preferred_username"),
                    string.Join(", ", roleClaims.DefaultIfEmpty("none")));

                context.Fail(
                    "You do not have an assigned role in TrackPoint. " +
                    "Contact your administrator to be granted access.");
                return;
            }

            // ?? Local user provisioning ???????????????????????????????????????
            // Looks up the local AspNetUsers record by Entra OID (via AspNetUserLogins),
            // falling back to email, and creates it on first sign-in if not found.
            // The local ID is injected into the principal so controllers can resolve
            // it via User.FindFirstValue(ClaimTypes.NameIdentifier).
            var provisioningService = context.HttpContext.RequestServices
                .GetRequiredService<IEntraUserProvisioningService>();

            var user = await provisioningService.GetOrCreateUserFromEntraAsync(context.Principal!);

            var identity = context.Principal!.Identity as ClaimsIdentity;
            
            // ?? CRITICAL: Remove existing NameIdentifier claim (the Entra OID) before adding local ID
            var existingNameIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);
            if (existingNameIdClaim != null)
            {
                identity?.RemoveClaim(existingNameIdClaim);
            }
            
            identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            logger.LogInformation(
                "User {Email} authenticated via Entra ID. Local ID: {UserId}. Roles: [{Roles}]",
                user.Email, user.Id, string.Join(", ", roleClaims));
        },

        // Fires when any part of the OIDC flow fails (including context.Fail() above).
        // Redirects to an appropriate page rather than showing a raw exception.
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<TrackPointAuth>>();

            // context.Fail() from OnTokenValidated wraps the message in an
            // AuthenticationFailureException — inspect the inner exception to distinguish
            // an intentional role-gate denial from an unexpected OIDC error.
            var isAccessDenied = context.Exception?.InnerException?.Message
                ?.Contains("do not have an assigned role") == true;

            logger.LogWarning(context.Exception,
                isAccessDenied
                    ? "Access denied — user has no valid TrackPoint app role"
                    : "Entra ID authentication failed");

            context.HandleResponse();
            context.Response.Redirect(isAccessDenied
                ? "/Identity/Account/AccessDenied"
                : "/error?message=auth_failed");

            return Task.CompletedTask;
        }
    };
}