using Xunit;
using Server.Domain.ValueObjects;
using Server.Domain.Aggregates;
using Server.Domain.Exceptions;

namespace Server.Tests.Domain
{
    /// <summary>
    /// Testes unitários para Value Objects e Agregados
    /// Demonstra como testar lógica de domínio pura sem banco de dados
    /// </summary>
    public class TransactionAggregateTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Arrange
            decimal amount = 100;
            string description = "Compra no mercado";
            var type = TransactionTypeEnum.Expense;
            int categoryId = 1;
            int personId = 1;
            bool isPersonMinor = false;
            bool categorySupportsType = true;

            // Act
            var transaction = TransactionAggregate.Create(
                amount, description, type, categoryId, personId, isPersonMinor, categorySupportsType);

            // Assert
            Assert.NotNull(transaction);
            Assert.Equal("Compra no mercado", transaction.Description);
            Assert.Equal(TransactionTypeEnum.Expense, transaction.Type);
        }

        [Fact]
        public void Create_WithNegativeAmount_ShouldThrowException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<InvalidTransactionException>(() =>
                TransactionAggregate.Create(
                    -100, "Test", TransactionTypeEnum.Expense, 1, 1, false, true
                ));

            Assert.Equal("O valor da transação deve ser maior que zero", ex.Message);
        }

        [Fact]
        public void Create_WhenMinorTriesToCreateIncome_ShouldThrowException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<InvalidTransactionException>(() =>
                TransactionAggregate.Create(
                    100,
                    "Receita",
                    TransactionTypeEnum.Income,  // Tipo: Receita
                    1,
                    1,
                    true,  // É menor!
                    true
                ));

            Assert.Contains("Menores de idade não podem registrar receitas", ex.Message);
        }

        [Fact]
        public void Create_WithIncompatibleCategory_ShouldThrowException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<InvalidTransactionException>(() =>
                TransactionAggregate.Create(
                    100,
                    "Test",
                    TransactionTypeEnum.Expense,
                    1,
                    1,
                    false,
                    false  // Categoria não suporta!
                ));

            Assert.Contains("categoria não suporta", ex.Message);
        }

        [Fact]
        public void IsExpense_ShouldReturnCorrectValue()
        {
            // Arrange
            var transaction = TransactionAggregate.Create(
                100, "Test", TransactionTypeEnum.Expense, 1, 1, false, true);

            // Assert
            Assert.True(transaction.IsExpense);
            Assert.False(transaction.IsIncome);
        }

        [Fact]
        public void Cancel_ShouldMarkAsInactive()
        {
            // Arrange
            var transaction = TransactionAggregate.Create(
                100, "Test", TransactionTypeEnum.Expense, 1, 1, false, true);

            // Act
            transaction.Cancel();

            // Assert
            Assert.False(transaction.IsActive);
        }
    }

    /// <summary>
    /// Testes para Value Objects
    /// </summary>
    public class MoneyValueObjectTests
    {
        [Fact]
        public void Create_WithPositiveAmount_ShouldSucceed()
        {
            // Act
            var money = Money.Create(100);

            // Assert
            Assert.Equal(100, money.Amount);
            Assert.Equal("BRL", money.Currency);
            Assert.True(money.IsPositive);
        }

        [Fact]
        public void Create_WithZeroAmount_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Money.Create(0));
            Assert.Contains("valor deve ser maior que zero", ex.Message);
        }

        [Fact]
        public void Add_ShouldReturnCorrectSum()
        {
            // Arrange
            var money1 = Money.Create(100);
            var money2 = Money.Create(50);

            // Act
            var result = money1.Add(money2);

            // Assert
            Assert.Equal(150, result.Amount);
        }

        [Fact]
        public void Subtract_ShouldReturnCorrectDifference()
        {
            // Arrange
            var money1 = Money.Create(100);
            var money2 = Money.Create(30);

            // Act
            var result = money1.Subtract(money2);

            // Assert
            Assert.Equal(70, result.Amount);
        }
    }

    /// <summary>
    /// Testes para Age Value Object
    /// </summary>
    public class AgeValueObjectTests
    {
        [Fact]
        public void Create_WithValidAge_ShouldSucceed()
        {
            // Act
            var age = Age.Create(25);

            // Assert
            Assert.Equal(25, age.Value);
            Assert.False(age.IsMinor);
            Assert.True(age.IsAdult);
        }

        [Fact]
        public void Create_WithMinorAge_ShouldMarkAsMinor()
        {
            // Act
            var age = Age.Create(15);

            // Assert
            Assert.Equal(15, age.Value);
            Assert.True(age.IsMinor);
            Assert.False(age.IsAdult);
        }

        [Fact]
        public void Create_WithInvalidAge_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Age.Create(-5));
            Assert.Throws<ArgumentException>(() => Age.Create(151));
        }
    }

    /// <summary>
    /// Testes para Category Aggregate
    /// </summary>
    public class CategoryAggregateTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Act
            var category = CategoryAggregate.Create("Alimentação", CategoryPurposeType.Both);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("Alimentação", category.Description.Value);
        }

        [Fact]
        public void CanBeUsedWith_ShouldValidateTransactionType()
        {
            // Arrange
            var categoryExpense = CategoryAggregate.Create("Transporte", CategoryPurposeType.Expense);
            var categoryBoth = CategoryAggregate.Create("Geral", CategoryPurposeType.Both);

            // Assert
            Assert.True(categoryExpense.CanBeUsedWith("Expense"));
            Assert.False(categoryExpense.CanBeUsedWith("Income"));

            Assert.True(categoryBoth.CanBeUsedWith("Expense"));
            Assert.True(categoryBoth.CanBeUsedWith("Income"));
        }
    }

    /// <summary>
    /// Testes para Person Aggregate
    /// </summary>
    public class PersonAggregateTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Act
            var person = PersonAggregate.Create("João Silva", 25);

            // Assert
            Assert.NotNull(person);
            Assert.Equal("João Silva", person.Name.Value);
            Assert.Equal(25, person.Age.Value);
            Assert.False(person.IsMinor);
        }

        [Fact]
        public void Create_WithMinorAge_ShouldMarkAsMinor()
        {
            // Act
            var person = PersonAggregate.Create("Maria", 16);

            // Assert
            Assert.True(person.IsMinor);
        }

        [Fact]
        public void UpdateAge_ShouldChangeAge()
        {
            // Arrange
            var person = PersonAggregate.Create("João", 20);

            // Act
            person.UpdateAge(25);

            // Assert
            Assert.Equal(25, person.Age.Value);
        }

        [Fact]
        public void AddTransaction_ShouldIncreaseCount()
        {
            // Arrange
            var person = PersonAggregate.Create("João", 20);

            // Act
            person.AddTransaction(1);
            person.AddTransaction(2);

            // Assert
            Assert.Equal(2, person.GetTransactionCount());
        }
    }
}
