using Emolumentos.Apresentacao;
using Emolumentos.Dominio;

namespace Emolumentos.Testes.Apresentacao;

public class CalculoPresenterTestes
{
    private static readonly DateTime Agora = new(2026, 10, 7, 15, 0, 0, DateTimeKind.Utc);

    private ViewFalsa _view = null!;
    private HistoricoEmMemoria _historico = null!;

    [SetUp]
    public void Preparar()
    {
        _view = new ViewFalsa();
        _historico = new HistoricoEmMemoria();
        new CalculoPresenter(_view, new CatalogoTabelas(), _historico, () => Agora).Iniciar();
    }

    [Test]
    public void Ao_iniciar_lista_ufs_e_atos()
    {
        Assert.That(_view.Ufs, Is.EqualTo(new[] { Uf.CE, Uf.MG }));
        Assert.That(_view.Atos, Is.EquivalentTo(Enum.GetValues<TipoAto>()));
    }

    [Test]
    public void Abre_na_escritura_com_valor_que_usa_todos_os_campos()
    {
        Assert.That(_view.AtoSelecionado, Is.EqualTo(TipoAto.EscrituraComValor));
        Assert.That(_view.ValorHabilitado, Is.True);
    }

    [Test]
    public void Valor_declarado_so_e_pedido_na_escritura_com_valor()
    {
        _view.Selecionar(Uf.CE, TipoAto.Procuracao);
        Assert.That(_view.ValorHabilitado, Is.False);

        _view.Selecionar(Uf.CE, TipoAto.EscrituraComValor);
        Assert.That(_view.ValorHabilitado, Is.True);
    }

    [Test]
    public void Orientacao_explica_cada_campo_desabilitado()
    {
        _view.Selecionar(Uf.MG, TipoAto.Procuracao);
        Assert.That(_view.Orientacao, Is.EqualTo(CalculoPresenter.OrientacaoValorFixo));

        _view.Selecionar(Uf.CE, TipoAto.EscrituraComValor);
        Assert.That(_view.Orientacao, Is.EqualTo(CalculoPresenter.OrientacaoSemReducao));

        _view.Selecionar(Uf.MG, TipoAto.EscrituraComValor);
        Assert.That(_view.Orientacao, Is.Empty);
    }

    [Test]
    public void Reducoes_seguem_a_uf_e_o_ato()
    {
        _view.Selecionar(Uf.CE, TipoAto.EscrituraComValor);
        Assert.That(_view.Reducoes, Is.EqualTo(new[] { ReducaoLegal.Nenhuma }));

        _view.Selecionar(Uf.MG, TipoAto.EscrituraComValor);
        Assert.That(_view.Reducoes, Has.Count.EqualTo(3));

        _view.Selecionar(Uf.MG, TipoAto.Procuracao);
        Assert.That(_view.Reducoes, Is.EqualTo(new[] { ReducaoLegal.Nenhuma }));
    }

    [Test]
    public void Calcula_formata_em_reais_e_grava_no_historico()
    {
        _view.Selecionar(Uf.MG, TipoAto.EscrituraComValor);
        _view.ValorDeclaradoDigitado = "250.000,00";

        _view.Calcular();

        Assert.That(_view.Erro, Is.Null);
        Assert.That(SemEspacoRigido(_view.Resultado!.Total), Is.EqualTo("R$ 4.772,07"));
        Assert.That(_view.Resultado.Parcelas.Select(p => p.Nome),
            Is.EqualTo(new[] { "Emolumentos", "Taxa de Fiscalização Judiciária" }));

        CalculoRegistrado gravado = _historico.Itens.Single();
        Assert.That(gravado.Uf, Is.EqualTo("MG"));
        Assert.That(gravado.Total, Is.EqualTo(4772.07m));
        Assert.That(gravado.ValorDeclarado, Is.EqualTo(250000m));
        Assert.That(gravado.CriadoEm, Is.EqualTo(Agora));
        Assert.That(gravado.Origem, Is.EqualTo("desktop"));
        Assert.That(_view.Historico, Has.Count.EqualTo(1));
    }

    [Test]
    public void Quantidade_multiplica_as_parcelas_exibidas()
    {
        _view.Selecionar(Uf.CE, TipoAto.ReconhecimentoFirma);
        _view.QuantidadeDigitada = "3";

        _view.Calcular();

        LinhaParcela emolumento = _view.Resultado!.Parcelas[0];
        Assert.That(SemEspacoRigido(emolumento.ValorUnitario), Is.EqualTo("R$ 4,13"));
        Assert.That(SemEspacoRigido(emolumento.Valor), Is.EqualTo("R$ 12,39"));
        Assert.That(SemEspacoRigido(_view.Resultado.Total), Is.EqualTo("R$ 19,32"));
    }

    [TestCase("abc")]
    [TestCase("")]
    [TestCase("1,5")]
    public void Quantidade_que_nao_e_inteiro_vira_erro_na_tela(string digitado)
    {
        _view.QuantidadeDigitada = digitado;

        _view.Calcular();

        Assert.That(_view.Erro, Does.Contain("quantidade"));
        Assert.That(_view.Resultado, Is.Null);
        Assert.That(_historico.Itens, Is.Empty);
    }

