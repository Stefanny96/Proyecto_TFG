using System.Text.Json.Serialization;

namespace Proyecto_TFG.Models
{
    public class Producto
    {

        public string producto_id { get; set; } = "";
        public string codigo { get; set; }
        public string nombre { get; set; } = "";
        public string descripcion { get; set; }
        public decimal precio_unitario { get; set; } = 0;
        public int unidades_disponibles { get; set; }
        public int stock_maximo { get; set; }
        public string proveedor_id { get; set; } = "";
        public decimal CantidadProducto { get; set; } = 0;
        public decimal SubtotalProducto { get; set; } = 0;
    }
}
