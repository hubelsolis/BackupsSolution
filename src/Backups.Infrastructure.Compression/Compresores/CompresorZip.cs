using ICSharpCode.SharpZipLib.Zip;

namespace Backups.Infrastructure.Compression.Compresores;

internal sealed class CompresorZip : ICompresor
{
    public string Comprimir(string rutaOrigen, string carpetaDestino)
    {
        string nombreBase = Path.GetFileName(rutaOrigen.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        string rutaZip = Path.Combine(carpetaDestino, nombreBase + ".zip");

        if (Directory.Exists(rutaOrigen))
        {
            var fastZip = new FastZip();
            fastZip.CreateZip(rutaZip, rutaOrigen, true, null);
            return rutaZip;
        }

        if (!File.Exists(rutaOrigen))
        {
            throw new FileNotFoundException("No se encontró el origen a comprimir.", rutaOrigen);
        }

        using var salida = File.Create(rutaZip);
        using var zipStream = new ZipOutputStream(salida);
        zipStream.SetLevel(9);
        var entry = new ZipEntry(Path.GetFileName(rutaOrigen)) { DateTime = DateTime.UtcNow };
        zipStream.PutNextEntry(entry);
        using (var origen = File.OpenRead(rutaOrigen))
        {
            origen.CopyTo(zipStream);
        }

        zipStream.CloseEntry();
        zipStream.Finish();
        return rutaZip;
    }
}
