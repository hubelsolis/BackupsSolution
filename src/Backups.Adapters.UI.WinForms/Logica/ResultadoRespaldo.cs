using Backups.Core.Domain.Entities;

namespace Backups.Adapters.UI.WinForms.Logica;

/// <summary>Resultado de intentar disparar un respaldo manual desde la UI.</summary>
public sealed record ResultadoRespaldo
{
    public required bool Exitoso { get; init; }

    public IReadOnlyList<string> Errores { get; init; } = Array.Empty<string>();

    public RespuestaEjecucion? Respuesta { get; init; }

    public static ResultadoRespaldo Invalido(IReadOnlyList<string> errores) =>
        new() { Exitoso = false, Errores = errores };

    public static ResultadoRespaldo Ok(RespuestaEjecucion respuesta) =>
        new() { Exitoso = true, Respuesta = respuesta };
}
