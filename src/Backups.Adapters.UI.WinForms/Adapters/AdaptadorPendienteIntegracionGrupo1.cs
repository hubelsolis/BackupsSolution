using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.In;

namespace Backups.Adapters.UI.WinForms.Adapters;

// PROPUESTO POR GRUPO 4 - NO MERGEAR SIN REVISIÓN
// A la fecha, main no trae el OrquestadorRespaldos real del Grupo 1 (existe
// en su rama nucleo-de-control, no mergeada). Este adaptador es un eco en
// memoria que permite compilar, probar y demostrar la UI de forma aislada.
// Debe eliminarse y reemplazarse por el registro DI del caso de uso real de
// Backups.Core cuando esa rama se mergee a main. Ver CHANGELOG.md.
public sealed class AdaptadorPendienteIntegracionGrupo1 : IEjecutarRespaldoUseCase, IObtenerHistorialUseCase
{
    private readonly List<LogRespaldo> _historialEnMemoria = new();

    public Task<RespuestaEjecucion> EjecutarAsync(SolicitudRespaldo solicitud, CancellationToken ct)
    {
        var backupId = Guid.NewGuid();
        var registro = new LogRespaldo
        {
            BackupId = backupId,
            Estado = EstadoRespaldo.PROCESANDO,
            MensajeTxt = $"[Pendiente integración Grupo 1] Solicitud '{solicitud.NombreCopia}' " +
                         $"recibida por la UI (origen={solicitud.RutaOrigen}, " +
                         $"algoritmo={solicitud.AlgoritmoCompresion}, destino={solicitud.IdDestinoConfig}).",
        };
        _historialEnMemoria.Insert(0, registro);

        var respuesta = new RespuestaEjecucion
        {
            BackupId = backupId,
            EstadoInicial = MensajesContrato.EstadoInicialIniciado,
        };
        return Task.FromResult(respuesta);
    }

    public Task<IReadOnlyList<LogRespaldo>> ObtenerHistorialAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<LogRespaldo>>(_historialEnMemoria.ToList());
}
