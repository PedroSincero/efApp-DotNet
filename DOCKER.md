# 🐳 Docker - EF Energia API

Este projeto inclui configuração Docker Compose para executar a aplicação e o banco de dados SQL Server.

## 📋 Pré-requisitos

- Docker Desktop instalado e rodando
- Docker Compose (geralmente incluído no Docker Desktop)

## 🚀 Como Executar

### Subir todos os serviços (API + Banco de Dados)

```bash
docker-compose up --build
```

### Subir em background (detached mode)

```bash
docker-compose up -d --build
```

### Parar os serviços

```bash
docker-compose down
```

### Parar e remover volumes (limpar dados do banco)

```bash
docker-compose down -v
```

## 🔧 Serviços

### API (ef-energia-api)
- **Porta**: `8080`
- **URL**: http://localhost:8080
- **Health Check**: http://localhost:8080/api/HealthCheck

### Banco de Dados (ef-energia-db)
- **Porta**: `1433`
- **Servidor**: `db` (nome do serviço no Docker)
- **Usuário**: `sa`
- **Senha**: `YourStrong@Passw0rd`
- **Database**: `EFEnergiaDB`

## 🔐 Variáveis de Ambiente

As variáveis de ambiente podem ser configuradas no `docker-compose.yml`:

- `ASPNETCORE_ENVIRONMENT`: Ambiente da aplicação (Development/Production)
- `ConnectionStrings__DefaultConnection`: String de conexão com o banco
- `Jwt__Key`: Chave secreta para JWT (mesma da versão Java)
- `Jwt__Issuer`: Emissor do token JWT
- `Jwt__Audience`: Audiência do token JWT

## 📝 Migrações do Banco de Dados

Após subir os containers, você precisará executar as migrações:

```bash
# Entrar no container da API
docker exec -it ef-energia-api bash

# Executar migrações
dotnet ef database update
```

Ou executar as migrações localmente apontando para o banco do Docker:

```bash
# String de conexão para o banco no Docker
dotnet ef database update --connection "Server=localhost,1433;Database=EFEnergiaDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"
```

## 🧪 Testar a API

Após subir os containers:

```bash
# Health Check
curl http://localhost:8080/api/HealthCheck
```

## 📊 Ver Logs

```bash
# Logs de todos os serviços
docker-compose logs -f

# Logs apenas da API
docker-compose logs -f api

# Logs apenas do banco
docker-compose logs -f db
```

## 🔍 Comandos Úteis

```bash
# Ver containers rodando
docker ps

# Ver logs de um container específico
docker logs ef-energia-api
docker logs ef-energia-db

# Entrar no container da API
docker exec -it ef-energia-api bash

# Entrar no container do banco
docker exec -it ef-energia-db bash

# Rebuild apenas a API
docker-compose build api
docker-compose up -d api
```

## ⚠️ Notas Importantes

1. **Senha do Banco**: A senha padrão é `YourStrong@Passw0rd`. **Altere em produção!**

2. **Portas**: Certifique-se de que as portas 8080 e 1433 não estão em uso.

3. **Volumes**: Os dados do banco são persistidos no volume `sqlserver_data`.

4. **Health Check**: A API só inicia após o banco estar saudável (healthcheck configurado).

## 🐛 Troubleshooting

### Erro: "Port already in use"
- Pare outros serviços usando as portas 8080 ou 1433
- Ou altere as portas no `docker-compose.yml`

### Erro: "Cannot connect to database"
- Verifique se o container do banco está rodando: `docker ps`
- Verifique os logs: `docker-compose logs db`
- Aguarde alguns segundos para o banco inicializar completamente

### Rebuild completo
```bash
docker-compose down -v
docker-compose build --no-cache
docker-compose up
```

