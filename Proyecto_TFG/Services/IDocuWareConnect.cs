using DocuWare.Platform.ServerClient;

namespace Proyecto_TFG.Services
{
    public interface IDocuWareConnect
    {

        /// <summary>
        /// Devuelve la conexión a DocuWare
        /// </summary>
        /// <returns></returns>
        ServiceConnection ConnectToDocuWare();


        /// <summary>
        /// Inserta datos de cabecera en DocuWare y devuelve el registro creado
        /// </summary>
        /// <param name="guidFileCabinet">El guid del archivador de DocuWare</param>
        /// <param name="datosCabecera">Datos del formulario (sin Lineas)</param>
        /// <returns></returns>
        Document InsertDataToDocuWare(string guidFileCabinet, Dictionary<string, string> datosCabecera);

    }
}
