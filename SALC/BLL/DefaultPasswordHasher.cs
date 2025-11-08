using System;

namespace SALC.BLL
{
    public class DefaultPasswordHasher : IPasswordHasher
    {
        public bool Verify(string plainText, string hashed)
        {
            try
            {
                // Intentar usar BCrypt.Net-Next si está disponible
                // NOTA: Para que esto funcione, debe instalar el paquete NuGet "BCrypt.Net-Next"
                // desde Visual Studio: Tools → NuGet Package Manager → Manage NuGet Packages for Solution
                return BCrypt.Net.BCrypt.Verify(plainText, hashed);
            }
            catch (Exception)
            {
                // Si BCrypt no está disponible, comparar como texto plano
                // SOLO para migración automática de contraseñas legacy
                return string.Equals(plainText, hashed, StringComparison.Ordinal);
            }
        }

        public string Hash(string plainText)
        {
            try
            {
                // Usar BCrypt con work factor 12 (recomendado para seguridad)
                // NOTA: Requiere paquete NuGet "BCrypt.Net-Next"
                return BCrypt.Net.BCrypt.HashPassword(plainText, workFactor: 12);
            }
            catch (TypeLoadException)
            {
                // BCrypt no está instalado - informar al usuario
                throw new InvalidOperationException(
                    "❌ BCrypt.Net-Next no está instalado.\n\n" +
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
        /// Determina si una contraseña está en texto plano o ya está hasheada con BCrypt
        /// </summary>
        public bool IsPlainText(string passwordHash)
        {
            // Los hashes de BCrypt siempre empiezan con $2a$, $2b$, $2x$ o $2y$
            // y tienen una longitud de 60 caracteres
            if (string.IsNullOrEmpty(passwordHash))
                return true;

            return !passwordHash.StartsWith("$2a$") && 
                   !passwordHash.StartsWith("$2b$") && 
                   !passwordHash.StartsWith("$2x$") && 
                   !passwordHash.StartsWith("$2y$");
        }
    }
}
