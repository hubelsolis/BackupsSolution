namespace Backups.Infrastructure.Network.Configuration;

// PROPUESTO POR GRUPO 3 - NO MERGEAR SIN REVISIÓN.
// IConfiguracionPort no existe en Backups.Core (main); esta es una abstracción propia
// del Grupo 3 mientras no haya un puerto central de configuración ni un endpoint de
// resolución de idDestinoConfig (el contrato solo define POST /config/destinos).
public interface IOpcionesTransmision
{
    ConexionDestino ObtenerConexion(string idDestinoConfig);
}
