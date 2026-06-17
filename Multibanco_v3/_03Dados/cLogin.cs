using System;
using System.Windows.Forms;
// -------------------------------------

using Npgsql; // biblioteca para comunicar com PostgreSQL

namespace Multibanco._03Dados
{
    // Esta classe comunica diretamente com a base de dados. É chamada pelo cControlo, nunca diretamente pelos formulários.
    // Operações suportadas: verificar credenciais de cliente, verificar acesso como admin, alterar PIN.
    internal class cLogin
    {
        // Estas variáveis comunicam o resultado ao cControlo depois de cada operação.
        public bool   operacao = true;   // false se houve erro técnico (ex: BD offline)
        public bool   existe   = true;   // false se as credenciais não foram encontradas
        public string mensagem = "";     // mensagem a mostrar ao utilizador

        // Dados da conta encontrada, preenchidos após login com sucesso.
        // O cControlo passa estes valores ao frmMultibanco via construtor.
        public int     contaId = 0;      // Id da linha em Credenciais (chave para ligar a Movimentos)
        public decimal saldo   = 0;      // Saldo atual da conta
        public string  banco   = "";     // Banco do cliente — para mostrar no ecrã principal
        public string  cliente = "";     // Nome do cliente — para mostrar no ecrã principal
        public int     conta   = 0;      // Número de conta — para mostrar no ecrã principal

        // Objeto para enviar comandos SQL ao PostgreSQL
        NpgsqlCommand oSqlCmd = new NpgsqlCommand();

        // Objeto de ligação —> usa a classe cConexao para abrir/fechar a BD
        cConexao oConexao = new cConexao();

        // Objeto para ler os resultados que o SQL devolve (linha a linha)
        NpgsqlDataReader oSqlDReader;

