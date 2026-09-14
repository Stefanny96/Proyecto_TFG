using MySqlConnector;
using Proyecto_TFG.Models;

namespace Proyecto_TFG.Services
{
    public interface IMysqlFunctions
    {
        Usuario ConectToFacturaEdi(Secrets secrets);
        Usuario ObtenerUsuarioPorId(int idUsuario);
        Empresa ObtenerEmpresaPorId(int idEmpresa);
        List<Proveedor> RecuperarProveedores();
        List<Producto> RecuperarProductos();

    }
}
