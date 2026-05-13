using Microsoft.EntityFrameworkCore;
using SerwisSamochodowy.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja bazy danych SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Dodanie obsługi sesji
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. Włączenie sesji w potoku HTTP
app.UseSession(); 

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();