namespace Backups.Adapters.UI.WinForms;

partial class FormHistorialLogs
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
        dgvLogs = new DataGridView();
        btnActualizar = new Button();
        ((System.ComponentModel.ISupportInitialize)dgvLogs).BeginInit();
        SuspendLayout();
        //
        // dgvLogs
        //
        dgvLogs.AllowUserToAddRows = false;
        dgvLogs.AllowUserToDeleteRows = false;
        dgvLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvLogs.Location = new Point(12, 12);
        dgvLogs.Name = "dgvLogs";
        dgvLogs.ReadOnly = true;
        dgvLogs.Size = new Size(660, 400);
        //
        // btnActualizar
        //
        btnActualizar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnActualizar.Location = new Point(12, 420);
        btnActualizar.Name = "btnActualizar";
        btnActualizar.Size = new Size(150, 27);
        btnActualizar.Text = "Actualizar";
        btnActualizar.UseVisualStyleBackColor = true;
        btnActualizar.Click += BtnActualizar_Click;
        //
        // FormHistorialLogs
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(684, 461);
        Controls.Add(btnActualizar);
        Controls.Add(dgvLogs);
        MinimumSize = new Size(500, 300);
        Name = "FormHistorialLogs";
        Text = "Ventana de Logs";
        ((System.ComponentModel.ISupportInitialize)dgvLogs).EndInit();
        ResumeLayout(false);
    }

    private DataGridView dgvLogs;
    private Button btnActualizar;
}
