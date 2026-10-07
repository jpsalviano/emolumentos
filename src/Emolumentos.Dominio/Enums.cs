namespace Emolumentos.Dominio
{
    public enum Uf
    {
        CE,
        MG
    }

    /// <summary>Atos notariais cobertos. Cada UF dá a eles código, unidade de cobrança e composição próprios.</summary>
    public enum TipoAto
    {
        ReconhecimentoFirma,
        AutenticacaoCopia,
        Procuracao,
        EscrituraSemValor,
        EscrituraComValor
    }

    /// <summary>Reduções previstas em lei sobre o valor final. Nem toda UF admite todas.</summary>
    public enum ReducaoLegal
    {
        Nenhuma,

        /// <summary>Imóvel financiado por entidade financeira, pelo Estado ou por prefeituras.</summary>
        FinanciamentoHabitacional,

        /// <summary>Aquisição financiada por entidade do SFI/SFH, cooperativa de crédito ou consórcio.</summary>
        SistemaFinanceiroHabitacao
    }
}
