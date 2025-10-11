using System;
using System.Reflection;

namespace SALC.BLL
{
    public class DefaultPasswordHasher : IPasswordHasher
    {
        private readonly MethodInfo _verify;
        private readonly MethodInfo _hash;
        private readonly object _bcrypt;

        public DefaultPasswordHasher()
        {
            // Intentar cargar BCrypt.Net-Next si está instalado.
            try
            {
                var asm = Assembly.Load("BCrypt.Net-Next");
                var type = asm.GetType("BCrypt.Net.BCrypt");
                _verify = type?.GetMethod("Verify", new[] { typeof(string), typeof(string) });
                _hash = type?.GetMethod("HashPassword", new[] { typeof(string) });
                _bcrypt = Activator.CreateInstance(type);
            }
            catch
            {
                // Paquete no presente: se usará fallback inseguro solo para desarrollo.
                _verify = null;
                _hash = null;
                _bcrypt = null;
            }
        }

        public bool Verify(string plainText, string hashed)
        {
            if (_verify != null)
            {
                return (bool)_verify.Invoke(_bcrypt, new object[] { plainText, hashed });
            }
            // Fallback: NO USAR en producción. Solo para que el login funcione con hashes de ejemplo.
            return string.Equals(plainText, hashed, StringComparison.Ordinal);
        }

        public string Hash(string plainText)
        {
            if (_hash != null)
            {
                return (string)_hash.Invoke(_bcrypt, new object[] { plainText });
            }
            // Fallback de desarrollo.
            return plainText;
        }
    }
}