        // ============================================================
        // OBTER CREDENCIAIS (CLIENTE)
        //
        // Temos de verificar se as credenciais do cliente existem na base de dados.
        // RECEBE 4 campos do formulário de login, VIA camada de Controlo
        // Q1 - verificar se a conta está bloqueda -> se sim PARA logo
        // Q2 - verificar se as credenciais existem na base de dados -> se não existe, PARA logo
        // Q3 - Se login OK: reset de contador de tentativas
        // Q4 - se login falhado: incrementar contador de tentativas e bloquear conta se nº tentativas = 3
        //
        // ============================================================
        public bool fObterCredenciais(string txtBanco, string txtCliente, string txtConta, string txtPin)
        {
            // Q1 — verificar bloqueio antes de tentar qualquer credencial

            NpgsqlCommand oCmdBlq = new NpgsqlCommand();
            oCmdBlq.Parameters.AddWithValue("@Conta", Convert.ToInt32(txtConta));
            oCmdBlq.CommandText = "SELECT Bloqueada FROM Credenciais WHERE Conta = @Conta";

            try
            {
                oCmdBlq.Connection = oConexao.conectar(); // abrir ligação
                NpgsqlDataReader oVerificarBloqueado = oCmdBlq.ExecuteReader(); // Executa o comando SQL

                if (oVerificarBloqueado.Read() && oVerificarBloqueado.GetBoolean(0))
                // Read() avança para a primeira linha; GetBoolean(0) lê a coluna Bloqueada
                // Se ambos forem TRUE -> conta existe E está bloqueada LOGO para aqui
                {
                    oVerificarBloqueado.Close();
                    oConexao.desConectar();
                    oCmdBlq.Dispose();
                    operacao = false; // passamos para o cControlo que a operacao = False
                    // existe = false; // ?? nao deveria ser existe = True, pq em cima a condição era existe e está bloqueada, certo?
                    mensagem = "Conta bloqueada após 3 tentativas. Contacte o administrador.";
                    return operacao;
                }

                oVerificarBloqueado.Close();
                oConexao.desConectar();
                oCmdBlq.Dispose();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro de acesso à base de dados: " + ex.Message;
                return operacao;
            }

            // Q2 — Existe e Não está bloqueada, vamos validar as 4 credenciais
            // @Param evita SQL Injection: os valores são tratados como dados, nunca como comandos SQL

            oSqlCmd.Parameters.Clear();
            oSqlCmd.Parameters.AddWithValue("@Banco",   txtBanco);
            oSqlCmd.Parameters.AddWithValue("@Cliente", txtCliente);
            oSqlCmd.Parameters.AddWithValue("@Conta",   Convert.ToInt32(txtConta)); // texto → número inteiro
            oSqlCmd.Parameters.AddWithValue("@Pin",     Convert.ToInt32(txtPin));   // texto → número inteiro

            // Buscar os dados da conta para passar ao cControl para este passar ao ecrã principal (Form1.cs 01) após login bem-sucedido
            oSqlCmd.CommandText = "SELECT Id, Saldo, Banco, Cliente, Conta FROM Credenciais WHERE Banco = @Banco AND Cliente = @Cliente AND Conta = @Conta AND Pin = @Pin";

            try // "tentar fazer" — se algo correr mal, cai no CATCH
            {

                oSqlCmd.Connection = oConexao.conectar(); // PRIMEIRO temos de abrir ligação
                oSqlDReader = oSqlCmd.ExecuteReader();    // DEPOIS os comando SQL: executar o SELECT

                if (oSqlDReader.HasRows) // Se encontrou pelo menos uma linha → credenciais válidas
                {
                    oSqlDReader.Read();
                    contaId  = oSqlDReader.GetInt32(0);   // coluna 0: Id
                    saldo    = oSqlDReader.GetDecimal(1); // coluna 1: Saldo
                    banco    = oSqlDReader.GetString(2);  // coluna 2: Banco
                    cliente  = oSqlDReader.GetString(3);  // coluna 3: Cliente
                    conta    = oSqlDReader.GetInt32(4);   // coluna 4: Conta
                    mensagem = "Credenciais válidas. Bem-vindo ao Multibanco!";
                    // como inicializamos as variaveis de control a True, não precisamos de as alterar.
                }
                else // não encontrou nenhuma linha → credenciais erradas
                {
                    existe = false; // temos de passar as variaveis de control a False para o cControlo saber que houve um problema
                    operacao = false;
                    mensagem = "Credenciais inválidas. Tente novamente.";
                }

                oSqlDReader.Close();    // fechar o leitor de resultados
                oConexao.desConectar(); // fechar ligação (boa prática: fechar sempre após usar)
                oSqlCmd.Dispose();      // limpar o comando SQL

                // TERCEIRO - vamos gerir as tentativas de login

                // Q3 — login OK: anular o contador (evita bloqueio por tentativas anteriores)
                if (operacao) // So fazemos isto se a operacao chegou aqui = TRUE
                {
                    NpgsqlCommand oCmdReset = new NpgsqlCommand();
                    oCmdReset.Parameters.AddWithValue("@Id", contaId);
                    oCmdReset.CommandText = "UPDATE Credenciais SET Tentativas = 0 WHERE Id = @Id";
                    oCmdReset.Connection = oConexao.conectar();
                    oCmdReset.ExecuteNonQuery();
                    oConexao.desConectar();
                    oCmdReset.Dispose();
                }

                // Passo 4 — se login falhado: incrementar tentativas e bloquear se chegar a 3
                if (!operacao) // fazemos isto se a operacao chegou aqui = FALSE
                {
                    NpgsqlCommand oCmdTentativas = new NpgsqlCommand();
                    oCmdTentativas.Parameters.AddWithValue("@Conta", Convert.ToInt32(txtConta));
                    oCmdTentativas.CommandText = "UPDATE Credenciais SET Tentativas = Tentativas + 1 WHERE Conta = @Conta";
                    oCmdTentativas.Connection = oConexao.conectar();
                    oCmdTentativas.ExecuteNonQuery();
                    oConexao.desConectar();
                    oCmdTentativas.Dispose();
                    // Agora vamos verificar se a conta chegou a 3 tentativas
                    NpgsqlCommand oCmdVerificarTentativas = new NpgsqlCommand();
                    oCmdVerificarTentativas.Parameters.AddWithValue("@Conta", Convert.ToInt32(txtConta));
                    oCmdVerificarTentativas.CommandText = "SELECT Tentativas FROM Credenciais WHERE Conta = @Conta";
                    oCmdVerificarTentativas.Connection = oConexao.conectar();
                    NpgsqlDataReader oReaderTentativas = oCmdVerificarTentativas.ExecuteReader();
                    if (oReaderTentativas.Read())
                    {
                        int tentativas = oReaderTentativas.GetInt32(0);
                        if (tentativas >= 3)
                        {
                            // Bloquear a conta
                            NpgsqlCommand oCmdBloquear = new NpgsqlCommand();
                            oCmdBloquear.Parameters.AddWithValue("@Conta", Convert.ToInt32(txtConta));
                            oCmdBloquear.CommandText = "UPDATE Credenciais SET Bloqueada = TRUE WHERE Conta = @Conta";
                            oCmdBloquear.Connection = oConexao.conectar();
                            oCmdBloquear.ExecuteNonQuery();
                            oConexao.desConectar();
                            oCmdBloquear.Dispose();
                            mensagem = "Conta bloqueada após 3 tentativas. Contacte o administrador.";
                        }
                        else
                        {
                            mensagem += $" Tentativa {tentativas} de 3.";
                        }
                    }
                    oReaderTentativas.Close();
                    oConexao.desConectar();
                    oCmdVerificarTentativas.Dispose();
                }
                {
                    NpgsqlCommand oCmdTent = new NpgsqlCommand();
                    oCmdTent.Parameters.AddWithValue("@Conta", Convert.ToInt32(txtConta));
                    oCmdTent.CommandText = @"UPDATE Credenciais 
                                                    SET Tentativas = Tentativas + 1,
                                                    Bloqueada  = CASE WHEN Tentativas + 1 >= 3 THEN TRUE ELSE Bloqueada END 
                                                    WHERE Conta = @Conta";
                    oCmdTent.Connection = oConexao.conectar();
                    oCmdTent.ExecuteNonQuery();
                    oConexao.desConectar();
                    oCmdTent.Dispose();
                }
            }
            catch (Exception ex) // erro técnico: BD não responde, password errada, etc.
            {
                operacao = false;
                mensagem = "Erro de acesso à base de dados: " + ex.Message;
            }

            return operacao; // true = login bem-sucedido; false = bloqueada, credenciais erradas ou erro técnico
        }

