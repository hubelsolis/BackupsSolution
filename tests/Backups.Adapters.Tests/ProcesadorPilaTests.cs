using Backups.Core;
using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.Out;
using NSubstitute;

namespace Backups.Adapters.Tests;

public class ProcesadorPilaTests
{
    private static TareaRespaldo CrearTarea() => new(
        Guid.NewGuid(),
        new SolicitudRespaldo
        {
            NombreCopia = "Copia",
            TipoDisparo = TipoDisparo.TIMER,
            RutaOrigen = "/origen/x",
            AlgoritmoCompresion = AlgoritmoCompresion.RAR,
            LimiteVolumenMb = 250,
            IdDestinoConfig = "destino-1",
        },
        "hash-actual");

    private static (IPilaEnviosPort pila, ICompressionService compresion, ITransmissionService transmision, IRegistroLogPort registroLog, List<EstadoRespaldo> estados) CrearFakes()
    {
        var pila = Substitute.For<IPilaEnviosPort>();
        var compresion = Substitute.For<ICompressionService>();
        var transmision = Substitute.For<ITransmissionService>();
        var registroLog = Substitute.For<IRegistroLogPort>();
        var estados = new List<EstadoRespaldo>();
        registroLog.When(x => x.EscribirAsync(Arg.Any<RegistroFisico>(), Arg.Any<CancellationToken>()))
            .Do(call => estados.Add(call.ArgAt<RegistroFisico>(0).Estado));
        return (pila, compresion, transmision, registroLog, estados);
    }

    private static void ConfigurarUnaTarea(IPilaEnviosPort pila, TareaRespaldo tarea)
    {
        var entregada = false;
        pila.TryDesapilar(out Arg.Any<TareaRespaldo>()).Returns(x =>
        {
            if (entregada)
            {
                x[0] = null!;
                return false;
            }

            entregada = true;
            x[0] = tarea;
            return true;
        });
    }

    [Fact]
    public async Task DrenarAsync_EnvioOk_TransicionaComprimiendoEnviandoCompletado()
    {
        var (pila, compresion, transmision, registroLog, estados) = CrearFakes();
        var tarea = CrearTarea();
        ConfigurarUnaTarea(pila, tarea);
        compresion.ComprimirYSegmentar(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns(["parte1.zip"]);
        transmision.EnviarArchivos(Arg.Any<List<string>>(), Arg.Any<string>()).Returns(true);

        var procesador = new ProcesadorPila(pila, compresion, transmision, registroLog);
        await procesador.DrenarAsync(CancellationToken.None);

        Assert.Equal([EstadoRespaldo.COMPRIMIENDO_VOLUMENES, EstadoRespaldo.ENVIANDO, EstadoRespaldo.COMPLETADO], estados);
        compresion.Received(1).ComprimirYSegmentar(tarea.Solicitud.RutaOrigen, "RAR", 250);
        transmision.Received(1).EnviarArchivos(Arg.Any<List<string>>(), tarea.Solicitud.IdDestinoConfig);
    }

    [Fact]
    public async Task DrenarAsync_EnviarArchivosDevuelveFalse_EstadoFinalFallido()
    {
        var (pila, compresion, transmision, registroLog, estados) = CrearFakes();
        ConfigurarUnaTarea(pila, CrearTarea());
        compresion.ComprimirYSegmentar(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns(["parte1.zip"]);
        transmision.EnviarArchivos(Arg.Any<List<string>>(), Arg.Any<string>()).Returns(false);

        var procesador = new ProcesadorPila(pila, compresion, transmision, registroLog);
        await procesador.DrenarAsync(CancellationToken.None);

        Assert.Equal(EstadoRespaldo.FALLIDO, estados[^1]);
    }

    [Fact]
    public async Task DrenarAsync_ComprimirYSegmentarLanzaExcepcion_EstadoFinalFallido_SinExcepcionNoControlada()
    {
        var (pila, compresion, transmision, registroLog, estados) = CrearFakes();
        ConfigurarUnaTarea(pila, CrearTarea());
        compresion.ComprimirYSegmentar(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
            .Returns(_ => throw new InvalidOperationException("boom"));

        var procesador = new ProcesadorPila(pila, compresion, transmision, registroLog);
        var excepcion = await Record.ExceptionAsync(() => procesador.DrenarAsync(CancellationToken.None));

        Assert.Null(excepcion);
        Assert.Equal(EstadoRespaldo.FALLIDO, estados[^1]);
        transmision.DidNotReceive().EnviarArchivos(Arg.Any<List<string>>(), Arg.Any<string>());
    }

    [Fact]
    public async Task DrenarAsync_PilaConDosTareas_ProcesaLasDos()
    {
        var (pila, compresion, transmision, registroLog, _) = CrearFakes();
        var pendientes = new List<TareaRespaldo> { CrearTarea(), CrearTarea() };
        var indice = 0;
        pila.TryDesapilar(out Arg.Any<TareaRespaldo>()).Returns(x =>
        {
            if (indice >= pendientes.Count)
            {
                x[0] = null!;
                return false;
            }

            x[0] = pendientes[indice++];
            return true;
        });
        compresion.ComprimirYSegmentar(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>()).Returns(["p.zip"]);
        transmision.EnviarArchivos(Arg.Any<List<string>>(), Arg.Any<string>()).Returns(true);

        var procesador = new ProcesadorPila(pila, compresion, transmision, registroLog);
        await procesador.DrenarAsync(CancellationToken.None);

        compresion.Received(2).ComprimirYSegmentar(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
        transmision.Received(2).EnviarArchivos(Arg.Any<List<string>>(), Arg.Any<string>());
    }
}
