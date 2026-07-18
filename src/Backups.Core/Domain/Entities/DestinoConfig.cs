using System.Text.Json.Serialization;

namespace Backups.Core.Domain.Entities;

public sealed record DestinoConfig
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("protocolo")]
    public required Protocolo Protocolo { get; init; }

    [JsonPropertyName("host")]
    public required string Host { get; init; }

    [JsonPropertyName("usuario")]
    public required string Usuario { get; init; }
}
