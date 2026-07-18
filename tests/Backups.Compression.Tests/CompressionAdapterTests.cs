using System.IO.Compression;
using Backups.Infrastructure.Compression;
using Backups.Infrastructure.Compression.Configuration;
using NSubstitute;
using SharpCompress.Compressors.LZMA;
using LzmaCompressionMode = SharpCompress.Compressors.CompressionMode;

namespace Backups.Compression.Tests;

// Integración G1+G2 (ProcesadorPila real + CompressionService real) se validará al mergear
// ambas ramas: Backups.Core en 'main' no incluye ProcesadorPila (vive solo en la rama
// nucleo-de-control del Grupo 1), por lo que aquí no hay nada contra qué integrar todavía.
public sealed class CompressionAdapterTests : IDisposable
{
    private readonly string _dirOrigen;
    private readonly string _dirSalida;
    private readonly IOpcionesCompresion _opciones;

    public CompressionAdapterTests()
    {
        _dirOrigen = Path.Combine(Path.GetTempPath(), "adapter-tests-origen-" + Guid.NewGuid());
        _dirSalida = Path.Combine(Path.GetTempPath(), "adapter-tests-salida-" + Guid.NewGuid());
        Directory.CreateDirectory(_dirOrigen);
        _opciones = new OpcionesCompresion(_dirSalida, rutaEjecutableRar: null);
    }

    public void Dispose()
    {
        if (Directory.Exists(_dirOrigen))
        {
            Directory.Delete(_dirOrigen, true);
        }

        if (Directory.Exists(_dirSalida))
        {
            Directory.Delete(_dirSalida, true);
        }
    }

    [Fact]
    public void Zip_GeneraArchivoValidoYReabrible()
    {
        var adapter = new CompressionAdapter(_opciones);
        string origen = CrearArchivoOrigen(1024);

        var resultado = adapter.ComprimirYSegmentar(origen, "ZIP", limiteVolumenMb: 0);

        string rutaZip = Assert.Single(resultado);
        Assert.EndsWith(".zip", rutaZip);
        using var zip = ZipFile.OpenRead(rutaZip);
        var entrada = Assert.Single(zip.Entries);
        Assert.Equal(Path.GetFileName(origen), entrada.Name);
    }

    [Fact]
    public void Lzma_GeneraArchivoValidoYReabrible()
    {
        var adapter = new CompressionAdapter(_opciones);
        byte[] contenidoOriginal = new byte[2048];
        Random.Shared.NextBytes(contenidoOriginal);
        string origen = CrearArchivoOrigen(contenidoOriginal);

        var resultado = adapter.ComprimirYSegmentar(origen, "LZMA", limiteVolumenMb: 0);

        string rutaLzma = Assert.Single(resultado);
        Assert.EndsWith(".lz", rutaLzma);

        using var comprimido = File.OpenRead(rutaLzma);
        using var descompresor = LZipStream.Create(comprimido, LzmaCompressionMode.Decompress, false);
        using var destino = new MemoryStream();
        descompresor.CopyTo(destino);

        Assert.Equal(contenidoOriginal, destino.ToArray());
    }

    [Theory]
    [InlineData("zip")]
    [InlineData("ZIP")]
    [InlineData("Zip")]
    public void Algoritmo_EsCaseInsensitive(string algoritmo)
    {
        var adapter = new CompressionAdapter(_opciones);
        string origen = CrearArchivoOrigen(256);

        var resultado = adapter.ComprimirYSegmentar(origen, algoritmo, limiteVolumenMb: 0);

        Assert.Single(resultado);
    }

    [Fact]
    public void AlgoritmoDesconocido_LanzaExcepcionYNoCreaNada()
    {
        var adapter = new CompressionAdapter(_opciones);
        string origen = CrearArchivoOrigen(256);

        Assert.Throws<ArgumentException>(() => adapter.ComprimirYSegmentar(origen, "TARGZ", limiteVolumenMb: 0));

        Assert.False(Directory.Exists(_dirSalida) && Directory.EnumerateFileSystemEntries(_dirSalida).Any());
    }

    [Fact]
    public void Zip_ComprimeDirectorioCompleto()
    {
        var adapter = new CompressionAdapter(_opciones);
        string subdirectorio = Path.Combine(_dirOrigen, "carpeta-" + Guid.NewGuid());
        Directory.CreateDirectory(subdirectorio);
        File.WriteAllBytes(Path.Combine(subdirectorio, "a.dat"), [1, 2, 3]);
        File.WriteAllBytes(Path.Combine(subdirectorio, "b.dat"), [4, 5, 6]);

        var resultado = adapter.ComprimirYSegmentar(subdirectorio, "ZIP", limiteVolumenMb: 0);

        string rutaZip = Assert.Single(resultado);
        using var zip = ZipFile.OpenRead(rutaZip);
        Assert.Equal(2, zip.Entries.Count);
    }

    [Fact]
    public void AlgoritmoVacio_LanzaExcepcion()
    {
        var adapter = new CompressionAdapter(_opciones);
        string origen = CrearArchivoOrigen(256);

        Assert.Throws<ArgumentException>(() => adapter.ComprimirYSegmentar(origen, string.Empty, limiteVolumenMb: 0));
    }

    [Fact]
    public void Rar_SinEjecutableConfigurado_LanzaNotSupportedException()
    {
        var adapter = new CompressionAdapter(_opciones);
        string origen = CrearArchivoOrigen(256);

        Assert.Throws<NotSupportedException>(() => adapter.ComprimirYSegmentar(origen, "RAR", limiteVolumenMb: 0));
    }

    [Fact]
    public void DirectorioSalida_SeTomaDesdeOpciones_NoHardcodeado()
    {
        var opcionesFalsas = Substitute.For<IOpcionesCompresion>();
        opcionesFalsas.DirectorioSalida.Returns(_dirSalida);
        opcionesFalsas.RutaEjecutableRar.Returns((string?)null);
        var adapter = new CompressionAdapter(opcionesFalsas);
        string origen = CrearArchivoOrigen(256);

        var resultado = adapter.ComprimirYSegmentar(origen, "ZIP", limiteVolumenMb: 0);

        Assert.StartsWith(_dirSalida, resultado[0]);
        _ = opcionesFalsas.Received().DirectorioSalida;
    }

    private string CrearArchivoOrigen(int tamanoBytes)
    {
        var datos = new byte[tamanoBytes];
        Random.Shared.NextBytes(datos);
        return CrearArchivoOrigen(datos);
    }

    private string CrearArchivoOrigen(byte[] contenido)
    {
        string ruta = Path.Combine(_dirOrigen, Guid.NewGuid() + ".dat");
        File.WriteAllBytes(ruta, contenido);
        return ruta;
    }
}
