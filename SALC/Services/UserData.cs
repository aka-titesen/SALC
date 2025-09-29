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
                string query = "SELECT id_rol, rol FROM roles ORDER BY rol";
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

            public static List<SALC.Usuario> GetUsers(string searchFilter = "", string roleFilter = "")
            {
                string query = @"
                    SELECT u.dni, u.nombre, u.apellido, u.email, u.telefono, r.rol, u.estado_usuario AS id_estado, e.estado AS estado
                    FROM usuario u
                    INNER JOIN roles r ON u.id_rol = r.id_rol
                    LEFT  JOIN estado_usuarios e ON u.estado_usuario = e.id_estado
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

                var list = new List<SALC.Usuario>();

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
                                    list.Add(new SALC.Usuario
                                    {
                                        Dni = reader.GetInt32(0),
                                        Nombre = reader.GetString(1),
                                        Apellido = reader.GetString(2),
                                        Email = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                        Telefono = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                        Rol = reader.GetString(5),
                                        ID_estado = reader.GetInt32(6),
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
                INSERT INTO usuario (dni, nombre, apellido, email, telefono, contraseña, id_rol)
                VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono, @Password, @IdRol)";

                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Dni", userCreate.Dni);
                            command.Parameters.AddWithValue("@Nombre", userCreate.Nombre);
                            command.Parameters.AddWithValue("@Apellido", userCreate.Apellido);
                            command.Parameters.AddWithValue("@Email", userCreate.Email);
                            command.Parameters.AddWithValue("@Telefono", userCreate.Telefono);
                            command.Parameters.AddWithValue("@Password", userCreate.Pass);
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
                           contraseña = @Password,
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
                            command.Parameters.AddWithValue("@Email", userChange.Email);
                            command.Parameters.AddWithValue("@Telefono", userChange.Telefono);
                            command.Parameters.AddWithValue("@IdRol", userChange.Rol);

                            if (actualizarPassword)
                                command.Parameters.AddWithValue("@Password", userChange.Pass);

                            command.ExecuteNonQuery();
                        }
                    }
                }
        }
    }
