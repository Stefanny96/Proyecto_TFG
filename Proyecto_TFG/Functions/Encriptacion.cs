using Microsoft.AspNetCore.Mvc;
using Proyecto_TFG.Services;

namespace Proyecto_TFG.Functions
{
    public class Encriptacion : IEncriptacion
    {
        public String Encriptar(string texto)
        {
            try
            {
                // Lógica de encriptación
                Encrypt64 encryptor = new Encrypt64(); //Instancio
                string textoEncriptado = encryptor.Encriptar(texto);

                return textoEncriptado;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encriptar: {ex.Message}");
                return ex.ToString();
            }
        }

        [HttpPost]
        public String Desencriptar(string texto)
        {
            try
            {
                if (texto == null)
                {
                    Console.WriteLine("El texto no puede estar vacío.");
                }

                // Lógica de desencriptación
                Encrypt64 encryptor = new Encrypt64();
                string textoDesencriptado = encryptor.Desencriptar(texto);

                return textoDesencriptado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error desencriptar: {ex.Message}");
                return ex.ToString();
            }
            
        }
    }
}
