using System;
using System.Collections.Generic;

using Npgsql;

namespace Multibanco._03Dados
{
    // Esta classe trata de todas as operações sobre movimentos e saldo.
    // É chamada pelo cControlo — nunca diretamente pelos formulários.
    // Responsabilidades: ler/atualizar saldo, inserir e listar movimentos.
    internal class cMovimento
    {
        // Resultado da operação — o cControlo lê estas variáveis após cada chamada.
        public bool   operacao = true;
        public string mensagem = "";

        // Lista de movimentos devolvida por fListarMovimentos — lida pelo frmMultibanco.
        // Cada entrada: [Tipo, Valor, SaldoApos, DataHora, Descricao]
        public List<string[]> listaMovimentos = new List<string[]>();

        cConexao oConexao = new cConexao();

        // ------------------------------------------------------------------
        // Devolve os movimentos de uma conta, opcionalmente filtrados por datas.
        // Se dataInicio ou dataFim forem null, não aplica esse limite.
        // Resultado guardado em listaMovimentos para o formulário apresentar.
        // ------------------------------------------------------------------
        public bool fListarMovimentos(int contaId, DateTime? dataInicio, DateTime? dataFim)
        {
            listaMovimentos.Clear();

            // "using var" garante que oCmd.Dispose() corre automaticamente ao sair do método — mesmo que ocorra uma exceção dentro do try.
            // Sem "using var", o Dispose() só corria no caminho sem erros.
            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@ContaId", contaId);

            string sql = "SELECT Tipo, Valor, SaldoApos, DataHora, Descricao FROM Movimentos WHERE ContaId = @ContaId";

            if (dataInicio.HasValue)
            {
                sql += " AND DataHora >= @DataInicio";
                oCmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                sql += " AND DataHora <= @DataFim";
                // O utilizador escolhe um dia (ex: 2026-06-17), que em C# começa às 00:00:00.
                // Um movimento feito às 15:30 desse dia ficaria fora do filtro com <= @DataFim.
                // AddDays(1).AddSeconds(-1) converte para 2026-06-17 23:59:59 — inclui o dia inteiro.
                oCmd.Parameters.AddWithValue("@DataFim", dataFim.Value.AddDays(1).AddSeconds(-1));
            }

            sql += " ORDER BY DataHora DESC";
            oCmd.CommandText = sql;

            try
            {
                oCmd.Connection = oConexao.conectar();

                // "using var" no reader garante que fecha automaticamente após o while, libertando o cursor antes de fecharmos a ligação
                using var oReader = oCmd.ExecuteReader();

                // Cada chamada a Read() avança para a linha seguinte; quando não há mais, devolve false e o ciclo termina.
                while (oReader.Read())
                {
                    listaMovimentos.Add(new string[]
                    {
                        oReader.GetString(0),                                // coluna 0: Tipo ("L","D","T","P","M")
                        oReader.GetDecimal(1).ToString("F2"),                // coluna 1: Valor (ex: "50.00")
                        oReader.GetDecimal(2).ToString("F2"),                // coluna 2: SaldoApos (ex: "800.00")
                        oReader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm"), // coluna 3: DataHora formatada
                        oReader.IsDBNull(4) ? "" : oReader.GetString(4)     // coluna 4: Descricao — IsDBNull evita exceção se o campo for null na BD
                    });
                }

                oConexao.desConectar();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro ao listar movimentos: " + ex.Message;
            }

            return operacao;
        }


        // ------------------------------------------------------------------
        // Helper privado — elimina o boilerplate try/catch que se repetia em
        // fAtualizarSaldo e fInserirMovimento.
        // Centraliza abertura/fecho de ligação e tratamento de erros.
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
        // Devolve o saldo atual de uma conta pelo seu Id em Credenciais.
        // Usado antes de levantar ou transferir para verificar se há saldo suficiente.
        // ------------------------------------------------------------------
        public decimal fObterSaldo(int contaId)
        {
            decimal saldo = -1; // valor sentinela — indica erro se não for substituído

            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id", contaId);
            oCmd.CommandText = "SELECT Saldo FROM Credenciais WHERE Id = @Id";

            try
            {
                oCmd.Connection = oConexao.conectar();
                using var oReader = oCmd.ExecuteReader();

                if (oReader.Read())
                    saldo = oReader.GetDecimal(0);

                oConexao.desConectar();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro ao obter saldo: " + ex.Message;
            }

            return saldo;
        }

