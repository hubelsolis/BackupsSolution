using Backups.Infrastructure.Network.Configuration;
using Backups.Infrastructure.Network.Domain;
using Backups.Infrastructure.Network.Transmisores;
using NSubstitute;

namespace Backups.Network.Tests.Transmisores;

public sealed class TransmisorFtpTests
{
    private static readonly ConexionDestino Destino = new()
    {
        Destino = new DestinoConfig { Id = "DEST_FTP", Protocolo = "FTP", Host = "host.invalido", Usuario = "usuario" },
        Puerto = 21,
        Contrasena = "contrasena",
        RutaRemota = "/backups/",
    };

    [Fact]
    public void Enviar_ConexionYSubidaExitosas_DevuelveTrueYCierraLaConexion()
    {
        var cliente = Substitute.For<IClienteTransmision>();
        var transmisor = new TransmisorFtp(_ => cliente);

        bool resultado = transmisor.Enviar(["a.zip", "b.zip"], Destino);

        Assert.True(resultado);
        cliente.Received(1).Connect();
        cliente.Received(1).SubirArchivo("a.zip", "/backups/a.zip");
        cliente.Received(1).SubirArchivo("b.zip", "/backups/b.zip");
        cliente.Received(1).Dispose();
    }

    [Fact]
    public void Enviar_FalloAlConectar_DevuelveFalseSinLanzarYCierraLaConexion()
    {
        var cliente = Substitute.For<IClienteTransmision>();
        cliente.When(c => c.Connect()).Do(_ => throw new IOException("host inalcanzable"));
        var transmisor = new TransmisorFtp(_ => cliente);

        bool resultado = transmisor.Enviar(["a.zip"], Destino);

        Assert.False(resultado);
        cliente.Received(1).Dispose();
    }

    [Fact]
    public void Enviar_FalloAlSubir_DevuelveFalseSinLanzarYCierraLaConexion()
    {
        var cliente = Substitute.For<IClienteTransmision>();
        cliente.When(c => c.SubirArchivo(Arg.Any<string>(), Arg.Any<string>())).Do(_ => throw new IOException("subida fallida"));
        var transmisor = new TransmisorFtp(_ => cliente);

        bool resultado = transmisor.Enviar(["a.zip"], Destino);

        Assert.False(resultado);
        cliente.Received(1).Dispose();
    }
}
