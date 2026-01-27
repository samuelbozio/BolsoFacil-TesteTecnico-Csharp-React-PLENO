using Server.Domain.Exceptions;
using Server.Domain.ValueObjects;

namespace Server.Domain.Aggregates
{
    /// <summary>
    /// Aggregate de Pessoa - root do agregado
    /// Contém toda a lógica de negócio relacionada a pessoas
    /// </summary>
    public class PersonAggregate
    {
        public int Id { get; private set; }
        public PersonName Name { get; private set; } = null!;
        public Age Age { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        // Coleção de IDs de transações (não carregamos as transações para manter o agregado leve)
        private readonly List<int> _transactionIds = new();

        // Construtor privado
        private PersonAggregate() { }

        /// <summary>
        /// Factory method para criar uma nova pessoa com validações
        /// </summary>
        public static PersonAggregate Create(string name, int age, int id = 0)
        {
            // Validar usando Value Objects
            var personName = PersonName.Create(name);
            var personAge = Age.Create(age);

            var person = new PersonAggregate
            {
                Id = id > 0 ? id : 0,
                Name = personName,
                Age = personAge,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return person;
        }

        /// <summary>
        /// Verifica se a pessoa é menor de idade
        /// </summary>
        public bool IsMinor => Age.IsMinor;

        /// <summary>
        /// Atualiza o nome da pessoa
        /// </summary>
        public void UpdateName(string newName)
        {
            if (!IsActive)
                throw new InvalidPersonException("Não é possível atualizar uma pessoa inativa");

            Name = PersonName.Create(newName);
        }

        /// <summary>
        /// Atualiza a idade da pessoa
        /// </summary>
        public void UpdateAge(int newAge)
        {
            if (!IsActive)
                throw new InvalidPersonException("Não é possível atualizar uma pessoa inativa");

            Age = Age.Create(newAge);
        }

        /// <summary>
        /// Desativa a pessoa
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidPersonException("A pessoa já está inativa");

            IsActive = false;
        }

        /// <summary>
        /// Registra uma transação na pessoa
        /// </summary>
        public void AddTransaction(int transactionId)
        {
            if (!IsActive)
                throw new InvalidPersonException("Não é possível adicionar transações a uma pessoa inativa");

            if (!_transactionIds.Contains(transactionId))
            {
                _transactionIds.Add(transactionId);
            }
        }

        /// <summary>
        /// Obtém a quantidade de transações
        /// </summary>
        public int GetTransactionCount() => _transactionIds.Count;

        /// <summary>
        /// Obtém as transações
        /// </summary>
        public IReadOnlyList<int> GetTransactionIds() => _transactionIds.AsReadOnly();
    }
}
