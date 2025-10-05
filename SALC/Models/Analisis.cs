// Models/Analisis.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un análisis/estudio en el sistema SALC
    /// </summary>
    public class Analisis
    {
        public int Id { get; set; }
        public int IdTipoAnalisis { get; set; }
        public int IdEstado { get; set; }
        public int DniPaciente { get; set; }
        public int DocPaciente => DniPaciente; // Alias para compatibilidad
        public string Observaciones { get; set; }
        
        /// <summary>
        /// DNI del doctor externo que solicita el análisis (FK a doctor_externo)
        /// </summary>
        public int DniDoctorExterno { get; set; }
        
        /// <summary>
        /// DNI del asistente que recepcionó la muestra (RF-17) (FK a asistente)
        /// </summary>
        public int DniRecepcion { get; set; }
        
        /// <summary>
        /// DNI del doctor interno que validó y firmó el resultado (RF-18) (FK a doctor)
        /// Puede ser NULL mientras el estado sea "Sin verificar"
        /// </summary>
        public int? DniFirma { get; set; }
        
        /// <summary>
        /// Fecha y hora en que se validó el resultado (cuando dni_firma se asigna)
        /// </summary>
        public DateTime? FechaFirma { get; set; }
        
        public string Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        
        // Propiedades de navegación
        public Paciente Paciente { get; set; }
        public TipoAnalisis TipoAnalisis { get; set; }
        public DoctorExterno DoctorExterno { get; set; }
        public Asistente AsistenteRecepcion { get; set; }
        public Doctor DoctorFirma { get; set; }
        public Estado Estado { get; set; }
        
        /// <summary>
        /// Indica si el análisis está validado (estado = "Verificado")
        /// </summary>
        public bool EstaValidado => IdEstado == 2 && DniFirma.HasValue;
        
        /// <summary>
        /// Nombre del doctor que firmó (si existe)
        /// </summary>
        public string NombreDoctorFirma => DoctorFirma != null ? $"Dr. {DoctorFirma.NombreCompleto}" : "Sin validar";
    }
}