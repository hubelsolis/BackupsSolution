using Backups.Infrastructure.Network;
using Backups.Infrastructure.Network.Configuration;
using Backups.Infrastructure.Network.Domain;
using Backups.Infrastructure.Network.Transmisores;
using NSubstitute;

namespace Backups.Network.Tests;

// Integración Grupo1+Grupo3 (ProcesadorPila real + TransmissionService real) se validará al
// mergear ambas ramas: Backups.Core en 'main' no incluye ProcesadorPila (vive solo en la rama
// nucleo-de-control del Grupo 1), por lo que aquí no hay nada contra qué integrar todavía.
// Sí se prueba que la firma de ITransmissionService encaja exactamente con lo que
// ProcesadorPila invoca (bool EnviarArchivos(List<string>, string)) - ver
// ITransmissionServiceContratoTests.
public sealed class NetworkTransmissionAdapterTests
{
    private const string IdDestino = "DEST_SFTP_SECURE";

    [Fact]
    public void Constructor_ConOpcionesReales_ConstruyeLosTresTransmisoresPorDefecto()
    {
        var opciones = Substitute.For<IOpcionesTransmision>();

        // Solo verifica que la composición (opciones -> diccionario de ITransmisor reales)
        // no lanza. No se invoca EnviarArchivos: eso conectaría de verdad, prohibido en tests.
        var adapter = new NetworkTransmissionAdapter(opciones);

        Assert.NotNull(adapter);
    }

    [Fact]
    public void ProtocoloFtp_SeleccionaTransmisorFtp()
    {
        var (opciones, transmisores) = CrearDependencias(protocolo: "FTP");
        var adapter = new NetworkTransmissionAdapter(opciones, transmisores);

        adapter.EnviarArchivos(["a.zip"], IdDestino);

        transmisores["FTP"].Received(1).Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
        transmisores["SFTP"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
        transmisores["SSH"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
    }

    [Fact]
    public void ProtocoloSftp_SeleccionaTransmisorSftp()
    {
        var (opciones, transmisores) = CrearDependencias(protocolo: "SFTP");
        var adapter = new NetworkTransmissionAdapter(opciones, transmisores);

        adapter.EnviarArchivos(["a.zip"], IdDestino);

        transmisores["SFTP"].Received(1).Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
        transmisores["FTP"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
        transmisores["SSH"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
    }

    [Fact]
    public void ProtocoloSsh_SeleccionaTransmisorSsh()
    {
        var (opciones, transmisores) = CrearDependencias(protocolo: "SSH");
        var adapter = new NetworkTransmissionAdapter(opciones, transmisores);

        adapter.EnviarArchivos(["a.zip"], IdDestino);

        transmisores["SSH"].Received(1).Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
        transmisores["FTP"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
        transmisores["SFTP"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
    }

    [Theory]
    [InlineData("ftp")]
    [InlineData("FTP")]
    [InlineData("Ftp")]
    public void SeleccionDeProtocolo_EsCaseInsensitive(string protocolo)
    {
        var (opciones, transmisores) = CrearDependencias(protocolo);
        var adapter = new NetworkTransmissionAdapter(opciones, transmisores);

        bool resultado = adapter.EnviarArchivos(["a.zip"], IdDestino);

        Assert.True(resultado);
        transmisores["FTP"].Received(1).Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
    }

    [Fact]
    public void ProtocoloDesconocido_LanzaExcepcionYNoIntentaEnviar()
    {
        var (opciones, transmisores) = CrearDependencias(protocolo: "TARGZ");
        var adapter = new NetworkTransmissionAdapter(opciones, transmisores);

        Assert.Throws<ArgumentException>(() => adapter.EnviarArchivos(["a.zip"], IdDestino));

        transmisores["FTP"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
        transmisores["SFTP"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
        transmisores["SSH"].DidNotReceive().Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>());
    }

    [Fact]
    public void FallaDeConexion_DevuelveFalloSinLanzarExcepcion()
    {
        var (opciones, transmisores) = CrearDependencias(protocolo: "SFTP");
        transmisores["SFTP"].Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>()).Returns(false);
        var adapter = new NetworkTransmissionAdapter(opciones, transmisores);

        bool resultado = adapter.EnviarArchivos(["a.zip"], IdDestino);

        Assert.False(resultado);
    }

    [Fact]
    public void MultiplesVolumenes_IntentaSubirTodosEnUnaSolaLlamada()
    {
        var (opciones, transmisores) = CrearDependencias(protocolo: "FTP");
        var adapter = new NetworkTransmissionAdapter(opciones, transmisores);
        List<string> volumenes = ["parte1.zip.001", "parte1.zip.002", "parte1.zip.003"];

        adapter.EnviarArchivos(volumenes, IdDestino);

        transmisores["FTP"].Received(1).Enviar(
            Arg.Is<IEnumerable<string>>(rutas => rutas != null && rutas.SequenceEqual(volumenes)),
            Arg.Any<ConexionDestino>());
    }

    [Fact]
    public void HostPuertoYCredenciales_SeLeenDeLaAbstraccionDeConfiguracion_NoHardcodeados()
    {
        var conexionEsperada = new ConexionDestino
        {
            Destino = new DestinoConfig { Id = IdDestino, Protocolo = "SFTP", Host = "host-de-prueba.invalido", Usuario = "usuario-prueba" },
            Puerto = 2222,
            Contrasena = "clave-de-prueba",
            RutaRemota = "/ruta/de/prueba",
        };
        var opciones = Substitute.For<IOpcionesTransmision>();
        opciones.ObtenerConexion(IdDestino).Returns(conexionEsperada);
        var transmisorSftp = Substitute.For<ITransmisor>();
        var transmisores = new Dictionary<string, ITransmisor>(StringComparer.OrdinalIgnoreCase) { ["SFTP"] = transmisorSftp };
        var adapter = new NetworkTransmissionAdapter(opciones, transmisores);

        adapter.EnviarArchivos(["a.zip"], IdDestino);

        opciones.Received(1).ObtenerConexion(IdDestino);
        transmisorSftp.Received(1).Enviar(Arg.Any<IEnumerable<string>>(), conexionEsperada);
    }

    private static (IOpcionesTransmision Opciones, Dictionary<string, ITransmisor> Transmisores) CrearDependencias(string protocolo)
    {
        var conexion = new ConexionDestino
        {
            Destino = new DestinoConfig { Id = IdDestino, Protocolo = protocolo, Host = "host.invalido", Usuario = "usuario" },
            Puerto = 21,
            Contrasena = "contrasena",
            RutaRemota = "/backups/",
        };
        var opciones = Substitute.For<IOpcionesTransmision>();
        opciones.ObtenerConexion(IdDestino).Returns(conexion);

        var transmisores = new Dictionary<string, ITransmisor>(StringComparer.OrdinalIgnoreCase)
        {
            ["FTP"] = Substitute.For<ITransmisor>(),
            ["SFTP"] = Substitute.For<ITransmisor>(),
            ["SSH"] = Substitute.For<ITransmisor>(),
        };
        foreach (ITransmisor transmisor in transmisores.Values)
        {
            transmisor.Enviar(Arg.Any<IEnumerable<string>>(), Arg.Any<ConexionDestino>()).Returns(true);
        }

        return (opciones, transmisores);
    }
}
