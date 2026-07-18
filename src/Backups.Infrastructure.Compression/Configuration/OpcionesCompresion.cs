namespace Backups.Infrastructure.Compression.Configuration;

// PROPUESTO POR GRUPO 2 - NO MERGEAR SIN REVISIÓN
public sealed class OpcionesCompresion : IOpcionesCompresion
{
    private const string VariableDirectorioSalida = "BACKUPS_COMPRESION_DIRECTORIO_SALIDA";
    private const string VariableRutaRar = "BACKUPS_COMPRESION_RUTA_RAR";

    public OpcionesCompresion(string directorioSalida, string? rutaEjecutableRar)
    {
        DirectorioSalida = directorioSalida;
        RutaEjecutableRar = rutaEjecutableRar;
    }

    public string DirectorioSalida { get; }

    public string? RutaEjecutableRar { get; }

    public static OpcionesCompresion DesdeVariablesDeEntorno()
    {
        string directorioSalida = Environment.GetEnvironmentVariable(VariableDirectorioSalida)
            ?? Path.Combine(AppContext.BaseDirectory, "salida-compresion");
        string? rutaRar = Environment.GetEnvironmentVariable(VariableRutaRar);
        return new OpcionesCompresion(directorioSalida, rutaRar);
    }
}
