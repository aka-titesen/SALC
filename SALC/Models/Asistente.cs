// Models/Asistente.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un asistente en el sistema SALC.
    /// Los asistentes trabajan bajo la supervisión de un doctor (clínico).
    /// Pueden recepcionar muestras (RF-17) pero NO pueden validar resultados.
    /// </summary>
    public class Asistente
    {
        /// <summary>
        /// DNI del asistente (PK, FK a usuario)
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// Número de legajo único del asistente
        /// </summary>
        public int NroLegajo { get; set; }

        /// <summary>
        /// DNI del doctor que supervisa a este asistente (FK a doctor)
        /// </summary>
        public int DniSupervisor { get; set; }

        /// <summary>
        /// Fecha de ingreso del asistente al laboratorio
        /// </summary>
        public DateTime FechaIngreso { get; set; }

        // Propiedades de navegación
        
        /// <summary>
        /// Usuario asociado al asistente
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Doctor que supervisa a este asistente
        /// </summary>
        public Doctor Supervisor { get; set; }

        /// <summary>
        /// Nombre completo del supervisor (calculado)
        /// </summary>
        public string NombreSupervisor => Supervisor != null ? Supervisor.NombreCompleto : "Sin supervisor";
    }
}
