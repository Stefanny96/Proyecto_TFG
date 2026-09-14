namespace Proyecto_TFG.Functions
{
    public class Encrypt64
    {
        public string Encriptar(string texto)
        {
            string result = string.Empty;
            byte[] encrypted = System.Text.Encoding.UTF8.GetBytes(texto);
            result = Convert.ToBase64String(encrypted);
            return result;
        }

        public string Desencriptar(string encryptedTexto)
        {
            // Decodificar el string Base64 a un arreglo de bytes
            byte[] decryptedBytes = Convert.FromBase64String(encryptedTexto);

            // Convertir esos bytes de vuelta a string usando UTF-8
            string result = System.Text.Encoding.UTF8.GetString(decryptedBytes);
            return result;
        }
    }
}