        // ------------------------------------------------------------------
        // Verifica se uma conta tem o MBWay ativo.
        // Usado antes de enviar dinheiro via MBWay — a conta destino tem de ser aderente.
        // ------------------------------------------------------------------
        public bool fObterMBWay(int contaId)
        {
            bool mbway = false;

            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id", contaId);
            oCmd.CommandText = "SELECT MBWay FROM Credenciais WHERE Id = @Id";

            try
            {
                oCmd.Connection = oConexao.conectar();
                using var oReader = oCmd.ExecuteReader();

                if (oReader.Read())
                    mbway = oReader.GetBoolean(0);

                oConexao.desConectar();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro ao verificar MBWay: " + ex.Message;
            }

            return mbway;
        }

        // ------------------------------------------------------------------
        // Resolve o número de conta visível (ex: 654321) para o Id interno da linha em Credenciais.
        // Devolve também o saldo atual, evitando uma segunda query separada.
        // Usado em fTransferir e fMBWay: o utilizador introduz o número de conta, mas os
        // métodos de atualização trabalham com o Id (chave primária, mais eficiente e seguro).
        //
        // Retorna true se encontrou a conta. Se não existir, operacao fica false e o cControlo
        // pode mostrar uma mensagem de erro ao utilizador antes de avançar.
        // ------------------------------------------------------------------
        public bool fObterContaDestino(int contaNum, out int idDestino, out decimal saldoDestino)
        {
            idDestino    = -1;  // valor sentinela — indica que a conta não foi encontrada
            saldoDestino =  0;

            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Conta", contaNum);
            // Uma única query devolve Id e Saldo — evita uma segunda viagem à BD
            oCmd.CommandText = "SELECT Id, Saldo FROM Credenciais WHERE Conta = @Conta";

            try
            {
                oCmd.Connection = oConexao.conectar();
                using var oReader = oCmd.ExecuteReader();

                if (oReader.Read())
                {
                    idDestino    = oReader.GetInt32(0);   // coluna 0: Id (chave primária)
                    saldoDestino = oReader.GetDecimal(1); // coluna 1: Saldo atual
                }
                else
                {
                    operacao = false;
                    mensagem = "Conta destino não encontrada.";
                }

                oConexao.desConectar();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro ao localizar conta destino: " + ex.Message;
            }

            return operacao;
        }

        // ------------------------------------------------------------------
        // Atualiza o saldo de uma conta em Credenciais.
        // Chamado após cada operação bancária (levantamento, depósito, transferência, etc.).
        // ------------------------------------------------------------------
        public bool fAtualizarSaldo(int contaId, decimal novoSaldo)
        {
            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id",    contaId);
            oCmd.Parameters.AddWithValue("@Saldo", novoSaldo);
            oCmd.CommandText = "UPDATE Credenciais SET Saldo = @Saldo WHERE Id = @Id";

            return fExecutarNonQuery(oCmd, "Erro ao atualizar saldo: ");
        }

        // ------------------------------------------------------------------
        // Insere um novo registo em Movimentos.
        // Chamado após cada operação bancária — registo imutável (só INSERT).
        // Tipo: 'D' depósito | 'L' levantamento | 'T' transferência | 'P' pagamento | 'M' MBWay
        // ------------------------------------------------------------------
        public bool fInserirMovimento(int contaId, string tipo, decimal valor, decimal saldoApos, string descricao)
        {
            using var oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@ContaId",   contaId);
            oCmd.Parameters.AddWithValue("@Tipo",      tipo);
            oCmd.Parameters.AddWithValue("@Valor",     valor);
            oCmd.Parameters.AddWithValue("@SaldoApos", saldoApos);
            oCmd.Parameters.AddWithValue("@Descricao", descricao);
            // DataHora é preenchida automaticamente pela BD (DEFAULT CURRENT_TIMESTAMP)
            oCmd.CommandText = "INSERT INTO Movimentos (ContaId, Tipo, Valor, SaldoApos, Descricao) VALUES (@ContaId, @Tipo, @Valor, @SaldoApos, @Descricao)";

            return fExecutarNonQuery(oCmd, "Erro ao registar movimento: ");
        }
    }
}
