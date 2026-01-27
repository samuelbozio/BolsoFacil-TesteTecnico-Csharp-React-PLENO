namespace Server.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando uma pessoa viola regras de negócio
    /// </summary>
    public class InvalidPersonException : DomainException
    {
        public InvalidPersonException(string message) : base(message) { }
    }
}
