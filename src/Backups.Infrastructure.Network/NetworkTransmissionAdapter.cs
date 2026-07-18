using Backups.Core.Ports.Out;
using Backups.Infrastructure.Network.Configuration;
using Backups.Infrastructure.Network.Transmisores;

namespace Backups.Infrastructure.Network;

public sealed class NetworkTransmissionAdapter : ITransmissionService
{
    private readonly IOpcionesTransmision _opciones;
    private readonly IReadOnlyDictionary<string, ITransmisor> _transmisores;

    public NetworkTransmissionAdapter()
        : this(OpcionesTransmisionJson.DesdeVariablesDeEntorno())
    {
    }

    public NetworkTransmissionAdapter(IOpcionesTransmision opciones)
        : this(opciones, TransmisoresPorDefecto())
    {
    }

    internal NetworkTransmissionAdapter(IOpcionesTransmision opciones, IReadOnlyDictionary<string, ITransmisor> transmisores)
    {
        _opciones = opciones;
        _transmisores = transmisores;
    }

    public bool EnviarArchivos(List<string> rutasArchivos, string idDestinoConfig)
    {
        ArgumentNullException.ThrowIfNull(rutasArchivos);
        ConexionDestino conexion = _opciones.ObtenerConexion(idDestinoConfig);
        string protocolo = conexion.Destino.Protocolo.Trim();

        if (string.IsNullOrEmpty(protocolo) || !_transmisores.TryGetValue(protocolo, out ITransmisor? transmisor))
        {
            throw new ArgumentException(
                $"Protocolo de transmisión no soportado: '{conexion.Destino.Protocolo}'. Valores válidos: FTP, SFTP, SSH.",
                nameof(idDestinoConfig));
        }

        return transmisor.Enviar(rutasArchivos, conexion);
    }

    private static IReadOnlyDictionary<string, ITransmisor> TransmisoresPorDefecto() =>
        new Dictionary<string, ITransmisor>(StringComparer.OrdinalIgnoreCase)
        {
            ["FTP"] = new TransmisorFtp(),
            ["SFTP"] = new TransmisorSftp(),
            ["SSH"] = new TransmisorSsh(),
        };
}
