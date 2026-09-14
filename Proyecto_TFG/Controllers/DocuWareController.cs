using Microsoft.AspNetCore.Mvc;
using Proyecto_TFG.Functions;
using Proyecto_TFG.Models;

namespace Proyecto_TFG.Controllers
{
    [ApiController]
    public class DocuWareController : Controller
    {
        [HttpPost]
        [Route("api/[Controller]")]
        public IActionResult GenerarDocumento([FromBody] DataInput dataInput)
        {
            Secrets secrets = new Secrets()
            {
                usuario = "",
                password = ""
            };

            string dwcloud = "";// lo definimos es el nombre del cloud de docuware - tenant


            CrearSolicitud.ProcesarSolicitudPedido(dataInput.dwDocId, dataInput.guidFileCabient, dataInput.docIdModelo, secrets);

            return Ok(new
            {
                status = "OK",
                response = "Tarea de creación de la solicitud"
            });

        }

    }

    public class  DataInput
    {
        public int dwDocId { get; set; }
        public string guidFileCabient { get; set; }
        public int docIdModelo { get; set;  }
    }
}
