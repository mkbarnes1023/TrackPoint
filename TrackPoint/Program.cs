using Microsoft.EntityFrameworkCore;
using TrackPoint.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC only
builder.Services.AddControllersWithViews();

// EF Core (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// NOTE: Identity and authorization removed. All endpoints are now anonymous.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
 app.UseExceptionHandler("/Home/Error");
 app.UseHsts();
}

app.UseHttpsRedirection();

// Serve static files (CSS/JS)
app.UseStaticFiles();

app.UseRouting();

// Removed: app.UseAuthentication();
// Removed: app.UseAuthorization();

// Standard MVC route
app.MapControllerRoute(
 name: "default",
 pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
