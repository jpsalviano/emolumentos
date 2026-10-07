using Emolumentos.Dados;

namespace Emolumentos.Testes.Dados;

/// <summary>Conferências que não precisam de banco.</summary>
public class MigrationsTestes
{
    [TestCase(TipoBanco.SqlServer)]
    [TestCase(TipoBanco.PostgreSql)]
    public void Scripts_vem_numerados_e_em_ordem(TipoBanco tipo)
    {
        IReadOnlyList<string> scripts = MigradorSql.Scripts(tipo);

        Assert.That(scripts, Is.Not.Empty);
        Assert.That(scripts, Is.Ordered.Using((IComparer<string>)StringComparer.Ordinal));
        Assert.That(scripts.Select(Numero), Is.EqualTo(Enumerable.Range(1, scripts.Count)));
    }

    [Test]
    public void Os_dois_bancos_tem_as_mesmas_migrations()
    {
        Assert.That(MigradorSql.Scripts(TipoBanco.PostgreSql).Select(Arquivo),
            Is.EqualTo(MigradorSql.Scripts(TipoBanco.SqlServer).Select(Arquivo)));
    }

    [TestCase(TipoBanco.SqlServer)]
    [TestCase(TipoBanco.PostgreSql)]
    public void Nenhum_script_esta_vazio(TipoBanco tipo)
    {
        foreach (string script in MigradorSql.Scripts(tipo))
        {
            Assert.That(MigradorSql.LerScript(script), Does.Contain("emolumentos.calculos"), script);
        }
    }

    private static string Arquivo(string script) => script[(script.IndexOf('/') + 1)..];

    private static int Numero(string script) => int.Parse(Arquivo(script)[..4]);
}
