using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IPacienteService
    {
        Paciente ObtenerPorDni(int dni);
        IEnumerable<Paciente> ObtenerTodos();
        IEnumerable<Paciente> ObtenerActivos(); // Solo pacientes activos
        void CrearPaciente(Paciente paciente);
        void ActualizarPaciente(Paciente paciente);
        void EliminarPaciente(int dni); // Baja lógica
        void CambiarEstadoPaciente(int dni, string nuevoEstado);
        void ActivarPaciente(int dni);
        void DesactivarPaciente(int dni);
        bool EstaActivo(int dni);
    }
}
