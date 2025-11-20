using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.DAL;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

namespace SALC.BLL
{
    /// <summary>
    /// Servicio de lógica de negocio para la gestión de pacientes.
    /// Implementa las reglas de negocio y coordina las operaciones con el repositorio.
    /// </summary>
    public class PacienteService : IPacienteService
    {
        private readonly PacienteRepositorio _repo = new PacienteRepositorio();
        
        /// <summary>
        /// Actualiza los datos de un paciente existente
        /// </summary>
        /// <param name="paciente">Paciente con los datos actualizados</param>
        public void ActualizarPaciente(Paciente paciente)
        {
            try
            {
                // Validaciones
                ValidarPaciente(paciente);

                // Asegurar que el estado no sea nulo
                if (string.IsNullOrEmpty(paciente.Estado))
                    paciente.Estado = "Activo";

                // Verificar que el paciente existe
                var existente = _repo.ObtenerPorId(paciente.Dni);
                if (existente == null)
                    throw new SalcBusinessException($"No existe un paciente con DNI {paciente.Dni}.");

                ExceptionHandler.LogInfo($"Actualizando paciente - DNI: {paciente.Dni}", "ActualizarPaciente");

                _repo.Actualizar(paciente);

                ExceptionHandler.LogInfo($"Paciente actualizado exitosamente - DNI: {paciente.Dni}", "ActualizarPaciente");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al actualizar el paciente", "ActualizarPaciente", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al actualizar paciente: {ex.Message}", "ActualizarPaciente");
                throw new SalcException(
                    "Error al actualizar paciente",
                    "No se pudo actualizar el paciente. Por favor, intente nuevamente.",
                    "UPDATE_PACIENTE_ERROR"
                );
            }
        }

