// Models/Analisis.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un análisis clínico en el sistema SALC
    /// </summary>
    public class Analisis
    {
        public int IdAnalisis { get; set; }
        public int IdTipoAnalisis { get; set; }
        public int IdEstado { get; set; }
        public int DocPaciente { get; set; }
        public string Observaciones { get; set; }
        public int? EncargadoCarga { get; set; }
        public int? DoctorEncargado { get; set; }
        
        // Propiedades de navegación (opcional)
        public TipoAnalisis TipoAnalisis { get; set; }
        public Estado Estado { get; set; }
        public Paciente Paciente { get; set; }
        public Usuario UsuarioCarga { get; set; }
        public Doctor Doctor { get; set; }
    }
}