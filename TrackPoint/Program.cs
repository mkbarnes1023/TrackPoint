using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;
using TrackPoint.Models.RepositoryInterfaces;
using TrackPoint.Models.Repositories.Local;

var builder = WebApplication.CreateBuilder(args);

// DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // In Development, enable EF Core logging and sensitive data logging so connection
    // errors and SQL details are written to the console for easier debugging.
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
    }
});

// Removed SMTP email sender configuration

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Consider requiring confirmed account to force email verification flows
    options.SignIn.RequireConfirmedAccount = false;

    // Optional: make reset tokens valid for a reasonable time
    // Reset to defaults
})
.AddDefaultUI()
.AddEntityFrameworkStores<ApplicationDbContext>();

// MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Account");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ReturnUrlParameter = "returnUrl";
});

builder.Services.AddScoped<IAssetRepository, AssetLocalRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryLocalRepository>();
builder.Services.AddScoped<ILocationRepository, LocationLocalRepository>();

var app = builder.Build();

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