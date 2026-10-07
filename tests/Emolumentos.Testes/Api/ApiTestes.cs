using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Emolumentos.Testes.Api;

/// <summary>A API de ponta a ponta, em memória e sem banco (histórico desligado).</summary>
public class ApiTestes
{
    private WebApplicationFactory<Program> _fabrica = null!;
    private HttpClient _cliente = null!;

    [OneTimeSetUp]
    public void Subir()
    {
        _fabrica = new WebApplicationFactory<Program>();
        _cliente = _fabrica.CreateClient();
    }

    [OneTimeTearDown]
    public void Derrubar()
    {
        _cliente.Dispose();
        _fabrica.Dispose();
    }

    [Test]
    public async Task Health_responde_ok()
    {
        JsonElement corpo = await Json("/api/v1/emolumentos/health");

        Assert.That(corpo.GetProperty("status").GetString(), Is.EqualTo("ok"));
    }

    [Test]
    public async Task Tabelas_lista_as_ufs_com_atos_e_reducoes()
    {
        JsonElement corpo = await Json("/api/v1/emolumentos/tabelas");

        Assert.That(corpo.EnumerateArray().Select(t => t.GetProperty("uf").GetString()),
            Is.EqualTo(new[] { "CE", "MG" }));
        Assert.That(corpo[0].GetProperty("atos").GetArrayLength(), Is.EqualTo(5));
        Assert.That(corpo[0].GetProperty("reducoes").GetArrayLength(), Is.EqualTo(1));
        Assert.That(corpo[1].GetProperty("reducoes").GetArrayLength(), Is.EqualTo(3));
    }

    [Test]
    public async Task Calculo_devolve_a_composicao_do_valor()
    {
        JsonElement corpo = await Json(
            "/api/v1/emolumentos/calculo?uf=ce&ato=ReconhecimentoFirma&quantidade=3");

        Assert.That(corpo.GetProperty("codigoAto").GetString(), Is.EqualTo("002001"));
        Assert.That(corpo.GetProperty("total").GetDecimal(), Is.EqualTo(19.32m));
        Assert.That(corpo.GetProperty("parcelas").EnumerateArray().Select(p => p.GetProperty("nome").GetString()),
            Is.EqualTo(new[] { "Emolumento", "FERMOJU", "Selo", "FAADEP", "FRMMP" }));
        Assert.That(corpo.GetProperty("parcelas")[0].GetProperty("valor").GetDecimal(), Is.EqualTo(12.39m));
    }

    [Test]
    public async Task Calculo_aceita_valor_e_reducao()
    {
        JsonElement corpo = await Json("/api/v1/emolumentos/calculo?uf=MG&ato=EscrituraComValor"
            + "&valor=250000.00&reducao=SistemaFinanceiroHabitacao");

        Assert.That(corpo.GetProperty("total").GetDecimal(), Is.EqualTo(954.41m));
    }

    [TestCase("uf=SP&ato=Procuracao")]
    [TestCase("uf=1&ato=Procuracao")]
    [TestCase("uf=CE")]
    [TestCase("uf=CE&ato=EscrituraComValor")]
    [TestCase("uf=CE&ato=EscrituraComValor&valor=1000&reducao=SistemaFinanceiroHabitacao")]
    [TestCase("uf=CE&ato=Procuracao&quantidade=0")]
    [TestCase("uf=MG&ato=EscrituraComValor&valor=1000&reducao=Outra")]
    public async Task Pedido_invalido_e_400_com_mensagem(string consulta)
    {
        HttpResponseMessage resposta = await _cliente.GetAsync("/api/v1/emolumentos/calculo?" + consulta);

        Assert.That(resposta.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        JsonElement corpo = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync()).RootElement;
        Assert.That(corpo.GetProperty("erro").GetString(), Is.Not.Empty);
    }

    [Test]
    public async Task Sem_banco_o_historico_vem_vazio()
    {
        JsonElement corpo = await Json("/api/v1/emolumentos/calculos/recentes");

        Assert.That(corpo.GetArrayLength(), Is.Zero);
    }

    [Test]
    public async Task Pagina_de_consulta_fica_em_emolumentos()
    {
        string html = await _cliente.GetStringAsync("/emolumentos/");

        Assert.That(html, Does.Contain("<title>Emolumentos</title>"));
    }

    private async Task<JsonElement> Json(string caminho)
    {
        HttpResponseMessage resposta = await _cliente.GetAsync(caminho);
        Assert.That(resposta.StatusCode, Is.EqualTo(HttpStatusCode.OK), caminho);
        return JsonDocument.Parse(await resposta.Content.ReadAsStringAsync()).RootElement;
    }
}
