using System;
using System.Collections.Generic;
using Emolumentos.Dominio;

namespace Emolumentos.Apresentacao
{
    /// <summary>
    /// Tela de cálculo vista pelo presenter. A view é passiva: expõe o que o usuário
    /// escolheu ou digitou, avisa quando ele age e mostra o que recebe, sem decidir nada.
    /// </summary>
    public interface ICalculoView
    {
        /// <summary>O usuário trocou a UF ou o ato.</summary>
        event EventHandler SelecaoAlterada;

        event EventHandler CalcularSolicitado;

        Uf UfSelecionada { get; }

        TipoAto AtoSelecionado { get; }

        ReducaoLegal ReducaoSelecionada { get; }

        /// <summary>Texto como digitado; o presenter é quem interpreta.</summary>
        string QuantidadeDigitada { get; }

        string ValorDeclaradoDigitado { get; }

        void ExibirUfs(IReadOnlyList<Uf> ufs);

        void ExibirAtos(IReadOnlyList<TipoAto> atos);

        void ExibirReducoes(IReadOnlyList<ReducaoLegal> reducoes);

        void HabilitarValorDeclarado(bool habilitado);

        void ExibirResultado(ResultadoExibido resultado);

        /// <summary>O pedido não pôde ser calculado.</summary>
        void ExibirErro(string mensagem);

        void ExibirHistorico(IReadOnlyList<LinhaHistorico> linhas);

        /// <summary>Algo secundário falhou (por exemplo, o banco), sem impedir o cálculo.</summary>
        void ExibirAviso(string mensagem);
    }

    /// <summary>Resultado já formatado para a tela.</summary>
    public sealed class ResultadoExibido
    {
        public ResultadoExibido(string titulo, IReadOnlyList<LinhaParcela> parcelas, string total, string fundamento)
        {
            Titulo = titulo;
            Parcelas = parcelas;
            Total = total;
            Fundamento = fundamento;
        }

        public string Titulo { get; }

        public IReadOnlyList<LinhaParcela> Parcelas { get; }

        public string Total { get; }

        public string Fundamento { get; }
    }

    public sealed class LinhaParcela
    {
        public LinhaParcela(string nome, string valorUnitario, string valor)
        {
            Nome = nome;
            ValorUnitario = valorUnitario;
            Valor = valor;
        }

        public string Nome { get; }

        public string ValorUnitario { get; }

        public string Valor { get; }
    }

    public sealed class LinhaHistorico
    {
        public LinhaHistorico(string data, string uf, string ato, string quantidade, string total)
        {
            Data = data;
            Uf = uf;
            Ato = ato;
            Quantidade = quantidade;
            Total = total;
        }

        public string Data { get; }

        public string Uf { get; }

        public string Ato { get; }

        public string Quantidade { get; }

        public string Total { get; }
    }
}
