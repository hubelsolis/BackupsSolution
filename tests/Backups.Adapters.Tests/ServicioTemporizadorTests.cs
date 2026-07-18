using System.Runtime.CompilerServices;
using Backups.Core;
using Backups.Core.Ports.Out;

namespace Backups.Adapters.Tests;

public class ServicioTemporizadorTests
{
    private sealed class TemporizadorFalso(int cantidadTicks) : ITemporizadorPort
    {
        public async IAsyncEnumerable<DateTimeOffset> TicksAsync([EnumeratorCancellation] CancellationToken ct)
        {
            for (var i = 0; i < cantidadTicks; i++)
            {
                yield return DateTimeOffset.UtcNow;
            }

            await Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    [Fact]
    public async Task EjecutarAsync_TickLlegaMientrasElAnteriorSigueEnCurso_LoOmiteYLoLoguea()
    {
        var primeraEjecucionEnCurso = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var puedeTerminarPrimeraEjecucion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var ejecuciones = 0;
        var mensajesOmitidos = new List<string>();

        async Task Accion(CancellationToken ct)
        {
            Interlocked.Increment(ref ejecuciones);
            primeraEjecucionEnCurso.TrySetResult();
            await puedeTerminarPrimeraEjecucion.Task.WaitAsync(ct);
        }

        await using var temporizadorFalso = new TemporizadorFalso(cantidadTicks: 2);
        await using var servicio = new ServicioTemporizador(temporizadorFalso, Accion, mensajesOmitidos.Add);

        var tareaEjecucion = servicio.EjecutarAsync(CancellationToken.None);

        await primeraEjecucionEnCurso.Task;
        puedeTerminarPrimeraEjecucion.TrySetResult();
        await tareaEjecucion;

        Assert.Equal(1, ejecuciones);
        Assert.Single(mensajesOmitidos);
    }

    [Fact]
    public async Task EjecutarAsync_SinSolapamiento_EjecutaCadaTick()
    {
        var ejecuciones = 0;
        Task Accion(CancellationToken ct)
        {
            Interlocked.Increment(ref ejecuciones);
            return Task.CompletedTask;
        }

        await using var temporizadorFalso = new TemporizadorFalso(cantidadTicks: 3);
        await using var servicio = new ServicioTemporizador(temporizadorFalso, Accion);

        await servicio.EjecutarAsync(CancellationToken.None);

        Assert.Equal(3, ejecuciones);
    }
}
