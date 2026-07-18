using Backups.Infrastructure.Network.Configuration;

namespace Backups.Infrastructure.Network.Transmisores;

internal sealed class TransmisorFtp : ITransmisor
{
    private readonly Func<ConexionDestino, IClienteTransmision> _fabricaCliente;

    public TransmisorFtp()
        : this(destino => new FtpClienteAdaptado(destino))
    {
    }

    internal TransmisorFtp(Func<ConexionDestino, IClienteTransmision> fabricaCliente)
    {
        _fabricaCliente = fabricaCliente;
    }

    public bool Enviar(IEnumerable<string> rutasLocales, ConexionDestino destino)
    {
        ArgumentNullException.ThrowIfNull(rutasLocales);
        ArgumentNullException.ThrowIfNull(destino);
        try
        {
            using IClienteTransmision cliente = _fabricaCliente(destino);
            cliente.Connect();

            foreach (string rutaLocal in rutasLocales)
            {
                string rutaRemota = RutaRemotaHelper.Combinar(destino.RutaRemota, rutaLocal);
                cliente.SubirArchivo(rutaLocal, rutaRemota);
            }

            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Console.Error.WriteLine($"[Grupo3][FTP] Fallo al transmitir al destino '{destino.Destino.Id}': {ex.Message}");
            return false;
        }
    }
}
