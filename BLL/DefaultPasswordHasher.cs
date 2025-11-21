using System;

namespace SALC.BLL
{
    /// <summary>
    /// Implementación por defecto del hasher de contraseñas utilizando BCrypt.
    /// Proporciona hashing seguro y verificación de contraseñas.
    /// </summary>
    public class DefaultPasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Verifica si una contraseña en texto plano coincide con un hash BCrypt.
        /// También soporta comparación directa para migración de contraseñas en texto plano.
        /// </summary>
        /// <param name="plainText">Contraseña en texto plano a verificar</param>
        /// <param name="hashed">Hash BCrypt para comparar (o contraseña en texto plano para migración)</param>
        /// <returns>True si la contraseña coincide, false en caso contrario</returns>
        public bool Verify(string plainText, string hashed)
        {
            if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(hashed))
                return false;

            try
            {
                // Si el hash comienza con $2, es un hash BCrypt válido
                if (hashed.StartsWith("$2a$") || hashed.StartsWith("$2b$") || 
                    hashed.StartsWith("$2x$") || hashed.StartsWith("$2y$"))
                {
                    return BCrypt.Net.BCrypt.Verify(plainText, hashed);
                }
                else
                {
                    // Comparación directa para contraseñas en texto plano (migración)
                    // ADVERTENCIA: Esto solo debe usarse durante la migración
                    return string.Equals(plainText, hashed, StringComparison.Ordinal);
                }
            }
            catch (Exception)
            {
                // En caso de error, intentar comparación directa como fallback
                return string.Equals(plainText, hashed, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Genera un hash BCrypt de una contraseña en texto plano
        /// </summary>
        /// <param name="plainText">Contraseña en texto plano</param>
        /// <returns>Hash BCrypt de la contraseña</returns>
        public string Hash(string plainText)
        {
            try
            {
                return BCrypt.Net.BCrypt.HashPassword(plainText, workFactor: 12);
            }
            catch (TypeLoadException)
            {
                throw new InvalidOperationException(
                    "BCrypt.Net-Next no está instalado.\n\n" +
                    "Para habilitar el hashing seguro de contraseñas:\n" +
                    "1. Abra Visual Studio\n" +
                    "2. Click derecho en el proyecto SALC\n" +
                    "3. Seleccione 'Manage NuGet Packages...'\n" +
                    "4. Busque 'BCrypt.Net-Next'\n" +
                    "5. Instale la versión 4.0.3 o superior\n\n" +
                    "Alternativamente, use la Package Manager Console:\n" +
                    "Install-Package BCrypt.Net-Next -Version 4.0.3"
                );
            }
        }

        /// <summary>
        /// Determina si una contraseña está en texto plano o ya está hasheada con BCrypt.
        /// Los hashes de BCrypt empiezan con $2a$, $2b$, $2x$ o $2y$ y tienen 60 caracteres.
        /// </summary>
        /// <param name="passwordHash">Cadena a evaluar</param>
        /// <returns>True si es texto plano, false si es un hash BCrypt</returns>
        public bool IsPlainText(string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
                return true;

            return !passwordHash.StartsWith("$2a$") && 
                   !passwordHash.StartsWith("$2b$") && 
                   !passwordHash.StartsWith("$2x$") && 
                   !passwordHash.StartsWith("$2y$");
        }
    }
}
