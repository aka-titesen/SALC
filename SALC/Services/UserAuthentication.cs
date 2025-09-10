using System;
using System.Security.Cryptography;
using System.Text;

namespace SALC
{
    public class UserAuthentication
    {
        /// <summary>
        /// Valida las credenciales del usuario
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>True si las credenciales son válidas, False en caso contrario</returns>
        public static bool ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            // En producción, aquí se implementaría la consulta a la base de datos
            // Por ahora, usamos credenciales de prueba
            
            // Usuarios de prueba (en producción esto vendría de la base de datos)
            var testUsers = new[]
            {
                new { Username = "admin", Password = "admin123", Role = "Administrador" },
                new { Username = "clinico", Password = "clinico123", Role = "Clínico" },
                new { Username = "tecnico", Password = "tecnico123", Role = "Técnico" }
            };

            foreach (var user in testUsers)
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    user.Password.Equals(password))
                {
                    CurrentUser = new UserInfo
                    {
                        Username = user.Username,
                        Role = user.Role,
                        LoginTime = DateTime.Now
                    };
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Información del usuario actualmente autenticado
        /// </summary>
        public static UserInfo CurrentUser { get; private set; }

        /// <summary>
        /// Cierra la sesión del usuario actual
        /// </summary>
        public static void Logout()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Verifica si el usuario actual tiene un rol específico
        /// </summary>
        /// <param name="role">Rol a verificar</param>
        /// <returns>True si el usuario tiene el rol especificado</returns>
        public static bool HasRole(string role)
        {
            return CurrentUser != null && CurrentUser.Role.Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Verifica si el usuario está autenticado
        /// </summary>
        /// <returns>True si hay un usuario autenticado</returns>
        public static bool IsAuthenticated()
        {
            return CurrentUser != null;
        }

        /// <summary>
        /// Genera un hash seguro de la contraseña (para uso futuro con base de datos)
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash de la contraseña</returns>
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }

    /// <summary>
    /// Clase que contiene la información del usuario autenticado
    /// </summary>
    public class UserInfo
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public DateTime LoginTime { get; set; }
        public string DisplayName { get { return Username + " (" + Role + ")"; } }
    }
}
