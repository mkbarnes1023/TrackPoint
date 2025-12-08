using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;

var builder = WebApplication.CreateBuilder(args);

// DB context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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