using SALC.Models;

namespace SALC.Services
{
    /// <summary>
    /// Servicio de autenticación en español con sufijo 'Service'.
    /// Fachada sobre UserAuthentication mientras coexista código legacy.
    /// </summary>
    public class AutenticacionService
    {
        public Usuario UsuarioActual => UserAuthentication.CurrentUser;

        public bool IniciarSesion(string credencial, string password)
            => UserAuthentication.Login(credencial, password);

        public void CerrarSesion() => UserAuthentication.Logout();

        public bool TieneRol(string rol) => UserAuthentication.HasRole(rol);
    }
}
