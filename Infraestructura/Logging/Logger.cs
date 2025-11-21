using System;
using System.IO;

namespace SALC.Infraestructura.Logging
{
    /// <summary>
    /// Niveles de severidad para los logs del sistema
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Información de depuración para diagnóstico detallado
        /// </summary>
        Debug,

        /// <summary>
        /// Información general sobre el flujo de la aplicación
        /// </summary>
        Info,

        /// <summary>
        /// Situaciones potencialmente problemáticas que no impiden la operación
        /// </summary>
        Warning,

        /// <summary>
        /// Errores que afectan la funcionalidad pero permiten continuar
        /// </summary>
        Error,

        /// <summary>
        /// Errores críticos que pueden causar el cierre de la aplicación
        /// </summary>
        Fatal
    }

    /// <summary>
    /// Logger simple basado en archivos de texto.
    /// Implementa el patrón Singleton para proporcionar un punto centralizado de logging.
    /// Los logs se guardan en archivos diarios en la carpeta Logs.
    /// </summary>
    public class Logger
    {
        private static Logger _instance;
        private static readonly object _lock = new object();
        private readonly string _logDirectory;
        private readonly string _logFileName;
        private readonly bool _isEnabled;

        /// <summary>
        /// Constructor privado para implementar el patrón Singleton
        /// </summary>
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
        /// Obtiene la instancia única del logger
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
        /// <param name="message">Mensaje a registrar</param>
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Registra un mensaje informativo
        /// </summary>
        /// <param name="message">Mensaje a registrar</param>
        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Registra una advertencia
        /// </summary>
        /// <param name="message">Mensaje de advertencia</param>
        public void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Registra un error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Registra un error con información detallada de la excepción
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="exception">Excepción que causó el error</param>
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
        /// <param name="message">Mensaje de error fatal</param>
        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        /// <summary>
        /// Registra un error fatal con información detallada de la excepción
        /// </summary>
        /// <param name="message">Mensaje de error fatal</param>
        /// <param name="exception">Excepción que causó el error fatal</param>
        public void Fatal(string message, Exception exception)
        {
            var fullMessage = $"{message}\nException: {exception.GetType().Name}\nMessage: {exception.Message}\nStackTrace: {exception.StackTrace}";
            
            if (exception.InnerException != null)
            {
                fullMessage += $"\n\nInner Exception: {exception.InnerException.GetType().Name}\nMessage: {exception.InnerException.Message}\nStackTrace: {exception.InnerException.StackTrace}";
            }
            
            Log(LogLevel.Fatal, fullMessage);
        }

        /// <summary>
        /// Escribe una entrada en el archivo de log
        /// </summary>
        /// <param name="level">Nivel de severidad del mensaje</param>
        /// <param name="message">Mensaje a registrar</param>
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
        /// Elimina archivos de log más antiguos que el período de retención especificado
        /// </summary>
        /// <param name="diasRetencion">Cantidad de días de retención de logs</param>
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
