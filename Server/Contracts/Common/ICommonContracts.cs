namespace Server.Contracts.Common
{
    /// <summary>
    /// Interface padrão para todos os DTOs
    /// Define o contrato que todos os Data Transfer Objects devem seguir
    /// </summary>
    public interface IDto
    {
        /// <summary>
        /// Valida o DTO
        /// </summary>
        bool IsValid();

        /// <summary>
        /// Retorna mensagens de validação
        /// </summary>
        IEnumerable<string> GetValidationMessages();
    }

    /// <summary>
    /// Interface para DTOs com ID
    /// </summary>
    public interface IDtoWithId : IDto
    {
        int Id { get; }
    }

    /// <summary>
    /// Interface base para resultado de operações
    /// </summary>
    public interface IResult
    {
        bool IsSuccess { get; }
        string Message { get; }
    }

    /// <summary>
    /// Interface para resultado com dados
    /// </summary>
    public interface IResult<T> : IResult
    {
        T? Value { get; }
    }

    /// <summary>
    /// Interface para requisição de login
    /// </summary>
    public interface ILoginRequest
    {
        string Username { get; }
        string Password { get; }
    }

    /// <summary>
    /// Interface para resposta de login
    /// </summary>
    public interface ILoginResponse
    {
        string Token { get; }
        string Username { get; }
        DateTime ExpiresAt { get; }
    }
}
