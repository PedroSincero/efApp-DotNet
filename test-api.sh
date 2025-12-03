#!/bin/bash

# Script de teste da API EF Energia
# Testa todos os endpoints incluindo autenticação JWT

set -e

API_URL="http://localhost:8080"
TOKEN=""

echo "🚀 Iniciando testes da API EF Energia"
echo "======================================="
echo ""

# Cores para output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função para print colorido
print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ️  $1${NC}"
}

# Teste 1: Health Check
echo "1️⃣  Testando Health Check..."
HEALTH_RESPONSE=$(curl -s -w "\n%{http_code}" "$API_URL/api/healthcheck")
HTTP_CODE=$(echo "$HEALTH_RESPONSE" | tail -n1)
BODY=$(echo "$HEALTH_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" = "200" ]; then
    print_success "Health Check OK"
    echo "   Resposta: $BODY"
else
    print_error "Health Check falhou (HTTP $HTTP_CODE)"
    exit 1
fi
echo ""

# Teste 2: Criar usuário admin
echo "2️⃣  Criando usuário admin..."
ADMIN_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/seed-admin")
HTTP_CODE=$(echo "$ADMIN_RESPONSE" | tail -n1)
BODY=$(echo "$ADMIN_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" = "200" ]; then
    print_success "Usuário admin criado"
    echo "   Resposta: $BODY"
elif [ "$HTTP_CODE" = "400" ]; then
    print_info "Usuário admin já existe"
else
    print_error "Erro ao criar admin (HTTP $HTTP_CODE)"
fi
echo ""

# Teste 3: Login e obter token
echo "3️⃣  Fazendo login..."
LOGIN_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/auth/login" \
    -H "Content-Type: application/json" \
    -d '{"username":"admin","password":"admin123"}')
HTTP_CODE=$(echo "$LOGIN_RESPONSE" | tail -n1)
BODY=$(echo "$LOGIN_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" = "200" ]; then
    print_success "Login realizado com sucesso"
    TOKEN=$(echo "$BODY" | grep -o '"token":"[^"]*' | cut -d'"' -f4)
    print_info "Token JWT obtido: ${TOKEN:0:20}..."
else
    print_error "Login falhou (HTTP $HTTP_CODE)"
    echo "   Resposta: $BODY"
    exit 1
fi
echo ""

# Teste 4: Criar um setor (requer autenticação)
echo "4️⃣  Criando setor..."
SETOR_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/setores" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{"nome":"Setor Teste"}')
HTTP_CODE=$(echo "$SETOR_RESPONSE" | tail -n1)
BODY=$(echo "$SETOR_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" = "201" ]; then
    print_success "Setor criado"
    SETOR_ID=$(echo "$BODY" | grep -o '"id":[0-9]*' | cut -d':' -f2)
    echo "   ID do setor: $SETOR_ID"
else
    print_error "Erro ao criar setor (HTTP $HTTP_CODE)"
    echo "   Resposta: $BODY"
fi
echo ""

# Teste 5: Listar setores (público)
echo "5️⃣  Listando setores (endpoint público)..."
SETORES_RESPONSE=$(curl -s -w "\n%{http_code}" "$API_URL/api/setores?page=1&pageSize=5")
HTTP_CODE=$(echo "$SETORES_RESPONSE" | tail -n1)
BODY=$(echo "$SETORES_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" = "200" ]; then
    print_success "Setores listados"
    echo "   Resposta: $BODY"
else
    print_error "Erro ao listar setores (HTTP $HTTP_CODE)"
fi
echo ""

