namespace Server.Middleware
{
    /// <summary>
    /// Middleware para logar todas as requisições e respostas
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Logar requisição
            var requestId = context.TraceIdentifier;
            var method = context.Request.Method;
            var path = context.Request.Path;
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");

            _logger.LogInformation(
                "[{timestamp}] [{requestId}] {method} {path} | IP: {remoteIP}",
                timestamp,
                requestId,
                method,
                path,
                context.Connection.RemoteIpAddress
            );

            // Capturar corpo da requisição se existir
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                if (!string.IsNullOrWhiteSpace(body) && body.Length < 1000)
                {
                    _logger.LogDebug("[{requestId}] Request Body: {body}", requestId, body);
                }
                context.Request.Body.Position = 0;
            }

            // Capturar resposta
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                // Logar resposta
                var statusCode = context.Response.StatusCode;
                var duration = context.Items.ContainsKey("RequestDuration")
                    ? context.Items["RequestDuration"]
                    : "N/A";

                _logger.LogInformation(
                    "[{requestId}] Response: {statusCode} | Duration: {duration}ms",
                    requestId,
                    statusCode,
                    duration
                );

                // Copiar resposta de volta para o stream original
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);

            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[{requestId}] Exceção não tratada: {exceptionMessage}",
                    requestId,
                    ex.Message
                );
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }

    /// <summary>
    /// Middleware para medir duração da requisição
    /// </summary>
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTimingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            await _next(context);
            
            stopwatch.Stop();
            context.Items["RequestDuration"] = stopwatch.ElapsedMilliseconds;
        }
    }
}
