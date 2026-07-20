using Backups.Core.Domain.Entities;

namespace Backups.Core.Ports.Out;

public interface IRegistroLogPort
{
    Task EscribirAsync(RegistroFisico registro, CancellationToken ct);

    Task<IReadOnlyList<LogRespaldo>> LeerHistorialAsync(CancellationToken ct);

    /// <summary>Ultimo hash de esa rutaOrigen cuyo estado sea COMPLETADO u OMITIDO_SIN_CAMBIOS. Null si no hay historial.</summary>
    Task<string?> ObtenerUltimoHashConfirmadoAsync(string rutaOrigen, CancellationToken ct);
}
