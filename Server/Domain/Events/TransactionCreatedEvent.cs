namespace Server.Domain.Events
{
    /// <summary>
    /// Evento disparado quando uma transação é criada
    /// </summary>
    public class TransactionCreatedEvent : DomainEvent
    {
        public int TransactionId { get; }
        public int PersonId { get; }
        public int CategoryId { get; }
        public decimal Amount { get; }
        public string Type { get; }
        public string Description { get; }

        public TransactionCreatedEvent(int transactionId, int personId, int categoryId, decimal amount, string type, string description)
        {
            TransactionId = transactionId;
            PersonId = personId;
            CategoryId = categoryId;
            Amount = amount;
            Type = type;
            Description = description;
        }
    }
}
