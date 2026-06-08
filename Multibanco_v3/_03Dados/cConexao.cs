using Npgsql; // biblioteca que permite ligar ao PostgreSQL

namespace Multibanco._03Dados
{
    // Esta classe trata de abrir e fechar a ligação à base de dados.
    // É usada sempre que precisamos de ir buscar ou guardar dados.
    internal class cConexao
    {
        // Objeto que representa a "ficha" de ligação ao PostgreSQL
        NpgsqlConnection oConectar = new NpgsqlConnection();

        // Construtor: corre automaticamente quando criamos um "new cConexao()"
        // Define onde está a base de dados, com que utilizador e palavra-passe
        public cConexao()
        {
            oConectar.ConnectionString = "Host=localhost;Database=lei2526;Username=postgres;Password=1234;Search Path=public";
            //                            servidor local     nome da BD      utilizador BD    password BD
        }

        // Abre a ligação e devolve-a para ser usada nos comandos SQL
        public NpgsqlConnection conectar()
        {
            oConectar.Open();
            return oConectar;
        }

        // Fecha a ligação à base de dados
        public void desConectar()
        {
            oConectar.Close();
        }
    }
}
