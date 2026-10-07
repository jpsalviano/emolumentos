using System.Collections.Generic;

namespace Emolumentos.Dominio
{
    /// <summary>Tabela de emolumentos de uma UF em um exercício.</summary>
    public interface ITabelaEmolumentos
    {
        Uf Uf { get; }

        int Exercicio { get; }

        /// <summary>Norma que publicou os valores.</summary>
        string Fundamento { get; }

        IReadOnlyCollection<ReducaoLegal> ReducoesAdmitidas { get; }

        /// <exception cref="PedidoInvalidoException">Pedido fora do que a tabela admite.</exception>
        ResultadoCalculo Calcular(PedidoCalculo pedido);
    }
}
