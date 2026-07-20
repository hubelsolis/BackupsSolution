using System.Diagnostics.CodeAnalysis;
using Backups.Adapters.UI.WinForms.Logica;
using Backups.Adapters.UI.WinForms.Configuration;

namespace Backups.Adapters.UI.WinForms;

/// <summary>Formulario de configuración: carpeta de origen por defecto y destinos disponibles (sesión actual).</summary>
[ExcludeFromCodeCoverage(Justification = "Orquestación de controles WinForms; la regla de negocio se probó por separado en GestionDestinosServiceTests.")]
public partial class FormConfiguracion : Form
{
    private List<string> _destinos = new();

    public FormConfiguracion()
    {
        InitializeComponent();
    }

    public UiDefaultsOptions OpcionesIniciales
    {
        set
        {
            txtCarpetaOrigen.Text = value.CarpetaOrigenPorDefecto;
            numLimiteVolumenMb.Value = Math.Max(numLimiteVolumenMb.Minimum, value.LimiteVolumenMbPorDefecto);
            _destinos = new List<string>(value.DestinosDisponibles);
            RefrescarListaDestinos();
        }
    }

    public UiDefaultsOptions OpcionesResultado => new()
    {
        CarpetaOrigenPorDefecto = txtCarpetaOrigen.Text.Trim(),
        LimiteVolumenMbPorDefecto = (int)numLimiteVolumenMb.Value,
        DestinosDisponibles = _destinos,
    };

    private void RefrescarListaDestinos()
    {
        lstDestinos.Items.Clear();
        lstDestinos.Items.AddRange(_destinos.Cast<object>().ToArray());
    }

    private void BtnSeleccionarCarpeta_Click(object? sender, EventArgs e)
    {
        using var dialogo = new FolderBrowserDialog();
        if (dialogo.ShowDialog(this) == DialogResult.OK)
        {
            txtCarpetaOrigen.Text = dialogo.SelectedPath;
        }
    }

    private void BtnAgregarDestino_Click(object? sender, EventArgs e)
    {
        if (GestionDestinosService.TryAgregar(txtNuevoDestino.Text, _destinos, out var error))
        {
            txtNuevoDestino.Clear();
            RefrescarListaDestinos();
        }
        else
        {
            MessageBox.Show(this, error, "No se pudo agregar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BtnQuitarDestino_Click(object? sender, EventArgs e)
    {
        if (lstDestinos.SelectedItem is string seleccionado)
        {
            _destinos.Remove(seleccionado);
            RefrescarListaDestinos();
        }
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnCancelar_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
