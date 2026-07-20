using Backups.Core.Domain;

namespace Backups.Adapters.UI.WinForms.Logica;

/// <summary>Datos crudos capturados en el formulario, sin dependencia de System.Windows.Forms.</summary>
public sealed record DatosFormularioRespaldo(
    string NombreCopia,
    string RutaOrigen,
    AlgoritmoCompresion AlgoritmoCompresion,
    int LimiteVolumenMb,
    string IdDestinoConfig);
