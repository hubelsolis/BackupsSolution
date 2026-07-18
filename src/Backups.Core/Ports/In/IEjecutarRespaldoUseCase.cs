using Backups.Core.Domain.Entities;

namespace Backups.Core.Ports.In;

public interface IEjecutarRespaldoUseCase
{
    Task<RespuestaEjecucion> EjecutarAsync(SolicitudRespaldo solicitud, CancellationToken ct);
}
