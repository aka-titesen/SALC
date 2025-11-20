using System;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando falla una regla de negocio
    /// </summary>
    public class SalcBusinessException : SalcException
    {
        public SalcBusinessException() : base()
        {
            ErrorCode = "BUSINESS_RULE_ERROR";
        }

        public SalcBusinessException(string message) : base(message)
        {
            ErrorCode = "BUSINESS_RULE_ERROR";
            UserFriendlyMessage = message;
        }

        public SalcBusinessException(string message, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = "BUSINESS_RULE_ERROR";
            UserFriendlyMessage = message;
        }
    }
}
