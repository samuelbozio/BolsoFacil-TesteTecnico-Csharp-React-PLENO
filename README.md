# Maxiprod – Household Expenses

## Visão geral

Aplicação full-stack para gerenciamento de despesas domésticas. Inclui um frontend em Vite/React/TypeScript e um backend em ASP.NET Core com Entity Framework Core e SQLite. O projeto é estruturado seguindo princípios de DDD e separação por camadas, com testes unitários e de integração, autenticação JWT, logging e conteinerização via Docker.

## Estrutura do projeto

Raiz do repositório:
- `Client/` – Frontend (Vite + React + TS)
- `Server/` – Backend (ASP.NET Core + EF Core + SQLite)
- `Server.Tests/` – Testes (xUnit/MSTest/NUnit, conforme configuração)
- `docker-compose.yml` – Orquestração de serviços
- `maxiprod.sln` – Solution .NET
- `NuGet.config` – Configuração de pacotes

### Backend (`Server/`)
- `Program.cs` – Bootstrap ASP.NET Core
- `Controllers/` – Endpoints REST (ex.: `AuthController`, `CategoriesController`, `PeopleController`, `TransactionController`)
- `Services/` – Serviços de infraestrutura (ex.: `JwtTokenService`, `LogService`)
- `Middleware/` – Pipeline cross-cutting (`ExceptionHandlingMiddleware`, `LoggingMiddleware`)
- `Data/` – `ApplicationDbContext` e `SeedData`
- `Migrations/` – Migrações EF Core
- `Domain/` – Camada de domínio (Aggregates, DomainServices, Events, Exceptions, Specifications, ValueObjects)
- `DTOs/` – Objetos de transferência para API (ex.: `CategoryDTO`, `TransactionDTO`)
- `Contracts/` – Contratos/abstrações compartilhadas (interfaces, Dtos contratuais)
- `appsettings.json` e `appsettings.Development.json` – Configurações
- `HouseholdExpensesAPI.http` – arquivo de requisições (opcional)
- `HouseholdExpenses.db` – Banco SQLite local (ambiente de desenvolvimento)

### Frontend (`Client/`)
- `src/` – Código da aplicação
  - `App.tsx`, `main.tsx`
  - `components/` – componentes React (ex.: `CategoriesSection.tsx`)
  - `hooks/`, `services/` – hooks e chamadas à API
  - `styles/`, `types/`, `test/` – estilos, tipos e testes
- `vite.config.ts` – Configuração Vite
- `vitest.config.ts` – Configuração de testes do frontend
- `package.json` – dependências e scripts

## DTOs

DTOs definem o contrato de troca de dados entre cliente e servidor, desacoplando modelos de domínio das formas de serialização e validação expostas pela API.
- Exemplos: `CategoryDTO`, `PersonDTO`, `TransactionDTO`, `LoginDTO`, `TotalSummaryDTO`.
- Benefícios: estabilidade de contrato, versionamento e redução de acoplamento.

## DDD (Domain-Driven Design)

O backend segue princípios de DDD:
- Aggregates e Entities representam invariantes e limites de consistência.
- Domain Services encapsulam regras que não pertencem a uma única entidade.
- Value Objects para tipos imutáveis com semântica própria.
- Specifications para consultas e regras composáveis.
- Events para integração e side-effects no domínio.

## Interfaces e contratos

Interfaces em `Server/Services` e `Server/Contracts` definem pontos de extensão e facilitam testes via mocks. Exemplos:
- `IJwtTokenService` para emissão/validação de tokens.
- Contratos de DTOs e abstrações de serviços/logging.

## Logs

Logging é configurado via `LogService` e middleware:
- `LoggingMiddleware` intercepta requisições/respostas.
- `ExceptionHandlingMiddleware` captura e normaliza erros, gerando logs estruturados.
- Configurações de nível de log via `appsettings*.json`.

## Autenticação JWT

Serviço `JwtTokenService` gera e valida tokens:
- Claims: `NameIdentifier`, `Name`, `Email`, `aud`.
- Assinatura HMAC-SHA256 usando `Jwt:SecretKey` em configuração.
- Validação com `TokenValidationParameters` (lifetime, issuer signing key, clock skew zero).

## Banco de dados (SQLite)

- SQLite como storage local para desenvolvimento: arquivo `HouseholdExpenses.db`.
- Schema gerenciado por EF Core Migrations em `Server/Migrations/`.
- `SeedData` inicializa dados de exemplo.
- Em produção, preferir um provedor gerenciado (PostgreSQL/SQL Server) e ajustar connection strings.

## Docker

- `docker-compose.yml` orquestra serviços de `Client` e `Server`.
- `Client/Dockerfile` e `Server/Dockerfile` definem imagens independentes.
- Scripts auxiliares (`docker-compose.ps1`/`.sh`) podem ser usados para automação local.

## Testes

Backend (`Server.Tests/`):
- Unit tests de domínio e serviços (ex.: `JwtTokenServiceTests`, `DomainTests`).
- Integration tests (ex.: `AuthIntegrationTests`).

Frontend (`Client/src/test/`):
- Testes com Vitest e Testing Library para componentes (ex.: `CategoriesSection.test.tsx`).

## Pacotes e dependências

Backend (NuGet principais):
- ASP.NET Core, Microsoft.Extensions.*
- System.IdentityModel.Tokens.Jwt, Microsoft.IdentityModel.Tokens
- Entity Framework Core (provider SQLite), Tools
- Logging (built-in)

Frontend (npm principais):
- React, React DOM
- Vite
- TypeScript
- Vitest + @testing-library/react

## Como rodar

### Pré-requisitos
- .NET 8 SDK (ou versão especificada no `*.csproj`)
- Node.js LTS
- Docker (opcional para conteinerização)

### Rodar em desenvolvimento (sem Docker)

Backend:

```powershell
# Na pasta Server
cd Server; dotnet restore; dotnet ef database update; dotnet run
```

Frontend:

```powershell
# Na pasta Client
cd Client; npm install; npm run dev
```

### Rodar com Docker

```powershell
# Na raiz do projeto
docker compose up --build
```

Isso iniciará os serviços do backend e frontend em containers, com mapeamentos de portas conforme definidos nos Dockerfiles/compose.

## Configuração

- Ajuste `appsettings.json` e `appsettings.Development.json` para connection strings, JWT (`Jwt:SecretKey`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpireMinutes`) e níveis de log.
- Use variáveis de ambiente em produção para segredos.

## Convenções e dicas

- DTOs devem permanecer estáveis; versionar endpoints se necessário.
- Domínio sem dependências de infraestrutura; use serviços e repositórios para persistência.
- Mantenha `bin/`, `obj/` e `node_modules/` fora do controle de versão.
- Use migrations em vez de criar o banco manualmente.

## Troubleshooting

- Erros de execução `dotnet run`: verifique dependências, migrações e connection string do SQLite.
- Frontend não inicia: confirme `npm install` e versões do Node.
- JWT inválido: revise `Jwt:SecretKey` e parâmetros de validação.

## Licença

Projeto acadêmico/demonstração. Ajuste conforme política da sua organização.
