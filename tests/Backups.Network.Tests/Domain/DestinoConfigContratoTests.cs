using System.Text.Json;
using Backups.Infrastructure.Network.Domain;

namespace Backups.Network.Tests.Domain;

// Verifica que DestinoConfig serializa exactamente como el schema DestinoConfig del
// contrato OpenAPI v1.3.0 (id/protocolo/host/usuario, literales de protocolo en mayúsculas).
public sealed class DestinoConfigContratoTests
{
    [Theory]
    [InlineData("FTP")]
    [InlineData("SFTP")]
    [InlineData("SSH")]
    public void Protocolo_SerializaAlLiteralExactoDelContrato(string protocolo)
    {
        var destino = new DestinoConfig { Id = "DEST_1", Protocolo = protocolo, Host = "backup.cloud.com", Usuario = "root_ssh" };

        string json = JsonSerializer.Serialize(destino);

        using JsonDocument documento = JsonDocument.Parse(json);
        Assert.Equal("DEST_1", documento.RootElement.GetProperty("id").GetString());
        Assert.Equal(protocolo, documento.RootElement.GetProperty("protocolo").GetString());
        Assert.Equal("backup.cloud.com", documento.RootElement.GetProperty("host").GetString());
        Assert.Equal("root_ssh", documento.RootElement.GetProperty("usuario").GetString());
    }

    [Fact]
    public void Deserializacion_DesdeJsonDelContrato_MapeaLosCamposExactos()
    {
        const string JsonContrato = """
            {
              "id": "DEST_SFTP_SECURE",
              "protocolo": "SFTP",
              "host": "backup.cloud.com",
              "usuario": "root_ssh"
            }
            """;

        var destino = JsonSerializer.Deserialize<DestinoConfig>(JsonContrato);

        Assert.NotNull(destino);
        Assert.Equal("DEST_SFTP_SECURE", destino.Id);
        Assert.Equal("SFTP", destino.Protocolo);
        Assert.Equal("backup.cloud.com", destino.Host);
        Assert.Equal("root_ssh", destino.Usuario);
    }
}
