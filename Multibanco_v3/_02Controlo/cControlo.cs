using System;
using System.Collections.Generic;
using Multibanco._03Dados; // para poder usar cLogin, cAdmin e cMovimento

namespace Multibanco._02Controlo
{
    // Esta classe é o intermediário entre os formulários e a base de dados.
    // O formulário nunca fala diretamente com a BD — fala sempre com o cControlo.
    // Responsabilidades: validar campos e coordenar as operações sobre credenciais.
    internal class cControlo
    {
        // Resultado da operação — o formulário lê estas variáveis após chamar os métodos
        public bool      operacao = true;   // false se houve qualquer problema
        public bool      existe   = true;   // false se as credenciais não foram encontradas
        public string    mensagem = "";     // texto da mensagem de erro ou sucesso
        public bool      mbwayAtivo = false;     // indica se o MBWay está ativo para a conta (usado no acesso ao formulário de MBWay)

        // Dados da conta após login com sucesso — passados ao frmMultibanco via construtor
        public int     contaId  = 0;      // Id da linha em Credenciais (chave para Movimentos)
        public decimal saldo    = 0;      // Saldo atual da conta
        public string  banco    = "";     // Banco do cliente
        public string  cliente  = "";     // Nome do cliente
        public int     conta    = 0;      // Número de conta

        
        
        // ======== LISTAS ========

        // Lista de clientes devolvida por fListarCredenciais — lida pelo frmAdmin
        public List<string[]> listaCredenciais = new List<string[]>();

        // Lista de movimentos devolvida por fListarMovimentos — lida pelo frmMultibanco
        public List<string[]> listaMovimentos = new List<string[]>();

        // Lista de serviços pré-definidos — lida pelo frmPagamentosServicos
        public List<string[]> listaServicos = new List<string[]>();

        // Lista de contas MBWay devolvida por fListarContasMBWay — lida pelo frmMBWay
        public List<string[]> listaContasMBWay = new List<string[]>();



        // ============= METODOS USADOS PARA AUTENTICAÇÃO e ALTERAR PIN =============

        // TESTAR CAMPOS (CLIENTE)
        // é usada pelo frmAutenticacao para validar os campos de login antes de chamar fValidarCredenciais
        // Verificação básica para clientes normais: algum dos 4 campos está vazio?
        // NOTA IMPORANTE: É chamado antes de tentar ir à base de dados.
        public void fTestarCampos(string txtBanco, string txtCliente, string txtConta, string txtPin)
        {
            if (txtBanco == "" || txtCliente == "" || txtConta == "" || txtPin == "")
            {
                mensagem = "Preencha todos os campos.";
                operacao = false; // sinalizar que não pode avançar
            }
        }

        // TESTA CAMPOS (ADMIN)
        // Verificação básica para o administrador "sibs": só valida Cliente e PIN.
        // Banco e Conta não são usados pelo admin — não faz sentido obrigá-lo a preenchê-los.
        public void fTestarCamposAdmin(string txtCliente, string txtPin)
        {
            if (txtCliente == "" || txtPin == "")
            {
                mensagem = "Preencha o utilizador e o PIN.";
                operacao = false;
            }
        }

        // Delega a validação de credenciais ao cLogin (Camada 3) e republica os resultados.
        // Só é chamado se fTestarCampos não encontrou campos vazios.
        public void fValidarCredenciais(string txtBanco, string txtCliente, string txtConta, string txtPin)
        {
            cLogin oLogin = new cLogin();

            // Execução passa para a Camada 3 — é aqui que as queries correm
            operacao = oLogin.fObterCredenciais(txtBanco, txtCliente, txtConta, txtPin);

            // Copiar sempre todas as variáveis do cLogin para as suas variáveis públicas.
            existe   = oLogin.existe;
            mensagem = oLogin.mensagem;
            contaId  = oLogin.contaId;
            saldo    = oLogin.saldo;
            banco    = oLogin.banco;
            cliente  = oLogin.cliente;
            conta    = oLogin.conta;
        }

