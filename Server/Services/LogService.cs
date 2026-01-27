namespace Server.Services
{
    /// <summary>
    /// Interface para serviço de logging centralizado
    /// </summary>
    public interface ILogService
    {
        void LogInfo(string message, string? context = null, object? data = null);
        void LogDebug(string message, string? context = null, object? data = null);
        void LogWarning(string message, string? context = null, object? data = null);
        void LogError(string message, Exception? exception = null, string? context = null, object? data = null);
        IEnumerable<string> GetRecentLogs(int count = 100);
    }

    /// <summary>
    /// Implementação de serviço de logging
    /// </summary>
    public class LogService : ILogService
    {
        private readonly ILogger<LogService> _logger;
        private readonly Queue<string> _logHistory;
        private readonly int _maxHistorySize = 1000;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
            _logHistory = new Queue<string>();
            _logger.LogInformation("LogService inicializado");
        }

        /// <summary>
        /// Log de informação
        /// </summary>
        public void LogInfo(string message, string? context = null, object? data = null)
        {
            var logMessage = FormatLogMessage("INFO", message, context, data);
            _logger.LogInformation(logMessage);
            AddToHistory(logMessage);
        }

        /// <summary>
        /// Log de debug
        /// </summary>
        public void LogDebug(string message, string? context = null, object? data = null)
        {
            var logMessage = FormatLogMessage("DEBUG", message, context, data);
            _logger.LogDebug(logMessage);
            AddToHistory(logMessage);
        }

        /// <summary>
        /// Log de warning
        /// </summary>
        public void LogWarning(string message, string? context = null, object? data = null)
        {
            var logMessage = FormatLogMessage("WARN", message, context, data);
            _logger.LogWarning(logMessage);
            AddToHistory(logMessage);
        }

        /// <summary>
        /// Log de error
        /// </summary>
        public void LogError(string message, Exception? exception = null, string? context = null, object? data = null)
        {
            var logMessage = FormatLogMessage("ERROR", message, context, data);
            if (exception != null)
            {
                _logger.LogError(exception, logMessage);
            }
            else
            {
                _logger.LogError(logMessage);
            }
            AddToHistory(logMessage);
        }

        /// <summary>
        /// Formatar mensagem de log
        /// </summary>
        private string FormatLogMessage(string level, string message, string? context, object? data)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var contextStr = !string.IsNullOrEmpty(context) ? $"[{context}]" : "";
            var dataStr = data != null ? $" | Data: {System.Text.Json.JsonSerializer.Serialize(data)}" : "";
            
            return $"{timestamp} {level} {contextStr} {message}{dataStr}";
        }

        /// <summary>
        /// Adicionar ao histórico
        /// </summary>
        private void AddToHistory(string logMessage)
        {
            _logHistory.Enqueue(logMessage);
            if (_logHistory.Count > _maxHistorySize)
            {
                _logHistory.Dequeue();
            }
        }

        /// <summary>
        /// Obter logs recentes
        /// </summary>
        public IEnumerable<string> GetRecentLogs(int count = 100)
        {
            return _logHistory.TakeLast(count);
        }
    }
}
