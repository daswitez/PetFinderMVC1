using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetFinderMVC1.Models;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace PetFinderMVC1.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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

        public async Task<IActionResult> DetallesAdopcion(string id)
        {
            var queryPublicaciones = @"
    query {
        publicaciones {
            id
            foto
            descripcion
            nombre
            especie
            raza
            color
            tamanho
            sexo
            ubicacionId
        }
    }";

            var requestBody = new StringContent(
                JsonConvert.SerializeObject(new { query = queryPublicaciones }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("http://petfinder.somee.com/graphql/", requestBody);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            var publicaciones = jsonResponse.data.publicaciones.ToObject<List<Publicacion>>();

            Publicacion publicacion = null;
            foreach (var p in publicaciones)
            {
                if (p.Id == id)
                {
                    publicacion = p;
                    break;
                }
            }

            if (publicacion == null)
                return NotFound();

            return View(publicacion);
        }
        public async Task<IActionResult> DetallesPerdida(string id)
        {
            var queryPublicaciones = @"
    query {
        publicaciones {
            id
            foto
            descripcion
            nombre
            especie
            raza
            color
            tamanho
            sexo
            ubicacionId
        }
    }";

            var requestBody = new StringContent(
                JsonConvert.SerializeObject(new { query = queryPublicaciones }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("http://petfinder.somee.com/graphql/", requestBody);

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

            var publicaciones = jsonResponse.data.publicaciones.ToObject<List<Publicacion>>();

            Publicacion publicacion = null;

            foreach (var p in publicaciones)
            {
                if (p.Id == id)
                {
                    publicacion = p;
                    break;
                }
            }

            if (publicacion == null)
                return NotFound();

            return View(publicacion);
        }




        public IActionResult AccountDetails()
        {
            return View();
        }

        public IActionResult EditAccount()
        {
            return View();
        }


        public IActionResult Cuidados() => View("~/Views/Home/Cuidados.cshtml");

        public IActionResult Beneficios() => View("~/Views/Home/Beneficios.cshtml");

        public IActionResult Responsabilidad() => View("~/Views/Home/Responsabilidad.cshtml");
        public IActionResult Adopcion()
        {
            return View("~/Views/Home/Adopcion.cshtml");
        }

        public IActionResult Perdidas()
        {
            return View("~/Views/Home/Perdidas.cshtml");
        }
    }
}
