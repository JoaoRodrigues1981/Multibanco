using Multibanco._02Controlo;

namespace Multibanco
{
    // Ecrã principal do multibanco — aparece após login bem-sucedido.
    // Recebe os dados da conta via construtor (passados pelo frmAutenticacao após login).
    public partial class frmMultibanco : Form
    {
        // Campos privados — encapsulamento: os dados da sessão não são expostos diretamente
        private int     _contaId;   // Id da linha em Credenciais — usado para operações e movimentos
        private decimal _saldo;     // Saldo atual — atualizado após cada operação
        private string  _banco;     // Banco do cliente
        private string  _cliente;   // Nome do cliente
        private int     _conta;     // Número de conta

        // Construtor: recebe os dados da conta do frmAutenticacao após login bem-sucedido
        public frmMultibanco(int contaId, decimal saldo, string banco, string cliente, int conta)
        {
            InitializeComponent();
            Tema.Aplicar(this);

            _contaId = contaId;
            _saldo   = saldo;
            _banco   = banco;
            _cliente = cliente;
            _conta   = conta;
        }

        // Evento que corre quando o formulário abre
        private void frmMultibanco_Load(object sender, EventArgs e)
        {
            // Colunas aqui e não no Designer, porque estava com dificulades uma vez que o Designer apaga-as ao guardar
            lstMovimentos.Columns.Add("Tipo",        55);
            lstMovimentos.Columns.Add("Valor €",     78);
            lstMovimentos.Columns.Add("Saldo Após",  88);
            lstMovimentos.Columns.Add("Data/Hora",  128);
            lstMovimentos.Columns.Add("Descrição",  125);

            lblBanco.Text   = _banco;
            lblCliente.Text = _cliente + " | Conta: " + _conta;
            txtSaldo.Text   = _saldo.ToString("F2") + " €";

            dtpInicio.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpFim.Value    = DateTime.Now;

            cControlo oCtrl = new cControlo(); // Vamos controlar se o botão MBWay esta enable ou disable
            oCtrl.fVerificarMBWay(_contaId);
            if (!oCtrl.mbwayAtivo)
                btnMBWay.Enabled = false;

            fCarregarMovimentos();
        }

        // Carrega movimentos com filtro de datas opcional.
        // Sem argumentos mostra todos; com datas aplica o filtro.
        private void fCarregarMovimentos(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            cControlo oCtrl = new cControlo();
            oCtrl.fListarMovimentos(_contaId, dataInicio, dataFim);

            lstMovimentos.Items.Clear();

            foreach (string[] linha in oCtrl.listaMovimentos)
            {
                ListViewItem item = new ListViewItem(fTraduzirTipo(linha[0]));
                item.SubItems.Add(linha[1]);
                item.SubItems.Add(linha[2]);
                item.SubItems.Add(linha[3]);
                item.SubItems.Add(linha[4]);
                lstMovimentos.Items.Add(item);
            }

            // Ajusta a largura da coluna "Descrição" ao conteúdo, porque me cortava e nao tinha como ler. Assim coloca o scroll
            lstMovimentos.AutoResizeColumn(4, ColumnHeaderAutoResizeStyle.ColumnContent); 
        }

        // Converte o código de tipo ('D','L','T','P','M') para texto legível
        private string fTraduzirTipo(string tipo)
        {
            switch (tipo)
            {
                case "D": return "Depósito";
                case "L": return "Levantamento";
                case "T": return "Transferência";
                case "P": return "Pagamento";
                case "M": return "MBWay";
                default:  return tipo;
            }
        }

        // Método auxiliar — atualiza o saldo no ecrã e recarrega os movimentos após operação
        private void fRefrescarEcra(decimal novoSaldo)
        {
            _saldo        = novoSaldo;
            txtSaldo.Text = _saldo.ToString("F2") + " €";
            fCarregarMovimentos();
            txtValor.Text          = ""; // limpar o campo de valor após operação
            txtContaDestino.Text   = ""; // limpar conta destino
        }

