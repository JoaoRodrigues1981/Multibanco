namespace Multibanco
{
    partial class frmMultibanco
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblBanco = new Label();
            lblCliente = new Label();
            label1 = new Label();
            lblSaldo = new Label();
            txtSaldo = new TextBox();
            lblMontante = new Label();
            txtValor = new TextBox();
            lblContaDestino = new Label();
            txtContaDestino = new TextBox();
            btnLevantar = new Button();
            btnDepositar = new Button();
            btnTransferir = new Button();
            btnMBWay = new Button();
            btnPagamentos = new Button();
            btnSimulador = new Button();
            btnSair = new Button();
            grpHistorico = new GroupBox();
            lstMovimentos = new ListView();
            lblDe = new Label();
            dtpInicio = new DateTimePicker();
            lblAte = new Label();
            dtpFim = new DateTimePicker();
            btnFiltrar = new Button();
            btnTodos = new Button();
            grpHistorico.SuspendLayout();
            SuspendLayout();
            // 
            // lblBanco
            // 
            lblBanco.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBanco.Location = new Point(10, 8);
            lblBanco.Name = "lblBanco";
            lblBanco.Size = new Size(171, 20);
            lblBanco.TabIndex = 18;
            lblBanco.Text = "BANCO";
            // 
            // lblCliente
            // 
            lblCliente.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCliente.Location = new Point(186, 8);
            lblCliente.Name = "lblCliente";
            lblCliente.Size = new Size(324, 20);
            lblCliente.TabIndex = 17;
            lblCliente.Text = "Cliente";
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            label1.ForeColor = Color.DarkRed;
            label1.Location = new Point(271, 28);
            label1.Name = "label1";
            label1.Size = new Size(319, 59);
            label1.TabIndex = 2;
            label1.Text = "MULTIBANCO";
            label1.Click += label1_Click;
            // 
            // lblSaldo
            // 
            lblSaldo.Font = new Font("Segoe UI", 8F);
            lblSaldo.ForeColor = Color.FromArgb(160, 160, 160);
            lblSaldo.Location = new Point(534, 11);
            lblSaldo.Name = "lblSaldo";
            lblSaldo.Size = new Size(102, 14);
            lblSaldo.TabIndex = 0;
            lblSaldo.Text = "Saldo disponível:";
            lblSaldo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSaldo
            // 
            txtSaldo.Enabled = false;
            txtSaldo.Font = new Font("Segoe UI", 16F);
            txtSaldo.ForeColor = Color.Yellow;
            txtSaldo.Location = new Point(640, 11);
            txtSaldo.Margin = new Padding(3, 2, 3, 2);
            txtSaldo.Name = "txtSaldo";
            txtSaldo.Size = new Size(168, 36);
            txtSaldo.TabIndex = 6;
            // 
            // lblMontante
            // 
            lblMontante.Font = new Font("Segoe UI", 9F);
            lblMontante.Location = new Point(160, 86);
            lblMontante.Name = "lblMontante";
            lblMontante.Size = new Size(105, 14);
            lblMontante.TabIndex = 16;
            lblMontante.Text = "Montante (€):";
            // 
            // txtValor
            // 
            txtValor.Font = new Font("Segoe UI", 18F);
            txtValor.ForeColor = Color.Yellow;
            txtValor.Location = new Point(160, 109);
            txtValor.Margin = new Padding(3, 2, 3, 2);
            txtValor.Name = "txtValor";
            txtValor.Size = new Size(492, 39);
            txtValor.TabIndex = 5;
            // 
            // lblContaDestino
            // 
            lblContaDestino.Font = new Font("Segoe UI", 9F);
            lblContaDestino.Location = new Point(285, 164);
            lblContaDestino.Name = "lblContaDestino";
            lblContaDestino.Size = new Size(96, 15);
            lblContaDestino.TabIndex = 14;
            lblContaDestino.Text = "Conta destino:";
            // 
            // txtContaDestino
            // 
            txtContaDestino.Location = new Point(387, 161);
            txtContaDestino.Margin = new Padding(3, 2, 3, 2);
            txtContaDestino.Name = "txtContaDestino";
            txtContaDestino.Size = new Size(123, 23);
            txtContaDestino.TabIndex = 15;
            // 
            // btnLevantar
            // 
            btnLevantar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLevantar.Location = new Point(10, 86);
            btnLevantar.Margin = new Padding(3, 2, 3, 2);
            btnLevantar.Name = "btnLevantar";
            btnLevantar.Size = new Size(136, 51);
            btnLevantar.TabIndex = 3;
            btnLevantar.Text = "Levantar";
            btnLevantar.UseVisualStyleBackColor = true;
            btnLevantar.Click += btnLevantar_Click;
            // 
            // btnDepositar
            // 
            btnDepositar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDepositar.Location = new Point(672, 86);
            btnDepositar.Margin = new Padding(3, 2, 3, 2);
            btnDepositar.Name = "btnDepositar";
            btnDepositar.Size = new Size(136, 51);
            btnDepositar.TabIndex = 4;
            btnDepositar.Text = "Depositar";
            btnDepositar.UseVisualStyleBackColor = true;
            btnDepositar.Click += btnDepositar_Click;
            // 
            // btnTransferir
            // 
            btnTransferir.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnTransferir.Location = new Point(10, 146);
            btnTransferir.Margin = new Padding(3, 2, 3, 2);
            btnTransferir.Name = "btnTransferir";
            btnTransferir.Size = new Size(136, 51);
            btnTransferir.TabIndex = 8;
            btnTransferir.Text = "Transferir";
            btnTransferir.UseVisualStyleBackColor = true;
            btnTransferir.Click += btnTransferir_Click;
            // 
            // btnMBWay
            // 
            btnMBWay.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnMBWay.Location = new Point(10, 206);
            btnMBWay.Margin = new Padding(3, 2, 3, 2);
            btnMBWay.Name = "btnMBWay";
            btnMBWay.Size = new Size(136, 51);
            btnMBWay.TabIndex = 9;
            btnMBWay.Text = "MBWay";
            btnMBWay.UseVisualStyleBackColor = true;
            btnMBWay.Click += btnMBWay_Click;
            // 
            // btnPagamentos
            // 
            btnPagamentos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPagamentos.Location = new Point(10, 267);
            btnPagamentos.Margin = new Padding(3, 2, 3, 2);
            btnPagamentos.Name = "btnPagamentos";
            btnPagamentos.Size = new Size(136, 51);
            btnPagamentos.TabIndex = 12;
            btnPagamentos.Text = "Pagamento de Serviços";
            btnPagamentos.UseVisualStyleBackColor = true;
            btnPagamentos.Click += btnPagamentos_Click;
            // 
            // btnSimulador
            // 
            btnSimulador.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSimulador.Location = new Point(672, 146);
            btnSimulador.Margin = new Padding(3, 2, 3, 2);
            btnSimulador.Name = "btnSimulador";
            btnSimulador.Size = new Size(136, 51);
            btnSimulador.TabIndex = 11;
            btnSimulador.Text = "Simulador\nEmpréstimo";
            btnSimulador.UseVisualStyleBackColor = true;
            btnSimulador.Click += btnSimulador_Click;
            // 
            // btnSair
            // 
            btnSair.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSair.Location = new Point(672, 206);
            btnSair.Margin = new Padding(3, 2, 3, 2);
            btnSair.Name = "btnSair";
            btnSair.Size = new Size(136, 51);
            btnSair.TabIndex = 10;
            btnSair.Text = "Sair";
            btnSair.UseVisualStyleBackColor = true;
            btnSair.Click += btnSair_Click;
            // 
            // grpHistorico
            // 
            grpHistorico.Controls.Add(lstMovimentos);
            grpHistorico.Controls.Add(lblDe);
            grpHistorico.Controls.Add(dtpInicio);
            grpHistorico.Controls.Add(lblAte);
            grpHistorico.Controls.Add(dtpFim);
            grpHistorico.Controls.Add(btnFiltrar);
            grpHistorico.Controls.Add(btnTodos);
            grpHistorico.Font = new Font("Segoe UI", 9F);
            grpHistorico.Location = new Point(160, 206);
            grpHistorico.Margin = new Padding(3, 2, 3, 2);
            grpHistorico.Name = "grpHistorico";
            grpHistorico.Padding = new Padding(3, 2, 3, 2);
            grpHistorico.Size = new Size(492, 158);
            grpHistorico.TabIndex = 13;
            grpHistorico.TabStop = false;
            grpHistorico.Text = "Histórico de Movimentos";
            // 
            // lstMovimentos
            // 
            lstMovimentos.FullRowSelect = true;
            lstMovimentos.GridLines = true;
            lstMovimentos.Location = new Point(4, 15);
            lstMovimentos.Margin = new Padding(3, 2, 3, 2);
            lstMovimentos.Name = "lstMovimentos";
            lstMovimentos.Size = new Size(482, 97);
            lstMovimentos.TabIndex = 7;
            lstMovimentos.UseCompatibleStateImageBehavior = false;
            lstMovimentos.View = View.Details;
            // 
            // lblDe
            // 
            lblDe.Font = new Font("Segoe UI", 9F);
            lblDe.Location = new Point(4, 124);
            lblDe.Name = "lblDe";
            lblDe.Size = new Size(24, 15);
            lblDe.TabIndex = 8;
            lblDe.Text = "De:";
            // 
            // dtpInicio
            // 
            dtpInicio.Format = DateTimePickerFormat.Short;
            dtpInicio.Location = new Point(52, 120);
            dtpInicio.Margin = new Padding(3, 2, 3, 2);
            dtpInicio.Name = "dtpInicio";
            dtpInicio.Size = new Size(98, 23);
            dtpInicio.TabIndex = 9;
            // 
            // lblAte
            // 
            lblAte.Font = new Font("Segoe UI", 9F);
            lblAte.Location = new Point(193, 124);
            lblAte.Name = "lblAte";
            lblAte.Size = new Size(28, 15);
            lblAte.TabIndex = 10;
            lblAte.Text = "Até:";
            // 
            // dtpFim
            // 
            dtpFim.Format = DateTimePickerFormat.Short;
            dtpFim.Location = new Point(239, 120);
            dtpFim.Margin = new Padding(3, 2, 3, 2);
            dtpFim.Name = "dtpFim";
            dtpFim.Size = new Size(98, 23);
            dtpFim.TabIndex = 11;
            // 
            // btnFiltrar
            // 
            btnFiltrar.Font = new Font("Segoe UI", 9F);
            btnFiltrar.Location = new Point(343, 120);
            btnFiltrar.Margin = new Padding(3, 2, 3, 2);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(52, 23);
            btnFiltrar.TabIndex = 12;
            btnFiltrar.Text = "Filtrar";
            btnFiltrar.Click += btnFiltrar_Click;
            // 
            // btnTodos
            // 
            btnTodos.Font = new Font("Segoe UI", 9F);
            btnTodos.Location = new Point(401, 120);
            btnTodos.Margin = new Padding(3, 2, 3, 2);
            btnTodos.Name = "btnTodos";
            btnTodos.Size = new Size(75, 23);
            btnTodos.TabIndex = 13;
            btnTodos.Text = "Ver Todos";
            btnTodos.Click += btnTodos_Click;
            // 
            // frmMultibanco
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(839, 426);
            Controls.Add(lblSaldo);
            Controls.Add(txtSaldo);
            Controls.Add(btnSair);
            Controls.Add(btnSimulador);
            Controls.Add(btnPagamentos);
            Controls.Add(btnMBWay);
            Controls.Add(grpHistorico);
            Controls.Add(lblContaDestino);
            Controls.Add(txtContaDestino);
            Controls.Add(btnTransferir);
            Controls.Add(lblMontante);
            Controls.Add(txtValor);
            Controls.Add(btnDepositar);
            Controls.Add(btnLevantar);
            Controls.Add(label1);
            Controls.Add(lblCliente);
            Controls.Add(lblBanco);
            Margin = new Padding(3, 2, 3, 2);
            Name = "frmMultibanco";
            Text = "Multibanco";
            Load += frmMultibanco_Load;
            grpHistorico.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label          lblBanco;
        private Label          lblCliente;
        private Label          label1;
        private Label          lblSaldo;
        private Label          lblMontante;
        private TextBox        txtValor;
        private TextBox        txtSaldo;
        private ListView       lstMovimentos;
        private Button         btnLevantar;
        private Button         btnDepositar;
        private Button         btnTransferir;
        private Button         btnMBWay;
        private Button         btnSair;
        private Button         btnSimulador;
        private Button         btnPagamentos;
        private GroupBox       grpHistorico;
        private Label          lblDe;
        private DateTimePicker dtpInicio;
        private Label          lblAte;
        private DateTimePicker dtpFim;
        private Button         btnFiltrar;
        private Button         btnTodos;
        private Label          lblContaDestino;
        private TextBox        txtContaDestino;
    }
}
