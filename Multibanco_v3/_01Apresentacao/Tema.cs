namespace Multibanco
{
    // Classe estática que centraliza toda a identidade visual da aplicação.
    // Basta chamar Tema.Aplicar(this) no construtor de cada formulário.
    // Qualquer alteração de cor ou estilo feita aqui propaga-se a toda a app.
    internal static class Tema
    {
        // --- Paleta de cores: tema escuro inspirado em terminais ATM ---
        public static readonly Color Fundo         = Color.FromArgb(22,  22,  22);   // fundo principal
        public static readonly Color FundoControlo = Color.FromArgb(45,  45,  45);   // fundo de inputs e listas
        public static readonly Color TextoPrimario = Color.FromArgb(210, 210, 210);  // texto normal
        public static readonly Color TextoAcento   = Color.FromArgb(255, 200,  0);   // amarelo ATM — valores e títulos
        public static readonly Color BotaoAcao     = Color.FromArgb(0,   100, 180);  // azul — ações principais
        public static readonly Color BotaoPerigo   = Color.FromArgb(180,  40,  40);  // vermelho — sair/cancelar
        public static readonly Color BotaoNeutro   = Color.FromArgb(65,   65,  65);  // cinza — ações secundárias

        // Aplica o tema ao formulário e a todos os seus controlos recursivamente
        public static void Aplicar(Form form)
        {
            form.BackColor = Fundo;
            form.ForeColor = TextoPrimario;
            AplicarAosControlos(form.Controls);
        }

        private static void AplicarAosControlos(Control.ControlCollection controlos)
        {
            foreach (Control c in controlos)
            {
                switch (c)
                {
                    case Button btn:
                        EstilizarBotao(btn);
                        break;

                    case TextBox txt:
                        txt.BackColor   = FundoControlo;
                        txt.ForeColor   = TextoAcento;
                        txt.BorderStyle = BorderStyle.FixedSingle;
                        break;

                    case Label lbl:
                        // Títulos (fonte grande ou nome contém "titulo") ficam em amarelo
                        lbl.ForeColor = (lbl.Font.Size >= 12 || lbl.Name.ToLower().Contains("titulo"))
                            ? TextoAcento
                            : TextoPrimario;
                        break;

                    case ListBox lst:
                        lst.BackColor = FundoControlo;
                        lst.ForeColor = TextoPrimario;
                        break;

                    case ListView lv:
                        lv.BackColor     = FundoControlo;
                        lv.ForeColor     = TextoPrimario;
                        lv.BorderStyle   = BorderStyle.None;
                        break;

                    case ComboBox cmb:
                        cmb.BackColor = FundoControlo;
                        cmb.ForeColor = TextoPrimario;
                        break;

                    case GroupBox grp:
                        grp.BackColor = Fundo;
                        grp.ForeColor = TextoPrimario;
                        break;

                    case DateTimePicker dtp:
                        dtp.BackColor = FundoControlo;
                        dtp.ForeColor = TextoPrimario;
                        dtp.CalendarForeColor         = TextoPrimario;
                        dtp.CalendarMonthBackground   = FundoControlo;
                        dtp.CalendarTitleBackColor     = Color.FromArgb(40, 40, 80);
                        dtp.CalendarTitleForeColor     = TextoAcento;
                        break;
                }

                // Aplicar recursivamente a containers (exceto botões)
                if (c.Controls.Count > 0 && c is not Button)
                    AplicarAosControlos(c.Controls);
            }
        }

        private static void EstilizarBotao(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize  = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(80, 80, 80);
            btn.ForeColor = Color.White;
            btn.Cursor    = Cursors.Hand;

            string nome = btn.Name.ToLower();

            if (nome.Contains("sair") || nome.Contains("cancelar") || nome.Contains("fechar") || nome.Contains("exit") || nome.Contains("eliminar"))
                btn.BackColor = BotaoPerigo;
            else if (nome.Contains("todos") || nome.Contains("limpar") || nome.Contains("filtrar"))
                btn.BackColor = BotaoNeutro;
            else
                btn.BackColor = BotaoAcao;
        }
    }
}
