namespace Backups.Infrastructure.Network.Transmisores;

// Abstrae el cliente real de FluentFTP/SSH.NET para poder probar la lógica de
// TransmisorFtp/TransmisorSftp/TransmisorSsh (dispatch, manejo de errores, Dispose) sin
// depender de un servidor real. Los adaptadores concretos (FtpClienteAdaptado,
// SftpClienteAdaptado, ScpClienteAdaptado) son el único código que toca la red de verdad.
internal interface IClienteTransmision : IDisposable
{
    void Connect();

    void SubirArchivo(string rutaLocal, string rutaRemota);
}
