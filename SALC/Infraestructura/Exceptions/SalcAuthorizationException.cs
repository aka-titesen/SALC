using System;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando no se tiene permiso para realizar una operación
    /// </summary>
    public class SalcAuthorizationException : SalcException
    {
        /// <summary>
        /// Operación a la que se intentó acceder
        /// </summary>
        public string Operacion { get; set; }

        /// <summary>
        /// Rol requerido para la operación
        /// </summary>
        public string RolRequerido { get; set; }

        public SalcAuthorizationException() : base()
        {
            ErrorCode = "AUTHORIZATION_ERROR";
            UserFriendlyMessage = "No tiene permisos para realizar esta operación.";
        }

        public SalcAuthorizationException(string message) : base(message)
        {
            ErrorCode = "AUTHORIZATION_ERROR";
            UserFriendlyMessage = message;
        }

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
