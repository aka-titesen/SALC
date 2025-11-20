using System;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción que se lanza cuando un usuario no tiene permisos para realizar una operación.
    /// Permite identificar la operación y el rol requerido.
    /// </summary>
    public class SalcAuthorizationException : SalcException
    {
        /// <summary>
        /// Operación a la que se intentó acceder sin autorización
        /// </summary>
        public string Operacion { get; set; }

        /// <summary>
        /// Rol requerido para realizar la operación
        /// </summary>
        public string RolRequerido { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public SalcAuthorizationException() : base()
        {
            ErrorCode = "AUTHORIZATION_ERROR";
            UserFriendlyMessage = "No tiene permisos para realizar esta operación.";
        }

        /// <summary>
        /// Constructor con mensaje de error
        /// </summary>
        /// <param name="message">Descripción del error de autorización</param>
        public SalcAuthorizationException(string message) : base(message)
        {
            ErrorCode = "AUTHORIZATION_ERROR";
            UserFriendlyMessage = message;
        }

        /// <summary>
        /// Constructor con operación y rol requerido
        /// </summary>
        /// <param name="operacion">Operación que se intentó realizar</param>
        /// <param name="rolRequerido">Rol necesario para la operación</param>
        public SalcAuthorizationException(string operacion, string rolRequerido) 
            : base($"No autorizado para {operacion}. Se requiere rol: {rolRequerido}")
        {
            Operacion = operacion;
            RolRequerido = rolRequerido;
            ErrorCode = "AUTHORIZATION_ERROR";
            UserFriendlyMessage = "No tiene permisos para realizar esta operación.";
        }
    }
}
