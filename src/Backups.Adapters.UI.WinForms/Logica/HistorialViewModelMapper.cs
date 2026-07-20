using Backups.Core.Domain.Entities;

namespace Backups.Adapters.UI.WinForms.Logica;

public static class HistorialViewModelMapper
{
    public static IReadOnlyList<HistorialRowViewModel> AFilas(IEnumerable<LogRespaldo> historial) =>
        historial
            .Select(log => new HistorialRowViewModel(
                log.BackupId.ToString(),
                log.Estado.ToString(),
                log.MensajeTxt))
            .ToList();
}
