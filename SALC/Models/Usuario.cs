// Models/Usuario.cs
using System;

// Models/Usuario.cs
namespace SALC
{
	/// <summary>
	/// Represents a user within the SALC system.
	/// </summary>
	public class Usuario
	{
		/// <summary>
		/// Gets or sets the user's DNI (National Identity Document number).
		/// </summary>
		public int Dni { get; set; }

		/// <summary>
		/// Gets or sets the user's first name.
		/// </summary>
		public string Nombre { get; set; }

		/// <summary>
		/// Gets or sets the user's last name.
		/// </summary>
		public string Apellido { get; set; }

		/// <summary>
		/// Gets or sets the user's role name (e.g., "Administrador", "Clinico").
		/// </summary>
		public string Rol { get; set; }

		public int id_rol {  get; set; }

		/// <summary>
		/// Gets or sets the user's email address.
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Gets or sets the user's phone number.
		/// </summary>
		public string Telefono { get; set; }

        // Cambiado de ID_estado a estado_usuario para coincidir con la BD
		public int? estado_usuario { get; set; }

		// Cambiado de Pass a password para coincidir con la BD
		public string password { get; set; }

		// Propiedad calculada para el nombre del estado (opcional)
		public string Estado { get; set; } = "";

        // Propiedades adicionales para gestión completa de usuarios
        
        /// <summary>
        /// Número de matrícula para doctores/clínicos
        /// </summary>
        public int? NumeroMatricula { get; set; }
        
        /// <summary>
        /// Especialidad médica para doctores/clínicos
        /// </summary>
        public string Especialidad { get; set; }
        
        /// <summary>
        /// Número de legajo para asistentes
        /// </summary>
        public int? NumeroLegajo { get; set; }
        
        /// <summary>
        /// DNI del supervisor para asistentes
        /// </summary>
        public int? DniSupervisor { get; set; }
        
        /// <summary>
        /// Nombre del supervisor para asistentes
        /// </summary>
        public string NombreSupervisor { get; set; }
        
        /// <summary>
        /// Fecha de ingreso para asistentes
        /// </summary>
        public DateTime? FechaIngreso { get; set; }

        // Propiedades de conveniencia
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public bool EsDoctor => id_rol == 2; // Según ERS: rol=2 es Clínico
        public bool EsAsistente => id_rol == 3; // Según ERS: rol=3 es Asistente
        public bool EsAdministrador => id_rol == 1; // Según ERS: rol=1 es Administrador
        public bool EstaActivo => estado_usuario == 1; // Según BD: estado=1 es Activo

        /// <summary>
        /// Valida que los datos del usuario sean correctos
        /// </summary>
        public bool EsValido()
        {
            return Dni > 0 &&
                   !string.IsNullOrWhiteSpace(Nombre) &&
                   !string.IsNullOrWhiteSpace(Apellido) &&
                   id_rol > 0 &&
                   (string.IsNullOrEmpty(Email) || Email.Contains("@"));
        }

        /// <summary>
        /// Valida los datos específicos según el rol del usuario
        /// </summary>
        public string ValidarDatosRol()
        {
            if (EsDoctor && (NumeroMatricula == null || NumeroMatricula <= 0))
                return "El número de matrícula es obligatorio para clínicos.";

            if (EsDoctor && string.IsNullOrWhiteSpace(Especialidad))
                return "La especialidad es obligatoria para clínicos.";

            if (EsAsistente && (NumeroLegajo == null || NumeroLegajo <= 0))
                return "El número de legajo es obligatorio para asistentes.";

            if (EsAsistente && (DniSupervisor == null || DniSupervisor <= 0))
                return "El supervisor es obligatorio para asistentes.";

            return null; // Sin errores
        }
    }
}