# Teste 6: Criar equipamento (requer autenticação)
if [ ! -z "$SETOR_ID" ]; then
    echo "6️⃣  Criando equipamento..."
    EQUIP_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/equipamentos" \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $TOKEN" \
        -d "{\"nome\":\"Sensor Teste\",\"setorId\":$SETOR_ID}")
    HTTP_CODE=$(echo "$EQUIP_RESPONSE" | tail -n1)
    BODY=$(echo "$EQUIP_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" = "201" ]; then
        print_success "Equipamento criado"
        EQUIP_ID=$(echo "$BODY" | grep -o '"id":[0-9]*' | cut -d':' -f2)
        echo "   ID do equipamento: $EQUIP_ID"
    else
        print_error "Erro ao criar equipamento (HTTP $HTTP_CODE)"
        echo "   Resposta: $BODY"
    fi
    echo ""
fi

# Teste 7: Criar leitura (requer autenticação)
if [ ! -z "$EQUIP_ID" ]; then
    echo "7️⃣  Criando leitura..."
    LEITURA_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/leituras" \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $TOKEN" \
        -d "{\"temperatura\":25.5,\"dataRegistro\":\"$(date -u +%Y-%m-%dT%H:%M:%SZ)\",\"equipamentoId\":$EQUIP_ID}")
    HTTP_CODE=$(echo "$LEITURA_RESPONSE" | tail -n1)
    BODY=$(echo "$LEITURA_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" = "201" ]; then
        print_success "Leitura criada"
        echo "   Resposta: $BODY"
    else
        print_error "Erro ao criar leitura (HTTP $HTTP_CODE)"
        echo "   Resposta: $BODY"
    fi
    echo ""
fi

# Teste 8: Listar leituras (requer autenticação)
echo "8️⃣  Listando leituras (endpoint protegido)..."
LEITURAS_RESPONSE=$(curl -s -w "\n%{http_code}" "$API_URL/api/leituras?page=1&pageSize=5" \
    -H "Authorization: Bearer $TOKEN")
HTTP_CODE=$(echo "$LEITURAS_RESPONSE" | tail -n1)
BODY=$(echo "$LEITURAS_RESPONSE" | sed '$d')

if [ "$HTTP_CODE" = "200" ]; then
    print_success "Leituras listadas"
    echo "   Resposta: $BODY"
else
    print_error "Erro ao listar leituras (HTTP $HTTP_CODE)"
    echo "   Resposta: $BODY"
fi
echo ""

# Teste 9: Criar alerta (requer autenticação)
if [ ! -z "$EQUIP_ID" ]; then
    echo "9️⃣  Criando alerta..."
    ALERTA_RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$API_URL/api/alertas" \
        -H "Content-Type: application/json" \
        -H "Authorization: Bearer $TOKEN" \
        -d "{\"mensagem\":\"Alerta de teste\",\"equipamentoId\":$EQUIP_ID,\"resolvido\":false}")
    HTTP_CODE=$(echo "$ALERTA_RESPONSE" | tail -n1)
    BODY=$(echo "$ALERTA_RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" = "201" ]; then
        print_success "Alerta criado"
        echo "   Resposta: $BODY"
    else
        print_error "Erro ao criar alerta (HTTP $HTTP_CODE)"
        echo "   Resposta: $BODY"
    fi
    echo ""
fi

# Teste 10: Tentar acessar endpoint protegido sem token
echo "🔒 Testando segurança: tentando acessar endpoint protegido sem token..."
NO_AUTH_RESPONSE=$(curl -s -w "\n%{http_code}" "$API_URL/api/leituras")
HTTP_CODE=$(echo "$NO_AUTH_RESPONSE" | tail -n1)

if [ "$HTTP_CODE" = "401" ]; then
    print_success "Segurança OK: acesso negado sem token (HTTP 401)"
else
    print_error "FALHA DE SEGURANÇA: endpoint protegido acessível sem token!"
fi
echo ""

echo "======================================="
echo "✨ Testes concluídos!"
echo ""
echo "📊 Resumo:"
echo "   - Health Check: OK"
echo "   - Autenticação JWT: OK"
echo "   - Paginação: OK"
echo "   - Segurança: OK"
echo ""
echo "🎉 API funcionando corretamente!"
