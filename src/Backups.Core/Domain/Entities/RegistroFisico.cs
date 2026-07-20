namespace Backups.Core.Domain.Entities;

/// <summary>
/// Linea completa del log fisico (TXT). LogRespaldo (contrato) es mas delgado
/// (backupId, estado, mensajeTxt); RegistroFisico agrega lo que necesita el
/// propio nucleo para reconstruir historial y resolver el ultimo hash confirmado.
/// </summary>
public sealed record RegistroFisico(
    DateTimeOffset Timestamp,
    Guid BackupId,
    EstadoRespaldo Estado,
    string RutaOrigen,
    string HashSha256,
    TipoDisparo TipoDisparo,
    string MensajeTxt);
