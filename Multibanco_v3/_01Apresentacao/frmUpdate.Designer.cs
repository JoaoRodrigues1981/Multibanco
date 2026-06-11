namespace Multibanco._01Apresentacao
{
    partial class frmUpdate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtNovoPin = new TextBox();
            txtConta = new TextBox();
            txtCliente = new TextBox();
            txtBanco = new TextBox();
            label1 = new Label();
            txtConfPin = new TextBox();
            btnOK = new Button();
            btnExit = new Button();
            SuspendLayout();
            //
            // txtNovoPin
            //
            txtNovoPin.Font = new Font("Segoe UI", 14F);
            txtNovoPin.Location = new Point(41, 305);
            txtNovoPin.Name = "txtNovoPin";
            txtNovoPin.PasswordChar = '@';
            txtNovoPin.PlaceholderText = "novo pin...";
            txtNovoPin.Size = new Size(396, 39);
            txtNovoPin.TabIndex = 12;
            //
            // txtConta
            //
            txtConta.Enabled = false;
            txtConta.Font = new Font("Segoe UI", 14F);
            txtConta.Location = new Point(41, 224);
            txtConta.Name = "txtConta";
            txtConta.PlaceholderText = "conta...";
            txtConta.Size = new Size(396, 39);
            txtConta.TabIndex = 11;
            //
            // txtCliente
            //
            txtCliente.Enabled = false;
            txtCliente.Font = new Font("Segoe UI", 14F);
            txtCliente.Location = new Point(41, 179);
            txtCliente.Name = "txtCliente";
            txtCliente.PlaceholderText = "cliente...";
            txtCliente.Size = new Size(396, 39);
            txtCliente.TabIndex = 10;
            //
            // txtBanco
            //
            txtBanco.Enabled = false;
            txtBanco.Font = new Font("Segoe UI", 14F);
            txtBanco.Location = new Point(41, 134);
            txtBanco.Name = "txtBanco";
            txtBanco.PlaceholderText = "banco....";
            txtBanco.Size = new Size(396, 39);
            txtBanco.TabIndex = 9;
            //
            // label1
            //
            label1.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            label1.ForeColor = Color.DarkRed;
            label1.Location = new Point(48, 41);
            label1.Name = "label1";
            label1.Size = new Size(389, 60);
            label1.TabIndex = 8;
            label1.Text = "MULTIBANCO";
            //
            // txtConfPin
            //
            txtConfPin.Font = new Font("Segoe UI", 14F);
            txtConfPin.Location = new Point(41, 350);
            txtConfPin.Name = "txtConfPin";
            txtConfPin.PasswordChar = '@';
            txtConfPin.PlaceholderText = "confirmar pin...";
            txtConfPin.Size = new Size(396, 39);
            txtConfPin.TabIndex = 13;
            //
            // btnOK
            //
            btnOK.Location = new Point(318, 432);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(114, 62);
            btnOK.TabIndex = 15;
            btnOK.Text = "Ok";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            //
            // btnExit
            //
            btnExit.Location = new Point(48, 432);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(114, 62);
            btnExit.TabIndex = 14;
            btnExit.Text = "Saída";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            //
            // frmUpdate
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(518, 530);
            Controls.Add(btnOK);
            Controls.Add(btnExit);
            Controls.Add(txtConfPin);
            Controls.Add(txtNovoPin);
            Controls.Add(txtConta);
            Controls.Add(txtCliente);
            Controls.Add(txtBanco);
            Controls.Add(label1);
            Name = "frmUpdate";
            Text = "Alterar PIN";
            Load += frmUpdate_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtNovoPin;
        private TextBox txtConta;
        private TextBox txtCliente;
        private TextBox txtBanco;
        private Label label1;
        private TextBox txtConfPin;
        private Button btnOK;
        private Button btnExit;
    }
}
