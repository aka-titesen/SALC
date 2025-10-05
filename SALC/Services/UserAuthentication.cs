// Services/UserAuthentication.cs
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace SALC
{
    /// <summary>
    /// Provides methods for user authentication and session management.
    /// </summary>
    public static class UserAuthentication
    {
        /// <summary>
        /// Gets the currently logged-in user. Null if no user is logged in.
        /// </summary>
        public static Usuario CurrentUser { get; private set; } = null;

        /// <summary>
        /// Authenticates a user against the database. Returns null if credentials/role/state are invalid.
        /// </summary>
        public static Usuario Authenticate(string usernameOrEmail, string password)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
                return null;

            string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                return null; // Sin cadena de conexión no se puede autenticar

            // Aceptar DNI numérico o Email
            bool isDni = int.TryParse(usernameOrEmail, out int dniValue);
            string emailValue = isDni ? null : usernameOrEmail.Trim();

            string query = @"
                SELECT u.dni, u.nombre, u.apellido, u.email, u.telefono, r.[rol]
                FROM usuario u
                INNER JOIN rol r ON u.id_rol = r.id_rol
                LEFT JOIN estado_usuario eu ON u.estado_usuario = eu.id_estado
                WHERE (u.dni = @Dni OR LOWER(u.email) = LOWER(@Email))
                  AND u.[password] = @Password
                  AND u.estado_usuario = 1";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Parámetros (DNI o Email)
                    command.Parameters.Add("@Dni", SqlDbType.Int).Value = isDni ? (object)dniValue : DBNull.Value;
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = (object)(emailValue ?? (object)DBNull.Value);
                    command.Parameters.Add("@Password", SqlDbType.NVarChar, 256).Value = password;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read()) return null;

                        int ordDni = reader.GetOrdinal("dni");
                        int ordNombre = reader.GetOrdinal("nombre");
                        int ordApellido = reader.GetOrdinal("apellido");
                        int ordEmail = reader.GetOrdinal("email");
                        int ordTelefono = reader.GetOrdinal("telefono");
                        int ordRol = reader.GetOrdinal("rol");

                        string rawRol = reader.IsDBNull(ordRol) ? string.Empty : reader.GetString(ordRol);
                        string normalizedRol = NormalizeRole(rawRol);
                        if (normalizedRol != "admin" && normalizedRol != "clinico" && normalizedRol != "asistente")
                            return null;

                        return new Usuario
                        {
                            Dni = reader.GetInt32(ordDni),
                            Nombre = reader.IsDBNull(ordNombre) ? string.Empty : reader.GetString(ordNombre),
                            Apellido = reader.IsDBNull(ordApellido) ? string.Empty : reader.GetString(ordApellido),
                            Email = reader.IsDBNull(ordEmail) ? string.Empty : reader.GetString(ordEmail),
                            Telefono = reader.IsDBNull(ordTelefono) ? string.Empty : reader.GetString(ordTelefono),
                            Rol = normalizedRol
                        };
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Performs user login, storing the authenticated user information. Strict roles.
        /// </summary>
        public static bool Login(string usernameOrEmail, string password)
        {
            Usuario user = Authenticate(usernameOrEmail, password);
            if (user == null) return false;
            CurrentUser = user;
            return true;
        }

        public static void Logout() => CurrentUser = null;
        public static bool IsLoggedIn() => CurrentUser != null;

        // SHA256 demo (reemplazar por BCrypt en producción)
        private static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        private static string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return string.Empty;
            string r = role.Trim().ToLowerInvariant();
            switch (r)
            {
                case "administrador":
                case "admin": return "admin";
                case "clinico":
                case "clínico": return "clinico";
                case "asistente": return "asistente";
                default: return r;
            }
        }
    }
}