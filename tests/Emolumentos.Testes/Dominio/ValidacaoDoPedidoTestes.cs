using Emolumentos.Dominio;

namespace Emolumentos.Testes.Dominio;

/// <summary>Regras da classe base, conferidas em todas as UFs do catálogo.</summary>
public class ValidacaoDoPedidoTestes
{
    private static IEnumerable<ITabelaEmolumentos> Tabelas => new CatalogoTabelas().Todas;

    [TestCaseSource(nameof(Tabelas))]
    public void Quantidade_menor_que_um_e_recusada(ITabelaEmolumentos tabela)
    {
        Assert.Throws<PedidoInvalidoException>(
            () => tabela.Calcular(new PedidoCalculo(TipoAto.ReconhecimentoFirma, 0)));
    }

    [TestCaseSource(nameof(Tabelas))]
    public void Escritura_com_valor_exige_valor_declarado(ITabelaEmolumentos tabela)
    {
        Assert.Throws<PedidoInvalidoException>(
            () => tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor)));
    }

    [TestCaseSource(nameof(Tabelas))]
    public void Ato_sem_valor_recusa_valor_declarado(ITabelaEmolumentos tabela)
    {
        Assert.Throws<PedidoInvalidoException>(
            () => tabela.Calcular(new PedidoCalculo(TipoAto.Procuracao, 1, 1000m)));
    }

    [TestCaseSource(nameof(Tabelas))]
    public void Todo_ato_e_calculado_em_toda_uf(ITabelaEmolumentos tabela)
    {
        foreach (TipoAto ato in Enum.GetValues<TipoAto>())
        {
            decimal valor = ato == TipoAto.EscrituraComValor ? 50000m : 0m;
            ResultadoCalculo resultado = tabela.Calcular(new PedidoCalculo(ato, 2, valor));

            Assert.That(resultado.Total, Is.EqualTo(resultado.TotalUnitario * 2));
            Assert.That(resultado.Fundamento, Is.EqualTo(tabela.Fundamento));
        }
    }

    [Test]
    public void Catalogo_tem_uma_tabela_por_uf()
    {
        var catalogo = new CatalogoTabelas();

        Assert.That(catalogo.Todas.Select(t => t.Uf), Is.EquivalentTo(Enum.GetValues<Uf>()));
        Assert.That(catalogo.Obter(Uf.MG), Is.InstanceOf<TabelaMinasGerais>());
    }
}
