using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace PetFinderMVC1.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult IsAuthenticated()
        {
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { isAuthenticated = false });
            }

            return Json(new { isAuthenticated = true });
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");

            return RedirectToAction("Login", "Account");
        }
    }
}
