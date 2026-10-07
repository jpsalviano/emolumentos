# Emolumentos

Cálculo de emolumentos e custas de atos notariais no Ceará e em Minas Gerais, com a
composição do valor segundo as tabelas oficiais de 2026.

As mesmas regras de negócio, escritas em C#, servem a duas aplicações:

- um **aplicativo desktop** em WinForms e .NET Framework 4.8, no padrão MVP;
- uma **API** com uma página de consulta, em produção em
  [jpsalviano.com.br/emolumentos](https://jpsalviano.com.br/emolumentos/).

![Tela do aplicativo desktop](docs/desktop.png)

## O que ele calcula

| Ato | Ceará | Minas Gerais |
| --- | --- | --- |
| Reconhecimento de firma | 002001, por firma | 5.a, por assinatura |
| Autenticação de cópia | 002002, por face | 3, por folha |
| Procuração | 002003, por outorgante | 4.f.1, por outorgante |
| Escritura sem valor declarado | 002007 | 4.a |
| Escritura com valor declarado | 002008 a 002017, por faixa | 4.b, por faixa |

Os dois estados compõem o valor de formas diferentes, e é isso que o projeto modela:

- **Ceará**: emolumento, FERMOJU e selo, mais FAADEP e FRMMP (5% do emolumento cada).
  Acima da última faixa, R$ 0,24 por R$ 10,98 ou fração excedente, com teto.
- **Minas Gerais**: emolumentos e Taxa de Fiscalização Judiciária. Acima da última
  faixa, acréscimo por faixa de R$ 500.000,00 (nota XXV). Reduções de 50% e de 80% para
  imóveis financiados (notas XV e XXIII).

Fontes: [Portaria nº 2982/2025-GABPRESI do TJCE](https://djea-con.tjce.jus.br/materias/162549)
(Tabela II) e [Portaria nº 8.664/CGJ/2025 do TJMG](https://www8.tjmg.jus.br/institucional/at/pdf/cpo86642025.pdf)
(Tabela 1). Os testes conferem cada linha dessas tabelas com o total publicado.

É um projeto de estudo, com atos selecionados: não substitui a consulta ao cartório.
Duas leituras minhas das normas estão marcadas no código: o "máximo de R$ 3.226,76" do
código 002017 do Ceará é tratado como teto do emolumento, e o acréscimo da nota XXV de
Minas Gerais é somado ao emolumento da última faixa.

## Arquitetura

```
src/
  Emolumentos.Dominio        netstandard2.0   regras por UF, sem dependências
  Emolumentos.Apresentacao   netstandard2.0   presenter e contrato da view (MVP)
  Emolumentos.Dados          net48 + ns2.1    Entity Framework 6 e migrador de SQL
  Emolumentos.Desktop        net48            WinForms: só a view
  Emolumentos.Api            net10.0          API e página de consulta
db/
  sqlserver/  postgresql/                     migrations idempotentes
tests/
  Emolumentos.Testes                          NUnit
```

- **Uma classe por UF.** `TabelaCeara` e `TabelaMinasGerais` implementam
  `ITabelaEmolumentos` e herdam de `TabelaEmolumentosBase` o que é comum: validação do
  pedido, busca da faixa e arredondamento. Uma UF nova é uma classe nova e uma linha no
  `CatalogoTabelas`.
- **MVP com view passiva.** `CalculoForm` só expõe o que foi digitado e mostra o que
  recebe. `CalculoPresenter` interpreta a entrada, chama o domínio, formata em reais e
  trata os erros. Por não referenciar WinForms, o presenter é testado sem abrir janela.
- **Migrations em SQL, idempotentes.** Cada script de `db/` pode rodar quantas vezes
  for, em banco novo ou já migrado. Por isso não há tabela de controle: o `MigradorSql`
  aplica todos na subida da aplicação. Há um conjunto para SQL Server e outro para
  PostgreSQL, com o mesmo esquema.
- **Entity Framework 6** lê e grava o histórico nos dois bancos com o mesmo mapeamento.
  O EF não cria tabelas: o esquema é só das migrations.
- **O banco é opcional.** Os valores vêm das tabelas em código. Sem banco, ou com ele
  fora do ar, desktop e API continuam calculando e apenas o histórico some.

## Como rodar

Requer o [SDK do .NET 10](https://dotnet.microsoft.com/download).

```sh
dotnet test tests/Emolumentos.Testes     # testes
dotnet run --project src/Emolumentos.Api # API e página em http://localhost:5000/emolumentos/
```

O desktop requer Windows (o .NET Framework 4.8 já vem no Windows 10 e 11):

```sh
dotnet run --project src/Emolumentos.Desktop
```

Ou baixe o `Emolumentos-desktop.zip` da [última release](https://github.com/jpsalviano/emolumentos/releases/latest),
extraia e abra `Emolumentos.exe`.

Para ligar o histórico no desktop, descomente uma connection string em
`Emolumentos.exe.config` (SQL Server ou PostgreSQL). Na API, defina
`EMOLUMENTOS_DATABASE_URL=postgresql://usuario:senha@host:5432/banco`.

## API

| Rota | Resposta |
| --- | --- |
| `GET /api/v1/emolumentos/tabelas` | UFs, atos e reduções disponíveis |
| `GET /api/v1/emolumentos/calculo?uf=MG&ato=EscrituraComValor&valor=250000&reducao=Nenhuma&quantidade=1` | composição do valor |
| `GET /api/v1/emolumentos/calculos/recentes` | últimos cálculos gravados |
| `GET /api/v1/emolumentos/health` | `{"status":"ok"}` |

## Testes e CI

O GitHub Actions ([ci.yml](.github/workflows/ci.yml)) roda a cada push e pull request:

1. sobe SQL Server e PostgreSQL e aplica cada migration **duas vezes** em cada um, pelo
   cliente do próprio banco (`sqlcmd` e `psql`);
2. roda os testes, incluindo os de migrations e EF6 contra os dois bancos;
3. compila o desktop em Windows e, em tags `v*`, publica o zip na release;
4. constrói a imagem Docker da API, publica no GHCR e confere a API dentro dela.

O [azure-pipelines.yml](azure-pipelines.yml) repete testes e build do desktop no Azure DevOps.

## Como foi feito

Desenvolvi este projeto com o Claude Opus 5.5 (Claude Code) como agente: ele escreveu
boa parte do código e dos testes sob minha direção, e eu defini o escopo, revisei cada
alteração e respondo pelo resultado.
