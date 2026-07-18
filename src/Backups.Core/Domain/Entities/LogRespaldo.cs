using System.Text.Json.Serialization;

namespace Backups.Core.Domain.Entities;

public sealed record LogRespaldo
{
    [JsonPropertyName("backupId")]
    public required Guid BackupId { get; init; }

    [JsonPropertyName("estado")]
    public required EstadoRespaldo Estado { get; init; }

    [JsonPropertyName("mensajeTxt")]
    public required string MensajeTxt { get; init; }
}
