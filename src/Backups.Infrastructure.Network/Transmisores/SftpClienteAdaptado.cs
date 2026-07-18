using System.Diagnostics.CodeAnalysis;
using Backups.Infrastructure.Network.Configuration;
using Renci.SshNet;

namespace Backups.Infrastructure.Network.Transmisores;

[ExcludeFromCodeCoverage(Justification = "Envoltorio delgado sobre Renci.SshNet.SftpClient (conexión real); no se prueba en CI sin servidor SFTP real. La lógica de dispatch/errores se cubre en TransmisorSftp.")]
internal sealed class SftpClienteAdaptado : IClienteTransmision
{
    private readonly SftpClient _cliente;

    public SftpClienteAdaptado(ConexionDestino destino)
    {
        ArgumentNullException.ThrowIfNull(destino);
        _cliente = new SftpClient(destino.Destino.Host, destino.Puerto, destino.Destino.Usuario, destino.Contrasena);
        _cliente.ConnectionInfo.Timeout = TimeSpan.FromSeconds(destino.TimeoutSegundos);
    }

    public void Connect() => _cliente.Connect();

    public void SubirArchivo(string rutaLocal, string rutaRemota)
    {
        using FileStream flujoLocal = File.OpenRead(rutaLocal);

        // SSH.NET UploadFile usa el parámetro overwrite de forma POSICIONAL, no nombrada.
        _cliente.UploadFile(flujoLocal, rutaRemota, true);
    }

    public void Dispose() => _cliente.Dispose();
}
