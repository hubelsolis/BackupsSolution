using Backups.Infrastructure.Compression.Configuration;

namespace Backups.Compression.Tests;

public sealed class OpcionesCompresionTests : IDisposable
{
    private const string VariableDirectorioSalida = "BACKUPS_COMPRESION_DIRECTORIO_SALIDA";
    private const string VariableRutaRar = "BACKUPS_COMPRESION_RUTA_RAR";

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(VariableDirectorioSalida, null);
        Environment.SetEnvironmentVariable(VariableRutaRar, null);
    }

    [Fact]
    public void DesdeVariablesDeEntorno_UsaValoresConfigurados()
    {
        string directorioEsperado = Path.Combine(Path.GetTempPath(), "opciones-tests-" + Guid.NewGuid());
        Environment.SetEnvironmentVariable(VariableDirectorioSalida, directorioEsperado);
        Environment.SetEnvironmentVariable(VariableRutaRar, "ruta-configurada-de-prueba");

        var opciones = OpcionesCompresion.DesdeVariablesDeEntorno();

        Assert.Equal(directorioEsperado, opciones.DirectorioSalida);
        Assert.Equal("ruta-configurada-de-prueba", opciones.RutaEjecutableRar);
    }

    [Fact]
    public void DesdeVariablesDeEntorno_SinConfigurar_UsaValorPorDefectoYRarNulo()
    {
        Environment.SetEnvironmentVariable(VariableDirectorioSalida, null);
        Environment.SetEnvironmentVariable(VariableRutaRar, null);

        var opciones = OpcionesCompresion.DesdeVariablesDeEntorno();

        Assert.False(string.IsNullOrWhiteSpace(opciones.DirectorioSalida));
        Assert.Null(opciones.RutaEjecutableRar);
    }
}
