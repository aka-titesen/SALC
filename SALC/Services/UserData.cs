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

        public static List<SALC.Usuario> GetUsers(string searchFilter = "", string roleFilter = "")
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
                        command.Parameters.AddWithValue("@IdRol", userChange.Rol);

                        if (actualizarPassword)
                            command.Parameters.AddWithValue("@Password", userChange.password);

                        command.ExecuteNonQuery();
                    }
                }
            }
    }
}
