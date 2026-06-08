namespace Multibanco
{
    // Simulador de empréstimo — extra, sem BD, cálculo puro.
    // Compara duas modalidades: indexada (Euribor + Spread) vs taxa fixa.
    public partial class frmSimuladorEmprestimo : Form
    {
        public frmSimuladorEmprestimo()
        {
            InitializeComponent();
            Tema.Aplicar(this);
        }

        // Fórmula PMT (equivalente ao PMT do Excel):
        //   Prestação = Capital × r / (1 - (1+r)^(-n))
        // onde r = TAN anual / 12 (taxa mensal) e n = prazo em meses.
        // Caso especial: se TAN = 0%, a prestação é simplesmente Capital / n.
        private decimal fCalcularPrestacao(decimal capital, decimal tanAnual, int nMeses)
        {
            if (tanAnual == 0)
                return Math.Round(capital / nMeses, 2);

            double r   = (double)tanAnual / 100.0 / 12.0;
            double pmt = (double)capital * r / (1 - Math.Pow(1 + r, -nMeses));
            return (decimal)Math.Round(pmt, 2);
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            // Validar valor do empréstimo
            if (!decimal.TryParse(txtValorEmp.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal capital) || capital <= 0)
            {
                MessageBox.Show("Introduza um valor de empréstimo válido e maior que zero.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar Euribor
            if (!decimal.TryParse(txtEuribor.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal euribor) || euribor < 0)
            {
                MessageBox.Show("Introduza uma Euribor válida (≥ 0%).", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar Spread
            if (!decimal.TryParse(txtSpread.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal spread) || spread < 0)
            {
                MessageBox.Show("Introduza um spread válido (≥ 0%).", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validar taxa fixa
            if (!decimal.TryParse(txtTaxaFixa.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal taxaFixa) || taxaFixa <= 0)
            {
                MessageBox.Show("Introduza uma taxa fixa válida e maior que zero.", "ERRO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int anos = int.Parse(cmbPrazo.SelectedItem.ToString());
            int n    = anos * 12; // prazo em meses

            // Calcular para cada modalidade
            decimal tanIndexado = euribor + spread;
            decimal tanFixo     = taxaFixa;

            decimal prestIndexado = fCalcularPrestacao(capital, tanIndexado, n);
            decimal prestFixo     = fCalcularPrestacao(capital, tanFixo,     n);

            decimal totalIndexado = prestIndexado * n;
            decimal totalFixo     = prestFixo     * n;

            decimal jurosIndexado = totalIndexado - capital;
            decimal jurosFixo     = totalFixo     - capital;

            // Mostrar resultados
            lblTAN_I.Text   = tanIndexado.ToString("F2") + "%";
            lblTAN_F.Text   = tanFixo.ToString("F2")     + "%";
            lblPrest_I.Text = prestIndexado.ToString("F2") + " €";
            lblPrest_F.Text = prestFixo.ToString("F2")     + " €";
            lblJuros_I.Text = jurosIndexado.ToString("F2") + " €";
            lblJuros_F.Text = jurosFixo.ToString("F2")     + " €";
            lblCusto_I.Text = totalIndexado.ToString("F2") + " €";
            lblCusto_F.Text = totalFixo.ToString("F2")     + " €";
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
