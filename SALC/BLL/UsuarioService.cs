using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public class UsuarioService : IUsuarioService
    {
        public void ActualizarUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null)
        {
        }

        public void CrearUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null)
        {
        }

        public void EliminarUsuario(int dni)
        {
        }

        public Usuario ObtenerPorDni(int dni)
        {
            return null;
        }

        public IEnumerable<Usuario> ObtenerTodos()
        {
            yield break;
        }
    }
}
