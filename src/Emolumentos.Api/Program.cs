using System.Threading.RateLimiting;
using Emolumentos.Api;
using Emolumentos.Dados;
using Emolumentos.Dominio;
using Microsoft.AspNetCore.RateLimiting;

const string LimiteDeCalculos = "calculos";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Instância pronta: deixar o contêiner escolher o construtor lhe daria uma lista vazia de tabelas.
builder.Services.AddSingleton(new CatalogoTabelas());
builder.Services.AddSingleton(servicos => HistoricoOpcional.Abrir(
    servicos.GetRequiredService<IConfiguration>()["EMOLUMENTOS_DATABASE_URL"],
    servicos.GetRequiredService<ILogger<HistoricoOpcional>>()));

// Cada cálculo grava uma linha: o limite protege o banco de um laço de requisições.
// É global, não por cliente: atrás do proxy todos chegam com o mesmo endereço.
builder.Services.AddRateLimiter(limites =>
{
    limites.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    limites.AddFixedWindowLimiter(LimiteDeCalculos, janela =>
    {
        janela.PermitLimit = 120;
        janela.Window = TimeSpan.FromMinutes(1);
    });
});

WebApplication app = builder.Build();

// Migra na subida, antes de aceitar requisições.
app.Services.GetRequiredService<HistoricoOpcional>();

app.UseRateLimiter();

// Página de consulta em /emolumentos/, servida do wwwroot.
const string Pagina = "/emolumentos";
app.UseDefaultFiles(new DefaultFilesOptions { RequestPath = Pagina });
app.UseStaticFiles(new StaticFileOptions { RequestPath = Pagina });

RouteGroupBuilder api = app.MapGroup("/api/v1/emolumentos");

api.MapGet("/health", () => Results.Ok(new { status = "ok" }));

api.MapGet("/tabelas", (CatalogoTabelas catalogo) => catalogo.Todas.Select(Respostas.Tabela));

api.MapGet("/calculo", (string? uf, string? ato, int? quantidade, decimal? valor, string? reducao,
    CatalogoTabelas catalogo, HistoricoOpcional historico) =>
{
    if (!Parametros.Ler(uf, out Uf ufLida) || !Parametros.Ler(ato, out TipoAto atoLido))
    {
        return Respostas.Erro("Informe uf e ato válidos; GET /api/v1/emolumentos/tabelas lista as opções.");
    }

    ReducaoLegal reducaoLida = ReducaoLegal.Nenhuma;
    if (reducao != null && !Parametros.Ler(reducao, out reducaoLida))
    {
        return Respostas.Erro("Redução desconhecida.");
    }

    try
    {
        var pedido = new PedidoCalculo(atoLido, quantidade ?? 1, valor ?? 0m, reducaoLida);
        ResultadoCalculo resultado = catalogo.Obter(ufLida).Calcular(pedido);
        historico.Registrar(resultado);
        return Results.Ok(Respostas.Calculo(resultado));
    }
    catch (PedidoInvalidoException ex)
    {
        return Respostas.Erro(ex.Message);
    }
}).RequireRateLimiting(LimiteDeCalculos);

api.MapGet("/calculos/recentes", (HistoricoOpcional historico) => historico.Recentes().Select(Respostas.Registro));

app.Run();

/// <summary>Visível para os testes de integração.</summary>
public partial class Program;
