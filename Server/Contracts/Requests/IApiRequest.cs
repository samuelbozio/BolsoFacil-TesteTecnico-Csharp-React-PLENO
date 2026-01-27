namespace Server.Contracts.Requests
{
    /// <summary>
    /// Interface para requisição padrão da API
    /// </summary>
    public interface IApiRequest
    {
        bool IsValid();
        IEnumerable<string> GetValidationErrors();
    }

    /// <summary>
    /// Interface para requisição com ID (para updates/deletes)
    /// </summary>
    public interface IApiRequestWithId : IApiRequest
    {
        int Id { get; }
    }

    /// <summary>
    /// Interface para requisição com paginação
    /// </summary>
    public interface IPaginatedRequest : IApiRequest
    {
        int PageNumber { get; }
        int PageSize { get; }
    }
}
