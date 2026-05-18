using Microsoft.EntityFrameworkCore;
using SerwisSamochodowy.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseSession(); 

app.MapControllerRoute(
    name: "default", 
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    context.Database.EnsureCreated();
    
    if (!context.Users.Any())
    {
        var adminUser = new Users
        {
            Username = "admin",
            PasswordHash = ApplicationDbContext.GetMd5Hash("admin123"),
            IsAdmin = true,
            ApiToken = Guid.NewGuid().ToString()
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

app.Run();