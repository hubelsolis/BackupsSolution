using System.Diagnostics.CodeAnalysis;
using Backups.Adapters.UI.WinForms.Logica;
using Backups.Adapters.UI.WinForms.Configuration;
using Backups.Core.Domain;
using Backups.Core.Ports.In;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backups.Adapters.UI.WinForms;

// Orquestación de UI -> caso de uso (sin lógica de negocio propia); la lógica
// testeable vive en Backups.Adapters.UI.WinForms.Logica. Requiere ventana real
// para probarse, por eso queda fuera de la medición de cobertura (Paso 5).
[ExcludeFromCodeCoverage(Justification = "Orquestación de controles WinForms; la lógica se probó por separado en Logica.*")]
public partial class FormPrincipal : Form
{
    private readonly SolicitarRespaldoManualService _respaldoService;
    private readonly IObtenerHistorialUseCase _historialUseCase;
    private readonly Func<FormHistorialLogs> _crearFormHistorial;
    private readonly Func<FormConfiguracion> _crearFormConfiguracion;
    private readonly ILogger<FormPrincipal> _logger;
    private UiDefaultsOptions _opciones;

    public FormPrincipal(
        SolicitarRespaldoManualService respaldoService,
        IObtenerHistorialUseCase historialUseCase,
        IOptions<UiDefaultsOptions> opciones,
        Func<FormHistorialLogs> crearFormHistorial,
        Func<FormConfiguracion> crearFormConfiguracion,
        ILogger<FormPrincipal> logger)
    {
        _respaldoService = respaldoService;
        _historialUseCase = historialUseCase;
        _crearFormHistorial = crearFormHistorial;
        _crearFormConfiguracion = crearFormConfiguracion;
        _logger = logger;
        _opciones = opciones.Value;

        InitializeComponent();
        InicializarControles();
        Load += async (_, _) => await ActualizarHistorialAsync().ConfigureAwait(true);
    }

    private void InicializarControles()
    {
        cmbAlgoritmo.Items.AddRange(Enum.GetValues<AlgoritmoCompresion>().Cast<object>().ToArray());
        cmbAlgoritmo.SelectedIndex = 0;

        cmbDestino.Items.AddRange(_opciones.DestinosDisponibles.ToArray());
        if (cmbDestino.Items.Count > 0)
        {
            cmbDestino.SelectedIndex = 0;
        }

        txtRutaOrigen.Text = _opciones.CarpetaOrigenPorDefecto;
        numLimiteVolumenMb.Value = Math.Max(numLimiteVolumenMb.Minimum, _opciones.LimiteVolumenMbPorDefecto);

        notifyIcon1.Visible = true;
        notifyIcon1.Text = "Backups - Grupo 4";
    }

    private void BtnSeleccionarOrigen_Click(object? sender, EventArgs e)
    {
        using var dialogo = new FolderBrowserDialog();
        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            txtRutaOrigen.Text = dialogo.SelectedPath;
        }
    }

    private DatosFormularioRespaldo LeerDatosFormulario() => new(
        txtNombreCopia.Text.Trim(),
        txtRutaOrigen.Text.Trim(),
        (AlgoritmoCompresion)(cmbAlgoritmo.SelectedItem ?? AlgoritmoCompresion.ZIP),
        (int)numLimiteVolumenMb.Value,
        cmbDestino.Text.Trim());

    private async void BtnRespaldarAhora_Click(object? sender, EventArgs e)
    {
        btnRespaldarAhora.Enabled = false;
        try
        {
            var datos = LeerDatosFormulario();
            var resultado = await _respaldoService.EjecutarAsync(datos, CancellationToken.None).ConfigureAwait(true);

            if (!resultado.Exitoso)
            {
                lblResultado.Text = "Faltan datos: " + string.Join(" ", resultado.Errores);
                MessageBox.Show(this, string.Join(Environment.NewLine, resultado.Errores), "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            lblResultado.Text = $"Respaldo {resultado.Respuesta!.EstadoInicial} (backupId={resultado.Respuesta.BackupId}).";
            await ActualizarHistorialAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallo al ejecutar el respaldo manual");
            lblResultado.Text = "No se pudo ejecutar el respaldo. Ver detalle.";
            MessageBox.Show(this, ex.Message, "Error al respaldar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnRespaldarAhora.Enabled = true;
        }
    }

    private async void BtnActualizarHistorial_Click(object? sender, EventArgs e) =>
        await ActualizarHistorialAsync().ConfigureAwait(true);

    private async Task ActualizarHistorialAsync()
    {
        try
        {
            var historial = await _historialUseCase.ObtenerHistorialAsync(CancellationToken.None).ConfigureAwait(true);
            dgvHistorial.DataSource = HistorialViewModelMapper.AFilas(historial).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallo al leer el historial de respaldos");
            MessageBox.Show(this, ex.Message, "Error al leer historial", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AbrirLogsToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using var form = _crearFormHistorial();
        form.ShowDialog(this);
    }

    private void AbrirConfiguracionToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using var form = _crearFormConfiguracion();
        form.OpcionesIniciales = _opciones;
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            _opciones = form.OpcionesResultado;
            cmbDestino.Items.Clear();
            cmbDestino.Items.AddRange(_opciones.DestinosDisponibles.ToArray());
            if (cmbDestino.Items.Count > 0)
            {
                cmbDestino.SelectedIndex = 0;
            }

            txtRutaOrigen.Text = _opciones.CarpetaOrigenPorDefecto;
        }
    }

    private void SalirToolStripMenuItem_Click(object? sender, EventArgs e) => Close();

    private void NotifyIcon1_MouseDoubleClick(object? sender, MouseEventArgs e) => MostrarDesdeTray();

    private void MostrarToolStripMenuItem_Click(object? sender, EventArgs e) => MostrarDesdeTray();

    private void MostrarDesdeTray()
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
    }

    private void FormPrincipal_Resize(object? sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            Hide();
        }
    }
}
