using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web.UI;
using TrackPoint.Configuration;
using TrackPoint.Data;
using TrackPoint.Data.SeedData;
using TrackPoint.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ?? Database ??????????????????????????????????????????????????????????????????
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StevenVM")));

// ?? Authentication / Authorization ???????????????????????????????????????????
// Entra ID (OIDC), Identity Core (user/role storage), provisioning, authorization.
// See TrackPoint/Extensions/AuthenticationExtensions.cs for full documentation.
builder.Services.AddTrackPointAuthentication(builder.Configuration);

// ?? MVC / Razor Pages ?????????????????????????????????????????????????????????
// AddMicrosoftIdentityUI registers the /MicrosoftIdentity/Account/SignIn|SignOut
// controller endpoints used by _LoginPartial and the root redirect.
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddRazorPages();

builder.Services.Configure<SeedOptions>(
    builder.Configuration.GetSection(SeedOptions.SectionName));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    // Run data seeding that requires the DbContext
    await TrackPoint.Data.SeedData.Seed.InitializeAsync(db);
}

// Seed admins from appsettings.json using AdminSeedData
await AdminSeedData.SeedAdminsAsync(app.Services);

// Optionally: additional data seeding spot
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // app data seeding here
    //await Seed.InitializeAsync(db);
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
        context.Response.Redirect("/MicrosoftIdentity/Account/SignIn");
    }
    await Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

await app.RunAsync();