using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Responses;
using Server.Contracts.Responses.Implementations;
using Server.Domain.Exceptions;
using System.Net;

namespace Server.Middleware
{
    /// <summary>
    /// Middleware para tratamento global de exceções e padronização de respostas
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Uma exceção não tratada ocorreu");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = ex switch
            {
                InvalidTransactionException => new
                {
                    success = false,
                    message = ex.Message,
                    errorCode = "INVALID_TRANSACTION",
                    statusCode = StatusCodes.Status400BadRequest
                },
                InvalidCategoryException => new
                {
                    success = false,
                    message = ex.Message,
                    errorCode = "INVALID_CATEGORY",
                    statusCode = StatusCodes.Status400BadRequest
                },
                InvalidPersonException => new
                {
                    success = false,
                    message = ex.Message,
                    errorCode = "INVALID_PERSON",
                    statusCode = StatusCodes.Status400BadRequest
                },
                DomainException => new
                {
                    success = false,
                    message = ex.Message,
                    errorCode = "DOMAIN_ERROR",
                    statusCode = StatusCodes.Status400BadRequest
                },
                _ => new
                {
                    success = false,
                    message = "Um erro interno ocorreu",
                    errorCode = "INTERNAL_ERROR",
                    statusCode = StatusCodes.Status500InternalServerError
                }
            };

            var statusCode = (int?)response.GetType().GetProperty("statusCode")?.GetValue(response) ?? StatusCodes.Status500InternalServerError;
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    /// <summary>
    /// Middleware para padronizar responses de sucesso
    /// </summary>
    public class ResponseFormattingMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseFormattingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    context.Response.Body = originalBodyStream;
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                else
                {
                    context.Response.Body = originalBodyStream;
                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
        }
    }
}
