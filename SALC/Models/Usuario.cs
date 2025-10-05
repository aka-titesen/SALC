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
    }
}