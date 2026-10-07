using Emolumentos.Dominio;

namespace Emolumentos.Testes.Dominio;

public class TabelaCearaTestes
{
    private readonly TabelaCeara _tabela = new();

    // Coluna TOTAL da Tabela II publicada pelo TJCE para 2026.
    [TestCase(TipoAto.ReconhecimentoFirma, "002001", 6.44)]
    [TestCase(TipoAto.AutenticacaoCopia, "002002", 3.68)]
    [TestCase(TipoAto.Procuracao, "002003", 64.90)]
    [TestCase(TipoAto.EscrituraSemValor, "002007", 120.10)]
    public void Ato_de_valor_fixo_confere_com_o_total_publicado(TipoAto ato, string codigo, decimal total)
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(ato));

        Assert.That(resultado.CodigoAto, Is.EqualTo(codigo));
        Assert.That(resultado.Total, Is.EqualTo(total));
    }

    [TestCase("002008", 104.00, 150.41)]
    [TestCase("002009", 235.00, 359.46)]
    [TestCase("002010", 784.00, 448.24)]
    [TestCase("002011", 2376.00, 486.68)]
    [TestCase("002012", 4684.00, 623.76)]
    [TestCase("002013", 6540.00, 670.08)]
    [TestCase("002014", 9810.00, 761.83)]
    [TestCase("002015", 18527.00, 898.91)]
    [TestCase("002016", 23322.58, 1007.31)]
    public void Escritura_no_limite_da_faixa_confere_com_o_total_publicado(string codigo, decimal limite,
        decimal total)
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, limite));

        Assert.That(resultado.CodigoAto, Is.EqualTo(codigo));
        Assert.That(resultado.Total, Is.EqualTo(total));
    }

    [Test]
    public void Um_centavo_acima_do_limite_cai_na_faixa_seguinte()
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, 104.01m));

        Assert.That(resultado.CodigoAto, Is.EqualTo("002009"));
    }

    [Test]
    public void As_faixas_estao_em_ordem_crescente_de_limite_e_de_emolumento()
    {
        Assert.That(TabelaCeara.Faixas.Select(f => f.Limite), Is.Ordered.Ascending);
        Assert.That(TabelaCeara.Faixas.Select(f => f.Emolumento), Is.Ordered.Ascending);
    }

    [Test]
    public void Acima_da_ultima_faixa_cobra_por_fracao_excedente()
    {
        // 23.322,58 + 10,98 + 0,01: duas frações de R$ 10,98 (a segunda, incompleta).
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, 23333.57m));

        Assert.That(resultado.CodigoAto, Is.EqualTo("002017"));
        Assert.That(Valor(resultado, "Emolumento"), Is.EqualTo(835.86m + 0.48m));
        // 5% do emolumento excedente (0,48) arredonda para 0,02, mais o FERMOJU da última faixa.
        Assert.That(Valor(resultado, "FERMOJU"), Is.EqualTo(49.97m + 0.02m));
        Assert.That(Valor(resultado, "FAADEP"), Is.EqualTo(41.82m));
        Assert.That(resultado.Total, Is.EqualTo(836.34m + 49.99m + 37.90m + 41.82m + 41.82m));
    }

    [Test]
    public void Emolumento_acima_da_ultima_faixa_tem_teto()
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, 50000000m));

        Assert.That(Valor(resultado, "Emolumento"), Is.EqualTo(3226.76m));
    }

    [Test]
    public void Fundos_sao_cinco_por_cento_do_emolumento()
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, 5000m));

        // 545,31 x 5% = 27,2655, publicado como 27,27.
        Assert.That(Valor(resultado, "FAADEP"), Is.EqualTo(27.27m));
        Assert.That(Valor(resultado, "FRMMP"), Is.EqualTo(27.27m));
    }

    [Test]
    public void Quantidade_multiplica_o_valor_da_unidade()
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.ReconhecimentoFirma, 3));

        Assert.That(resultado.Unidade, Is.EqualTo("firma"));
        Assert.That(resultado.Total, Is.EqualTo(19.32m));
    }

    [Test]
    public void Nao_admite_reducao_legal()
    {
        var pedido = new PedidoCalculo(TipoAto.EscrituraComValor, 1, 100000m,
            ReducaoLegal.SistemaFinanceiroHabitacao);

        Assert.Throws<PedidoInvalidoException>(() => _tabela.Calcular(pedido));
    }

    private static decimal Valor(ResultadoCalculo resultado, string parcela)
    {
        return resultado.ParcelasUnitarias.Single(p => p.Nome == parcela).Valor;
    }
}
