namespace Backups.Core.Ports.Out;

public interface IArchivoPort
{
    bool Existe(string ruta);

    DateTime ObtenerUltimaModificacion(string ruta);

    long ObtenerTamano(string ruta);

    Stream AbrirLectura(string ruta);
}
