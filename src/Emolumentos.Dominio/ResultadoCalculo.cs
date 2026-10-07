using System.Collections.Generic;
using System.Linq;

namespace Emolumentos.Dominio
{
    /// <summary>Uma linha da composição do valor: emolumento, fundo, selo ou taxa.</summary>
    public sealed class Parcela
    {
        public Parcela(string nome, decimal valor)
        {
            Nome = nome;
            Valor = valor;
        }

        public string Nome { get; }

        public decimal Valor { get; }
    }

    public sealed class ResultadoCalculo
    {
        public ResultadoCalculo(Uf uf, int exercicio, PedidoCalculo pedido, string codigoAto, string descricao,
            string unidade, IEnumerable<Parcela> parcelasUnitarias, string fundamento)
        {
            Uf = uf;
            Exercicio = exercicio;
            Pedido = pedido;
            CodigoAto = codigoAto;
            Descricao = descricao;
            Unidade = unidade;
            ParcelasUnitarias = parcelasUnitarias.ToList().AsReadOnly();
            Fundamento = fundamento;
        }

        public Uf Uf { get; }

        public int Exercicio { get; }

        public PedidoCalculo Pedido { get; }

        /// <summary>Código ou item do ato na tabela oficial da UF.</summary>
        public string CodigoAto { get; }

        public string Descricao { get; }

        /// <summary>O que a quantidade conta nesta UF ("firma", "folha", "outorgante"...).</summary>
        public string Unidade { get; }

        /// <summary>Composição do valor de uma unidade, na ordem da tabela oficial.</summary>
        public IReadOnlyList<Parcela> ParcelasUnitarias { get; }

        public string Fundamento { get; }

        public decimal TotalUnitario
        {
            get { return ParcelasUnitarias.Sum(p => p.Valor); }
        }

        /// <summary>As tabelas fixam o valor por unidade já arredondado: o total é múltiplo dele.</summary>
        public decimal Total
        {
            get { return TotalUnitario * Pedido.Quantidade; }
        }
    }
}
