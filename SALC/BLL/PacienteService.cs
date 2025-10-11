using System.Collections.Generic;
using SALC.Domain;
using SALC.DAL;

namespace SALC.BLL
{
    public class PacienteService : IPacienteService
    {
        private readonly PacienteRepositorio _repo = new PacienteRepositorio();
        public void ActualizarPaciente(Paciente paciente)
        {
            _repo.Actualizar(paciente);
        }

        public void CrearPaciente(Paciente paciente)
        {
            _repo.Crear(paciente);
        }

        public void EliminarPaciente(int dni)
        {
            _repo.Eliminar(dni);
        }

        public Paciente ObtenerPorDni(int dni)
        {
            return _repo.ObtenerPorId(dni);
        }

        public IEnumerable<Paciente> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }
    }
}
