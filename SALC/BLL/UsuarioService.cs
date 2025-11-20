using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.DAL;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

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
            try
            {
                // Validaciones
                ValidarUsuario(usuario);

                // Verificar que el usuario existe
                var existente = _usuarios.ObtenerPorId(usuario.Dni);
                if (existente == null)
                    throw new SalcBusinessException($"No existe un usuario con DNI {usuario.Dni}.");

                // Si PasswordHash trae una contraseña plana (heurística), la hasheamos
                if (!string.IsNullOrEmpty(usuario.PasswordHash) && !usuario.PasswordHash.StartsWith("$2"))
                {
                    usuario.PasswordHash = _hasher.Hash(usuario.PasswordHash);
                }

                ExceptionHandler.LogInfo($"Actualizando usuario - DNI: {usuario.Dni}, Rol: {usuario.IdRol}", "ActualizarUsuario");

                _usuarios.Actualizar(usuario);

                if (usuario.IdRol == 2 && medico != null)
                {
                    ValidarMedico(medico);
                    _medicos.Actualizar(medico);
                    ExceptionHandler.LogInfo($"Datos de médico actualizados - DNI: {medico.Dni}", "ActualizarUsuario");
                }
                else if (usuario.IdRol == 3 && asistente != null)
                {
                    ValidarAsistente(asistente);
                    _asistentes.Actualizar(asistente);
                    ExceptionHandler.LogInfo($"Datos de asistente actualizados - DNI: {asistente.Dni}", "ActualizarUsuario");
                }

                ExceptionHandler.LogInfo($"Usuario actualizado exitosamente - DNI: {usuario.Dni}", "ActualizarUsuario");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al actualizar el usuario", "ActualizarUsuario", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al actualizar usuario: {ex.Message}", "ActualizarUsuario");
                throw new SalcException(
                    "Error al actualizar usuario",
                    "No se pudo actualizar el usuario. Por favor, intente nuevamente.",
                    "UPDATE_USUARIO_ERROR"
                );
            }
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            try
            {
                ValidarUsuario(usuario);

                var existente = _usuarios.ObtenerPorId(usuario.Dni);
                if (existente == null)
                    throw new SalcBusinessException($"No existe un usuario con DNI {usuario.Dni}.");

                // Si PasswordHash trae una contraseña plana (heurística), la hasheamos
                if (!string.IsNullOrEmpty(usuario.PasswordHash) && !usuario.PasswordHash.StartsWith("$2"))
                {
                    usuario.PasswordHash = _hasher.Hash(usuario.PasswordHash);
                }

                ExceptionHandler.LogInfo($"Actualizando datos base de usuario - DNI: {usuario.Dni}", "ActualizarUsuario");

                _usuarios.Actualizar(usuario);

                ExceptionHandler.LogInfo($"Datos base actualizados exitosamente - DNI: {usuario.Dni}", "ActualizarUsuario");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al actualizar el usuario", "ActualizarUsuario", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void CrearUsuario(Usuario usuario, Medico medico = null, Asistente asistente = null)
        {
            try
            {
                // Validaciones
                ValidarUsuario(usuario);

                // Verificar que no exista un usuario con el mismo DNI
                var existente = _usuarios.ObtenerPorId(usuario.Dni);
                if (existente != null)
                    throw new SalcBusinessException($"Ya existe un usuario con DNI {usuario.Dni}.");

                // Validar datos específicos según el rol
                if (usuario.IdRol == 2)
                {
                    if (medico == null)
                        throw new SalcValidacionException("Debe proporcionar los datos del médico.", "medico");
                    ValidarMedico(medico);
                }
                else if (usuario.IdRol == 3)
                {
                    if (asistente == null)
                        throw new SalcValidacionException("Debe proporcionar los datos del asistente.", "asistente");
                    ValidarAsistente(asistente);
                }

                // Hash de contraseña
                if (string.IsNullOrWhiteSpace(usuario.PasswordHash))
                    throw new SalcValidacionException("La contraseña es obligatoria.", "password");

                usuario.PasswordHash = _hasher.Hash(usuario.PasswordHash);

                // Asegurar que el estado por defecto sea "Activo"
                if (string.IsNullOrEmpty(usuario.Estado))
                    usuario.Estado = "Activo";

                ExceptionHandler.LogInfo($"Creando usuario - DNI: {usuario.Dni}, Rol: {usuario.IdRol}, Email: {usuario.Email}", "CrearUsuario");

                if (usuario.IdRol == 2 && medico != null)
                {
                    _usuarios.CrearUsuarioMedico(usuario, medico);
                    ExceptionHandler.LogInfo($"Usuario médico creado - DNI: {usuario.Dni}, Matrícula: {medico.NroMatricula}", "CrearUsuario");
                }
                else if (usuario.IdRol == 3 && asistente != null)
                {
                    _usuarios.CrearUsuarioAsistente(usuario, asistente);
                    ExceptionHandler.LogInfo($"Usuario asistente creado - DNI: {usuario.Dni}, Supervisor: {asistente.DniSupervisor}", "CrearUsuario");
                }
                else
                {
                    _usuarios.Crear(usuario);
                    ExceptionHandler.LogInfo($"Usuario creado - DNI: {usuario.Dni}", "CrearUsuario");
                }

                ExceptionHandler.LogInfo($"Usuario creado exitosamente - DNI: {usuario.Dni}", "CrearUsuario");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al crear el usuario", "CrearUsuario", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al crear usuario: {ex.Message}", "CrearUsuario");
                throw new SalcException(
                    "Error al crear usuario",
                    "No se pudo crear el usuario. Por favor, intente nuevamente.",
                    "CREATE_USUARIO_ERROR"
                );
            }
        }

        public void EliminarUsuario(int dni)
        {
            try
            {
                if (dni <= 0)
                    throw new SalcValidacionException("El DNI del usuario no es válido.", "dni");

                var existente = _usuarios.ObtenerPorId(dni);
                if (existente == null)
                    throw new SalcBusinessException($"No existe un usuario con DNI {dni}.");

                ExceptionHandler.LogInfo($"Eliminando usuario (baja lógica) - DNI: {dni}", "EliminarUsuario");

                // Usar baja lógica
                _usuarios.Eliminar(dni);

                ExceptionHandler.LogInfo($"Usuario eliminado exitosamente - DNI: {dni}", "EliminarUsuario");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al eliminar el usuario", "EliminarUsuario", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public Usuario ObtenerPorDni(int dni)
        {
            try
            {
                if (dni <= 0)
                    throw new SalcValidacionException("El DNI del usuario no es válido.", "dni");

                return _usuarios.ObtenerPorId(dni);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener el usuario", "ObtenerPorDni", sqlEx);
            }
        }

        public IEnumerable<Usuario> ObtenerTodos()
        {
            try
            {
                return _usuarios.ObtenerTodos();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener la lista de usuarios", "ObtenerTodos", sqlEx);
            }
        }

        public IEnumerable<Usuario> ObtenerActivos()
        {
            try
            {
                return _usuarios.ObtenerActivos();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener usuarios activos", "ObtenerActivos", sqlEx);
            }
        }

        public void CambiarEstadoUsuario(int dni, string nuevoEstado)
        {
            try
            {
                if (dni <= 0)
                    throw new SalcValidacionException("El DNI del usuario no es válido.", "dni");

                if (string.IsNullOrWhiteSpace(nuevoEstado))
                    throw new SalcValidacionException("El estado no puede estar vacío.", "estado");

                if (nuevoEstado != "Activo" && nuevoEstado != "Inactivo")
                    throw new SalcValidacionException("El estado debe ser 'Activo' o 'Inactivo'.", "estado");

                var usuario = _usuarios.ObtenerPorId(dni);
                if (usuario == null)
                    throw new SalcBusinessException($"No existe un usuario con DNI {dni}.");

                ExceptionHandler.LogInfo($"Cambiando estado de usuario - DNI: {dni}, Nuevo Estado: {nuevoEstado}", "CambiarEstadoUsuario");

                usuario.Estado = nuevoEstado;
                _usuarios.Actualizar(usuario);

                ExceptionHandler.LogInfo($"Estado cambiado exitosamente - DNI: {dni}", "CambiarEstadoUsuario");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al cambiar el estado del usuario", "CambiarEstadoUsuario", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        public void ActivarUsuario(int dni)
        {
            CambiarEstadoUsuario(dni, "Activo");
        }

        public void DesactivarUsuario(int dni)
        {
            CambiarEstadoUsuario(dni, "Inactivo");
        }

        /// <summary>
        /// Valida los datos del usuario
        /// </summary>
        private void ValidarUsuario(Usuario usuario)
        {
            if (usuario == null)
                throw new SalcValidacionException("Los datos del usuario son obligatorios.", "usuario");

            if (usuario.Dni <= 0)
                throw new SalcValidacionException("El DNI debe ser un número positivo.", "dni");

            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new SalcValidacionException("El nombre del usuario es obligatorio.", "nombre");

            if (string.IsNullOrWhiteSpace(usuario.Apellido))
                throw new SalcValidacionException("El apellido del usuario es obligatorio.", "apellido");

            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new SalcValidacionException("El email del usuario es obligatorio.", "email");

            if (!usuario.Email.Contains("@") || !usuario.Email.Contains("."))
                throw new SalcValidacionException("El formato del email no es válido.", "email");

            if (usuario.IdRol <= 0)
                throw new SalcValidacionException("Debe seleccionar un rol válido.", "idRol");

            if (usuario.IdRol < 1 || usuario.IdRol > 3)
                throw new SalcValidacionException("El rol debe ser Administrador (1), Médico (2) o Asistente (3).", "idRol");
        }

        /// <summary>
        /// Valida los datos específicos de un médico
        /// </summary>
        private void ValidarMedico(Medico medico)
        {
            if (medico == null)
                throw new SalcValidacionException("Los datos del médico son obligatorios.", "medico");

            if (medico.Dni <= 0)
                throw new SalcValidacionException("El DNI del médico no es válido.", "medico.dni");

            if (medico.NroMatricula <= 0)
                throw new SalcValidacionException("El número de matrícula debe ser un número positivo.", "medico.nroMatricula");

            if (string.IsNullOrWhiteSpace(medico.Especialidad))
                throw new SalcValidacionException("La especialidad del médico es obligatoria.", "medico.especialidad");
        }

        /// <summary>
        /// Valida los datos específicos de un asistente
        /// </summary>
        private void ValidarAsistente(Asistente asistente)
        {
            if (asistente == null)
                throw new SalcValidacionException("Los datos del asistente son obligatorios.", "asistente");

            if (asistente.Dni <= 0)
                throw new SalcValidacionException("El DNI del asistente no es válido.", "asistente.dni");

            if (asistente.DniSupervisor <= 0)
                throw new SalcValidacionException("Debe asignar un médico supervisor válido.", "asistente.dniSupervisor");

            if (asistente.FechaIngreso > DateTime.Now)
                throw new SalcValidacionException("La fecha de ingreso no puede ser futura.", "asistente.fechaIngreso");
        }
    }
}
