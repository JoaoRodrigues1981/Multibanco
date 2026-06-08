using System.Collections.Generic;
using Multibanco._02Controlo;

namespace Multibanco
{
    // Pagamentos de serviços pré-definidos — o utilizador escolhe da lista,
    // introduz a referência e o valor. A entidade é preenchida automaticamente.
    public partial class frmPagamentosServicos : Form
    {
        private int _contaId;
        public decimal NovoSaldo { get; private set; }

        // Lista carregada da BD: cada entrada é [Id, Nome, Entidade]
        private List<string[]> _servicos = new List<string[]>();

        public frmPagamentosServicos(int contaId)
        {
            InitializeComponent();
            Tema.Aplicar(this);
            _contaId = contaId;

            // Carregar serviços da BD via cControlo
            cControlo oCtrl = new cControlo();
            oCtrl.fListarServicos();

            if (!oCtrl.operacao)
            {
                MessageBox.Show("Erro ao carregar serviços: " + oCtrl.mensagem, "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnPagar.Enabled = false;
                return;
            }

            _servicos = oCtrl.listaServicos;

            foreach (string[] s in _servicos)
                lstServicos.Items.Add(s[1]); // Nome visível

            if (lstServicos.Items.Count > 0)
                lstServicos.SelectedIndex = 0;
        }

        // Quando o utilizador muda a seleção, atualiza o label da entidade
        private void lstServicos_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = lstServicos.SelectedIndex;
            if (idx >= 0)
                lblEntidadeValor.Text = _servicos[idx][2]; // Entidade
        }

        // Só dígitos na referência
        private void txtReferencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            if (lstServicos.SelectedIndex < 0)
            {
                MessageBox.Show("Selecione um serviço.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtReferencia.Text.Length != 9)
            {
                MessageBox.Show("A referência tem de ter exatamente 9 dígitos.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(txtValorServ.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Introduza um valor numérico válido e maior que zero.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string servico  = _servicos[lstServicos.SelectedIndex][1];
            string entidade = _servicos[lstServicos.SelectedIndex][2];
            string descricao = servico + " | Ent " + entidade + " Ref " + txtReferencia.Text;

            cControlo oCtrl = new cControlo();
            oCtrl.fPagamento(_contaId, valor, descricao);

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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
