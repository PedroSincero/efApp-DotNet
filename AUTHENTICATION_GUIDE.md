# 🔐 Guia de Autenticação JWT - EF Energia API

## 📋 Índice
1. [Visão Geral](#visão-geral)
2. [Endpoints de Autenticação](#endpoints-de-autenticação)
3. [Como Usar](#como-usar)
4. [Política de Segurança dos Endpoints](#política-de-segurança-dos-endpoints)
5. [Exemplos com cURL](#exemplos-com-curl)

---

## 🎯 Visão Geral

A API agora utiliza **JWT (JSON Web Token)** para autenticação. Os endpoints críticos requerem um token válido no header da requisição.

### Configurações JWT
- **Algoritmo**: HS256
- **Expiração**: 60 minutos (configurável)
- **Issuer**: EFEnergiaAPI
- **Audience**: EFEnergiaAPI

---

## 🔑 Endpoints de Autenticação

### 1. Criar Usuário Admin Inicial (Apenas Desenvolvimento)
```
POST /api/auth/seed-admin
```

**Resposta:**
```json
{
  "message": "Usuário admin criado com sucesso",
  "username": "admin",
  "password": "admin123",
  "warning": "AVISO: Altere essa senha imediatamente em produção!"
}
```

⚠️ **IMPORTANTE**: Este endpoint só funciona quando não há usuários no banco. Use-o apenas uma vez para criar o primeiro usuário.

---

### 2. Registrar Novo Usuário
```
POST /api/auth/register
Content-Type: application/json

{
  "username": "seu_usuario",
  "password": "sua_senha_minimo_6_caracteres",
  "email": "seu@email.com"  // opcional
}
```

**Resposta de Sucesso:**
```json
{
  "message": "Usuário registrado com sucesso",
  "username": "seu_usuario"
}
```

---

### 3. Login (Obter Token)
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

**Resposta de Sucesso:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-12-02T23:30:00Z",
  "username": "admin"
}
```

---

## 🚀 Como Usar

### Passo 1: Criar Usuário Admin
```bash
curl -X POST http://localhost:8080/api/auth/seed-admin
```

### Passo 2: Fazer Login
```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123"
  }'
```

Copie o **token** da resposta.

### Passo 3: Usar o Token nas Requisições
Adicione o token no header `Authorization` com o prefixo `Bearer`:

```bash
curl -X POST http://localhost:8080/api/equipamentos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "nome": "Sensor 01",
    "setorId": 1
  }'
```

---

## 🔒 Política de Segurança dos Endpoints

### Endpoints PÚBLICOS (sem autenticação)
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/setores` | Listar setores |
| GET | `/api/setores/{id}` | Detalhes do setor |
| GET | `/api/equipamentos` | Listar equipamentos |
| GET | `/api/equipamentos/{id}` | Detalhes do equipamento |
| GET | `/api/healthcheck` | Status da API |

### Endpoints PROTEGIDOS (requerem autenticação)
| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST | `/api/setores` | Criar setor |
| PUT | `/api/setores/{id}` | Atualizar setor |
| DELETE | `/api/setores/{id}` | Deletar setor |
| POST | `/api/equipamentos` | Criar equipamento |
| PUT | `/api/equipamentos/{id}` | Atualizar equipamento |
| DELETE | `/api/equipamentos/{id}` | Deletar equipamento |
| **TODOS** | `/api/leituras/*` | **Todas operações com leituras** |
| **TODOS** | `/api/alertas/*` | **Todas operações com alertas** |

---

## 📝 Exemplos com cURL

### Registrar um novo usuário
```bash
curl -X POST http://localhost:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "joao",
    "password": "senha123",
    "email": "joao@example.com"
  }'
```

### Fazer login e salvar token
```bash
# Login
TOKEN=$(curl -s -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123"
  }' | jq -r '.token')

echo "Token: $TOKEN"
```

### Criar um alerta (requer autenticação)
```bash
curl -X POST http://localhost:8080/api/alertas \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "mensagem": "Temperatura elevada detectada",
    "equipamentoId": 1,
    "resolvido": false
  }'
```

### Listar leituras (requer autenticação)
```bash
curl -X GET "http://localhost:8080/api/leituras?page=1&pageSize=20" \
  -H "Authorization: Bearer $TOKEN"
```

---

## ⚙️ Configuração de Variáveis de Ambiente

Você pode personalizar as configurações JWT via variáveis de ambiente no Docker Compose:

```yaml
environment:
  - Jwt__Key=sua_chave_secreta_aqui_minimo_32_caracteres
  - Jwt__Issuer=EFEnergiaAPI
  - Jwt__Audience=EFEnergiaAPI
  - Jwt__ExpirationMinutes=60
```

---

## 🛡️ Segurança em Produção

1. **Altere as credenciais padrão** imediatamente
2. Use uma **chave JWT forte** (mínimo 32 caracteres)
3. **Não exponha** o endpoint `/seed-admin` em produção
4. Configure **HTTPS** em produção
5. Implemente **refresh tokens** para sessões longas
6. Considere usar **BCrypt** para hash de senhas (atualmente usando SHA256)

---

## 🐛 Troubleshooting

### Token Expirado
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```
**Solução**: Faça login novamente para obter um novo token.

### Token Inválido
**Sintoma**: Erro 401 mesmo com token presente
**Solução**: Verifique se o token está no formato correto: `Bearer SEU_TOKEN`

### Sem Permissão
**Sintoma**: Erro 401 em endpoints protegidos
**Solução**: Certifique-se de incluir o header Authorization com um token válido.

---

## 📚 Recursos Adicionais

- [JWT.io](https://jwt.io) - Debugger de tokens JWT
- [RFC 7519](https://tools.ietf.org/html/rfc7519) - Especificação JWT
- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)
