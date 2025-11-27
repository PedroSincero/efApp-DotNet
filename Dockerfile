# Use a imagem base do .NET 10 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copiar arquivos do projeto e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar todo o código
COPY . ./

# Build e publish em um único passo
RUN dotnet publish -c Release -o out

# Imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 80

# Copiar arquivos publicados
COPY --from=build /app/out .

# Definir entry point
ENTRYPOINT ["dotnet", "HelloWorld.dll"]

