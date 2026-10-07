using System.Data.Common;
using Emolumentos.Dados;
using Emolumentos.Dominio;

namespace Emolumentos.Testes.Dados;

/// <summary>
/// Testes contra bancos de verdade. Cada um roda quando a variável de ambiente com a
/// connection string do banco existe (o CI sobe SQL Server e PostgreSQL); sem ela, é ignorado.
/// </summary>
[TestFixture(TipoBanco.SqlServer, "EMOLUMENTOS_TESTE_SQLSERVER")]
[TestFixture(TipoBanco.PostgreSql, "EMOLUMENTOS_TESTE_POSTGRESQL")]
public class BancoDeDadosTestes
{
    private readonly TipoBanco _tipo;
    private readonly string _variavel;
    private Banco _banco = null!;

    public BancoDeDadosTestes(TipoBanco tipo, string variavel)
    {
        _tipo = tipo;
        _variavel = variavel;
    }

    [OneTimeSetUp]
    public void Conectar()
    {
        string? connectionString = Environment.GetEnvironmentVariable(_variavel);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Assert.Ignore($"{_variavel} não definida: sem banco {_tipo} para testar.");
        }

        _banco = new Banco(_tipo, connectionString);
        Executar("DROP TABLE IF EXISTS emolumentos.calculos");
    }

    [Test, Order(1)]
    public void Migrations_criam_o_esquema_em_banco_vazio()
    {
        new MigradorSql(_banco).Aplicar();

        Assert.That(Colunas(), Is.EquivalentTo(new[]
        {
            "id", "criado_em", "uf", "ato", "codigo_ato", "quantidade", "valor_declarado", "total", "origem"
        }));
    }

    [Test, Order(2)]
    public void Migrations_sao_idempotentes()
    {
        var historico = new HistoricoCalculosEf(_banco);
        historico.Registrar(Calculo(Uf.CE, TipoAto.Procuracao, 0m, new DateTime(2026, 10, 1, 12, 0, 0)));

        // Segunda e terceira passadas, agora com dados: nada falha, nada se perde.
        new MigradorSql(_banco).Aplicar();
        new MigradorSql(_banco).Aplicar();

        Assert.That(historico.Recentes(10), Has.Count.EqualTo(1));
        Assert.That(Colunas(), Has.Count.EqualTo(9));
    }

    [Test, Order(3)]
    public void Historico_grava_e_devolve_do_mais_recente_para_o_mais_antigo()
    {
        var historico = new HistoricoCalculosEf(_banco);
        var momento = new DateTime(2026, 10, 7, 15, 30, 0, DateTimeKind.Utc);
        historico.Registrar(Calculo(Uf.MG, TipoAto.EscrituraComValor, 250000m, momento));

        IReadOnlyList<CalculoRegistrado> recentes = historico.Recentes(10);

        Assert.That(recentes.Select(c => c.Uf), Is.EqualTo(new[] { "MG", "CE" }));
        CalculoRegistrado lido = recentes[0];
        Assert.That(lido.Id, Is.GreaterThan(0));
        Assert.That(lido.CriadoEm, Is.EqualTo(momento));
        Assert.That(lido.CriadoEm.Kind, Is.EqualTo(DateTimeKind.Utc));
        Assert.That(lido.Ato, Is.EqualTo("EscrituraComValor"));
        Assert.That(lido.CodigoAto, Is.EqualTo("4.b"));
        Assert.That(lido.ValorDeclarado, Is.EqualTo(250000m));
        Assert.That(lido.Total, Is.EqualTo(4772.07m));
        Assert.That(lido.Origem, Is.EqualTo("teste"));
        Assert.That(historico.Recentes(1), Has.Count.EqualTo(1));
    }

    private static CalculoRegistrado Calculo(Uf uf, TipoAto ato, decimal valor, DateTime momento)
    {
        ResultadoCalculo resultado = new CatalogoTabelas().Obter(uf).Calcular(new PedidoCalculo(ato, 1, valor));
        return CalculoRegistrado.De(resultado, "teste", DateTime.SpecifyKind(momento, DateTimeKind.Utc));
    }

    private List<string> Colunas()
    {
        const string sql = "SELECT column_name FROM information_schema.columns "
            + "WHERE table_schema = 'emolumentos' AND table_name = 'calculos'";
        var colunas = new List<string>();
        using DbConnection conexao = _banco.CriarConexao();
        conexao.Open();
        using DbCommand comando = conexao.CreateCommand();
        comando.CommandText = sql;
        using DbDataReader leitor = comando.ExecuteReader();
        while (leitor.Read())
        {
            colunas.Add(leitor.GetString(0));
        }

        return colunas;
    }

    private void Executar(string sql)
    {
        using DbConnection conexao = _banco.CriarConexao();
        conexao.Open();
        using DbCommand comando = conexao.CreateCommand();
        comando.CommandText = sql;
        comando.ExecuteNonQuery();
    }
}
