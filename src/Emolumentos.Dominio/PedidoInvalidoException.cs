using System;

namespace Emolumentos.Dominio
{
    /// <summary>O pedido não pode ser calculado. A mensagem é para o usuário final.</summary>
    public class PedidoInvalidoException : Exception
    {
        public PedidoInvalidoException(string mensagem) : base(mensagem)
        {
        }
    }
}
