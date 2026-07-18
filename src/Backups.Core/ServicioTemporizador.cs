using Backups.Core.Ports.Out;

namespace Backups.Core;

/// <summary>
/// TIPO DISPARO = TIMER. Cadencia real via ITemporizadorPort (PeriodicTimer en
/// el adaptador). Anti-solapamiento: el handler de cada tick corre en
/// background (no bloquea el bucle de ticks); si un tick llega mientras el
/// anterior sigue en curso, se salta y se loguea.
/// </summary>
public sealed class ServicioTemporizador : IAsyncDisposable
{
    private readonly ITemporizadorPort _temporizador;
    private readonly Func<CancellationToken, Task> _accionPorTick;
    private readonly Action<string> _logOmision;
    private readonly SemaphoreSlim _semaforo = new(1, 1);
    private Task _ultimaEjecucion = Task.CompletedTask;

    public ServicioTemporizador(
        ITemporizadorPort temporizador,
        Func<CancellationToken, Task> accionPorTick,
        Action<string>? logOmision = null)
    {
        _temporizador = temporizador;
        _accionPorTick = accionPorTick;
        _logOmision = logOmision ?? (_ => { });
    }

    public async Task EjecutarAsync(CancellationToken ct)
    {
        await foreach (var _ in _temporizador.TicksAsync(ct).WithCancellation(ct).ConfigureAwait(false))
        {
            if (!await _semaforo.WaitAsync(0, ct).ConfigureAwait(false))
            {
                _logOmision("Tick de timer omitido: la ejecucion anterior sigue en curso.");
                continue;
            }

            _ultimaEjecucion = EjecutarTickAsync(ct);
        }

        await _ultimaEjecucion.ConfigureAwait(false);
    }

    private async Task EjecutarTickAsync(CancellationToken ct)
    {
        try
        {
            await _accionPorTick(ct).ConfigureAwait(false);
        }
        finally
        {
            _semaforo.Release();
        }
    }

    public ValueTask DisposeAsync() => _temporizador.DisposeAsync();
}
