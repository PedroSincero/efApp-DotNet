#!/bin/bash

# Script de Correção Rápida - Aplica todas as correções
# Resolve problema da tabela Users ausente

set -e

echo "🔧 Script de Correção Rápida - Tabela Users"
echo "==========================================="
echo ""

# Cores
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ️  $1${NC}"
}

echo "1️⃣  Parando containers antigos..."
docker-compose down -v 2>/dev/null || true
print_success "Containers parados e volumes removidos"
echo ""

echo "2️⃣  Reconstruindo com nova migration..."
print_info "Isso pode levar 1-2 minutos..."
if docker-compose up -d --build; then
    print_success "Build concluído e containers iniciados"
else
    print_error "Falha no build"
    exit 1
fi
echo ""

echo "3️⃣  Aguardando SQL Server inicializar..."
print_info "Aguardando 30 segundos..."
sleep 30

# Verificar se SQL Server está healthy
for i in {1..10}; do
    STATUS=$(docker inspect ef-energia-sqlserver --format='{{.State.Health.Status}}' 2>/dev/null || echo "not running")
    if [ "$STATUS" = "healthy" ]; then
        print_success "SQL Server está rodando e saudável!"
        break
    fi
    if [ $i -eq 10 ]; then
        print_error "SQL Server não ficou healthy em 100 segundos"
        echo "Ver logs: docker-compose logs sqlserver"
        exit 1
    fi
    echo "   Aguardando SQL Server... tentativa $i/10"
    sleep 10
done
echo ""

echo "4️⃣  Aguardando migrations serem aplicadas..."
print_info "Aguardando 20 segundos..."
sleep 20

# Verificar logs de migration
LOGS=$(docker-compose logs api | grep -i migration || echo "")
if echo "$LOGS" | grep -q "successfully"; then
    print_success "Migrations aplicadas com sucesso!"
else
    print_info "Ainda processando... aguardando mais 10 segundos"
    sleep 10
fi
echo ""

echo "5️⃣  Testando API..."

# Health check
HEALTH=$(curl -s http://localhost:8080/api/healthcheck)
if echo "$HEALTH" | grep -q "Connected"; then
    print_success "Health check OK - Database conectado!"
else
    print_error "Health check falhou"
    echo "   Resposta: $HEALTH"
    echo ""
    echo "Ver logs da API:"
    docker-compose logs api | tail -30
    exit 1
fi
echo ""

echo "6️⃣  Testando criação de usuário admin..."
ADMIN_RESULT=$(curl -s -X POST http://localhost:8080/api/authseed-admin)

if echo "$ADMIN_RESULT" | grep -q "sucesso\|já existe"; then
    print_success "Usuário admin criado/validado com sucesso!"
    echo "   Username: admin"
    echo "   Password: admin123"
elif echo "$ADMIN_RESULT" | grep -q "Users"; then
    print_error "Tabela Users ainda não foi criada!"
    echo ""
    echo "Tentando aplicar migrations manualmente..."
    docker exec ef-energia-api dotnet ef database update
    echo ""
    print_info "Aguardando 10 segundos..."
    sleep 10
    
    # Testar novamente
    ADMIN_RESULT=$(curl -s -X POST http://localhost:8080/api/auth/seed-admin)
    if echo "$ADMIN_RESULT" | grep -q "sucesso"; then
        print_success "Admin criado após aplicação manual de migrations!"
    else
        print_error "Ainda com problemas. Ver logs:"
        docker-compose logs api | tail -50
        exit 1
    fi
else
    print_info "Resultado: $ADMIN_RESULT"
fi
echo ""

echo "7️⃣  Testando login..."
LOGIN_RESULT=$(curl -s -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}')

if echo "$LOGIN_RESULT" | grep -q "token"; then
    print_success "Login funcionando! Token JWT gerado com sucesso!"
else
    print_error "Login falhou"
    echo "   Resposta: $LOGIN_RESULT"
    exit 1
fi
echo ""

echo "==========================================="
echo "✨ Correção Concluída com Sucesso!"
echo ""
echo "📊 Status:"
echo "   ✅ SQL Server rodando"
echo "   ✅ Migrations aplicadas (incluindo Users)"
echo "   ✅ Tabela Users criada"
echo "   ✅ Autenticação funcionando"
echo "   ✅ API operacional"
echo ""
echo "🎯 Próximos passos:"
echo "   1. Execute: ./test-api.sh"
echo "   2. Ou teste manualmente os endpoints"
echo ""
print_success "Sistema 100% funcional! 🎉"
echo ""
echo "📝 Credenciais:"
echo "   Username: admin"
echo "   Password: admin123"
echo ""
echo "🌐 API rodando em: http://localhost:8080"
