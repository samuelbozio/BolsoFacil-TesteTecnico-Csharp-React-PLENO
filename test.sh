#!/bin/bash
# Script para executar testes - macOS/Linux
# Uso: chmod +x test.sh && ./test.sh [backend|frontend|all]

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

if [ -z "$1" ]; then
    echo ""
    echo "===================================="
    echo "  TESTES - Household Expenses API"
    echo "===================================="
    echo ""
    echo "Opções:"
    echo "  test backend      - Executar testes backend (C#)"
    echo "  test frontend     - Executar testes frontend (React)"
    echo "  test all          - Executar todos os testes"
    echo "  test watch        - Executar frontend em watch mode"
    echo "  test coverage     - Gerar relatório de cobertura frontend"
    echo ""
    exit 1
fi

case "$1" in
    backend)
        echo ""
        echo -e "${YELLOW}[Backend Tests]${NC} Executando testes C# com xUnit..."
        cd Server
        dotnet test --logger:console;verbosity=detailed
        BACKEND_EXIT=$?
        cd ..
        exit $BACKEND_EXIT
        ;;
    frontend)
        echo ""
        echo -e "${YELLOW}[Frontend Tests]${NC} Executando testes React com Vitest..."
        cd Client
        npm test
        FRONTEND_EXIT=$?
        cd ..
        exit $FRONTEND_EXIT
        ;;
    all)
        echo ""
        echo -e "${YELLOW}[All Tests]${NC} Executando TODOS os testes..."
        echo ""
        echo -e "${YELLOW}========== BACKEND TESTS ==========${NC}"
        cd Server
        dotnet test --logger:console;verbosity=normal
        BACKEND_EXIT=$?
        cd ..
        echo ""
        echo -e "${YELLOW}========== FRONTEND TESTS ==========${NC}"
        cd Client
        npm test -- --run
        FRONTEND_EXIT=$?
        cd ..
        echo ""
        echo -e "${YELLOW}========== RESUMO ==========${NC}"
        [ $BACKEND_EXIT -eq 0 ] && echo -e "${GREEN}✓ Backend: PASSED${NC}" || echo -e "${RED}✗ Backend: FAILED${NC}"
        [ $FRONTEND_EXIT -eq 0 ] && echo -e "${GREEN}✓ Frontend: PASSED${NC}" || echo -e "${RED}✗ Frontend: FAILED${NC}"
        exit 0
        ;;
    watch)
        echo ""
        echo -e "${YELLOW}[Watch Mode]${NC} Frontend em modo watch..."
        cd Client
        npm test -- --watch
        exit $?
        ;;
    coverage)
        echo ""
        echo -e "${YELLOW}[Coverage Report]${NC} Gerando relatório de cobertura..."
        cd Client
        npm run test:coverage
        exit $?
        ;;
    *)
        echo -e "${RED}Opção desconhecida: $1${NC}"
        exit 1
        ;;
esac
