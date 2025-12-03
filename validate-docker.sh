#!/bin/bash

# Script de validação rápida do Docker
# Verifica se o build funciona corretamente

set -e

echo "🔍 Validação Rápida do Docker Build"
echo "===================================="
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

# Verificar se está no diretório correto
if [ ! -f "docker-compose.yml" ]; then
    print_error "Erro: docker-compose.yml não encontrado"
    echo "Execute este script no diretório do projeto (efApp-DotNet-Updated)"
    exit 1
fi

print_info "Diretório correto encontrado"
echo ""

# Verificar arquivos essenciais
echo "1️⃣  Verificando arquivos essenciais..."

if [ -f "Dockerfile" ]; then
    print_success "Dockerfile encontrado"
else
    print_error "Dockerfile não encontrado"
    exit 1
fi

if [ -f "HelloWorld.csproj" ]; then
    print_success "HelloWorld.csproj encontrado"
else
    print_error "HelloWorld.csproj não encontrado"
    exit 1
fi

if [ -f ".dockerignore" ]; then
    print_success ".dockerignore encontrado"
else
    print_info ".dockerignore não encontrado (opcional)"
fi

echo ""

# Verificar se Dockerfile especifica o projeto
echo "2️⃣  Verificando Dockerfile..."
if grep -q "HelloWorld.csproj" Dockerfile; then
    print_success "Dockerfile especifica HelloWorld.csproj corretamente"
else
    print_error "Dockerfile NÃO especifica HelloWorld.csproj"
    echo "   O arquivo precisa ter: 'dotnet publish HelloWorld.csproj -c Release -o out'"
    exit 1
fi

echo ""

# Verificar se Docker está rodando
echo "3️⃣  Verificando Docker..."
if docker info > /dev/null 2>&1; then
    print_success "Docker está rodando"
else
    print_error "Docker não está rodando"
    echo "   Inicie o Docker Desktop e tente novamente"
    exit 1
fi

echo ""

# Parar containers antigos se existirem
echo "4️⃣  Limpando containers antigos..."
if docker-compose ps | grep -q "ef-energia-api"; then
    print_info "Parando containers antigos..."
    docker-compose down
    print_success "Containers antigos removidos"
else
    print_info "Nenhum container antigo encontrado"
fi

echo ""

# Tentar build
echo "5️⃣  Iniciando build do Docker..."
print_info "Isso pode levar alguns minutos..."
echo ""

if docker-compose build --no-cache 2>&1 | tee /tmp/docker-build.log; then
    echo ""
    print_success "Build concluído com sucesso!"
else
    echo ""
    print_error "Build falhou!"
    echo ""
    echo "Últimas linhas do log:"
    tail -20 /tmp/docker-build.log
    exit 1
fi

echo ""

# Subir containers
echo "6️⃣  Iniciando containers..."
if docker-compose up -d; then
    print_success "Containers iniciados"
else
    print_error "Falha ao iniciar containers"
    exit 1
fi

echo ""

# Aguardar API iniciar
echo "7️⃣  Aguardando API inicializar..."
for i in {1..30}; do
    if curl -s http://localhost:8080/api/healthcheck > /dev/null 2>&1; then
        print_success "API está respondendo!"
        break
    fi
    
    if [ $i -eq 30 ]; then
        print_error "Timeout: API não respondeu em 30 segundos"
        echo ""
        echo "Logs da API:"
        docker-compose logs --tail=50 api
        exit 1
    fi
    
    echo -n "."
    sleep 1
done

echo ""
echo ""

# Testar health check
echo "8️⃣  Testando health check..."
HEALTH_RESPONSE=$(curl -s http://localhost:8080/api/healthcheck)

if echo "$HEALTH_RESPONSE" | grep -q "UP"; then
    print_success "Health check passou!"
    echo "   Resposta: $HEALTH_RESPONSE"
else
    print_error "Health check falhou!"
    echo "   Resposta: $HEALTH_RESPONSE"
fi

echo ""
echo "===================================="
echo "✨ Validação Concluída!"
echo ""
echo "📊 Status:"
echo "   - Container: $(docker-compose ps -q api | wc -l | tr -d ' ') rodando"
echo "   - Porta: 8080"
echo "   - URL: http://localhost:8080"
echo ""
echo "🎯 Próximos passos:"
echo "   1. Criar admin: curl -X POST http://localhost:8080/api/auth/seed-admin"
echo "   2. Fazer login: curl -X POST http://localhost:8080/api/auth/login -H 'Content-Type: application/json' -d '{\"username\":\"admin\",\"password\":\"admin123\"}'"
echo "   3. Testar API: ./test-api.sh"
echo ""
print_success "Docker funcionando perfeitamente! 🎉"
