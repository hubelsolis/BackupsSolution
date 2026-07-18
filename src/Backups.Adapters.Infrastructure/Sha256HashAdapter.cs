using System.Security.Cryptography;
using Backups.Core.Ports.Out;

namespace Backups.Adapters.Infrastructure;

/// <summary>
/// SHA-256 en streaming via SHA256.HashDataAsync (.NET 8). Nunca
/// File.ReadAllBytes. MD5 prohibido (el ejemplo del contrato es MD5 de 32
/// chars; contradice al Ing., que pidio SHA-256, y ademas MD5 es security
/// hotspot en SonarCloud).
/// </summary>
public sealed class Sha256HashAdapter : ICalculadoraHashPort
{
    private readonly IArchivoPort _archivo;

    public Sha256HashAdapter(IArchivoPort archivo)
    {
        _archivo = archivo;
    }

    public async Task<string> CalcularSha256Async(string rutaOrigen, CancellationToken ct)
    {
        var stream = _archivo.AbrirLectura(rutaOrigen);
        await using (stream.ConfigureAwait(false))
        {
            var hash = await SHA256.HashDataAsync(stream, ct).ConfigureAwait(false);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
