using Backups.Core.Domain;
using Backups.Core.Domain.Entities;
using Backups.Core.Ports.Out;

namespace Backups.Core;

/// <summary>
/// Drena IPilaEnviosPort (LIFO) y delega a ICompressionService/ITransmissionService
/// (Grupo 2/3, firmas congeladas - ver CHANGELOG). Transiciones:
/// COMPRIMIENDO_VOLUMENES -> ENVIANDO -> COMPLETADO/FALLIDO.
/// </summary>
public sealed class ProcesadorPila
{
    private readonly IPilaEnviosPort _pilaEnvios;
    private readonly ICompressionService _compresion;
    private readonly ITransmissionService _transmision;
    private readonly IRegistroLogPort _registroLog;

    public ProcesadorPila(
        IPilaEnviosPort pilaEnvios,
        ICompressionService compresion,
        ITransmissionService transmision,
        IRegistroLogPort registroLog)
    {
        _pilaEnvios = pilaEnvios;
        _compresion = compresion;
        _transmision = transmision;
        _registroLog = registroLog;
    }

    public async Task DrenarAsync(CancellationToken ct)
    {
        while (_pilaEnvios.TryDesapilar(out var tarea))
        {
            await ProcesarAsync(tarea, ct).ConfigureAwait(false);
        }
    }

    private async Task ProcesarAsync(TareaRespaldo tarea, CancellationToken ct)
    {
        await RegistrarAsync(tarea, EstadoRespaldo.COMPRIMIENDO_VOLUMENES, "Comprimiendo volumenes.", ct).ConfigureAwait(false);

        try
        {
            var volumenes = _compresion.ComprimirYSegmentar(
                tarea.Solicitud.RutaOrigen,
                tarea.Solicitud.AlgoritmoCompresion.ToString(),
                tarea.Solicitud.LimiteVolumenMb);

            await RegistrarAsync(tarea, EstadoRespaldo.ENVIANDO, "Enviando volumenes al destino.", ct).ConfigureAwait(false);

            var enviado = _transmision.EnviarArchivos(volumenes, tarea.Solicitud.IdDestinoConfig);
            var estadoFinal = enviado ? EstadoRespaldo.COMPLETADO : EstadoRespaldo.FALLIDO;
            var mensajeFinal = enviado ? "Respaldo completado correctamente." : "Fallo el envio de los volumenes.";
            await RegistrarAsync(tarea, estadoFinal, mensajeFinal, ct).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await RegistrarAsync(tarea, EstadoRespaldo.FALLIDO, $"Error al procesar el respaldo: {ex.Message}", ct).ConfigureAwait(false);
        }
    }

    private Task RegistrarAsync(TareaRespaldo tarea, EstadoRespaldo estado, string mensaje, CancellationToken ct)
    {
        var registro = new RegistroFisico(DateTimeOffset.UtcNow, tarea.BackupId, estado, tarea.Solicitud.RutaOrigen, tarea.HashActual, tarea.Solicitud.TipoDisparo, mensaje);
        return _registroLog.EscribirAsync(registro, ct);
    }
}