        // Valida as credenciais do administrador "sibs" na tabela Admins.
        // Chamado pelo frmAutenticacao quando deteta que o utilizador é "sibs".
        public void fValidarAdmin(string username, string pin)
        {
            cLogin oLogin = new cLogin();

            // Verificar na tabela Admins — separada de Credenciais porque o admin não tem conta bancária
            operacao = oLogin.fValidarAdmin(username, pin);
            existe   = oLogin.existe;
            mensagem = oLogin.mensagem;
        }

        // VALIDAR SE NOVO PIN E CONFIRMAÇÃO SÃO IGUAIS
        // chamado pelo frmUpdate.cs > btnOK_Click, antes de ir à base de dados para alterar o PIN.
        // Verifica se os dois PINs introduzidos pelo utilizador são iguais, antes de ir à base de dados.
        public void fValidarPinsIguais(string pin1, string pin2)
        {
            if (pin1 != pin2)
            {
                operacao = false;
                mensagem = "Os PINs introduzidos são diferentes.";
            }
        }

        // ATUALIZA PIN via camada 3
        // Só deve ser chamado depois de fValidarPinsIguais confirmar que são iguais.
        public void fAtualizarPin(string banco, string cliente, string conta, string oldPin, string newPin)
        {
            cLogin oClog = new cLogin();

            operacao = oClog.fGravarNovoPin(banco, cliente, conta, oldPin, newPin);
            mensagem = oClog.mensagem;
            existe   = oClog.existe;
        }



        // ================ METODOS LISTA DE MOVIMENTOS ================

        // Ao entrar no form1, vamos logo ter de mostrar a lista de movimentos do cliente.
        // Este metodo carrega os movimentos de uma conta com filtro de datas opcional.
        // dataInicio e dataFim podem ser null para listar todos os movimentos.
        public void fListarMovimentos(int contaId, DateTime? dataInicio, DateTime? dataFim)
        {
            cMovimento oMov = new cMovimento();

            operacao = oMov.fListarMovimentos(contaId, dataInicio, dataFim);
            mensagem = oMov.mensagem;
            listaMovimentos = oMov.listaMovimentos;
        }



        // ========= METODOS DE OPERAÇÕES BANCÁRIAS (cliente realiza diretamente no frmMultibanco) ==========

        // Saldo atualizado após operação — o formulário usa para refrescar o ecrã
        public decimal saldoAtualizado = 0;

        // LAVANTAMENTO
        // Vai usar metodos auxiliares (helper): fSaldoSuficiente para validar se o cliente tem saldo suficiente para o levantamento.

        public void fLevantar(int contaId, decimal valor)
        {
            cMovimento oMov = new cMovimento();

            decimal saldoAtual = oMov.fObterSaldo(contaId);

            if (!oMov.operacao) // erro ao aceder à BD
            {
                operacao = false;
                mensagem = oMov.mensagem;
                return;
            }

            // Regra de negócio: o valor tem de ser positivo — o formulário só valida formato
            if (valor <= 0)
            {
                operacao = false;
                mensagem = "O valor a levantar tem de ser maior que zero.";
                return;
            }

            if (!fSaldoSuficiente(saldoAtual, valor)) return; // regra do enunciado

            decimal novoSaldo = saldoAtual - valor; // calcular o novo saldo com o saldo atual recebido do cMovimento

            oMov.fAtualizarSaldo(contaId, novoSaldo); // enviamos o novo saldo para a camada 3 atualizar na BD
            oMov.fInserirMovimento(contaId, "L", valor, novoSaldo, "Levantamento");

            operacao = oMov.operacao;
            mensagem = oMov.operacao ? "Levantamento de " + valor.ToString("F2") + "€ realizado." : oMov.mensagem;
            saldoAtualizado = novoSaldo;
        }

        // ------------------------------------------------------------------
        // Helper privado — verifica se o saldo é suficiente para a operação. 
        // Reutilizado em fLevantar, fTransferir, fMBWay e fPagamento para evitar a repetição do bloco if + mensagem de erro em cada método.
        // Devolve false e define operacao/mensagem se o saldo for insuficiente.
        // ------------------------------------------------------------------
        private bool fSaldoSuficiente(decimal saldo, decimal valor)
        {
            if (saldo < valor)
            {
                operacao = false;
                mensagem = "Saldo insuficiente. Saldo disponível: " + saldo.ToString("F2") + "€.";
                return false;
            }
            return true;
        }


