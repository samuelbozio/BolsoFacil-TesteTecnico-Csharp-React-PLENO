using Server.Domain.Events;
using Server.Domain.Exceptions;
using Server.Domain.Specifications;
using Server.Domain.ValueObjects;

namespace Server.Domain.Aggregates
{
    /// <summary>
    /// Enum de propósito da categoria (domínio)
    /// </summary>
    public enum CategoryPurposeType
    {
        Expense,
        Income,
        Both
    }

    /// <summary>
    /// Aggregate de Categoria - root do agregado
    /// Contém toda a lógica de negócio relacionada a categorias
    /// </summary>
    public class CategoryAggregate
    {
        private readonly List<DomainEvent> _events = new();

        public int Id { get; private set; }
        public CategoryDescription Description { get; private set; } = null!;
        public CategoryPurposeType Purpose { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        // Construtor privado para evitar instanciação direta incorreta
        private CategoryAggregate() { }

        /// <summary>
        /// Factory method para criar uma nova categoria com validações
        /// </summary>
        public static CategoryAggregate Create(string description, CategoryPurposeType purpose, int id = 0)
        {
            // Validar usando Value Objects
            var categoryDescription = CategoryDescription.Create(description);

            if (!Enum.IsDefined(typeof(CategoryPurposeType), purpose))
                throw new InvalidCategoryException("Tipo de propósito inválido");

            var category = new CategoryAggregate
            {
                Id = id > 0 ? id : 0, // ID 0 significa novo agregado
                Description = categoryDescription,
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Registrar evento de domínio
            if (id == 0)
            {
                category._events.Add(new CategoryCreatedEvent(
                    id,
                    description,
                    purpose.ToString()
                ));
            }

            return category;
        }

        /// <summary>
        /// Atualiza a descrição da categoria com validações
        /// </summary>
        public void UpdateDescription(string newDescription)
        {
            if (!IsActive)
                throw new InvalidCategoryException("Não é possível atualizar uma categoria inativa");

            var updatedDescription = CategoryDescription.Create(newDescription);
            Description = updatedDescription;
        }

        /// <summary>
        /// Desativa a categoria
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidCategoryException("A categoria já está inativa");

            IsActive = false;
        }

        /// <summary>
        /// Valida se a categoria pode ser usada com um tipo de transação
        /// </summary>
        public bool CanBeUsedWith(string transactionType)
        {
            return CategorySpecification.CanBeUsedWithTransactionType(Purpose.ToString(), transactionType);
        }

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
