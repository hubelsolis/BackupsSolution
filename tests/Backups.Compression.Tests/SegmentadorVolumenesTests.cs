using System.Security.Cryptography;
using Backups.Infrastructure.Compression.Volumenes;

namespace Backups.Compression.Tests;

public sealed class SegmentadorVolumenesTests : IDisposable
{
    private readonly string _directorio;

    public SegmentadorVolumenesTests()
    {
        _directorio = Path.Combine(Path.GetTempPath(), "segmentador-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(_directorio);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directorio))
        {
            Directory.Delete(_directorio, true);
        }
    }

    [Fact]
    public void ArchivoMayorQueLimite_GeneraVariosVolumenesOrdenables()
    {
        string ruta = CrearArchivo(tamanoBytes: 5 * 1024 * 1024);

        var volumenes = SegmentadorVolumenes.Segmentar(ruta, limiteVolumenMb: 2);

        Assert.Equal(3, volumenes.Count);
        Assert.Equal(ruta + ".001", volumenes[0]);
        Assert.Equal(ruta + ".002", volumenes[1]);
        Assert.Equal(ruta + ".003", volumenes[2]);
        Assert.All(volumenes, v => Assert.True(File.Exists(v)));
        Assert.False(File.Exists(ruta));
    }

    [Fact]
    public void ArchivoMenorQueLimite_NoParteYDevuelveUnSoloVolumen()
    {
        string ruta = CrearArchivo(tamanoBytes: 1024);

        var volumenes = SegmentadorVolumenes.Segmentar(ruta, limiteVolumenMb: 10);

        Assert.Single(volumenes);
        Assert.Equal(ruta, volumenes[0]);
        Assert.True(File.Exists(ruta));
    }

    [Fact]
    public void LimiteVolumenCeroOMenor_NoParte()
    {
        string ruta = CrearArchivo(tamanoBytes: 5 * 1024 * 1024);

        var volumenes = SegmentadorVolumenes.Segmentar(ruta, limiteVolumenMb: 0);

        Assert.Single(volumenes);
        Assert.Equal(ruta, volumenes[0]);
    }

    [Fact]
    public void SplitYReconstruccion_Sha256IdenticoAlOriginal()
    {
        string ruta = CrearArchivo(tamanoBytes: (3 * 1024 * 1024) + 777);
        byte[] hashOriginal = SHA256.HashData(File.ReadAllBytes(ruta));

        var volumenes = SegmentadorVolumenes.Segmentar(ruta, limiteVolumenMb: 1);

        using var reconstruido = new MemoryStream();
        foreach (string volumen in volumenes)
        {
            using var flujoVolumen = File.OpenRead(volumen);
            flujoVolumen.CopyTo(reconstruido);
        }

        byte[] hashReconstruido = SHA256.HashData(reconstruido.ToArray());
        Assert.Equal(hashOriginal, hashReconstruido);
    }

    private string CrearArchivo(int tamanoBytes)
    {
        string ruta = Path.Combine(_directorio, Guid.NewGuid() + ".bin");
        var datos = new byte[tamanoBytes];
        Random.Shared.NextBytes(datos);
        File.WriteAllBytes(ruta, datos);
        return ruta;
    }
}
