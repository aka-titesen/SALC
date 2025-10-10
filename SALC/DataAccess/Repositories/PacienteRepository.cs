using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Models;

namespace SALC.DataAccess.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD de pacientes según ERS v2.7
    /// Maneja la tabla: pacientes
    /// </summary>
    public class PacienteRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public PacienteRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        #region Operaciones de Consulta (Read)

        /// <summary>
        /// Obtiene un paciente por DNI
        /// </summary>
        public Paciente ObtenerPorDni(int dni)
        {
            const string query = @"
                SELECT 
                    p.dni, p.nombre, p.apellido, p.fecha_nac, p.sexo, p.email, p.telefono, p.id_obra_social,
                    os.nombre as nombre_obra_social, os.cuit
                FROM pacientes p
                LEFT JOIN obras_sociales os ON p.id_obra_social = os.id_obra_social
                WHERE p.dni = @Dni";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = dni;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearPaciente(reader);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene todos los pacientes con filtros opcionales
        /// </summary>
        public List<Paciente> ObtenerTodos(string filtroNombre = "", int? filtroObraSocial = null)
        {
            var pacientes = new List<Paciente>();
            string query = @"
                SELECT 
                    p.dni, p.nombre, p.apellido, p.fecha_nac, p.sexo, p.email, p.telefono, p.id_obra_social,
                    os.nombre as nombre_obra_social, os.cuit
                FROM pacientes p
                LEFT JOIN obras_sociales os ON p.id_obra_social = os.id_obra_social
                WHERE 1=1";

            var parametros = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(filtroNombre))
            {
                query += " AND (p.nombre LIKE @FiltroNombre OR p.apellido LIKE @FiltroNombre OR CAST(p.dni AS NVARCHAR) LIKE @FiltroNombre)";
                parametros.Add(new SqlParameter("@FiltroNombre", SqlDbType.NVarChar, 100) { Value = $"%{filtroNombre}%" });
            }

            if (filtroObraSocial.HasValue)
            {
                query += " AND p.id_obra_social = @FiltroObraSocial";
                parametros.Add(new SqlParameter("@FiltroObraSocial", SqlDbType.Int) { Value = filtroObraSocial.Value });
            }

            query += " ORDER BY p.apellido, p.nombre";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddRange(parametros.ToArray());
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pacientes.Add(MapearPaciente(reader));
                        }
                    }
                }
            }

            return pacientes;
        }

        #endregion

        #region Operaciones de Creación (Create)

        /// <summary>
        /// Crea un nuevo paciente
        /// </summary>
        public bool Crear(Paciente paciente)
        {
            if (paciente == null || !paciente.EsValido())
                throw new ArgumentException("Datos de paciente inválidos");

            const string query = @"
                INSERT INTO pacientes (dni, nombre, apellido, fecha_nac, sexo, email, telefono, id_obra_social)
                VALUES (@Dni, @Nombre, @Apellido, @FechaNac, @Sexo, @Email, @Telefono, @IdObraSocial)";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = paciente.Dni;
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 50).Value = paciente.Nombre;
                    command.Parameters.Add("@Apellido", SqlDbType.NVarChar, 50).Value = paciente.Apellido;
                    command.Parameters.Add("@FechaNac", SqlDbType.Date).Value = paciente.FechaNacimiento;
                    command.Parameters.Add("@Sexo", SqlDbType.Char, 1).Value = paciente.Sexo;
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = paciente.Email ?? (object)DBNull.Value;
                    command.Parameters.Add("@Telefono", SqlDbType.NVarChar, 20).Value = paciente.Telefono ?? (object)DBNull.Value;
                    command.Parameters.Add("@IdObraSocial", SqlDbType.Int).Value = paciente.IdObraSocial ?? (object)DBNull.Value;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Actualización (Update)

        /// <summary>
        /// Actualiza un paciente existente
        /// </summary>
        public bool Actualizar(Paciente paciente)
        {
            if (paciente == null || !paciente.EsValido())
                throw new ArgumentException("Datos de paciente inválidos");

            const string query = @"
                UPDATE pacientes 
                SET nombre = @Nombre, apellido = @Apellido, fecha_nac = @FechaNac, 
                    sexo = @Sexo, email = @Email, telefono = @Telefono, id_obra_social = @IdObraSocial
                WHERE dni = @Dni";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = paciente.Dni;
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 50).Value = paciente.Nombre;
                    command.Parameters.Add("@Apellido", SqlDbType.NVarChar, 50).Value = paciente.Apellido;
                    command.Parameters.Add("@FechaNac", SqlDbType.Date).Value = paciente.FechaNacimiento;
                    command.Parameters.Add("@Sexo", SqlDbType.Char, 1).Value = paciente.Sexo;
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = paciente.Email ?? (object)DBNull.Value;
                    command.Parameters.Add("@Telefono", SqlDbType.NVarChar, 20).Value = paciente.Telefono ?? (object)DBNull.Value;
                    command.Parameters.Add("@IdObraSocial", SqlDbType.Int).Value = paciente.IdObraSocial ?? (object)DBNull.Value;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Eliminación (Delete)

        /// <summary>
        /// Elimina un paciente (solo si no tiene análisis asociados)
        /// </summary>
        public bool Eliminar(int dni)
        {
            // Verificar que no tenga análisis asociados
            if (TieneAnalisisAsociados(dni))
                throw new InvalidOperationException("No se puede eliminar un paciente que tiene análisis asociados");

            const string query = "DELETE FROM pacientes WHERE dni = @Dni";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = dni;
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si existe un paciente con el DNI especificado
        /// </summary>
        public bool ExistePorDni(int dni)
        {
            const string query = "SELECT COUNT(*) FROM pacientes WHERE dni = @Dni";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = dni;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si existe otro paciente con el mismo email
        /// </summary>
        public bool ExisteEmailEnOtroPaciente(string email, int dniExcluido)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            const string query = "SELECT COUNT(*) FROM pacientes WHERE LOWER(email) = LOWER(@Email) AND dni != @DniExcluido";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                    command.Parameters.Add("@DniExcluido", SqlDbType.Int).Value = dniExcluido;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// Verifica si el paciente tiene análisis asociados
        /// </summary>
        public bool TieneAnalisisAsociados(int dni)
        {
            const string query = "SELECT COUNT(*) FROM analisis WHERE dni_paciente = @Dni";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = dni;
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        #endregion

        #region Métodos Privados de Mapeo

        private Paciente MapearPaciente(SqlDataReader reader)
        {
            var paciente = new Paciente
            {
                Dni = reader.GetInt32("dni"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                FechaNacimiento = reader.GetDateTime("fecha_nac"),
                Sexo = reader.GetString("sexo"),
                Email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                Telefono = reader.IsDBNull("telefono") ? null : reader.GetString("telefono"),
                IdObraSocial = reader.IsDBNull("id_obra_social") ? (int?)null : reader.GetInt32("id_obra_social")
            };

            // Mapear obra social si existe
            if (!reader.IsDBNull("nombre_obra_social"))
            {
                paciente.ObraSocial = new ObraSocial
                {
                    IdObraSocial = paciente.IdObraSocial.Value,
                    Nombre = reader.GetString("nombre_obra_social"),
                    Cuit = reader.GetString("cuit")
                };
            }

            return paciente;
        }

        #endregion
    }
}