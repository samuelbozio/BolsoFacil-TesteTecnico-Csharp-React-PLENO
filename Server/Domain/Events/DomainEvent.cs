namespace Server.Domain.Events
{
    /// <summary>
    /// Base para todos os eventos de domínio
    /// </summary>
    public abstract class DomainEvent
    {
        public DateTime OccurredAt { get; }
        public Guid EventId { get; }

        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            OccurredAt = DateTime.UtcNow;
        }
    }
}
