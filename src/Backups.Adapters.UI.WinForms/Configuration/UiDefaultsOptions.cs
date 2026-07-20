using System.Diagnostics.CodeAnalysis;

namespace Backups.Adapters.UI.WinForms.Configuration;

/// <summary>Valores por defecto de la UI, cargados desde appsettings.json. Nada hardcodeado en el código.</summary>
[ExcludeFromCodeCoverage(Justification = "DTO de configuración: propiedades planas sin lógica, pobladas por el binder de IConfiguration.")]
public sealed class UiDefaultsOptions
{
    public const string SeccionConfiguracion = "UiDefaults";

    public string CarpetaOrigenPorDefecto { get; set; } = string.Empty;

    public int LimiteVolumenMbPorDefecto { get; set; } = 1;

    public IList<string> DestinosDisponibles { get; set; } = new List<string>();
}
