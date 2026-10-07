using System;
using System.Configuration;
using System.Windows.Forms;
using Emolumentos.Apresentacao;
using Emolumentos.Dados;
using Emolumentos.Dominio;

namespace Emolumentos.Desktop
{
    internal static class Program
    {
        private const string NomeDaConexao = "Emolumentos";

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string avisoDoBanco;
            IHistoricoCalculos historico = AbrirHistorico(out avisoDoBanco);

            using (var form = new CalculoForm())
            {
                var presenter = new CalculoPresenter(form, new CatalogoTabelas(), historico, () => DateTime.UtcNow);
                presenter.Iniciar();
                if (avisoDoBanco != null)
                {
                    form.ExibirAviso(avisoDoBanco);
                }

                Application.Run(form);
            }
        }

        /// <summary>Histórico no banco do App.config, ou null se não há banco ou ele não responde.</summary>
        private static IHistoricoCalculos AbrirHistorico(out string aviso)
        {
            aviso = null;
            ConnectionStringSettings conexao = ConfigurationManager.ConnectionStrings[NomeDaConexao];
            if (conexao == null)
            {
                return null;
            }

            try
            {
                Banco banco = Banco.DoProvider(conexao.ProviderName, conexao.ConnectionString);
                new MigradorSql(banco).Aplicar();
                return new HistoricoCalculosEf(banco);
            }
            catch (Exception ex)
            {
                // Banco fora do ar não impede o atendimento: o programa abre sem histórico.
                aviso = "Histórico desligado, banco indisponível: " + ex.Message;
                return null;
            }
        }
    }
}
