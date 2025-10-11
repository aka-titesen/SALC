using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Models;

namespace SALC.DataAccess.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD de obras sociales según ERS v2.7
    /// Maneja la tabla: obras_sociales
    /// </summary>
    public class ObraSocialRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public ObraSocialRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        #region Operaciones de Consulta (Read)

        /// <summary>
        /// Obtiene todas las obras sociales
        /// </summary>
        public List<ObraSocial> ObtenerTodas()
        {
            var obrasSociales = new List<ObraSocial>();
            const string query = "SELECT id_obra_social, cuit, nombre FROM obras_sociales ORDER BY nombre";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        obrasSociales.Add(new ObraSocial
                        {
                            IdObraSocial = reader.GetInt32("id_obra_social"),
                            Cuit = reader.GetString("cuit"),
                            Nombre = reader.GetString("nombre")
                        });
                    }
                }
            }

            return obrasSociales;
        }

        /// <summary>
        /// Obtiene una obra social específica por ID
        /// </summary>
        public ObraSocial ObtenerPorId(int idObraSocial)
        {
            const string query = "SELECT id_obra_social, cuit, nombre FROM obras_sociales WHERE id_obra_social = @IdObraSocial";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdObraSocial", SqlDbType.Int).Value = idObraSocial;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ObraSocial
                            {
                                IdObraSocial = reader.GetInt32("id_obra_social"),
                                Cuit = reader.GetString("cuit"),
                                Nombre = reader.GetString("nombre")
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene una obra social por CUIT
        /// </summary>
        public ObraSocial ObtenerPorCuit(string cuit)
        {
            const string query = "SELECT id_obra_social, cuit, nombre FROM obras_sociales WHERE cuit = @Cuit";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Cuit", SqlDbType.NVarChar, 13).Value = cuit;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ObraSocial
                            {
                                IdObraSocial = reader.GetInt32("id_obra_social"),
                                Cuit = reader.GetString("cuit"),
                                Nombre = reader.GetString("nombre")
                            };
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Operaciones de Creación (Create)

        /// <summary>
        /// Crea una nueva obra social
        /// </summary>
        public bool Crear(ObraSocial obraSocial)
        {
            if (obraSocial == null || !obraSocial.EsValida())
                throw new ArgumentException("Datos de obra social inválidos");

            const string query = @"
                INSERT INTO obras_sociales (cuit, nombre)
                VALUES (@Cuit, @Nombre)";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Cuit", SqlDbType.NVarChar, 13).Value = obraSocial.Cuit;
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 50).Value = obraSocial.Nombre;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Actualización (Update)

        /// <summary>
        /// Actualiza una obra social existente
        /// </summary>
        public bool Actualizar(ObraSocial obraSocial)
        {
            if (obraSocial == null || !obraSocial.EsValida())
                throw new ArgumentException("Datos de obra social inválidos");

            const string query = @"
                UPDATE obras_sociales 
                SET cuit = @Cuit, nombre = @Nombre
                WHERE id_obra_social = @IdObraSocial";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdObraSocial", SqlDbType.Int).Value = obraSocial.IdObraSocial;
                    command.Parameters.Add("@Cuit", SqlDbType.NVarChar, 13).Value = obraSocial.Cuit;
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 50).Value = obraSocial.Nombre;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Eliminación (Delete)

        /// <summary>
        /// Elimina una obra social (solo si no tiene pacientes asociados)
        /// </summary>
        public bool Eliminar(int idObraSocial)
        {
            // Verificar que no tenga pacientes asociados
            if (TienePacientesAsociados(idObraSocial))
                throw new InvalidOperationException("No se puede eliminar una obra social que tiene pacientes asociados");

            const string query = "DELETE FROM obras_sociales WHERE id_obra_social = @IdObraSocial";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdObraSocial", SqlDbType.Int).Value = idObraSocial;
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si existe una obra social con el ID especificado
        /// </summary>
        public bool ExistePorId(int idObraSocial)
        {
            const string query = "SELECT COUNT(*) FROM obras_sociales WHERE id_obra_social = @IdObraSocial";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdObraSocial", SqlDbType.Int).Value = idObraSocial;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si existe otra obra social con el mismo CUIT
        /// </summary>
        public bool ExisteCuitEnOtraObraSocial(string cuit, int idObraSocialExcluida)
        {
            const string query = "SELECT COUNT(*) FROM obras_sociales WHERE cuit = @Cuit AND id_obra_social != @IdObraSocialExcluida";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Cuit", SqlDbType.NVarChar, 13).Value = cuit;
                    command.Parameters.Add("@IdObraSocialExcluida", SqlDbType.Int).Value = idObraSocialExcluida;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si la obra social tiene pacientes asociados
        /// </summary>
        public bool TienePacientesAsociados(int idObraSocial)
        {
            const string query = "SELECT COUNT(*) FROM pacientes WHERE id_obra_social = @IdObraSocial";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdObraSocial", SqlDbType.Int).Value = idObraSocial;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        #endregion
    }
}