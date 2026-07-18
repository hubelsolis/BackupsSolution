using System.Text.Json.Serialization;

namespace Backups.Core.Domain.Entities;

public sealed record SolicitudRespaldo
{
    [JsonPropertyName("nombreCopia")]
    public required string NombreCopia { get; init; }

    [JsonPropertyName("tipoDisparo")]
    public required TipoDisparo TipoDisparo { get; init; }

    [JsonPropertyName("rutaOrigen")]
    public required string RutaOrigen { get; init; }

    /// <summary>Opcional segun el contrato: si es null, se consulta el ultimo hash confirmado del log fisico.</summary>
    [JsonPropertyName("ultimoHashConocido")]
    public string? UltimoHashConocido { get; init; }

    [JsonPropertyName("algoritmoCompresion")]
    public required AlgoritmoCompresion AlgoritmoCompresion { get; init; }

    [JsonPropertyName("limiteVolumenMb")]
    public required int LimiteVolumenMb { get; init; }

    [JsonPropertyName("idDestinoConfig")]
    public required string IdDestinoConfig { get; init; }

    public IReadOnlyList<string> Validar()
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(NombreCopia))
        {
            errores.Add("nombreCopia es requerido.");
        }

        if (string.IsNullOrWhiteSpace(RutaOrigen))
        {
            errores.Add("rutaOrigen es requerido.");
        }

        if (string.IsNullOrWhiteSpace(IdDestinoConfig))
        {
            errores.Add("idDestinoConfig es requerido.");
        }

        if (LimiteVolumenMb <= 0)
        {
            errores.Add("limiteVolumenMb debe ser mayor a 0.");
        }

        return errores;
    }
}
