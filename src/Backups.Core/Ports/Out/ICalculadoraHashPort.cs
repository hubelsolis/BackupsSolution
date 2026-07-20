namespace Backups.Core.Ports.Out;

public interface ICalculadoraHashPort
{
    Task<string> CalcularSha256Async(string rutaOrigen, CancellationToken ct);
}
