using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TrackPoint.Configuration;
using TrackPoint.Data;
using TrackPoint.Services;

var builder = WebApplication.CreateBuilder(args);

// DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Authentication: Entra ID (OIDC) + Identity (for user storage)
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        
        // Transform Entra role claims to ClaimTypes.Role
        options.TokenValidationParameters.RoleClaimType = "roles";
        
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async context =>
            {
                // Provision user in local AspNetUsers table
                var provisioningService = context.HttpContext.RequestServices
                    .GetRequiredService<IEntraUserProvisioningService>();
                
                var user = await provisioningService.GetOrCreateUserFromEntraAsync(context.Principal!);
                
                // Add user ID claim for Identity integration
                var identity = context.Principal!.Identity as ClaimsIdentity;
                identity?.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                
                // Log successful authentication
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("User {Email} authenticated via Entra ID with UserId {UserId}", 
                    user.Email, user.Id);
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Entra ID authentication failed");
                return Task.CompletedTask;
            }
        };
    });

// Add Identity for user management (storage only, not for authentication)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    // Password not used for Entra users, but keep for potential local accounts
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>() // Keep roles infrastructure for potential future use
.AddEntityFrameworkStores<ApplicationDbContext>();

// Register provisioning service
builder.Services.AddScoped<IEntraUserProvisioningService, EntraUserProvisioningService>();

// Authorization - uses role claims from Entra ID
builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI(); // Add Microsoft Identity UI for sign-in/sign-out

builder.Services.AddRazorPages();

builder.Services.Configure<SeedOptions>(
    builder.Configuration.GetSection(SeedOptions.SectionName));

var app = builder.Build();

// Ensure database exists and is up to date
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

// NOTE: Local role seeding removed - roles now come from Entra ID app roles
// Users are provisioned on first sign-in via EntraUserProvisioningService

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", async context =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        context.Response.Redirect("/Home/Index");
    }
    else
    {
        // Redirect to Entra ID login
        context.Response.Redirect("/MicrosoftIdentity/Account/SignIn");
    }
    await Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await app.RunAsync();