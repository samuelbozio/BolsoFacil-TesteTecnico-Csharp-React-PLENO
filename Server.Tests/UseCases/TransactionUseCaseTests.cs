using Xunit;
using Server.Data;
using Server.Models;
using Server.Application.DTOs;
using Server.Application.UseCases;
using Server.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Tests.UseCases
{
    /// <summary>
    /// Testes para Transaction Use Cases - usando DDD
    /// </summary>
    public class TransactionUseCaseTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICreateTransactionUseCase _createTransactionUseCase;
        private readonly IGetAllTransactionsUseCase _getAllTransactionsUseCase;

        public TransactionUseCaseTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            var transactionValidationService = new Server.Domain.DomainServices.TransactionValidationService();
            var categoryValidationService = new Server.Domain.DomainServices.CategoryValidationService();

            _createTransactionUseCase = new CreateTransactionUseCase(_dbContext, transactionValidationService, categoryValidationService);
            _getAllTransactionsUseCase = new GetAllTransactionsUseCase(_dbContext);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Add categories
            var categories = new List<Category>
            {
                new Category { Id = 1, Description = "Salário", Purpose = PurposeType.Income },
                new Category { Id = 2, Description = "Alimentação", Purpose = PurposeType.Expense },
                new Category { Id = 3, Description = "Geral", Purpose = PurposeType.Both }
            };
            _dbContext.Categories.AddRange(categories);

            // Add people
            var people = new List<Person>
            {
                new Person { Id = 1, Name = "João Silva", Age = 30 },
                new Person { Id = 2, Name = "Maria Santos", Age = 28 },
                new Person { Id = 3, Name = "Pedro Menor", Age = 16 }
            };
            _dbContext.People.AddRange(people);

            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllTransactionsAsync_ReturnsEmptyList()
        {
            // Act
            var result = await _getAllTransactionsUseCase.ExecuteAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateTransactionAsync_WithValidExpense_CreatesTransaction()
        {
            // Arrange
            var request = new CreateTransactionRequestDTO
            {
                Amount = 350.50m,
                Description = "Compra de alimentos",
                Type = "Expense",
                CategoryId = 2,
                PersonId = 1
            };

            // Act
            var result = await _createTransactionUseCase.ExecuteAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(350.50m, result.Amount);
            Assert.Equal("Compra de alimentos", result.Description);
            Assert.Equal("Expense", result.Type);
        }

        [Fact]
        public async Task CreateTransactionAsync_WithMinorCreatingIncome_ThrowsException()
        {
            // Arrange - Pedro tem 16 anos (menor)
            var request = new CreateTransactionRequestDTO
            {
                Amount = 1000m,
                Description = "Receita do menor",
                Type = "Income",
                CategoryId = 1,
                PersonId = 3  // Pedro (16 anos)
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidTransactionException>(() =>
                _createTransactionUseCase.ExecuteAsync(request)
            );
            Assert.Contains("Menores de idade não podem registrar receitas", ex.Message);
        }

        [Fact]
        public async Task CreateTransactionAsync_MinorCanCreateExpense()
        {
            // Arrange - Pedro tem 16 anos mas pode criar DESPESA
            var request = new CreateTransactionRequestDTO
            {
                Amount = 50m,
                Description = "Gastos do menor",
                Type = "Expense",
                CategoryId = 2,
                PersonId = 3  // Pedro (16 anos)
            };

            // Act
            var result = await _createTransactionUseCase.ExecuteAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Expense", result.Type);
        }

        [Fact]
        public async Task CreateTransactionAsync_WithNegativeAmount_ThrowsException()
        {
            // Arrange
            var request = new CreateTransactionRequestDTO
            {
                Amount = -100m,
                Description = "Valor negativo",
                Type = "Expense",
                CategoryId = 2,
                PersonId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTransactionException>(() =>
                _createTransactionUseCase.ExecuteAsync(request)
            );
        }

        [Fact]
        public async Task CreateTransactionAsync_WithZeroAmount_ThrowsException()
        {
            // Arrange
            var request = new CreateTransactionRequestDTO
            {
                Amount = 0,
                Description = "Valor zero",
                Type = "Expense",
                CategoryId = 2,
                PersonId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTransactionException>(() =>
                _createTransactionUseCase.ExecuteAsync(request)
            );
        }

        [Fact]
        public async Task CreateTransactionAsync_WithInvalidPerson_ThrowsException()
        {
            // Arrange
            var request = new CreateTransactionRequestDTO
            {
                Amount = 100m,
                Description = "Pessoa inválida",
                Type = "Expense",
                CategoryId = 2,
                PersonId = 999
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTransactionException>(() =>
                _createTransactionUseCase.ExecuteAsync(request)
            );
        }

        [Fact]
        public async Task CreateTransactionAsync_WithInvalidCategory_ThrowsException()
        {
            // Arrange
            var request = new CreateTransactionRequestDTO
            {
                Amount = 100m,
                Description = "Categoria inválida",
                Type = "Expense",
                CategoryId = 999,
                PersonId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTransactionException>(() =>
                _createTransactionUseCase.ExecuteAsync(request)
            );
        }

        [Fact]
        public async Task CreateTransactionAsync_IncomeWithExpenseCategory_ThrowsException()
        {
            // Arrange - Categoria de Despesa não suporta Receita
            var request = new CreateTransactionRequestDTO
            {
                Amount = 1000m,
                Description = "Receita com categoria de despesa",
                Type = "Income",
                CategoryId = 2,  // Alimentação (Expense only)
                PersonId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTransactionException>(() =>
                _createTransactionUseCase.ExecuteAsync(request)
            );
        }
    }
}
