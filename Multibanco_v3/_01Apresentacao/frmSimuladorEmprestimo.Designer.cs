namespace Multibanco
{
    partial class frmSimuladorEmprestimo
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
            lblTitulo    = new Label();
            // Inputs
            lblValorEmp  = new Label();
            txtValorEmp  = new TextBox();
            lblPrazo     = new Label();
            cmbPrazo     = new ComboBox();
            lblEuribor   = new Label();
            txtEuribor   = new TextBox();
            lblSpread    = new Label();
            txtSpread    = new TextBox();
            lblTaxaFixa  = new Label();
            txtTaxaFixa  = new TextBox();
            btnCalcular  = new Button();
            // Cabeçalhos de resultados
            lblColIndexado = new Label();
            lblColFixo     = new Label();
            // Linhas de resultados
            lblRowTAN    = new Label();
            lblTAN_I     = new Label();
            lblTAN_F     = new Label();
            lblRowPrest  = new Label();
            lblPrest_I   = new Label();
            lblPrest_F   = new Label();
            lblRowJuros  = new Label();
            lblJuros_I   = new Label();
            lblJuros_F   = new Label();
            lblRowCusto  = new Label();
            lblCusto_I   = new Label();
            lblCusto_F   = new Label();
            btnFechar    = new Button();
            SuspendLayout();

            // lblTitulo
            lblTitulo.Font      = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.DarkRed;
            lblTitulo.Location  = new Point(12, 12);
            lblTitulo.Size      = new Size(476, 32);
            lblTitulo.Text      = "Simulador de Empréstimo";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

            // lblValorEmp
            lblValorEmp.Location  = new Point(15, 60);
            lblValorEmp.Size      = new Size(175, 25);
            lblValorEmp.Text      = "Valor do empréstimo (€):";
            lblValorEmp.TextAlign = ContentAlignment.MiddleRight;
            // txtValorEmp
            txtValorEmp.Location = new Point(198, 57);
            txtValorEmp.Size     = new Size(110, 27);
            txtValorEmp.Name     = "txtValorEmp";

            // lblPrazo
            lblPrazo.Location  = new Point(15, 95);
            lblPrazo.Size      = new Size(175, 25);
            lblPrazo.Text      = "Prazo (anos):";
            lblPrazo.TextAlign = ContentAlignment.MiddleRight;
            // cmbPrazo
            cmbPrazo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPrazo.Items.AddRange(new object[] { "5", "10", "15", "20", "25", "30" });
            cmbPrazo.SelectedIndex = 3; // 20 anos por defeito
            cmbPrazo.Location = new Point(198, 92);
            cmbPrazo.Size     = new Size(70, 27);
            cmbPrazo.Name     = "cmbPrazo";

            // lblEuribor
            lblEuribor.Location  = new Point(15, 130);
            lblEuribor.Size      = new Size(175, 25);
            lblEuribor.Text      = "Euribor 12M atual (%):";
            lblEuribor.TextAlign = ContentAlignment.MiddleRight;
            // txtEuribor
            txtEuribor.Location = new Point(198, 127);
            txtEuribor.Size     = new Size(70, 27);
            txtEuribor.Text     = "2.50";
            txtEuribor.Name     = "txtEuribor";

            // lblSpread
            lblSpread.Location  = new Point(15, 165);
            lblSpread.Size      = new Size(175, 25);
            lblSpread.Text      = "Spread (%):";
            lblSpread.TextAlign = ContentAlignment.MiddleRight;
            // txtSpread
            txtSpread.Location = new Point(198, 162);
            txtSpread.Size     = new Size(70, 27);
            txtSpread.Text     = "1.50";
            txtSpread.Name     = "txtSpread";

            // lblTaxaFixa
            lblTaxaFixa.Location  = new Point(15, 200);
            lblTaxaFixa.Size      = new Size(175, 25);
            lblTaxaFixa.Text      = "Taxa fixa — TAN (%):";
            lblTaxaFixa.TextAlign = ContentAlignment.MiddleRight;
            // txtTaxaFixa
            txtTaxaFixa.Location = new Point(198, 197);
            txtTaxaFixa.Size     = new Size(70, 27);
            txtTaxaFixa.Text     = "5.50";
            txtTaxaFixa.Name     = "txtTaxaFixa";

            // btnCalcular
            btnCalcular.Font     = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCalcular.Location = new Point(198, 235);
            btnCalcular.Size     = new Size(110, 32);
            btnCalcular.Text     = "Calcular";
            btnCalcular.Click   += btnCalcular_Click;

            // --- Cabeçalhos de resultados ---
            lblColIndexado.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblColIndexado.Location  = new Point(205, 285);
            lblColIndexado.Size      = new Size(135, 20);
            lblColIndexado.Text      = "Indexado (Eur+Spr)";
            lblColIndexado.TextAlign = ContentAlignment.MiddleCenter;

            lblColFixo.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblColFixo.Location  = new Point(350, 285);
            lblColFixo.Size      = new Size(120, 20);
            lblColFixo.Text      = "Taxa Fixa";
            lblColFixo.TextAlign = ContentAlignment.MiddleCenter;

            // --- Linha TAN ---
            lblRowTAN.Location  = new Point(15, 313);
            lblRowTAN.Size      = new Size(185, 22);
            lblRowTAN.Text      = "TAN:";
            lblRowTAN.TextAlign = ContentAlignment.MiddleRight;
            lblTAN_I.Location   = new Point(205, 313);
            lblTAN_I.Size       = new Size(135, 22);
            lblTAN_I.TextAlign  = ContentAlignment.MiddleCenter;
            lblTAN_F.Location   = new Point(350, 313);
            lblTAN_F.Size       = new Size(120, 22);
            lblTAN_F.TextAlign  = ContentAlignment.MiddleCenter;

            // --- Linha Prestação ---
            lblRowPrest.Location  = new Point(15, 341);
            lblRowPrest.Size      = new Size(185, 22);
            lblRowPrest.Text      = "Prestação mensal:";
            lblRowPrest.TextAlign = ContentAlignment.MiddleRight;
            lblPrest_I.Location   = new Point(205, 341);
            lblPrest_I.Size       = new Size(135, 22);
            lblPrest_I.TextAlign  = ContentAlignment.MiddleCenter;
            lblPrest_F.Location   = new Point(350, 341);
            lblPrest_F.Size       = new Size(120, 22);
            lblPrest_F.TextAlign  = ContentAlignment.MiddleCenter;

            // --- Linha Juros ---
            lblRowJuros.Location  = new Point(15, 369);
            lblRowJuros.Size      = new Size(185, 22);
            lblRowJuros.Text      = "Total de juros:";
            lblRowJuros.TextAlign = ContentAlignment.MiddleRight;
            lblJuros_I.Location   = new Point(205, 369);
            lblJuros_I.Size       = new Size(135, 22);
            lblJuros_I.TextAlign  = ContentAlignment.MiddleCenter;
            lblJuros_F.Location   = new Point(350, 369);
            lblJuros_F.Size       = new Size(120, 22);
            lblJuros_F.TextAlign  = ContentAlignment.MiddleCenter;

            // --- Linha Custo Total ---
            lblRowCusto.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblRowCusto.Location  = new Point(15, 397);
            lblRowCusto.Size      = new Size(185, 22);
            lblRowCusto.Text      = "Custo total:";
            lblRowCusto.TextAlign = ContentAlignment.MiddleRight;
            lblCusto_I.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCusto_I.Location  = new Point(205, 397);
            lblCusto_I.Size      = new Size(135, 22);
            lblCusto_I.TextAlign = ContentAlignment.MiddleCenter;
            lblCusto_F.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCusto_F.Location  = new Point(350, 397);
            lblCusto_F.Size      = new Size(120, 22);
            lblCusto_F.TextAlign = ContentAlignment.MiddleCenter;

            // btnFechar
            btnFechar.Location = new Point(375, 430);
            btnFechar.Size     = new Size(100, 30);
            btnFechar.Text     = "Fechar";
            btnFechar.Click   += btnFechar_Click;

            // frmSimuladorEmprestimo
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(500, 475);
            FormBorderStyle     = FormBorderStyle.FixedDialog;
            MaximizeBox         = false;
            MinimizeBox         = false;
            StartPosition       = FormStartPosition.CenterParent;
            Text                = "Simulador de Empréstimo";

            Controls.Add(lblTitulo);
            Controls.Add(lblValorEmp);  Controls.Add(txtValorEmp);
            Controls.Add(lblPrazo);     Controls.Add(cmbPrazo);
            Controls.Add(lblEuribor);   Controls.Add(txtEuribor);
            Controls.Add(lblSpread);    Controls.Add(txtSpread);
            Controls.Add(lblTaxaFixa);  Controls.Add(txtTaxaFixa);
            Controls.Add(btnCalcular);
            Controls.Add(lblColIndexado); Controls.Add(lblColFixo);
            Controls.Add(lblRowTAN);   Controls.Add(lblTAN_I);   Controls.Add(lblTAN_F);
            Controls.Add(lblRowPrest); Controls.Add(lblPrest_I); Controls.Add(lblPrest_F);
            Controls.Add(lblRowJuros); Controls.Add(lblJuros_I); Controls.Add(lblJuros_F);
            Controls.Add(lblRowCusto); Controls.Add(lblCusto_I); Controls.Add(lblCusto_F);
            Controls.Add(btnFechar);

            ResumeLayout(false);
            PerformLayout();
        }

        private Label    lblTitulo;
        private Label    lblValorEmp;
        private TextBox  txtValorEmp;
        private Label    lblPrazo;
        private ComboBox cmbPrazo;
        private Label    lblEuribor;
        private TextBox  txtEuribor;
        private Label    lblSpread;
        private TextBox  txtSpread;
        private Label    lblTaxaFixa;
        private TextBox  txtTaxaFixa;
        private Button   btnCalcular;
        private Label    lblColIndexado;
        private Label    lblColFixo;
        private Label    lblRowTAN;
        private Label    lblTAN_I;
        private Label    lblTAN_F;
        private Label    lblRowPrest;
        private Label    lblPrest_I;
        private Label    lblPrest_F;
        private Label    lblRowJuros;
        private Label    lblJuros_I;
        private Label    lblJuros_F;
        private Label    lblRowCusto;
        private Label    lblCusto_I;
        private Label    lblCusto_F;
        private Button   btnFechar;
    }
}
