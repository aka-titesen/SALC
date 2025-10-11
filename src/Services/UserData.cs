using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Models;
using SALC.DataAccess.Repositories;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gestión de datos de usuarios según ERS v2.7
    /// Implementa la lógica de negocio para operaciones CRUD de usuarios
    /// </summary>
    public class UserDataService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly RolRepository _rolRepository;

        public UserDataService()
        {
            _usuarioRepository = new UsuarioRepository();
            _rolRepository = new RolRepository();
        }

        #region Operaciones de Consulta

        /// <summary>
        /// Obtiene todos los usuarios con filtros opcionales
        /// </summary>
        /// <param name="filtroNombre">Filtro por nombre, apellido o DNI</param>
        /// <param name="filtroRol">Filtro por nombre del rol</param>
        /// <returns>Lista de usuarios</returns>
        public List<Usuario> ObtenerUsuarios(string filtroNombre = "", string filtroRol = "")
        {
            try
            {
                int? idRolFiltro = null;
                
                // Convertir nombre de rol a ID si se especifica
                if (!string.IsNullOrEmpty(filtroRol) && filtroRol != "Todos los roles")
                {
                    var roles = _rolRepository.ObtenerTodos();
                    var rol = roles.FirstOrDefault(r => string.Equals(r.NombreRol, filtroRol, StringComparison.OrdinalIgnoreCase));
                    if (rol != null)
                    {
                        idRolFiltro = rol.IdRol;
                    }
                }

                return _usuarioRepository.ObtenerTodos(filtroNombre, idRolFiltro);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener usuarios: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por DNI
        /// </summary>
        public Usuario ObtenerUsuario(int dni)
        {
            try
            {
                return _usuarioRepository.ObtenerPorDni(dni);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener usuario con DNI {dni}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los roles disponibles
        /// </summary>
        public Dictionary<int, string> ObtenerRoles()
        {
            try
            {
                var roles = _rolRepository.ObtenerTodos();
                return roles.ToDictionary(r => r.IdRol, r => r.NombreRol);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener roles: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los médicos activos (para supervisores)
        /// </summary>
        public List<Medico> ObtenerMedicosParaSupervisor()
        {
            try
            {
                return _usuarioRepository.ObtenerMedicosActivos();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener médicos supervisores: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Creación

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        public bool CrearUsuario(Usuario usuario)
        {
            try
            {
                // Validaciones de negocio
                ValidarUsuarioParaCreacion(usuario);

                // Crear en base de datos
                return _usuarioRepository.Crear(usuario);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al crear usuario: {ex.Message}", ex);
            }
        }

        #endregion

        #region Operaciones de Actualización

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        public bool ActualizarUsuario(Usuario usuario, bool cambiarPassword = false)
        {
            try
            {
                // Validaciones de negocio
                ValidarUsuarioParaActualizacion(usuario);

                // Actualizar en base de datos
                return _usuarioRepository.Actualizar(usuario, cambiarPassword);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar usuario: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Desactiva un usuario (eliminación lógica)
        /// </summary>
        public bool DesactivarUsuario(int dni)
        {
            try
            {
                // Verificar que el usuario existe
                var usuario = _usuarioRepository.ObtenerPorDni(dni);
                if (usuario == null)
                    throw new ArgumentException("Usuario no encontrado");

                // No permitir desactivar al administrador principal
                if (usuario.EsAdministrador && dni == 30000001) // DNI del admin principal según lote de datos
                    throw new InvalidOperationException("No se puede desactivar al administrador principal del sistema");

                return _usuarioRepository.EliminarLogico(dni);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al desactivar usuario: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Activa un usuario previamente desactivado
        /// </summary>
        public bool ActivarUsuario(int dni)
        {
            try
            {
                // Verificar que el usuario existe
                var usuario = _usuarioRepository.ObtenerPorDni(dni);
                if (usuario == null)
                    throw new ArgumentException("Usuario no encontrado");

                return _usuarioRepository.Activar(dni);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al activar usuario: {ex.Message}", ex);
            }
        }

        #endregion

        #region Métodos de Validación Privados

        private void ValidarUsuarioParaCreacion(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (!usuario.EsValido())
                throw new ArgumentException("Los datos básicos del usuario son inválidos");

            var errorRol = usuario.ValidarDatosRol();
            if (!string.IsNullOrEmpty(errorRol))
                throw new ArgumentException(errorRol);

            // Verificar que no existe otro usuario con el mismo DNI
            if (_usuarioRepository.ExistePorDni(usuario.Dni))
                throw new ArgumentException($"Ya existe un usuario con DNI {usuario.Dni}");

            // Verificar que no existe otro usuario con el mismo email
            if (!string.IsNullOrEmpty(usuario.Email) && _usuarioRepository.ObtenerPorEmail(usuario.Email) != null)
                throw new ArgumentException($"Ya existe un usuario con email {usuario.Email}");

            // Validaciones específicas por rol
            if (usuario.EsMedico)
            {
                // Verificar que no existe otro médico con la misma matrícula
                // TODO: Implementar validación de matrícula única
            }

            if (usuario.EsAsistente)
            {
                // Verificar que el supervisor existe y es médico
                var supervisor = _usuarioRepository.ObtenerPorDni(usuario.DniSupervisor.Value);
                if (supervisor == null || !supervisor.EsMedico || !supervisor.EstaActivo)
                    throw new ArgumentException("El supervisor especificado no es válido o no está activo");
            }
        }

        private void ValidarUsuarioParaActualizacion(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (!usuario.EsValido())
                throw new ArgumentException("Los datos básicos del usuario son inválidos");

            var errorRol = usuario.ValidarDatosRol();
            if (!string.IsNullOrEmpty(errorRol))
                throw new ArgumentException(errorRol);

            // Verificar que el usuario existe
            if (!_usuarioRepository.ExistePorDni(usuario.Dni))
                throw new ArgumentException($"No existe un usuario con DNI {usuario.Dni}");

            // Verificar que no existe otro usuario con el mismo email
            if (!string.IsNullOrEmpty(usuario.Email) && 
                _usuarioRepository.ExisteEmailEnOtroUsuario(usuario.Email, usuario.Dni))
                throw new ArgumentException($"Ya existe otro usuario con email {usuario.Email}");

            // Validaciones específicas por rol (similar a creación)
            if (usuario.EsAsistente && usuario.DniSupervisor.HasValue)
            {
                var supervisor = _usuarioRepository.ObtenerPorDni(usuario.DniSupervisor.Value);
                if (supervisor == null || !supervisor.EsMedico || !supervisor.EstaActivo)
                    throw new ArgumentException("El supervisor especificado no es válido o no está activo");
            }
        }

        #endregion
    }

    #region Clase de Compatibilidad (Deprecated - ACTUALIZADA AL ESPAÑOL)

    /// <summary>
    /// Clase estática para compatibilidad con código existente
    /// Se recomienda usar UserDataService en su lugar
    /// TODOS LOS MÉTODOS ACTUALIZADOS AL ESPAÑOL
    /// </summary>
    [Obsolete("Usar UserDataService en su lugar")]
    public static class DatosUsuario
    {
        private static readonly UserDataService _servicio = new UserDataService();

        public static Dictionary<int, string> CargarRoles()
        {
            return _servicio.ObtenerRoles();
        }

        public static List<Usuario> ObtenerUsuarios(string filtroNombre = "", string filtroRol = "")
        {
            return _servicio.ObtenerUsuarios(filtroNombre, filtroRol);
        }

        public static bool EliminarUsuario(int dni)
        {
            return _servicio.DesactivarUsuario(dni);
        }

        public static bool ActivarUsuario(int dni)
        {
            return _servicio.ActivarUsuario(dni);
        }

        public static void CrearUsuario(Usuario usuario)
        {
            _servicio.CrearUsuario(usuario);
        }

        public static void ActualizarUsuario(Usuario usuario, bool cambiarContrasena)
        {
            _servicio.ActualizarUsuario(usuario, cambiarContrasena);
        }
    }

    #endregion
}
