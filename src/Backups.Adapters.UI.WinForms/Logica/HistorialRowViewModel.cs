namespace Backups.Adapters.UI.WinForms.Logica;

/// <summary>Fila lista para pintar en el DataGridView de historial.</summary>
public sealed record HistorialRowViewModel(string BackupId, string Estado, string MensajeTxt);
