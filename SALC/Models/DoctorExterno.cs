// Models/DoctorExterno.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un doctor externo (solicitante) en el sistema SALC.
    /// Los doctores externos NO tienen acceso al sistema, solo se registran
    /// como solicitantes de análisis para fines de trazabilidad.
    /// </summary>
    public class DoctorExterno
    {
        /// <summary>
        /// DNI del doctor externo (PK)
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// Nombre del doctor externo
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Apellido del doctor externo
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Número de matrícula profesional (único)
        /// </summary>
        public int NroMatricula { get; set; }

        /// <summary>
        /// Especialidad del doctor externo
        /// </summary>
        public string Especialidad { get; set; }

        /// <summary>
        /// Teléfono de contacto (opcional)
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Email de contacto (opcional)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nombre completo del doctor externo (calculado)
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
