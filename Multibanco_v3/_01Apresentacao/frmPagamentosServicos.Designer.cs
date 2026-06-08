namespace Multibanco
{
    partial class frmPagamentosServicos
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
            lblTitulo        = new Label();
            lblServico       = new Label();
            lstServicos      = new ListBox();
            lblEntidade      = new Label();
            lblEntidadeValor = new Label();
            lblReferencia    = new Label();
            txtReferencia    = new TextBox();
            lblValor         = new Label();
            txtValorServ     = new TextBox();
            btnPagar         = new Button();
            btnCancelar      = new Button();
            SuspendLayout();

            // lblTitulo
            lblTitulo.Font      = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.DarkRed;
            lblTitulo.Location  = new Point(12, 12);
            lblTitulo.Size      = new Size(396, 30);
            lblTitulo.Text      = "Pagamento de Serviços";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            // lblServico
            lblServico.Location  = new Point(12, 55);
            lblServico.Size      = new Size(110, 22);
            lblServico.Text      = "Selecione o serviço:";

            // lstServicos
            lstServicos.Location              = new Point(12, 78);
            lstServicos.Size                  = new Size(220, 175);
            lstServicos.Name                  = "lstServicos";
            lstServicos.SelectedIndexChanged += lstServicos_SelectedIndexChanged;

            // lblEntidade
            lblEntidade.Location  = new Point(248, 78);
            lblEntidade.Size      = new Size(65, 22);
            lblEntidade.Text      = "Entidade:";
            lblEntidade.TextAlign = ContentAlignment.MiddleRight;

            // lblEntidadeValor — preenchido automaticamente pela seleção
            lblEntidadeValor.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEntidadeValor.Location  = new Point(318, 78);
            lblEntidadeValor.Size      = new Size(80, 22);
            lblEntidadeValor.TextAlign = ContentAlignment.MiddleLeft;

            // lblReferencia
            lblReferencia.Location  = new Point(248, 118);
            lblReferencia.Size      = new Size(65, 22);
            lblReferencia.Text      = "Referência:";
            lblReferencia.TextAlign = ContentAlignment.MiddleRight;

            // txtReferencia
            txtReferencia.Location  = new Point(318, 115);
            txtReferencia.MaxLength = 9;
            txtReferencia.Size      = new Size(90, 27);
            txtReferencia.Name      = "txtReferencia";
            txtReferencia.KeyPress += txtReferencia_KeyPress;

            // lblValor
            lblValor.Location  = new Point(248, 158);
            lblValor.Size      = new Size(65, 22);
            lblValor.Text      = "Valor (€):";
            lblValor.TextAlign = ContentAlignment.MiddleRight;

            // txtValorServ
            txtValorServ.Location = new Point(318, 155);
            txtValorServ.Size     = new Size(90, 27);
            txtValorServ.Name     = "txtValorServ";

            // btnPagar
            btnPagar.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPagar.Location = new Point(248, 205);
            btnPagar.Size     = new Size(75, 32);
            btnPagar.Text     = "Pagar";
            btnPagar.Click   += btnPagar_Click;

            // btnCancelar
            btnCancelar.Location = new Point(333, 205);
            btnCancelar.Size     = new Size(75, 32);
            btnCancelar.Text     = "Cancelar";
            btnCancelar.Click   += btnCancelar_Click;

            // frmPagamentosServicos
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(425, 270);
            FormBorderStyle     = FormBorderStyle.FixedDialog;
            MaximizeBox         = false;
            MinimizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            Text                = "Pagamento de Serviços";

            Controls.Add(lblTitulo);
            Controls.Add(lblServico);
            Controls.Add(lstServicos);
            Controls.Add(lblEntidade);
            Controls.Add(lblEntidadeValor);
            Controls.Add(lblReferencia);
            Controls.Add(txtReferencia);
            Controls.Add(lblValor);
            Controls.Add(txtValorServ);
            Controls.Add(btnPagar);
            Controls.Add(btnCancelar);

            ResumeLayout(false);
            PerformLayout();
        }

        private Label   lblTitulo;
        private Label   lblServico;
        private ListBox lstServicos;
        private Label   lblEntidade;
        private Label   lblEntidadeValor;
        private Label   lblReferencia;
        private TextBox txtReferencia;
        private Label   lblValor;
        private TextBox txtValorServ;
        private Button  btnPagar;
        private Button  btnCancelar;
    }
}
