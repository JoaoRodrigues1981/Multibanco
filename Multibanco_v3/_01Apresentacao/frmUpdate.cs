using Multibanco._02Controlo; // para poder usar a classe cControlo

namespace Multibanco._01Apresentacao
{
    // Ecrã de alteração de PIN.
    // Recebe os dados do utilizador do frmAutenticacao (já validados),
    // pede o novo PIN duas vezes e grava na base de dados.
    public partial class frmUpdate : Form
    {
        // Guardar os dados do utilizador recebidos do ecrã de login
        private string txt_Banco   = "";
        private string txt_Cliente = "";
        private string txt_Conta   = "";
        private string txt_Pin     = "";
        private string oldPin      = "";   // PIN original, para verificação no UPDATE da BD

        // Getters e setters — encapsulamento: os campos privados só são acedidos por estes métodos. (FORMA de proteger dados)
        //
        // O frmUpdate recebe 4 dados do frmAutenticacao (Banco, Cliente, Conta, PIN). Esses dados não devem ser alterados "fora"
        // São a identidade do utilizador já validada.
        //
        // Em C# o equivalente seriam propriedades (get/set), mas optou-se pelo padrão
        // explícito get_X / set_X para tornar a leitura mais clara para quem aprende.
        // get_X() - permite ler o valor de X
        // set_X() - permite alterar o valor de X, de forma controlada
        public string get_Banco()    { return txt_Banco; }
        public string get_Cliente()  { return txt_Cliente; }
        public string get_Conta()    { return txt_Conta; }
        public string get_Pin()      { return txt_Pin; }
        public string get_oldPin()   { return oldPin; }

        public void set_Banco(string ban)    { txt_Banco   = ban; }
        public void set_Cliente(string cli)  { txt_Cliente = cli; }
        public void set_Conta(string cont)   { txt_Conta   = cont; }
        public void set_Pin(string pin)      { txt_Pin     = pin; }
        public void set_oldPin(string pin)   { oldPin      = pin; }

        // Construtor: recebe os dados do utilizador vindos do frmAutenticacao
        public frmUpdate(string txtBanco, string txtCliente, string txtConta, string txtPin)
        {
            InitializeComponent();
            Tema.Aplicar(this);

            set_Banco(txtBanco);
            set_Cliente(txtCliente);
            set_Conta(txtConta);
            set_Pin(txtPin);
            set_oldPin(txtPin); // guardar o PIN antigo para o WHERE do UPDATE
        }

        // Quando o formulário abre, preenche os campos com os dados do utilizador (somente leitura)
        private void frmUpdate_Load(object sender, EventArgs e)
        {
            txtBanco.Text    = get_Banco();
            txtCliente.Text  = get_Cliente();
            txtConta.Text    = get_Conta();
            txtNovoPin.Text  = ""; // campo novo PIN começa vazio
        }

        // BOTÃO "SAIR" — fecha este ecrã sem gravar nada
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        // BOTÃO "OK" — valida os PINs e grava o novo PIN na base de dados
        private void btnOK_Click(object sender, EventArgs e)
        {
            cControlo oCtrl = new cControlo();

            // Passo 1: verificar se os dois campos de PIN são iguais
            oCtrl.fValidarPinsIguais(txtNovoPin.Text, txtConfPin.Text);

            if (!oCtrl.operacao)
                MessageBox.Show(oCtrl.mensagem, "ERRO - PINs diferentes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                // Passo 2: gravar o novo PIN na base de dados
                // O oldPin serve como verificação extra no WHERE do UPDATE
                oCtrl.fAtualizarPin(get_Banco(), get_Cliente(), get_Conta(), get_oldPin(), txtConfPin.Text);

                if (!oCtrl.operacao)
                    MessageBox.Show(oCtrl.mensagem, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    // PIN alterado com sucesso → mostrar confirmação e fechar o ecrã
                    MessageBox.Show(oCtrl.mensagem, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
        }
    }
}
