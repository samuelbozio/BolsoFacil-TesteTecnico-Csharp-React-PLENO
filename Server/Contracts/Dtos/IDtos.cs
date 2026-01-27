namespace Server.Contracts.Dtos
{
    /// <summary>
    /// Interface para DTO de Transação no request
    /// </summary>
    public interface ITransactionRequest
    {
        decimal Amount { get; }
        string Description { get; }
        string Type { get; }
        int CategoryId { get; }
        int PersonId { get; }
    }

    /// <summary>
    /// Interface para DTO de Transação no response
    /// </summary>
    public interface ITransactionResponse
    {
        int Id { get; }
        decimal Amount { get; }
        string Currency { get; }
        string Description { get; }
        string Type { get; }
        int CategoryId { get; }
        string CategoryName { get; }
        int PersonId { get; }
        string PersonName { get; }
        DateTime CreatedAt { get; }
        bool IsActive { get; }
    }

    /// <summary>
    /// Interface para DTO de Pessoa no request
    /// </summary>
    public interface IPersonRequest
    {
        string Name { get; }
        int Age { get; }
    }

    /// <summary>
    /// Interface para DTO de Pessoa no response
    /// </summary>
    public interface IPersonResponse
    {
        int Id { get; }
        string Name { get; }
        int Age { get; }
        bool IsMinor { get; }
        bool IsActive { get; }
    }

    /// <summary>
    /// Interface para DTO de Categoria no request
    /// </summary>
    public interface ICategoryRequest
    {
        string Name { get; }
        string Description { get; }
        string SupportedTransactionTypes { get; }
    }

    /// <summary>
    /// Interface para DTO de Categoria no response
    /// </summary>
    public interface ICategoryResponse
    {
        int Id { get; }
        string Name { get; }
        string Description { get; }
        string SupportedTransactionTypes { get; }
        decimal TotalAmount { get; }
        int TransactionCount { get; }
        bool IsActive { get; }
    }
}
