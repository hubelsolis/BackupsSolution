using System.Diagnostics.CodeAnalysis;
using Backups.Infrastructure.Network.Configuration;
using Renci.SshNet;

namespace Backups.Infrastructure.Network.Transmisores;

[ExcludeFromCodeCoverage(Justification = "Envoltorio delgado sobre Renci.SshNet.ScpClient (conexión real); no se prueba en CI sin servidor SSH real. La lógica de dispatch/errores se cubre en TransmisorSsh.")]
internal sealed class ScpClienteAdaptado : IClienteTransmision
{
    private readonly ScpClient _cliente;

    public ScpClienteAdaptado(ConexionDestino destino)
    {
        ArgumentNullException.ThrowIfNull(destino);
        _cliente = new ScpClient(destino.Destino.Host, destino.Puerto, destino.Destino.Usuario, destino.Contrasena);
        _cliente.ConnectionInfo.Timeout = TimeSpan.FromSeconds(destino.TimeoutSegundos);
    }

    public void Connect() => _cliente.Connect();

    public void SubirArchivo(string rutaLocal, string rutaRemota) => _cliente.Upload(new FileInfo(rutaLocal), rutaRemota);

    public void Dispose() => _cliente.Dispose();
}
