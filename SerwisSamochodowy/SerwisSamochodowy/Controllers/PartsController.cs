using Microsoft.AspNetCore.Mvc;
using SerwisSamochodowy.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SerwisSamochodowy.Controllers;

[Route("Parts")]
public class PartsController : Controller
{
    private readonly ApplicationDbContext _context;

    public PartsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    private bool IsAuthorized() => !string.IsNullOrEmpty(HttpContext.Session.GetString("User"));

    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        return View(_context.Parts.ToList());
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        return View();
    }

    [HttpPost("Create")]
    public IActionResult Create(Parts part)
    {
        if (!IsAuthorized()) return Unauthorized();

        if (ModelState.IsValid)
        {
            _context.Parts.Add(part);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(part);
    }

    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");

        var part = _context.Parts.Find(id);
        if (part == null) return NotFound();

        return View(part);
    }

    [HttpPost("Edit/{id}")]
    public IActionResult Edit(int id, Parts updatedPart)
    {
        if (!IsAuthorized()) return Unauthorized();

        if (ModelState.IsValid)
        {
            var partInDb = _context.Parts.Find(id);
            if (partInDb == null) return NotFound();
            
            partInDb.Name = updatedPart.Name;
            partInDb.Cost = updatedPart.Cost; 

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(updatedPart);
    }

    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
        {
            return RedirectToAction("Login", "Users");
        }

        var part = _context.Parts.Find(id);
        if (part == null)
        {
            return NotFound();
        }

        return View(part);
    }
    
    [HttpPost("DeleteConfirmed/{id}")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
        {
            return Unauthorized();
        }

        var part = _context.Parts.Find(id);
        if (part != null)
        {
            _context.Parts.Remove(part);
            _context.SaveChanges();
        }
    
        return RedirectToAction("Index");
    }
    
    private bool AuthenticateApi() {
        var user = _context.Users.FirstOrDefault(u => u.Username == Request.Headers["X-Username"] && u.ApiToken == Request.Headers["X-Api-Token"]);
        return user != null;
    }
    private bool AuthenticateApiAsAdmin() {
        string user = Request.Headers["X-Username"].ToString();
        string token = Request.Headers["X-Api-Token"].ToString();
        return _context.Users.Any(u => u.Username == user && u.ApiToken == token && u.IsAdmin);
    }

    [HttpGet("api/get")]
    public IActionResult ApiGet() {
        string user = Request.Headers["X-Username"].ToString();
        string token = Request.Headers["X-Api-Token"].ToString();
        if (!_context.Users.Any(u => u.Username == user && u.ApiToken == token)) return StatusCode(401);
        
        return Json(_context.Parts.Select(p => new { p.IdPart, p.Name, p.Cost }).ToList());
    }

    [HttpPost("api/add")]
    public IActionResult ApiAdd([FromBody] Parts p) {
        if (!AuthenticateApiAsAdmin()) return StatusCode(401, "Brak uprawnień Admina.");
        var newPart = new Parts { Name = p.Name, Cost = p.Cost };
        _context.Parts.Add(newPart);
        _context.SaveChanges();
        return Ok(new { Status = "Dodano", Id = newPart.IdPart });
    }

    [HttpPut("api/update/{id}")]
    public IActionResult ApiUpdate(int id, [FromBody] Parts p) {
        if (!AuthenticateApiAsAdmin()) return StatusCode(401, "API: Wymagany Admin.");
        var db = _context.Parts.Find(id);
        if (db == null) return NotFound();
        
        db.Name = p.Name; 
        db.Cost = p.Cost;
        _context.SaveChanges();
        return Ok(new { Status = "Zaktualizowano", Id = db.IdPart });
    }
    
    [HttpDelete("api/delete/{id}")]
    public IActionResult ApiDelete(int id) {
        if (!AuthenticateApiAsAdmin()) return StatusCode(401, "API: Wymagany Admin.");
        var db = _context.Parts.Find(id);
        if (db == null) return NotFound();
        
        _context.Parts.Remove(db);
        _context.SaveChanges();
        return Ok(new { Status = "Usunięto" });
    }
}