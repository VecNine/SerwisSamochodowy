using Microsoft.AspNetCore.Mvc;
using SerwisSamochodowy.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SerwisSamochodowy.Controllers;

[Route("Mechanic")]
public class MechanicController : Controller
{
    private readonly ApplicationDbContext _context;

    public MechanicController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool IsAuthorized() => !string.IsNullOrEmpty(HttpContext.Session.GetString("User"));
    private bool IsAdmin() => HttpContext.Session.GetString("IsAdmin") == "True";

    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        return View(_context.Mechanics.ToList());
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        if (!IsAdmin()) return RedirectToAction("Index");
        return View();
    }

    [HttpPost("Create")]
    public IActionResult Create(Mechanic mechanic)
    {
        if (!IsAdmin()) return Unauthorized();

        if (ModelState.IsValid)
        {
            _context.Mechanics.Add(mechanic);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(mechanic);
    }

    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Index");
        var mechanic = _context.Mechanics.Find(id);
        if (mechanic == null) return NotFound();
        return View(mechanic);
    }

    [HttpPost("Edit/{id}")]
    public IActionResult Edit(int id, Mechanic updated)
    {
        if (!IsAdmin()) return Unauthorized();

        if (ModelState.IsValid)
        {
            var dbEntry = _context.Mechanics.Find(id);
            if (dbEntry == null) return NotFound();

            dbEntry.Name = updated.Name;
            dbEntry.Surname = updated.Surname;
            dbEntry.Salary = updated.Salary;
            dbEntry.EmploymentStartDate = updated.EmploymentStartDate;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(updated);
    }

    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
        if (!IsAdmin()) return RedirectToAction("Index");
        var mechanic = _context.Mechanics.Find(id);
        if (mechanic == null) return NotFound();
        return View(mechanic);
    }

    [HttpPost("DeleteConfirmed/{id}")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!IsAdmin()) return Unauthorized();
        var mechanic = _context.Mechanics.Find(id);
        if (mechanic != null)
        {
            _context.Mechanics.Remove(mechanic);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
    
    private bool AuthenticateApi() {
        string? username = Request.Headers["X-Username"].ToString();
        string? token = Request.Headers["X-Api-Token"].ToString();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token)) return false;

        return _context.Users.Any(u => u.Username == username && u.ApiToken == token);
    }

    private bool AuthenticateApiAsAdmin() {
        string? username = Request.Headers["X-Username"].ToString();
        string? token = Request.Headers["X-Api-Token"].ToString();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(token)) return false;

        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.ApiToken == token);
        return user != null && user.IsAdmin;
    }

    [HttpGet("api/get")]
    public IActionResult ApiGet()
    {
        if (!AuthenticateApi()) 
            return StatusCode(401, "Błąd autoryzacji API: Niepoprawny login lub token.");

        var data = _context.Mechanics.Select(m => new {
            m.IdMechanic,
            m.Name,
            m.Surname,
            m.Salary,
            m.EmploymentStartDate
        }).ToList();

        return Json(data);
    }
    
    [HttpPost("api/add")]
    public IActionResult ApiAdd([FromBody] Mechanic m) {
        if (!AuthenticateApiAsAdmin()) 
            return StatusCode(401, "Błąd autoryzacji API: Wymagane uprawnienia administratora.");
        
        m.Repairments = null; 
        
        _context.Mechanics.Add(m);
        _context.SaveChanges();
        return Ok(new { m.IdMechanic, m.Name, m.Surname, Status = "Dodano" });
    }

    [HttpPut("api/update/{id}")]
    public IActionResult ApiUpdate(int id, [FromBody] Mechanic m) {
        if (!AuthenticateApiAsAdmin()) 
            return StatusCode(401, "Błąd autoryzacji API.");

        var db = _context.Mechanics.Find(id);
        if (db == null) return NotFound();

        db.Name = m.Name; 
        db.Surname = m.Surname; 
        db.Salary = m.Salary; 
        db.EmploymentStartDate = m.EmploymentStartDate;

        _context.SaveChanges();
        return Ok(new { db.IdMechanic, Status = "Zaktualizowano" });
    }

    [HttpDelete("api/delete/{id}")]
    public IActionResult ApiDelete(int id) {
        if (!AuthenticateApiAsAdmin()) 
            return StatusCode(401, "Błąd autoryzacji API.");

        var db = _context.Mechanics.Find(id);
        if (db == null) return NotFound();

        _context.Mechanics.Remove(db);
        _context.SaveChanges();
        return Ok(new { Message = "Usunięto pomyślnie" });
    }
}