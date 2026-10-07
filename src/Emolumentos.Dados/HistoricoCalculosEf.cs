using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Emolumentos.Dominio;

namespace Emolumentos.Dados
{
    /// <summary>Histórico de cálculos no banco, pelo Entity Framework 6. Um contexto por operação.</summary>
    public sealed class HistoricoCalculosEf : IHistoricoCalculos
    {
        private readonly Banco _banco;

        public HistoricoCalculosEf(Banco banco)
        {
            _banco = banco ?? throw new ArgumentNullException(nameof(banco));
        }

        public void Registrar(CalculoRegistrado calculo)
        {
            using (var contexto = new EmolumentosContext(_banco.CriarConexao()))
            {
                contexto.Calculos.Add(calculo);
                contexto.SaveChanges();
            }
        }

        public IReadOnlyList<CalculoRegistrado> Recentes(int quantidade)
        {
            using (var contexto = new EmolumentosContext(_banco.CriarConexao()))
            {
                List<CalculoRegistrado> calculos = contexto.Calculos.AsNoTracking()
                    .OrderByDescending(c => c.CriadoEm)
                    .ThenByDescending(c => c.Id)
                    .Take(quantidade)
                    .ToList();

                // A coluna não guarda fuso: o valor volta sem Kind, e é UTC.
                foreach (CalculoRegistrado calculo in calculos)
                {
                    calculo.CriadoEm = DateTime.SpecifyKind(calculo.CriadoEm, DateTimeKind.Utc);
                }

                return calculos;
            }
        }
    }
}
