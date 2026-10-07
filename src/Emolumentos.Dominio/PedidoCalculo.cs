namespace Emolumentos.Dominio
{
    public sealed class PedidoCalculo
    {
        public PedidoCalculo(TipoAto ato, int quantidade = 1, decimal valorDeclarado = 0m,
            ReducaoLegal reducao = ReducaoLegal.Nenhuma)
        {
            Ato = ato;
            Quantidade = quantidade;
            ValorDeclarado = valorDeclarado;
            Reducao = reducao;
        }

        public TipoAto Ato { get; }

        /// <summary>Unidades cobradas: firmas, faces ou folhas, outorgantes, escrituras.</summary>
        public int Quantidade { get; }

        /// <summary>Valor do negócio, só para atos com valor declarado.</summary>
        public decimal ValorDeclarado { get; }

        public ReducaoLegal Reducao { get; }
    }
}
