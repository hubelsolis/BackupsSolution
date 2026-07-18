using System.Text;
using Backups.Adapters.Infrastructure;
using Backups.Core.Ports.Out;
using NSubstitute;

namespace Backups.Adapters.Tests;

public class Sha256HashAdapterTests
{
    [Fact]
    public async Task CalcularSha256Async_StringVacio_DevuelveVectorConocido()
    {
        var archivo = Substitute.For<IArchivoPort>();
        archivo.AbrirLectura(Arg.Any<string>()).Returns(_ => new MemoryStream([]));
        var adaptador = new Sha256HashAdapter(archivo);

        var hash = await adaptador.CalcularSha256Async("cualquier-ruta", CancellationToken.None);

        Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", hash);
    }

    [Fact]
    public async Task CalcularSha256Async_DevuelveHashEnMinusculasYDe64Caracteres()
    {
        var archivo = Substitute.For<IArchivoPort>();
        archivo.AbrirLectura(Arg.Any<string>()).Returns(_ => new MemoryStream(Encoding.UTF8.GetBytes("contenido de prueba")));
        var adaptador = new Sha256HashAdapter(archivo);

        var hash = await adaptador.CalcularSha256Async("cualquier-ruta", CancellationToken.None);

        Assert.Equal(64, hash.Length);
        Assert.Equal(hash, hash.ToLowerInvariant());
        Assert.Matches("^[0-9a-f]{64}$", hash);
    }

    [Fact]
    public async Task CalcularSha256Async_AbreElArchivoUnaSolaVez()
    {
        var archivo = Substitute.For<IArchivoPort>();
        archivo.AbrirLectura("ruta-x").Returns(_ => new MemoryStream(Encoding.UTF8.GetBytes("dato")));
        var adaptador = new Sha256HashAdapter(archivo);

        await adaptador.CalcularSha256Async("ruta-x", CancellationToken.None);

        archivo.Received(1).AbrirLectura("ruta-x");
    }
}
