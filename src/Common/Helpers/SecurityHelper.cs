using System;
using System.Security.Cryptography;
using System.Text;

namespace SALC.Common.Helpers
{
    /// <summary>
    /// Clase helper para operaciones de seguridad como hash de contraseñas.
    /// </summary>
    public static class SecurityHelper
    {
        /// <summary>
        /// Genera un hash BCrypt de una contraseña
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash BCrypt de la contraseña</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña no puede ser nula o vacía.", nameof(password));

            // TODO: Implementar BCrypt cuando se agregue la dependencia
            // Por ahora usamos una implementación básica para la estructura
            return GenerateBasicHash(password);
        }

        /// <summary>
        /// Verifica si una contraseña coincide con su hash
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="hash">Hash almacenado</param>
        /// <returns>True si la contraseña es correcta</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            // TODO: Implementar verificación BCrypt cuando se agregue la dependencia
            // Por ahora usamos una implementación básica para la estructura
            return GenerateBasicHash(password) == hash;
        }

        /// <summary>
        /// Genera un hash básico usando SHA256 (temporal hasta implementar BCrypt)
        /// </summary>
        /// <param name="input">Texto a hashear</param>
        /// <returns>Hash SHA256</returns>
        private static string GenerateBasicHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input + "SALC_SALT"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Genera una cadena aleatoria para tokens o salts
        /// </summary>
        /// <param name="length">Longitud de la cadena</param>
        /// <returns>Cadena aleatoria</returns>
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Sanitiza una cadena para prevenir ataques de inyección
        /// </summary>
        /// <param name="input">Cadena a sanitizar</param>
        /// <returns>Cadena sanitizada</returns>
        public static string SanitizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remover caracteres peligrosos
            return input.Replace("'", "''")
                       .Replace(";", "")
                       .Replace("--", "")
                       .Replace("/*", "")
                       .Replace("*/", "")
                       .Replace("xp_", "")
                       .Replace("sp_", "");
        }

        /// <summary>
        /// Verifica si una cadena contiene caracteres potencialmente peligrosos
        /// </summary>
        /// <param name="input">Cadena a verificar</param>
        /// <returns>True si contiene caracteres peligrosos</returns>
        public static bool ContainsDangerousCharacters(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] dangerousPatterns = { "'", ";", "--", "/*", "*/", "xp_", "sp_", "drop", "delete", "truncate" };

            var lowerInput = input.ToLower();
            foreach (var pattern in dangerousPatterns)
            {
                if (lowerInput.Contains(pattern))
                    return true;
            }

            return false;
        }
    }
}
