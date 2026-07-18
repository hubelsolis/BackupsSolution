using SharpCompress.Compressors;
using SharpCompress.Compressors.LZMA;

namespace Backups.Infrastructure.Compression.Compresores;

internal sealed class CompresorLzma : ICompresor
{
    public string Comprimir(string rutaOrigen, string carpetaDestino)
    {
        if (Directory.Exists(rutaOrigen))
        {
            throw new NotSupportedException("LZMA comprime un único archivo; no soporta directorios directamente.");
        }

        if (!File.Exists(rutaOrigen))
        {
            throw new FileNotFoundException("No se encontró el origen a comprimir.", rutaOrigen);
        }

        string nombreBase = Path.GetFileName(rutaOrigen);
        string rutaLzma = Path.Combine(carpetaDestino, nombreBase + ".lz");

        using (var destino = File.Create(rutaLzma))
        using (var lzip = LZipStream.Create(destino, CompressionMode.Compress, false))
        using (var origen = File.OpenRead(rutaOrigen))
        {
            origen.CopyTo(lzip);
        }

        return rutaLzma;
    }
}
