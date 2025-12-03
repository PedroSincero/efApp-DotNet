# Use a imagem base do .NET 8 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar arquivos do projeto e restaurar dependências
COPY *.csproj ./
COPY *.sln ./
RUN dotnet restore HelloWorld.csproj

# Copiar todo o código (excluindo testes via .dockerignore)
COPY . ./
# Remover arquivos de teste se foram copiados acidentalmente
RUN rm -rf EFEnergiaAPI.Tests/ || true

# Build e publish especificando o projeto correto
RUN dotnet publish HelloWorld.csproj -c Release -o out

# Imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80

# Copiar arquivos publicados
COPY --from=build /app/out .

# Definir entry point
ENTRYPOINT ["dotnet", "HelloWorld.dll"]
