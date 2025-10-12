using System.Collections.Generic;
using SALC.Domain;
using SALC.DAL;

namespace SALC.BLL
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuarioRepositorio _usuarios = new UsuarioRepositorio();
        private readonly MedicoRepositorio _medicos = new MedicoRepositorio();
        private readonly AsistenteRepositorio _asistentes = new AsistenteRepositorio();
        private readonly IPasswordHasher _hasher = new DefaultPasswordHasher();

        public void ActualizarUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null)
        {
            // Si PasswordHash trae una contraseña plana (heurística), la hasheamos
            if (!string.IsNullOrEmpty(usuario.PasswordHash) && !usuario.PasswordHash.StartsWith("$2"))
            {
                usuario.PasswordHash = _hasher.Hash(usuario.PasswordHash);
            }
            _usuarios.Actualizar(usuario);
            if (usuario.IdRol == 2 && medico != null)
            {
                _medicos.Actualizar(medico);
            }
            else if (usuario.IdRol == 3 && asistente != null)
            {
                _asistentes.Actualizar(asistente);
            }
        }

        // Sobrecarga para actualizar solo los datos base del usuario (útil para cambios de estado)
        public void ActualizarUsuario(Usuario usuario)
        {
            // Si PasswordHash trae una contraseña plana (heurística), la hasheamos
            if (!string.IsNullOrEmpty(usuario.PasswordHash) && !usuario.PasswordHash.StartsWith("$2"))
            {
                usuario.PasswordHash = _hasher.Hash(usuario.PasswordHash);
            }
            _usuarios.Actualizar(usuario);
        }

        public void CrearUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null)
        {
            // Hash de contraseña
            usuario.PasswordHash = _hasher.Hash(usuario.PasswordHash);
            // Asegurar que el estado por defecto sea "Activo"
            if (string.IsNullOrEmpty(usuario.Estado))
                usuario.Estado = "Activo";
                
            if (usuario.IdRol == 2 && medico != null)
            {
                _usuarios.CrearUsuarioMedico(usuario, medico);
            }
            else if (usuario.IdRol == 3 && asistente != null)
            {
                _usuarios.CrearUsuarioAsistente(usuario, asistente);
            }
            else
            {
                _usuarios.Crear(usuario);
            }
        }

        public void EliminarUsuario(int dni)
        {
            // Usar baja lógica
            _usuarios.Eliminar(dni);
        }

        public Usuario ObtenerPorDni(int dni)
        {
            return _usuarios.ObtenerPorId(dni);
        }

        public IEnumerable<Usuario> ObtenerTodos()
        {
            return _usuarios.ObtenerTodos();
        }

        // Método para obtener solo usuarios activos
        public IEnumerable<Usuario> ObtenerActivos()
        {
            return _usuarios.ObtenerActivos();
        }

        // Método para activar/desactivar usuario
        public void CambiarEstadoUsuario(int dni, string nuevoEstado)
        {
            var usuario = _usuarios.ObtenerPorId(dni);
            if (usuario != null)
            {
                usuario.Estado = nuevoEstado;
                _usuarios.Actualizar(usuario);
            }
        }

        // Método para activar usuario
        public void ActivarUsuario(int dni)
        {
            CambiarEstadoUsuario(dni, "Activo");
        }

        // Método para desactivar usuario
        public void DesactivarUsuario(int dni)
        {
            CambiarEstadoUsuario(dni, "Inactivo");
        }
    }
}
