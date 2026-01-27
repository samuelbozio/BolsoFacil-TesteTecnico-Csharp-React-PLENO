using Server.Contracts.Responses;
using System.Collections.Generic;

namespace Server.Contracts.Responses.Implementations
{
    public class ValidationErrorResponse : IErrorResponse
    {
        public bool Success => false;
        public string Message { get; }
        public string? ErrorCode { get; }
        public IEnumerable<string>? Errors { get; }

        public ValidationErrorResponse(string message, IEnumerable<string> errors, string? errorCode = null)
        {
            Message = message;
            Errors = errors;
            ErrorCode = errorCode;
        }

        public static ValidationErrorResponse Create(string message, IEnumerable<string> errors, string? errorCode = null)
        {
            return new ValidationErrorResponse(message, errors, errorCode);
        }
    }
}
