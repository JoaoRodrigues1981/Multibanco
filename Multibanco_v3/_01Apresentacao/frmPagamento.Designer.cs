namespace Multibanco
{
    partial class frmPagamento
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
            lblTitulo      = new Label();
            lblEntidade    = new Label();
            txtEntidade    = new TextBox();
            lblReferencia  = new Label();
            txtReferencia  = new TextBox();
            lblValor       = new Label();
            txtValorPag    = new TextBox();
            btnPagar       = new Button();
            btnCancelar    = new Button();
            SuspendLayout();
            //
            // lblTitulo
            //
            lblTitulo.Font      = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.DarkRed;
            lblTitulo.Location  = new Point(12, 12);
            lblTitulo.Size      = new Size(376, 32);
            lblTitulo.Text      = "Pagamento de Serviço";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lblEntidade
            //
            lblEntidade.Location = new Point(12, 65);
            lblEntidade.Size     = new Size(100, 25);
            lblEntidade.Text     = "Entidade:";
            lblEntidade.TextAlign = ContentAlignment.MiddleRight;
            //
            // txtEntidade
            //
            txtEntidade.Location  = new Point(118, 62);
            txtEntidade.MaxLength = 5;
            txtEntidade.Size      = new Size(80, 27);
            txtEntidade.Name      = "txtEntidade";
            txtEntidade.KeyPress += txtEntidade_KeyPress;
            //
            // lblReferencia
            //
            lblReferencia.Location  = new Point(12, 105);
            lblReferencia.Size      = new Size(100, 25);
            lblReferencia.Text      = "Referência:";
            lblReferencia.TextAlign = ContentAlignment.MiddleRight;
            //
            // txtReferencia
            //
            txtReferencia.Location  = new Point(118, 102);
            txtReferencia.MaxLength = 9;
            txtReferencia.Size      = new Size(120, 27);
            txtReferencia.Name      = "txtReferencia";
            txtReferencia.KeyPress += txtReferencia_KeyPress;
            //
            // lblValor
            //
            lblValor.Location  = new Point(12, 145);
            lblValor.Size      = new Size(100, 25);
            lblValor.Text      = "Valor (€):";
            lblValor.TextAlign = ContentAlignment.MiddleRight;
            //
            // txtValorPag
            //
            txtValorPag.Location = new Point(118, 142);
            txtValorPag.Size     = new Size(120, 27);
            txtValorPag.Name     = "txtValorPag";
            //
            // btnPagar
            //
            btnPagar.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPagar.Location = new Point(60, 190);
            btnPagar.Size     = new Size(120, 35);
            btnPagar.Text     = "Pagar";
            btnPagar.Click   += btnPagar_Click;
            //
            // btnCancelar
            //
            btnCancelar.Location = new Point(210, 190);
            btnCancelar.Size     = new Size(120, 35);
            btnCancelar.Text     = "Cancelar";
            btnCancelar.Click   += btnCancelar_Click;
            //
            // frmPagamento
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(400, 250);
            FormBorderStyle     = FormBorderStyle.FixedDialog;
            MaximizeBox         = false;
            MinimizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            Text                = "Pagamento de Serviço";
            Controls.Add(lblTitulo);
            Controls.Add(lblEntidade);
            Controls.Add(txtEntidade);
            Controls.Add(lblReferencia);
            Controls.Add(txtReferencia);
            Controls.Add(lblValor);
            Controls.Add(txtValorPag);
            Controls.Add(btnPagar);
            Controls.Add(btnCancelar);
            ResumeLayout(false);
            PerformLayout();
        }

        private Label   lblTitulo;
        private Label   lblEntidade;
        private TextBox txtEntidade;
        private Label   lblReferencia;
        private TextBox txtReferencia;
        private Label   lblValor;
        private TextBox txtValorPag;
        private Button  btnPagar;
        private Button  btnCancelar;
    }
}
