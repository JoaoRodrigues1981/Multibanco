using Multibanco._02Controlo;

namespace Multibanco
{
    // Formulário de pagamento de serviços — abre a partir do frmMultibanco.
    // Recebe o contaId via construtor. Após pagamento bem-sucedido, expõe
    // NovoSaldo para o frmMultibanco refrescar o ecrã sem ir à BD de novo.
    public partial class frmPagamento : Form
    {
        private int _contaId;

        // Saldo após pagamento — lido pelo frmMultibanco se DialogResult == OK
        public decimal NovoSaldo { get; private set; }

        public frmPagamento(int contaId)
        {
            InitializeComponent();
            Tema.Aplicar(this);
            _contaId = contaId;
        }

        // Aceita só dígitos nos campos de entidade e referência
        private void txtEntidade_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void txtReferencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        // Botão "Pagar"
        private void btnPagar_Click(object sender, EventArgs e)
        {
            // Validar entidade: exatamente 5 dígitos
            if (txtEntidade.Text.Length != 5)
            {
                MessageBox.Show("A entidade tem de ter exatamente 5 dígitos.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar referência: exatamente 9 dígitos
            if (txtReferencia.Text.Length != 9)
            {
                MessageBox.Show("A referência tem de ter exatamente 9 dígitos.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar valor: número positivo
            if (!decimal.TryParse(txtValorPag.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Introduza um valor numérico válido e maior que zero.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string descricao = "Entidade " + txtEntidade.Text + " Ref " + txtReferencia.Text;

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

        // Botão "Cancelar" — fecha sem fazer nada
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
