using System;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción que se lanza cuando falla una validación de datos o regla de negocio.
    /// Permite identificar el campo específico que causó el error de validación.
    /// </summary>
    public class SalcValidacionException : SalcException
    {
        /// <summary>
        /// Nombre del campo o propiedad que falló la validación
        /// </summary>
        public string Campo { get; set; }

        /// <summary>
        /// Constructor por defecto que establece el código de error genérico
        /// </summary>
        public SalcValidacionException() : base()
        {
            ErrorCode = "VALIDATION_ERROR";
        }

        /// <summary>
        /// Constructor con mensaje de error descriptivo
        /// </summary>
        /// <param name="message">Descripción del error de validación</param>
        public SalcValidacionException(string message) : base(message)
        {
            ErrorCode = "VALIDATION_ERROR";
            UserFriendlyMessage = message;
        }

        /// <summary>
        /// Constructor con mensaje de error y campo específico que falló
        /// </summary>
        /// <param name="message">Descripción del error de validación</param>
        /// <param name="campo">Nombre del campo que causó el error</param>
        public SalcValidacionException(string message, string campo) : base(message)
        {
            Campo = campo;
            ErrorCode = "VALIDATION_ERROR";
            UserFriendlyMessage = message;
        }

        /// <summary>
        /// Constructor con mensaje de error y excepción interna
        /// </summary>
        /// <param name="message">Descripción del error de validación</param>
        /// <param name="innerException">Excepción original que causó este error</param>
        public SalcValidacionException(string message, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = "VALIDATION_ERROR";
            UserFriendlyMessage = message;
        }
    }
}
