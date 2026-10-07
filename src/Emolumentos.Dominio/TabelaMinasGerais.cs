using System;
using System.Collections.Generic;

namespace Emolumentos.Dominio
{
    /// <summary>
    /// Minas Gerais, Tabela 1 (atos do tabelião de notas), exercício de 2026.
    /// O valor final ao usuário soma emolumentos e Taxa de Fiscalização Judiciária (TFJ).
    /// </summary>
    public sealed class TabelaMinasGerais : TabelaEmolumentosBase
    {
        // Nota XXV: acima da última faixa, cada R$ 500.000,00 ou fração acresce aos emolumentos
        // R$ 3.289,90 na primeira faixa e R$ 2.193,27 nas seguintes, até cem faixas. A TFJ é fixa.
        private const decimal TamanhoFaixaExcedente = 500000m;
        private const int MaximoFaixasExcedentes = 100;
        private const decimal AcrescimoPrimeiraFaixa = 3289.90m;
        private const decimal AcrescimoFaixaSeguinte = 2193.27m;
        private const decimal TfjAcimaDasFaixas = 4673.83m;

        private static readonly ReducaoLegal[] Reducoes =
        {
            ReducaoLegal.Nenhuma,
            ReducaoLegal.FinanciamentoHabitacional,
            ReducaoLegal.SistemaFinanceiroHabitacao
        };

        private static readonly Faixa[] FaixasEscritura =
        {
            new Faixa("4.b", 1400m, 159.20m, 61.35m),
            new Faixa("4.b", 2720m, 259.68m, 100.08m),
            new Faixa("4.b", 5440m, 376.34m, 145.01m),
            new Faixa("4.b", 7000m, 520.99m, 200.76m),
            new Faixa("4.b", 14000m, 694.78m, 267.69m),
            new Faixa("4.b", 28000m, 897.58m, 345.89m),
            new Faixa("4.b", 42000m, 1129.02m, 435.05m),
            new Faixa("4.b", 56000m, 1389.81m, 535.50m),
            new Faixa("4.b", 70000m, 1679.40m, 647.11m),
            new Faixa("4.b", 105000m, 2113.64m, 814.42m),
            new Faixa("4.b", 140000m, 2540.87m, 1180.65m),
            new Faixa("4.b", 175000m, 2717.08m, 1262.61m),
            new Faixa("4.b", 210000m, 2893.66m, 1344.66m),
            new Faixa("4.b", 280000m, 3070.72m, 1701.35m),
            new Faixa("4.b", 350000m, 3155.22m, 1748.31m),
            new Faixa("4.b", 420000m, 3240.20m, 1795.39m),
            new Faixa("4.b", 560000m, 3325.70m, 2197.44m),
            new Faixa("4.b", 700000m, 3508.36m, 2318.34m),
            new Faixa("4.b", 840000m, 3691.51m, 2439.36m),
            new Faixa("4.b", 1120000m, 3875.31m, 2991.22m),
            new Faixa("4.b", 1400000m, 4197.56m, 3240.08m),
            new Faixa("4.b", 1680000m, 4520.42m, 3489.30m),
            new Faixa("4.b", 3200000m, 4844.02m, 3738.95m)
        };

        public override Uf Uf
        {
            get { return Uf.MG; }
        }

        public override int Exercicio
        {
            get { return 2026; }
        }

        public override string Fundamento
        {
            get { return "Portaria nº 8.664/CGJ/2025 (TJMG), Anexo da Lei estadual nº 15.424/2004, Tabela 1"; }
        }

        public override IReadOnlyCollection<ReducaoLegal> ReducoesAdmitidas
        {
            get { return Reducoes; }
        }

        /// <summary>Faixas publicadas da escritura com conteúdo financeiro, para conferência com a tabela oficial.</summary>
        public static IReadOnlyList<Faixa> Faixas
        {
            get { return FaixasEscritura; }
        }

        protected override ResultadoCalculo CalcularUnidade(PedidoCalculo pedido)
        {
            switch (pedido.Ato)
            {
                case TipoAto.ReconhecimentoFirma:
                    return Compor(pedido, "5.a", "Reconhecimento de firma, por assinatura", "assinatura",
                        8.55m, 2.66m);
                case TipoAto.AutenticacaoCopia:
                    return Compor(pedido, "3", "Autenticação de cópia, por folha", "folha", 8.55m, 2.66m);
                case TipoAto.Procuracao:
                    return Compor(pedido, "4.f.1", "Escritura de procuração genérica, por outorgante", "outorgante",
                        52.43m, 16.51m);
                case TipoAto.EscrituraSemValor:
                    return Compor(pedido, "4.a", "Escritura relativa a situação jurídica sem conteúdo financeiro",
                        "escritura", 55.45m, 17.45m);
                case TipoAto.EscrituraComValor:
                    return CalcularEscrituraComValor(pedido);
                default:
                    throw new PedidoInvalidoException("Ato não previsto na tabela de Minas Gerais.");
            }
        }

        private ResultadoCalculo CalcularEscrituraComValor(PedidoCalculo pedido)
        {
            const string descricao = "Escritura relativa a situação jurídica com conteúdo financeiro";
            decimal emolumentos;
            decimal tfj;

            Faixa faixa = LocalizarFaixa(FaixasEscritura, pedido.ValorDeclarado);
            if (faixa != null)
            {
                emolumentos = faixa.Emolumento;
                tfj = faixa.Adicional;
            }
            else
            {
                Faixa ultima = FaixasEscritura[FaixasEscritura.Length - 1];
                decimal excedentes = Math.Ceiling((pedido.ValorDeclarado - ultima.Limite) / TamanhoFaixaExcedente);
                excedentes = Math.Min(excedentes, MaximoFaixasExcedentes);
                emolumentos = ultima.Emolumento + AcrescimoPrimeiraFaixa
                    + (excedentes - 1) * AcrescimoFaixaSeguinte;
                tfj = TfjAcimaDasFaixas;
            }

            decimal fator = FatorDaReducao(pedido.Reducao);
            return Compor(pedido, "4.b", descricao, "escritura", Arredondar(emolumentos * fator),
                Arredondar(tfj * fator));
        }

        /// <summary>Notas XV (redução de 50%) e XXIII (redução de 80%) da Tabela 1.</summary>
        private static decimal FatorDaReducao(ReducaoLegal reducao)
        {
            switch (reducao)
            {
                case ReducaoLegal.FinanciamentoHabitacional:
                    return 0.50m;
                case ReducaoLegal.SistemaFinanceiroHabitacao:
                    return 0.20m;
                default:
                    return 1m;
            }
        }

        private ResultadoCalculo Compor(PedidoCalculo pedido, string codigo, string descricao, string unidade,
            decimal emolumentos, decimal tfj)
        {
            return Resultado(pedido, codigo, descricao, unidade,
                new Parcela("Emolumentos", emolumentos),
                new Parcela("Taxa de Fiscalização Judiciária", tfj));
        }
    }
}
