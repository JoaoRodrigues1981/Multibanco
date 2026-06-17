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
        // Devolve todos os clientes e contas registados em Credenciais.
        // Resultado guardado em listaCredenciais para o formulário apresentar.
        // ------------------------------------------------------------------
        public bool fListarCredenciais()
        {
            listaCredenciais.Clear(); // limpar resultados de chamadas anteriores

            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.CommandText = "SELECT Id, Banco, Cliente, Conta, Saldo, MBWay, Bloqueada FROM Credenciais ORDER BY Cliente, Conta";

            try
            {
                oCmd.Connection = oConexao.conectar();
                NpgsqlDataReader oReader = oCmd.ExecuteReader();

                while (oReader.Read()) // percorrer linha a linha
                {
                    // Guardar cada linha como array de strings para o formulário apresentar
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

                oReader.Close();
                oConexao.desConectar();
                oCmd.Dispose();

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
            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Banco",   banco);
            oCmd.Parameters.AddWithValue("@Cliente", cliente);
            oCmd.Parameters.AddWithValue("@Conta",   Convert.ToInt32(conta));
            oCmd.Parameters.AddWithValue("@Pin",     Convert.ToInt32(pin));
            // Saldo inicial fixo em 100€ — definido pelo enunciado, não configurável
            oCmd.Parameters.AddWithValue("@Saldo",   100.00m);

            oCmd.CommandText = "INSERT INTO Credenciais (Banco, Cliente, Conta, Pin, Saldo) VALUES (@Banco, @Cliente, @Conta, @Pin, @Saldo)";

            // Erro mais comum: número de conta já existe (UNIQUE constraint)
            if (fExecutarNonQuery(oCmd, "Erro ao inserir cliente: "))
                mensagem = "Cliente inserido com sucesso. Saldo inicial: 100,00€.";

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
            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id",     id);
            oCmd.Parameters.AddWithValue("@MBWay",  novoEstado);
            oCmd.CommandText = "UPDATE Credenciais SET MBWay = @MBWay WHERE Id = @Id";

            if (fExecutarNonQuery(oCmd, "Erro ao alterar MBWay: "))
                mensagem = "MBWay " + (novoEstado ? "ativado" : "desativado") + " com sucesso.";

            return operacao;
        }

        // ------------------------------------------------------------------
        // Elimina uma conta de Credenciais pelo seu Id.
        // Só deve ser chamado após confirmar que o saldo é 0 (validação no cControlo).
        // ------------------------------------------------------------------
        public bool fEliminarCredencial(int id)
        {
            NpgsqlCommand oCmd = new NpgsqlCommand();
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
            NpgsqlCommand oCmd = new NpgsqlCommand();
            oCmd.Parameters.AddWithValue("@Id", id);
            oCmd.CommandText = "UPDATE Credenciais SET Tentativas = 0, Bloqueada = FALSE WHERE Id = @Id";

            if (fExecutarNonQuery(oCmd, "Erro ao desbloquear conta: "))
                mensagem = "Conta desbloqueada com sucesso.";

            return operacao;
        }
    }
}
