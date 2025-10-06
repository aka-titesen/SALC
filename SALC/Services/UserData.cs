using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SALC
{
    public static class UserData
    {
        private static string GetConnectionString()
        {
            var cs = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(cs))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");
            return cs;
        }

        public static Dictionary<int, string> LoadRoles()
        {
            string query = "SELECT id_rol, rol FROM rol ORDER BY rol";
            var roles = new Dictionary<int, string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idRol = reader.GetInt32(0);
                            string rol = reader.GetString(1);
                            roles[idRol] = rol;
                        }
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Error al buscar la tabla rol.");
            }

            return roles;
        }

        public static List<Usuario> GetUsers(string searchFilter = "", string roleFilter = "")
        {
            string query = @"
                SELECT u.dni, u.nombre, u.apellido, u.email, u.telefono, r.rol, u.estado_usuario AS id_estado, e.estado AS estado
                FROM usuario u
                INNER JOIN rol r ON u.id_rol = r.id_rol
                LEFT  JOIN estado_usuario e ON u.estado_usuario = e.id_estado
                WHERE 1=1";

            if (!string.IsNullOrEmpty(searchFilter))
            {
                query += " AND (u.nombre LIKE @Search OR u.apellido LIKE @Search OR CAST(u.dni AS NVARCHAR) LIKE @Search)";
            }

            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "Todos los roles")
            {
                query += " AND r.rol = @Role";
            }

            query += " ORDER BY u.apellido, u.nombre";

            var list = new List<Usuario>();

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchFilter))
                            command.Parameters.AddWithValue("@Search", $"%{searchFilter}%");

                        if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "Todos los roles")
                            command.Parameters.AddWithValue("@Role", roleFilter);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new Usuario
                                {
                                    Dni = reader.GetInt32(0),
                                    Nombre = reader.GetString(1),
                                    Apellido = reader.GetString(2),
                                    Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Rol = reader.GetString(5),
                                    // CORREGIDO: Manejar NULL en estado_usuario
                                    estado_usuario = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                    Estado = reader.IsDBNull(7) ? "" : reader.GetString(7)

                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                throw; 
            }

            return list;
        }

        public static bool DeleteUser(int dni)
        {
            string query = "UPDATE usuario SET estado_usuario = 2 WHERE dni = @Dni";

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Dni", dni);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch
            {
                throw; 
            }
        }

        public static bool ActivateUser(int dni)
        {
            string query = "UPDATE usuario SET estado_usuario = 1 WHERE dni = @Dni";

            try
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Dni", dni);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public static void CreateUser(Usuario userCreate)
        {
                string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception("Cadena de conexión no encontrada.");

                const string query = @"
            INSERT INTO usuario (dni, nombre, apellido, email, telefono, password, id_rol)
            VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono, @Password, @IdRol)";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Dni", userCreate.Dni);
                        command.Parameters.AddWithValue("@Nombre", userCreate.Nombre);
                        command.Parameters.AddWithValue("@Apellido", userCreate.Apellido);
                        command.Parameters.AddWithValue("@Email", userCreate.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Telefono", userCreate.Telefono ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Password", userCreate.password);
                        command.Parameters.AddWithValue("@IdRol", userCreate.id_rol);

                        command.ExecuteNonQuery();
                    }
                }
            }
        public static void UpdateUser(Usuario userChange, bool passChange)
        {
                string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception("Cadena de conexión no encontrada.");

                string query = @"
            UPDATE usuario
               SET nombre = @Nombre,
                   apellido = @Apellido,
                   email = @Email,
                   telefono = @Telefono,
                   id_rol = @IdRol
             WHERE dni = @Dni";

                bool actualizarPassword = passChange;
                if (actualizarPassword)
                {
                    query = @"
                UPDATE usuario
                   SET nombre = @Nombre,
                       apellido = @Apellido,
                       email = @Email,
                       telefono = @Telefono,
                       password = @Password,
                       id_rol = @IdRol
                 WHERE dni = @Dni";
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Dni", userChange.Dni);
                        command.Parameters.AddWithValue("@Nombre", userChange.Nombre);
                        command.Parameters.AddWithValue("@Apellido", userChange.Apellido);
                        command.Parameters.AddWithValue("@Email", userChange.Email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Telefono", userChange.Telefono ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IdRol", userChange.id_rol);

                        if (actualizarPassword)
                            command.Parameters.AddWithValue("@Password", userChange.password);

                        command.ExecuteNonQuery();
                    }
                }
            }
    }

    // Namespace adicional para el servicio de gestión avanzada
    namespace SALC.Services
    {
        /// <summary>
        /// Servicio para gestión completa de usuarios internos del sistema SALC
        /// Implementa RF-10: Gestionar usuarios según ERS-SALC_IEEE830
        /// </summary>
        public class UserManagementService
        {
            private readonly string _connectionString;

            public UserManagementService()
            {
                _connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");
            }

            /// <summary>
            /// Obtiene un usuario específico por DNI con información completa
            /// </summary>
            public Usuario GetUsuario(int dni)
            {
                string query = @"
                    SELECT 
                        u.dni, u.nombre, u.apellido, u.email, u.telefono, u.id_rol, u.estado_usuario,
                        r.rol as nombre_rol,
                        eu.estado as nombre_estado,
                        d.nro_matricula, d.especialidad,
                        a.nro_legajo, a.dni_supervisor, a.fecha_ingreso,
                        CONCAT(us.nombre, ' ', us.apellido) as nombre_supervisor
                    FROM usuario u
                    INNER JOIN rol r ON u.id_rol = r.id_rol
                    LEFT JOIN estado_usuario eu ON u.estado_usuario = eu.id_estado
                    LEFT JOIN doctor d ON u.dni = d.dni
                    LEFT JOIN asistente a ON u.dni = a.dni
                    LEFT JOIN usuario us ON a.dni_supervisor = us.dni
                    WHERE u.dni = @Dni";

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Dni", dni);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Usuario
                                {
                                    Dni = reader.GetInt32(reader.GetOrdinal("dni")),
                                    Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                    Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                                    Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                    Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                                    id_rol = reader.GetInt32(reader.GetOrdinal("id_rol")),
                                    Rol = reader.GetString(reader.GetOrdinal("nombre_rol")),
                                    estado_usuario = reader.IsDBNull(reader.GetOrdinal("estado_usuario")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("estado_usuario")),
                                    Estado = reader.IsDBNull(reader.GetOrdinal("nombre_estado")) ? "Sin estado" : reader.GetString(reader.GetOrdinal("nombre_estado")),
                                    
                                    // Datos específicos de doctor
                                    NumeroMatricula = reader.IsDBNull(reader.GetOrdinal("nro_matricula")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("nro_matricula")),
                                    Especialidad = reader.IsDBNull(reader.GetOrdinal("especialidad")) ? null : reader.GetString(reader.GetOrdinal("especialidad")),
                                    
                                    // Datos específicos de asistente
                                    NumeroLegajo = reader.IsDBNull(reader.GetOrdinal("nro_legajo")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("nro_legajo")),
                                    DniSupervisor = reader.IsDBNull(reader.GetOrdinal("dni_supervisor")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("dni_supervisor")),
                                    FechaIngreso = reader.IsDBNull(reader.GetOrdinal("fecha_ingreso")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_ingreso")),
                                    NombreSupervisor = reader.IsDBNull(reader.GetOrdinal("nombre_supervisor")) ? null : reader.GetString(reader.GetOrdinal("nombre_supervisor"))
                                };
                            }
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// Obtiene lista de roles disponibles
            /// </summary>
            public Dictionary<int, string> GetRoles()
            {
                return UserData.LoadRoles();
            }

            /// <summary>
            /// Obtiene lista de doctores que pueden ser supervisores
            /// </summary>
            public List<Usuario> GetDoctoresParaSupervisor()
            {
                const string query = @"
                    SELECT u.dni, u.nombre, u.apellido, d.especialidad
                    FROM usuario u
                    INNER JOIN doctor d ON u.dni = d.dni
                    WHERE u.estado_usuario = 1
                    ORDER BY u.apellido, u.nombre";

                var doctores = new List<Usuario>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            doctores.Add(new Usuario
                            {
                                Dni = reader.GetInt32(reader.GetOrdinal("dni")),
                                Nombre = reader.GetString(reader.GetOrdinal("nombre")),
                                Apellido = reader.GetString(reader.GetOrdinal("apellido")),
                                Especialidad = reader.GetString(reader.GetOrdinal("especialidad"))
                            });
                        }
                    }
                }

                return doctores;
            }

            /// <summary>
            /// Crea un nuevo usuario según su rol
            /// </summary>
            public bool CrearUsuario(Usuario usuario)
            {
                if (usuario == null || !usuario.EsValido())
                    throw new ArgumentException("Datos de usuario inválidos");

                var errorRol = usuario.ValidarDatosRol();
                if (!string.IsNullOrEmpty(errorRol))
                    throw new ArgumentException(errorRol);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Crear usuario base
                            const string queryUsuario = @"
                                INSERT INTO usuario (dni, nombre, apellido, email, telefono, password, id_rol, estado_usuario)
                                VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono, @Password, @IdRol, @EstadoUsuario)";

                            using (var command = new SqlCommand(queryUsuario, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Dni", usuario.Dni);
                                command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                                command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                                command.Parameters.AddWithValue("@Email", usuario.Email ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Telefono", usuario.Telefono ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Password", usuario.password ?? "temp123");
                                command.Parameters.AddWithValue("@IdRol", usuario.id_rol);
                                command.Parameters.AddWithValue("@EstadoUsuario", usuario.estado_usuario ?? 1);

                                command.ExecuteNonQuery();
                            }

                            // 2. Crear registro específico según rol
                            if (usuario.EsDoctor)
                            {
                                const string queryDoctor = @"
                                    INSERT INTO doctor (dni, nro_matricula, especialidad)
                                    VALUES (@Dni, @NumeroMatricula, @Especialidad)";

                                using (var command = new SqlCommand(queryDoctor, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Dni", usuario.Dni);
                                    command.Parameters.AddWithValue("@NumeroMatricula", usuario.NumeroMatricula.Value);
                                    command.Parameters.AddWithValue("@Especialidad", usuario.Especialidad);
                                    command.ExecuteNonQuery();
                                }
                            }
                            else if (usuario.EsAsistente)
                            {
                                const string queryAsistente = @"
                                    INSERT INTO asistente (dni, nro_legajo, dni_supervisor, fecha_ingreso)
                                    VALUES (@Dni, @NumeroLegajo, @DniSupervisor, @FechaIngreso)";

                                using (var command = new SqlCommand(queryAsistente, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Dni", usuario.Dni);
                                    command.Parameters.AddWithValue("@NumeroLegajo", usuario.NumeroLegajo.Value);
                                    command.Parameters.AddWithValue("@DniSupervisor", usuario.DniSupervisor.Value);
                                    command.Parameters.AddWithValue("@FechaIngreso", usuario.FechaIngreso ?? DateTime.Now);
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

            /// <summary>
            /// Actualiza un usuario existente
            /// </summary>
            public bool ActualizarUsuario(Usuario usuario, bool cambiarPassword = false)
            {
                if (usuario == null || !usuario.EsValido())
                    throw new ArgumentException("Datos de usuario inválidos");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 1. Actualizar usuario base
                            string queryUsuario = @"
                                UPDATE usuario 
                                SET nombre = @Nombre, apellido = @Apellido, email = @Email, 
                                    telefono = @Telefono, id_rol = @IdRol, estado_usuario = @EstadoUsuario";

                            if (cambiarPassword)
                            {
                                queryUsuario += ", password = @Password";
                            }

                            queryUsuario += " WHERE dni = @Dni";

                            using (var command = new SqlCommand(queryUsuario, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@Dni", usuario.Dni);
                                command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                                command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                                command.Parameters.AddWithValue("@Email", usuario.Email ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Telefono", usuario.Telefono ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@IdRol", usuario.id_rol);
                                command.Parameters.AddWithValue("@EstadoUsuario", usuario.estado_usuario ?? 1);

                                if (cambiarPassword)
                                {
                                    command.Parameters.AddWithValue("@Password", usuario.password);
                                }

                                command.ExecuteNonQuery();
                            }

                            // 2. Actualizar datos específicos según rol
                            if (usuario.EsDoctor)
                            {
                                // Verificar si ya existe registro en doctor
                                const string checkDoctor = "SELECT COUNT(*) FROM doctor WHERE dni = @Dni";
                                using (var command = new SqlCommand(checkDoctor, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Dni", usuario.Dni);
                                    int count = (int)command.ExecuteScalar();

                                    if (count > 0)
                                    {
                                        // Actualizar
                                        const string updateDoctor = @"
                                            UPDATE doctor 
                                            SET nro_matricula = @NumeroMatricula, especialidad = @Especialidad
                                            WHERE dni = @Dni";

                                        using (var updateCmd = new SqlCommand(updateDoctor, connection, transaction))
                                        {
                                            updateCmd.Parameters.AddWithValue("@Dni", usuario.Dni);
                                            updateCmd.Parameters.AddWithValue("@NumeroMatricula", usuario.NumeroMatricula.Value);
                                            updateCmd.Parameters.AddWithValue("@Especialidad", usuario.Especialidad);
                                            updateCmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Insertar
                                        const string insertDoctor = @"
                                            INSERT INTO doctor (dni, nro_matricula, especialidad)
                                            VALUES (@Dni, @NumeroMatricula, @Especialidad)";

                                        using (var insertCmd = new SqlCommand(insertDoctor, connection, transaction))
                                        {
                                            insertCmd.Parameters.AddWithValue("@Dni", usuario.Dni);
                                            insertCmd.Parameters.AddWithValue("@NumeroMatricula", usuario.NumeroMatricula.Value);
                                            insertCmd.Parameters.AddWithValue("@Especialidad", usuario.Especialidad);
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            else if (usuario.EsAsistente)
                            {
                                // Verificar si ya existe registro en asistente
                                const string checkAsistente = "SELECT COUNT(*) FROM asistente WHERE dni = @Dni";
                                using (var command = new SqlCommand(checkAsistente, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Dni", usuario.Dni);
                                    int count = (int)command.ExecuteScalar();

                                    if (count > 0)
                                    {
                                        // Actualizar
                                        const string updateAsistente = @"
                                            UPDATE asistente 
                                            SET nro_legajo = @NumeroLegajo, dni_supervisor = @DniSupervisor, fecha_ingreso = @FechaIngreso
                                            WHERE dni = @Dni";

                                        using (var updateCmd = new SqlCommand(updateAsistente, connection, transaction))
                                        {
                                            updateCmd.Parameters.AddWithValue("@Dni", usuario.Dni);
                                            updateCmd.Parameters.AddWithValue("@NumeroLegajo", usuario.NumeroLegajo.Value);
                                            updateCmd.Parameters.AddWithValue("@DniSupervisor", usuario.DniSupervisor.Value);
                                            updateCmd.Parameters.AddWithValue("@FechaIngreso", usuario.FechaIngreso ?? DateTime.Now);
                                            updateCmd.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        // Insertar
                                        const string insertAsistente = @"
                                            INSERT INTO asistente (dni, nro_legajo, dni_supervisor, fecha_ingreso)
                                            VALUES (@Dni, @NumeroLegajo, @DniSupervisor, @FechaIngreso)";

                                        using (var insertCmd = new SqlCommand(insertAsistente, connection, transaction))
                                        {
                                            insertCmd.Parameters.AddWithValue("@Dni", usuario.Dni);
                                            insertCmd.Parameters.AddWithValue("@NumeroLegajo", usuario.NumeroLegajo.Value);
                                            insertCmd.Parameters.AddWithValue("@DniSupervisor", usuario.DniSupervisor.Value);
                                            insertCmd.Parameters.AddWithValue("@FechaIngreso", usuario.FechaIngreso ?? DateTime.Now);
                                            insertCmd.ExecuteNonQuery();
                                        }
                                    }
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
        }
    }
}
