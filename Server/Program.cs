using Server.Data;
using Server.Services;
using Server.Models;
using Server.Domain.DomainServices;
using Server.Application.UseCases;
using Server.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
    if (builder.Environment.IsDevelopment())
    {
        config.SetMinimumLevel(LogLevel.Debug);
    }
    else
    {
        config.SetMinimumLevel(LogLevel.Information);
    }
});

// Add services to the container.
builder.Services.AddControllers();

// Configure database context (SQLite for development)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Log Service
builder.Services.AddScoped<ILogService, LogService>();

// Register JWT Service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Register Domain Services (DDD - Regras de Negócio)
builder.Services.AddScoped<ITransactionValidationService, TransactionValidationService>();
builder.Services.AddScoped<ICategoryValidationService, CategoryValidationService>();
builder.Services.AddScoped<ICreateCategoryUseCase, CreateCategoryUseCase>();

// Register Use Cases (DDD - Orquestração de Domínio)
// Transaction Use Cases
builder.Services.AddScoped<ICreateTransactionUseCase, CreateTransactionUseCase>();
builder.Services.AddScoped<IGetAllTransactionsUseCase, GetAllTransactionsUseCase>();

// Person Use Cases
builder.Services.AddScoped<ICreatePersonUseCase, CreatePersonUseCase>();
builder.Services.AddScoped<IGetAllPeopleUseCase, GetAllPeopleUseCase>();
builder.Services.AddScoped<IGetPersonByIdUseCase, GetPersonByIdUseCase>();
builder.Services.AddScoped<IUpdatePersonUseCase, UpdatePersonUseCase>();
builder.Services.AddScoped<IDeletePersonUseCase, DeletePersonUseCase>();

// Category Use Cases
builder.Services.AddScoped<IGetAllCategoriesUseCase, GetAllCategoriesUseCase>();
builder.Services.AddScoped<IGetCategoriesWithTotalsUseCase, GetCategoriesWithTotalsUseCase>();

// Configure JWT Authentication
var secretKey = builder.Configuration["Jwt:SecretKey"];
if (!string.IsNullOrEmpty(secretKey))
{
    var key = Encoding.ASCII.GetBytes(secretKey);
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
}

// Add CORS policy for React development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger PRIMEIRO
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HouseholdExpensesAPI v1");
        c.RoutePrefix = "swagger";
    });
}

// Depois seus middlewares
app.UseMiddleware<RequestTimingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseRouting(); // Adicionado para garantir que o roteamento seja configurado antes do CORS

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seed initial data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();

    // Seed initial data if database is empty
    if (!dbContext.Categories.Any())
    {
        dbContext.Categories.AddRange(
            new Category { Description = "Salário", Purpose = PurposeType.Income },
            new Category { Description = "Investimentos", Purpose = PurposeType.Income },
            new Category { Description = "Alimentação", Purpose = PurposeType.Expense },
            new Category { Description = "Moradia", Purpose = PurposeType.Expense },
            new Category { Description = "Transporte", Purpose = PurposeType.Expense },
            new Category { Description = "Lazer", Purpose = PurposeType.Both }
        );
        dbContext.SaveChanges();
    }

    // Seed pessoas if empty
    if (!dbContext.People.Any())
    {
        var people = new List<Person>
        {
            new Person { Name = "João Silva", Age = 30 },
            new Person { Name = "Maria Santos", Age = 28 },
            new Person { Name = "Pedro Costa", Age = 16 },
            new Person { Name = "Ana Paula", Age = 35 }
        };
        dbContext.People.AddRange(people);
        dbContext.SaveChanges();

        // Seed transações se houver pessoas
        if (dbContext.People.Any() && dbContext.Categories.Any())
        {
            var categories = dbContext.Categories.ToList();
            var peoples = dbContext.People.ToList();

            var transactions = new List<Transaction>
            {
                // Receitas
                new Transaction
                {
                    Description = "Salário Janeiro",
                    Value = 5000,
                    Type = TransactionType.Income,
                    CategoryId = categories.First(c => c.Description == "Salário").Id,
                    PersonId = peoples.First(p => p.Name == "João Silva").Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Transaction
                {
                    Description = "Freelance",
                    Value = 1500,
                    Type = TransactionType.Income,
                    CategoryId = categories.First(c => c.Description == "Investimentos").Id,
                    PersonId = peoples.First(p => p.Name == "Maria Santos").Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                // Despesas
                new Transaction
                {
                    Description = "Compras supermercado",
                    Value = 350.50m,
                    Type = TransactionType.Expense,
                    CategoryId = categories.First(c => c.Description == "Alimentação").Id,
                    PersonId = peoples.First(p => p.Name == "João Silva").Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new Transaction
                {
                    Description = "Aluguel",
                    Value = 1200,
                    Type = TransactionType.Expense,
                    CategoryId = categories.First(c => c.Description == "Moradia").Id,
                    PersonId = peoples.First(p => p.Name == "João Silva").Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new Transaction
                {
                    Description = "Uber/Táxi",
                    Value = 75.00m,
                    Type = TransactionType.Expense,
                    CategoryId = categories.First(c => c.Description == "Transporte").Id,
                    PersonId = peoples.First(p => p.Name == "Maria Santos").Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Transaction
                {
                    Description = "Cinema",
                    Value = 60.00m,
                    Type = TransactionType.Expense,
                    CategoryId = categories.First(c => c.Description == "Lazer").Id,
                    PersonId = peoples.First(p => p.Name == "Ana Paula").Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                }
            };
            dbContext.Transactions.AddRange(transactions);
            dbContext.SaveChanges();
        }
    }
}

app.Run();

// Expor Program para testes de integração
public partial class Program { }