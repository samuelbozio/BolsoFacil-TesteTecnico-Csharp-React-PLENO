namespace Server.Contracts.Responses
{
    /// <summary>
    /// Interface para resposta padrão da API
    /// Padroniza todas as respostas com sucesso/erro
    /// </summary>
    public interface IApiResponse
    {
        bool Success { get; }
        string Message { get; }
    }

    /// <summary>
    /// Interface para resposta com dados
    /// </summary>
    public interface IApiResponse<T> : IApiResponse
    {
        T? Data { get; }
    }

    /// <summary>
    /// Interface para resposta paginada
    /// </summary>
    public interface IPaginatedResponse<T> : IApiResponse<IEnumerable<T>>
    {
        int TotalCount { get; }
        int PageNumber { get; }
        int PageSize { get; }
        int TotalPages { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
    }

    /// <summary>
    /// Interface para resposta de erro
    /// </summary>
    public interface IErrorResponse : IApiResponse
    {
        string? ErrorCode { get; }
        IEnumerable<string>? Errors { get; }
    }
}
