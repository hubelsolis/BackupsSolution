namespace Backups.Infrastructure.Network.Transmisores;

internal static class RutaRemotaHelper
{
    internal static string Combinar(string carpetaRemota, string rutaLocal)
    {
        string nombreArchivo = Path.GetFileName(rutaLocal);
        if (string.IsNullOrWhiteSpace(carpetaRemota))
        {
            return nombreArchivo;
        }

        return carpetaRemota.TrimEnd('/') + "/" + nombreArchivo;
    }
}
