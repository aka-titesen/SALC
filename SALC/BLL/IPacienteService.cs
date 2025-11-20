using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de gestión de pacientes.
    /// Define las operaciones para crear, modificar, consultar y gestionar el estado de pacientes.
    /// </summary>
    public interface IPacienteService
    {
        /// <summary>
        /// Obtiene un paciente por su DNI
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <returns>Paciente encontrado o null si no existe</returns>
        Paciente ObtenerPorDni(int dni);

        /// <summary>
        /// Obtiene todos los pacientes del sistema
        /// </summary>
        /// <returns>Colección de todos los pacientes</returns>
        IEnumerable<Paciente> ObtenerTodos();

        /// <summary>
        /// Obtiene solo los pacientes activos del sistema
        /// </summary>
        /// <returns>Colección de pacientes activos</returns>
        IEnumerable<Paciente> ObtenerActivos();

        /// <summary>
        /// Crea un nuevo paciente
        /// </summary>
        /// <param name="paciente">Paciente a crear</param>
        void CrearPaciente(Paciente paciente);

        /// <summary>
        /// Actualiza los datos de un paciente existente
        /// </summary>
        /// <param name="paciente">Paciente con datos actualizados</param>
        void ActualizarPaciente(Paciente paciente);

        /// <summary>
        /// Elimina un paciente mediante baja lógica
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        void EliminarPaciente(int dni);

        /// <summary>
        /// Cambia el estado de un paciente
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <param name="nuevoEstado">Nuevo estado del paciente</param>
        void CambiarEstadoPaciente(int dni, string nuevoEstado);

        /// <summary>
        /// Activa un paciente (cambia su estado a Activo)
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        void ActivarPaciente(int dni);

        /// <summary>
        /// Desactiva un paciente (cambia su estado a Inactivo)
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        void DesactivarPaciente(int dni);

        /// <summary>
        /// Verifica si un paciente está activo
        /// </summary>
        /// <param name="dni">DNI del paciente</param>
        /// <returns>True si el paciente está activo, false en caso contrario</returns>
        bool EstaActivo(int dni);
    }
}
