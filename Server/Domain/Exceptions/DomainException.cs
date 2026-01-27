namespace Server.Domain.Exceptions
{
    /// <summary>
    /// Exceção base para todas as exceções de domínio
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message) { }
    }
}