        // Botão "Levantar"
        private void btnLevantar_Click(object sender, EventArgs e)
        {
            if (!fValidarValor(out decimal valor)) return;

            cControlo oCtrl = new cControlo();
            oCtrl.fLevantar(_contaId, valor);
            fMostrarResultado(oCtrl);
        }

        // Botão "Depositar"
        private void btnDepositar_Click(object sender, EventArgs e)
        {
            if (!fValidarValor(out decimal valor)) return;

            cControlo oCtrl = new cControlo();
            oCtrl.fDepositar(_contaId, valor);
            fMostrarResultado(oCtrl);
        }

        // Botão "Transferir"
        private void btnTransferir_Click(object sender, EventArgs e)
        {
            if (!fValidarValor(out decimal valor))      return;
            if (!fValidarContaDestino(out int destino)) return;

            cControlo oCtrl = new cControlo();
            oCtrl.fTransferir(_contaId, destino, valor);
            fMostrarResultado(oCtrl);
        }

        // Botão "MBWay" — abre a janela de envio MBWay com a lista de destinatários disponíveis
        private void btnMBWay_Click(object sender, EventArgs e)
        {
            frmMBWay frm = new frmMBWay(_contaId);
            if (frm.ShowDialog() == DialogResult.OK)
                fRefrescarEcra(frm.NovoSaldo);
        }

        // Mostra o resultado de uma operação bancária e atualiza o ecrã se foi bem-sucedida.
        // Reutilizado em btnLevantar_Click, btnDepositar_Click, btnTransferir_Click e btnMBWay_Click
        // para evitar o bloco if/else com MessageBox duplicado em cada handler.
        private void fMostrarResultado(cControlo oCtrl)
        {
            if (!oCtrl.operacao)
                MessageBox.Show(oCtrl.mensagem, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                MessageBox.Show(oCtrl.mensagem, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                fRefrescarEcra(oCtrl.saldoAtualizado);
            }
        }

        // Valida o campo txtValor — tem de ser um número positivo
        private bool fValidarValor(out decimal valor)
        {
            if (!decimal.TryParse(txtValor.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out valor) || valor <= 0)
            {
                MessageBox.Show("Introduza um valor numérico válido e maior que zero.", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        // Valida o campo txtContaDestino — tem de ser um número inteiro
        private bool fValidarContaDestino(out int contaDestino)
        {
            if (!int.TryParse(txtContaDestino.Text, out contaDestino) || contaDestino <= 0)
            {
                MessageBox.Show("Introduza um número de conta destino válido.", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        // Botão "Filtrar" — aplica o filtro de datas ao histórico
        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            if (dtpInicio.Value.Date > dtpFim.Value.Date)
            {
                MessageBox.Show("A data de início não pode ser posterior à data de fim.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            fCarregarMovimentos(dtpInicio.Value.Date, dtpFim.Value.Date);

            if (lstMovimentos.Items.Count == 0)
                MessageBox.Show("Nenhum movimento encontrado no período selecionado.", "Filtro",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Botão "Todos" — remove o filtro e mostra todos os movimentos
        private void btnTodos_Click(object sender, EventArgs e)
        {
            fCarregarMovimentos();
        }

        // Botão "Simulador" — abre simulador de empréstimo (extra)
        private void btnSimulador_Click(object sender, EventArgs e)
        {
            new frmSimuladorEmprestimo().ShowDialog();
        }

        // Botão "Pagar Serviços" — abre lista de serviços pré-definidos
        private void btnPagamentos_Click(object sender, EventArgs e)
        {
            frmPagamentosServicos frm = new frmPagamentosServicos(_contaId);
            if (frm.ShowDialog() == DialogResult.OK)
                fRefrescarEcra(frm.NovoSaldo);
        }

        // Tick do relógio — atualiza o label a cada segundo
        private void tmrRelogio_Tick(object sender, EventArgs e)
        {
            lblRelogio.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        // Botão "Sair" — fecha toda a aplicação
        private void btnSair_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
