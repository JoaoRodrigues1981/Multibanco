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
        // Helper privado — elimina o boilerplate try/catch que se repetia em
        // fAtualizarSaldo e fInserirMovimento.
        // Centraliza abertura/fecho de ligação e tratamento de erros.
        // ------------------------------------------------------------------
        private bool fExecutarNonQuery(NpgsqlCommand oCmd, string prefixoErro)
        {
            try
            {
                oCmd.Connection = oConexao.conectar();
                oCmd.ExecuteNonQuery();
                oConexao.desConectar();
                oCmd.Dispose();
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

            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id", contaId);
            oCmd.CommandText = "SELECT Saldo FROM Credenciais WHERE Id = @Id";

            try
            {
                oCmd.Connection = oConexao.conectar();
                NpgsqlDataReader oReader = oCmd.ExecuteReader();

                if (oReader.Read())
                    saldo = oReader.GetDecimal(0);

                oReader.Close();
                oConexao.desConectar();
                oCmd.Dispose();
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

            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id", contaId);
            oCmd.CommandText = "SELECT MBWay FROM Credenciais WHERE Id = @Id";

            try
            {
                oCmd.Connection = oConexao.conectar();
                NpgsqlDataReader oReader = oCmd.ExecuteReader();

                if (oReader.Read())
                
                    mbway = oReader.GetBoolean(0);
                    //MessageBox.Show(Convert.ToString(mbway)); //control de variavel para testes

                oReader.Close();
                oConexao.desConectar();
                oCmd.Dispose();
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
            idDestino     = -1;  // valor sentinela — indica que a conta não foi encontrada
            saldoDestino  =  0;

            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Conta", contaNum);
            // Uma única query devolve Id e Saldo — evita uma segunda viagem à BD
            oCmd.CommandText = "SELECT Id, Saldo FROM Credenciais WHERE Conta = @Conta";

            try
            {
                oCmd.Connection = oConexao.conectar();
                NpgsqlDataReader oReader = oCmd.ExecuteReader();

                if (oReader.Read())
                {
                    idDestino    = oReader.GetInt32(0);   // coluna 0: Id (chave primária)
                    saldoDestino = oReader.GetDecimal(1); // coluna 1: Saldo atual
                }
                else
                {
                    // Conta com esse número não existe na BD
                    operacao = false;
                    mensagem = "Conta destino não encontrada.";
                }

                oReader.Close();
                oConexao.desConectar();
                oCmd.Dispose();
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
            NpgsqlCommand oCmd = new NpgsqlCommand();
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
            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@ContaId",   contaId);
            oCmd.Parameters.AddWithValue("@Tipo",      tipo);
            oCmd.Parameters.AddWithValue("@Valor",     valor);
            oCmd.Parameters.AddWithValue("@SaldoApos", saldoApos);
            oCmd.Parameters.AddWithValue("@Descricao", descricao);
            // DataHora é preenchida automaticamente pela BD (DEFAULT CURRENT_TIMESTAMP)
            oCmd.CommandText = "INSERT INTO Movimentos (ContaId, Tipo, Valor, SaldoApos, Descricao) VALUES (@ContaId, @Tipo, @Valor, @SaldoApos, @Descricao)";

            return fExecutarNonQuery(oCmd, "Erro ao registar movimento: ");
        }

        // ------------------------------------------------------------------
        // Devolve os movimentos de uma conta, opcionalmente filtrados por datas.
        // Se dataInicio ou dataFim forem null, não aplica esse limite.
        // Resultado guardado em listaMovimentos para o formulário apresentar.
        // ------------------------------------------------------------------
        public bool fListarMovimentos(int contaId, DateTime? dataInicio, DateTime? dataFim)
        {
            listaMovimentos.Clear();

            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@ContaId", contaId);

            // Construir a query com os filtros de data opcionais
            string sql = "SELECT Tipo, Valor, SaldoApos, DataHora, Descricao FROM Movimentos WHERE ContaId = @ContaId";

            if (dataInicio.HasValue)
            {
                sql += " AND DataHora >= @DataInicio";
                oCmd.Parameters.AddWithValue("@DataInicio", dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                sql += " AND DataHora <= @DataFim";
                oCmd.Parameters.AddWithValue("@DataFim", dataFim.Value.AddDays(1).AddSeconds(-1)); // incluir o dia inteiro
            }

            sql += " ORDER BY DataHora DESC"; // mais recente primeiro
            oCmd.CommandText = sql;

            try
            {
                oCmd.Connection = oConexao.conectar();
                NpgsqlDataReader oReader = oCmd.ExecuteReader();

                while (oReader.Read())
                {
                    listaMovimentos.Add(new string[]
                    {
                        oReader.GetString(0),                              // Tipo
                        oReader.GetDecimal(1).ToString("F2"),              // Valor
                        oReader.GetDecimal(2).ToString("F2"),              // SaldoApos
                        oReader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm"), // DataHora
                        oReader.IsDBNull(4) ? "" : oReader.GetString(4)   // Descricao (pode ser null)
                    });
                }

                oReader.Close();
                oConexao.desConectar();
                oCmd.Dispose();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro ao listar movimentos: " + ex.Message;
            }

            return operacao;
        }
    }
}
