using Backups.Infrastructure.Network.Configuration;

namespace Backups.Network.Tests.Configuration;

public sealed class OpcionesTransmisionJsonTests
{
    private const string JsonValido = """
        {
          "DEST_SFTP_SECURE": {
            "protocolo": "SFTP",
            "host": "backup.cloud.com",
            "usuario": "root_ssh",
            "puerto": 2222,
            "contrasena": "clave-secreta",
            "rutaRemota": "/backups/",
            "timeoutSegundos": 45
          }
        }
        """;

    [Fact]
    public void ObtenerConexion_ConDestinoExistente_DevuelveTodosLosCamposDesdeElJson()
    {
        var opciones = new OpcionesTransmisionJson(JsonValido);

        ConexionDestino conexion = opciones.ObtenerConexion("DEST_SFTP_SECURE");

        Assert.Equal("DEST_SFTP_SECURE", conexion.Destino.Id);
        Assert.Equal("SFTP", conexion.Destino.Protocolo);
        Assert.Equal("backup.cloud.com", conexion.Destino.Host);
        Assert.Equal("root_ssh", conexion.Destino.Usuario);
        Assert.Equal(2222, conexion.Puerto);
        Assert.Equal("clave-secreta", conexion.Contrasena);
        Assert.Equal("/backups/", conexion.RutaRemota);
        Assert.Equal(45, conexion.TimeoutSegundos);
    }

    [Fact]
    public void ObtenerConexion_ConIdDesconocido_LanzaExcepcionClara()
    {
        var opciones = new OpcionesTransmisionJson(JsonValido);

        var excepcion = Assert.Throws<KeyNotFoundException>(() => opciones.ObtenerConexion("NO_EXISTE"));
        Assert.Contains("NO_EXISTE", excepcion.Message);
    }

    [Fact]
    public void Constructor_ConJsonInvalido_LanzaExcepcionClara()
    {
        Assert.Throws<InvalidOperationException>(() => new OpcionesTransmisionJson("{ esto no es json valido"));
    }

    [Fact]
    public void Constructor_ConDestinoIncompleto_LanzaExcepcionClara()
    {
        const string JsonIncompleto = """{ "DEST_1": { "puerto": 21 } }""";

        Assert.Throws<InvalidOperationException>(() => new OpcionesTransmisionJson(JsonIncompleto));
    }

    [Fact]
    public void DesdeVariablesDeEntorno_SinVariableConfigurada_LanzaExcepcionClara()
    {
        Environment.SetEnvironmentVariable("BACKUPS_TRANSMISION_RUTA_CONFIG", null);

        Assert.Throws<InvalidOperationException>(() => OpcionesTransmisionJson.DesdeVariablesDeEntorno());
    }

    [Fact]
    public void DesdeVariablesDeEntorno_ConArchivoValido_LeeLaRutaDesdeLaVariableDeEntorno_NoHardcodeada()
    {
        string rutaTemporal = Path.Combine(Path.GetTempPath(), "destinos-prueba-" + Guid.NewGuid() + ".json");
        File.WriteAllText(rutaTemporal, JsonValido);
        Environment.SetEnvironmentVariable("BACKUPS_TRANSMISION_RUTA_CONFIG", rutaTemporal);

        try
        {
            var opciones = OpcionesTransmisionJson.DesdeVariablesDeEntorno();
            ConexionDestino conexion = opciones.ObtenerConexion("DEST_SFTP_SECURE");

            Assert.Equal("backup.cloud.com", conexion.Destino.Host);
        }
        finally
        {
            Environment.SetEnvironmentVariable("BACKUPS_TRANSMISION_RUTA_CONFIG", null);
            File.Delete(rutaTemporal);
        }
    }
}
