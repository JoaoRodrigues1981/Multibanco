using Multibanco._01Apresentacao; // para poder usar o frmUpdate
using Multibanco._02Controlo;    // para poder usar a classe cControlo

namespace Multibanco
{
    // ============================================================
    // CAMADA 1 — APRESENTAÇÃO
    // frmAutenticacao é o ecrã de login — a primeira janela que aparece quando a aplicação arranca.
    // Responsabilidade desta camada:
    //   • Recolher os dados introduzidos pelo utilizador
    //   • Validar o formato dos campos (letras vs números) em tempo real
    //   • Chamar a camada de Controlo (cControlo) para validar as credenciais
    //   • Nunca acede diretamente à base de dados
    // ============================================================
    public partial class frmAutenticacao : Form
    {
        // Construtor: corre quando o formulário é criado
        public frmAutenticacao()
        {
            InitializeComponent();
            Tema.Aplicar(this); // aplica o tema visual (cores, fontes)
        }

        // BOTÃO "SAIR" — fecha toda a aplicação
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        // ============================================================
        // VALIDAÇÃO DE CAMPOS DIRETAMENTE FEITO PELO FORMULÁRIO
        // estas validaçõe são feitas diretamente no formulário, sem recorrer à camada de Controlo
        // ============================================================

        // Campo banco - Só aceita letras e espaços (ex: "BPI")
        private void txtBanco_KeyPress(object sender, KeyPressEventArgs e) //VALIDAÇÃO EM TEMPO REAL — eventos KeyPress
        {
            //   char.IsLetter(c)  → é uma letra (a-z, A-Z, incluindo acentos)?
            //   char.IsControl(c) → é uma tecla de controlo (Backspace, Enter, Tab)?
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true; // O evento KeyPress dispara ANTES de a tecla aparecer no campo. Se definirmos e.Handled = true,
                MessageBox.Show("Inseriu um número! O campo Banco somente admite letras",
                                "Erro de Inserção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Campo Cliente — só aceita letras e espaços (ex: "João Silva")
        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
                MessageBox.Show("Inseriu um número! O campo Cliente somente admite letras",
                                "Erro de Inserção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Campo Conta — só aceita dígitos (ex: "123456") - Não aceita letras, vírgulas, pontos nem espaços — só números inteiros
        private void txtConta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Inseriu uma letra! O campo Conta somente admite Números",
                                "Erro de Inserção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Campo PIN — só aceita dígitos (ex: "1234") - Igual ao campo Conta — o PIN é um número inteiro sem espaços
        private void txtPin_KeyPress(object sender, KeyPressEventArgs e)
        {
            //   char.IsDigit(c)   → é um algarismo (0-9)?
            //   char.IsControl(c) → é uma tecla de controlo (Backspace, Enter, Tab)?
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Inseriu uma letra! O campo PIN somente admite Números",
                                "Erro de Inserção!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================================
        // BOTÃO OK — fluxo de autenticação
        //
        // Criamos uma nova instância de cControlo a cada clique para garantir estado limpo (operacao=true, existe=true).
        // Se reutilizássemos a mesma instância, uma falha anterior ficaria gravada e impediria logins seguintes.
        //
        // O campo txtCliente distingue os dois perfis:
        //   1. "sibs"         - administrador da plataforma (BackOffice)
        //   2. qualquer outro - cliente normal (ecrã principal Multibanco)
        // 
        // Este botão precisa de invocar a camada de controlo para validar as credenciais do utilizador.
        // Receberá da camada de controlo a informação de poder passar ou não ao form1 (formulario central do multibanco)
        // ============================================================
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Nova instância a cada clique — garante estado limpo (operacao=true, existe=true)
            cControlo oCtrl = new cControlo();

            if (txtCliente.Text.ToLower() == "sibs")
            {
                // ── Ramo ADMIN ──────────────────────────────────────────
                // O admin só precisa de preencher Cliente e PIN.
                // Banco e Conta não fazem sentido para o administrador.

                // Passo 1: verificar se Cliente e PIN estão preenchidos
                oCtrl.fTestarCamposAdmin(txtCliente.Text, txtPin.Text);

                if (!oCtrl.operacao)
                {
                    MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Passo 2: verificar as credenciais do admin na tabela Admins (BD)
                    oCtrl.fValidarAdmin(txtCliente.Text, txtPin.Text);

                    if (!oCtrl.operacao || !oCtrl.existe)
                        MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        // Admin autenticado → abrir o BackOffice e fechar o ecrã de login
                        this.Hide();
                        new frmAdmin().ShowDialog(); // ShowDialog bloqueia até o admin fechar o BackOffice
                        this.Close();
                    }
                }
            }
            else
            {
                // ── Ramo CLIENTE ─────────────────────────────────────────
                // O cliente preenche 4 campos: Banco, Cliente, Conta, PIN.

                // Passo 1: verificar se todos os campos estão preenchidos (sem ir à BD)
                oCtrl.fTestarCampos(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);

                if (!oCtrl.operacao)
                {
                    MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Passo 2: verificar as 4 credenciais na tabela Credenciais (BD)
                    // Internamente: verifica bloqueio → valida credenciais → atualiza tentativas
                    oCtrl.fValidarCredenciais(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);

                    // operacao=false -> erro técnico (BD offline) ou conta bloqueada
                    // existe=false   -> credenciais não encontradas
                    // Na prática andam sempre juntos, mas verificar os dois torna a intenção explícita
                    if (!oCtrl.operacao || !oCtrl.existe)
                    {
                        MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // Login com sucesso -> os dados já estão em oCtrl (contaId, saldo, banco, cliente, conta)
                        // Passamos tudo pelo construtor: frmMultibanco não pode existir sem sessão válida
                        // Hide() oculta este ecrã; ShowDialog() abre o Multibanco e bloqueia até ele fechar; Close() fecha o login
                        this.Hide();
                        new frmMultibanco(oCtrl.contaId, oCtrl.saldo, oCtrl.banco, oCtrl.cliente, oCtrl.conta).ShowDialog();
                        this.Close();
                    }
                }
            }
        }

        // ============================================================
        // BOTÃO ALTERAR PIN
        //
        // Antes de abrir o formulário de alteração, valida as credenciais
        // atuais — o utilizador tem de provar que sabe o PIN atual.
        // Se as credenciais forem inválidas, o frmUpdate não abre.
        // ============================================================
        private void btnAltPin_Click(object sender, EventArgs e)
        {
            cControlo oCtrl = new cControlo();

            // Passo 1: verificar se todos os campos estão preenchidos
            oCtrl.fTestarCampos(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);

            if (!oCtrl.operacao)
            {
                MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Passo 2: confirmar que as credenciais existem na BD antes de permitir alterar
                oCtrl.fValidarCredenciais(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);

                if (oCtrl.operacao && oCtrl.existe)
                {
                    // Credenciais válidas → abrir o ecrã de alteração de PIN.
                    // Passa os 4 campos para o frmUpdate pré-identificar o utilizador.
                    frmUpdate oUpd = new frmUpdate(txtBanco.Text, txtCliente.Text, txtConta.Text, txtPin.Text);
                    oUpd.ShowDialog(); // modal: espera que o utilizador feche antes de continuar
                }
                else
                {
                    MessageBox.Show(oCtrl.mensagem, "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
