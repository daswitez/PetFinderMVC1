using Microsoft.AspNetCore.Mvc;

namespace TuProyecto.Controllers
{
    public class PublicarController : Controller
    {
        public IActionResult Perdida() => View("~/Views/Home/publicar_perdida.cshtml");
        public IActionResult Adopcion() => View("~/Views/Home/publicar_adopcion.cshtml");
    }
}