        // DEPOSITO
        // Atualiza saldo e regista movimento 'D'.
        public void fDepositar(int contaId, decimal valor)
        {
            cMovimento oMov = new cMovimento();

            // Regra de negócio: o valor tem de ser positivo — o formulário só valida formato
            if (valor <= 0)
            {
                operacao = false;
                mensagem = "O valor a depositar tem de ser maior que zero.";
                return;
            }

            decimal saldoAtual = oMov.fObterSaldo(contaId);

            if (!oMov.operacao)
            {
                operacao = false;
                mensagem = oMov.mensagem;
                return;
            }

            decimal novoSaldo = saldoAtual + valor;

            oMov.fAtualizarSaldo(contaId, novoSaldo);
            oMov.fInserirMovimento(contaId, "D", valor, novoSaldo, "Depósito");

            operacao        = oMov.operacao;
            mensagem        = oMov.operacao ? "Depósito de " + valor.ToString("F2") + "€ realizado." : oMov.mensagem;
            saldoAtualizado = novoSaldo;
        }


        // TRANSFERENCIA
        // Transferência: valida saldo origem, atualiza os dois saldos e regista 2 movimentos 'T'.
        // contaOrigem  — Id interno da conta do utilizador logado (vem do login, é o Id da linha)
        // contaNumDest — número de conta visível introduzido no txtContaDestino (ex: 654321)
        //                É resolvido para o Id interno via fObterContaDestino antes de avançar.
        public void fTransferir(int contaOrigem, int contaNumDest, decimal valor)
        {
            cMovimento oMov = new cMovimento();

            // Regra de negócio: o valor tem de ser positivo — o formulário só valida formato
            if (valor <= 0)
            {
                operacao = false;
                mensagem = "O valor a transferir tem de ser maior que zero.";
                return;
            }

            // Resolver o número de conta destino para o Id interno + saldo (1 única query)
            if (!oMov.fObterContaDestino(contaNumDest, out int idDestino, out decimal saldoDestino))
            {
                operacao = false;
                mensagem = oMov.mensagem; // "Conta destino não encontrada." ou erro de BD
                return;
            }

            // Não permitir transferência para a própria conta (comparar Ids, não números)
            if (contaOrigem == idDestino)
            {
                operacao = false;
                mensagem = "A conta de origem e destino não podem ser iguais.";
                return;
            }

            decimal saldoOrigem   = oMov.fObterSaldo(contaOrigem);

            if (!oMov.operacao) { operacao = false; mensagem = oMov.mensagem; return; }

            if (!fSaldoSuficiente(saldoOrigem, valor)) return;

            decimal novoSaldoOrig = saldoOrigem  - valor;
            decimal novoSaldoDest = saldoDestino + valor;

            // Atualizar os dois saldos e registar um movimento em cada conta
            oMov.fAtualizarSaldo(contaOrigem, novoSaldoOrig); // atualiza saldo da conta de origem
            oMov.fAtualizarSaldo(idDestino,   novoSaldoDest); // atualiza saldo da conta de destino
            oMov.fInserirMovimento(contaOrigem, "T", valor, novoSaldoOrig, "Transferência para conta " + contaNumDest); // regista movimento na conta de origem
            oMov.fInserirMovimento(idDestino,   "T", valor, novoSaldoDest, "Transferência de conta "   + contaNumDest); // regista movimento na conta de destino

            operacao        = oMov.operacao;
            mensagem        = oMov.operacao ? "Transferência de " + valor.ToString("F2") + "€ realizada." : oMov.mensagem;
            saldoAtualizado = novoSaldoOrig;
        }


