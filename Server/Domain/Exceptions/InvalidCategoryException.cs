namespace Server.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando uma categoria viola regras de negócio
    /// </summary>
    public class InvalidCategoryException : DomainException
    {
        public InvalidCategoryException(string message) : base(message) { }
    }
}
