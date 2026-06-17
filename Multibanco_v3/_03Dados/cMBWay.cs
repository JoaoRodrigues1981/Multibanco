// Criado esta classe a 11/06/2026 por João Rodrigues
// decisão de criar um form para MBWay igual ao de Pagamento de Serviços, mas com lista de contactos de contas com MBWay = True

// Esta classe trata do acesso aos dados para operações MBWay.
// É chamada pelo cControlo (nunca diretamente pelos formulários)

using System;
using System.Collections.Generic;  // Para usar List<> de acordo com IA é aconselhado
using Npgsql;

namespace Multibanco._03Dados
{
    internal class cMBWay
    {
        public bool operacao = true; // true = operação bem sucedida, false = operação falhada como temos feito sempre
        public string mensagem = "";

        // Cada entrada: [Cliente, Telefone, Conta] --> Conta pq depois fMBWay precisa do número da conta para fazer a transferência
        // Lista de contas com MBWay, vazia inicialmente, preenchida pelo método ListarContasMBWay()
        public List<string[]> listaContasMBWay = new List<string[]>(); 

        cConexao oConexao = new cConexao(); // chamamos a classe de conexão para aceder à base de dados

        // Devolve as contas com MBWay ativo, excluindo a conta do utilizador logado.
        public bool fListarContasMBWay(int contaOrigem)
        {
            listaContasMBWay.Clear(); // Garante que a lista esteja vazia antes de preencher
            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@ContaOrigem", contaOrigem); // Parâmetro para excluir a conta do utilizador na query seguinte
            oCmd.CommandText = "SELECT Cliente, Telefone, Conta FROM Credenciais WHERE MBWay = TRUE AND Id != @ContaOrigem ORDER BY Cliente"; //excluimos a conta do utilizador

            // Tentamos executar a query e preencher a lista
            try
            {
                oCmd.Connection = oConexao.conectar();
                NpgsqlDataReader oReader = oCmd.ExecuteReader();

                while (oReader.Read())
                {
                    listaContasMBWay.Add(new string[] // Adiciona cada conta encontrada à lista, com Id, Cliente e Telefone
                    {
                        oReader.GetString(0),           // Cliente
                        oReader.GetString(1),            // Telefone
                        oReader.GetInt32(2).ToString() // conta
                    });
                }
                oReader.Close();
                oConexao.desConectar();
                oCmd.Dispose();

                mensagem = listaContasMBWay.Count + " conta(s) MBWay disponível(eis)."; 
            }

            // Em caso de erro, definimos operacao como false e guardamos a mensagem de erro
            catch (Exception ex) 
            {
                operacao = false;
                mensagem = "Erro ao carregar contas MBWay: " + ex.Message;
            }

            return operacao;
        }
    }

}