using System.Collections.Generic;
using SALC.Models;

namespace SALC.Services
{
    /// <summary>
    /// Servicio de gestión de usuarios en español con sufijo 'Service'.
    /// Delegado a UserDataService.
    /// </summary>
    public class UsuariosService
    {
        private readonly UserDataService _core = new UserDataService();

        public List<Usuario> ObtenerUsuarios(string filtroNombre = "", string filtroRol = "")
            => _core.ObtenerUsuarios(filtroNombre, filtroRol);

        public Usuario ObtenerUsuario(int dni)
            => _core.ObtenerUsuario(dni);

        public Dictionary<int, string> ObtenerRoles()
            => _core.ObtenerRoles();

        public List<Medico> ObtenerMedicosParaSupervisor()
            => _core.ObtenerMedicosParaSupervisor();

        public bool DesactivarUsuario(int dni) => _core.DesactivarUsuario(dni);

        public bool ActivarUsuario(int dni) => _core.ActivarUsuario(dni);

        public void CrearUsuario(Usuario usuario) => _core.CrearUsuario(usuario);

        public void ActualizarUsuario(Usuario usuario, bool cambiarContrasena) => _core.ActualizarUsuario(usuario, cambiarContrasena);
    }
}
