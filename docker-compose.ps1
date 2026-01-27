# Script para build e deploy com Docker Compose no Windows

param(
    [Parameter(Position = 0)]
    [ValidateSet('build', 'up', 'down', 'logs', 'ps', 'clean', 'rebuild', 'help')]
    [string]$Command = 'help'
)

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "$Message" -ForegroundColor Yellow
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Build-Images {
    Write-Info "Building Docker images..."
    docker-compose build --no-cache
    Write-Success "Build completed"
}

function Start-Containers {
    Write-Info "Starting containers..."
    docker-compose up -d
    Write-Success "Containers started"
    Write-Host ""
    Write-Host "Services:" -ForegroundColor Green
    Write-Host "  API:      http://localhost:5000"
    Write-Host "  Frontend: http://localhost:3000"
}

function Stop-Containers {
    Write-Info "Stopping containers..."
    docker-compose down
    Write-Success "Containers stopped"
}

function Show-Logs {
    Write-Info "Showing logs..."
    docker-compose logs -f
}

function Show-Status {
    Write-Info "Container status:"
    docker-compose ps
}

function Clean-Resources {
    Write-Info "Cleaning up Docker resources..."
    docker-compose down -v
    Write-Success "Cleanup completed"
}

function Rebuild-All {
    Write-Info "Rebuilding and starting..."
    docker-compose down
    docker-compose build --no-cache
    docker-compose up -d
    Write-Success "Rebuild completed"
}

function Show-Help {
    Write-Host "MaxiProd - Docker Build & Deploy" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\docker-compose.ps1 [command]" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Commands:" -ForegroundColor Green
    Write-Host "  build    - Build Docker images"
    Write-Host "  up       - Start containers"
    Write-Host "  down     - Stop containers"
    Write-Host "  logs     - Show container logs"
    Write-Host "  ps       - Show container status"
    Write-Host "  clean    - Remove containers and volumes"
    Write-Host "  rebuild  - Build and start everything from scratch"
    Write-Host "  help     - Show this help message"
}

# Main
switch ($Command) {
    'build' { Build-Images }
    'up' { Start-Containers }
    'down' { Stop-Containers }
    'logs' { Show-Logs }
    'ps' { Show-Status }
    'clean' { Clean-Resources }
    'rebuild' { Rebuild-All }
    default { Show-Help }
}
