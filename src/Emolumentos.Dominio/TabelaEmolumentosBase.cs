using System;
using System.Collections.Generic;
using System.Linq;

namespace Emolumentos.Dominio
{
    /// <summary>Faixa de valor declarado de uma tabela: vale até <see cref="Limite"/>, inclusive.</summary>
    public sealed class Faixa
    {
        public Faixa(string codigo, decimal limite, decimal emolumento, decimal adicional)
        {
            Codigo = codigo;
            Limite = limite;
            Emolumento = emolumento;
            Adicional = adicional;
        }

        public string Codigo { get; }

        public decimal Limite { get; }

        public decimal Emolumento { get; }

        /// <summary>Segunda coluna da tabela: FERMOJU no CE, TFJ em MG.</summary>
        public decimal Adicional { get; }
    }

    /// <summary>
    /// O que as UFs têm em comum: validação do pedido, busca da faixa e arredondamento.
    /// Cada estado só diz como compõe o valor de uma unidade do ato.
    /// </summary>
    public abstract class TabelaEmolumentosBase : ITabelaEmolumentos
    {
        private static readonly ReducaoLegal[] SemReducoes = { ReducaoLegal.Nenhuma };

        public abstract Uf Uf { get; }

        public abstract int Exercicio { get; }

        public abstract string Fundamento { get; }

        public virtual IReadOnlyCollection<ReducaoLegal> ReducoesAdmitidas
        {
            get { return SemReducoes; }
        }

        public ResultadoCalculo Calcular(PedidoCalculo pedido)
        {
            if (pedido == null)
            {
                throw new ArgumentNullException(nameof(pedido));
            }

            Validar(pedido);
            return CalcularUnidade(pedido);
        }

        /// <summary>Compõe o valor de uma unidade do ato. O pedido já chega validado.</summary>
        protected abstract ResultadoCalculo CalcularUnidade(PedidoCalculo pedido);

        protected ResultadoCalculo Resultado(PedidoCalculo pedido, string codigo, string descricao, string unidade,
            params Parcela[] parcelas)
        {
            return new ResultadoCalculo(Uf, Exercicio, pedido, codigo, descricao, unidade, parcelas, Fundamento);
        }

        /// <summary>Primeira faixa cujo limite alcança o valor, ou null se o valor passa da última.</summary>
        protected static Faixa LocalizarFaixa(IEnumerable<Faixa> faixas, decimal valor)
        {
            return faixas.FirstOrDefault(f => valor <= f.Limite);
        }

        /// <summary>Centavos, com meio centavo para cima, como nas tabelas publicadas.</summary>
        protected static decimal Arredondar(decimal valor)
        {
            return Math.Round(valor, 2, MidpointRounding.AwayFromZero);
        }

        private void Validar(PedidoCalculo pedido)
        {
            if (pedido.Quantidade < 1)
            {
                throw new PedidoInvalidoException("A quantidade deve ser de pelo menos 1.");
            }

            bool comValor = pedido.Ato == TipoAto.EscrituraComValor;
            if (comValor && pedido.ValorDeclarado <= 0m)
            {
                throw new PedidoInvalidoException("Informe o valor declarado da escritura.");
            }

            if (!comValor && pedido.ValorDeclarado != 0m)
            {
                throw new PedidoInvalidoException("Este ato não tem valor declarado.");
            }

            if (pedido.Reducao != ReducaoLegal.Nenhuma)
            {
                if (!comValor)
                {
                    throw new PedidoInvalidoException("Reduções legais só se aplicam à escritura com valor declarado.");
                }

                if (!ReducoesAdmitidas.Contains(pedido.Reducao))
                {
                    throw new PedidoInvalidoException("A tabela de " + Uf + " não prevê esta redução.");
                }
            }
        }
    }
}
