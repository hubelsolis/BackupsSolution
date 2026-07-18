using Backups.Infrastructure.Network.Configuration;
using Backups.Infrastructure.Network.Domain;
using Backups.Infrastructure.Network.Transmisores;
using NSubstitute;

namespace Backups.Network.Tests.Transmisores;

public sealed class TransmisorSftpTests
{
    private static readonly ConexionDestino Destino = new()
    {
        Destino = new DestinoConfig { Id = "DEST_SFTP", Protocolo = "SFTP", Host = "host.invalido", Usuario = "usuario" },
        Puerto = 22,
        Contrasena = "contrasena",
        RutaRemota = "/backups/",
    };

    [Fact]
    public void Enviar_ConexionYSubidaExitosas_DevuelveTrueYCierraLaConexion()
    {
        var cliente = Substitute.For<IClienteTransmision>();
        var transmisor = new TransmisorSftp(_ => cliente);

        bool resultado = transmisor.Enviar(["a.zip"], Destino);

        Assert.True(resultado);
        cliente.Received(1).Connect();
        cliente.Received(1).SubirArchivo("a.zip", "/backups/a.zip");
        cliente.Received(1).Dispose();
    }

    [Fact]
    public void Enviar_FalloAlConectar_DevuelveFalseSinLanzarYCierraLaConexion()
    {
        var cliente = Substitute.For<IClienteTransmision>();
        cliente.When(c => c.Connect()).Do(_ => throw new IOException("host inalcanzable"));
        var transmisor = new TransmisorSftp(_ => cliente);

        bool resultado = transmisor.Enviar(["a.zip"], Destino);

        Assert.False(resultado);
        cliente.Received(1).Dispose();
    }

    [Fact]
    public void Enviar_FalloAlSubir_DevuelveFalseSinLanzarYCierraLaConexion()
    {
        var cliente = Substitute.For<IClienteTransmision>();
        cliente.When(c => c.SubirArchivo(Arg.Any<string>(), Arg.Any<string>())).Do(_ => throw new IOException("subida fallida"));
        var transmisor = new TransmisorSftp(_ => cliente);

        bool resultado = transmisor.Enviar(["a.zip"], Destino);

        Assert.False(resultado);
        cliente.Received(1).Dispose();
    }

    [Fact]
    public void Enviar_MultiplesVolumenes_SubeTodosEnOrden()
    {
        var cliente = Substitute.For<IClienteTransmision>();
        var transmisor = new TransmisorSftp(_ => cliente);
        List<string> volumenes = ["p1.zip.001", "p1.zip.002", "p1.zip.003"];

        transmisor.Enviar(volumenes, Destino);

        Received.InOrder(() =>
        {
            cliente.SubirArchivo("p1.zip.001", "/backups/p1.zip.001");
            cliente.SubirArchivo("p1.zip.002", "/backups/p1.zip.002");
            cliente.SubirArchivo("p1.zip.003", "/backups/p1.zip.003");
        });
    }
}