        /// <summary>
        /// Crea un nuevo paciente en el sistema
        /// </summary>
        /// <param name="paciente">Datos del paciente a crear</param>
        public void CrearPaciente(Paciente paciente)
        {
            try
            {
                // Validaciones
                ValidarPaciente(paciente);

                // Verificar que no exista un paciente con el mismo DNI
                var existente = _repo.ObtenerPorId(paciente.Dni);
                if (existente != null)
                    throw new SalcBusinessException($"Ya existe un paciente con DNI {paciente.Dni}.");

                // Asegurar que el estado por defecto sea "Activo"
                if (string.IsNullOrEmpty(paciente.Estado))
                    paciente.Estado = "Activo";

                ExceptionHandler.LogInfo($"Creando paciente - DNI: {paciente.Dni}, Nombre: {paciente.Nombre} {paciente.Apellido}", "CrearPaciente");

                _repo.Crear(paciente);

                ExceptionHandler.LogInfo($"Paciente creado exitosamente - DNI: {paciente.Dni}", "CrearPaciente");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al crear el paciente", "CrearPaciente", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al crear paciente: {ex.Message}", "CrearPaciente");
                throw new SalcException(
                    "Error al crear paciente",
                    "No se pudo crear el paciente. Por favor, intente nuevamente.",
                    "CREATE_PACIENTE_ERROR"
                );
            }
        }

        /// <summary>
        /// Elimina un paciente mediante baja lógica (cambia estado a Inactivo)
        /// </summary>
        /// <param name="dni">DNI del paciente a eliminar</param>
        public void EliminarPaciente(int dni)
        {
            try
            {
                // Validaciones
                if (dni <= 0)
                    throw new SalcValidacionException("El DNI del paciente no es válido.", "dni");

                // Verificar que el paciente existe
                var existente = _repo.ObtenerPorId(dni);
                if (existente == null)
                    throw new SalcBusinessException($"No existe un paciente con DNI {dni}.");

                ExceptionHandler.LogInfo($"Eliminando paciente (baja lógica) - DNI: {dni}", "EliminarPaciente");

                // Baja lógica - cambiar estado a "Inactivo"
                _repo.Eliminar(dni);

                ExceptionHandler.LogInfo($"Paciente eliminado exitosamente - DNI: {dni}", "EliminarPaciente");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al eliminar el paciente", "EliminarPaciente", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogWarning($"Error inesperado al eliminar paciente: {ex.Message}", "EliminarPaciente");
                throw new SalcException(
                    "Error al eliminar paciente",
                    "No se pudo eliminar el paciente. Por favor, intente nuevamente.",
                    "DELETE_PACIENTE_ERROR"
                );
            }
        }

        /// <summary>
        /// Obtiene un paciente específico por su DNI
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <returns>El paciente encontrado o null si no existe</returns>
        public Paciente ObtenerPorDni(int dni)
        {
            try
            {
                if (dni <= 0)
                    throw new SalcValidacionException("El DNI del paciente no es válido.", "dni");

                return _repo.ObtenerPorId(dni);
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener el paciente", "ObtenerPorDni", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los pacientes del sistema
        /// </summary>
        /// <returns>Colección de todos los pacientes</returns>
        public IEnumerable<Paciente> ObtenerTodos()
        {
            try
            {
                return _repo.ObtenerTodos();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener la lista de pacientes", "ObtenerTodos", sqlEx);
            }
        }

        /// <summary>
        /// Obtiene todos los pacientes activos del sistema
        /// </summary>
        /// <returns>Colección de pacientes con estado Activo</returns>
        public IEnumerable<Paciente> ObtenerActivos()
        {
            try
            {
                return _repo.ObtenerActivos();
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al obtener pacientes activos", "ObtenerActivos", sqlEx);
            }
        }

        /// <summary>
        /// Cambia el estado de un paciente
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <param name="nuevoEstado">Nuevo estado (Activo o Inactivo)</param>
        public void CambiarEstadoPaciente(int dni, string nuevoEstado)
        {
            try
            {
                // Validaciones
                if (dni <= 0)
                    throw new SalcValidacionException("El DNI del paciente no es válido.", "dni");

                if (string.IsNullOrWhiteSpace(nuevoEstado))
                    throw new SalcValidacionException("El estado no puede estar vacío.", "estado");

                if (nuevoEstado != "Activo" && nuevoEstado != "Inactivo")
                    throw new SalcValidacionException("El estado debe ser 'Activo' o 'Inactivo'.", "estado");

                var paciente = _repo.ObtenerPorId(dni);
                if (paciente == null)
                    throw new SalcBusinessException($"No existe un paciente con DNI {dni}.");

                ExceptionHandler.LogInfo($"Cambiando estado de paciente - DNI: {dni}, Nuevo Estado: {nuevoEstado}", "CambiarEstadoPaciente");

                paciente.Estado = nuevoEstado;
                _repo.Actualizar(paciente);

                ExceptionHandler.LogInfo($"Estado cambiado exitosamente - DNI: {dni}", "CambiarEstadoPaciente");
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al cambiar el estado del paciente", "CambiarEstadoPaciente", sqlEx);
            }
            catch (SalcException)
            {
                throw;
            }
        }

        /// <summary>
        /// Activa un paciente cambiando su estado a Activo
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        public void ActivarPaciente(int dni)
        {
            CambiarEstadoPaciente(dni, "Activo");
        }

        /// <summary>
        /// Desactiva un paciente cambiando su estado a Inactivo
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        public void DesactivarPaciente(int dni)
        {
            CambiarEstadoPaciente(dni, "Inactivo");
        }

        /// <summary>
        /// Verifica si un paciente está activo
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <returns>True si el paciente está activo, false en caso contrario</returns>
        public bool EstaActivo(int dni)
        {
            try
            {
                if (dni <= 0)
                    return false;

                var paciente = _repo.ObtenerPorId(dni);
                return paciente?.Estado == "Activo";
            }
            catch (SqlException sqlEx)
            {
                throw new SalcDatabaseException("Error al verificar el estado del paciente", "EstaActivo", sqlEx);
            }
        }

        /// <summary>
        /// Valida que los datos del paciente cumplan con las reglas de negocio
        /// </summary>
        /// <param name="paciente">Paciente a validar</param>
        private void ValidarPaciente(Paciente paciente)
        {
            if (paciente == null)
                throw new SalcValidacionException("Los datos del paciente son obligatorios.", "paciente");

            if (paciente.Dni <= 0)
                throw new SalcValidacionException("El DNI debe ser un número positivo.", "dni");

            if (string.IsNullOrWhiteSpace(paciente.Nombre))
                throw new SalcValidacionException("El nombre del paciente es obligatorio.", "nombre");

            if (string.IsNullOrWhiteSpace(paciente.Apellido))
                throw new SalcValidacionException("El apellido del paciente es obligatorio.", "apellido");

            if (paciente.FechaNac > DateTime.Now)
                throw new SalcValidacionException("La fecha de nacimiento no puede ser futura.", "fechaNac");

            if (paciente.FechaNac < new DateTime(1900, 1, 1))
                throw new SalcValidacionException("La fecha de nacimiento no es válida.", "fechaNac");

            if (string.IsNullOrWhiteSpace(paciente.Sexo.ToString()))
                throw new SalcValidacionException("El sexo del paciente es obligatorio.", "sexo");

            if (paciente.Sexo != 'M' && paciente.Sexo != 'F' && paciente.Sexo != 'X')
                throw new SalcValidacionException("El sexo debe ser 'M', 'F' o 'X'.", "sexo");

            // Validar email si está presente
            if (!string.IsNullOrWhiteSpace(paciente.Email))
            {
                if (!paciente.Email.Contains("@") || !paciente.Email.Contains("."))
                    throw new SalcValidacionException("El formato del email no es válido.", "email");
            }

            // Validar teléfono si está presente
            if (!string.IsNullOrWhiteSpace(paciente.Telefono))
            {
                if (paciente.Telefono.Length < 8)
                    throw new SalcValidacionException("El teléfono debe tener al menos 8 caracteres.", "telefono");
            }
        }
    }
}
