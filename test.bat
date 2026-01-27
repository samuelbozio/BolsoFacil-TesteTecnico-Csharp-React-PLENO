@echo off
REM Script para executar testes - Windows
REM Uso: test.bat [backend|frontend|all]

setlocal enabledelayedexpansion

if "%1"=="" (
    echo.
    echo ====================================
    echo   TESTES - Household Expenses API
    echo ====================================
    echo.
    echo Opcoes:
    echo   test backend      - Executar testes backend (C#)
    echo   test frontend     - Executar testes frontend (React)
    echo   test all          - Executar todos os testes
    echo   test watch        - Executar frontend em watch mode
    echo   test coverage     - Gerar relatorio de cobertura frontend
    echo.
    exit /b 1
)

if "%1"=="backend" (
    echo.
    echo [Backend Tests] Executando testes C# com xUnit...
    cd Server
    dotnet test --logger:console;verbosity=detailed
    cd ..
    exit /b !errorlevel!
)

if "%1"=="frontend" (
    echo.
    echo [Frontend Tests] Executando testes React com Vitest...
    cd Client
    npm test
    cd ..
    exit /b !errorlevel!
)

if "%1"=="all" (
    echo.
    echo [All Tests] Executando TODOS os testes...
    echo.
    echo ========== BACKEND TESTS ==========
    cd Server
    dotnet test --logger:console;verbosity=normal
    set BACKEND_RESULT=!errorlevel!
    cd ..
    echo.
    echo ========== FRONTEND TESTS ==========
    cd Client
    npm test -- --run
    set FRONTEND_RESULT=!errorlevel!
    cd ..
    echo.
    echo ========== RESUMO ==========
    echo Backend:  !BACKEND_RESULT!
    echo Frontend: !FRONTEND_RESULT!
    exit /b 0
)

if "%1"=="watch" (
    echo.
    echo [Watch Mode] Frontend em modo watch...
    cd Client
    npm test -- --watch
    cd ..
    exit /b !errorlevel!
)

if "%1"=="coverage" (
    echo.
    echo [Coverage Report] Gerando relatorio de cobertura...
    cd Client
    npm run test:coverage
    cd ..
    exit /b !errorlevel!
)

echo Opcao desconhecida: %1
exit /b 1

endlocal
