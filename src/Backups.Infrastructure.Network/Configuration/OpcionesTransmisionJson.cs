using System.Text.Json;
using System.Text.Json.Serialization;
using Backups.Infrastructure.Network.Domain;

namespace Backups.Infrastructure.Network.Configuration;

// PROPUESTO POR GRUPO 3 - NO MERGEAR SIN REVISIÓN.
// Implementación por defecto de IOpcionesTransmision: lee los destinos desde un JSON cuya
// ruta viene de la variable de entorno BACKUPS_TRANSMISION_RUTA_CONFIG. Ningún host, puerto,
// usuario, contraseña o ruta remota está hardcodeado en el código.
public sealed class OpcionesTransmisionJson : IOpcionesTransmision
{
    private const string VariableRutaConfiguracion = "BACKUPS_TRANSMISION_RUTA_CONFIG";

    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IReadOnlyDictionary<string, ConexionDestino> _destinos;

    public OpcionesTransmisionJson(string contenidoJson)
    {
        ArgumentNullException.ThrowIfNull(contenidoJson);
        _destinos = Parsear(contenidoJson);
    }

    public static OpcionesTransmisionJson DesdeVariablesDeEntorno()
    {
        string? ruta = Environment.GetEnvironmentVariable(VariableRutaConfiguracion);
        if (string.IsNullOrWhiteSpace(ruta) || !File.Exists(ruta))
        {
            throw new InvalidOperationException(
                $"No se encontró el archivo de configuración de destinos. Configure la variable de entorno '{VariableRutaConfiguracion}' apuntando a un JSON válido.");
        }

        return new OpcionesTransmisionJson(File.ReadAllText(ruta));
    }

    public ConexionDestino ObtenerConexion(string idDestinoConfig)
    {
        ArgumentNullException.ThrowIfNull(idDestinoConfig);
        if (!_destinos.TryGetValue(idDestinoConfig, out ConexionDestino? conexion))
        {
            throw new KeyNotFoundException($"No se encontró configuración para el destino '{idDestinoConfig}'.");
        }

        return conexion;
    }

    private static IReadOnlyDictionary<string, ConexionDestino> Parsear(string contenidoJson)
    {
        Dictionary<string, EntradaDestinoDto>? entradas;
        try
        {
            entradas = JsonSerializer.Deserialize<Dictionary<string, EntradaDestinoDto>>(contenidoJson, OpcionesJson);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("El JSON de configuración de destinos no es válido.", ex);
        }

        var resultado = new Dictionary<string, ConexionDestino>(StringComparer.Ordinal);
        foreach ((string id, EntradaDestinoDto entrada) in entradas ?? [])
        {
            if (string.IsNullOrWhiteSpace(entrada.Protocolo) || string.IsNullOrWhiteSpace(entrada.Host) || string.IsNullOrWhiteSpace(entrada.Usuario))
            {
                throw new InvalidOperationException($"El destino '{id}' no tiene protocolo, host o usuario configurados.");
            }

            resultado[id] = new ConexionDestino
            {
                Destino = new DestinoConfig
                {
                    Id = id,
                    Protocolo = entrada.Protocolo,
                    Host = entrada.Host,
                    Usuario = entrada.Usuario,
                },
                Puerto = entrada.Puerto,
                Contrasena = entrada.Contrasena ?? string.Empty,
                RutaRemota = entrada.RutaRemota ?? string.Empty,
                TimeoutSegundos = entrada.TimeoutSegundos ?? 30,
            };
        }

        return resultado;
    }

    private sealed class EntradaDestinoDto
    {
        [JsonPropertyName("protocolo")]
        public string? Protocolo { get; set; }

        [JsonPropertyName("host")]
        public string? Host { get; set; }

        [JsonPropertyName("usuario")]
        public string? Usuario { get; set; }

        [JsonPropertyName("puerto")]
        public int Puerto { get; set; }

        [JsonPropertyName("contrasena")]
        public string? Contrasena { get; set; }

        [JsonPropertyName("rutaRemota")]
        public string? RutaRemota { get; set; }

        [JsonPropertyName("timeoutSegundos")]
        public int? TimeoutSegundos { get; set; }
    }
}
