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

        // Acción para manejar el inicio de sesión
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

                    // Verificar si el token está presente en la respuesta
                    if (data?.data?.login?.token != null)
                    {
                        var token = data.data.login.token.ToString();

                        // Guardar el token en cookies
                        Response.Cookies.Append("AuthToken", token, new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTime.UtcNow.AddDays(7) // Ajustar expiración
                        });

                        // Establecer un mensaje de éxito
                        TempData["SuccessMessage"] = "¡Login exitoso! Bienvenido, " + data.data.login.usuario.nombre.ToString() + ".";

                        // Redirigir al Home después de iniciar sesión correctamente
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Usuario o contraseña incorrectos.";
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

        // Acción para manejar el registro
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
                        // Redirigir al login después de registrarse
                        TempData["SuccessMessage"] = "¡Registro exitoso! Ahora puedes iniciar sesión.";
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


        // Acción para cerrar sesión
        public IActionResult Logout()
        {
            // Eliminar las cookies de autenticación al cerrar sesión
            Response.Cookies.Delete("AuthToken");

            // Redirigir al Home después de cerrar sesión
            return RedirectToAction("Index", "Home");
        }
    }
}
