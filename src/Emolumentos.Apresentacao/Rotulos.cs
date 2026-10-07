using System;
using Emolumentos.Dominio;

namespace Emolumentos.Apresentacao
{
    /// <summary>Textos exibidos ao usuário para os valores do domínio.</summary>
    public static class Rotulos
    {
        public static string De(Uf uf)
        {
            switch (uf)
            {
                case Uf.CE:
                    return "Ceará";
                case Uf.MG:
                    return "Minas Gerais";
                default:
                    throw new ArgumentOutOfRangeException(nameof(uf));
            }
        }

        public static string De(TipoAto ato)
        {
            switch (ato)
            {
                case TipoAto.ReconhecimentoFirma:
                    return "Reconhecimento de firma";
                case TipoAto.AutenticacaoCopia:
                    return "Autenticação de cópia";
                case TipoAto.Procuracao:
                    return "Procuração";
                case TipoAto.EscrituraSemValor:
                    return "Escritura sem valor declarado";
                case TipoAto.EscrituraComValor:
                    return "Escritura com valor declarado";
                default:
                    throw new ArgumentOutOfRangeException(nameof(ato));
            }
        }

        public static string De(ReducaoLegal reducao)
        {
            switch (reducao)
            {
                case ReducaoLegal.Nenhuma:
                    return "Nenhuma";
                case ReducaoLegal.FinanciamentoHabitacional:
                    return "Financiamento por entidade financeira, Estado ou prefeitura (50%)";
                case ReducaoLegal.SistemaFinanceiroHabitacao:
                    return "Aquisição pelo SFI/SFH, cooperativa ou consórcio (80%)";
                default:
                    throw new ArgumentOutOfRangeException(nameof(reducao));
            }
        }
    }
}
