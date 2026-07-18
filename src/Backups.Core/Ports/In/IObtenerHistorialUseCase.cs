using Backups.Core.Domain.Entities;

namespace Backups.Core.Ports.In;

public interface IObtenerHistorialUseCase
{
    Task<IReadOnlyList<LogRespaldo>> ObtenerHistorialAsync(CancellationToken ct);
}
