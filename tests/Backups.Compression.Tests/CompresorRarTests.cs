using Backups.Infrastructure.Compression.Compresores;

namespace Backups.Compression.Tests;

public sealed class CompresorRarTests
{
    [Fact]
    public void ConstruirRutaSalida_AgregaExtensionRarEnCarpetaDestino()
    {
        string rutaOrigen = Path.Combine("origen", "carpeta-a-respaldar");
        string carpetaDestino = Path.Combine("salida", "destino");

        string rutaRar = CompresorRar.ConstruirRutaSalida(rutaOrigen, carpetaDestino);

        Assert.Equal(Path.Combine(carpetaDestino, "carpeta-a-respaldar.rar"), rutaRar);
    }

    [Fact]
    public void ConstruirProcessStartInfo_UsaArgumentListSinConcatenarStrings()
    {
        var psi = CompresorRar.ConstruirProcessStartInfo("ruta con espacios\\rar.exe", "salida con espacios.rar", "origen con espacios.dat");

        Assert.Equal(["a", "-ep1", "salida con espacios.rar", "origen con espacios.dat"], psi.ArgumentList);
        Assert.False(psi.UseShellExecute);
        Assert.True(psi.RedirectStandardError);
        Assert.True(psi.CreateNoWindow);
    }
}
