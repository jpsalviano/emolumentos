using Emolumentos.Apresentacao;
using Emolumentos.Dominio;

namespace Emolumentos.Api;

/// <summary>Leitura dos parâmetros de consulta que são enums do domínio.</summary>
public static class Parametros
{
    /// <summary>Aceita o nome do valor, sem diferenciar maiúsculas; número não vale.</summary>
    public static bool Ler<T>(string? texto, out T valor) where T : struct, Enum
    {
        valor = default;
        return !string.IsNullOrWhiteSpace(texto)
            && !char.IsDigit(texto[0]) && texto[0] != '-'
            && Enum.TryParse(texto, ignoreCase: true, out valor)
            && Enum.IsDefined(valor);
    }
}

/// <summary>Formato JSON das respostas. Os nomes das propriedades são o contrato da API.</summary>
public static class Respostas
{
    public static IResult Erro(string mensagem) => Results.BadRequest(new { erro = mensagem });

    public static object Tabela(ITabelaEmolumentos tabela) => new
    {
        uf = tabela.Uf.ToString(),
        nome = Rotulos.De(tabela.Uf),
        exercicio = tabela.Exercicio,
        fundamento = tabela.Fundamento,
        atos = Enum.GetValues<TipoAto>().Select(ato => new { codigo = ato.ToString(), rotulo = Rotulos.De(ato) }),
        reducoes = tabela.ReducoesAdmitidas.Select(r => new { codigo = r.ToString(), rotulo = Rotulos.De(r) })
    };

    public static object Calculo(ResultadoCalculo resultado)
    {
        int quantidade = resultado.Pedido.Quantidade;
        return new
        {
            uf = resultado.Uf.ToString(),
            exercicio = resultado.Exercicio,
            ato = resultado.Pedido.Ato.ToString(),
            codigoAto = resultado.CodigoAto,
            descricao = resultado.Descricao,
            unidade = resultado.Unidade,
            quantidade,
            valorDeclarado = resultado.Pedido.ValorDeclarado,
            reducao = resultado.Pedido.Reducao.ToString(),
            parcelas = resultado.ParcelasUnitarias.Select(p => new
            {
                nome = p.Nome,
                valorUnitario = p.Valor,
                valor = p.Valor * quantidade
            }),
            totalUnitario = resultado.TotalUnitario,
            total = resultado.Total,
            fundamento = resultado.Fundamento
        };
    }

    public static object Registro(CalculoRegistrado calculo) => new
    {
        criadoEm = DateTime.SpecifyKind(calculo.CriadoEm, DateTimeKind.Utc),
        uf = calculo.Uf,
        ato = calculo.Ato,
        codigoAto = calculo.CodigoAto,
        quantidade = calculo.Quantidade,
        valorDeclarado = calculo.ValorDeclarado,
        total = calculo.Total,
        origem = calculo.Origem
    };
}
