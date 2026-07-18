using Backups.Adapters.Infrastructure;

namespace Backups.Adapters.Tests;

public class ConfiguracionAdapterTests : IDisposable
{
    private readonly string _carpetaTemporal;
    private readonly string _rutaConfig;

    public ConfiguracionAdapterTests()
    {
        _carpetaTemporal = Path.Combine(Path.GetTempPath(), $"backups-config-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_carpetaTemporal);
        _rutaConfig = Path.Combine(_carpetaTemporal, "config.json");
    }

    public void Dispose()
    {
        if (Directory.Exists(_carpetaTemporal))
        {
            Directory.Delete(_carpetaTemporal, recursive: true);
        }
    }

    private void EscribirConfig(string json) => File.WriteAllText(_rutaConfig, json);

    [Fact]
    public void Getters_DevuelvenLosValoresDelConfigJson()
    {
        EscribirConfig("""
        {
          "logging": { "path": "./data/logs/backup.log" },
          "timer": { "intervaloSegundos": 3600 },
          "hash": { "algoritmoDefault": "SHA256" }
        }
        """);

        var adaptador = new ConfiguracionAdapter(_rutaConfig);

        Assert.Equal("./data/logs/backup.log", adaptador.ObtenerRutaLogTxt());
        Assert.Equal(3600, adaptador.ObtenerIntervaloTimerSegundos());
        Assert.Equal("SHA256", adaptador.ObtenerAlgoritmoHashDefault());
    }

    [Fact]
    public void ObtenerRutaLogTxt_SinClave_LanzaInvalidOperationException()
    {
        EscribirConfig("""{ "timer": { "intervaloSegundos": 3600 }, "hash": { "algoritmoDefault": "SHA256" } }""");

        var adaptador = new ConfiguracionAdapter(_rutaConfig);

        Assert.Throws<InvalidOperationException>(() => adaptador.ObtenerRutaLogTxt());
    }
}
