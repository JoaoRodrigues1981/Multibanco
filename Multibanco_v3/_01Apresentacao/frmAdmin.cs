using Multibanco._02Controlo; // para poder usar cControlo

namespace Multibanco._01Apresentacao
{
    // Ecrã de BackOffice — exclusivo do administrador "sibs".
    // Permite listar, inserir e eliminar clientes e contas.
    public partial class frmAdmin : Form
    {
        // Construtor: corre quando o formulário é criado
        public frmAdmin()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        // Quando o formulário abre, carregar imediatamente a lista de clientes
        private void frmAdmin_Load(object sender, EventArgs e)
        {
            fCarregarLista();
        }

        // Método auxiliar — carrega a lista de clientes e preenche o ListView.
        // Chamado no Load e após cada inserção ou eliminação.
        private void fCarregarLista()
        {
            cControlo oCtrl = new cControlo();
            oCtrl.fListarCredenciais();

            lvClientes.Items.Clear(); // limpar o ListView antes de preencher

            if (!oCtrl.operacao)
            {
                MessageBox.Show(oCtrl.mensagem, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Preencher o ListView com cada linha da lista devolvida pelo cControlo
            foreach (string[] linha in oCtrl.listaCredenciais)
            {
                ListViewItem item = new ListViewItem(linha[0]); // coluna 0: Id
                item.SubItems.Add(linha[1]); // Banco
                item.SubItems.Add(linha[2]); // Cliente
                item.SubItems.Add(linha[3]); // Conta
                item.SubItems.Add(linha[4]); // Saldo
                item.SubItems.Add(linha[5]); // MBWay (Sim/Não)
                lvClientes.Items.Add(item);
            }

            lblTotal.Text = "Total: " + oCtrl.listaCredenciais.Count + " conta(s)";
        }

        // Botão "Inserir" — cria um novo cliente com saldo inicial de 100€
        private void btnInserir_Click(object sender, EventArgs e)
        {
            cControlo oCtrl = new cControlo();

            // Validar se os campos de inserção estão todos preenchidos
            oCtrl.fTestarCampos(txtNovoBanco.Text, txtNovoCliente.Text, txtNovaConta.Text, txtNovoPin.Text);

            if (!oCtrl.operacao)
            {
                MessageBox.Show(oCtrl.mensagem, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Inserir o novo cliente — saldo inicial de 100€ é aplicado automaticamente
            oCtrl.fInserirCliente(txtNovoBanco.Text, txtNovoCliente.Text, txtNovaConta.Text, txtNovoPin.Text);

            if (oCtrl.operacao)
            {
                // Limpar os campos após inserção bem-sucedida
                txtNovoBanco.Text   = "";
                txtNovoCliente.Text = "";
                txtNovaConta.Text   = "";
                txtNovoPin.Text     = "";
            }

            fMostrarResultadoEAtualizar(oCtrl);
        }

        // Botão "Eliminar" — elimina o cliente selecionado no ListView
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Verificar se há uma linha selecionada
            if (lvClientes.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecione um cliente na lista para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obter o Id da linha selecionada (coluna 0 do ListView)
            int id = Convert.ToInt32(lvClientes.SelectedItems[0].Text);
            string nomeCliente = lvClientes.SelectedItems[0].SubItems[2].Text; // coluna 2: Cliente

            // Pedir confirmação antes de eliminar
            DialogResult confirmar = MessageBox.Show(
                "Tem a certeza que quer eliminar o cliente \"" + nomeCliente + "\"?",
                "Confirmar eliminação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            cControlo oCtrl = new cControlo();
            oCtrl.fEliminarCliente(id);

            fMostrarResultadoEAtualizar(oCtrl);
        }

        // Botão "Ativar / Desativar MBWay" — inverte o estado MBWay da conta selecionada
        // Funcionalidade extra — o sibs gere a adesão ao MBWay por conta
        private void btnMBWay_Click(object sender, EventArgs e)
        {
            if (lvClientes.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecione um cliente na lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int    id           = Convert.ToInt32(lvClientes.SelectedItems[0].Text);           // coluna 0: Id
            string nomeCliente  = lvClientes.SelectedItems[0].SubItems[2].Text;                // coluna 2: Cliente
            bool   estadoAtual  = lvClientes.SelectedItems[0].SubItems[5].Text == "Sim";       // coluna 5: MBWay
            bool   novoEstado   = !estadoAtual; // inverter o estado atual

            string acao = novoEstado ? "ativar" : "desativar";

            DialogResult confirmar = MessageBox.Show(
                "Deseja " + acao + " o MBWay para \"" + nomeCliente + "\"?",
                "Confirmar MBWay",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmar != DialogResult.Yes)
                return;

            cControlo oCtrl = new cControlo();
            oCtrl.fAlternarMBWay(id, novoEstado);

            fMostrarResultadoEAtualizar(oCtrl); // atualiza a lista para refletir o novo estado
        }

        // Mostra o resultado de uma operação e atualiza a lista se foi bem-sucedida.
        // Reutilizado em btnInserir_Click, btnEliminar_Click e btnMBWay_Click
        // para evitar o bloco if/else com MessageBox duplicado em cada handler.
        private void fMostrarResultadoEAtualizar(cControlo oCtrl)
        {
            if (!oCtrl.operacao)
                MessageBox.Show(oCtrl.mensagem, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                MessageBox.Show(oCtrl.mensagem, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fCarregarLista();
            }
        }

        // Botão "Desbloquear" — remove o bloqueio da conta selecionada no ListView
        private void btnDesbloquear_Click(object sender, EventArgs e)
        {
            if (lvClientes.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecione um cliente na lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(lvClientes.SelectedItems[0].Text); // coluna 0: Id
            string nomeCliente = lvClientes.SelectedItems[0].SubItems[2].Text; // coluna 2: Cliente

            cControlo oCtrl = new cControlo();
            oCtrl.fDesbloquearConta(id); // método a implementar no cControlo para remover o bloqueio da conta

            fMostrarResultadoEAtualizar(oCtrl); // atualiza a lista para refletir o novo estado (desbloqueado) da conta
        }


        // Botão "Sair" — fecha o BackOffice e regressa ao ecrã de login
        private void btnSair_Click(object sender, EventArgs e) // handler do clique no botão "Sair"
        {
            this.Close();
        }
    }
}
