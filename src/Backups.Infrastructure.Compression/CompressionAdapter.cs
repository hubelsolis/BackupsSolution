using Backups.Core.Ports.Out;
using Backups.Infrastructure.Compression.Compresores;
using Backups.Infrastructure.Compression.Configuration;
using Backups.Infrastructure.Compression.Volumenes;

namespace Backups.Infrastructure.Compression;

public sealed class CompressionAdapter : ICompressionService
{
    private readonly IOpcionesCompresion _opciones;
    private readonly IReadOnlyDictionary<string, ICompresor> _compresores;

    public CompressionAdapter()
        : this(OpcionesCompresion.DesdeVariablesDeEntorno())
    {
    }

    public CompressionAdapter(IOpcionesCompresion opciones)
    {
        _opciones = opciones;
        _compresores = new Dictionary<string, ICompresor>(StringComparer.OrdinalIgnoreCase)
        {
            ["ZIP"] = new CompresorZip(),
            ["LZMA"] = new CompresorLzma(),
            ["RAR"] = new CompresorRar(opciones),
        };
    }

    public List<string> ComprimirYSegmentar(string rutaOrigen, string algoritmo, int limiteVolumenMb)
    {
        if (string.IsNullOrEmpty(algoritmo) || !_compresores.TryGetValue(algoritmo, out ICompresor? compresor))
        {
            throw new ArgumentException(
                $"Algoritmo de compresión no soportado: '{algoritmo}'. Valores válidos: ZIP, RAR, LZMA.",
                nameof(algoritmo));
        }

        Directory.CreateDirectory(_opciones.DirectorioSalida);
        string rutaComprimida = compresor.Comprimir(rutaOrigen, _opciones.DirectorioSalida);
        return SegmentadorVolumenes.Segmentar(rutaComprimida, limiteVolumenMb);
    }
}
