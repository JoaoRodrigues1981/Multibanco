using System;
using System.Collections.Generic;
using Npgsql;

namespace Multibanco._03Dados
{
    // Acesso à tabela Servicos — lista de serviços pré-definidos para pagamentos.
    internal class cServico
    {
        public bool   operacao = true;
        public string mensagem = "";

        // Cada entrada: [Id, Nome, Entidade]
        public List<string[]> listaServicos = new List<string[]>();

        cConexao oConexao = new cConexao();

        public bool fListarServicos()
        {
            listaServicos.Clear();

            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.CommandText = "SELECT Id, Nome, Entidade FROM servicos ORDER BY Nome";

            try
            {
                oCmd.Connection = oConexao.conectar();
                NpgsqlDataReader oReader = oCmd.ExecuteReader();

                while (oReader.Read())
                {
                    listaServicos.Add(new string[]
                    {
                        oReader.GetInt32(0).ToString(), // Id
                        oReader.GetString(1),           // Nome
                        oReader.GetString(2)            // Entidade
                    });
                }

                oReader.Close();
                oConexao.desConectar();
                oCmd.Dispose();

                mensagem = listaServicos.Count + " serviço(s) carregado(s).";
            }
            catch (Exception ex)
            {
                operacao = false;
                mensagem = "Erro ao carregar serviços: " + ex.Message;
            }

            return operacao;
        }
    }
}
