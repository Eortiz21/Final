using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Si el usuario no está autenticado, redirige al login
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
