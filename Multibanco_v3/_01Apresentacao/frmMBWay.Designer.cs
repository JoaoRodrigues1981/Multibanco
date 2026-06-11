namespace Multibanco
{
    partial class frmMBWay
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitulo   = new Label();
            lblContas   = new Label();
            lstContas   = new ListBox();
            lblValor    = new Label();
            lblLimite   = new Label();
            txtValor    = new TextBox();
            btnEnviar   = new Button();
            btnCancelar = new Button();
            SuspendLayout();

            // lblTitulo
            lblTitulo.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.Location  = new Point(12, 12);
            lblTitulo.Size      = new Size(380, 30);
            lblTitulo.Text      = "Enviar MBWay";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            // lblContas
            lblContas.Location = new Point(12, 55);
            lblContas.Size     = new Size(180, 22);
            lblContas.Text     = "Selecione o destinatário:";

            // lstContas
            lstContas.Location = new Point(12, 78);
            lstContas.Size     = new Size(380, 160);
            lstContas.Name     = "lstContas";

            // lblValor
            lblValor.Location  = new Point(12, 255);
            lblValor.Size      = new Size(70, 22);
            lblValor.Text      = "Valor (€):";
            lblValor.TextAlign = ContentAlignment.MiddleRight;

            // txtValor
            txtValor.Location = new Point(88, 252);
            txtValor.Size     = new Size(100, 27);
            txtValor.Name     = "txtValor";

            // lblLimite — lembrete do limite de 300€
            lblLimite.Location  = new Point(195, 255);
            lblLimite.Size      = new Size(80, 22);
            lblLimite.Text      = "(máx. 300€)";
            lblLimite.ForeColor = Color.Gray;

            // btnEnviar
            btnEnviar.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEnviar.Location = new Point(210, 290);
            btnEnviar.Size     = new Size(85, 32);
            btnEnviar.Text     = "Enviar";
            btnEnviar.Click   += btnEnviar_Click;

            // btnCancelar
            btnCancelar.Location = new Point(305, 290);
            btnCancelar.Size     = new Size(85, 32);
            btnCancelar.Text     = "Cancelar";
            btnCancelar.Click   += btnCancelar_Click;

            // frmMBWay
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(406, 340);
            FormBorderStyle     = FormBorderStyle.FixedDialog;
            MaximizeBox         = false;
            MinimizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            Text                = "MBWay";

            Controls.Add(lblTitulo);
            Controls.Add(lblContas);
            Controls.Add(lstContas);
            Controls.Add(lblValor);
            Controls.Add(txtValor);
            Controls.Add(lblLimite);
            Controls.Add(btnEnviar);
            Controls.Add(btnCancelar);

            ResumeLayout(false);
            PerformLayout();
        }

        private Label   lblTitulo;
        private Label   lblContas;
        private ListBox lstContas;
        private Label   lblValor;
        private Label   lblLimite;
        private TextBox txtValor;
        private Button  btnEnviar;
        private Button  btnCancelar;
    }
}
