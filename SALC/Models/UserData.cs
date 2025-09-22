
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace SALC.Data
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
                throw;
            }

            return roles;
        }

        public static List<SALC.Usuario> GetUsers(string searchFilter = "", string roleFilter = "")
        {
            string query = @"
                SELECT u.dni, u.nombre, u.apellido, u.email, u.telefono, r.rol, u.estado_usuario, e.estado AS estado
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
    }
}
