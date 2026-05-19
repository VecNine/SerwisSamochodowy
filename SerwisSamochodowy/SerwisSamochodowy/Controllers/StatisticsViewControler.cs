using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerwisSamochodowy.Models;

namespace SerwisSamochodowy.Controllers;

[Route("Statistics")]
public class StatisticsController : Controller
{
    private readonly ApplicationDbContext _context;
    public StatisticsController(ApplicationDbContext context) => _context = context;

    [HttpGet("")]
    public IActionResult Index(DateTime? startDate, DateTime? endDate)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "True") 
        {
            return RedirectToAction("Index", "Home"); 
        }
        
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("User"))) 
            return RedirectToAction("Login", "Users");

        var start = startDate ?? DateTime.Today.AddMonths(-2);
        var end = endDate ?? DateTime.Today;

        var rStart = new DateTime(start.Year, start.Month, 1);
        var rEnd = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month), 23, 59, 59);

        var repairs = _context.Repairments
            .Include(r => r.Mechanic).Include(r => r.Vehicle)
            .Include(r => r.RepairmentParts).ThenInclude(rp => rp.Part)
            .Where(r => r.Date >= rStart && r.Date <= rEnd).ToList();

        var model = new StatisticsViewModel {
            StartDate = rStart, EndDate = rEnd,
            TotalServiceRevenue = repairs.Sum(r => r.ServicePrice),
            TotalPartsValue = repairs.Sum(r => r.RepairmentParts.Sum(rp => rp.Part.Cost * rp.Quantity))
        };
        
        var mechs = _context.Mechanics.ToList();
        decimal salariesSum = 0;
        foreach (var m in mechs) {
            int months = 0;
            for (DateTime d = rStart; d <= rEnd; d = d.AddMonths(1)) {
                if (m.EmploymentStartDate <= new DateTime(d.Year, d.Month, DateTime.DaysInMonth(d.Year, d.Month))) months++;
            }
            if (months > 0 || repairs.Any(r => r.IdMechanic == m.IdMechanic)) {
                var mRepairs = repairs.Where(r => r.IdMechanic == m.IdMechanic).ToList();
                model.MechanicStats.Add(new MechanicPerformanceDetail {
                    FullName = m.Name + " " + m.Surname, MonthsWorkedInRange = months,
                    SalaryCosts = months * m.Salary, RepairCount = mRepairs.Count,
                    RevenueGenerated = mRepairs.Sum(r => r.ServicePrice)
                });
                salariesSum += (months * m.Salary);
            }
        }
        model.TotalMechanicSalaries = salariesSum;
        
        model.TopVehicles = repairs.GroupBy(r => r.IdVehicle).Select(g => {
            var v = g.First().Vehicle;
            return new TopVehicleStats {
                VehicleId = g.Key, OwnerFullName = v.OwnerName + " " + v.OwnerSurname,
                VehicleModel = v.Model, RepairCount = g.Count(),
                TotalSpent = g.Sum(r => r.ServicePrice + r.RepairmentParts.Sum(rp => rp.Part.Cost * rp.Quantity))
            };
        }).OrderByDescending(x => x.RepairCount).Take(3).ToList();

        return View(model);
    }
}