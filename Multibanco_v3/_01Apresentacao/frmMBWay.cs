using System;
using System.Collections.Generic;
using Multibanco._02Controlo;

namespace Multibanco
{
    // Janela de envio MBWay — o utilizador escolhe o destinatário da lista
    // (só aparecem contas com MBWay ativo, excluindo a própria conta),
    // introduz o valor e confirma. Máximo 300€ por transação.
    public partial class frmMBWay : Form
    {
        private int _contaId;
        public decimal NovoSaldo { get; private set; }

        // Lista carregada da BD: cada entrada é [Cliente, Telefone, Conta]
        private List<string[]> _contas = new List<string[]>();

        public frmMBWay(int contaId)
        {
            InitializeComponent();
            Tema.Aplicar(this);
            _contaId = contaId;

            // Carregar contas MBWay ativas da BD, excluindo a própria conta
            cControlo oCtrl = new cControlo();
            oCtrl.fListarContasMBWay(_contaId);

            if (!oCtrl.operacao)
            {
                MessageBox.Show("Erro ao carregar contactos MBWay: " + oCtrl.mensagem, "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnEnviar.Enabled = false;
                return;
            }

            _contas = oCtrl.listaContasMBWay;

            // Preencher a listbox com "Cliente — Telefone" (Conta fica interno em _contas)
            foreach (string[] c in _contas)
                lstContas.Items.Add(c[0] + " — " + c[1]);

            if (lstContas.Items.Count > 0)
                lstContas.SelectedIndex = 0;
            else
            {
                // Nenhuma conta disponível — desativar envio
                btnEnviar.Enabled = false;
                MessageBox.Show("Não existem contas aderentes ao MBWay disponíveis.", "MBWay",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Botão "Enviar" — valida o valor e executa o MBWay
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            if (lstContas.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione um destinatário.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validação de formato — o campo tem de ser um número parseável. 
            // NOTA: as Regras de negócio (valor > 0, máximo 300€) ficam no cControlo.fMBWay,
            // são regras da operação, não do formulário, e assim aplicam-se independentemente de quem chamar fMBWay.
            if (!decimal.TryParse(txtValor.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal valor))
            {
                MessageBox.Show("Introduza um valor numérico válido.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Obter o número de conta destino (índice 2 da entrada selecionada)
            int contaDestino = int.Parse(_contas[lstContas.SelectedIndex][2]);

            cControlo oCtrl = new cControlo();
            oCtrl.fMBWay(_contaId, contaDestino, valor);

            if (!oCtrl.operacao)
            {
                MessageBox.Show(oCtrl.mensagem, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                NovoSaldo = oCtrl.saldoAtualizado;
                MessageBox.Show(oCtrl.mensagem, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        // Botão "Cancelar" — fecha sem efetuar envio
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
