using Server.Contracts.Responses;

namespace Server.Contracts.Responses.Implementations
{
    /// <summary>
    /// Implementação padrão de resposta com dados
    /// </summary>
    public class ApiResponse<T> : IApiResponse<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T? Data { get; }

        public ApiResponse(T? data, string message = "Operação realizada com sucesso", bool success = true)
        {
            Data = data;
            Message = message;
            Success = success;
        }

        public static ApiResponse<T> Ok(T? data, string message = "Operação realizada com sucesso")
            => new(data, message, true);

        public static ApiResponse<T> Error(string message, T? data = default)
            => new(data, message, false);
    }

    /// <summary>
    /// Implementação padrão de resposta sem dados
    /// </summary>
    public class ApiResponse : IApiResponse
    {
        public bool Success { get; }
        public string Message { get; }

        public ApiResponse(string message = "Operação realizada com sucesso", bool success = true)
        {
            Message = message;
            Success = success;
        }

        public static ApiResponse Ok(string message = "Operação realizada com sucesso")
            => new(message, true);

        public static ApiResponse Error(string message)
            => new(message, false);
    }

    /// <summary>
    /// Implementação de resposta paginada
    /// </summary>
    public class PaginatedResponse<T> : IPaginatedResponse<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public IEnumerable<T>? Data { get; }
        public int TotalCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage { get; }

        public PaginatedResponse(
            IEnumerable<T>? data,
            int totalCount,
            int pageNumber,
            int pageSize,
            string message = "Operação realizada com sucesso")
        {
            Data = data;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            HasNextPage = pageNumber < TotalPages;
            HasPreviousPage = pageNumber > 1;
            Message = message;
            Success = true;
        }

        public static PaginatedResponse<T> Ok(
            IEnumerable<T> data,
            int totalCount,
            int pageNumber,
            int pageSize,
            string message = "Operação realizada com sucesso")
            => new(data, totalCount, pageNumber, pageSize, message);
    }

    /// <summary>
    /// Implementação de resposta de erro
    /// </summary>
    public class ErrorResponse : IErrorResponse
    {
        public bool Success { get; }
        public string Message { get; }
        public string? ErrorCode { get; }
        public IEnumerable<string>? Errors { get; }

        public ErrorResponse(
            string message,
            string? errorCode = null,
            IEnumerable<string>? errors = null)
        {
            Message = message;
            ErrorCode = errorCode;
            Errors = errors;
            Success = false;
        }

        public static ErrorResponse CreateFromException(Exception ex, string errorCode = "INTERNAL_ERROR")
            => new(ex.Message, errorCode, new[] { ex.Message });

        public static ErrorResponse CreateFromValidationErrors(IEnumerable<string> errors, string message = "Erro de validação")
            => new(message, "VALIDATION_ERROR", errors);
    }
}
