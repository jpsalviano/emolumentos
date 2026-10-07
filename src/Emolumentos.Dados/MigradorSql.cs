using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Emolumentos.Dados
{
    /// <summary>
    /// Aplica as migrations em SQL embutidas nesta DLL, na ordem do nome do arquivo.
    /// Não há tabela de controle: todo script é idempotente, então todos rodam sempre.
    /// </summary>
    public sealed class MigradorSql
    {
        private readonly Banco _banco;

        public MigradorSql(Banco banco)
        {
            _banco = banco ?? throw new ArgumentNullException(nameof(banco));
        }

        /// <summary>Nomes dos scripts do banco, na ordem em que são aplicados.</summary>
        public static IReadOnlyList<string> Scripts(TipoBanco tipo)
        {
            string prefixo = Pasta(tipo) + "/";
            return typeof(MigradorSql).Assembly.GetManifestResourceNames()
                .Where(nome => nome.StartsWith(prefixo, StringComparison.Ordinal))
                .OrderBy(nome => nome, StringComparer.Ordinal)
                .ToList();
        }

        public static string LerScript(string nome)
        {
            Assembly assembly = typeof(MigradorSql).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(nome))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Migration não encontrada: " + nome);
                }

                using (var leitor = new StreamReader(stream))
                {
                    return leitor.ReadToEnd();
                }
            }
        }

        /// <returns>Os scripts aplicados.</returns>
        public IReadOnlyList<string> Aplicar()
        {
            IReadOnlyList<string> scripts = Scripts(_banco.Tipo);
            using (DbConnection conexao = _banco.CriarConexao())
            {
                conexao.Open();
                foreach (string script in scripts)
                {
                    // Cada script em sua transação: ou entra inteiro, ou não entra.
                    using (DbTransaction transacao = conexao.BeginTransaction())
                    using (DbCommand comando = conexao.CreateCommand())
                    {
                        comando.Transaction = transacao;
                        comando.CommandText = LerScript(script);
                        comando.ExecuteNonQuery();
                        transacao.Commit();
                    }
                }
            }

            return scripts;
        }

        private static string Pasta(TipoBanco tipo)
        {
            return tipo == TipoBanco.PostgreSql ? "postgresql" : "sqlserver";
        }
    }
}
