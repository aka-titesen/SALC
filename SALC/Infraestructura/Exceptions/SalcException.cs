using System;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción base para todas las excepciones personalizadas del sistema SALC
    /// </summary>
    public class SalcException : Exception
    {
        /// <summary>
        /// Código de error específico del sistema
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Mensaje amigable para mostrar al usuario
        /// </summary>
        public string UserFriendlyMessage { get; set; }

        /// <summary>
        /// Indica si el error debe ser registrado en el log
        /// </summary>
        public bool ShouldLog { get; set; }

        public SalcException() : base()
        {
            ShouldLog = true;
        }

        public SalcException(string message) : base(message)
        {
            UserFriendlyMessage = message;
            ShouldLog = true;
        }

        public SalcException(string message, Exception innerException) 
            : base(message, innerException)
        {
            UserFriendlyMessage = message;
            ShouldLog = true;
        }

        public SalcException(string message, string userFriendlyMessage, string errorCode = null) 
            : base(message)
        {
            UserFriendlyMessage = userFriendlyMessage;
            ErrorCode = errorCode;
            ShouldLog = true;
        }
    }
}