        // ============================================================
        // OBTER CREDENCIAIS (SIBS - Backoffice)
        //
        // Temos de verifica as credenciais do administrador "sibs" na tabela Admins.
        // O login do sibs é separado — admin não tem conta bancária em Credenciais.
        // Devolve true se o username e PIN existem, false caso contrário.
        // ------------------------------------------------------------------
        public bool fValidarAdmin(string username, string pin)
        {
            oSqlCmd.Parameters.Clear();
            oSqlCmd.Parameters.AddWithValue("@Username", username);
            oSqlCmd.Parameters.AddWithValue("@Pin",      Convert.ToInt32(pin));

            oSqlCmd.CommandText = "SELECT Id FROM Admins WHERE Username = @Username AND Pin = @Pin";

            try
            {
                oSqlCmd.Connection = oConexao.conectar();
                oSqlDReader = oSqlCmd.ExecuteReader();

                if (oSqlDReader.HasRows) // admin encontrado → credenciais válidas
                {
                    mensagem = "Administrador autenticado.";
                }
                else // admin não encontrado → credenciais erradas
                {
                    existe   = false;
                    operacao = false;
                    mensagem = "Credenciais de administrador inválidas.";
                }

                oSqlDReader.Close();
                oConexao.desConectar();
                oSqlCmd.Dispose();
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro de acesso à base de dados: " + ex.Message;
            }

            return operacao;
        }

        // ============================================================
        // ALTERAR PIN (CLIENTE)
        //
        // Atualiza o PIN do utilizador na base de dados.
        // Recebe os dados de identificação + o PIN antigo (para confirmar) + o novo PIN.
        // O oldPin é incluído no WHERE como verificação extra de segurança —
        // só atualiza se o utilizador souber o PIN atual.
        // ------------------------------------------------------------------
        public bool fGravarNovoPin(string txtBanco, string txtCliente, string txtConta, string oldPin, string novoPin)
        {
            oSqlCmd.Parameters.Clear();
            // O WHERE inclui o PIN antigo como verificação extra de segurança:
            // só atualiza se o utilizador souber o PIN atual.
            oSqlCmd.Parameters.AddWithValue("@Banco",   txtBanco);
            oSqlCmd.Parameters.AddWithValue("@Cliente", txtCliente);
            oSqlCmd.Parameters.AddWithValue("@Conta",   Convert.ToInt32(txtConta));
            oSqlCmd.Parameters.AddWithValue("@oldPin",  Convert.ToInt32(oldPin));
            oSqlCmd.Parameters.AddWithValue("@novoPin", Convert.ToInt32(novoPin));

            // Atualiza apenas a linha onde todos os campos batem certo
            oSqlCmd.CommandText = "UPDATE Credenciais SET Pin = @novoPin WHERE Banco = @Banco AND Cliente = @Cliente AND Conta = @Conta AND Pin = @oldPin";

            try
            {
                oSqlCmd.Connection = oConexao.conectar(); // abrir ligação
                oSqlCmd.ExecuteNonQuery();                // UPDATE não devolve linhas — usar ExecuteNonQuery
                oConexao.desConectar();                   // fechar ligação
                oSqlCmd.Dispose();                        // limpar o comando SQL

                mensagem = "PIN alterado com sucesso!";
                operacao = true;
            }
            catch (Exception erro) // erro técnico: BD não responde, violação de constraints, etc.
            {
                operacao = false;
                mensagem = "Erro na BD: " + erro.Message;
            }

            return operacao;
        }
    }

}
