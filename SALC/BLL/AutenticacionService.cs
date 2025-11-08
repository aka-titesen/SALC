using SALC.Domain;
using SALC.DAL;

namespace SALC.BLL
{
    public class AutenticacionService : IAutenticacionService
    {
        private readonly UsuarioRepositorio _usuarios;
        private readonly IPasswordHasher _hasher;

        public AutenticacionService(UsuarioRepositorio usuarios, IPasswordHasher hasher)
        {
            _usuarios = usuarios;
            _hasher = hasher;
        }

        public Usuario ValidarCredenciales(int dni, string contrasenia)
        {
            var usuario = _usuarios.ObtenerPorId(dni);
            if (usuario == null) return null;
            if (!string.Equals(usuario.Estado, "Activo", System.StringComparison.OrdinalIgnoreCase)) return null;

            // ===== MIGRACIÓN AUTOMÁTICA DE CONTRASEÑAS =====
            // Verificar si la contraseña está en texto plano
            if (_hasher.IsPlainText(usuario.PasswordHash))
            {
                // La contraseña está en texto plano, verificar si coincide
                if (string.Equals(contrasenia, usuario.PasswordHash, System.StringComparison.Ordinal))
                {
                    // ? Contraseña correcta en texto plano ? Migrar a BCrypt
                    try
                    {
                        string nuevoHash = _hasher.Hash(contrasenia);
                        usuario.PasswordHash = nuevoHash;
                        _usuarios.Actualizar(usuario);
                        
                        System.Diagnostics.Debug.WriteLine($"? Contraseña migrada a BCrypt para usuario DNI: {dni}");
                        
                        return usuario;
                    }
                    catch (System.Exception ex)
                    {
                        // Si falla la migración, aún permitir el login pero registrar el error
                        System.Diagnostics.Debug.WriteLine($"?? Error al migrar contraseña para DNI {dni}: {ex.Message}");
                        return usuario;
                    }
                }
                else
                {
                    // ? Contraseña incorrecta
                    return null;
                }
            }
            else
            {
                // La contraseña ya está hasheada con BCrypt, verificar normalmente
                if (!_hasher.Verify(contrasenia, usuario.PasswordHash))
                {
                    return null;
                }
                
                return usuario;
            }
        }
    }
}
