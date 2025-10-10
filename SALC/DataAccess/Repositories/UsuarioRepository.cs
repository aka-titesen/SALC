using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SALC.Models;

namespace SALC.DataAccess.Repositories
{
    /// <summary>
    /// Repositorio para operaciones CRUD de usuarios según la estructura normalizada ERS v2.7
    /// Maneja las tablas: usuarios, medicos, asistentes
    /// </summary>
    public class UsuarioRepository
    {
        private readonly DatabaseConnection _dbConnection;

        public UsuarioRepository()
        {
            _dbConnection = DatabaseConnection.Instance;
        }

        #region Operaciones de Consulta (Read)

        /// <summary>
        /// Obtiene un usuario por DNI con información completa según su rol
        /// </summary>
        public Usuario ObtenerPorDni(int dni)
        {
            const string query = @"
                SELECT 
                    u.dni, u.nombre, u.apellido, u.email, u.password_hash, u.id_rol, u.estado,
                    r.nombre_rol,
                    m.nro_matricula, m.especialidad,
                    a.dni_supervisor, a.fecha_ingreso,
                    supervisor.nombre + ' ' + supervisor.apellido as nombre_supervisor
                FROM usuarios u
                INNER JOIN roles r ON u.id_rol = r.id_rol
                LEFT JOIN medicos m ON u.dni = m.dni
                LEFT JOIN asistentes a ON u.dni = a.dni
                LEFT JOIN usuarios supervisor ON a.dni_supervisor = supervisor.dni
                WHERE u.dni = @Dni";

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
                            return MapearUsuario(reader);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene un usuario por email para autenticación
        /// </summary>
        public Usuario ObtenerPorEmail(string email)
        {
            const string query = @"
                SELECT 
                    u.dni, u.nombre, u.apellido, u.email, u.password_hash, u.id_rol, u.estado,
                    r.nombre_rol
                FROM usuarios u
                INNER JOIN roles r ON u.id_rol = r.id_rol
                WHERE LOWER(u.email) = LOWER(@Email) AND u.estado = 'Activo'";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearUsuarioBasico(reader);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene todos los usuarios con filtros opcionales
        /// </summary>
        public List<Usuario> ObtenerTodos(string filtroNombre = "", int? filtroRol = null)
        {
            var usuarios = new List<Usuario>();
            string query = @"
                SELECT 
                    u.dni, u.nombre, u.apellido, u.email, u.password_hash, u.id_rol, u.estado,
                    r.nombre_rol,
                    m.nro_matricula, m.especialidad,
                    a.dni_supervisor, a.fecha_ingreso
                FROM usuarios u
                INNER JOIN roles r ON u.id_rol = r.id_rol
                LEFT JOIN medicos m ON u.dni = m.dni
                LEFT JOIN asistentes a ON u.dni = a.dni
                WHERE 1=1";

            var parametros = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(filtroNombre))
            {
                query += " AND (u.nombre LIKE @FiltroNombre OR u.apellido LIKE @FiltroNombre OR CAST(u.dni AS NVARCHAR) LIKE @FiltroNombre)";
                parametros.Add(new SqlParameter("@FiltroNombre", SqlDbType.NVarChar, 100) { Value = $"%{filtroNombre}%" });
            }

            if (filtroRol.HasValue)
            {
                query += " AND u.id_rol = @FiltroRol";
                parametros.Add(new SqlParameter("@FiltroRol", SqlDbType.Int) { Value = filtroRol.Value });
            }

            query += " ORDER BY u.apellido, u.nombre";

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
                            usuarios.Add(MapearUsuario(reader));
                        }
                    }
                }
            }

            return usuarios;
        }

        /// <summary>
        /// Obtiene todos los médicos activos (para supervisores)
        /// </summary>
        public List<Medico> ObtenerMedicosActivos()
        {
            var medicos = new List<Medico>();
            const string query = @"
                SELECT 
                    u.dni, u.nombre, u.apellido, u.email, u.estado,
                    m.nro_matricula, m.especialidad
                FROM usuarios u
                INNER JOIN medicos m ON u.dni = m.dni
                WHERE u.estado = 'Activo'
                ORDER BY u.apellido, u.nombre";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        medicos.Add(new Medico
                        {
                            Dni = reader.GetInt32("dni"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                            Email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                            Estado = reader.GetString("estado"),
                            NumeroMatricula = reader.GetInt32("nro_matricula"),
                            Especialidad = reader.GetString("especialidad")
                        });
                    }
                }
            }

            return medicos;
        }

        #endregion

        #region Operaciones de Creación (Create)

        /// <summary>
        /// Crea un nuevo usuario con transacción para garantizar integridad
        /// </summary>
        public bool Crear(Usuario usuario)
        {
            if (usuario == null || !usuario.EsValido())
                throw new ArgumentException("Datos de usuario inválidos");

            var errorRol = usuario.ValidarDatosRol();
            if (!string.IsNullOrEmpty(errorRol))
                throw new ArgumentException(errorRol);

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Crear usuario base
                        const string queryUsuario = @"
                            INSERT INTO usuarios (dni, nombre, apellido, email, password_hash, id_rol, estado)
                            VALUES (@Dni, @Nombre, @Apellido, @Email, @PasswordHash, @IdRol, @Estado)";

                        using (var command = new SqlCommand(queryUsuario, connection, transaction))
                        {
                            command.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                            command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 50).Value = usuario.Nombre;
                            command.Parameters.Add("@Apellido", SqlDbType.NVarChar, 50).Value = usuario.Apellido;
                            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = usuario.Email ?? (object)DBNull.Value;
                            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = usuario.PasswordHash ?? "temp123";
                            command.Parameters.Add("@IdRol", SqlDbType.Int).Value = usuario.IdRol;
                            command.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = usuario.Estado ?? "Activo";

                            command.ExecuteNonQuery();
                        }

                        // 2. Crear registro específico según rol
                        if (usuario.EsMedico)
                        {
                            const string queryMedico = @"
                                INSERT INTO medicos (dni, nro_matricula, especialidad)
                                VALUES (@Dni, @NumeroMatricula, @Especialidad)";

                            using (var command = new SqlCommand(queryMedico, connection, transaction))
                            {
                                command.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                                command.Parameters.Add("@NumeroMatricula", SqlDbType.Int).Value = usuario.NumeroMatricula.Value;
                                command.Parameters.Add("@Especialidad", SqlDbType.NVarChar, 50).Value = usuario.Especialidad;
                                command.ExecuteNonQuery();
                            }
                        }
                        else if (usuario.EsAsistente)
                        {
                            const string queryAsistente = @"
                                INSERT INTO asistentes (dni, dni_supervisor, fecha_ingreso)
                                VALUES (@Dni, @DniSupervisor, @FechaIngreso)";

                            using (var command = new SqlCommand(queryAsistente, connection, transaction))
                            {
                                command.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                                command.Parameters.Add("@DniSupervisor", SqlDbType.Int).Value = usuario.DniSupervisor.Value;
                                command.Parameters.Add("@FechaIngreso", SqlDbType.Date).Value = usuario.FechaIngreso ?? DateTime.Now;
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Operaciones de Actualización (Update)

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        public bool Actualizar(Usuario usuario, bool cambiarPassword = false)
        {
            if (usuario == null || !usuario.EsValido())
                throw new ArgumentException("Datos de usuario inválidos");

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Actualizar usuario base
                        string queryUsuario = @"
                            UPDATE usuarios 
                            SET nombre = @Nombre, apellido = @Apellido, email = @Email, 
                                id_rol = @IdRol, estado = @Estado";

                        if (cambiarPassword)
                        {
                            queryUsuario += ", password_hash = @PasswordHash";
                        }

                        queryUsuario += " WHERE dni = @Dni";

                        using (var command = new SqlCommand(queryUsuario, connection, transaction))
                        {
                            command.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                            command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 50).Value = usuario.Nombre;
                            command.Parameters.Add("@Apellido", SqlDbType.NVarChar, 50).Value = usuario.Apellido;
                            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = usuario.Email ?? (object)DBNull.Value;
                            command.Parameters.Add("@IdRol", SqlDbType.Int).Value = usuario.IdRol;
                            command.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = usuario.Estado ?? "Activo";

                            if (cambiarPassword)
                            {
                                command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = usuario.PasswordHash;
                            }

                            command.ExecuteNonQuery();
                        }

                        // 2. Actualizar o crear datos específicos según rol
                        ActualizarDatosEspecificosPorRol(usuario, connection, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Cambia el estado de un usuario (Activo/Inactivo)
        /// </summary>
        public bool CambiarEstado(int dni, string nuevoEstado)
        {
            const string query = "UPDATE usuarios SET estado = @Estado WHERE dni = @Dni";

            using (var connection = _dbConnection.GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = dni;
                    command.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = nuevoEstado;

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        #endregion

        #region Operaciones de Eliminación (Delete)

        /// <summary>
        /// Elimina lógicamente un usuario (cambio de estado a Inactivo)
        /// </summary>
        public bool EliminarLogico(int dni)
        {
            return CambiarEstado(dni, "Inactivo");
        }

        /// <summary>
        /// Activa un usuario (cambio de estado a Activo)
        /// </summary>
        public bool Activar(int dni)
        {
            return CambiarEstado(dni, "Activo");
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si existe un usuario con el DNI especificado
        /// </summary>
        public bool ExistePorDni(int dni)
        {
            const string query = "SELECT COUNT(*) FROM usuarios WHERE dni = @Dni";

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
        /// Verifica si existe otro usuario con el mismo email
        /// </summary>
        public bool ExisteEmailEnOtroUsuario(string email, int dniExcluido)
        {
            const string query = "SELECT COUNT(*) FROM usuarios WHERE LOWER(email) = LOWER(@Email) AND dni != @DniExcluido";

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

        #endregion

        #region Métodos Privados de Mapeo

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            var usuario = new Usuario
            {
                Dni = reader.GetInt32("dni"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                PasswordHash = reader.GetString("password_hash"),
                IdRol = reader.GetInt32("id_rol"),
                Estado = reader.GetString("estado"),
                NombreRol = reader.GetString("nombre_rol")
            };

            // Mapear datos específicos de médico
            if (!reader.IsDBNull("nro_matricula"))
            {
                usuario.NumeroMatricula = reader.GetInt32("nro_matricula");
                usuario.Especialidad = reader.GetString("especialidad");
            }

            // Mapear datos específicos de asistente
            if (!reader.IsDBNull("dni_supervisor"))
            {
                usuario.DniSupervisor = reader.GetInt32("dni_supervisor");
                usuario.FechaIngreso = reader.GetDateTime("fecha_ingreso");
                if (!reader.IsDBNull("nombre_supervisor"))
                {
                    usuario.NombreSupervisor = reader.GetString("nombre_supervisor");
                }
            }

            return usuario;
        }

        private Usuario MapearUsuarioBasico(SqlDataReader reader)
        {
            return new Usuario
            {
                Dni = reader.GetInt32("dni"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Email = reader.IsDBNull("email") ? null : reader.GetString("email"),
                PasswordHash = reader.GetString("password_hash"),
                IdRol = reader.GetInt32("id_rol"),
                Estado = reader.GetString("estado"),
                NombreRol = reader.GetString("nombre_rol")
            };
        }

        private void ActualizarDatosEspecificosPorRol(Usuario usuario, SqlConnection connection, SqlTransaction transaction)
        {
            if (usuario.EsMedico)
            {
                // Verificar si ya existe registro en medicos
                const string checkMedico = "SELECT COUNT(*) FROM medicos WHERE dni = @Dni";
                using (var command = new SqlCommand(checkMedico, connection, transaction))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        // Actualizar
                        const string updateMedico = @"
                            UPDATE medicos 
                            SET nro_matricula = @NumeroMatricula, especialidad = @Especialidad
                            WHERE dni = @Dni";

                        using (var updateCmd = new SqlCommand(updateMedico, connection, transaction))
                        {
                            updateCmd.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                            updateCmd.Parameters.Add("@NumeroMatricula", SqlDbType.Int).Value = usuario.NumeroMatricula.Value;
                            updateCmd.Parameters.Add("@Especialidad", SqlDbType.NVarChar, 50).Value = usuario.Especialidad;
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Insertar
                        const string insertMedico = @"
                            INSERT INTO medicos (dni, nro_matricula, especialidad)
                            VALUES (@Dni, @NumeroMatricula, @Especialidad)";

                        using (var insertCmd = new SqlCommand(insertMedico, connection, transaction))
                        {
                            insertCmd.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                            insertCmd.Parameters.Add("@NumeroMatricula", SqlDbType.Int).Value = usuario.NumeroMatricula.Value;
                            insertCmd.Parameters.Add("@Especialidad", SqlDbType.NVarChar, 50).Value = usuario.Especialidad;
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            else if (usuario.EsAsistente)
            {
                // Verificar si ya existe registro en asistentes
                const string checkAsistente = "SELECT COUNT(*) FROM asistentes WHERE dni = @Dni";
                using (var command = new SqlCommand(checkAsistente, connection, transaction))
                {
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                    int count = (int)command.ExecuteScalar();

                    if (count > 0)
                    {
                        // Actualizar
                        const string updateAsistente = @"
                            UPDATE asistentes 
                            SET dni_supervisor = @DniSupervisor, fecha_ingreso = @FechaIngreso
                            WHERE dni = @Dni";

                        using (var updateCmd = new SqlCommand(updateAsistente, connection, transaction))
                        {
                            updateCmd.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                            updateCmd.Parameters.Add("@DniSupervisor", SqlDbType.Int).Value = usuario.DniSupervisor.Value;
                            updateCmd.Parameters.Add("@FechaIngreso", SqlDbType.Date).Value = usuario.FechaIngreso ?? DateTime.Now;
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Insertar
                        const string insertAsistente = @"
                            INSERT INTO asistentes (dni, dni_supervisor, fecha_ingreso)
                            VALUES (@Dni, @DniSupervisor, @FechaIngreso)";

                        using (var insertCmd = new SqlCommand(insertAsistente, connection, transaction))
                        {
                            insertCmd.Parameters.Add("@Dni", SqlDbType.Int).Value = usuario.Dni;
                            insertCmd.Parameters.Add("@DniSupervisor", SqlDbType.Int).Value = usuario.DniSupervisor.Value;
                            insertCmd.Parameters.Add("@FechaIngreso", SqlDbType.Date).Value = usuario.FechaIngreso ?? DateTime.Now;
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        #endregion
    }
}