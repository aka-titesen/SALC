using System;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción que se lanza cuando falla una regla de negocio del sistema.
    /// Se utiliza para errores relacionados con la lógica del dominio y restricciones del negocio.
    /// </summary>
    public class SalcBusinessException : SalcException
    {
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public SalcBusinessException() : base()
        {
            ErrorCode = "BUSINESS_RULE_ERROR";
        }

        /// <summary>
        /// Constructor con mensaje de error
        /// </summary>
        /// <param name="message">Descripción del error de negocio</param>
        public SalcBusinessException(string message) : base(message)
        {
            ErrorCode = "BUSINESS_RULE_ERROR";
            UserFriendlyMessage = message;
        }

        /// <summary>
        /// Constructor con mensaje de error y excepción interna
        /// </summary>
        /// <param name="message">Descripción del error de negocio</param>
        /// <param name="innerException">Excepción original que causó este error</param>
        public SalcBusinessException(string message, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = "BUSINESS_RULE_ERROR";
            UserFriendlyMessage = message;
        }
    }
}
