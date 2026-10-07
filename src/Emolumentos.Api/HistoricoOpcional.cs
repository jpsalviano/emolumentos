using Emolumentos.Dados;
using Emolumentos.Dominio;

namespace Emolumentos.Api;

/// <summary>
/// Histórico que pode não existir. Sem banco, ou com o banco fora do ar, a API continua
/// calculando: os valores vêm das tabelas em código, e o histórico é só um registro.
/// </summary>
public sealed class HistoricoOpcional
{
    public const string Origem = "api";

    private const int Linhas = 20;

    private readonly IHistoricoCalculos? _historico;
    private readonly ILogger _logger;

    public HistoricoOpcional(IHistoricoCalculos? historico, ILogger logger)
    {
        _historico = historico;
        _logger = logger;
    }

    public bool Ligado => _historico != null;

    /// <summary>Aplica as migrations e liga o histórico; com falha, registra o erro e segue sem ele.</summary>
    public static HistoricoOpcional Abrir(string? url, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            logger.LogWarning("EMOLUMENTOS_DATABASE_URL não definida: histórico desligado.");
            return new HistoricoOpcional(null, logger);
        }

        try
        {
            Banco banco = Banco.DaUrlPostgres(url);
            IReadOnlyList<string> scripts = new MigradorSql(banco).Aplicar();
            logger.LogInformation("Migrations aplicadas: {Scripts}", string.Join(", ", scripts));
            return new HistoricoOpcional(new HistoricoCalculosEf(banco), logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Banco indisponível na subida: histórico desligado.");
            return new HistoricoOpcional(null, logger);
        }
    }

    public void Registrar(ResultadoCalculo resultado)
    {
        if (_historico == null)
        {
            return;
        }

        try
        {
            _historico.Registrar(CalculoRegistrado.De(resultado, Origem, DateTime.UtcNow));
        }
        catch (Exception ex)
        {
            // Quem pediu o cálculo recebe o valor mesmo assim.
            _logger.LogError(ex, "Cálculo não gravado no histórico.");
        }
    }

    public IReadOnlyList<CalculoRegistrado> Recentes()
    {
        if (_historico == null)
        {
            return [];
        }

        try
        {
            return _historico.Recentes(Linhas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Histórico não pôde ser lido.");
            return [];
        }
    }
}
