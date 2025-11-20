namespace SALC.Domain
{
    /// <summary>
    /// Representa un médico que trabaja en el laboratorio.
    /// Extiende la información del usuario con datos específicos de la profesión médica.
    /// </summary>
    public class Medico
    {
        /// <summary>
        /// DNI del médico. Debe coincidir con el DNI en la tabla de usuarios
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// Número de matrícula profesional del médico
        /// </summary>
        public int NroMatricula { get; set; }

        /// <summary>
        /// Especialidad médica del profesional
        /// </summary>
        public string Especialidad { get; set; }
    }
}
