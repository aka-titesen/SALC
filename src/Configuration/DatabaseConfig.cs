using System;
using System.Configuration;

namespace SALC.Configuration
{
    /// <summary>
    /// Clase para gestionar la configuración de la base de datos.
    /// Centraliza el acceso a los parámetros de conexión.
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// Nombre de la cadena de conexión en el archivo de configuración
        /// </summary>
        public const string ConnectionStringName = "SALCDatabase";

        /// <summary>
        /// Obtiene la cadena de conexión a la base de datos
        /// </summary>
        /// <returns>Cadena de conexión</returns>
        /// <exception cref="InvalidOperationException">Si no se encuentra la cadena de conexión</exception>
        public static string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"No se encontró la cadena de conexión '{ConnectionStringName}' en el archivo de configuración.");
            }

            return connectionString;
        }

        /// <summary>
        /// Obtiene el timeout para comandos de base de datos
        /// </summary>
        /// <returns>Timeout en segundos</returns>
        public static int GetCommandTimeout()
        {
            var timeoutSetting = ConfigurationManager.AppSettings["DatabaseCommandTimeout"];

            if (int.TryParse(timeoutSetting, out int timeout))
            {
                return timeout;
            }

            return 30; // Valor por defecto
        }

        /// <summary>
        /// Verifica si la configuración de base de datos es válida
        /// </summary>
        /// <returns>True si la configuración es válida</returns>
        public static bool IsConfigurationValid()
        {
            try
            {
                var connectionString = GetConnectionString();
                return !string.IsNullOrWhiteSpace(connectionString);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el nombre del servidor de base de datos desde la cadena de conexión
        /// </summary>
        /// <returns>Nombre del servidor</returns>
        public static string GetServerName()
        {
            try
            {
                var connectionString = GetConnectionString();
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                return builder.DataSource;
            }
            catch
            {
                return "Desconocido";
            }
        }

        /// <summary>
        /// Obtiene el nombre de la base de datos desde la cadena de conexión
        /// </summary>
        /// <returns>Nombre de la base de datos</returns>
        public static string GetDatabaseName()
        {
            try
            {
                var connectionString = GetConnectionString();
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                return builder.InitialCatalog;
            }
            catch
            {
                return "Desconocido";
            }
        }
    }
}
