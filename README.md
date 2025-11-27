# EF Energia API - .NET Core 8

API RESTful para Eficiência Energética desenvolvida em ASP.NET Core 8.

## 📋 Requisitos

- .NET 8 SDK
- SQL Server (ou SQL Server LocalDB)
- Docker (opcional, para containerização)

## 🚀 Como executar

### Desenvolvimento Local

1. Restaurar dependências:
```bash
dotnet restore
```

2. Configurar banco de dados:
```bash
# Criar migração inicial (quando os modelos estiverem prontos)
dotnet ef migrations add InitialCreate

# Aplicar migrações
dotnet ef database update
```

3. Executar a aplicação:
```bash
dotnet run
```

4. Acessar:
- API: `https://localhost:5001` ou `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

### Docker

```bash
docker build -t ef-energia-api .
docker run -p 8080:80 ef-energia-api
```

## 🏗️ Estrutura do Projeto

```
WTC-Chat-C-plus/
├── Controllers/          # Controllers da API
│   └── HealthCheckController.cs
├── Services/            # Lógica de negócio
│   ├── IHealthCheckService.cs
│   └── HealthCheckService.cs
├── Models/              # Modelos de dados
│   └── HealthCheckResponse.cs
├── Data/                # Contexto do Entity Framework
│   └── ApplicationDbContext.cs
├── EFEnergiaAPI.Tests/  # Projeto de testes
│   ├── Controllers/
│   └── WebApplicationFactory.cs
├── Program.cs           # Configuração da aplicação
├── appsettings.json     # Configurações
├── Dockerfile           # Configuração Docker
└── .dockerignore        # Arquivos ignorados no Docker
```

## 🧪 Testes

Executar testes unitários:

```bash
dotnet test
```

## 📝 Endpoints Disponíveis

### Health Check
- **GET** `/api/HealthCheck` - Verifica status da aplicação e conexão com banco de dados

## 🔐 Autenticação

A API utiliza JWT Bearer Token para autenticação. Configure as chaves JWT no `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyForJWTTokenGenerationThatMustBeAtLeast32Characters",
    "Issuer": "EFEnergiaAPI",
    "Audience": "EFEnergiaAPI",
    "ExpirationMinutes": 60
  }
}
```

## 🗄️ Banco de Dados

A aplicação está configurada para usar SQL Server. A string de conexão está em `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EFEnergiaDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

## 📦 Migrações

Para criar uma nova migração:
```bash
dotnet ef migrations add NomeDaMigracao
```

Para aplicar migrações:
```bash
dotnet ef database update
```

## 🎯 Próximos Passos

- [ ] Implementar modelos de domínio (Setor, Equipamento, Leitura, Alerta)
- [ ] Criar endpoints RESTful (mínimo 4)
- [ ] Implementar paginação nos endpoints de listagem
- [ ] Adicionar autenticação/autorização nos endpoints críticos
- [ ] Implementar validação e tratamento de exceções
- [ ] Adicionar mais testes unitários

## 📄 Licença

Este projeto foi desenvolvido para fins acadêmicos.

