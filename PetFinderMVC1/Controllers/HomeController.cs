using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TuProyecto.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var userName = Request.Cookies["UserName"];
            var userLastName = Request.Cookies["UserLastName"];

            if (string.IsNullOrEmpty(userName) && User.Identity.IsAuthenticated)
            {
                userName = User.Identity.Name;
                userLastName = "Apellido"; 
            }

            ViewBag.UserName = userName;
            ViewBag.UserLastName = userLastName;

            return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult AccountDetails()
        {
            return View();
        }

        public IActionResult EditAccount()
        {
            return View(); 
        }
        public ActionResult DetallesPerdida() => View("~/Views/Home/DetallesPerdida.cshtml");
        
            
            
        

        public IActionResult Cuidados() => View("~/Views/Home/Cuidados.cshtml");

        public IActionResult Beneficios() => View("~/Views/Home/Beneficios.cshtml");

        public IActionResult Responsabilidad() => View("~/Views/Home/Responsabilidad.cshtml");
    }
}
