// Models/Analisis.cs
using System;
using System.Collections.Generic;

namespace SALC.Models
{
    /// <summary>
    /// Representa un an�lisis cl�nico en el sistema SALC seg�n ERS v2.7
    /// Tabla: analisis
    /// </summary>
    public class Analisis
    {
        #region Propiedades de la tabla analisis
        /// <summary>
        /// ID �nico del an�lisis (PK, IDENTITY)
        /// </summary>
        public int IdAnalisis { get; set; }

        /// <summary>
        /// ID del tipo de an�lisis (FK a tipos_analisis.id_tipo_analisis)
        /// </summary>
        public int IdTipoAnalisis { get; set; }

        /// <summary>
        /// ID del estado del an�lisis (FK a estados_analisis.id_estado)
        /// </summary>
        public int IdEstado { get; set; }

        /// <summary>
        /// DNI del paciente (FK a pacientes.dni)
        /// </summary>
        public int DniPaciente { get; set; }

        /// <summary>
        /// DNI del m�dico que crea el an�lisis (FK a medicos.dni)
        /// </summary>
        public int DniCarga { get; set; }

        /// <summary>
        /// DNI del m�dico que valida el an�lisis (FK a medicos.dni, nullable)
        /// </summary>
        public int? DniFirma { get; set; }

        /// <summary>
        /// Fecha y hora de creaci�n del an�lisis
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha y hora de validaci�n/firma del an�lisis (nullable)
        /// </summary>
        public DateTime? FechaFirma { get; set; }

        /// <summary>
        /// Observaciones del an�lisis (opcional)
        /// </summary>
        public string Observaciones { get; set; }
        #endregion

        #region Propiedades de navegaci�n
        /// <summary>
        /// Paciente asociado al an�lisis
        /// </summary>
        public Paciente Paciente { get; set; }

        /// <summary>
        /// Tipo de an�lisis
        /// </summary>
        public TipoAnalisis TipoAnalisis { get; set; }

        /// <summary>
        /// Estado del an�lisis
        /// </summary>
        public EstadoAnalisis Estado { get; set; }

        /// <summary>
        /// M�dico que cre�/carg� el an�lisis
        /// </summary>
        public Medico MedicoCarga { get; set; }

        /// <summary>
        /// M�dico que valid�/firm� el an�lisis (si existe)
        /// </summary>
        public Medico MedicoFirma { get; set; }

        /// <summary>
        /// Resultados de m�tricas del an�lisis
        /// </summary>
        public List<AnalisisMetrica> Resultados { get; set; } = new List<AnalisisMetrica>();
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Verifica si el an�lisis est� sin verificar (estado = 1)
        /// </summary>
        public bool EstaSinVerificar => IdEstado == 1;

        /// <summary>
        /// Verifica si el an�lisis est� verificado (estado = 2)
        /// </summary>
        public bool EstaVerificado => IdEstado == 2 && DniFirma.HasValue;

        /// <summary>
        /// Nombre del m�dico que cre� el an�lisis
        /// </summary>
        public string NombreMedicoCarga => MedicoCarga?.NombreCompleto ?? "No especificado";

        /// <summary>
        /// Nombre del m�dico que valid� el an�lisis
        /// </summary>
        public string NombreMedicoFirma => MedicoFirma?.NombreCompleto ?? "Sin validar";

        /// <summary>
        /// Descripci�n del estado
        /// </summary>
        public string DescripcionEstado => Estado?.Descripcion ?? "Estado desconocido";

        /// <summary>
        /// Descripci�n del tipo de an�lisis
        /// </summary>
        public string DescripcionTipoAnalisis => TipoAnalisis?.Descripcion ?? "Tipo desconocido";

        /// <summary>
        /// Indica si se pueden modificar los resultados (solo si no est� verificado)
        /// </summary>
        public bool PuedeModificarResultados => EstaSinVerificar;

        /// <summary>
        /// Indica si el an�lisis puede ser validado
        /// </summary>
        public bool PuedeSerValidado => EstaSinVerificar && Resultados.Count > 0;
        #endregion

        #region M�todos de validaci�n
        /// <summary>
        /// Valida los datos b�sicos del an�lisis
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
        /// Valida si el an�lisis puede ser firmado
        /// </summary>
        public string ValidarParaFirma()
        {
            if (EstaVerificado)
                return "El an�lisis ya est� verificado.";

            if (Resultados.Count == 0)
                return "No se pueden validar an�lisis sin resultados cargados.";

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