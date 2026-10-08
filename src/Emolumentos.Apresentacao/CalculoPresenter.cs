using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Emolumentos.Dominio;

namespace Emolumentos.Apresentacao
{
    /// <summary>Toda a lógica da tela de cálculo: interpreta a entrada, chama o domínio e formata a saída.</summary>
    public sealed class CalculoPresenter
    {
        public const string Origem = "desktop";

        public const string OrientacaoValorFixo =
            "Ato de valor fixo na tabela: sem valor declarado nem redução.";

        public const string OrientacaoSemReducao = "A tabela desta UF não prevê redução legal.";

        private const int LinhasDoHistorico = 20;
        private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

        private readonly ICalculoView _view;
        private readonly CatalogoTabelas _catalogo;
        private readonly IHistoricoCalculos _historico;
        private readonly Func<DateTime> _agoraUtc;

        /// <param name="historico">Null quando a aplicação roda sem banco: só o histórico some.</param>
        public CalculoPresenter(ICalculoView view, CatalogoTabelas catalogo, IHistoricoCalculos historico,
            Func<DateTime> agoraUtc)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _catalogo = catalogo ?? throw new ArgumentNullException(nameof(catalogo));
            _historico = historico;
            _agoraUtc = agoraUtc ?? throw new ArgumentNullException(nameof(agoraUtc));

            _view.SelecaoAlterada += (sender, e) => AtualizarOpcoes();
            _view.CalcularSolicitado += (sender, e) => Calcular();
        }

        public void Iniciar()
        {
            _view.ExibirUfs(_catalogo.Todas.Select(t => t.Uf).ToList());
            _view.ExibirAtos(Enum.GetValues(typeof(TipoAto)).Cast<TipoAto>().ToList());
            // A tela abre no ato que usa todos os campos; nos demais, dois deles ficam desabilitados.
            _view.SelecionarAto(TipoAto.EscrituraComValor);
            AtualizarOpcoes();
            CarregarHistorico();
        }

        private void AtualizarOpcoes()
        {
            bool comValor = _view.AtoSelecionado == TipoAto.EscrituraComValor;
            _view.HabilitarValorDeclarado(comValor);

            // Só a escritura com valor admite redução, e cada UF tem as suas.
            List<ReducaoLegal> reducoes = comValor
                ? _catalogo.Obter(_view.UfSelecionada).ReducoesAdmitidas.ToList()
                : new List<ReducaoLegal> { ReducaoLegal.Nenhuma };
            _view.ExibirReducoes(reducoes);

            _view.ExibirOrientacao(!comValor ? OrientacaoValorFixo
                : reducoes.Count < 2 ? OrientacaoSemReducao
                : string.Empty);
        }

        private void Calcular()
        {
            ResultadoCalculo resultado;
            try
            {
                ITabelaEmolumentos tabela = _catalogo.Obter(_view.UfSelecionada);
                resultado = tabela.Calcular(LerPedido());
            }
            catch (PedidoInvalidoException ex)
            {
                _view.ExibirErro(ex.Message);
                return;
            }

            _view.ExibirResultado(Formatar(resultado));
            Registrar(resultado);
        }

        private PedidoCalculo LerPedido()
        {
            int quantidade;
            if (!int.TryParse(_view.QuantidadeDigitada, NumberStyles.Integer, Cultura, out quantidade))
            {
                throw new PedidoInvalidoException("A quantidade deve ser um número inteiro.");
            }

            decimal valor = 0m;
            if (_view.AtoSelecionado == TipoAto.EscrituraComValor
                && !decimal.TryParse(_view.ValorDeclaradoDigitado, NumberStyles.Number, Cultura, out valor))
            {
                throw new PedidoInvalidoException("O valor declarado deve ser um número, como 250.000,00.");
            }

            return new PedidoCalculo(_view.AtoSelecionado, quantidade, valor, _view.ReducaoSelecionada);
        }

        private static ResultadoExibido Formatar(ResultadoCalculo resultado)
        {
            int quantidade = resultado.Pedido.Quantidade;
            string titulo = string.Format(Cultura, "{0} ({1} {2}): {3} x {4}", resultado.Descricao,
                Rotulos.De(resultado.Uf), resultado.CodigoAto, quantidade, resultado.Unidade);

            List<LinhaParcela> parcelas = resultado.ParcelasUnitarias
                .Select(p => new LinhaParcela(p.Nome, Moeda(p.Valor), Moeda(p.Valor * quantidade)))
                .ToList();

            return new ResultadoExibido(titulo, parcelas, Moeda(resultado.Total), resultado.Fundamento);
        }

        private void Registrar(ResultadoCalculo resultado)
        {
            if (_historico == null)
            {
                return;
            }

            try
            {
                _historico.Registrar(CalculoRegistrado.De(resultado, Origem, _agoraUtc()));
            }
            catch (Exception ex)
            {
                // O valor já está na tela: perder o registro não pode derrubar o atendimento.
                _view.ExibirAviso("O cálculo não foi gravado no histórico: " + ex.Message);
                return;
            }

            CarregarHistorico();
        }

        private void CarregarHistorico()
        {
            if (_historico == null)
            {
                _view.ExibirAviso("Sem banco de dados configurado: o histórico está desligado.");
                return;
            }

            try
            {
                _view.ExibirHistorico(_historico.Recentes(LinhasDoHistorico).Select(FormatarLinha).ToList());
            }
            catch (Exception ex)
            {
                _view.ExibirAviso("Não foi possível ler o histórico: " + ex.Message);
            }
        }

        private static LinhaHistorico FormatarLinha(CalculoRegistrado calculo)
        {
            TipoAto ato;
            string rotuloDoAto = Enum.TryParse(calculo.Ato, out ato) ? Rotulos.De(ato) : calculo.Ato;
            DateTime local = DateTime.SpecifyKind(calculo.CriadoEm, DateTimeKind.Utc).ToLocalTime();

            return new LinhaHistorico(local.ToString("dd/MM/yyyy HH:mm", Cultura), calculo.Uf, rotuloDoAto,
                calculo.Quantidade.ToString(Cultura), Moeda(calculo.Total));
        }

        private static string Moeda(decimal valor)
        {
            return valor.ToString("C2", Cultura);
        }
    }
}
