using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IUsuarioService
    {
        void CrearUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null);
        void ActualizarUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null);
        void ActualizarUsuario(Usuario usuario); // Sobrecarga para actualizar solo datos base
        void EliminarUsuario(int dni);
        Usuario ObtenerPorDni(int dni);
        IEnumerable<Usuario> ObtenerTodos();
    }
}
