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
            if (!_hasher.Verify(contrasenia, usuario.PasswordHash)) return null;
            return usuario;
        }
    }
}
