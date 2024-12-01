using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System;
using PetFinderMVC1.ViewModels;

namespace PetFinderMVC1.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Vista de login
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        // Vista de registro
        public IActionResult Register()
        {
            return View("~/Views/Home/Register.cshtml");
        }

        // Acci�n para manejar el inicio de sesi�n
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            var query = @"
mutation Login($input: LoginInput!) {
    login(input: $input) {
        token
        usuario {
            id
            nombre
            apellido
            email
            telefono
        }
    }
}";

            var variables = new
            {
                input = new { email = loginViewModel.Email, password = loginViewModel.Password }
            };

            var requestContent = new { query, variables };
            var jsonRequest = JsonConvert.SerializeObject(requestContent);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://localhost:7055/graphql/", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(responseString);

                    // Verificar si el token est� presente en la respuesta
                    if (data?.data?.login?.token != null)
                    {
                        var token = data.data.login.token.ToString();

                        // Guardar el token en cookies
                        Response.Cookies.Append("AuthToken", token, new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTime.UtcNow.AddDays(7) // Ajustar expiraci�n
                        });

                        // Establecer un mensaje de �xito
                        TempData["SuccessMessage"] = "�Login exitoso! Bienvenido, " + data.data.login.usuario.nombre.ToString() + ".";

                        // Redirigir al Home despu�s de iniciar sesi�n correctamente
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Usuario o contrase�a incorrectos.";
                    }
                }
                else
                {
                    // Mostrar error en el servidor
                    TempData["ErrorMessage"] = $"Error en el servidor: {response.StatusCode} - {responseString}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al procesar la solicitud: {ex.Message}";
            }

            return View("~/Views/Home/Login.cshtml");
        }

        // Acci�n para manejar el registro
        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string apellido, string email, string telefono, string contrase_a)
        {
            var query = @"
mutation CreateUsuario($input: UsuarioInput!) {
    createUsuario(input: $input) {
        id
        nombre
        apellido
        email
        telefono
    }
}";

            var variables = new
            {
                input = new
                {
                    nombre,
                    apellido,
                    email,
                    telefono,
                    contrase_a,
                    tipo = "Usuario"
                }
            };

            var requestContent = new { query, variables };
            var jsonRequest = JsonConvert.SerializeObject(requestContent);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://localhost:7055/graphql/", httpContent);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(responseString);

                    if (data?.data?.createUsuario != null)
                    {
                        // Redirigir al login despu�s de registrarse
                        TempData["SuccessMessage"] = "�Registro exitoso! Ahora puedes iniciar sesi�n.";
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al registrar el usuario.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error en el servidor: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al procesar la solicitud: {ex.Message}";
            }

            return View("~/Views/Home/Register.cshtml");
        }

        public IActionResult AccountDetails()
        {
            return View(); // Simplemente retorna la vista de detalles
        }
        public IActionResult EditAccount()
        {
            return View();
        }


        // Acci�n para cerrar sesi�n
        public IActionResult Logout()
        {
            // Eliminar las cookies de autenticaci�n al cerrar sesi�n
            Response.Cookies.Delete("AuthToken");

            // Redirigir al Home despu�s de cerrar sesi�n
            return RedirectToAction("Index", "Home");
        }
    }
}
