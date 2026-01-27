using Xunit;
using Server.Data;
using Server.Models;
using Server.DTOs;
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
    /// Testes para Person Use Cases - usando DDD
    /// </summary>
    public class PersonUseCaseTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICreatePersonUseCase _createPersonUseCase;
        private readonly IGetAllPeopleUseCase _getAllPeopleUseCase;
        private readonly IGetPersonByIdUseCase _getPersonByIdUseCase;
        private readonly IUpdatePersonUseCase _updatePersonUseCase;
        private readonly IDeletePersonUseCase _deletePersonUseCase;

        public PersonUseCaseTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _createPersonUseCase = new CreatePersonUseCase(_dbContext);
            _getAllPeopleUseCase = new GetAllPeopleUseCase(_dbContext);
            _getPersonByIdUseCase = new GetPersonByIdUseCase(_dbContext);
            _updatePersonUseCase = new UpdatePersonUseCase(_dbContext);
            _deletePersonUseCase = new DeletePersonUseCase(_dbContext);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var people = new List<Person>
            {
                new Person { Id = 1, Name = "João Silva", Age = 30 },
                new Person { Id = 2, Name = "Maria Santos", Age = 28 }
            };

            _dbContext.People.AddRange(people);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllPeopleAsync_ReturnsAllPeople()
        {
            // Act
            var result = await _getAllPeopleUseCase.ExecuteAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithValidId_ReturnsPerson()
        {
            // Act
            var result = await _getPersonByIdUseCase.ExecuteAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("João Silva", result.Name);
            Assert.Equal(30, result.Age);
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = await _getPersonByIdUseCase.ExecuteAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreatePersonAsync_WithValidData_CreatesPerson()
        {
            // Arrange
            var personDto = new PersonDTO { Name = "Ana Paula", Age = 35 };

            // Act
            var result = await _createPersonUseCase.ExecuteAsync(personDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Ana Paula", result.Name);
            Assert.Equal(35, result.Age);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task CreatePersonAsync_WithDuplicateName_ThrowsException()
        {
            // Arrange
            var personDto = new PersonDTO { Name = "João Silva", Age = 25 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidPersonException>(() =>
                _createPersonUseCase.ExecuteAsync(personDto)
            );
        }

        [Fact]
        public async Task CreatePersonAsync_WithEmptyName_ThrowsException()
        {
            // Arrange
            var personDto = new PersonDTO { Name = "", Age = 25 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _createPersonUseCase.ExecuteAsync(personDto)
            );
        }

        [Fact]
        public async Task CreatePersonAsync_WithInvalidAge_ThrowsException()
        {
            // Arrange
            var personDto = new PersonDTO { Name = "Novo", Age = -5 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _createPersonUseCase.ExecuteAsync(personDto)
            );
        }

        [Fact]
        public async Task CreatePersonAsync_WithAgeOver150_ThrowsException()
        {
            // Arrange
            var personDto = new PersonDTO { Name = "Velho", Age = 151 };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _createPersonUseCase.ExecuteAsync(personDto)
            );
        }

        [Fact]
        public async Task UpdatePersonAsync_WithValidData_UpdatesPerson()
        {
            // Arrange
            var personDto = new PersonDTO { Name = "João Silva Atualizado", Age = 31 };

            // Act
            var result = await _updatePersonUseCase.ExecuteAsync(1, personDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("João Silva Atualizado", result.Name);
            Assert.Equal(31, result.Age);
        }

        [Fact]
        public async Task UpdatePersonAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var personDto = new PersonDTO { Name = "Alguém", Age = 40 };

            // Act
            var result = await _updatePersonUseCase.ExecuteAsync(999, personDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeletePersonAsync_WithValidId_DeletesPerson()
        {
            // Act
            var deleted = await _deletePersonUseCase.ExecuteAsync(1);

            // Assert
            Assert.True(deleted);
            var result = await _getPersonByIdUseCase.ExecuteAsync(1);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeletePersonAsync_WithInvalidId_ReturnsFalse()
        {
            // Act
            var deleted = await _deletePersonUseCase.ExecuteAsync(999);

            // Assert
            Assert.False(deleted);
        }
    }
}
