// Models/Turno.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un turno en el sistema SALC
    /// </summary>
    public class Turno
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public int DoctorId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; }
        public string Motivo { get; set; }
        public string Observaciones { get; set; }
        
        // Propiedades de navegación
        public Paciente Paciente { get; set; }
        public Doctor Doctor { get; set; }
    }
}