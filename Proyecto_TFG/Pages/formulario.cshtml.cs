using DocuWare.Platform.ServerClient;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Proyecto_TFG.Functions;
using Proyecto_TFG.Models;
using Proyecto_TFG.Services;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_TFG.Pages
{
    [Authorize]
    public class formularioModel : PageModel
    {
        //Cargar Interfaz de funciones de MySql
        private readonly IMysqlFunctions _mysqlFunctions;

        //Cargar interfaz encriptacion
        private readonly IEncriptacion _encriptacion;

        public formularioModel(IMysqlFunctions mysqlFunctions, IEncriptacion encriptacion)
        {
            _mysqlFunctions = mysqlFunctions;
            _encriptacion = encriptacion;
        }

        /// <summary>
        /// Carga inicial del formulario
        /// </summary>

        public string UsuarioDesencriptado { get; set; }
        public Usuario DatosUsuario { get; set; }
        public Empresa DatosEmpresa { get; set; }


        //public IActionResult OnGet([FromQuery] string user)
        //{
        //    if (!string.IsNullOrEmpty(user))
        //    {
        //        // Desencriptar el parámetro 'user' recibido
        //        UsuarioDesencriptado = _encriptacion.Desencriptar(user);

        //        // Convertir el usuario desencriptado (id) a entero
        //        // Obtener los datos del usuario usando su ID
        //        int idUsuario = Convert.ToInt32(UsuarioDesencriptado);
        //        DatosUsuario = _mysqlFunctions.ObtenerUsuarioPorId(idUsuario);

        //        int idEmpresa = DatosUsuario.IdEmpresa;
        //        DatosEmpresa = _mysqlFunctions.ObtenerEmpresaPorId(idEmpresa);
        //    }
        //    else
        //    {
        //        UsuarioDesencriptado = "No se recibió el parámetro 'user'.";
        //    }

        //    return Page();

        //}
        public IActionResult OnGet()
        {
            // Obtener el usuario autenticado desde los claims de la cookie
            var user = HttpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                // Recuperar los datos del usuario desde los claims
                string userId = user.FindFirst("UserId")?.Value; // Obtener el UserId desde los claims

                // Si los datos del usuario son válidos, puedes obtener más información desde la base de datos
                if (!string.IsNullOrEmpty(userId))
                {
                    // Obtener los datos del usuario desde la base de datos usando el ID
                    DatosUsuario = _mysqlFunctions.ObtenerUsuarioPorId(Convert.ToInt32(userId));

                    if (DatosUsuario != null)
                    {
                        int idEmpresa = DatosUsuario.IdEmpresa;
                        DatosEmpresa = _mysqlFunctions.ObtenerEmpresaPorId(idEmpresa);
                    }
                }
            }
            else
            {
                // Redirigir al login si el usuario no está autenticado
                return RedirectToPage("/Index");
            }

            return Page();
        }

    }
}
