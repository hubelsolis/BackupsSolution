namespace Backups.Core.Ports.Out;

/// <summary>Abstraccion del Timer para testear sin esperar tiempo real. La implementacion real usa PeriodicTimer.</summary>
public interface ITemporizadorPort : IAsyncDisposable
{
    IAsyncEnumerable<DateTimeOffset> TicksAsync(CancellationToken ct);
}
