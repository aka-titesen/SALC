using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de gestión de usuarios del sistema.
    /// Define las operaciones para crear, modificar, consultar y gestionar usuarios, médicos y asistentes.
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Crea un nuevo usuario con sus datos específicos de rol (médico o asistente)
        /// </summary>
        /// <param name="usuario">Datos del usuario</param>
        /// <param name="medico">Datos específicos si es médico (opcional)</param>
        /// <param name="asistente">Datos específicos si es asistente (opcional)</param>
        void CrearUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null);

        /// <summary>
        /// Actualiza los datos de un usuario con sus datos específicos de rol
        /// </summary>
        /// <param name="usuario">Usuario con datos actualizados</param>
        /// <param name="medico">Datos específicos del médico (opcional)</param>
        /// <param name="asistente">Datos específicos del asistente (opcional)</param>
        void ActualizarUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null);

        /// <summary>
        /// Actualiza solo los datos base de un usuario
        /// </summary>
        /// <param name="usuario">Usuario con datos actualizados</param>
        void ActualizarUsuario(Usuario usuario);

        /// <summary>
        /// Elimina un usuario mediante baja lógica
        /// </summary>
        /// <param name="dni">DNI del usuario</param>
        void EliminarUsuario(int dni);

        /// <summary>
        /// Obtiene un usuario por su DNI
        /// </summary>
        /// <param name="dni">DNI del usuario</param>
        /// <returns>Usuario encontrado o null si no existe</returns>
        Usuario ObtenerPorDni(int dni);

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// </summary>
        /// <returns>Colección de todos los usuarios</returns>
        IEnumerable<Usuario> ObtenerTodos();

        /// <summary>
        /// Obtiene solo los usuarios activos del sistema
        /// </summary>
        /// <returns>Colección de usuarios activos</returns>
        IEnumerable<Usuario> ObtenerActivos();

        /// <summary>
        /// Cambia el estado de un usuario
        /// </summary>
        /// <param name="dni">DNI del usuario</param>
        /// <param name="nuevoEstado">Nuevo estado del usuario</param>
        void CambiarEstadoUsuario(int dni, string nuevoEstado);

        /// <summary>
        /// Activa un usuario (cambia su estado a Activo)
        /// </summary>
        /// <param name="dni">DNI del usuario</param>
        void ActivarUsuario(int dni);

        /// <summary>
        /// Desactiva un usuario (cambia su estado a Inactivo)
        /// </summary>
        /// <param name="dni">DNI del usuario</param>
        void DesactivarUsuario(int dni);
    }
}
