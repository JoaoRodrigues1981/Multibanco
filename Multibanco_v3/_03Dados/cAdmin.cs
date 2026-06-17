using System;
using System.Collections.Generic;

using Npgsql; // biblioteca para comunicar com PostgreSQL

namespace Multibanco._03Dados
{
    // Esta classe trata das operações de gestão de clientes e contas.
    // É usada exclusivamente pelo BackOffice (admin "sibs").
    // Separada do cLogin por responsabilidade — cLogin autentica, cAdmin gere.
    internal class cAdmin
    {
        // Resultado da operação — o cControlo lê estas variáveis após cada chamada.
        public bool   operacao = true;
        public string mensagem = "";

        // Lista de clientes devolvida por fListarCredenciais.
        // Cada entrada é um array: [Id, Banco, Cliente, Conta, Saldo, MBWay, Bloqueada].
        public List<string[]> listaCredenciais = new List<string[]>();

        // Objeto de ligação — usa cConexao para abrir/fechar a BD
        cConexao oConexao = new cConexao();

        // ------------------------------------------------------------------
        // Helper privado — elimina o boilerplate try/catch que se repetia em
        // fInserirCredencial, fAlternarMBWay e fEliminarCredencial.
        // Cada método público fica focado apenas na sua query; a mecânica de
        // abertura/fecho de ligação fica centralizada aqui.
        // prefixoErro é prefixado à mensagem de exceção para facilitar diagnóstico.
        //
        // Nota: oCmd é criado com "using var" pelo método que chama fExecutarNonQuery.
        // Por isso não chamamos oCmd.Dispose() aqui — o "using var" do chamador
        // garante que o Dispose() corre automaticamente ao sair do seu scope,
        // mesmo que ocorra uma exceção.
        // ------------------------------------------------------------------
        private bool fExecutarNonQuery(NpgsqlCommand oCmd, string prefixoErro)
        {
            try
            {
                oCmd.Connection = oConexao.conectar();
                oCmd.ExecuteNonQuery();
                oConexao.desConectar();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = prefixoErro + ex.Message;
            }
            return operacao;
        }

        // ------------------------------------------------------------------
        // Devolve todos os clientes e contas registados em Credenciais.
        // Resultado guardado em listaCredenciais para o formulário apresentar.
        // ------------------------------------------------------------------
        public bool fListarCredenciais()
        {
            listaCredenciais.Clear(); // limpar resultados de chamadas anteriores

            // "using var" garante que oCmd.Dispose() corre automaticamente ao sair
            // do método — mesmo que ocorra uma exceção dentro do try.
            // Sem "using var", o Dispose() só corria no caminho sem erros.
            using var oCmd = new NpgsqlCommand();
            oCmd.CommandText = "SELECT Id, Banco, Cliente, Conta, Saldo, MBWay, Bloqueada FROM Credenciais ORDER BY Cliente, Conta";

            try
            {
                oCmd.Connection = oConexao.conectar();

                // "using var" no reader garante que fecha automaticamente após o while,
                // libertando o cursor antes de fecharmos a ligação
                using var oReader = oCmd.ExecuteReader();

                while (oReader.Read()) // percorrer linha a linha
                {
                    listaCredenciais.Add(new string[]
                    {
                        oReader.GetInt32(0).ToString(),        // Id
                        oReader.GetString(1),                  // Banco
                        oReader.GetString(2),                  // Cliente
                        oReader.GetInt32(3).ToString(),        // Conta
                        oReader.GetDecimal(4).ToString("F2"),  // Saldo (2 casas decimais)
                        oReader.GetBoolean(5) ? "Sim" : "Não", // MBWay ativo?
                        oReader.GetBoolean(6) ? "Sim" : "Não"  // Bloqueada?
                    });
                }

                oConexao.desConectar();
                mensagem = listaCredenciais.Count + " cliente(s) encontrado(s).";
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro ao listar clientes: " + ex.Message;
            }

            return operacao;
        }

