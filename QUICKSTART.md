# 🚀 Quick Start Guide - EF Energia API

## Passos para Iniciar a Aplicação

### 1. Subir o Docker Compose

```bash
docker-compose up -d --build
```

Este comando irá:
- ✅ Construir a imagem Docker da aplicação
- ✅ Instalar todas as dependências .NET
- ✅ Iniciar a API na porta 8080
- ✅ Criar o banco de dados automaticamente
- ✅ Aplicar as migrações

### 2. Verificar se está rodando

```bash
# Ver logs
docker-compose logs -f api

# Testar health check
curl http://localhost:8080/api/healthcheck
```

### 3. Inicializar dados e testar

```bash
# Tornar o script executável (apenas primeira vez)
chmod +x test-api.sh

# Executar testes automatizados
./test-api.sh
```

Ou manualmente:

```bash
# 1. Criar usuário admin
curl -X POST http://localhost:8080/api/auth/seed-admin

# 2. Fazer login
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123"
  }'

# 3. Copiar o token da resposta e usar em requisições
TOKEN="seu_token_aqui"

# 4. Criar um setor (exemplo de endpoint protegido)
curl -X POST http://localhost:8080/api/setores \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "nome": "Setor Produção"
  }'
```

## 📝 Verificar se precisa criar migration

Se você modificou os models, crie uma nova migration:

```bash
# Entrar no container
docker exec -it ef-energia-api bash

# Dentro do container
dotnet ef migrations add AddUsersTable
dotnet ef database update

# Sair do container
exit
```

## 🔄 Recriar banco de dados do zero

```bash
# Parar e remover containers e volumes
docker-compose down -v

# Subir novamente (vai recriar o banco)
docker-compose up -d --build
```

## 📊 Estrutura de Banco de Dados Criada

Após iniciar, as seguintes tabelas serão criadas:

- **Users** - Usuários do sistema
- **Setores** - Setores da empresa
- **Equipamentos** - Equipamentos de cada setor
- **Leituras** - Leituras de temperatura dos equipamentos
- **Alertas** - Alertas gerados pelos equipamentos

## 🎯 Próximos Passos

1. ✅ Aplicação rodando em http://localhost:8080
2. ✅ Fazer login e obter token JWT
3. ✅ Testar endpoints com Postman (usar collection em `/postman`)
4. ✅ Ler documentação completa em `AUTHENTICATION_GUIDE.md`

## 🆘 Problemas Comuns

### Porta 8080 já em uso
```bash
# Mudar porta no docker-compose.yml
ports:
  - "8081:80"  # Alterar 8080 para 8081
```

### Container não inicia
```bash
# Ver logs detalhados
docker-compose logs api

# Reconstruir imagem
docker-compose build --no-cache
docker-compose up -d
```

### Erro de conexão com banco
```bash
# Aguardar alguns segundos para o SQL Server inicializar
# Ou reiniciar o container
docker-compose restart api
```

## 📞 Comandos Úteis

```bash
# Status dos containers
docker-compose ps

# Parar API
docker-compose down

# Ver logs em tempo real
docker-compose logs -f

# Reiniciar apenas a API
docker-compose restart api

# Acessar bash do container
docker exec -it ef-energia-api bash
```

---

**🎉 Tudo pronto! Sua API está rodando com:**
- ✅ Paginação implementada
- ✅ Autenticação JWT funcionando
- ✅ Docker configurado e pronto para uso
