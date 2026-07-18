using System.Diagnostics.CodeAnalysis;
using Backups.Infrastructure.Network.Configuration;
using FluentFTP;

namespace Backups.Infrastructure.Network.Transmisores;

[ExcludeFromCodeCoverage(Justification = "Envoltorio delgado sobre FluentFTP.FtpClient (conexión real); no se prueba en CI sin servidor FTP real. La lógica de dispatch/errores se cubre en TransmisorFtp.")]
internal sealed class FtpClienteAdaptado : IClienteTransmision
{
    private readonly FtpClient _cliente;
    private readonly string _idDestino;

    public FtpClienteAdaptado(ConexionDestino destino)
    {
        ArgumentNullException.ThrowIfNull(destino);
        _idDestino = destino.Destino.Id;
        _cliente = new FtpClient(destino.Destino.Host, destino.Destino.Usuario, destino.Contrasena, destino.Puerto);
        _cliente.Config.ConnectTimeout = destino.TimeoutSegundos * 1000;
    }

    public void Connect() => _cliente.Connect();

    public void SubirArchivo(string rutaLocal, string rutaRemota)
    {
        FtpStatus estado = _cliente.UploadFile(rutaLocal, rutaRemota, FtpRemoteExists.Overwrite, true);
        if (estado == FtpStatus.Failed)
        {
            throw new IOException($"FluentFTP no pudo subir un volumen al destino '{_idDestino}'.");
        }
    }

    public void Dispose() => _cliente.Dispose();
}
