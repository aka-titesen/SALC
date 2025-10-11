// Models/Asistente.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un asistente en el sistema SALC según ERS v2.7
    /// Tabla: asistentes (extensión de usuarios)
    /// </summary>
    public class Asistente
    {
        #region Propiedades de la tabla asistentes
        /// <summary>
        /// DNI del asistente (PK, FK a usuarios.dni)
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// DNI del supervisor (FK a medicos.dni)
        /// </summary>
        public int DniSupervisor { get; set; }

        /// <summary>
        /// Fecha de ingreso del asistente
        /// </summary>
        public DateTime FechaIngreso { get; set; }
        #endregion

        #region Propiedades extendidas desde usuarios
        /// <summary>
        /// Nombre del asistente (desde tabla usuarios)
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Apellido del asistente (desde tabla usuarios)
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Email del asistente (desde tabla usuarios)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Estado del asistente (desde tabla usuarios)
        /// </summary>
        public string Estado { get; set; }
        #endregion

        #region Propiedades del supervisor
        /// <summary>
        /// Nombre del supervisor (calculado)
        /// </summary>
        public string NombreSupervisor { get; set; }

        /// <summary>
        /// Apellido del supervisor (calculado)
        /// </summary>
        public string ApellidoSupervisor { get; set; }

        /// <summary>
        /// Especialidad del supervisor (calculado)
        /// </summary>
        public string EspecialidadSupervisor { get; set; }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Nombre completo del asistente
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellido}";

        /// <summary>
        /// Nombre completo del supervisor
        /// </summary>
        public string NombreCompletoSupervisor => $"{NombreSupervisor} {ApellidoSupervisor}";

        /// <summary>
        /// Verifica si el asistente está activo
        /// </summary>
        public bool EstaActivo => Estado == "Activo";

        /// <summary>
        /// Años de experiencia desde el ingreso
        /// </summary>
        public int AniosExperiencia => DateTime.Now.Year - FechaIngreso.Year;
        #endregion
    }
}
