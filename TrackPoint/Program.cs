using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;
//using TrackPoint.Models.RepositoryInterfaces;
//using TrackPoint.Models.Repositories.Local;
using System;
using System.Linq;
using TrackPoint.Models;

var builder = WebApplication.CreateBuilder(args);

// DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity with roles enabled
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>() // enable roles
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Authorization (no restrictive policies here)
builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Account"); // OK
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ReturnUrlParameter = "returnUrl";

    // Auto-assign Borrower to any non-admin user on first sign-in
    options.Events.OnSignedIn = async ctx =>
    {
        var userManager = ctx.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
        var config = ctx.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var adminEmail = config["Seed:AdminEmail"];

        var user = await userManager.GetUserAsync(ctx.Principal);
        if (user is null) return;

        // Skip the configured admin
        if (!string.IsNullOrWhiteSpace(adminEmail) &&
            string.Equals(user.Email, adminEmail, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Ensure Borrower role for everyone else
        if (!await userManager.IsInRoleAsync(user, "Borrower"))
        {
            await userManager.AddToRoleAsync(user, "Borrower");
        }
    };
});

//builder.Services.AddScoped<IAssetRepository, AssetLocalRepository>();
//builder.Services.AddScoped<ICategoryRepository, CategoryLocalRepository>();
//builder.Services.AddScoped<ILocationRepository, LocationLocalRepository>();

var app = builder.Build();

// Ensure database exists and is up to date BEFORE seeding roles/users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

// Seed roles and enforce: one Admin, everyone else Borrower
using (var scope = app.Services.CreateScope())
{
    try
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Ensure roles exist
        foreach (var role in new[] { "Admin", "Borrower" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var createResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create role '{role}': {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }
        }

        // Configured admin email
        var adminEmail = config["Seed:AdminEmail"] ?? "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser != null)
        {
            // Ensure admin has Admin role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var addResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (!addResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to add Admin role to '{adminEmail}': {string.Join(", ", addResult.Errors.Select(e => e.Description))}");
                }
            }
        }
        else
        {
            Console.WriteLine($"Seed:AdminEmail user not found: {adminEmail}");
        }

        // Backfill everyone else: Borrower yes, Admin no
        var allUsers = await userManager.Users.ToListAsync();
        foreach (var u in allUsers)
        {
            var isConfiguredAdmin = !string.IsNullOrWhiteSpace(adminEmail) &&
                                    string.Equals(u.Email, adminEmail, StringComparison.OrdinalIgnoreCase);

            if (isConfiguredAdmin)
            {
                continue;
            }

            // Ensure Borrower
            if (!await userManager.IsInRoleAsync(u, "Borrower"))
            {
                await userManager.AddToRoleAsync(u, "Borrower");
            }

            // Ensure not Admin
            if (await userManager.IsInRoleAsync(u, "Admin"))
            {
                await userManager.RemoveFromRoleAsync(u, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        // Emit detailed startup errors to help diagnose configuration issues
        Console.Error.WriteLine($"Startup seeding failed: {ex}");
        throw;
    }
}

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
        context.Response.Redirect("/Identity/Account/Login?returnUrl=/");
    }
    await Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();