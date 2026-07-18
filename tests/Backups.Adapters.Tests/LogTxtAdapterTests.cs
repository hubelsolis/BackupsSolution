using Backups.Adapters.Infrastructure;
using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.Out;
using NSubstitute;

namespace Backups.Adapters.Tests;

public class LogTxtAdapterTests : IDisposable
{
    private readonly string _carpetaTemporal;
    private readonly string _rutaLog;
    private readonly IConfiguracionPort _configuracion;

    public LogTxtAdapterTests()
    {
        _carpetaTemporal = Path.Combine(Path.GetTempPath(), $"backups-tests-{Guid.NewGuid()}");
        _rutaLog = Path.Combine(_carpetaTemporal, "backup.log");
        _configuracion = Substitute.For<IConfiguracionPort>();
        _configuracion.ObtenerRutaLogTxt().Returns(_rutaLog);
    }

    public void Dispose()
    {
        if (Directory.Exists(_carpetaTemporal))
        {
            Directory.Delete(_carpetaTemporal, recursive: true);
        }
    }

    [Fact]
    public async Task EscribirYLeer_RoundTrip_MismoContenido()
    {
        var adaptador = new LogTxtAdapter(_configuracion);
        var registro = new RegistroFisico(
            DateTimeOffset.UtcNow, Guid.NewGuid(), EstadoRespaldo.COMPLETADO, "/origen/x", "hash123", TipoDisparo.TIMER, "Respaldo completado correctamente.");

        await adaptador.EscribirAsync(registro, CancellationToken.None);
        var historial = await adaptador.LeerHistorialAsync(CancellationToken.None);

        var entrada = Assert.Single(historial);
        Assert.Equal(registro.BackupId, entrada.BackupId);
        Assert.Equal(registro.Estado, entrada.Estado);
        Assert.Equal(registro.MensajeTxt, entrada.MensajeTxt);
    }

    [Fact]
    public async Task EscrituraConcurrente_100Escrituras_100LineasIntegras()
    {
        var adaptador = new LogTxtAdapter(_configuracion);

        var tareas = Enumerable.Range(0, 100).Select(i =>
        {
            var registro = new RegistroFisico(
                DateTimeOffset.UtcNow, Guid.NewGuid(), EstadoRespaldo.COMPLETADO, $"/origen/{i}", $"hash{i}", TipoDisparo.TIMER, $"mensaje {i}");
            return adaptador.EscribirAsync(registro, CancellationToken.None);
        });

        await Task.WhenAll(tareas);

        var lineas = await File.ReadAllLinesAsync(_rutaLog);
        Assert.Equal(100, lineas.Length);
        Assert.All(lineas, linea => Assert.Equal(7, linea.Split('|').Length));

        var historial = await adaptador.LeerHistorialAsync(CancellationToken.None);
        Assert.Equal(100, historial.Count);
    }

    [Fact]
    public async Task ObtenerUltimoHashConfirmadoAsync_SinHistorial_DevuelveNull()
    {
        var adaptador = new LogTxtAdapter(_configuracion);
        Assert.Null(await adaptador.ObtenerUltimoHashConfirmadoAsync("/no/existe", CancellationToken.None));
    }

    [Fact]
    public async Task ObtenerUltimoHashConfirmadoAsync_ConsideraSoloCompletadoUOmitido()
    {
        var adaptador = new LogTxtAdapter(_configuracion);
        const string ruta = "/origen/y";

        await adaptador.EscribirAsync(new RegistroFisico(DateTimeOffset.UtcNow.AddMinutes(-2), Guid.NewGuid(), EstadoRespaldo.FALLIDO, ruta, "hash-fallido", TipoDisparo.TIMER, "m1"), CancellationToken.None);
        await adaptador.EscribirAsync(new RegistroFisico(DateTimeOffset.UtcNow.AddMinutes(-1), Guid.NewGuid(), EstadoRespaldo.COMPLETADO, ruta, "hash-bueno", TipoDisparo.TIMER, "m2"), CancellationToken.None);

        Assert.Equal("hash-bueno", await adaptador.ObtenerUltimoHashConfirmadoAsync(ruta, CancellationToken.None));
    }

    [Fact]
    public async Task EscribirAsync_LeeLaRutaDesdeIConfiguracionPort()
    {
        var adaptador = new LogTxtAdapter(_configuracion);
        var registro = new RegistroFisico(DateTimeOffset.UtcNow, Guid.NewGuid(), EstadoRespaldo.PROCESANDO, "/x", "h", TipoDisparo.BOTON_MANUAL, "m");

        await adaptador.EscribirAsync(registro, CancellationToken.None);

        _configuracion.Received().ObtenerRutaLogTxt();
    }
}
