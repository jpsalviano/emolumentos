using Emolumentos.Dominio;

namespace Emolumentos.Testes.Dominio;

public class TabelaMinasGeraisTestes
{
    private readonly TabelaMinasGerais _tabela = new();

    // Coluna "Valor Final ao Usuário" da Tabela 1 publicada pelo TJMG para 2026.
    [TestCase(TipoAto.ReconhecimentoFirma, "5.a", 11.21)]
    [TestCase(TipoAto.AutenticacaoCopia, "3", 11.21)]
    [TestCase(TipoAto.Procuracao, "4.f.1", 68.94)]
    [TestCase(TipoAto.EscrituraSemValor, "4.a", 72.90)]
    public void Ato_de_valor_fixo_confere_com_o_valor_final_publicado(TipoAto ato, string item, decimal valorFinal)
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(ato));

        Assert.That(resultado.CodigoAto, Is.EqualTo(item));
        Assert.That(resultado.Total, Is.EqualTo(valorFinal));
    }

    [TestCase(1400, 220.55)]
    [TestCase(2720, 359.76)]
    [TestCase(5440, 521.35)]
    [TestCase(7000, 721.75)]
    [TestCase(14000, 962.47)]
    [TestCase(28000, 1243.47)]
    [TestCase(42000, 1564.07)]
    [TestCase(56000, 1925.31)]
    [TestCase(70000, 2326.51)]
    [TestCase(105000, 2928.06)]
    [TestCase(140000, 3721.52)]
    [TestCase(175000, 3979.69)]
    [TestCase(210000, 4238.32)]
    [TestCase(280000, 4772.07)]
    [TestCase(350000, 4903.53)]
    [TestCase(420000, 5035.59)]
    [TestCase(560000, 5523.14)]
    [TestCase(700000, 5826.70)]
    [TestCase(840000, 6130.87)]
    [TestCase(1120000, 6866.53)]
    [TestCase(1400000, 7437.64)]
    [TestCase(1680000, 8009.72)]
    [TestCase(3200000, 8582.97)]
    public void Escritura_no_limite_da_faixa_confere_com_o_valor_final_publicado(decimal limite, decimal valorFinal)
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, limite));

        Assert.That(resultado.Total, Is.EqualTo(valorFinal));
    }

    [Test]
    public void Um_centavo_acima_do_limite_cai_na_faixa_seguinte()
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, 1400.01m));

        Assert.That(resultado.Total, Is.EqualTo(359.76m));
    }

    [Test]
    public void As_faixas_estao_em_ordem_crescente_de_limite_e_de_emolumento()
    {
        Assert.That(TabelaMinasGerais.Faixas.Select(f => f.Limite), Is.Ordered.Ascending);
        Assert.That(TabelaMinasGerais.Faixas.Select(f => f.Emolumento), Is.Ordered.Ascending);
    }

    // Nota XXV: 4.844,02 + 3.289,90 na primeira faixa excedente e 2.193,27 em cada seguinte; TFJ fixa.
    [TestCase(3200000.01, 8133.92)]
    [TestCase(3700000, 8133.92)]
    [TestCase(3700000.01, 10327.19)]
    [TestCase(4700000, 12520.46)]
    public void Acima_da_ultima_faixa_acresce_por_faixa_de_quinhentos_mil(decimal valor, decimal emolumentos)
    {
        ResultadoCalculo resultado = _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, valor));

        Assert.That(resultado.ParcelasUnitarias[0].Valor, Is.EqualTo(emolumentos));
        Assert.That(resultado.ParcelasUnitarias[1].Valor, Is.EqualTo(4673.83m));
    }

    [Test]
    public void Faixas_excedentes_param_em_cem()
    {
        var emCem = new PedidoCalculo(TipoAto.EscrituraComValor, 1, 3200000m + 100 * 500000m);
        var alem = new PedidoCalculo(TipoAto.EscrituraComValor, 1, 900000000m);

        Assert.That(_tabela.Calcular(alem).Total, Is.EqualTo(_tabela.Calcular(emCem).Total));
    }

    // Faixa até 280.000,00: emolumentos 3.070,72 e TFJ 1.701,35.
    [TestCase(ReducaoLegal.FinanciamentoHabitacional, 1535.36, 850.68)]
    [TestCase(ReducaoLegal.SistemaFinanceiroHabitacao, 614.14, 340.27)]
    public void Reducao_legal_incide_sobre_emolumentos_e_taxa(ReducaoLegal reducao, decimal emolumentos,
        decimal tfj)
    {
        ResultadoCalculo resultado =
            _tabela.Calcular(new PedidoCalculo(TipoAto.EscrituraComValor, 1, 250000m, reducao));

        Assert.That(resultado.ParcelasUnitarias[0].Valor, Is.EqualTo(emolumentos));
        Assert.That(resultado.ParcelasUnitarias[1].Valor, Is.EqualTo(tfj));
    }

    [Test]
    public void Reducao_nao_se_aplica_a_ato_sem_valor()
    {
        var pedido = new PedidoCalculo(TipoAto.Procuracao, 1, 0m, ReducaoLegal.FinanciamentoHabitacional);

        Assert.Throws<PedidoInvalidoException>(() => _tabela.Calcular(pedido));
    }
}
