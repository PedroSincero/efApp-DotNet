# 🌟 EF Energia API - Sistema de Gestão de Energia

API RESTful desenvolvida em .NET 10.0 com Entity Framework Core para gerenciamento de consumo energético, equipamentos e alertas.

## 📋 Funcionalidades

- ✅ **CRUD completo** para Setores, Equipamentos, Leituras e Alertas
- ✅ **Paginação** em todos os endpoints de listagem
- ✅ **Autenticação JWT** nos endpoints críticos
- ✅ **Docker & Docker Compose** para deploy simplificado
- ✅ **SQL Server** como banco de dados
- ✅ **Health Check** para monitoramento

---

## 🚀 Quick Start

### Pré-requisitos
- Docker & Docker Compose instalados
- (Opcional) .NET 10 SDK para desenvolvimento local

### 1️⃣ Subir a aplicação com Docker

```bash
# Clone o repositório
git clone <seu-repositorio>
cd efApp-DotNet

# Inicie os containers
docker-compose up -d

# Verificar logs
docker-compose logs -f api
```

A API estará disponível em: **http://localhost:8080**

### 2️⃣ Inicializar o banco de dados

A aplicação criará automaticamente o banco de dados e as tabelas na primeira execução.

### 3️⃣ Criar usuário admin

```bash
curl -X POST http://localhost:8080/api/auth/seed-admin
```

Resposta:
```json
{
  "message": "Usuário admin criado com sucesso",
  "username": "admin",
  "password": "admin123"
}
```

### 4️⃣ Fazer login e obter token

```bash
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123"
  }'
```

Guarde o **token** retornado!

---

## 📡 Endpoints da API

### 🔓 Autenticação

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| POST | `/api/auth/seed-admin` | Criar usuário admin inicial | ❌ |
| POST | `/api/auth/register` | Registrar novo usuário | ❌ |
| POST | `/api/auth/login` | Login e obter token JWT | ❌ |

### 🏢 Setores

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/setores?page=1&pageSize=10` | Listar setores (paginado) | ❌ |
| GET | `/api/setores/{id}` | Buscar setor por ID | ❌ |
| POST | `/api/setores` | Criar novo setor | ✅ |
| PUT | `/api/setores/{id}` | Atualizar setor | ✅ |
| DELETE | `/api/setores/{id}` | Deletar setor | ✅ |

### 🔧 Equipamentos

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/equipamentos?page=1&pageSize=10` | Listar equipamentos (paginado) | ❌ |
| GET | `/api/equipamentos/{id}` | Buscar equipamento por ID | ❌ |
| POST | `/api/equipamentos` | Criar novo equipamento | ✅ |
| PUT | `/api/equipamentos/{id}` | Atualizar equipamento | ✅ |
| DELETE | `/api/equipamentos/{id}` | Deletar equipamento | ✅ |

### 📊 Leituras

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/leituras?page=1&pageSize=10` | Listar leituras (paginado) | ✅ |
| GET | `/api/leituras/{id}` | Buscar leitura por ID | ✅ |
| POST | `/api/leituras` | Criar nova leitura | ✅ |
| PUT | `/api/leituras/{id}` | Atualizar leitura | ✅ |
| DELETE | `/api/leituras/{id}` | Deletar leitura | ✅ |

### 🚨 Alertas

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/alertas?page=1&pageSize=10` | Listar alertas (paginado) | ✅ |
| GET | `/api/alertas/{id}` | Buscar alerta por ID | ✅ |
| POST | `/api/alertas` | Criar novo alerta | ✅ |
| PUT | `/api/alertas/{id}` | Atualizar alerta | ✅ |
| DELETE | `/api/alertas/{id}` | Deletar alerta | ✅ |

### ❤️ Health Check

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/api/healthcheck` | Verificar status da API | ❌ |

---

## 🔐 Autenticação JWT

### Como usar em requisições

Após fazer login, adicione o token no header **Authorization**:

```bash
curl -X GET http://localhost:8080/api/leituras \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### Exemplo Completo com JavaScript/Fetch

```javascript
// 1. Login
const loginResponse = await fetch('http://localhost:8080/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'admin',
    password: 'admin123'
  })
});

const { token } = await loginResponse.json();

// 2. Usar token em requisições
const leiturasResponse = await fetch('http://localhost:8080/api/leituras?page=1&pageSize=10', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
});

const leituras = await leiturasResponse.json();
console.log(leituras);
```

📖 **Guia Completo**: Veja [AUTHENTICATION_GUIDE.md](./AUTHENTICATION_GUIDE.md) para mais detalhes.

---

## 📄 Paginação

Todos os endpoints de listagem suportam paginação via query parameters:

```bash
GET /api/equipamentos?page=2&pageSize=20
```

**Parâmetros:**
- `page` - Número da página (padrão: 1)
- `pageSize` - Itens por página (padrão: 10)

