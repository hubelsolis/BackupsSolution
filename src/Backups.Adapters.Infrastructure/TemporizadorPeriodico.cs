using System.Runtime.CompilerServices;
using Backups.Core.Ports.Out;

namespace Backups.Adapters.Infrastructure;

/// <summary>PeriodicTimer (.NET 8). PROHIBIDO System.Timers.Timer, Thread.Sleep, async void.</summary>
public sealed class TemporizadorPeriodico : ITemporizadorPort
{
    private readonly PeriodicTimer _timer;

    public TemporizadorPeriodico(IConfiguracionPort configuracion)
    {
        var segundos = configuracion.ObtenerIntervaloTimerSegundos();
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(segundos));
    }

    public async IAsyncEnumerable<DateTimeOffset> TicksAsync([EnumeratorCancellation] CancellationToken ct)
    {
        while (await _timer.WaitForNextTickAsync(ct).ConfigureAwait(false))
        {
            yield return DateTimeOffset.UtcNow;
        }
    }

    public ValueTask DisposeAsync()
    {
        _timer.Dispose();
        return ValueTask.CompletedTask;
    }
}
