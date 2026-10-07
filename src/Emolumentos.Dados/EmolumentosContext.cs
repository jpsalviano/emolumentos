using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using Emolumentos.Dominio;
using Npgsql;

namespace Emolumentos.Dados
{
    /// <summary>
    /// Configuração do EF6 em código, para valer igual com ou sem App.config.
    /// O EF a encontra por estar no mesmo assembly do contexto.
    /// </summary>
    public sealed class ConfiguracaoEf : DbConfiguration
    {
        public ConfiguracaoEf()
        {
            // O SQL Server já vem registrado no EF6; o PostgreSQL entra aqui.
            SetProviderFactory("Npgsql", NpgsqlFactory.Instance);
            SetProviderServices("Npgsql", NpgsqlServices.Instance);
        }
    }

    public sealed class EmolumentosContext : DbContext
    {
        public const string Esquema = "emolumentos";

        static EmolumentosContext()
        {
            // O esquema é das migrations em SQL (pasta db/): o EF nunca cria nem altera tabela.
            Database.SetInitializer<EmolumentosContext>(null);
        }

        public EmolumentosContext(DbConnection conexao) : base(conexao, contextOwnsConnection: true)
        {
        }

        public DbSet<CalculoRegistrado> Calculos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Mesmo esquema e mesmos nomes nos dois bancos, em minúsculas: no PostgreSQL
            // o EF põe aspas nos identificadores, e nome com maiúscula exigiria aspas para sempre.
            modelBuilder.HasDefaultSchema(Esquema);

            EntityTypeConfiguration<CalculoRegistrado> calculo = modelBuilder.Entity<CalculoRegistrado>();
            calculo.ToTable("calculos");
            calculo.HasKey(c => c.Id);
            calculo.Property(c => c.Id).HasColumnName("id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            calculo.Property(c => c.CriadoEm).HasColumnName("criado_em");
            calculo.Property(c => c.Uf).HasColumnName("uf").IsRequired().IsFixedLength().HasMaxLength(2)
                .IsUnicode(false);
            calculo.Property(c => c.Ato).HasColumnName("ato").IsRequired().HasMaxLength(40).IsUnicode(false);
            calculo.Property(c => c.CodigoAto).HasColumnName("codigo_ato").IsRequired().HasMaxLength(20)
                .IsUnicode(false);
            calculo.Property(c => c.Quantidade).HasColumnName("quantidade");
            calculo.Property(c => c.ValorDeclarado).HasColumnName("valor_declarado").HasPrecision(18, 2);
            calculo.Property(c => c.Total).HasColumnName("total").HasPrecision(18, 2);
            calculo.Property(c => c.Origem).HasColumnName("origem").IsRequired().HasMaxLength(20).IsUnicode(false);
        }
    }
}
