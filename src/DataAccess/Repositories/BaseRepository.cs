using System;
using System.Data.SqlClient;
using System.Configuration;

namespace SALC.DataAccess.Repositories
{
    /// <summary>
    /// Clase base para todos los repositorios del sistema SALC.
    /// Implementa funcionalidad común de acceso a datos siguiendo principios SOLID.
    /// </summary>
    public abstract class BaseRepository : IDisposable
    {
        protected readonly string _connectionString;
        protected SqlConnection _connection;
        private bool _disposed = false;

        /// <summary>
        /// Constructor que inicializa la conexión a la base de datos
        /// </summary>
        protected BaseRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SALCDatabase"].ConnectionString;
        }

        /// <summary>
        /// Obtiene una conexión a la base de datos
        /// </summary>
        /// <returns>Conexión SQL activa</returns>
        protected SqlConnection GetConnection()
        {
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        /// <summary>
        /// Ejecuta un comando SQL con parámetros para prevenir inyección SQL
        /// </summary>
        /// <param name="query">Consulta SQL parametrizada</param>
        /// <param name="parameters">Parámetros de la consulta</param>
        /// <returns>Número de filas afectadas</returns>
        protected int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(query, GetConnection()))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL y retorna un SqlDataReader
        /// </summary>
        /// <param name="query">Consulta SQL parametrizada</param>
        /// <param name="parameters">Parámetros de la consulta</param>
        /// <returns>SqlDataReader con los resultados</returns>
        protected SqlDataReader ExecuteReader(string query, params SqlParameter[] parameters)
        {
            var command = new SqlCommand(query, GetConnection());
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            return command.ExecuteReader();
        }

        /// <summary>
        /// Ejecuta una consulta SQL y retorna un valor escalar
        /// </summary>
        /// <param name="query">Consulta SQL parametrizada</param>
        /// <param name="parameters">Parámetros de la consulta</param>
        /// <returns>Resultado escalar</returns>
        protected object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(query, GetConnection()))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Libera los recursos utilizados
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Patrón Dispose para liberar recursos
        /// </summary>
        /// <param name="disposing">Indica si se está liberando desde Dispose</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _connection?.Close();
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }
}
