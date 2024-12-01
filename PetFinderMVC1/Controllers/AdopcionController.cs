using Microsoft.AspNetCore.Mvc;

namespace TuProyecto.Controllers
{
    public class AdopcionController : Controller
    {
        public IActionResult Index() => View("~/Views/Home/Adopcion.cshtml");
    }
}
