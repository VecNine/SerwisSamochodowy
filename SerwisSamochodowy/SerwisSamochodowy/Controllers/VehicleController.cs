using Microsoft.AspNetCore.Mvc;
using SerwisSamochodowy.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SerwisSamochodowy.Controllers;

[Route("Vehicle")]
public class VehicleController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehicleController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool IsAuthorized() => !string.IsNullOrEmpty(HttpContext.Session.GetString("User"));

    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        return View(_context.Vehicles.ToList());
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        return View();
    }

    [HttpPost("Create")]
    public IActionResult Create(Vehicle vehicle)
    {
        if (!IsAuthorized()) return Unauthorized();

        if (ModelState.IsValid)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(vehicle);
    }

    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        
        var vehicle = _context.Vehicles.Find(id);
        if (vehicle == null) return NotFound();
        
        return View(vehicle);
    }

    [HttpPost("Edit/{id}")]
    public IActionResult Edit(int id, Vehicle updated)
    {
        if (!IsAuthorized()) return Unauthorized();

        if (ModelState.IsValid)
        {
            var dbEntry = _context.Vehicles.Find(id);
            if (dbEntry == null) return NotFound();
            
            dbEntry.Model = updated.Model;
            dbEntry.Year = updated.Year;
            dbEntry.OwnerName = updated.OwnerName;
            dbEntry.OwnerSurname = updated.OwnerSurname;
            dbEntry.OwnerPhone = updated.OwnerPhone;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(updated);
    }

    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        
        var vehicle = _context.Vehicles.Find(id);
        if (vehicle == null) return NotFound();
        
        return View(vehicle);
    }

    [HttpPost("DeleteConfirmed/{id}")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!IsAuthorized()) return Unauthorized();
        
        var vehicle = _context.Vehicles.Find(id);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
    
    
    private bool AuthenticateApi() {
        string user = Request.Headers["X-Username"].ToString();
        string token = Request.Headers["X-Api-Token"].ToString();
        return _context.Users.Any(u => u.Username == user && u.ApiToken == token);
    }

    [HttpGet("api/get")]
    public IActionResult ApiGet() {
        if (!AuthenticateApi()) return StatusCode(401);
        return Json(_context.Vehicles.Select(v => new { v.IdVehicle, v.Model, v.Year, v.OwnerName, v.OwnerSurname }).ToList());
    }

    [HttpPost("api/add")]
    public IActionResult ApiAdd([FromBody] Vehicle v) {
        if (!AuthenticateApi()) return StatusCode(401);
        v.Repairments = null;
        _context.Vehicles.Add(v);
        _context.SaveChanges();
        return Ok(new { v.IdVehicle, Status = "Pojazd zarejestrowany" });
    }
    
    [HttpPut("api/update/{id}")]
    public IActionResult ApiUpdate(int id, [FromBody] Vehicle v) {
        if (!AuthenticateApi()) return StatusCode(401, "API: Brak autoryzacji.");
        var db = _context.Vehicles.Find(id);
        if (db == null) return NotFound();

        db.Model = v.Model;
        db.Year = v.Year;
        db.OwnerName = v.OwnerName;
        db.OwnerSurname = v.OwnerSurname;
        db.OwnerPhone = v.OwnerPhone;
        
        _context.SaveChanges();
        return Ok(new { Status = "Zaktualizowano", Id = db.IdVehicle });
    }

    [HttpDelete("api/delete/{id}")]
    public IActionResult ApiDelete(int id) {
        if (!AuthenticateApi()) return StatusCode(401, "API: Brak autoryzacji.");
        var db = _context.Vehicles.Find(id);
        if (db == null) return NotFound();

        _context.Vehicles.Remove(db);
        _context.SaveChanges();
        return Ok(new { Status = "Usunięto" });
    }
}