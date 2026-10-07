using System;
using System.Collections.Generic;

namespace Emolumentos.Dominio
{
    /// <summary>
    /// Ceará, Tabela II (serviços notariais), exercício de 2026.
    /// O total soma emolumento, FERMOJU e selo, mais FAADEP e FRMMP, cada um 5% do emolumento.
    /// </summary>
    public sealed class TabelaCeara : TabelaEmolumentosBase
    {
        private const decimal SeloEscritura = 37.90m;
        private const decimal PercentualFundo = 0.05m;

        // Código 002017 (OBS. 01): acima da última faixa, R$ 0,24 por R$ 10,98 ou fração excedente.
        private const decimal AcrescimoPorFracao = 0.24m;
        private const decimal TamanhoFracao = 10.98m;

        // "Até o máximo de R$ 3.226,76": lido aqui como teto do emolumento do ato.
        private const decimal TetoEmolumento = 3226.76m;
        private const string CodigoAcimaDasFaixas = "002017";

        private static readonly Faixa[] FaixasEscritura =
        {
            new Faixa("002008", 104.00m, 96.92m, 5.89m),
            new Faixa("002009", 235.00m, 276.27m, 17.67m),
            new Faixa("002010", 784.00m, 351.66m, 23.52m),
            new Faixa("002011", 2376.00m, 383.91m, 26.47m),
            new Faixa("002012", 4684.00m, 505.88m, 29.40m),
            new Faixa("002013", 6540.00m, 545.31m, 32.33m),
            new Faixa("002014", 9810.00m, 623.39m, 38.20m),
            new Faixa("002015", 18527.00m, 742.59m, 44.16m),
            new Faixa("002016", 23322.58m, 835.86m, 49.97m)
        };

        public override Uf Uf
        {
            get { return Uf.CE; }
        }

        public override int Exercicio
        {
            get { return 2026; }
        }

        public override string Fundamento
        {
            get { return "Portaria nº 2982/2025-GABPRESI (TJCE), Tabela II, vigente desde 02/01/2026"; }
        }

        /// <summary>Faixas publicadas da escritura com valor, para conferência com a tabela oficial.</summary>
        public static IReadOnlyList<Faixa> Faixas
        {
            get { return FaixasEscritura; }
        }

        protected override ResultadoCalculo CalcularUnidade(PedidoCalculo pedido)
        {
            switch (pedido.Ato)
            {
                case TipoAto.ReconhecimentoFirma:
                    return Compor(pedido, "002001", "Reconhecimento de firma, sinal ou chancela", "firma",
                        4.13m, 0.26m, 1.63m);
                case TipoAto.AutenticacaoCopia:
                    return Compor(pedido, "002002", "Autenticação de cópia reprográfica", "face",
                        2.03m, 0.09m, 1.36m);
                case TipoAto.Procuracao:
                    return Compor(pedido, "002003", "Instrumento de procuração pública", "outorgante",
                        46.74m, 5.89m, 7.59m);
                case TipoAto.EscrituraSemValor:
                    return Compor(pedido, "002007", "Instrumento público de contratos, sem valor declarado",
                        "escritura", 96.92m, 5.89m, 7.59m);
                case TipoAto.EscrituraComValor:
                    return CalcularEscrituraComValor(pedido);
                default:
                    throw new PedidoInvalidoException("Ato não previsto na tabela do Ceará.");
            }
        }

        private ResultadoCalculo CalcularEscrituraComValor(PedidoCalculo pedido)
        {
            const string descricao = "Instrumento público de contratos ou valores expressos ou conversíveis";
            Faixa faixa = LocalizarFaixa(FaixasEscritura, pedido.ValorDeclarado);
            if (faixa != null)
            {
                return Compor(pedido, faixa.Codigo, descricao, "escritura", faixa.Emolumento, faixa.Adicional,
                    SeloEscritura);
            }

            Faixa ultima = FaixasEscritura[FaixasEscritura.Length - 1];
            decimal fracoes = Math.Ceiling((pedido.ValorDeclarado - ultima.Limite) / TamanhoFracao);
            decimal emolumento = Math.Min(ultima.Emolumento + fracoes * AcrescimoPorFracao, TetoEmolumento);

            // OBS. 01, item (2): 5% sobre o emolumento excedente, mais o FERMOJU da última faixa.
            decimal fermoju = Arredondar((emolumento - ultima.Emolumento) * PercentualFundo) + ultima.Adicional;
            return Compor(pedido, CodigoAcimaDasFaixas, descricao, "escritura", emolumento, fermoju, SeloEscritura);
        }

        private ResultadoCalculo Compor(PedidoCalculo pedido, string codigo, string descricao, string unidade,
            decimal emolumento, decimal fermoju, decimal selo)
        {
            decimal fundo = Arredondar(emolumento * PercentualFundo);
            return Resultado(pedido, codigo, descricao, unidade,
                new Parcela("Emolumento", emolumento),
                new Parcela("FERMOJU", fermoju),
                new Parcela("Selo", selo),
                new Parcela("FAADEP", fundo),
                new Parcela("FRMMP", fundo));
        }
    }
}
