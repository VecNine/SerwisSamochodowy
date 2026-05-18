using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerwisSamochodowy.Models;

namespace SerwisSamochodowy.Controllers;

[Route("Users")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "True";

    [HttpGet("/Login")]
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View("~/Views/Login/Index.cshtml");
    }

    [HttpPost("/Login")]
    [HttpPost("Login")]
    public IActionResult Login(string login, string password)
    {
        string hashedPassword = ApplicationDbContext.GetMd5Hash(password);
        var user = _context.Users.FirstOrDefault(u => u.Username == login && u.PasswordHash == hashedPassword);
    
        if (user != null)
        {
            HttpContext.Session.SetString("User", user.Username);
            HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());
            
            return RedirectToAction("Index", "Management"); 
        }

        ViewBag.Message = "Błędny login lub hasło";
        return View("~/Views/Login/Index.cshtml");
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    
    
    // --- ZARZĄDZANIE UŻYTKOWNIKAMI (WEB) ---

    [HttpGet("List")]
    public IActionResult List()
    {
        if (!IsAdmin()) return Unauthorized("Brak uprawnień.");
        return View(_context.Users.ToList());
    }

    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
        if (!IsAdmin()) return Unauthorized("Brak uprawnień.");
        var user = _context.Users.Find(id);
        return user == null ? NotFound() : View(user);
    }

    [HttpPost("Edit/{id}")]
    public IActionResult Edit(int id, string username, string? password, bool isAdmin)
    {
        if (!IsAdmin()) return Unauthorized("Brak uprawnień.");
        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        user.Username = username;
        user.IsAdmin = isAdmin;
        if (!string.IsNullOrEmpty(password)) 
            user.PasswordHash = ApplicationDbContext.GetMd5Hash(password);

        _context.SaveChanges();
        return RedirectToAction("List");
    }

    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True") 
            return RedirectToAction("List");

        var user = _context.Users.Find(id);
        if (user == null) return NotFound();

        return View(user);
    }
    
    [HttpPost("DeleteConfirmed/{id}")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True") 
            return RedirectToAction("List");

        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
        
        return RedirectToAction("List");
    }
    
    [HttpGet("Create")]
    public IActionResult Create()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
        {
            return RedirectToAction("List"); 
        }

        return View();
    }
    
    [HttpPost("Create")]
    public IActionResult Create(string username, string password, bool isAdmin) 
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True")
        {
            return RedirectToAction("Login");
        }

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ViewBag.Message = "Błąd: Login i hasło nie mogą być puste.";
            return View();
        }

        var newUser = new Users
        {
            Username = username,
            PasswordHash = ApplicationDbContext.GetMd5Hash(password),
            IsAdmin = isAdmin,
            ApiToken = Guid.NewGuid().ToString()
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();
        
        return RedirectToAction("List");
    }
    

    private bool AuthenticateApi()
    {
        string? userHeader = Request.Headers["X-Username"];
        string? tokenHeader = Request.Headers["X-Api-Token"];
        return _context.Users.Any(u => u.Username == userHeader && u.ApiToken == tokenHeader);
    }

    [HttpGet("api/get")]
    public IActionResult ApiGet()
    {
        if (!AuthenticateApi()) return Unauthorized();
        return Json(_context.Users.Select(u => new { u.IdUser, u.Username, u.IsAdmin }).ToList());
    }

    [HttpPost("api/create")]
    public IActionResult ApiCreate([FromBody] Users newUser)
    {
        if (!AuthenticateApi()) return Unauthorized();
        newUser.ApiToken = Guid.NewGuid().ToString();
        newUser.PasswordHash = ApplicationDbContext.GetMd5Hash(newUser.PasswordHash);
        _context.Users.Add(newUser);
        _context.SaveChanges();
        return Ok(newUser);
    }
}