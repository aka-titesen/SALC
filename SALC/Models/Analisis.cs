// Models/Analisis.cs
using System;
using System.Collections.Generic;

namespace SALC.Models
{
    /// <summary>
    /// Representa un análisis clínico en el sistema SALC según ERS v2.7
    /// Tabla: analisis
    /// </summary>
    public class Analisis
    {
        #region Propiedades de la tabla analisis
        /// <summary>
        /// ID único del análisis (PK, IDENTITY)
        /// </summary>
        public int IdAnalisis { get; set; }

        /// <summary>
        /// ID del tipo de análisis (FK a tipos_analisis.id_tipo_analisis)
        /// </summary>
        public int IdTipoAnalisis { get; set; }

        /// <summary>
        /// ID del estado del análisis (FK a estados_analisis.id_estado)
        /// </summary>
        public int IdEstado { get; set; }

        /// <summary>
        /// DNI del paciente (FK a pacientes.dni)
        /// </summary>
        public int DniPaciente { get; set; }

        /// <summary>
        /// DNI del médico que crea el análisis (FK a medicos.dni)
        /// </summary>
        public int DniCarga { get; set; }

        /// <summary>
        /// DNI del médico que valida el análisis (FK a medicos.dni, nullable)
        /// </summary>
        public int? DniFirma { get; set; }

        /// <summary>
        /// Fecha y hora de creación del análisis
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha y hora de validación/firma del análisis (nullable)
        /// </summary>
        public DateTime? FechaFirma { get; set; }

        /// <summary>
        /// Observaciones del análisis (opcional)
        /// </summary>
        public string Observaciones { get; set; }
        #endregion

        #region Propiedades de navegación
        /// <summary>
        /// Paciente asociado al análisis
        /// </summary>
        public Paciente Paciente { get; set; }

        /// <summary>
        /// Tipo de análisis
        /// </summary>
        public TipoAnalisis TipoAnalisis { get; set; }

        /// <summary>
        /// Estado del análisis
        /// </summary>
        public EstadoAnalisis Estado { get; set; }

        /// <summary>
        /// Médico que creó/cargó el análisis
        /// </summary>
        public Medico MedicoCarga { get; set; }

        /// <summary>
        /// Médico que validó/firmó el análisis (si existe)
        /// </summary>
        public Medico MedicoFirma { get; set; }

        /// <summary>
        /// Resultados de métricas del análisis
        /// </summary>
        public List<AnalisisMetrica> Resultados { get; set; } = new List<AnalisisMetrica>();
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Verifica si el análisis está sin verificar (estado = 1)
        /// </summary>
        public bool EstaSinVerificar => IdEstado == 1;

        /// <summary>
        /// Verifica si el análisis está verificado (estado = 2)
        /// </summary>
        public bool EstaVerificado => IdEstado == 2 && DniFirma.HasValue;

        /// <summary>
        /// Nombre del médico que creó el análisis
        /// </summary>
        public string NombreMedicoCarga => MedicoCarga?.NombreCompleto ?? "No especificado";

        /// <summary>
        /// Nombre del médico que validó el análisis
        /// </summary>
        public string NombreMedicoFirma => MedicoFirma?.NombreCompleto ?? "Sin validar";

        /// <summary>
        /// Descripción del estado
        /// </summary>
        public string DescripcionEstado => Estado?.Descripcion ?? "Estado desconocido";

        /// <summary>
        /// Descripción del tipo de análisis
        /// </summary>
        public string DescripcionTipoAnalisis => TipoAnalisis?.Descripcion ?? "Tipo desconocido";

        /// <summary>
        /// Indica si se pueden modificar los resultados (solo si no está verificado)
        /// </summary>
        public bool PuedeModificarResultados => EstaSinVerificar;

        /// <summary>
        /// Indica si el análisis puede ser validado
        /// </summary>
        public bool PuedeSerValidado => EstaSinVerificar && Resultados.Count > 0;
        #endregion

        #region Métodos de validación
        /// <summary>
        /// Valida los datos básicos del análisis
        /// </summary>
        public bool EsValido()
        {
            return IdTipoAnalisis > 0 &&
                   IdEstado > 0 &&
                   DniPaciente > 0 &&
                   DniCarga > 0 &&
                   FechaCreacion <= DateTime.Now;
        }

        /// <summary>
        /// Valida si el análisis puede ser firmado
        /// </summary>
        public string ValidarParaFirma()
        {
            if (EstaVerificado)
                return "El análisis ya está verificado.";

            if (Resultados.Count == 0)
                return "No se pueden validar análisis sin resultados cargados.";

            return null; // Sin errores
        }
        #endregion

        #region Propiedades de compatibilidad (deprecated)
        [Obsolete("Use IdAnalisis instead")]
        public int Id
        {
            get => IdAnalisis;
            set => IdAnalisis = value;
        }

        [Obsolete("Use DniPaciente instead")]
        public int DocPaciente => DniPaciente;

        [Obsolete("Not used in new structure")]
        public string Prioridad { get; set; }

        [Obsolete("Not used in new structure")]
        public int DniDoctorExterno { get; set; }

        [Obsolete("Not used in new structure")]
        public int DniRecepcion { get; set; }

        [Obsolete("Use MedicoCarga instead")]
        public Doctor DoctorFirma { get; set; }

        [Obsolete("Use Estado instead")]
        public Estado EstadoLegacy { get; set; }

        [Obsolete("Use MedicoCarga/MedicoFirma instead")]
        public Asistente AsistenteRecepcion { get; set; }
        #endregion
    }
}