namespace Backups.Core.Domain;

public static class MensajesContrato
{
    public const string CopiaCanceladaSinCambios =
        "Copia cancelada: El archivo plano no presenta modificaciones desde el último respaldo.";

    public const string EstadoInicialIniciado = "INICIADO";

    public const string EstadoInicialOmitidoSinCambios = "OMITIDO_SIN_CAMBIOS";
}
