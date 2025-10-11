using System;
using System.Configuration;

namespace SALC.Configuration
{
    /// <summary>
    /// Clase para gestionar la configuración general de la aplicación.
    /// Centraliza el acceso a los parámetros de configuración del App.config.
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Obtiene el título de la aplicación
        /// </summary>
        public static string ApplicationTitle =>
            GetSetting("ApplicationTitle", Common.Constants.SALCConstants.UI.AppTitle);

        /// <summary>
        /// Obtiene la versión de la aplicación
        /// </summary>
        public static string ApplicationVersion =>
            GetSetting("ApplicationVersion", "2.7");

        /// <summary>
        /// Obtiene el formato de fecha por defecto
        /// </summary>
        public static string DefaultDateFormat =>
            GetSetting("DefaultDateFormat", Common.Constants.SALCConstants.UI.DefaultDateFormat);

        /// <summary>
        /// Obtiene el formato de fecha y hora por defecto
        /// </summary>
        public static string DefaultDateTimeFormat =>
            GetSetting("DefaultDateTimeFormat", Common.Constants.SALCConstants.UI.DefaultDateTimeFormat);

        /// <summary>
        /// Obtiene la ruta por defecto para los reportes PDF
        /// </summary>
        public static string ReportsPath =>
            GetSetting("ReportsPath", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SALC\\Reportes");

        /// <summary>
        /// Obtiene la ruta por defecto para las copias de seguridad
        /// </summary>
        public static string BackupPath =>
            GetSetting("BackupPath", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SALC\\Backups");

        /// <summary>
        /// Obtiene si el logging está habilitado
        /// </summary>
        public static bool LoggingEnabled =>
            GetBooleanSetting("LoggingEnabled", true);

        /// <summary>
        /// Obtiene el nivel de logging
        /// </summary>
        public static string LogLevel =>
            GetSetting("LogLevel", "Info");

        /// <summary>
        /// Obtiene la ruta del archivo de log
        /// </summary>
        public static string LogFilePath =>
            GetSetting("LogFilePath", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SALC\\Logs\\salc.log");

        /// <summary>
        /// Obtiene el timeout de sesión en minutos
        /// </summary>
        public static int SessionTimeoutMinutes =>
            GetIntegerSetting("SessionTimeoutMinutes", 60);

        /// <summary>
        /// Obtiene si el modo debug está habilitado
        /// </summary>
        public static bool DebugMode =>
            GetBooleanSetting("DebugMode", false);

        /// <summary>
        /// Obtiene el servidor SMTP para envío de emails
        /// </summary>
        public static string SmtpServer =>
            GetSetting("SmtpServer", "localhost");

        /// <summary>
        /// Obtiene el puerto SMTP
        /// </summary>
        public static int SmtpPort =>
            GetIntegerSetting("SmtpPort", 587);

        /// <summary>
        /// Obtiene si el SMTP requiere SSL
        /// </summary>
        public static bool SmtpUseSsl =>
            GetBooleanSetting("SmtpUseSsl", true);

        /// <summary>
        /// Obtiene el email del remitente para notificaciones
        /// </summary>
        public static string SenderEmail =>
            GetSetting("SenderEmail", "noreply@salc.com");

        /// <summary>
        /// Método auxiliar para obtener un setting como string
        /// </summary>
        /// <param name="key">Clave del setting</param>
        /// <param name="defaultValue">Valor por defecto</param>
        /// <returns>Valor del setting</returns>
        private static string GetSetting(string key, string defaultValue)
        {
            try
            {
                return ConfigurationManager.AppSettings[key] ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Método auxiliar para obtener un setting como boolean
        /// </summary>
        /// <param name="key">Clave del setting</param>
        /// <param name="defaultValue">Valor por defecto</param>
        /// <returns>Valor del setting</returns>
        private static bool GetBooleanSetting(string key, bool defaultValue)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                return bool.TryParse(value, out bool result) ? result : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Método auxiliar para obtener un setting como integer
        /// </summary>
        /// <param name="key">Clave del setting</param>
        /// <param name="defaultValue">Valor por defecto</param>
        /// <returns>Valor del setting</returns>
        private static int GetIntegerSetting(string key, int defaultValue)
        {
            try
            {
                var value = ConfigurationManager.AppSettings[key];
                return int.TryParse(value, out int result) ? result : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
