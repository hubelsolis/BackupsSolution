using System.Text.Json.Serialization;

namespace Backups.Infrastructure.Network.Domain;

// PROPUESTO POR GRUPO 3 - NO MERGEAR SIN REVISIÓN.
// Duplica el modelo DestinoConfig del contrato OpenAPI v1.3.0 (id/protocolo/host/usuario).
// Backups.Core en 'main' todavía no expone esta entidad (vive en la rama nucleo-de-control,
// no mergeada); esta copia solo existe para que Grupo 3 compile y pruebe de forma aislada.
// Riesgo de conflicto "added by both" al mergear - ver CHANGELOG.md.
public sealed record DestinoConfig
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("protocolo")]
    public required string Protocolo { get; init; }

    [JsonPropertyName("host")]
    public required string Host { get; init; }

    [JsonPropertyName("usuario")]
    public required string Usuario { get; init; }
}
