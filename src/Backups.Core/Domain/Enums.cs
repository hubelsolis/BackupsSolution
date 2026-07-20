using System.Text.Json.Serialization;

namespace Backups.Core.Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TipoDisparo
{
    TIMER,
    BOTON_MANUAL,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AlgoritmoCompresion
{
    ZIP,
    RAR,
    LZMA,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Protocolo
{
    FTP,
    SFTP,
    SSH,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EstadoRespaldo
{
    OMITIDO_SIN_CAMBIOS,
    PROCESANDO,
    COMPRIMIENDO_VOLUMENES,
    ENVIANDO,
    COMPLETADO,
    FALLIDO,
}
