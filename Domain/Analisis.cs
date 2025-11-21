using System;

namespace SALC.Domain
{
    /// <summary>
    /// Representa un análisis clínico realizado a un paciente.
    /// Incluye información sobre el tipo de análisis, estado, fechas y médicos involucrados.
    /// </summary>
    public class Analisis
    {
        /// <summary>
        /// Identificador único del análisis
        /// </summary>
        public int IdAnalisis { get; set; }

        /// <summary>
        /// Identificador del tipo de análisis realizado
        /// </summary>
        public int IdTipoAnalisis { get; set; }

        /// <summary>
        /// Identificador del estado actual del análisis (Sin verificar, Verificado, Anulado)
        /// </summary>
        public int IdEstado { get; set; }

        /// <summary>
        /// DNI del paciente al que se realizó el análisis
        /// </summary>
        public int DniPaciente { get; set; }

        /// <summary>
        /// DNI del médico que creó y cargó el análisis
        /// </summary>
        public int DniCarga { get; set; }

        /// <summary>
        /// DNI del médico que firmó y validó el análisis. Null si aún no ha sido firmado
        /// </summary>
        public int? DniFirma { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó el análisis
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha y hora en que se firmó el análisis. Null si aún no ha sido firmado
        /// </summary>
        public DateTime? FechaFirma { get; set; }

        /// <summary>
        /// Observaciones adicionales sobre el análisis
        /// </summary>
        public string Observaciones { get; set; }
    }
}
