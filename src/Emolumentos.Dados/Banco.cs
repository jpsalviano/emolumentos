using System;
using System.Data.Common;
using System.Data.SqlClient;
using Npgsql;

namespace Emolumentos.Dados
{
    public enum TipoBanco
    {
        SqlServer,
        PostgreSql
    }

    /// <summary>Um banco configurado: qual é e como abrir conexão com ele.</summary>
    public sealed class Banco
    {
        public Banco(TipoBanco tipo, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string vazia.", nameof(connectionString));
            }

            Tipo = tipo;
            ConnectionString = connectionString;
        }

        public TipoBanco Tipo { get; }

        public string ConnectionString { get; }

        /// <summary>Reconhece o banco pelo nome do provider ADO.NET, como no App.config.</summary>
        public static Banco DoProvider(string providerName, string connectionString)
        {
            bool postgres = string.Equals(providerName, "Npgsql", StringComparison.OrdinalIgnoreCase);
            return new Banco(postgres ? TipoBanco.PostgreSql : TipoBanco.SqlServer, connectionString);
        }

        /// <summary>
        /// Lê uma URL de PostgreSQL no formato de variável de ambiente,
        /// "postgresql://usuario:senha@host:5432/banco". Um driver no esquema
        /// ("postgresql+asyncpg://"), comum em URLs feitas para Python, é ignorado.
        /// </summary>
        public static Banco DaUrlPostgres(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri)
                || !uri.Scheme.StartsWith("postgres", StringComparison.OrdinalIgnoreCase))
            {
                // Sem a URL na mensagem: ela carrega a senha.
                throw new FormatException("URL de PostgreSQL inválida.");
            }

            string[] credenciais = uri.UserInfo.Split(new[] { ':' }, 2);
            var conexao = new NpgsqlConnectionStringBuilder
            {
                Host = uri.Host,
                Port = uri.IsDefaultPort || uri.Port < 0 ? 5432 : uri.Port,
                Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
                Username = Uri.UnescapeDataString(credenciais[0]),
                Password = credenciais.Length > 1 ? Uri.UnescapeDataString(credenciais[1]) : null,
                Timeout = 5,
                // O papel do banco costuma ter limite de conexões; o histórico precisa de poucas.
                MaxPoolSize = 5
            };

            return new Banco(TipoBanco.PostgreSql, conexao.ConnectionString);
        }

        /// <summary>Conexão fechada; quem recebe abre e descarta.</summary>
        public DbConnection CriarConexao()
        {
            if (Tipo == TipoBanco.PostgreSql)
            {
                return new NpgsqlConnection(ConnectionString);
            }

            return new SqlConnection(ConnectionString);
        }
    }
}
