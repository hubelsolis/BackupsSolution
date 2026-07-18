using Backups.Core.Ports.Out;

namespace Backups.Adapters.Infrastructure;

public sealed class ArchivoAdapter : IArchivoPort
{
    public bool Existe(string ruta) => File.Exists(ruta);

    public DateTime ObtenerUltimaModificacion(string ruta) => File.GetLastWriteTimeUtc(ruta);

    public long ObtenerTamano(string ruta) => new FileInfo(ruta).Length;

    public Stream AbrirLectura(string ruta) => new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read);
}
