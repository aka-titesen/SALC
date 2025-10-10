using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Models;

namespace SALC.DataAccess.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD de m�tricas seg�n ERS v2.7
    /// Maneja la tabla: metricas
    /// </summary>
    public class MetricaRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public MetricaRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        #region Operaciones de Consulta (Read)

        /// <summary>
        /// Obtiene todas las m�tricas
        /// </summary>
        public List<Metrica> ObtenerTodas()
        {
            var metricas = new List<Metrica>();
            const string query = @"
                SELECT id_metrica, nombre, unidad_medida, valor_minimo, valor_maximo
                FROM metricas
                ORDER BY nombre";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        metricas.Add(MapearMetrica(reader));
                    }
                }
            }

            return metricas;
        }

        /// <summary>
        /// Obtiene una m�trica por ID
        /// </summary>
        public Metrica ObtenerPorId(int idMetrica)
        {
            const string query = @"
                SELECT id_metrica, nombre, unidad_medida, valor_minimo, valor_maximo
                FROM metricas
                WHERE id_metrica = @IdMetrica";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdMetrica", SqlDbType.Int).Value = idMetrica;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearMetrica(reader);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Busca m�tricas por nombre
        /// </summary>
        public List<Metrica> BuscarPorNombre(string nombre)
        {
            var metricas = new List<Metrica>();
            const string query = @"
                SELECT id_metrica, nombre, unidad_medida, valor_minimo, valor_maximo
                FROM metricas
                WHERE nombre LIKE @Nombre
                ORDER BY nombre";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = $"%{nombre}%";
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            metricas.Add(MapearMetrica(reader));
                        }
                    }
                }
            }

            return metricas;
        }

        #endregion

        #region Operaciones de Creaci�n (Create)

        /// <summary>
        /// Crea una nueva m�trica
        /// </summary>
        public bool Crear(Metrica metrica)
        {
            if (metrica == null || !metrica.EsValida())
                throw new ArgumentException("Datos de m�trica inv�lidos");

            const string query = @"
                INSERT INTO metricas (nombre, unidad_medida, valor_minimo, valor_maximo)
                VALUES (@Nombre, @UnidadMedida, @ValorMinimo, @ValorMaximo)";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = metrica.Nombre;
                    command.Parameters.Add("@UnidadMedida", SqlDbType.NVarChar, 20).Value = metrica.UnidadMedida;
                    command.Parameters.Add("@ValorMinimo", SqlDbType.Decimal).Value = metrica.ValorMinimo ?? (object)DBNull.Value;
                    command.Parameters.Add("@ValorMaximo", SqlDbType.Decimal).Value = metrica.ValorMaximo ?? (object)DBNull.Value;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Actualizaci�n (Update)

        /// <summary>
        /// Actualiza una m�trica existente
        /// </summary>
        public bool Actualizar(Metrica metrica)
        {
            if (metrica == null || !metrica.EsValida())
                throw new ArgumentException("Datos de m�trica inv�lidos");

            const string query = @"
                UPDATE metricas 
                SET nombre = @Nombre, unidad_medida = @UnidadMedida, 
                    valor_minimo = @ValorMinimo, valor_maximo = @ValorMaximo
                WHERE id_metrica = @IdMetrica";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdMetrica", SqlDbType.Int).Value = metrica.IdMetrica;
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = metrica.Nombre;
                    command.Parameters.Add("@UnidadMedida", SqlDbType.NVarChar, 20).Value = metrica.UnidadMedida;
                    command.Parameters.Add("@ValorMinimo", SqlDbType.Decimal).Value = metrica.ValorMinimo ?? (object)DBNull.Value;
                    command.Parameters.Add("@ValorMaximo", SqlDbType.Decimal).Value = metrica.ValorMaximo ?? (object)DBNull.Value;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Eliminaci�n (Delete)

        /// <summary>
        /// Elimina una m�trica (solo si no tiene an�lisis asociados)
        /// </summary>
        public bool Eliminar(int idMetrica)
        {
            // Verificar que no tenga an�lisis asociados
            if (TieneAnalisisAsociados(idMetrica))
                throw new InvalidOperationException("No se puede eliminar una m�trica que tiene an�lisis asociados");

            const string query = "DELETE FROM metricas WHERE id_metrica = @IdMetrica";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdMetrica", SqlDbType.Int).Value = idMetrica;
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region M�todos de Validaci�n

        /// <summary>
        /// Verifica si existe una m�trica con el ID especificado
        /// </summary>
        public bool ExistePorId(int idMetrica)
        {
            const string query = "SELECT COUNT(*) FROM metricas WHERE id_metrica = @IdMetrica";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdMetrica", SqlDbType.Int).Value = idMetrica;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si existe una m�trica con el nombre especificado
        /// </summary>
        public bool ExistePorNombre(string nombre)
        {
            const string query = "SELECT COUNT(*) FROM metricas WHERE LOWER(nombre) = LOWER(@Nombre)";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si existe otra m�trica con el mismo nombre
        /// </summary>
        public bool ExisteNombreEnOtraMetrica(string nombre, int idMetricaExcluida)
        {
            const string query = "SELECT COUNT(*) FROM metricas WHERE LOWER(nombre) = LOWER(@Nombre) AND id_metrica != @IdMetricaExcluida";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = nombre;
                    command.Parameters.Add("@IdMetricaExcluida", SqlDbType.Int).Value = idMetricaExcluida;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si la m�trica tiene an�lisis asociados
        /// </summary>
        public bool TieneAnalisisAsociados(int idMetrica)
        {
            const string query = "SELECT COUNT(*) FROM analisis_metrica WHERE id_metrica = @IdMetrica";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdMetrica", SqlDbType.Int).Value = idMetrica;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        #endregion

        #region M�todos Privados de Mapeo

        private Metrica MapearMetrica(SqlDataReader reader)
        {
            return new Metrica
            {
                IdMetrica = reader.GetInt32("id_metrica"),
                Nombre = reader.GetString("nombre"),
                UnidadMedida = reader.GetString("unidad_medida"),
                ValorMinimo = reader.IsDBNull("valor_minimo") ? (decimal?)null : reader.GetDecimal("valor_minimo"),
                ValorMaximo = reader.IsDBNull("valor_maximo") ? (decimal?)null : reader.GetDecimal("valor_maximo")
            };
        }

        #endregion
    }
}