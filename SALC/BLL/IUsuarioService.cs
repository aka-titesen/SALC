using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IUsuarioService
    {
        Usuario ObtenerPorDni(int dni);
        IEnumerable<Usuario> ObtenerTodos();
        void CrearUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null);
        void ActualizarUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null);
        void EliminarUsuario(int dni);
    }
}
