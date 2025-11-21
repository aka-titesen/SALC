using System;

namespace SALC.Domain
{
    /// <summary>
    /// Representa un paciente del laboratorio clínico.
    /// Contiene información personal y médica necesaria para la gestión de análisis.
    /// </summary>
    public class Paciente
    {
        /// <summary>
        /// DNI del paciente (Documento Nacional de Identidad). Actúa como identificador único
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// Nombre del paciente
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Apellido del paciente
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Fecha de nacimiento del paciente
        /// </summary>
        public DateTime FechaNac { get; set; }

        /// <summary>
        /// Sexo del paciente. Valores válidos: 'M' (Masculino), 'F' (Femenino), 'X' (Otro)
        /// </summary>
        public char Sexo { get; set; }

        /// <summary>
        /// Dirección de correo electrónico del paciente
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Número de teléfono de contacto del paciente
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Identificador de la obra social del paciente. Null si no tiene obra social
        /// </summary>
        public int? IdObraSocial { get; set; }

        /// <summary>
        /// Estado del paciente para baja lógica. Valores válidos: 'Activo' o 'Inactivo'
        /// </summary>
        public string Estado { get; set; } = "Activo";
    }
}
