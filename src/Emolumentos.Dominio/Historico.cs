using System;
using System.Collections.Generic;

namespace Emolumentos.Dominio
{
    /// <summary>Um cálculo feito, como fica gravado. Não guarda dado de quem pediu.</summary>
    public class CalculoRegistrado
    {
        public long Id { get; set; }

        /// <summary>Sempre em UTC.</summary>
        public DateTime CriadoEm { get; set; }

        public string Uf { get; set; }

        public string Ato { get; set; }

        public string CodigoAto { get; set; }

        public int Quantidade { get; set; }

        public decimal ValorDeclarado { get; set; }

        public decimal Total { get; set; }

        /// <summary>Aplicação que fez o cálculo: "desktop" ou "api".</summary>
        public string Origem { get; set; }

        public static CalculoRegistrado De(ResultadoCalculo resultado, string origem, DateTime agoraUtc)
        {
            return new CalculoRegistrado
            {
                CriadoEm = agoraUtc,
                Uf = resultado.Uf.ToString(),
                Ato = resultado.Pedido.Ato.ToString(),
                CodigoAto = resultado.CodigoAto,
                Quantidade = resultado.Pedido.Quantidade,
                ValorDeclarado = resultado.Pedido.ValorDeclarado,
                Total = resultado.Total,
                Origem = origem
            };
        }
    }

    /// <summary>Onde os cálculos feitos ficam gravados. A implementação está na camada de dados.</summary>
    public interface IHistoricoCalculos
    {
        void Registrar(CalculoRegistrado calculo);

        /// <summary>Os cálculos mais recentes primeiro.</summary>
        IReadOnlyList<CalculoRegistrado> Recentes(int quantidade);
    }
}
