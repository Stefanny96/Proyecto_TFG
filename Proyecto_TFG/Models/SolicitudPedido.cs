using System.Text.Json.Serialization;

namespace Proyecto_TFG.Models
{
    //SolicitudPedido permite trabajar con los datos de una forma más entendible. Este objeto es la representación lógica de lo que se esta procesando en el backend.
    //ESTO ES EL MODELO DE LO QUE CONTIENE MI JSON
    public class SolicitudPedido
    {
        [JsonPropertyName("numeroSolicitud")]
        public string NumeroSolicitud { get; set; }


        [JsonPropertyName("fechaSolicitud")]
        public string FechaSolicitud { get; set; }


        [JsonPropertyName("txtNombreSolicitante")]
        public string TxtNombreSolicitante { get; set; }


        [JsonPropertyName("txtNifSolicitante")]
        public string TxtNifSolicitante { get; set; }


        [JsonPropertyName("txtEmailSolicitante")]
        public string TxtEmailSolicitante { get; set; }


        [JsonPropertyName("nombreProveedor")]
        public string NombreProveedor { get; set; }

        [JsonPropertyName("tdNifProveedor")]
        public string TdNifProveedor { get; set; }

        [JsonPropertyName("tdDireccionProveedor")]
        public string TdDireccionProveedor { get; set; }

        [JsonPropertyName("tdTelefonoProveedor")]
        public string TdTelefonoProveedor { get; set ; }

        [JsonPropertyName("tdEmailProveedor")]
        public string TdEmailProveedor { get; set; }

        [JsonPropertyName("direccionEntrega")]
        public string DireccionEntrega { get; set; }


        [JsonPropertyName("codigoPostal")]
        public string CodigoPostal { get; set; }


        [JsonPropertyName("provincia")]
        public string Provincia { get; set; }


        [JsonPropertyName("municipio")]
        public string Municipio { get; set; }


        [JsonPropertyName("pais")]
        public string Pais { get; set; }


        [JsonPropertyName("telefono")]
        public string Telefono { get; set; }


        [JsonPropertyName("fechaEntrega")]
        public string FechaEntrega { get; set; }


        [JsonPropertyName("metodoPago")]
        public string MetodoPago { get; set; }


        [JsonPropertyName("totalPedido")]
        public string TotalPedido { get; set; }


        public List<Producto> Productos { get; set; }  // Aquí mapeamos el array de productos
    }
}
