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
            // Asegurar que el estado no sea nulo
            if (string.IsNullOrEmpty(paciente.Estado))
                paciente.Estado = "Activo";
            _repo.Actualizar(paciente);
        }

        public void CrearPaciente(Paciente paciente)
        {
            // Asegurar que el estado por defecto sea "Activo"
            if (string.IsNullOrEmpty(paciente.Estado))
                paciente.Estado = "Activo";
            _repo.Crear(paciente);
        }

        public void EliminarPaciente(int dni)
        {
            // Baja l�gica - cambiar estado a "Inactivo"
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

        // M�todo para obtener solo pacientes activos
        public IEnumerable<Paciente> ObtenerActivos()
        {
            return _repo.ObtenerActivos();
        }

        // M�todo para cambiar el estado de un paciente
        public void CambiarEstadoPaciente(int dni, string nuevoEstado)
        {
            var paciente = _repo.ObtenerPorId(dni);
            if (paciente != null)
            {
                paciente.Estado = nuevoEstado;
                _repo.Actualizar(paciente);
            }
        }

        // M�todo para activar paciente
        public void ActivarPaciente(int dni)
        {
            CambiarEstadoPaciente(dni, "Activo");
        }

        // M�todo para desactivar paciente
        public void DesactivarPaciente(int dni)
        {
            CambiarEstadoPaciente(dni, "Inactivo");
        }

        // M�todo para verificar si un paciente est� activo
        public bool EstaActivo(int dni)
        {
            var paciente = _repo.ObtenerPorId(dni);
            return paciente?.Estado == "Activo";
        }
    }
}
