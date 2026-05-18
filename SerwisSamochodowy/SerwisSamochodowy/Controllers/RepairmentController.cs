using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SerwisSamochodowy.Models;
using Microsoft.AspNetCore.Http;

namespace SerwisSamochodowy.Controllers;

[Route("Repairment")]
public class RepairmentController : Controller
{
    private readonly ApplicationDbContext _context;

    public RepairmentController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool IsAuthorized() => !string.IsNullOrEmpty(HttpContext.Session.GetString("User"));
    
    [HttpGet("")]
    [HttpGet("Index")]
    public IActionResult Index()
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");

        var repairments = _context.Repairments
            .Include(r => r.Mechanic)
            .Include(r => r.Vehicle)
            .Include(r => r.RepairmentParts)
                .ThenInclude(rp => rp.Part)
            .ToList();

        return View(repairments);
    }
    
    [HttpGet("Create")]
    public IActionResult Create()
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        PrepareViewBags();
        return View();
    }

    [HttpPost("Create")]
    public IActionResult Create(Repairment repairment, int[] selectedParts, int[] quantities)
    {
        if (!IsAuthorized()) return Unauthorized();
        
        ModelState.Remove("Vehicle");
        ModelState.Remove("Mechanic");
        ModelState.Remove("RepairmentParts");

        if (ModelState.IsValid)
        {
            repairment.Vehicle = null;
            repairment.Mechanic = null;
            repairment.RepairmentParts = new List<RepairmentParts>();

            _context.Repairments.Add(repairment);
            _context.SaveChanges();
            
            if (selectedParts != null)
            {
                for (int i = 0; i < selectedParts.Length; i++)
                {
                    if (selectedParts[i] > 0 && quantities[i] > 0)
                    {
                        _context.RepairmentParts.Add(new RepairmentParts
                        {
                            IdRepairment = repairment.IdRepairment,
                            IdPart = selectedParts[i],
                            Quantity = quantities[i]
                        });
                    }
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        PrepareViewBags();
        return View(repairment);
    }

    [HttpGet("Edit/{id}")]
    public IActionResult Edit(int id)
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");

        var repairment = _context.Repairments
            .Include(r => r.RepairmentParts)
            .FirstOrDefault(r => r.IdRepairment == id);

        if (repairment == null) return NotFound();

        PrepareViewBags();
        return View(repairment);
    }
    
    [HttpPost("Edit/{id}")]
    public IActionResult Edit(int id, Repairment updated, int[] selectedParts, int[] quantities)
    {
        if (!IsAuthorized()) return Unauthorized();
        
        ModelState.Remove("Vehicle");
        ModelState.Remove("Mechanic");
        ModelState.Remove("RepairmentParts");

        if (ModelState.IsValid)
        {
            var dbEntry = _context.Repairments
                .Include(r => r.RepairmentParts)
                .FirstOrDefault(r => r.IdRepairment == id);

            if (dbEntry == null) return NotFound();

            dbEntry.Date = updated.Date;
            dbEntry.ServicePrice = updated.ServicePrice;
            dbEntry.IdMechanic = updated.IdMechanic;
            dbEntry.IdVehicle = updated.IdVehicle;
            
            _context.RepairmentParts.RemoveRange(dbEntry.RepairmentParts);
            
            if (selectedParts != null)
            {
                for (int i = 0; i < selectedParts.Length; i++)
                {
                    if (selectedParts[i] > 0 && quantities[i] > 0)
                    {
                        _context.RepairmentParts.Add(new RepairmentParts
                        {
                            IdRepairment = id,
                            IdPart = selectedParts[i],
                            Quantity = quantities[i]
                        });
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        PrepareViewBags();
        return View(updated);
    }

    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
        if (!IsAuthorized()) return RedirectToAction("Login", "Users");
        
        var repairment = _context.Repairments
            .Include(r => r.Mechanic)
            .Include(r => r.Vehicle)
            .Include(r => r.RepairmentParts)
            .ThenInclude(rp => rp.Part)
            .FirstOrDefault(r => r.IdRepairment == id);
            
        if (repairment == null) return NotFound();
        return View(repairment);
    }
    
    [HttpPost("DeleteConfirmed/{id}")]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!IsAuthorized()) return Unauthorized();

        var repairment = _context.Repairments
            .Include(r => r.RepairmentParts)
            .FirstOrDefault(r => r.IdRepairment == id);
            
        if (repairment != null)
        {
            _context.RepairmentParts.RemoveRange(repairment.RepairmentParts); 
            
            _context.Repairments.Remove(repairment);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }
    private void PrepareViewBags()
    {
        ViewBag.Mechanics = new SelectList(_context.Mechanics.Select(m => new { m.IdMechanic, Full = m.Name + " " + m.Surname }), "IdMechanic", "Full");
        ViewBag.Vehicles = new SelectList(_context.Vehicles.Select(v => new { v.IdVehicle, Full = v.Model + " (" + v.OwnerSurname + ")" }), "IdVehicle", "Full");
        ViewBag.AllParts = _context.Parts.ToList();
    }
    
    private bool AuthenticateApi() {
        string user = Request.Headers["X-Username"].ToString();
        string token = Request.Headers["X-Api-Token"].ToString();
        return _context.Users.Any(u => u.Username == user && u.ApiToken == token);
    }

    [HttpGet("api/get")]
    public IActionResult ApiGet() {
        if (!AuthenticateApi()) return StatusCode(401);
        return Json(_context.Repairments.Select(r => new { 
            r.IdRepairment, r.Date, r.ServicePrice, r.IdMechanic, r.IdVehicle 
        }).ToList());
    }

    [HttpPost("api/add")]
    public IActionResult ApiAdd([FromBody] Repairment r) {
        if (!AuthenticateApi()) return StatusCode(401);
        
        r.Vehicle = null;
        r.Mechanic = null;
        r.RepairmentParts = null;

        _context.Repairments.Add(r);
        _context.SaveChanges();
        return Ok(new { r.IdRepairment, Status = "Zlecenie dodane" });
    }
    
    [HttpPost("api/addPart")]
    public IActionResult ApiAddPart([FromBody] RepairmentParts rp)
    {
        if (!AuthenticateApi()) return StatusCode(401);
        
        var repairExists = _context.Repairments.Any(r => r.IdRepairment == rp.IdRepairment);
        var partExists = _context.Parts.Any(p => p.IdPart == rp.IdPart);

        if (!repairExists || !partExists) 
            return BadRequest("Nie znaleziono podanej naprawy lub części.");

        _context.RepairmentParts.Add(rp);
        _context.SaveChanges();

        return Ok(new { Status = "Część przypisana do naprawy" });
    }
    
    [HttpPut("api/update/{id}")]
    public IActionResult ApiUpdate(int id, [FromBody] Repairment r) {
        if (!AuthenticateApi()) return StatusCode(401, "API: Brak autoryzacji.");
        var db = _context.Repairments.Find(id);
        if (db == null) return NotFound();

        db.Date = r.Date;
        db.ServicePrice = r.ServicePrice;
        db.IdMechanic = r.IdMechanic;
        db.IdVehicle = r.IdVehicle;

        _context.SaveChanges();
        return Ok(new { Status = "Zaktualizowano", Id = db.IdRepairment });
    }
    
    [HttpDelete("api/delete/{id}")]
    public IActionResult ApiDelete(int id) {
        if (!AuthenticateApi()) return StatusCode(401, "API: Brak autoryzacji.");
        var db = _context.Repairments.Include(rep => rep.RepairmentParts).FirstOrDefault(rep => rep.IdRepairment == id);
        if (db == null) return NotFound();
        
        _context.RepairmentParts.RemoveRange(db.RepairmentParts);
        _context.Repairments.Remove(db);
        _context.SaveChanges();
        return Ok(new { Status = "Usunięto" });
    }
}