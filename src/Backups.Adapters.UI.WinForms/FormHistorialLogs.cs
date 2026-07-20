using System.Diagnostics.CodeAnalysis;
using Backups.Adapters.UI.WinForms.Logica;
using Backups.Core.Ports.In;
using Microsoft.Extensions.Logging;

namespace Backups.Adapters.UI.WinForms;

/// <summary>Ventana de Logs: historial completo de respaldos leído a través del Grupo 1.</summary>
[ExcludeFromCodeCoverage(Justification = "Orquestación de controles WinForms; el mapeo se probó por separado en HistorialViewModelMapperTests.")]
public partial class FormHistorialLogs : Form
{
    private readonly IObtenerHistorialUseCase _historialUseCase;
    private readonly ILogger<FormHistorialLogs> _logger;

    public FormHistorialLogs(IObtenerHistorialUseCase historialUseCase, ILogger<FormHistorialLogs> logger)
    {
        _historialUseCase = historialUseCase;
        _logger = logger;
        InitializeComponent();
        Load += async (_, _) => await CargarAsync().ConfigureAwait(true);
    }

    private async void BtnActualizar_Click(object? sender, EventArgs e) => await CargarAsync().ConfigureAwait(true);

    private async Task CargarAsync()
    {
        try
        {
            var historial = await _historialUseCase.ObtenerHistorialAsync(CancellationToken.None).ConfigureAwait(true);
            dgvLogs.DataSource = HistorialViewModelMapper.AFilas(historial).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallo al leer el historial en la ventana de logs");
            MessageBox.Show(this, ex.Message, "Error al leer logs", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
