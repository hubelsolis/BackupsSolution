using Backups.Infrastructure.Network.Domain;

namespace Backups.Infrastructure.Network.Configuration;

// PROPUESTO POR GRUPO 3 - NO MERGEAR SIN REVISIÓN.
// Datos de conexión que el contrato NO define en DestinoConfig (puerto, contraseña, ruta
// remota, timeout). Grupo 3 los resuelve vía IOpcionesTransmision, nunca hardcodeados.
public sealed record ConexionDestino
{
    public required DestinoConfig Destino { get; init; }

    public required int Puerto { get; init; }

    public string Contrasena { get; init; } = string.Empty;

    public string RutaRemota { get; init; } = string.Empty;

    public int TimeoutSegundos { get; init; } = 30;
}
