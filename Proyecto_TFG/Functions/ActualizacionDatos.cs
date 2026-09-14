using System.Text.Json;
using Proyecto_TFG.Models;
using Proyecto_TFG.Services;

namespace Proyecto_TFG.Functions
{
    public class ActualizacionDatos
    {
        public static List<Proveedor> obtenerDatosProveedores()
        {
            IMysqlFunctions _iMysqlConector = new MySqlFunctions();

            List<Proveedor> listadoProveedores = _iMysqlConector.RecuperarProveedores();

            string jsonString = JsonSerializer.Serialize(listadoProveedores, new JsonSerializerOptions { WriteIndented = true });
            string filePath = "wwwroot/js/datosProveedores.json";
            File.WriteAllText(filePath, jsonString);

            return listadoProveedores;
        }

        public static List<Producto> obtenerDatosProductos()
        {
            IMysqlFunctions _iMysqlConector = new MySqlFunctions();

            List<Producto> listadoProductos = _iMysqlConector.RecuperarProductos();

            string jsonString = JsonSerializer.Serialize(listadoProductos, new JsonSerializerOptions { WriteIndented = true });
            string filePath = "wwwroot/js/datosProductos.json";
            File.WriteAllText(filePath, jsonString);

            return listadoProductos;
        }
    }
}
