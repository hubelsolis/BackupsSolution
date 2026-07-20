namespace Backups.Adapters.UI.WinForms;

partial class FormPrincipal
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
        menuStrip1 = new MenuStrip();
        configuracionToolStripMenuItem = new ToolStripMenuItem();
        logsToolStripMenuItem = new ToolStripMenuItem();
        salirToolStripMenuItem = new ToolStripMenuItem();
        lblOrigen = new Label();
        txtRutaOrigen = new TextBox();
        btnSeleccionarOrigen = new Button();
        lblNombreCopia = new Label();
        txtNombreCopia = new TextBox();
        lblAlgoritmo = new Label();
        cmbAlgoritmo = new ComboBox();
        lblLimiteVolumen = new Label();
        numLimiteVolumenMb = new NumericUpDown();
        lblDestino = new Label();
        cmbDestino = new ComboBox();
        btnRespaldarAhora = new Button();
        lblResultado = new Label();
        lblHistorial = new Label();
        dgvHistorial = new DataGridView();
        btnActualizarHistorial = new Button();
        notifyIcon1 = new NotifyIcon(components);
        contextMenuStrip1 = new ContextMenuStrip(components);
        mostrarToolStripMenuItem = new ToolStripMenuItem();
        salirTrayToolStripMenuItem = new ToolStripMenuItem();
        menuStrip1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numLimiteVolumenMb).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvHistorial).BeginInit();
        contextMenuStrip1.SuspendLayout();
        SuspendLayout();
        //
        // menuStrip1
        //
        menuStrip1.Items.AddRange(new ToolStripItem[] { configuracionToolStripMenuItem, logsToolStripMenuItem, salirToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(620, 24);
        menuStrip1.TabIndex = 0;
        //
        // configuracionToolStripMenuItem
        //
        configuracionToolStripMenuItem.Name = "configuracionToolStripMenuItem";
        configuracionToolStripMenuItem.Size = new Size(97, 20);
        configuracionToolStripMenuItem.Text = "Configuración";
        configuracionToolStripMenuItem.Click += AbrirConfiguracionToolStripMenuItem_Click;
        //
        // logsToolStripMenuItem
        //
        logsToolStripMenuItem.Name = "logsToolStripMenuItem";
        logsToolStripMenuItem.Size = new Size(50, 20);
        logsToolStripMenuItem.Text = "Logs";
        logsToolStripMenuItem.Click += AbrirLogsToolStripMenuItem_Click;
        //
        // salirToolStripMenuItem
        //
        salirToolStripMenuItem.Name = "salirToolStripMenuItem";
        salirToolStripMenuItem.Size = new Size(44, 20);
        salirToolStripMenuItem.Text = "Salir";
        salirToolStripMenuItem.Click += SalirToolStripMenuItem_Click;
        //
        // lblOrigen
        //
        lblOrigen.AutoSize = true;
        lblOrigen.Location = new Point(16, 36);
        lblOrigen.Name = "lblOrigen";
        lblOrigen.Text = "Carpeta de origen:";
        //
        // txtRutaOrigen
        //
        txtRutaOrigen.Location = new Point(16, 54);
        txtRutaOrigen.Name = "txtRutaOrigen";
        txtRutaOrigen.ReadOnly = true;
        txtRutaOrigen.Size = new Size(470, 23);
        //
        // btnSeleccionarOrigen
        //
        btnSeleccionarOrigen.Location = new Point(492, 53);
        btnSeleccionarOrigen.Name = "btnSeleccionarOrigen";
        btnSeleccionarOrigen.Size = new Size(112, 25);
        btnSeleccionarOrigen.Text = "Seleccionar...";
        btnSeleccionarOrigen.UseVisualStyleBackColor = true;
        btnSeleccionarOrigen.Click += BtnSeleccionarOrigen_Click;
        //
        // lblNombreCopia
        //
        lblNombreCopia.AutoSize = true;
        lblNombreCopia.Location = new Point(16, 90);
        lblNombreCopia.Name = "lblNombreCopia";
        lblNombreCopia.Text = "Nombre de la copia:";
        //
        // txtNombreCopia
        //
        txtNombreCopia.Location = new Point(16, 108);
        txtNombreCopia.Name = "txtNombreCopia";
        txtNombreCopia.Size = new Size(300, 23);
        //
        // lblAlgoritmo
        //
        lblAlgoritmo.AutoSize = true;
        lblAlgoritmo.Location = new Point(332, 90);
        lblAlgoritmo.Name = "lblAlgoritmo";
        lblAlgoritmo.Text = "Algoritmo:";
        //
        // cmbAlgoritmo
        //
        cmbAlgoritmo.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbAlgoritmo.Location = new Point(332, 108);
        cmbAlgoritmo.Name = "cmbAlgoritmo";
        cmbAlgoritmo.Size = new Size(130, 23);
        //
        // lblLimiteVolumen
        //
        lblLimiteVolumen.AutoSize = true;
        lblLimiteVolumen.Location = new Point(474, 90);
        lblLimiteVolumen.Name = "lblLimiteVolumen";
        lblLimiteVolumen.Text = "Volumen (MB):";
        //
        // numLimiteVolumenMb
        //
        numLimiteVolumenMb.Location = new Point(474, 108);
        numLimiteVolumenMb.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numLimiteVolumenMb.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numLimiteVolumenMb.Name = "numLimiteVolumenMb";
        numLimiteVolumenMb.Size = new Size(130, 23);
        numLimiteVolumenMb.Value = new decimal(new int[] { 100, 0, 0, 0 });
        //
        // lblDestino
        //
        lblDestino.AutoSize = true;
        lblDestino.Location = new Point(16, 144);
        lblDestino.Name = "lblDestino";
        lblDestino.Text = "Destino (idDestinoConfig):";
        //
        // cmbDestino
        //
        cmbDestino.DropDownStyle = ComboBoxStyle.DropDown;
        cmbDestino.Location = new Point(16, 162);
        cmbDestino.Name = "cmbDestino";
        cmbDestino.Size = new Size(300, 23);
        //
        // btnRespaldarAhora
        //
        btnRespaldarAhora.Location = new Point(332, 160);
        btnRespaldarAhora.Name = "btnRespaldarAhora";
        btnRespaldarAhora.Size = new Size(150, 27);
        btnRespaldarAhora.Text = "Respaldar ahora";
        btnRespaldarAhora.UseVisualStyleBackColor = true;
        btnRespaldarAhora.Click += BtnRespaldarAhora_Click;
        //
        // lblResultado
        //
        lblResultado.AutoSize = true;
        lblResultado.Location = new Point(16, 198);
        lblResultado.MaximumSize = new Size(588, 40);
        lblResultado.Name = "lblResultado";
        lblResultado.Text = string.Empty;
        //
        // lblHistorial
        //
        lblHistorial.AutoSize = true;
        lblHistorial.Location = new Point(16, 240);
        lblHistorial.Name = "lblHistorial";
        lblHistorial.Text = "Historial de respaldos:";
        //
        // dgvHistorial
        //
        dgvHistorial.AllowUserToAddRows = false;
        dgvHistorial.AllowUserToDeleteRows = false;
        dgvHistorial.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvHistorial.Location = new Point(16, 260);
        dgvHistorial.Name = "dgvHistorial";
        dgvHistorial.ReadOnly = true;
        dgvHistorial.Size = new Size(588, 260);
        //
        // btnActualizarHistorial
        //
        btnActualizarHistorial.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnActualizarHistorial.Location = new Point(16, 528);
        btnActualizarHistorial.Name = "btnActualizarHistorial";
        btnActualizarHistorial.Size = new Size(150, 27);
        btnActualizarHistorial.Text = "Actualizar";
        btnActualizarHistorial.UseVisualStyleBackColor = true;
        btnActualizarHistorial.Click += BtnActualizarHistorial_Click;
        //
        // notifyIcon1
        //
        notifyIcon1.ContextMenuStrip = contextMenuStrip1;
        notifyIcon1.Text = "Backups - Grupo 4";
        notifyIcon1.Visible = true;
        notifyIcon1.MouseDoubleClick += NotifyIcon1_MouseDoubleClick;
        //
        // contextMenuStrip1
        //
        contextMenuStrip1.Items.AddRange(new ToolStripItem[] { mostrarToolStripMenuItem, salirTrayToolStripMenuItem });
        contextMenuStrip1.Name = "contextMenuStrip1";
        contextMenuStrip1.Size = new Size(120, 48);
        //
        // mostrarToolStripMenuItem
        //
        mostrarToolStripMenuItem.Name = "mostrarToolStripMenuItem";
        mostrarToolStripMenuItem.Size = new Size(119, 22);
        mostrarToolStripMenuItem.Text = "Mostrar";
        mostrarToolStripMenuItem.Click += MostrarToolStripMenuItem_Click;
        //
        // salirTrayToolStripMenuItem
        //
        salirTrayToolStripMenuItem.Name = "salirTrayToolStripMenuItem";
        salirTrayToolStripMenuItem.Size = new Size(119, 22);
        salirTrayToolStripMenuItem.Text = "Salir";
        salirTrayToolStripMenuItem.Click += SalirToolStripMenuItem_Click;
        //
        // FormPrincipal
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(620, 575);
        Controls.Add(dgvHistorial);
        Controls.Add(btnActualizarHistorial);
        Controls.Add(lblHistorial);
        Controls.Add(lblResultado);
        Controls.Add(btnRespaldarAhora);
        Controls.Add(cmbDestino);
        Controls.Add(lblDestino);
        Controls.Add(numLimiteVolumenMb);
        Controls.Add(lblLimiteVolumen);
        Controls.Add(cmbAlgoritmo);
        Controls.Add(lblAlgoritmo);
        Controls.Add(txtNombreCopia);
        Controls.Add(lblNombreCopia);
        Controls.Add(btnSeleccionarOrigen);
        Controls.Add(txtRutaOrigen);
        Controls.Add(lblOrigen);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        MinimumSize = new Size(560, 400);
        Name = "FormPrincipal";
        Text = "Backups - Grupo 4 (Interfaz WinForms)";
        Resize += FormPrincipal_Resize;
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numLimiteVolumenMb).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvHistorial).EndInit();
        contextMenuStrip1.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private MenuStrip menuStrip1;
    private ToolStripMenuItem configuracionToolStripMenuItem;
    private ToolStripMenuItem logsToolStripMenuItem;
    private ToolStripMenuItem salirToolStripMenuItem;
    private Label lblOrigen;
    private TextBox txtRutaOrigen;
    private Button btnSeleccionarOrigen;
    private Label lblNombreCopia;
    private TextBox txtNombreCopia;
    private Label lblAlgoritmo;
    private ComboBox cmbAlgoritmo;
    private Label lblLimiteVolumen;
    private NumericUpDown numLimiteVolumenMb;
    private Label lblDestino;
    private ComboBox cmbDestino;
    private Button btnRespaldarAhora;
    private Label lblResultado;
    private Label lblHistorial;
    private DataGridView dgvHistorial;
    private Button btnActualizarHistorial;
    private NotifyIcon notifyIcon1;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem mostrarToolStripMenuItem;
    private ToolStripMenuItem salirTrayToolStripMenuItem;
}
