namespace Multibanco
{
    partial class frmAutenticacao
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
            label1 = new Label();
            txtBanco = new TextBox();
            txtCliente = new TextBox();
            txtConta = new TextBox();
            txtPin = new TextBox();
            btnExit = new Button();
            btnOK = new Button();
            btnAltPin = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            label1.ForeColor = Color.DarkRed;
            label1.Location = new Point(158, 33);
            label1.Name = "label1";
            label1.Size = new Size(389, 60);
            label1.TabIndex = 3;
            label1.Text = "MULTIBANCO";
            // 
            // txtBanco
            // 
            txtBanco.Font = new Font("Segoe UI", 14F);
            txtBanco.Location = new Point(151, 126);
            txtBanco.Name = "txtBanco";
            txtBanco.PlaceholderText = "banco....";
            txtBanco.Size = new Size(396, 39);
            txtBanco.TabIndex = 4;
            txtBanco.KeyPress += txtBanco_KeyPress;
            // 
            // txtCliente
            // 
            txtCliente.Font = new Font("Segoe UI", 14F);
            txtCliente.Location = new Point(151, 171);
            txtCliente.Name = "txtCliente";
            txtCliente.PlaceholderText = "cliente...";
            txtCliente.Size = new Size(396, 39);
            txtCliente.TabIndex = 5;
            txtCliente.KeyPress += txtCliente_KeyPress;
            // 
            // txtConta
            // 
            txtConta.Font = new Font("Segoe UI", 14F);
            txtConta.Location = new Point(151, 216);
            txtConta.Name = "txtConta";
            txtConta.PlaceholderText = "conta...";
            txtConta.Size = new Size(396, 39);
            txtConta.TabIndex = 6;
            txtConta.KeyPress += txtConta_KeyPress;
            // 
            // txtPin
            // 
            txtPin.Font = new Font("Segoe UI", 14F);
            txtPin.Location = new Point(151, 261);
            txtPin.Name = "txtPin";
            txtPin.PasswordChar = '@';
            txtPin.PlaceholderText = "pin...";
            txtPin.Size = new Size(396, 39);
            txtPin.TabIndex = 7;
            txtPin.KeyPress += txtPin_KeyPress;
            // 
            // btnExit
            //
            btnExit.Location = new Point(80, 355);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(150, 50);
            btnExit.TabIndex = 8;
            btnExit.Text = "Saída";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            //
            // btnAltPin  — centro, mesma linha
            //
            btnAltPin.Location = new Point(272, 355);
            btnAltPin.Name = "btnAltPin";
            btnAltPin.Size = new Size(155, 50);
            btnAltPin.TabIndex = 10;
            btnAltPin.Text = "Alterar PIN";
            btnAltPin.UseVisualStyleBackColor = true;
            btnAltPin.Click += btnAltPin_Click;
            //
            // btnOK
            //
            btnOK.Location = new Point(469, 355);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(150, 50);
            btnOK.TabIndex = 9;
            btnOK.Text = "Entrar";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // frmAutenticacao
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(699, 450);
            Controls.Add(btnAltPin);
            Controls.Add(btnOK);
            Controls.Add(btnExit);
            Controls.Add(txtPin);
            Controls.Add(txtConta);
            Controls.Add(txtCliente);
            Controls.Add(txtBanco);
            Controls.Add(label1);
            Name = "frmAutenticacao";
            Text = "v";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtBanco;
        private TextBox txtCliente;
        private TextBox txtConta;
        private TextBox txtPin;
        private Button btnExit;
        private Button btnOK;
        private Button btnAltPin;
    }
}