    [Test]
    public void Valor_declarado_ilegivel_vira_erro_na_tela()
    {
        _view.Selecionar(Uf.CE, TipoAto.EscrituraComValor);
        _view.ValorDeclaradoDigitado = "duzentos mil";

        _view.Calcular();

        Assert.That(_view.Erro, Does.Contain("valor declarado"));
    }

    [Test]
    public void Erro_do_dominio_chega_a_tela_com_a_mensagem_dele()
    {
        _view.Selecionar(Uf.CE, TipoAto.EscrituraComValor);
        _view.ValorDeclaradoDigitado = "0";

        _view.Calcular();

        Assert.That(_view.Erro, Is.EqualTo("Informe o valor declarado da escritura."));
    }

    [Test]
    public void Falha_ao_gravar_nao_esconde_o_resultado()
    {
        _historico.FalharAoRegistrar = true;
        _view.Selecionar(Uf.CE, TipoAto.ReconhecimentoFirma);

        _view.Calcular();

        Assert.That(_view.Resultado, Is.Not.Null);
        Assert.That(_view.Erro, Is.Null);
        Assert.That(_view.Aviso, Does.Contain("não foi gravado"));
    }

    [Test]
    public void Sem_banco_calcula_e_avisa_que_o_historico_esta_desligado()
    {
        var view = new ViewFalsa();
        new CalculoPresenter(view, new CatalogoTabelas(), null, () => Agora).Iniciar();
        view.Selecionar(Uf.CE, TipoAto.ReconhecimentoFirma);

        view.Calcular();

        Assert.That(view.Resultado, Is.Not.Null);
        Assert.That(view.Aviso, Does.Contain("histórico está desligado"));
    }

    // A cultura pt-BR separa "R$" do número com espaço rígido (U+00A0).
    private static string SemEspacoRigido(string texto)
    {
        return texto.Replace(' ', ' ');
    }

    private sealed class ViewFalsa : ICalculoView
    {
        public event EventHandler? SelecaoAlterada;

        public event EventHandler? CalcularSolicitado;

        public Uf UfSelecionada { get; private set; } = Uf.CE;

        public TipoAto AtoSelecionado { get; private set; } = TipoAto.ReconhecimentoFirma;

        public ReducaoLegal ReducaoSelecionada { get; set; } = ReducaoLegal.Nenhuma;

        public string QuantidadeDigitada { get; set; } = "1";

        public string ValorDeclaradoDigitado { get; set; } = "";

        public IReadOnlyList<Uf> Ufs { get; private set; } = [];

        public IReadOnlyList<TipoAto> Atos { get; private set; } = [];

        public IReadOnlyList<ReducaoLegal> Reducoes { get; private set; } = [];

        public bool ValorHabilitado { get; private set; }

        public string? Orientacao { get; private set; }

        public ResultadoExibido? Resultado { get; private set; }

        public string? Erro { get; private set; }

        public string? Aviso { get; private set; }

        public IReadOnlyList<LinhaHistorico> Historico { get; private set; } = [];

        public void Selecionar(Uf uf, TipoAto ato)
        {
            UfSelecionada = uf;
            AtoSelecionado = ato;
            SelecaoAlterada?.Invoke(this, EventArgs.Empty);
        }

        public void Calcular()
        {
            CalcularSolicitado?.Invoke(this, EventArgs.Empty);
        }

        public void ExibirUfs(IReadOnlyList<Uf> ufs) => Ufs = ufs;

        public void ExibirAtos(IReadOnlyList<TipoAto> atos) => Atos = atos;

        public void ExibirReducoes(IReadOnlyList<ReducaoLegal> reducoes) => Reducoes = reducoes;

        public void SelecionarAto(TipoAto ato) => AtoSelecionado = ato;

        public void HabilitarValorDeclarado(bool habilitado) => ValorHabilitado = habilitado;

        public void ExibirOrientacao(string texto) => Orientacao = texto;

        public void ExibirResultado(ResultadoExibido resultado) => Resultado = resultado;

        public void ExibirErro(string mensagem) => Erro = mensagem;

        public void ExibirHistorico(IReadOnlyList<LinhaHistorico> linhas) => Historico = linhas;

        public void ExibirAviso(string mensagem) => Aviso = mensagem;
    }

    private sealed class HistoricoEmMemoria : IHistoricoCalculos
    {
        public List<CalculoRegistrado> Itens { get; } = [];

        public bool FalharAoRegistrar { get; set; }

        public void Registrar(CalculoRegistrado calculo)
        {
            if (FalharAoRegistrar)
            {
                throw new InvalidOperationException("banco fora do ar");
            }

            Itens.Add(calculo);
        }

        public IReadOnlyList<CalculoRegistrado> Recentes(int quantidade)
        {
            return Itens.AsEnumerable().Reverse().Take(quantidade).ToList();
        }
    }
}
