#!/bin/bash
# Script para build e deploy com Docker Compose

set -e

echo "======================================"
echo "MaxiProd - Docker Build & Deploy"
echo "======================================"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Funções
build() {
    echo -e "${YELLOW}Building Docker images...${NC}"
    docker-compose build --no-cache
    echo -e "${GREEN}✓ Build completed${NC}"
}

up() {
    echo -e "${YELLOW}Starting containers...${NC}"
    docker-compose up -d
    echo -e "${GREEN}✓ Containers started${NC}"
    echo ""
    echo "Services:"
    echo "  API:      http://localhost:5000"
    echo "  Frontend: http://localhost:3000"
}

down() {
    echo -e "${YELLOW}Stopping containers...${NC}"
    docker-compose down
    echo -e "${GREEN}✓ Containers stopped${NC}"
}

logs() {
    echo -e "${YELLOW}Showing logs...${NC}"
    docker-compose logs -f
}

ps() {
    echo -e "${YELLOW}Container status:${NC}"
    docker-compose ps
}

clean() {
    echo -e "${YELLOW}Cleaning up Docker resources...${NC}"
    docker-compose down -v
    echo -e "${GREEN}✓ Cleanup completed${NC}"
}

rebuild() {
    echo -e "${YELLOW}Rebuilding and starting...${NC}"
    docker-compose down
    docker-compose build --no-cache
    docker-compose up -d
    echo -e "${GREEN}✓ Rebuild completed${NC}"
}

# Main
case "${1:-help}" in
    build)
        build
        ;;
    up)
        up
        ;;
    down)
        down
        ;;
    logs)
        logs
        ;;
    ps)
        ps
        ;;
    clean)
        clean
        ;;
    rebuild)
        rebuild
        ;;
    *)
        echo "Usage: $0 {build|up|down|logs|ps|clean|rebuild}"
        echo ""
        echo "Commands:"
        echo "  build    - Build Docker images"
        echo "  up       - Start containers"
        echo "  down     - Stop containers"
        echo "  logs     - Show container logs"
        echo "  ps       - Show container status"
        echo "  clean    - Remove containers and volumes"
        echo "  rebuild  - Build and start everything from scratch"
        exit 1
        ;;
esac
