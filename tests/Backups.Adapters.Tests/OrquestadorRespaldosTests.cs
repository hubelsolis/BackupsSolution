using Backups.Core;
using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.Out;
using NSubstitute;

namespace Backups.Adapters.Tests;

public class OrquestadorRespaldosTests
{
    private static SolicitudRespaldo CrearSolicitudValida(string? ultimoHashConocido) => new()
    {
        NombreCopia = "Copia de prueba",
        TipoDisparo = TipoDisparo.TIMER,
        RutaOrigen = "/origen/archivo.dat",
        UltimoHashConocido = ultimoHashConocido,
        AlgoritmoCompresion = AlgoritmoCompresion.ZIP,
        LimiteVolumenMb = 100,
        IdDestinoConfig = "destino-1",
    };

    private static (IArchivoPort archivo, ICalculadoraHashPort hash, IRegistroLogPort registroLog, IPilaEnviosPort pila) CrearFakes()
    {
        var archivo = Substitute.For<IArchivoPort>();
        archivo.Existe(Arg.Any<string>()).Returns(true);
        return (archivo, Substitute.For<ICalculadoraHashPort>(), Substitute.For<IRegistroLogPort>(), Substitute.For<IPilaEnviosPort>());
    }

    [Fact]
    public async Task EjecutarAsync_HashIgual_RetornaOmitidoSinCambios_NoApila_MensajeExacto()
    {
        var (archivo, hash, registroLog, pila) = CrearFakes();
        hash.CalcularSha256Async(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("abc123");
        var solicitud = CrearSolicitudValida(ultimoHashConocido: "abc123");
        var orquestador = new OrquestadorRespaldos(archivo, hash, registroLog, pila);

        var respuesta = await orquestador.EjecutarAsync(solicitud, CancellationToken.None);

        Assert.Equal(MensajesContrato.EstadoInicialOmitidoSinCambios, respuesta.EstadoInicial);
        pila.DidNotReceive().Apilar(Arg.Any<TareaRespaldo>());
        await registroLog.Received(1).EscribirAsync(
            Arg.Is<RegistroFisico>(r => r!.Estado == EstadoRespaldo.OMITIDO_SIN_CAMBIOS && r.MensajeTxt == MensajesContrato.CopiaCanceladaSinCambios),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EjecutarAsync_HashDistinto_RetornaIniciado_Apila_LogProcesando()
    {
        var (archivo, hash, registroLog, pila) = CrearFakes();
        hash.CalcularSha256Async(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("hash-nuevo");
        var solicitud = CrearSolicitudValida(ultimoHashConocido: "hash-viejo");
        var orquestador = new OrquestadorRespaldos(archivo, hash, registroLog, pila);

        var respuesta = await orquestador.EjecutarAsync(solicitud, CancellationToken.None);

        Assert.Equal(MensajesContrato.EstadoInicialIniciado, respuesta.EstadoInicial);
        pila.Received(1).Apilar(Arg.Is<TareaRespaldo>(t => t!.BackupId == respuesta.BackupId && t.HashActual == "hash-nuevo"));
        await registroLog.Received(1).EscribirAsync(Arg.Is<RegistroFisico>(r => r!.Estado == EstadoRespaldo.PROCESANDO), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EjecutarAsync_UltimoHashConocidoNull_ConsultaElRegistroLog()
    {
        var (archivo, hash, registroLog, pila) = CrearFakes();
        hash.CalcularSha256Async(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("hash-actual");
        registroLog.ObtenerUltimoHashConfirmadoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("hash-actual");
        var solicitud = CrearSolicitudValida(ultimoHashConocido: null);
        var orquestador = new OrquestadorRespaldos(archivo, hash, registroLog, pila);

        await orquestador.EjecutarAsync(solicitud, CancellationToken.None);

        await registroLog.Received(1).ObtenerUltimoHashConfirmadoAsync(solicitud.RutaOrigen, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EjecutarAsync_SinHistorial_PrimeraCopia_RetornaIniciado()
    {
        var (archivo, hash, registroLog, pila) = CrearFakes();
        hash.CalcularSha256Async(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("hash-actual");
        registroLog.ObtenerUltimoHashConfirmadoAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((string?)null);
        var solicitud = CrearSolicitudValida(ultimoHashConocido: null);
        var orquestador = new OrquestadorRespaldos(archivo, hash, registroLog, pila);

        var respuesta = await orquestador.EjecutarAsync(solicitud, CancellationToken.None);

        Assert.Equal(MensajesContrato.EstadoInicialIniciado, respuesta.EstadoInicial);
        pila.Received(1).Apilar(Arg.Any<TareaRespaldo>());
    }

    [Fact]
    public async Task EjecutarAsync_ArchivoOrigenInexistente_RetornaFallido_SinExcepcionNoControlada()
    {
        var (archivo, hash, registroLog, pila) = CrearFakes();
        archivo.Existe(Arg.Any<string>()).Returns(false);
        var solicitud = CrearSolicitudValida(ultimoHashConocido: null);
        var orquestador = new OrquestadorRespaldos(archivo, hash, registroLog, pila);

        RespuestaEjecucion? respuesta = null;
        var excepcion = await Record.ExceptionAsync(async () => respuesta = await orquestador.EjecutarAsync(solicitud, CancellationToken.None));

        Assert.Null(excepcion);
        Assert.Equal(nameof(EstadoRespaldo.FALLIDO), respuesta!.EstadoInicial);
        await hash.DidNotReceive().CalcularSha256Async(Arg.Any<string>(), Arg.Any<CancellationToken>());
        pila.DidNotReceive().Apilar(Arg.Any<TareaRespaldo>());
    }

    [Fact]
    public async Task EjecutarAsync_CamposRequeridosFaltantes_RetornaFallido_SinConsultarHash()
    {
        var (archivo, hash, registroLog, pila) = CrearFakes();
        var solicitud = CrearSolicitudValida(ultimoHashConocido: null) with { IdDestinoConfig = string.Empty };
        var orquestador = new OrquestadorRespaldos(archivo, hash, registroLog, pila);

        var respuesta = await orquestador.EjecutarAsync(solicitud, CancellationToken.None);

        Assert.Equal(nameof(EstadoRespaldo.FALLIDO), respuesta.EstadoInicial);
        await hash.DidNotReceive().CalcularSha256Async(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(TipoDisparo.TIMER)]
    [InlineData(TipoDisparo.BOTON_MANUAL)]
    public async Task EjecutarAsync_AmbosDisparos_SonValidos_YSeRegistraCual(TipoDisparo tipoDisparo)
    {
        var (archivo, hash, registroLog, pila) = CrearFakes();
        hash.CalcularSha256Async(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("hash-x");
        var solicitud = CrearSolicitudValida(ultimoHashConocido: "otro-hash") with { TipoDisparo = tipoDisparo };
        var orquestador = new OrquestadorRespaldos(archivo, hash, registroLog, pila);

        await orquestador.EjecutarAsync(solicitud, CancellationToken.None);

        await registroLog.Received(1).EscribirAsync(Arg.Is<RegistroFisico>(r => r!.TipoDisparo == tipoDisparo), Arg.Any<CancellationToken>());
    }
}
