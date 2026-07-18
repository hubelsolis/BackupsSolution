using System.Text.Json.Serialization;

namespace Backups.Core.Domain.Entities;

public sealed record RespuestaEjecucion
{
    [JsonPropertyName("backupId")]
    public required Guid BackupId { get; init; }

    /// <summary>
    /// "INICIADO" u "OMITIDO_SIN_CAMBIOS". El contrato original trae como
    /// ejemplo "INICIADO_O_OMITIDO_SIN_CAMBIOS", que no es un enum valido;
    /// ver CHANGELOG.md.
    /// </summary>
    [JsonPropertyName("estadoInicial")]
    public required string EstadoInicial { get; init; }
}
