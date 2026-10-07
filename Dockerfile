# syntax=docker/dockerfile:1
# Imagem da API e da página web. O desktop (.NET Framework, WinForms) não entra aqui.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Só os arquivos de projeto primeiro: a camada do restore sobrevive a mudanças de código.
COPY src/Emolumentos.Dominio/Emolumentos.Dominio.csproj src/Emolumentos.Dominio/
COPY src/Emolumentos.Apresentacao/Emolumentos.Apresentacao.csproj src/Emolumentos.Apresentacao/
COPY src/Emolumentos.Dados/Emolumentos.Dados.csproj src/Emolumentos.Dados/
COPY src/Emolumentos.Api/Emolumentos.Api.csproj src/Emolumentos.Api/

RUN dotnet restore src/Emolumentos.Api

COPY src src
# As migrations em SQL são embutidas na DLL de dados.
COPY db db

RUN dotnet publish src/Emolumentos.Api --no-restore -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

WORKDIR /app

COPY --from=build /app .

# Usuário sem privilégios que a imagem base já traz.
USER $APP_UID

EXPOSE 8080

ENTRYPOINT ["dotnet", "Emolumentos.Api.dll"]
