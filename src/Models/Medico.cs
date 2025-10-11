// Models/Doctor.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un médico en el sistema SALC según ERS v2.7
    /// Tabla: medicos (extensión de usuarios)
    /// </summary>
    public class Medico
    {
        #region Propiedades de la tabla medicos
        /// <summary>
        /// DNI del médico (PK, FK a usuarios.dni)
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// Número de matrícula profesional (único)
        /// </summary>
        public int NumeroMatricula { get; set; }

        /// <summary>
        /// Especialidad médica
        /// </summary>
        public string Especialidad { get; set; }
        #endregion

        #region Propiedades extendidas desde usuarios
        /// <summary>
        /// Nombre del médico (desde tabla usuarios)
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Apellido del médico (desde tabla usuarios)
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Email del médico (desde tabla usuarios)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Estado del médico (desde tabla usuarios)
        /// </summary>
        public string Estado { get; set; }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Nombre completo del médico
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellido}";

        /// <summary>
        /// Verifica si el médico está activo
        /// </summary>
        public bool EstaActivo => Estado == "Activo";
        #endregion

        #region Propiedades de compatibilidad (deprecated)
        [Obsolete("Use NumeroMatricula instead")]
        public int NroMatricula
        {
            get => NumeroMatricula;
            set => NumeroMatricula = value;
        }
        #endregion
    }
}