**Resposta:**
```json
{
  "totalItems": 150,
  "page": 2,
  "pageSize": 20,
  "items": [ /* ... */ ]
}
```

---

## 🐳 Docker

### Comandos úteis

```bash
# Iniciar
docker-compose up -d

# Parar
docker-compose down

# Ver logs
docker-compose logs -f api

# Rebuild
docker-compose up -d --build

# Parar e remover volumes (limpa banco de dados)
docker-compose down -v
```

### Variáveis de Ambiente

Configure no `docker-compose.yml`:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - Jwt__Key=sua_chave_secreta_minimo_32_caracteres
  - Jwt__Issuer=EFEnergiaAPI
  - Jwt__Audience=EFEnergiaAPI
  - Jwt__ExpirationMinutes=60
```

---

## 🛠️ Desenvolvimento Local (sem Docker)

### Pré-requisitos
- .NET 10 SDK
- SQL Server

### Passos

```bash
# Restaurar dependências
dotnet restore

# Atualizar banco de dados
dotnet ef database update

# Executar
dotnet run
```

A API estará em: **http://localhost:5000**

---

## 🗂️ Estrutura do Projeto

```
efApp-DotNet/
├── Controllers/          # Controllers da API
│   ├── AuthController.cs           # Autenticação JWT
│   ├── SetoresController.cs        # CRUD Setores
│   ├── EquipamentosController.cs   # CRUD Equipamentos
│   ├── LeiturasController.cs       # CRUD Leituras
│   ├── AlertasController.cs        # CRUD Alertas
│   └── HealthCheckController.cs    # Health Check
├── Models/               # Modelos de dados
│   ├── User.cs           # Modelo de usuário
│   ├── AuthModels.cs     # DTOs de autenticação
│   ├── Setor.cs
│   ├── Equipamento.cs
│   ├── Leitura.cs
│   └── Alerta.cs
├── Data/
│   └── ApplicationDbContext.cs    # Contexto do EF Core
├── Services/             # Serviços
│   ├── IHealthCheckService.cs
│   └── HealthCheckService.cs
├── Migrations/           # Migrações do EF
├── Program.cs            # Configuração da aplicação
├── Dockerfile            # Dockerfile para build
├── docker-compose.yml    # Orquestração Docker
├── README.md             # Este arquivo
└── AUTHENTICATION_GUIDE.md  # Guia de autenticação
```

---

## 📊 Modelos de Dados

### Setor
```json
{
  "id": 1,
  "nome": "Produção"
}
```

### Equipamento
```json
{
  "id": 1,
  "nome": "Sensor Térmico 01",
  "setorId": 1
}
```

### Leitura
```json
{
  "id": 1,
  "temperatura": 25.5,
  "dataRegistro": "2024-12-02T10:30:00Z",
  "equipamentoId": 1
}
```

### Alerta
```json
{
  "id": 1,
  "mensagem": "Temperatura acima do normal",
  "dataCriacao": "2024-12-02T10:35:00Z",
  "resolvido": false,
  "equipamentoId": 1
}
```

---

## 🧪 Testando a API

### Com cURL

Veja exemplos completos em [AUTHENTICATION_GUIDE.md](./AUTHENTICATION_GUIDE.md)

### Com Postman

1. Importe a collection: `postman/EF-Energia-API.postman_collection.json`
2. Configure a variável `{{token}}` com o token obtido no login
3. Execute as requisições

---

## 🔧 Troubleshooting

### Problema: API não inicia
```bash
# Verificar logs
docker-compose logs api

# Verificar se a porta 8080 está livre
netstat -an | grep 8080
```

### Problema: Erro de autenticação
- Certifique-se de incluir `Bearer` antes do token
- Verifique se o token não expirou (60 minutos)
- Faça login novamente para obter novo token

### Problema: Banco de dados não criado
```bash
# Remover containers e volumes
docker-compose down -v

# Subir novamente
docker-compose up -d
```

---

## 📝 Notas de Segurança

⚠️ **IMPORTANTE para Produção:**

1. Altere as credenciais padrão (`admin`/`admin123`)
2. Use uma chave JWT forte (mínimo 32 caracteres aleatórios)
3. Desabilite o endpoint `/seed-admin` em produção
4. Configure HTTPS
5. Implemente rate limiting
6. Use BCrypt ou Argon2 para hash de senhas

---

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

---

## 📄 Licença

Este projeto é open source e está sob a licença MIT.

---

## 📞 Suporte

Para dúvidas ou problemas:
- Abra uma issue no GitHub
- Consulte a documentação de autenticação: [AUTHENTICATION_GUIDE.md](./AUTHENTICATION_GUIDE.md)

---

**Desenvolvido com ❤️ usando .NET 10.0 e Entity Framework Core**
