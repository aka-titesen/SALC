using System;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando falla una validación de datos o regla de negocio
    /// </summary>
    public class SalcValidacionException : SalcException
    {
        /// <summary>
        /// Campo o propiedad que falló la validación
        /// </summary>
        public string Campo { get; set; }

        public SalcValidacionException() : base()
        {
            ErrorCode = "VALIDATION_ERROR";
        }

        public SalcValidacionException(string message) : base(message)
        {
            ErrorCode = "VALIDATION_ERROR";
            UserFriendlyMessage = message;
        }

        public SalcValidacionException(string message, string campo) : base(message)
        {
            Campo = campo;
            ErrorCode = "VALIDATION_ERROR";
            UserFriendlyMessage = message;
        }

        public SalcValidacionException(string message, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = "VALIDATION_ERROR";
            UserFriendlyMessage = message;
        }
    }
}
