export type LogLevel = 'debug' | 'info' | 'warn' | 'error';

export interface LogEntry {
  timestamp: string;
  level: LogLevel;
  message: string;
  context?: string;
  data?: any;
  error?: Error;
}

class LoggerService {
  private static isDevelopment = import.meta.env.DEV;
  private static logHistory: LogEntry[] = [];
  private static maxHistorySize = 1000;

  static debug(message: string, context?: string, data?: any) {
    if (this.isDevelopment) {
      this.log('debug', message, context, data);
    }
  }

  static info(message: string, context?: string, data?: any) {
    this.log('info', message, context, data);
  }

  static warn(message: string, context?: string, data?: any) {
    this.log('warn', message, context, data);
  }

  static error(message: string, context?: string, error?: any, data?: any) {
    this.log('error', message, context, { ...data, error });
  }

  private static log(level: LogLevel, message: string, context?: string, data?: any) {
    const entry: LogEntry = {
      timestamp: new Date().toISOString(),
      level,
      message,
      context: context || 'APP',
      data,
    };

    this.logHistory.push(entry);
    if (this.logHistory.length > this.maxHistorySize) {
      this.logHistory.shift();
    }

    const styles = this.getConsoleStyles(level);
    const contextStr = context ? `[${context}]` : '';
    const timestamp = new Date().toLocaleTimeString();

    console.log(
      `%c${timestamp} %c${level.toUpperCase()} %c${contextStr} %c${message}`,
      'color: gray; font-size: 12px',
      `${styles.color}; font-weight: bold`,
      'color: blue; font-size: 11px',
      'color: inherit'
    );

    if (data) {
      console.log('Data:', data);
    }
  }

  private static getConsoleStyles(level: LogLevel): { color: string } {
    const styles: Record<LogLevel, string> = {
      debug: 'color: #666',
      info: 'color: #0066cc',
      warn: 'color: #ff9900',
      error: 'color: #cc0000',
    };
    return { color: styles[level] };
  }

  static getHistory(): LogEntry[] {
    return [...this.logHistory];
  }

  static clearHistory() {
    this.logHistory = [];
  }

  static exportLogs(): string {
    return JSON.stringify(this.logHistory, null, 2);
  }

  static downloadLogs() {
    const logs = this.exportLogs();
    const element = document.createElement('a');
    element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(logs));
    element.setAttribute('download', `logs-${new Date().toISOString()}.json`);
    element.style.display = 'none';
    document.body.appendChild(element);
    element.click();
    document.body.removeChild(element);
  }
}

export default LoggerService;
