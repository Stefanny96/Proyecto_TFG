namespace Proyecto_TFG.Services
{
    public interface IEncriptacion
    {
        string Encriptar(string text);
        string Desencriptar(string text);
    }
}
