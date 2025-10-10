// Models/Paciente.cs
using System;

namespace SALC.Models
{
    /// <summary>
    /// Representa un paciente en el sistema SALC según ERS v2.7
    /// Tabla: pacientes
    /// </summary>
    public class Paciente
    {
        #region Propiedades de la tabla pacientes
        /// <summary>
        /// DNI del paciente (Clave primaria)
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
        public DateTime FechaNacimiento { get; set; }

        /// <summary>
        /// Sexo del paciente ('M', 'F', 'X')
        /// </summary>
        public string Sexo { get; set; }

        /// <summary>
        /// Email del paciente (opcional)
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Teléfono del paciente (opcional)
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// ID de la obra social (FK a obras_sociales.id_obra_social)
        /// </summary>
        public int? IdObraSocial { get; set; }
        #endregion

        #region Propiedades de navegación
        /// <summary>
        /// Obra social del paciente (navegación)
        /// </summary>
        public ObraSocial ObraSocial { get; set; }
        #endregion

        #region Propiedades calculadas
        /// <summary>
        /// Nombre completo del paciente
        /// </summary>
        public string NombreCompleto => $"{Nombre} {Apellido}";

        /// <summary>
        /// Edad del paciente
        /// </summary>
        public int Edad
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// Descripción del sexo
        /// </summary>
        public string SexoDescripcion
        {
            get
            {
                return Sexo switch
                {
                    "M" => "Masculino",
                    "F" => "Femenino",
                    "X" => "No binario",
                    _ => "No especificado"
                };
            }
        }
        #endregion

        #region Métodos de validación
        /// <summary>
        /// Valida los datos del paciente
        /// </summary>
        public bool EsValido()
        {
            return Dni > 0 &&
                   !string.IsNullOrWhiteSpace(Nombre) &&
                   !string.IsNullOrWhiteSpace(Apellido) &&
                   FechaNacimiento <= DateTime.Today &&
                   (Sexo == "M" || Sexo == "F" || Sexo == "X") &&
                   (string.IsNullOrEmpty(Email) || Email.Contains("@"));
        }
        #endregion

        #region Propiedades de compatibilidad (deprecated)
        [Obsolete("Use Dni instead")]
        public int dni
        {
            get => Dni;
            set => Dni = value;
        }

        [Obsolete("Use Dni instead")]
        public int NroDoc => Dni;

        [Obsolete("Not used in new structure")]
        public string TipoDoc { get; set; }

        [Obsolete("Use Email instead")]
        public string Mail
        {
            get => Email;
            set => Email = value;
        }

        [Obsolete("Not used in new structure")]
        public string Direccion { get; set; }

        [Obsolete("Not used in new structure")]
        public string Localidad { get; set; }

        [Obsolete("Not used in new structure")]
        public string Provincia { get; set; }
        #endregion
    }
}