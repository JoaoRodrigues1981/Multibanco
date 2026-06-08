using Multibanco._01Apresentacao; // para poder usar o frmUpdate
using Multibanco._02Controlo;    // para poder usar a classe cControlo

namespace Multibanco
{
    // Ecrã de login — é a primeira janela que aparece quando a aplicação arranca.
    // Responsabilidade: recolher as credenciais do utilizador e verificá-las.
    public partial class frmAutenticacao : Form
    {
            // Criar uma nova instância do controlo em cada operação para evitar estado residual
        // (se fosse reutilizada, operacao/existe ficavam a false após a primeira falha)

        // Construtor: corre quando o formulário é criado
        public frmAutenticacao()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        // Botão "Saída" — fecha toda a aplicação
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Evento de teclado no campo Banco — bloqueia qualquer tecla que não seja letra
        private void txtBanco_KeyPress(object sender, KeyPressEventArgs e)
        {
            // char.IsLetter → é uma letra (a-z, A-Z)?
            // char.IsControl → é uma tecla de controlo (Backspace, Tab, Enter)?
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // bloquear a tecla — não aparece no campo
                MessageBox.Show("Inseriu um número! O campo Banco somente admite letras", "Erro de Inserção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento que corre quando o formulário carrega (abre) — vazio por agora
        private void frmAutenticacao_Load(object sender, EventArgs e)
        {
        }

        // Evento de teclado no campo Cliente — bloqueia qualquer tecla que não seja letra
        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Inseriu um número! O campo Cliente somente admite letras", "Erro de Inserção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento KeyUp no campo Conta — vazio (reservado para uso futuro)
        private void txtConta_KeyUp(object sender, KeyEventArgs e)
        {
        }

        // Evento de teclado no campo Conta — bloqueia qualquer tecla que não seja dígito
        private void txtConta_KeyPress(object sender, KeyPressEventArgs e)
        {
            // char.IsDigit → é um número (0-9)?
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Inseriu uma letra! O campo Conta somente admite Números", "Erro de Inserção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Evento de teclado no campo PIN — bloqueia qualquer tecla que não seja dígito
        private void txtPin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Inseriu uma letra! O campo PIN somente admite Números", "Erro de Inserção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botão "OK" — tenta fazer login com os dados inseridos
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Nova instância a cada clique — garante estado limpo (operacao=true, existe=true)
            cControlo oCtrl = new cControlo();

            // Passo 1: detetar perfil — sibs ou cliente normal — antes de validar campos
            if (txtCliente.Text.ToLower() == "sibs")
            {
                // Admin: validar só Cliente e PIN (Banco e Conta não são usados)
                oCtrl.fTestarCamposAdmin(txtCliente.Text, txtPin.Text);

                if (!oCtrl.operacao)
                    MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    oCtrl.fValidarAdmin(txtCliente.Text, txtPin.Text);

                    if (!oCtrl.operacao || !oCtrl.existe)
                        MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        // Admin autenticado → abrir o BackOffice
                        this.Hide();
                        new frmAdmin().ShowDialog();
                        this.Close();
                    }
                }
            }
            else
            {
                // Cliente normal: validar os 4 campos
                oCtrl.fTestarCampos(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);

                if (!oCtrl.operacao)
                    MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    // Verificar as credenciais na tabela Credenciais
                    oCtrl.fValidarCredenciais(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);

                    if (!oCtrl.operacao || !oCtrl.existe)
                        MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        // Login com sucesso → passar todos os dados da conta ao ecrã principal
                        this.Hide();
                        new frmMultibanco(oCtrl.contaId, oCtrl.saldo, oCtrl.banco, oCtrl.cliente, oCtrl.conta).ShowDialog();
                        this.Close();
                    }
                }
            }
        }

        // Botão "Alterar Pin" — valida as credenciais e abre o formulário de alteração de PIN
        private void btnAltPin_Click(object sender, EventArgs e)
        {
            // Nova instância a cada clique — garante estado limpo (operacao=true, existe=true)
            cControlo oCtrl = new cControlo();

            // Passo 1: verificar se todos os campos estão preenchidos
            oCtrl.fTestarCampos(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);

            if (!oCtrl.operacao)
                MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                // Passo 2: confirmar que as credenciais existem na BD antes de permitir alterar
                oCtrl.fValidarCredenciais(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);

                if (oCtrl.operacao && oCtrl.existe)
                {
                    // Credenciais válidas → abrir o ecrã de alteração de PIN
                    // Passa os dados do utilizador para o frmUpdate pré-preencher os campos
                    frmUpdate oUpd = new frmUpdate(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);
                    oUpd.ShowDialog(); // modal: espera que o utilizador feche antes de continuar
                }
                else
                    MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
