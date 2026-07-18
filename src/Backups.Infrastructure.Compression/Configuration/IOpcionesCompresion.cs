namespace Backups.Infrastructure.Compression.Configuration;

// PROPUESTO POR GRUPO 2 - NO MERGEAR SIN REVISIÓN
// IConfiguracionPort no existe en Backups.Core (main); esta es una abstracción propia
// del Grupo 2 mientras no haya un puerto central de configuración.
public interface IOpcionesCompresion
{
    string DirectorioSalida { get; }

    string? RutaEjecutableRar { get; }
}
