using Proyecto_TFG.Models;
using Proyecto_TFG.Services;

namespace Proyecto_TFG.Functions
{
    public class Comunes : IComunes
    {

        public bool ComprobarDatosUsuario(Usuario user)
        {
            return !string.IsNullOrEmpty(user?.Nombre);

        }
    }
}
