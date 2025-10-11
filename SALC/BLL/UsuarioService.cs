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

        public void CrearUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null)
        {
            // Hash de contraseña
            usuario.PasswordHash = _hasher.Hash(usuario.PasswordHash);
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
            // Intentar eliminar extensión y luego usuario
            try { _usuarios.EliminarUsuarioMedico(dni); return; } catch { }
            try { _usuarios.EliminarUsuarioAsistente(dni); return; } catch { }
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
    }
}
