using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Models;

namespace SALC.DataAccess.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD de roles según ERS v2.7
    /// Maneja la tabla: roles
    /// </summary>
    public class RolRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public RolRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        #region Operaciones de Consulta (Read)

        /// <summary>
        /// Obtiene todos los roles del sistema
        /// </summary>
        public List<Rol> ObtenerTodos()
        {
            var roles = new List<Rol>();
            const string query = "SELECT id_rol, nombre_rol FROM roles ORDER BY id_rol";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(new Rol
                        {
                            IdRol = reader.GetInt32("id_rol"),
                            NombreRol = reader.GetString("nombre_rol")
                        });
                    }
                }
            }

            return roles;
        }

        /// <summary>
        /// Obtiene un rol específico por ID
        /// </summary>
        public Rol ObtenerPorId(int idRol)
        {
            const string query = "SELECT id_rol, nombre_rol FROM roles WHERE id_rol = @IdRol";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdRol", SqlDbType.Int).Value = idRol;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rol
                            {
                                IdRol = reader.GetInt32("id_rol"),
                                NombreRol = reader.GetString("nombre_rol")
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene un rol específico por nombre
        /// </summary>
        public Rol ObtenerPorNombre(string nombreRol)
        {
            const string query = "SELECT id_rol, nombre_rol FROM roles WHERE LOWER(nombre_rol) = LOWER(@NombreRol)";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@NombreRol", SqlDbType.NVarChar, 50).Value = nombreRol;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Rol
                            {
                                IdRol = reader.GetInt32("id_rol"),
                                NombreRol = reader.GetString("nombre_rol")
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
        /// Crea un nuevo rol (solo para administradores del sistema)
        /// </summary>
        public bool Crear(Rol rol)
        {
            if (rol == null || !rol.EsValido())
                throw new ArgumentException("Datos de rol inválidos");

            const string query = "INSERT INTO roles (nombre_rol) VALUES (@NombreRol)";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@NombreRol", SqlDbType.NVarChar, 50).Value = rol.NombreRol;
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Actualización (Update)

        /// <summary>
        /// Actualiza un rol existente (solo para administradores del sistema)
        /// </summary>
        public bool Actualizar(Rol rol)
        {
            if (rol == null || !rol.EsValido())
                throw new ArgumentException("Datos de rol inválidos");

            const string query = "UPDATE roles SET nombre_rol = @NombreRol WHERE id_rol = @IdRol";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdRol", SqlDbType.Int).Value = rol.IdRol;
                    command.Parameters.Add("@NombreRol", SqlDbType.NVarChar, 50).Value = rol.NombreRol;
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si existe un rol con el ID especificado
        /// </summary>
        public bool ExistePorId(int idRol)
        {
            const string query = "SELECT COUNT(*) FROM roles WHERE id_rol = @IdRol";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdRol", SqlDbType.Int).Value = idRol;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si existe otro rol con el mismo nombre
        /// </summary>
        public bool ExisteNombreEnOtroRol(string nombreRol, int idRolExcluido)
        {
            const string query = "SELECT COUNT(*) FROM roles WHERE LOWER(nombre_rol) = LOWER(@NombreRol) AND id_rol != @IdRolExcluido";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@NombreRol", SqlDbType.NVarChar, 50).Value = nombreRol;
                    command.Parameters.Add("@IdRolExcluido", SqlDbType.Int).Value = idRolExcluido;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si el rol está siendo usado por algún usuario
        /// </summary>
        public bool EstaEnUso(int idRol)
        {
            const string query = "SELECT COUNT(*) FROM usuarios WHERE id_rol = @IdRol";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@IdRol", SqlDbType.Int).Value = idRol;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        #endregion
    }
}