namespace Server.Domain.Events
{
    /// <summary>
    /// Evento disparado quando uma categoria é criada
    /// </summary>
    public class CategoryCreatedEvent : DomainEvent
    {
        public int CategoryId { get; }
        public string Description { get; }
        public string Purpose { get; }

        public CategoryCreatedEvent(int categoryId, string description, string purpose)
        {
            CategoryId = categoryId;
            Description = description;
            Purpose = purpose;
        }
    }
}
