using CateringMvc.Data;
using CateringMvc.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services.AddControllersWithViews();
// SQLite: database ek file (catering.db) hai, exe ke saath wale folder mein. Kuch install nahi karna.
var dbPath = Path.Combine(AppContext.BaseDirectory, "catering.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

// The database file and tables are created on first run, and default prices are added.
// (If you change the model later, delete catering.db to rebuild it - or tell me and I will add migrations.)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.PriceSettings.Any())
    {
        db.PriceSettings.Add(new PriceSetting());
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
