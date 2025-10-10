using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Models;

namespace SALC.DataAccess.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD de tipos de análisis según ERS v2.7
    /// Maneja la tabla: tipos_analisis
    /// </summary>
    public class TipoAnalisisRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public TipoAnalisisRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        #region Operaciones de Consulta (Read)

        /// <summary>
        /// Obtiene todos los tipos de análisis
        /// </summary>
        public List<TipoAnalisis> ObtenerTodos()
        {
            var tiposAnalisis = new List<TipoAnalisis>();
            const string query = @"
                SELECT id_tipo_analisis, descripcion
                FROM tipos_analisis
                ORDER BY descripcion";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tiposAnalisis.Add(MapearTipoAnalisis(reader));
                    }
                }
            }

            return tiposAnalisis;
        }

        /// <summary>
        /// Obtiene un tipo de análisis por ID
        /// </summary>
        public TipoAnalisis ObtenerPorId(int idTipoAnalisis)
        {
            const string query = @"
                SELECT id_tipo_analisis, descripcion
                FROM tipos_analisis
                WHERE id_tipo_analisis = @IdTipoAnalisis";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdTipoAnalisis", SqlDbType.Int).Value = idTipoAnalisis;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearTipoAnalisis(reader);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Busca tipos de análisis por descripción
        /// </summary>
        public List<TipoAnalisis> BuscarPorDescripcion(string descripcion)
        {
            var tiposAnalisis = new List<TipoAnalisis>();
            const string query = @"
                SELECT id_tipo_analisis, descripcion
                FROM tipos_analisis
                WHERE descripcion LIKE @Descripcion
                ORDER BY descripcion";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 100).Value = $"%{descripcion}%";
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tiposAnalisis.Add(MapearTipoAnalisis(reader));
                        }
                    }
                }
            }

            return tiposAnalisis;
        }

        #endregion

        #region Operaciones de Creación (Create)

        /// <summary>
        /// Crea un nuevo tipo de análisis
        /// </summary>
        public bool Crear(TipoAnalisis tipoAnalisis)
        {
            if (tipoAnalisis == null || !tipoAnalisis.EsValido())
                throw new ArgumentException("Datos de tipo de análisis inválidos");

            const string query = @"
                INSERT INTO tipos_analisis (descripcion)
                VALUES (@Descripcion)";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 100).Value = tipoAnalisis.Descripcion;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Actualización (Update)

        /// <summary>
        /// Actualiza un tipo de análisis existente
        /// </summary>
        public bool Actualizar(TipoAnalisis tipoAnalisis)
        {
            if (tipoAnalisis == null || !tipoAnalisis.EsValido())
                throw new ArgumentException("Datos de tipo de análisis inválidos");

            const string query = @"
                UPDATE tipos_analisis 
                SET descripcion = @Descripcion
                WHERE id_tipo_analisis = @IdTipoAnalisis";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdTipoAnalisis", SqlDbType.Int).Value = tipoAnalisis.IdTipoAnalisis;
                    command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 100).Value = tipoAnalisis.Descripcion;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Eliminación (Delete)

        /// <summary>
        /// Elimina un tipo de análisis (solo si no tiene análisis asociados)
        /// </summary>
        public bool Eliminar(int idTipoAnalisis)
        {
            // Verificar que no tenga análisis asociados
            if (TieneAnalisisAsociados(idTipoAnalisis))
                throw new InvalidOperationException("No se puede eliminar un tipo de análisis que tiene análisis asociados");

            const string query = "DELETE FROM tipos_analisis WHERE id_tipo_analisis = @IdTipoAnalisis";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdTipoAnalisis", SqlDbType.Int).Value = idTipoAnalisis;
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si existe un tipo de análisis con el ID especificado
        /// </summary>
        public bool ExistePorId(int idTipoAnalisis)
        {
            const string query = "SELECT COUNT(*) FROM tipos_analisis WHERE id_tipo_analisis = @IdTipoAnalisis";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdTipoAnalisis", SqlDbType.Int).Value = idTipoAnalisis;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si existe un tipo de análisis con la descripción especificada
        /// </summary>
        public bool ExistePorDescripcion(string descripcion)
        {
            const string query = "SELECT COUNT(*) FROM tipos_analisis WHERE LOWER(descripcion) = LOWER(@Descripcion)";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 100).Value = descripcion;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si el tipo de análisis tiene análisis asociados
        /// </summary>
        public bool TieneAnalisisAsociados(int idTipoAnalisis)
        {
            const string query = "SELECT COUNT(*) FROM analisis WHERE id_tipo_analisis = @IdTipoAnalisis";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdTipoAnalisis", SqlDbType.Int).Value = idTipoAnalisis;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        #endregion

        #region Métodos Privados de Mapeo

        private TipoAnalisis MapearTipoAnalisis(SqlDataReader reader)
        {
            return new TipoAnalisis
            {
                IdTipoAnalisis = reader.GetInt32("id_tipo_analisis"),
                Descripcion = reader.GetString("descripcion")
            };
        }

        #endregion
    }
}