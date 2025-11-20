using System;
using System.IO;

namespace SALC.Infraestructura.Logging
{
    /// <summary>
    /// Niveles de severidad para los logs
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    /// <summary>
    /// Logger simple basado en archivos de texto
    /// Implementación ligera para .NET Framework 4.7.2 sin dependencias externas
    /// </summary>
    public class Logger
    {
        private static Logger _instance;
        private static readonly object _lock = new object();
        private readonly string _logDirectory;
        private readonly string _logFileName;
        private readonly bool _isEnabled;

        private Logger()
        {
            _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            _logFileName = $"SALC_{DateTime.Now:yyyyMMdd}.log";
            _isEnabled = true;

            // Crear directorio de logs si no existe
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
            }
            catch
            {
                _isEnabled = false;
            }
        }

        /// <summary>
        /// Obtiene la instancia única del logger (Singleton)
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Registra un mensaje de depuración
        /// </summary>
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Registra un mensaje informativo
        /// </summary>
        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Registra una advertencia
        /// </summary>
        public void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Registra un error
        /// </summary>
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Registra un error con excepción
        /// </summary>
        public void Error(string message, Exception exception)
        {
            var fullMessage = $"{message}\nException: {exception.GetType().Name}\nMessage: {exception.Message}\nStackTrace: {exception.StackTrace}";
            
            if (exception.InnerException != null)
            {
                fullMessage += $"\n\nInner Exception: {exception.InnerException.GetType().Name}\nMessage: {exception.InnerException.Message}\nStackTrace: {exception.InnerException.StackTrace}";
            }
            
            Log(LogLevel.Error, fullMessage);
        }

        /// <summary>
        /// Registra un error fatal
        /// </summary>
        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        /// <summary>
        /// Registra un error fatal con excepción
        /// </summary>
        public void Fatal(string message, Exception exception)
        {
            var fullMessage = $"{message}\nException: {exception.GetType().Name}\nMessage: {exception.Message}\nStackTrace: {exception.StackTrace}";
            
            if (exception.InnerException != null)
            {
                fullMessage += $"\n\nInner Exception: {exception.InnerException.GetType().Name}\nMessage: {exception.InnerException.Message}\nStackTrace: {exception.InnerException.StackTrace}";
            }
            
            Log(LogLevel.Fatal, fullMessage);
        }

        private void Log(LogLevel level, string message)
        {
            if (!_isEnabled)
                return;

            try
            {
                var logFilePath = Path.Combine(_logDirectory, _logFileName);
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var logEntry = $"[{timestamp}] [{level.ToString().ToUpper()}] {message}";

                lock (_lock)
                {
                    File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
                }
            }
            catch
            {
                // Si falla el logging, no queremos que afecte la aplicación
                // Silenciosamente ignoramos el error
            }
        }

        /// <summary>
        /// Limpia logs antiguos (más de X días)
        /// </summary>
        public void LimpiarLogsAntiguos(int diasRetencion = 30)
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                    return;

                var archivos = Directory.GetFiles(_logDirectory, "SALC_*.log");
                var fechaLimite = DateTime.Now.AddDays(-diasRetencion);

                foreach (var archivo in archivos)
                {
                    var archivoInfo = new FileInfo(archivo);
                    if (archivoInfo.CreationTime < fechaLimite)
                    {
                        File.Delete(archivo);
                    }
                }
            }
            catch
            {
                // Ignorar errores al limpiar logs antiguos
            }
        }
    }
}
