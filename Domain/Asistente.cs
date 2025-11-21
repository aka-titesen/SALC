using System;

namespace SALC.Domain
{
    /// <summary>
    /// Representa un asistente de laboratorio.
    /// Extiende la información del usuario con datos específicos del rol de asistente.
    /// </summary>
    public class Asistente
    {
        /// <summary>
        /// DNI del asistente. Debe coincidir con el DNI en la tabla de usuarios
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// DNI del médico supervisor responsable del asistente
        /// </summary>
        public int DniSupervisor { get; set; }

        /// <summary>
        /// Fecha en que el asistente ingresó al laboratorio
        /// </summary>
        public DateTime FechaIngreso { get; set; }
    }
}
