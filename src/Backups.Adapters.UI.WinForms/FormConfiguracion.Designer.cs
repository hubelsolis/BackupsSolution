namespace Backups.Adapters.UI.WinForms;

partial class FormConfiguracion
{
    private System.ComponentModel.IContainer components = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        lblCarpetaOrigen = new Label();
        txtCarpetaOrigen = new TextBox();
        btnSeleccionarCarpeta = new Button();
        lblLimiteVolumen = new Label();
        numLimiteVolumenMb = new NumericUpDown();
        lblDestinos = new Label();
        lstDestinos = new ListBox();
        txtNuevoDestino = new TextBox();
        btnAgregarDestino = new Button();
        btnQuitarDestino = new Button();
        btnGuardar = new Button();
        btnCancelar = new Button();
        ((System.ComponentModel.ISupportInitialize)numLimiteVolumenMb).BeginInit();
        SuspendLayout();
        //
        // lblCarpetaOrigen
        //
        lblCarpetaOrigen.AutoSize = true;
        lblCarpetaOrigen.Location = new Point(16, 16);
        lblCarpetaOrigen.Name = "lblCarpetaOrigen";
        lblCarpetaOrigen.Text = "Carpeta de origen por defecto:";
        //
        // txtCarpetaOrigen
        //
        txtCarpetaOrigen.Location = new Point(16, 34);
        txtCarpetaOrigen.Name = "txtCarpetaOrigen";
        txtCarpetaOrigen.ReadOnly = true;
        txtCarpetaOrigen.Size = new Size(300, 23);
        //
        // btnSeleccionarCarpeta
        //
        btnSeleccionarCarpeta.Location = new Point(322, 33);
        btnSeleccionarCarpeta.Name = "btnSeleccionarCarpeta";
        btnSeleccionarCarpeta.Size = new Size(112, 25);
        btnSeleccionarCarpeta.Text = "Seleccionar...";
        btnSeleccionarCarpeta.UseVisualStyleBackColor = true;
        btnSeleccionarCarpeta.Click += BtnSeleccionarCarpeta_Click;
        //
        // lblLimiteVolumen
        //
        lblLimiteVolumen.AutoSize = true;
        lblLimiteVolumen.Location = new Point(16, 70);
        lblLimiteVolumen.Name = "lblLimiteVolumen";
        lblLimiteVolumen.Text = "Límite de volumen (MB) por defecto:";
        //
        // numLimiteVolumenMb
        //
        numLimiteVolumenMb.Location = new Point(16, 88);
        numLimiteVolumenMb.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numLimiteVolumenMb.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numLimiteVolumenMb.Name = "numLimiteVolumenMb";
        numLimiteVolumenMb.Size = new Size(150, 23);
        numLimiteVolumenMb.Value = new decimal(new int[] { 100, 0, 0, 0 });
        //
        // lblDestinos
        //
        lblDestinos.AutoSize = true;
        lblDestinos.Location = new Point(16, 124);
        lblDestinos.Name = "lblDestinos";
        lblDestinos.Text = "Destinos disponibles (idDestinoConfig):";
        //
        // lstDestinos
        //
        lstDestinos.FormattingEnabled = true;
        lstDestinos.Location = new Point(16, 142);
        lstDestinos.Name = "lstDestinos";
        lstDestinos.Size = new Size(418, 94);
        //
        // txtNuevoDestino
        //
        txtNuevoDestino.Location = new Point(16, 244);
        txtNuevoDestino.Name = "txtNuevoDestino";
        txtNuevoDestino.Size = new Size(200, 23);
        //
        // btnAgregarDestino
        //
        btnAgregarDestino.Location = new Point(222, 243);
        btnAgregarDestino.Name = "btnAgregarDestino";
        btnAgregarDestino.Size = new Size(100, 25);
        btnAgregarDestino.Text = "Agregar";
        btnAgregarDestino.UseVisualStyleBackColor = true;
        btnAgregarDestino.Click += BtnAgregarDestino_Click;
        //
        // btnQuitarDestino
        //
        btnQuitarDestino.Location = new Point(334, 243);
        btnQuitarDestino.Name = "btnQuitarDestino";
        btnQuitarDestino.Size = new Size(100, 25);
        btnQuitarDestino.Text = "Quitar";
        btnQuitarDestino.UseVisualStyleBackColor = true;
        btnQuitarDestino.Click += BtnQuitarDestino_Click;
        //
        // btnGuardar
        //
        btnGuardar.Location = new Point(259, 284);
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(85, 27);
        btnGuardar.Text = "Guardar";
        btnGuardar.UseVisualStyleBackColor = true;
        btnGuardar.Click += BtnGuardar_Click;
        //
        // btnCancelar
        //
        btnCancelar.Location = new Point(350, 284);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(85, 27);
        btnCancelar.Text = "Cancelar";
        btnCancelar.UseVisualStyleBackColor = true;
        btnCancelar.Click += BtnCancelar_Click;
        //
        // FormConfiguracion
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(450, 325);
        Controls.Add(btnCancelar);
        Controls.Add(btnGuardar);
        Controls.Add(btnQuitarDestino);
        Controls.Add(btnAgregarDestino);
        Controls.Add(txtNuevoDestino);
        Controls.Add(lstDestinos);
        Controls.Add(lblDestinos);
        Controls.Add(numLimiteVolumenMb);
        Controls.Add(lblLimiteVolumen);
        Controls.Add(btnSeleccionarCarpeta);
        Controls.Add(txtCarpetaOrigen);
        Controls.Add(lblCarpetaOrigen);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormConfiguracion";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Configuración";
        ((System.ComponentModel.ISupportInitialize)numLimiteVolumenMb).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblCarpetaOrigen;
    private TextBox txtCarpetaOrigen;
    private Button btnSeleccionarCarpeta;
    private Label lblLimiteVolumen;
    private NumericUpDown numLimiteVolumenMb;
    private Label lblDestinos;
    private ListBox lstDestinos;
    private TextBox txtNuevoDestino;
    private Button btnAgregarDestino;
    private Button btnQuitarDestino;
    private Button btnGuardar;
    private Button btnCancelar;
}
