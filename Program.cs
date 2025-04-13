using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;

var builder = WebApplication.CreateBuilder(args);

// 🧱 Dodanie DbContext i Identity przed builder.Build()
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=befit.db"));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// 💻 Dodanie MVC i Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Konfiguracja Kestrel do używania HTTP
builder.WebHost.UseUrls("http://localhost:5207");

var app = builder.Build();

// 🧪 Inicjalizacja danych testowych
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.Seed(services);
}

// 🔧 Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// 📍 Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
