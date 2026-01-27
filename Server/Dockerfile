# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copiar arquivos de projeto
COPY ["HouseholdExpensesAPI.csproj", "./"]

# Restaurar dependências
RUN dotnet restore "HouseholdExpensesAPI.csproj"

# Copiar código-fonte
COPY . .

# Build
RUN dotnet build "HouseholdExpensesAPI.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "HouseholdExpensesAPI.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copiar build do stage anterior
COPY --from=builder /app/publish .

# Criar diretório para database
RUN mkdir -p /app/data

# Variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Expor porta
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=10s --retries=3 \
  CMD dotnet --version || exit 1

# Comando para rodar
ENTRYPOINT ["dotnet", "HouseholdExpensesAPI.dll"]
