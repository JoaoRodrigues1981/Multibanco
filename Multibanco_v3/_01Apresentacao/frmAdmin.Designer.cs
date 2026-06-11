namespace Multibanco._01Apresentacao
{
    partial class frmAdmin
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
            this.lblTitulo       = new System.Windows.Forms.Label();
            this.lvClientes      = new System.Windows.Forms.ListView();
            this.colId           = new System.Windows.Forms.ColumnHeader();
            this.colBanco        = new System.Windows.Forms.ColumnHeader();
            this.colCliente      = new System.Windows.Forms.ColumnHeader();
            this.colConta        = new System.Windows.Forms.ColumnHeader();
            this.colSaldo        = new System.Windows.Forms.ColumnHeader();
            this.colMBWay        = new System.Windows.Forms.ColumnHeader();
            this.lblTotal        = new System.Windows.Forms.Label();
            this.grpInserir      = new System.Windows.Forms.GroupBox();
            this.lblNovoBanco    = new System.Windows.Forms.Label();
            this.txtNovoBanco    = new System.Windows.Forms.TextBox();
            this.lblNovoCliente  = new System.Windows.Forms.Label();
            this.txtNovoCliente  = new System.Windows.Forms.TextBox();
            this.lblNovaConta    = new System.Windows.Forms.Label();
            this.txtNovaConta    = new System.Windows.Forms.TextBox();
            this.lblNovoPin      = new System.Windows.Forms.Label();
            this.txtNovoPin      = new System.Windows.Forms.TextBox();
            this.btnInserir      = new System.Windows.Forms.Button();
            this.btnEliminar     = new System.Windows.Forms.Button();
            this.btnMBWay        = new System.Windows.Forms.Button();
            this.btnDesbloquear  = new System.Windows.Forms.Button();
            this.btnSair         = new System.Windows.Forms.Button();
            this.grpInserir.SuspendLayout();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.Font      = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location  = new System.Drawing.Point(12, 10);
            this.lblTitulo.Size      = new System.Drawing.Size(300, 30);
            this.lblTitulo.Text      = "BackOffice — SIBS";

            // lvClientes
            this.lvClientes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colId, this.colBanco, this.colCliente, this.colConta, this.colSaldo, this.colMBWay });
            this.lvClientes.FullRowSelect = true;         // selecionar linha inteira
            this.lvClientes.GridLines     = true;         // linhas de grelha para legibilidade
            this.lvClientes.MultiSelect   = false;        // só uma linha de cada vez
            this.lvClientes.View          = System.Windows.Forms.View.Details;
            this.lvClientes.Location      = new System.Drawing.Point(12, 48);
            this.lvClientes.Size          = new System.Drawing.Size(660, 220);

            // colunas do ListView
            this.colId.Text      = "Id";      this.colId.Width      = 40;
            this.colBanco.Text   = "Banco";   this.colBanco.Width   = 80;
            this.colCliente.Text = "Cliente"; this.colCliente.Width = 160;
            this.colConta.Text   = "Conta";   this.colConta.Width   = 80;
            this.colSaldo.Text   = "Saldo €"; this.colSaldo.Width   = 90;
            this.colMBWay.Text   = "MBWay";   this.colMBWay.Width   = 60;

            // lblTotal
            this.lblTotal.Location = new System.Drawing.Point(12, 274);
            this.lblTotal.Size     = new System.Drawing.Size(200, 20);
            this.lblTotal.Text     = "Total: 0 conta(s)";

            // grpInserir
            this.grpInserir.Location = new System.Drawing.Point(12, 300);
            this.grpInserir.Size     = new System.Drawing.Size(660, 90);
            this.grpInserir.Text     = "Inserir novo cliente (saldo inicial: 100€)";
            this.grpInserir.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblNovoBanco, this.txtNovoBanco,
                this.lblNovoCliente, this.txtNovoCliente,
                this.lblNovaConta, this.txtNovaConta,
                this.lblNovoPin, this.txtNovoPin,
                this.btnInserir });

            // campos dentro do grpInserir
            this.lblNovoBanco.Text     = "Banco:";
            this.lblNovoBanco.Location = new System.Drawing.Point(8, 30);
            this.lblNovoBanco.Size     = new System.Drawing.Size(45, 20);
            this.txtNovoBanco.Location = new System.Drawing.Point(55, 27);
            this.txtNovoBanco.Size     = new System.Drawing.Size(80, 23);

            this.lblNovoCliente.Text     = "Cliente:";
            this.lblNovoCliente.Location = new System.Drawing.Point(145, 30);
            this.lblNovoCliente.Size     = new System.Drawing.Size(50, 20);
            this.txtNovoCliente.Location = new System.Drawing.Point(198, 27);
            this.txtNovoCliente.Size     = new System.Drawing.Size(140, 23);

            this.lblNovaConta.Text     = "Conta:";
            this.lblNovaConta.Location = new System.Drawing.Point(348, 30);
            this.lblNovaConta.Size     = new System.Drawing.Size(45, 20);
            this.txtNovaConta.Location = new System.Drawing.Point(396, 27);
            this.txtNovaConta.Size     = new System.Drawing.Size(80, 23);

            this.lblNovoPin.Text     = "PIN:";
            this.lblNovoPin.Location = new System.Drawing.Point(486, 30);
            this.lblNovoPin.Size     = new System.Drawing.Size(30, 20);
            this.txtNovoPin.Location = new System.Drawing.Point(518, 27);
            this.txtNovoPin.Size     = new System.Drawing.Size(60, 23);
            this.txtNovoPin.PasswordChar = '*'; // ocultar o PIN

            this.btnInserir.Text     = "Inserir";
            this.btnInserir.Location = new System.Drawing.Point(588, 25);
            this.btnInserir.Size     = new System.Drawing.Size(65, 28);
            this.btnInserir.Click   += new System.EventHandler(this.btnInserir_Click);

            // btnEliminar
            this.btnEliminar.Text      = "Eliminar selecionado";
            this.btnEliminar.Location  = new System.Drawing.Point(12, 405);
            this.btnEliminar.Size      = new System.Drawing.Size(160, 30);
            this.btnEliminar.Click    += new System.EventHandler(this.btnEliminar_Click);

            // btnMBWay
            this.btnMBWay.Text      = "Ativar / Desativar MBWay";
            this.btnMBWay.Location  = new System.Drawing.Point(185, 405);
            this.btnMBWay.Size      = new System.Drawing.Size(180, 30);
            this.btnMBWay.Click    += new System.EventHandler(this.btnMBWay_Click);

            // btnDesbloquear
            this.btnDesbloquear.Text     = "Desbloquear conta";
            this.btnDesbloquear.Location = new System.Drawing.Point(378, 405);
            this.btnDesbloquear.Size     = new System.Drawing.Size(150, 30);
            this.btnDesbloquear.Click   += new System.EventHandler(this.btnDesbloquear_Click);

            // btnSair
            this.btnSair.Text     = "Sair";
            this.btnSair.Location = new System.Drawing.Point(587, 405);
            this.btnSair.Size     = new System.Drawing.Size(85, 30);
            this.btnSair.Click   += new System.EventHandler(this.btnSair_Click);

            // frmAdmin
            this.ClientSize = new System.Drawing.Size(686, 450);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lvClientes);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.grpInserir);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnMBWay);
            this.Controls.Add(this.btnDesbloquear);
            this.Controls.Add(this.btnSair);
            this.Name = "frmAdmin";
            this.Text = "Multibanco — BackOffice SIBS";
            this.Load += new System.EventHandler(this.frmAdmin_Load);
            this.grpInserir.ResumeLayout(false);
            this.grpInserir.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label     lblTitulo;
        private System.Windows.Forms.ListView  lvClientes;
        private System.Windows.Forms.ColumnHeader colId;
        private System.Windows.Forms.ColumnHeader colBanco;
        private System.Windows.Forms.ColumnHeader colCliente;
        private System.Windows.Forms.ColumnHeader colConta;
        private System.Windows.Forms.ColumnHeader colSaldo;
        private System.Windows.Forms.ColumnHeader colMBWay;
        private System.Windows.Forms.Label     lblTotal;
        private System.Windows.Forms.GroupBox  grpInserir;
        private System.Windows.Forms.Label     lblNovoBanco;
        private System.Windows.Forms.TextBox   txtNovoBanco;
        private System.Windows.Forms.Label     lblNovoCliente;
        private System.Windows.Forms.TextBox   txtNovoCliente;
        private System.Windows.Forms.Label     lblNovaConta;
        private System.Windows.Forms.TextBox   txtNovaConta;
        private System.Windows.Forms.Label     lblNovoPin;
        private System.Windows.Forms.TextBox   txtNovoPin;
        private System.Windows.Forms.Button    btnInserir;
        private System.Windows.Forms.Button    btnEliminar;
        private System.Windows.Forms.Button    btnMBWay;
        private System.Windows.Forms.Button    btnDesbloquear;
        private System.Windows.Forms.Button    btnSair;
    }
}
