using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrackPoint.Configuration;
using TrackPoint.Data;

var builder = WebApplication.CreateBuilder(args);

// DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StevenVM")));

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
        var adminEmails = config.GetSection("Seed:AdminEmails").Get<string[]>();

        var user = await userManager.GetUserAsync(ctx.Principal);
        if (user is null)
        {
            return;
        }

        // Skip *legacy* single-admin logic; role seeding is handled separately now
        if (adminEmails != null && adminEmails.Length > 0)
        {
            foreach (var adminEmail in adminEmails)
            {
                if (string.Equals(user.Email, adminEmail, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
        }

        // Ensure Borrower role for everyone else
        if (!await userManager.IsInRoleAsync(user, "Borrower"))
        {
            // Throwing error on login: Role BORROWER does not exist
            //await userManager.AddToRoleAsync(user, "Borrower");
        }
    };
});

builder.Services.Configure<SeedOptions>(
    builder.Configuration.GetSection(SeedOptions.SectionName));

var app = builder.Build();

/* Copilot:
// Ensure database exists and is up to date BEFORE seeding roles/users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    // Run data seeding that requires the DbContext
    await TrackPoint.Data.SeedData.Seed.InitializeAsync(db);
}
*/

// Ensure database exists and is up to date BEFORE seeding roles/users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

// Seed admins from appsettings.json using AdminSeedData
await AdminSeedData.SeedAdminsAsync(app.Services);

// Optionally: additional data seeding spot
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     // app data seeding here
// }

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

await app.RunAsync();