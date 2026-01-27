using Server.Domain.Events;
using Server.Domain.Exceptions;
using Server.Domain.Specifications;
using Server.Domain.ValueObjects;

namespace Server.Domain.Aggregates
{
    /// <summary>
    /// Enum de tipo de transação (domínio)
    /// </summary>
    public enum TransactionTypeEnum
    {
        Expense,
        Income
    }

    /// <summary>
    /// Aggregate de Transação - root do agregado
    /// Contém toda a lógica de negócio relacionada a transações
    /// Implementa todas as regras de negócio críticas
    /// </summary>
    public class TransactionAggregate
    {
        private readonly List<DomainEvent> _events = new();

        public int Id { get; private set; }
        public Money Amount { get; private set; } = null!;
        public string Description { get; private set; } = string.Empty;
        public TransactionTypeEnum Type { get; private set; }
        public int CategoryId { get; private set; }
        public int PersonId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        // Construtor privado
        private TransactionAggregate() { }

        /// <summary>
        /// Factory method para criar uma transação com todas as validações de negócio
        /// </summary>
        public static TransactionAggregate Create(
            decimal amount,
            string description,
            TransactionTypeEnum type,
            int categoryId,
            int personId,
            bool isPersonMinor,
            bool categorySupportsType,
            int id = 0)
        {
            // Validação 1: Valor deve ser válido
            if (!TransactionSpecification.IsValidAmount(amount))
                throw new InvalidTransactionException("O valor da transação deve ser maior que zero");

            // Validação 2: Descrição deve ser válida
            if (!TransactionSpecification.HasValidDescription(description))
                throw new InvalidTransactionException("Descrição inválida ou não fornecida");

            // Validação 3: Tipo de transação deve ser válido
            if (!TransactionSpecification.IsValidTransactionType(type.ToString()))
                throw new InvalidTransactionException("Tipo de transação inválido");

            // Validação 4: REGRA DE NEGÓCIO CRÍTICA - Menores não podem registrar receitas
            if (isPersonMinor && type == TransactionTypeEnum.Income)
                throw new InvalidTransactionException("Menores de idade não podem registrar receitas diretamente");

            // Validação 5: Categoria deve suportar o tipo de transação
            if (!categorySupportsType)
                throw new InvalidTransactionException("A categoria não suporta este tipo de transação");

            // Criar Money (Value Object)
            var money = Money.Create(amount);

            var transaction = new TransactionAggregate
            {
                Id = id > 0 ? id : 0,
                Amount = money,
                Description = description.Trim(),
                Type = type,
                CategoryId = categoryId,
                PersonId = personId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Registrar evento de domínio
            if (id == 0)
            {
                transaction._events.Add(new TransactionCreatedEvent(
                    id,
                    personId,
                    categoryId,
                    amount,
                    type.ToString(),
                    description
                ));
            }

            return transaction;
        }

        /// <summary>
        /// Cancela a transação (soft delete)
        /// </summary>
        public void Cancel()
        {
            if (!IsActive)
                throw new InvalidTransactionException("A transação já está cancelada");

            IsActive = false;
        }

        /// <summary>
        /// Verifica se a transação é uma despesa
        /// </summary>
        public bool IsExpense => Type == TransactionTypeEnum.Expense;

        /// <summary>
        /// Verifica se a transação é uma receita
        /// </summary>
        public bool IsIncome => Type == TransactionTypeEnum.Income;

        /// <summary>
        /// Obtém os eventos de domínio não publicados
        /// </summary>
        public IReadOnlyList<DomainEvent> GetUncommittedEvents() => _events.AsReadOnly();

        /// <summary>
        /// Limpa os eventos após publicação
        /// </summary>
        public void ClearUncommittedEvents() => _events.Clear();
    }
}
