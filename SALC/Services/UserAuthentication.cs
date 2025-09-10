// Services/UserAuthentication.cs
using System;
using System.Configuration; // Necesario para ConfigurationManager
using System.Data;
using System.Data.SqlClient; // Necesario para SQL Server
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
        /// Authenticates a user against the database.
        /// </summary>
        /// <param name="username">The user's DNI (as a string).</param>
        /// <param name="password">The plaintext password.</param>
        /// <returns>The Usuario object if authentication is successful, null otherwise.</returns>
        public static Usuario Authenticate(string username, string password)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            // El DNI debe ser numérico
            if (!int.TryParse(username, out int dniValue))
            {
                System.Windows.Forms.MessageBox.Show("El DNI debe ser numérico.", "Validación", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                return null;
            }

            // Obtener la cadena de conexión del App.config
            string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                // Manejar error: cadena de conexión no encontrada
                System.Windows.Forms.MessageBox.Show("Error de configuración: Cadena de conexión no encontrada.", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return null;
            }

            // IMPORTANTE: Autenticación temporal con contraseña en texto plano
            // string hashedInputPassword = HashPassword(password);
            string plainPassword = password;

            // Consulta SQL para verificar credenciales y obtener datos del usuario y rol
            string query = @"
                SELECT u.dni, u.nombre, u.apellido, u.email, r.rol
                FROM usuario u
                INNER JOIN roles r ON u.id_rol = r.id_rol
                WHERE u.dni = @Dni AND u.contraseña = @Contraseña"; // 'contraseña' en texto plano en la BD

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parámetros tipados
                        command.Parameters.Add("@Dni", SqlDbType.Int).Value = dniValue;
                        command.Parameters.Add("@Contraseña", SqlDbType.NVarChar, 256).Value = plainPassword;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Obtener ordinales de columnas
                                int ordDni = reader.GetOrdinal("dni");
                                int ordNombre = reader.GetOrdinal("nombre");
                                int ordApellido = reader.GetOrdinal("apellido");
                                int ordEmail = reader.GetOrdinal("email");
                                int ordRol = reader.GetOrdinal("rol");

                                // Si se encuentra un usuario, crear el objeto Usuario
                                string rawRol = reader.IsDBNull(ordRol) ? string.Empty : reader.GetString(ordRol);
                                string normalizedRol = NormalizeRole(rawRol);

                                Usuario authenticatedUser = new Usuario
                                {
                                    Dni = reader.GetInt32(ordDni),
                                    Nombre = reader.IsDBNull(ordNombre) ? string.Empty : reader.GetString(ordNombre),
                                    Apellido = reader.IsDBNull(ordApellido) ? string.Empty : reader.GetString(ordApellido),
                                    Email = reader.IsDBNull(ordEmail) ? string.Empty : reader.GetString(ordEmail),
                                    Rol = normalizedRol
                                };
                                return authenticatedUser;
                            }
                            else
                            {
                                // Credenciales inválidas
                                return null;
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejar errores específicos de SQL
                System.Windows.Forms.MessageBox.Show($"Error de base de datos: {sqlEx.Message}", "Error de BD", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Manejar otros errores inesperados
                System.Windows.Forms.MessageBox.Show($"Error inesperado durante la autenticación: {ex.Message}", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            // Si llegamos aquí, hubo un error o las credenciales no coincidieron
            return null;
        }

        /// <summary>
        /// Performs user login, storing the authenticated user information.
        /// </summary>
        /// <param name="username">The user's DNI.</param>
        /// <param name="password">The plaintext password.</param>
        /// <returns>True if login was successful, False otherwise.</returns>
        public static bool Login(string username, string password)
        {
            Usuario user = Authenticate(username, password);
            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Checks if a user is currently logged in.
        /// </summary>
        /// <returns>True if a user is logged in, False otherwise.</returns>
        public static bool IsLoggedIn()
        {
            return CurrentUser != null;
        }

        /// <summary>
        /// Hashes a password using SHA256. (Note: For production, use bcrypt/Argon2/PBKDF2).
        /// </summary>
        /// <param name="password">The plaintext password.</param>
        /// <returns>The SHA256 hash as a hexadecimal string.</returns>
        private static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // "x2" formatea como hexadecimal en minúsculas
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// Normaliza el rol leído desde la base de datos a claves internas: "admin", "clinico", "tecnico".
        /// </summary>
        private static string NormalizeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return string.Empty;

            string r = role.Trim().ToLowerInvariant();
            switch (r)
            {
                case "administrador":
                case "admin":
                    return "admin";
                case "clinico":
                case "clínico":
                    return "clinico";
                case "tecnico":
                case "técnico":
                    return "tecnico";
                default:
                    return r; // dejar como está en minúsculas si es otro
            }
        }
    }
}