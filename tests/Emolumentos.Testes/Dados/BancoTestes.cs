using Emolumentos.Dados;
using Npgsql;

namespace Emolumentos.Testes.Dados;

public class BancoTestes
{
    [TestCase("postgresql://app:s3nha@postgres:5433/emolumentos", 5433)]
    [TestCase("postgresql+asyncpg://app:s3nha@postgres/emolumentos", 5432)]
    [TestCase("postgres://app:s3nha@postgres:5432/emolumentos", 5432)]
    public void Url_de_postgres_vira_connection_string(string url, int porta)
    {
        Banco banco = Banco.DaUrlPostgres(url);

        var conexao = new NpgsqlConnectionStringBuilder(banco.ConnectionString);
        Assert.That(banco.Tipo, Is.EqualTo(TipoBanco.PostgreSql));
        Assert.That(conexao.Host, Is.EqualTo("postgres"));
        Assert.That(conexao.Port, Is.EqualTo(porta));
        Assert.That(conexao.Database, Is.EqualTo("emolumentos"));
        Assert.That(conexao.Username, Is.EqualTo("app"));
        Assert.That(conexao.Password, Is.EqualTo("s3nha"));
    }

    [Test]
    public void Senha_com_caracteres_escapados_e_decodificada()
    {
        Banco banco = Banco.DaUrlPostgres("postgresql://app:a%40b%3Ac@host/db");

        Assert.That(new NpgsqlConnectionStringBuilder(banco.ConnectionString).Password, Is.EqualTo("a@b:c"));
    }

    [TestCase("")]
    [TestCase("Host=localhost;Database=x")]
    [TestCase("https://exemplo.com/banco")]
    public void Url_invalida_falha_sem_repetir_o_texto(string url)
    {
        var erro = Assert.Throws<FormatException>(() => Banco.DaUrlPostgres(url));

        Assert.That(erro!.Message, Is.EqualTo("URL de PostgreSQL inválida."));
    }

    [TestCase("Npgsql", TipoBanco.PostgreSql)]
    [TestCase("System.Data.SqlClient", TipoBanco.SqlServer)]
    [TestCase("", TipoBanco.SqlServer)]
    public void Provider_do_app_config_escolhe_o_banco(string provider, TipoBanco esperado)
    {
        Assert.That(Banco.DoProvider(provider, "Server=x").Tipo, Is.EqualTo(esperado));
    }
}