        // ================ METODOS USADOS NO FORMULÁRIO MBWAY ================
        // MBWay: verifica se conta destino é aderente, depois funciona como transferência com tipo 'M'.
        // contaOrigem  — Id interno da conta do utilizador logado
        // contaNumDest — número de conta visível introduzido no txtContaDestino (ex: 654321)
        //                Resolvido para Id interno + saldo via fObterContaDestino (1 query).
        //
        // Regras de negócio centralizadas aqui (não no formulário):
        // o formulário só valida formato (é um número?); as regras da operação ficam nesta camada
        // para se aplicarem independentemente de qual formulário chamar fMBWay.
        public void fMBWay(int contaOrigem, int contaNumDest, decimal valor)
        {
            cMovimento oMov = new cMovimento();

            // Regra de negócio: o valor tem de ser positivo
            if (valor <= 0)
            {
                operacao = false;
                mensagem = "O valor a enviar tem de ser maior que zero.";
                return;
            }

            // Regra de negócio: limite MBWay por transação
            if (valor > 300)
            {
                operacao = false;
                mensagem = "O valor a enviar não pode exceder 300€ por transação.";
                return;
            }


            // Resolver o número de conta destino para o Id interno + saldo (1 única query)
            if (!oMov.fObterContaDestino(contaNumDest, out int idDestino, out decimal saldoDestino))
            {
                operacao = false;
                mensagem = oMov.mensagem; // "Conta destino não encontrada." ou erro de BD
                return;
            }

            // A conta destino tem de ser aderente ao MBWay — verificar pelo Id interno
            bool destinoAderente = oMov.fObterMBWay(idDestino);

            if (!oMov.operacao) { operacao = false; mensagem = oMov.mensagem; return; }

            if (!destinoAderente)
            {
                operacao = false;
                mensagem = "A conta destino não é aderente ao MBWay.";
                return;
            }

            decimal saldoOrigem   = oMov.fObterSaldo(contaOrigem);

            if (!oMov.operacao) { operacao = false; mensagem = oMov.mensagem; return; }

            if (!fSaldoSuficiente(saldoOrigem, valor)) return;

            decimal novoSaldoOrig = saldoOrigem  - valor;
            decimal novoSaldoDest = saldoDestino + valor;

            oMov.fAtualizarSaldo(contaOrigem, novoSaldoOrig);
            oMov.fAtualizarSaldo(idDestino,   novoSaldoDest);
            oMov.fInserirMovimento(contaOrigem, "M", valor, novoSaldoOrig, "MBWay enviado para conta " + contaNumDest);
            oMov.fInserirMovimento(idDestino,   "M", valor, novoSaldoDest, "MBWay recebido de conta "  + contaNumDest);

            operacao        = oMov.operacao;
            mensagem        = oMov.operacao ? "MBWay de " + valor.ToString("F2") + "€ enviado." : oMov.mensagem;
            saldoAtualizado = novoSaldoOrig;
        }

        // Verificar se a conta é aderente ao MBWay (usado para mostrar/ocultar opção no UI)
        public void fVerificarMBWay(int contaId) 
        {
            cMovimento oMov = new cMovimento();
            bool aderente = oMov.fObterMBWay(contaId);
            mbwayAtivo = aderente;
        }

        // Carrega a LISTA com as contas com MBWay ativo, excluindo a conta do utilizador logado.
        public void fListarContasMBWay(int contaOrigem)
        {
            cMBWay oMBWay = new cMBWay();

            operacao = oMBWay.fListarContasMBWay(contaOrigem); // preciso do resultado para saber se houve erro de BD
            mensagem = oMBWay.mensagem; // Se tiver erro de BD preciso da mensagem para mostrar ao utilizador
            listaContasMBWay = oMBWay.listaContasMBWay; // esta é a lista que vou enviar para o formulário de transferência MBWay para preencher a dropdown
        }



        // ================ METODOS USADOS NO FORMULÁRIO SERVIÇOS ================

