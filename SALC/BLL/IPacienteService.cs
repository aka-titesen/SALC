using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IPacienteService
    {
        Paciente ObtenerPorDni(int dni);
        IEnumerable<Paciente> ObtenerTodos();
        void CrearPaciente(Paciente paciente);
        void ActualizarPaciente(Paciente paciente);
        void EliminarPaciente(int dni);
    }
}
