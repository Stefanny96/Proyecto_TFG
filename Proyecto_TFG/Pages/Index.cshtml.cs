using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using Proyecto_TFG.Models;
using Proyecto_TFG.Services;


namespace Proyecto_TFG.Pages
{
    public class IndexModel : PageModel
    {
        //Cargar Interfaz de funciones de MySql
        private readonly IMysqlFunctions _mysqlFunctions;

        //Cargar Interfaz de Encriptacion
        private readonly IEncriptacion _encriptacion;

        /// <summary>
        /// Constructor del Index
        /// </summary>
        /// <param name="mysqlFunctions"></param>

        public IndexModel(IMysqlFunctions mysqlFunctions, IEncriptacion encriptacion)
        {
            _mysqlFunctions = mysqlFunctions;
            _encriptacion = encriptacion;
        }
        //Variable de campo. Poner "_" antes de la variable es una convenci�n com�n en C# para indicar que se trata de un campo privado de la clase.
        //private readonly AppDbContext _dbContext; //Almacena el contexto de la base de datos

        //El atributo [BindProperty] en Razor Pages se utiliza para enlazar autom�ticamente los valores enviados desde un formulario HTML
        // Propiedades para almacenar los datos del formulario
        [BindProperty]
        public string Usuario { get; set; }

        [BindProperty]
        public string Contrasena { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet()

        {

            // Cadena de conexión

            return Page();
        }

        public IActionResult OnPostLogInCheck()
        {
            Secrets secrets = new Secrets()
            {
                usuario = Usuario,
                password = Contrasena
            };

           Usuario user = _mysqlFunctions.ConectToFacturaEdi(secrets);

            if (user != null)
            {
                // Crear los reclamos de autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("UserId", user.IdUsuario.ToString()) // Agregar el ID del usuario como un reclamo
                };

                // Crear la identidad de los reclamos
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth"); // Usa el esquema "CookieAuth"

                // Autenticar al usuario y guardar los claims en la cookie
                HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity)).Wait();


                /*
                 * En Razor Pages, RedirectToPage espera solo el nombre de la página (sin parámetros de consulta), 
                 * y los parámetros de consulta deben proporcionarse por separado.
                */
                //string usuarioEncriptado = _encriptacion.Encriptar(Convert.ToString(user.IdUsuario));
                return RedirectToPage("formulario"); //, new { user = usuarioEncriptado }
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
                return Page();
            }

        }

    }
}