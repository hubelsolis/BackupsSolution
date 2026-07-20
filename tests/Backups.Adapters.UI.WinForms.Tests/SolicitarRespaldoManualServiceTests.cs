using Backups.Adapters.UI.WinForms.Logica;
using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.In;
using NSubstitute;

namespace Backups.Adapters.UI.WinForms.Tests;

public class SolicitarRespaldoManualServiceTests
{
    private static DatosFormularioRespaldo DatosValidos(
        AlgoritmoCompresion algoritmo = AlgoritmoCompresion.ZIP,
        int limiteVolumenMb = 100) =>
        new("Respaldo_Contabilidad", @"D:\Origen", algoritmo, limiteVolumenMb, "DEST_SFTP_SECURE");

    [Fact]
    public async Task EjecutarAsync_ConDatosValidos_ConstruyeSolicitudYLlamaAlCasoDeUso()
    {
        var useCase = Substitute.For<IEjecutarRespaldoUseCase>();
        var respuestaEsperada = new RespuestaEjecucion { BackupId = Guid.NewGuid(), EstadoInicial = "INICIADO" };
        useCase.EjecutarAsync(Arg.Any<SolicitudRespaldo>(), Arg.Any<CancellationToken>()).Returns(respuestaEsperada);
        var servicio = new SolicitarRespaldoManualService(useCase);

        var resultado = await servicio.EjecutarAsync(DatosValidos(), CancellationToken.None);

        Assert.True(resultado.Exitoso);
        Assert.Same(respuestaEsperada, resultado.Respuesta);
        await useCase.Received(1).EjecutarAsync(
            Arg.Is<SolicitudRespaldo>(s =>
                s.NombreCopia == "Respaldo_Contabilidad" &&
                s.RutaOrigen == @"D:\Origen" &&
                s.IdDestinoConfig == "DEST_SFTP_SECURE"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EjecutarAsync_BotonManual_UsaTipoDisparoBotonManual()
    {
        var useCase = Substitute.For<IEjecutarRespaldoUseCase>();
        useCase.EjecutarAsync(Arg.Any<SolicitudRespaldo>(), Arg.Any<CancellationToken>())
            .Returns(new RespuestaEjecucion { BackupId = Guid.NewGuid(), EstadoInicial = "INICIADO" });
        var servicio = new SolicitarRespaldoManualService(useCase);

        await servicio.EjecutarAsync(DatosValidos(), CancellationToken.None);

        await useCase.Received(1).EjecutarAsync(
            Arg.Is<SolicitudRespaldo>(s => s.TipoDisparo == TipoDisparo.BOTON_MANUAL),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(AlgoritmoCompresion.ZIP)]
    [InlineData(AlgoritmoCompresion.RAR)]
    [InlineData(AlgoritmoCompresion.LZMA)]
    public async Task EjecutarAsync_MapeaElAlgoritmoSeleccionadoAlLiteralDelContrato(AlgoritmoCompresion algoritmo)
    {
        var useCase = Substitute.For<IEjecutarRespaldoUseCase>();
        useCase.EjecutarAsync(Arg.Any<SolicitudRespaldo>(), Arg.Any<CancellationToken>())
            .Returns(new RespuestaEjecucion { BackupId = Guid.NewGuid(), EstadoInicial = "INICIADO" });
        var servicio = new SolicitarRespaldoManualService(useCase);

        await servicio.EjecutarAsync(DatosValidos(algoritmo), CancellationToken.None);

        await useCase.Received(1).EjecutarAsync(
            Arg.Is<SolicitudRespaldo>(s => s.AlgoritmoCompresion == algoritmo),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EjecutarAsync_PasaElLimiteVolumenMbTalCualLoIngresoElUsuario()
    {
        var useCase = Substitute.For<IEjecutarRespaldoUseCase>();
        useCase.EjecutarAsync(Arg.Any<SolicitudRespaldo>(), Arg.Any<CancellationToken>())
            .Returns(new RespuestaEjecucion { BackupId = Guid.NewGuid(), EstadoInicial = "INICIADO" });
        var servicio = new SolicitarRespaldoManualService(useCase);

        await servicio.EjecutarAsync(DatosValidos(limiteVolumenMb: 250), CancellationToken.None);

        await useCase.Received(1).EjecutarAsync(
            Arg.Is<SolicitudRespaldo>(s => s.LimiteVolumenMb == 250),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EjecutarAsync_SinCarpetaDeOrigen_NoInvocaElCasoDeUsoYDevuelveError()
    {
        var useCase = Substitute.For<IEjecutarRespaldoUseCase>();
        var servicio = new SolicitarRespaldoManualService(useCase);
        var datos = DatosValidos() with { RutaOrigen = string.Empty };

        var resultado = await servicio.EjecutarAsync(datos, CancellationToken.None);

        Assert.False(resultado.Exitoso);
        Assert.Contains(resultado.Errores, e => e.Contains("origen", StringComparison.OrdinalIgnoreCase));
        await useCase.DidNotReceive().EjecutarAsync(Arg.Any<SolicitudRespaldo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EjecutarAsync_SinDestino_NoInvocaElCasoDeUsoYDevuelveError()
    {
        var useCase = Substitute.For<IEjecutarRespaldoUseCase>();
        var servicio = new SolicitarRespaldoManualService(useCase);
        var datos = DatosValidos() with { IdDestinoConfig = string.Empty };

        var resultado = await servicio.EjecutarAsync(datos, CancellationToken.None);

        Assert.False(resultado.Exitoso);
        Assert.Contains(resultado.Errores, e => e.Contains("destino", StringComparison.OrdinalIgnoreCase));
        await useCase.DidNotReceive().EjecutarAsync(Arg.Any<SolicitudRespaldo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EjecutarAsync_ConLimiteVolumenMbCeroOMenor_NoInvocaElCasoDeUso()
    {
        var useCase = Substitute.For<IEjecutarRespaldoUseCase>();
        var servicio = new SolicitarRespaldoManualService(useCase);
        var datos = DatosValidos(limiteVolumenMb: 0);

        var resultado = await servicio.EjecutarAsync(datos, CancellationToken.None);

        Assert.False(resultado.Exitoso);
        await useCase.DidNotReceive().EjecutarAsync(Arg.Any<SolicitudRespaldo>(), Arg.Any<CancellationToken>());
    }
}
