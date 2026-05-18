using Microsoft.AspNetCore.Mvc;

namespace SerwisSamochodowy.Controllers;

public class ManagementController : Controller
{
    public IActionResult Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("User")))
        {
            return RedirectToAction("Login", "Users");
        }
        return View();
    }
}