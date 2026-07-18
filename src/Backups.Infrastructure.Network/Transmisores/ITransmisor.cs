using Backups.Infrastructure.Network.Configuration;

namespace Backups.Infrastructure.Network.Transmisores;

internal interface ITransmisor
{
    bool Enviar(IEnumerable<string> rutasLocales, ConexionDestino destino);
}
