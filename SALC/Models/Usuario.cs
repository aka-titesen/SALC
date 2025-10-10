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
		#region Propiedades de la tabla usuarios
		/// <summary>
		/// DNI del usuario (Clave primaria)
		/// </summary>
		public int Dni { get; set; }

		/// <summary>
		/// Nombre del usuario
		/// </summary>
		public string Nombre { get; set; }

		/// <summary>
		/// Apellido del usuario
		/// </summary>
		public string Apellido { get; set; }

		/// <summary>
		/// Email del usuario (�nico)
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// Hash de la contrase�a (BCrypt)
		/// </summary>
		public string PasswordHash { get; set; }

		/// <summary>
		/// ID del rol (FK a roles.id_rol)
		/// </summary>
		public int IdRol { get; set; }

		/// <summary>
		/// Estado del usuario ('Activo' o 'Inactivo')
		/// </summary>
		public string Estado { get; set; }
		#endregion

		#region Propiedades extendidas para m�dicos (tabla medicos)
		/// <summary>
		/// N�mero de matr�cula (solo para m�dicos)
		/// </summary>
		public int? NumeroMatricula { get; set; }

		/// <summary>
		/// Especialidad m�dica (solo para m�dicos)
		/// </summary>
		public string Especialidad { get; set; }
		#endregion

		#region Propiedades extendidas para asistentes (tabla asistentes)
		/// <summary>
		/// DNI del supervisor (solo para asistentes)
		/// </summary>
		public int? DniSupervisor { get; set; }

		/// <summary>
		/// Fecha de ingreso (solo para asistentes)
		/// </summary>
		public DateTime? FechaIngreso { get; set; }

		/// <summary>
		/// Nombre del supervisor (calculado)
		/// </summary>
		public string NombreSupervisor { get; set; }
		#endregion

		#region Propiedades de navegaci�n y calculadas
		/// <summary>
		/// Nombre del rol (calculado desde tabla roles)
		/// </summary>
		public string NombreRol { get; set; }

		/// <summary>
		/// Nombre completo del usuario
		/// </summary>
		public string NombreCompleto => $"{Nombre} {Apellido}";

		/// <summary>
		/// Verifica si el usuario es administrador (id_rol = 1)
		/// </summary>
		public bool EsAdministrador => IdRol == 1;

		/// <summary>
		/// Verifica si el usuario es m�dico (id_rol = 2)
		/// </summary>
		public bool EsMedico => IdRol == 2;

		/// <summary>
		/// Verifica si el usuario es asistente (id_rol = 3)
		/// </summary>
		public bool EsAsistente => IdRol == 3;

		/// <summary>
		/// Verifica si el usuario est� activo
		/// </summary>
		public bool EstaActivo => Estado == "Activo";
		#endregion

		#region M�todos de validaci�n
		/// <summary>
		/// Valida los datos b�sicos del usuario
		/// </summary>
		public bool EsValido()
		{
			return Dni > 0 &&
                   !string.IsNullOrWhiteSpace(Nombre) &&
                   !string.IsNullOrWhiteSpace(Apellido) &&
                   IdRol > 0 &&
                   (string.IsNullOrEmpty(Email) || Email.Contains("@")) &&
                   !string.IsNullOrWhiteSpace(Estado);
		}

		/// <summary>
		/// Valida los datos espec�ficos seg�n el rol del usuario
		/// </summary>
		public string ValidarDatosRol()
		{
			if (EsMedico)
			{
				if (!NumeroMatricula.HasValue || NumeroMatricula <= 0)
					return "El n�mero de matr�cula es obligatorio para m�dicos.";

				if (string.IsNullOrWhiteSpace(Especialidad))
					return "La especialidad es obligatoria para m�dicos.";
			}

			if (EsAsistente)
			{
				if (!DniSupervisor.HasValue || DniSupervisor <= 0)
					return "El supervisor es obligatorio para asistentes.";

				if (!FechaIngreso.HasValue)
					return "La fecha de ingreso es obligatoria para asistentes.";
			}

			return null; // Sin errores
		}
		#endregion

		#region Propiedades de compatibilidad (deprecated - usar nuevas propiedades)
		[Obsolete("Use PasswordHash instead")]
		public string password
		{
			get => PasswordHash;
			set => PasswordHash = value;
		}

		[Obsolete("Use IdRol instead")]
		public int id_rol
		{
			get => IdRol;
			set => IdRol = value;
		}

		[Obsolete("Use NombreRol instead")]
		public string Rol
		{
			get => NombreRol;
			set => NombreRol = value;
		}

		[Obsolete("Use Estado string property instead")]
		public int? estado_usuario
		{
			get => Estado == "Activo" ? 1 : (Estado == "Inactivo" ? 2 : (int?)null);
			set => Estado = value == 1 ? "Activo" : (value == 2 ? "Inactivo" : "");
		}

		[Obsolete("Not used in new structure")]
		public string Telefono { get; set; }

		[Obsolete("Not used in new structure")]
		public int? NumeroLegajo { get; set; }
		#endregion
	}
}