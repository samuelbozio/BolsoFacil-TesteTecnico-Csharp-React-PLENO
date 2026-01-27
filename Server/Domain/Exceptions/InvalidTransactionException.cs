namespace Server.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando uma transação viola regras de negócio
    /// </summary>
    public class InvalidTransactionException : DomainException
    {
        public InvalidTransactionException(string message) : base(message) { }
    }
}
