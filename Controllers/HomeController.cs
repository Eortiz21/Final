using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        // Si el usuario no está autenticado, redirige al login de Identity
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
