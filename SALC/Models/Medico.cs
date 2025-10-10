// Models/Doctor.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un m�dico en el sistema SALC seg�n ERS v2.7
    /// Tabla: medicos (extensi�n de usuarios)
    /// </summary>
    public class Medico
    {
        #region Propiedades de la tabla medicos
        /// <summary>
        /// DNI del m�dico (PK, FK a usuarios.dni)
        /// </summary>
        public int Dni { get; set; }

        /// <summary>
        /// N�mero de matr�cula profesional (�nico)
        /// </summary>
        public int NumeroMatricula { get; set; }

        /// <summary>
        /// Especialidad m�dica
        /// </summary>
        public string Especialidad { get; set; }
        #endregion

        #region Propiedades extendidas desde usuarios
        /// <summary>
        /// Nombre del m�dico (desde tabla usuarios)
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Apellido del m�dico (desde tabla usuarios)
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Email del m�dico (desde tabla usuarios)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Estado del m�dico (desde tabla usuarios)
        /// </summary>
        public string Estado { get; set; }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Nombre completo del m�dico
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellido}";

        /// <summary>
        /// Verifica si el m�dico est� activo
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