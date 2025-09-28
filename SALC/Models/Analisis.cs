// Models/Analisis.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un an�lisis/estudio en el sistema SALC
    /// </summary>
    public class Analisis
    {
        public int Id { get; set; }
        public int IdTipoAnalisis { get; set; }
        public int IdEstado { get; set; }
        public int DocPaciente { get; set; }
        public string Observaciones { get; set; }
        public int EncargadoCarga { get; set; }
        public int DoctorEncargado { get; set; }
        public string Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        
        // Propiedades de navegaci�n
        public Paciente Paciente { get; set; }
        public TipoAnalisis TipoAnalisis { get; set; }
        public Doctor Doctor { get; set; }
        public Estado Estado { get; set; }
    }
}