        // Pagamento de serviço pré-definido: igual ao levantamento mas tipo 'P' e descrição personalizada.
        // Regras de negócio centralizadas aqui — o formulário só valida formato (campo é número?).
        public void fPagamento(int contaId, decimal valor, string descricao, string referencia)
        {
            cMovimento oMov = new cMovimento();

            // Regra de negócio: referência Multibanco tem sempre 9 dígitos
            if (referencia.Length != 9)
            {
                operacao = false;
                mensagem = "A referência tem de ter exatamente 9 dígitos.";
                return;
            }

            // Regra de negócio: o valor tem de ser positivo
            if (valor <= 0)
            {
                operacao = false;
                mensagem = "O valor a pagar tem de ser maior que zero.";
                return;
            }

            decimal saldoAtual = oMov.fObterSaldo(contaId);

            if (!oMov.operacao) { operacao = false; mensagem = oMov.mensagem; return; }

            if (!fSaldoSuficiente(saldoAtual, valor)) return;

            decimal novoSaldo = saldoAtual - valor;

            oMov.fAtualizarSaldo(contaId, novoSaldo);
            oMov.fInserirMovimento(contaId, "P", valor, novoSaldo, descricao);

            operacao        = oMov.operacao;
            mensagem        = oMov.operacao ? "Pagamento de " + valor.ToString("F2") + "€ realizado." : oMov.mensagem;
            saldoAtualizado = novoSaldo;
        }

        // Carrega a lista de serviços pré-definidos da BD.
        public void fListarServicos()
        {
            cServico oServ = new cServico();

            operacao     = oServ.fListarServicos();
            mensagem     = oServ.mensagem;
            listaServicos = oServ.listaServicos;
        }



        //  ================ METODOS USADOS NO FORMULÁRIO BACK OFFICE SIBS ================

        // Carrega todos os clientes e contas da BD para listaCredenciais.
        // O frmAdmin lê listaCredenciais para preencher o ListView.
        public void fListarCredenciais()
        {
            cAdmin oAdmin = new cAdmin();

            operacao = oAdmin.fListarCredenciais();
            mensagem = oAdmin.mensagem;
            listaCredenciais = oAdmin.listaCredenciais; // copiar a lista para o formulário ler
        }

        // Insere um novo cliente e conta com saldo inicial de 100€.
        // Regra do enunciado: o admin sibs associa sempre 100€ ao criar uma conta.
        public void fInserirCliente(string banco, string cliente, string conta, string pin)
        {
            cAdmin oAdmin = new cAdmin();

            operacao = oAdmin.fInserirCredencial(banco, cliente, conta, pin);
            mensagem = oAdmin.mensagem;
        }

        // Elimina um cliente pelo Id — mas só se o saldo for exatamente 0.
        // Regra do enunciado: não pode eliminar se houver saldo (positivo ou negativo).
        // A validação do saldo fica aqui (camada de negócio), não no cAdmin (camada de dados).
        public void fEliminarCliente(int id)
        {
            // Usar cMovimento para obter o saldo — evita duplicar o mesmo SQL em cAdmin
            cMovimento oMov = new cMovimento();
            decimal saldoAtual = oMov.fObterSaldo(id);

            if (!oMov.operacao)
            {
                operacao = false;
                mensagem = oMov.mensagem;
                return;
            }

            if (saldoAtual != 0)
            {
                operacao = false;
                mensagem = "Não é possível eliminar. A conta tem saldo de " + saldoAtual.ToString("F2") + "€.";
                return;
            }

            cAdmin oAdmin = new cAdmin();
            operacao = oAdmin.fEliminarCredencial(id);
            mensagem = oAdmin.mensagem;
        }

        // Ativa ou desativa o MBWay de uma conta pelo Id.
        // Funcionalidade extra — gerida pelo admin sibs no BackOffice.
        public void fAlternarMBWay(int id, bool novoEstado)
        {
            cAdmin oAdmin = new cAdmin();

            operacao = oAdmin.fAlternarMBWay(id, novoEstado);
            mensagem = oAdmin.mensagem;
        }

        // Desbloqueia uma conta bloqueada — só o admin sibs pode chamar isto.
        public void fDesbloquearConta(int id)
        {
            cAdmin oAdmin = new cAdmin();

            operacao = oAdmin.fDesbloquearConta(id);
            mensagem = oAdmin.mensagem;
        }
    }
}
