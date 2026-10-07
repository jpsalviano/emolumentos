using System.Collections.Generic;
using System.Linq;

namespace Emolumentos.Dominio
{
    /// <summary>As tabelas disponíveis, uma por UF. Uma UF nova entra aqui e em mais nenhum lugar.</summary>
    public sealed class CatalogoTabelas
    {
        private readonly Dictionary<Uf, ITabelaEmolumentos> _tabelas;

        public CatalogoTabelas() : this(new ITabelaEmolumentos[] { new TabelaCeara(), new TabelaMinasGerais() })
        {
        }

        public CatalogoTabelas(IEnumerable<ITabelaEmolumentos> tabelas)
        {
            _tabelas = tabelas.ToDictionary(t => t.Uf);
        }

        public IReadOnlyCollection<ITabelaEmolumentos> Todas
        {
            get { return _tabelas.Values.OrderBy(t => t.Uf.ToString()).ToList().AsReadOnly(); }
        }

        public ITabelaEmolumentos Obter(Uf uf)
        {
            ITabelaEmolumentos tabela;
            if (!_tabelas.TryGetValue(uf, out tabela))
            {
                throw new PedidoInvalidoException("Não há tabela de emolumentos para " + uf + ".");
            }

            return tabela;
        }
    }
}
