using System;
using System.Configuration;
using System.Data.SqlClient;

namespace SALC.DataAccess
{
    /// <summary>
    /// Clase centralizada para gestión de conexiones a la base de datos SALC
    /// Implementa el patrón Singleton para optimizar recursos
    /// </summary>
    public class DatabaseConnection
    {
        private static DatabaseConnection _instance;
        private readonly string _connectionString;
        private static readonly object _lock = new object();

        private DatabaseConnection()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada en app.config");
        }

        /// <summary>
        /// Obtiene la instancia única de DatabaseConnection
        /// </summary>
        public static DatabaseConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new DatabaseConnection();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Obtiene una nueva conexión a la base de datos
        /// </summary>
        /// <returns>Nueva instancia de SqlConnection</returns>
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Obtiene la cadena de conexión
        /// </summary>
        public string ConnectionString => _connectionString;

        /// <summary>
        /// Verifica si la conexión a la base de datos es válida
        /// </summary>
        /// <returns>True si se puede conectar, False en caso contrario</returns>
        public bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return connection.State == System.Data.ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ejecuta una consulta de prueba para verificar la estructura de la base de datos
        /// </summary>
        /// <returns>True si la estructura es válida</returns>
        public bool ValidateDatabaseStructure()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    
                    // Verificar existencia de tablas principales
                    string query = @"
                        SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME IN ('usuarios', 'medicos', 'asistentes', 'pacientes', 'analisis', 'analisis_metrica')";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        int tableCount = (int)command.ExecuteScalar();
                        return tableCount == 6; // Debe existir todas las tablas principales
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}