        // ------------------------------------------------------------------
        // Insere um novo cliente e conta em Credenciais.
        // O saldo inicial é sempre 100€ — regra do enunciado para o admin sibs.
        // ------------------------------------------------------------------
        public bool fInserirCredencial(string banco, string cliente, string conta, string pin)
        {
            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Banco",   banco);
            oCmd.Parameters.AddWithValue("@Cliente", cliente);
            oCmd.Parameters.AddWithValue("@Conta",   Convert.ToInt32(conta));
            oCmd.Parameters.AddWithValue("@Pin",     Convert.ToInt32(pin));
            // Saldo inicial fixo em 100€ — definido pelo enunciado, não configurável
            oCmd.Parameters.AddWithValue("@Saldo",   100.00m);

            oCmd.CommandText = "INSERT INTO Credenciais (Banco, Cliente, Conta, Pin, Saldo) VALUES (@Banco, @Cliente, @Conta, @Pin, @Saldo)";

            // Não usamos fExecutarNonQuery aqui porque precisamos de distinguir o erro 23505
            // (conta duplicada) do erro genérico de BD — para mostrar uma mensagem clara ao utilizador.
            try
            {
                oCmd.Connection = oConexao.conectar();
                oCmd.ExecuteNonQuery();
                oConexao.desConectar();
                mensagem = "Cliente inserido com sucesso. Saldo inicial: 100,00€.";
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                // 23505 = UNIQUE violation — número de conta já existe na BD
                operacao = false;
                mensagem = "Já existe uma conta com o número " + conta + ". Escolha outro número de conta.";
                oConexao.desConectar();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro ao inserir cliente: " + ex.Message;
                oConexao.desConectar();
            }

            return operacao;
        }

        // Nota: fObterSaldo não existe aqui intencionalmente.
        // Já existe em cMovimento — duplicar seria ter a mesma query em dois sítios.
        // O cControlo usa cMovimento.fObterSaldo antes de chamar fEliminarCredencial.

        // ------------------------------------------------------------------
        // Ativa ou desativa o MBWay de uma conta pelo seu Id.
        // Funcionalidade extra — gerida pelo admin sibs.
        // ------------------------------------------------------------------
        public bool fAlternarMBWay(int id, bool novoEstado)
        {
            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id",    id);
            oCmd.Parameters.AddWithValue("@MBWay", novoEstado);
            oCmd.CommandText = "UPDATE Credenciais SET MBWay = @MBWay WHERE Id = @Id";

            if (fExecutarNonQuery(oCmd, "Erro ao alterar MBWay: "))
                mensagem = "MBWay " + (novoEstado ? "ativado" : "desativado") + " com sucesso.";

            return operacao;
        }

        // ------------------------------------------------------------------
        // Elimina uma conta de Credenciais pelo seu Id.
        // Só deve ser chamado após confirmar que o saldo é 0 (validação no cControlo).
        //
        // PORQUÊ dois DELETEs em vez de um?
        // A tabela Movimentos tem uma chave estrangeira (FK):
        //   Movimentos.ContaId → Credenciais.Id
        // Isso significa que cada movimento "pertence" a uma conta.
        // O PostgreSQL não permite apagar uma linha de Credenciais enquanto
        // existirem linhas em Movimentos que a referenciam — daria erro 23503
        // (foreign key violation).
        // Solução: apagar primeiro os movimentos da conta, depois a conta.
        // ------------------------------------------------------------------
        public bool fEliminarCredencial(int id)
        {
            // PASSO 1 — apagar o histórico de movimentos da conta
            // Tem de ser feito ANTES de apagar a conta por causa da FK
            using var oCmdMov = new NpgsqlCommand();
            oCmdMov.Parameters.AddWithValue("@Id", id);
            oCmdMov.CommandText = "DELETE FROM Movimentos WHERE ContaId = @Id";

            if (!fExecutarNonQuery(oCmdMov, "Erro ao eliminar movimentos: "))
                return operacao; // se falhou aqui, não tenta apagar a conta

            // PASSO 2 — apagar a conta (só chega aqui se o passo 1 correu bem)
            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id", id);
            oCmd.CommandText = "DELETE FROM Credenciais WHERE Id = @Id";

            if (fExecutarNonQuery(oCmd, "Erro ao eliminar cliente: "))
                mensagem = "Cliente eliminado com sucesso.";

            return operacao;
        }

        // ------------------------------------------------------------------
        // Desbloqueia uma conta bloqueada pelo Id — reset às tentativas e Bloqueada = FALSE.
        // Só deve ser chamado pelo administrador "sibs" no BackOffice.
        // ------------------------------------------------------------------
        public bool fDesbloquearConta(int id)
        {
            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id", id);
            oCmd.CommandText = "UPDATE Credenciais SET Tentativas = 0, Bloqueada = FALSE WHERE Id = @Id";

            if (fExecutarNonQuery(oCmd, "Erro ao desbloquear conta: "))
                mensagem = "Conta desbloqueada com sucesso.";

            return operacao;
        }
    }
}
