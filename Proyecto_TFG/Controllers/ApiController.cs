using DocuWare.Platform.ServerClient;
using Microsoft.AspNetCore.Mvc;
using Proyecto_TFG.Services;
using Proyecto_TFG.Models;
using System.Text.Json;
using Proyecto_TFG.Functions;

namespace Proyecto_TFG.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiController : Controller
    {
        //Cargar interfaz DW
        private readonly IDocuWareConnect _docuwareConnect;

        //Constructor
        public ApiController(IDocuWareConnect docuwareConnect)
        {
            _docuwareConnect = docuwareConnect;
        }

        // Método para convertir SolicitudPedido a Dictionary<string, string> porque la API de DocuWare espera un formato más simple (Dictionary<string, string>)
        private Dictionary<string, string> ConvertirSolicitudPedidoADiccionario(SolicitudPedido solicitud) //Llega por parametro
        {
            var diccionario = new Dictionary<string, string>();

            // Agregar las propiedades simples al diccionario
            diccionario.Add("NR_PEDIDO", solicitud.NumeroSolicitud);
            diccionario.Add("FECHA_FACTURA", solicitud.FechaSolicitud);
            diccionario.Add("RAZON_SOCIAL_RECEPTOR", solicitud.TxtNombreSolicitante);
            diccionario.Add("CIF_RECEPTOR", solicitud.TxtNifSolicitante);
            diccionario.Add("EMAIL_SOLICITANTE", solicitud.TxtEmailSolicitante);
            diccionario.Add("RAZON_SOCIAL_PROVEEDOR", solicitud.NombreProveedor);
            diccionario.Add("CIF_PROVEEDOR", solicitud.TdNifProveedor);
            diccionario.Add("DIRECCION_PROVEEDOR", solicitud.TdDireccionProveedor);
            diccionario.Add("TELEFONO_PROVEEDOR", solicitud.TdTelefonoProveedor);
            diccionario.Add("EMAIL_PROVEEDOR", solicitud.TdEmailProveedor);
            diccionario.Add("DIRRECION_ENTREGA", solicitud.DireccionEntrega);
            diccionario.Add("CODIGO_POSTAL", solicitud.CodigoPostal);
            diccionario.Add("PROVINCIA", solicitud.Provincia);
            diccionario.Add("MUNICIPIO", solicitud.Municipio);
            diccionario.Add("PAIS", solicitud.Pais);
            diccionario.Add("TELEFONO", solicitud.Telefono);
            diccionario.Add("FECHA_ENTREGA", solicitud.FechaEntrega);
            diccionario.Add("MODALIDAD_DE_PAGO", solicitud.MetodoPago);
            diccionario.Add("TOTAL_FACTURA", solicitud.TotalPedido);

            string productosJson = JsonSerializer.Serialize(solicitud.Productos);
            diccionario.Add("LINEAS", productosJson);

            return diccionario;
        }



        //API
        [HttpPost("enviarSolicitudDocuware")]
        public IActionResult EnviarSolicitudPedidoDW([FromBody] SolicitudPedido datosSolicitud) //Detecta que el json tiene la estructurada de este modelo
        {
            Dictionary<string, string> datosSolicitudDiccionario = ConvertirSolicitudPedidoADiccionario(datosSolicitud);
            Document documentoEnviado = _docuwareConnect.InsertDataToDocuWare("969fe738-e75f-4266-b6d0-d980badbe7ad", datosSolicitudDiccionario);
            if (documentoEnviado != null)
            {
                return Ok(new {
                    status = "OK",
                    message = "Documento enviado exitosamente a DW" }
                );
            }
            else
            {
                return BadRequest(new { 
                    status = "KO",
                    message = "No se ha podido enviar el documento a DW" 
                });
            }

        }

       


    }
}
