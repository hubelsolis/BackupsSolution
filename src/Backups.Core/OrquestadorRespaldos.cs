using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.In;
using Backups.Core.Ports.Out;

namespace Backups.Core;

/// <summary>
/// Implementacion real de IEjecutarRespaldoUseCase/IObtenerHistorialUseCase.
/// Mapea POST /backups/ejecutar (202) y GET /backups/historial.
/// </summary>
public sealed class OrquestadorRespaldos : IEjecutarRespaldoUseCase, IObtenerHistorialUseCase
{
    private readonly IArchivoPort _archivo;
    private readonly ICalculadoraHashPort _calculadoraHash;
    private readonly IRegistroLogPort _registroLog;
    private readonly IPilaEnviosPort _pilaEnvios;

    public OrquestadorRespaldos(
        IArchivoPort archivo,
        ICalculadoraHashPort calculadoraHash,
        IRegistroLogPort registroLog,
        IPilaEnviosPort pilaEnvios)
    {
        _archivo = archivo;
        _calculadoraHash = calculadoraHash;
        _registroLog = registroLog;
        _pilaEnvios = pilaEnvios;
    }

    public async Task<RespuestaEjecucion> EjecutarAsync(SolicitudRespaldo solicitud, CancellationToken ct)
    {
        var backupId = Guid.NewGuid();

        var errores = solicitud.Validar();
        if (errores.Count > 0)
        {
            var mensaje = $"Solicitud invalida: {string.Join(" ", errores)}";
            await RegistrarAsync(backupId, EstadoRespaldo.FALLIDO, solicitud, hash: string.Empty, mensaje, ct).ConfigureAwait(false);
            return new RespuestaEjecucion { BackupId = backupId, EstadoInicial = EstadoRespaldo.FALLIDO.ToString() };
        }

        if (!_archivo.Existe(solicitud.RutaOrigen))
        {
            const string mensaje = "Archivo de origen no encontrado.";
            await RegistrarAsync(backupId, EstadoRespaldo.FALLIDO, solicitud, hash: string.Empty, mensaje, ct).ConfigureAwait(false);
            return new RespuestaEjecucion { BackupId = backupId, EstadoInicial = EstadoRespaldo.FALLIDO.ToString() };
        }

        var hashActual = await _calculadoraHash.CalcularSha256Async(solicitud.RutaOrigen, ct).ConfigureAwait(false);
        var hashPrevio = solicitud.UltimoHashConocido
            ?? await _registroLog.ObtenerUltimoHashConfirmadoAsync(solicitud.RutaOrigen, ct).ConfigureAwait(false);

        if (hashPrevio is not null && string.Equals(hashPrevio, hashActual, StringComparison.OrdinalIgnoreCase))
        {
            await RegistrarAsync(backupId, EstadoRespaldo.OMITIDO_SIN_CAMBIOS, solicitud, hashActual, MensajesContrato.CopiaCanceladaSinCambios, ct).ConfigureAwait(false);
            return new RespuestaEjecucion { BackupId = backupId, EstadoInicial = MensajesContrato.EstadoInicialOmitidoSinCambios };
        }

        _pilaEnvios.Apilar(new TareaRespaldo(backupId, solicitud, hashActual));
        await RegistrarAsync(backupId, EstadoRespaldo.PROCESANDO, solicitud, hashActual, "Respaldo apilado para procesamiento.", ct).ConfigureAwait(false);
        return new RespuestaEjecucion { BackupId = backupId, EstadoInicial = MensajesContrato.EstadoInicialIniciado };
    }

    public Task<IReadOnlyList<LogRespaldo>> ObtenerHistorialAsync(CancellationToken ct) =>
        _registroLog.LeerHistorialAsync(ct);

    private Task RegistrarAsync(Guid backupId, EstadoRespaldo estado, SolicitudRespaldo solicitud, string hash, string mensaje, CancellationToken ct)
    {
        var registro = new RegistroFisico(DateTimeOffset.UtcNow, backupId, estado, solicitud.RutaOrigen, hash, solicitud.TipoDisparo, mensaje);
        return _registroLog.EscribirAsync(registro, ct);
    }
}
