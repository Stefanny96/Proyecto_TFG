using Microsoft.AspNetCore.Mvc;
using Proyecto_TFG.Functions;

namespace Proyecto_TFG.Controllers
{
    [ApiController]
    public class ActualizarMaestros : Controller
    {
        [HttpPost("api/[controller]")]

        public dynamic ActualizacionDatosMaestrosProveedores([FromBody] DataInputMaestros datosInputMaestros)
        {
           dynamic response = ActualizacionDatos.obtenerDatosProveedores();

            return response;
        }

        [HttpPost("ActualizacionDatosMaestrosProductos")]
        public dynamic ActualizacionDatosMaestrosProductos([FromBody] DataInputMaestros datosInputMaestros)
        {
            dynamic response = ActualizacionDatos.obtenerDatosProductos();

            return response;
        }
    }

    public class DataInputMaestros
    {
        public string nombre { get; set; }
    }
}
