using System;
using System.Data.SqlClient;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción que se lanza cuando ocurre un error en la capa de acceso a datos.
    /// Proporciona mensajes amigables según el tipo de error de SQL Server.
    /// </summary>
    public class SalcDatabaseException : SalcException
    {
        /// <summary>
        /// Número de error de SQL Server (si aplica)
        /// </summary>
        public int? SqlErrorNumber { get; set; }

        /// <summary>
        /// Nombre de la operación que falló
        /// </summary>
        public string Operacion { get; set; }

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public SalcDatabaseException() : base()
        {
            ErrorCode = "DATABASE_ERROR";
            UserFriendlyMessage = "Ha ocurrido un error al acceder a la base de datos. Por favor, contacte al administrador del sistema.";
        }

        /// <summary>
        /// Constructor con mensaje de error
        /// </summary>
        /// <param name="message">Descripción del error</param>
        public SalcDatabaseException(string message) : base(message)
        {
            ErrorCode = "DATABASE_ERROR";
            UserFriendlyMessage = "Ha ocurrido un error al acceder a la base de datos. Por favor, contacte al administrador del sistema.";
        }

        /// <summary>
        /// Constructor con mensaje y excepción SQL
        /// </summary>
        /// <param name="message">Descripción del error</param>
        /// <param name="sqlException">Excepción SQL original</param>
        public SalcDatabaseException(string message, SqlException sqlException) 
            : base(message, sqlException)
        {
            ErrorCode = "DATABASE_ERROR";
            SqlErrorNumber = sqlException?.Number;
            UserFriendlyMessage = ObtenerMensajeAmigable(sqlException);
        }

        /// <summary>
        /// Constructor con mensaje, operación y excepción SQL
        /// </summary>
        /// <param name="message">Descripción del error</param>
        /// <param name="operacion">Operación que se estaba ejecutando</param>
        /// <param name="sqlException">Excepción SQL original</param>
        public SalcDatabaseException(string message, string operacion, SqlException sqlException) 
            : base(message, sqlException)
        {
            Operacion = operacion;
            ErrorCode = "DATABASE_ERROR";
            SqlErrorNumber = sqlException?.Number;
            UserFriendlyMessage = ObtenerMensajeAmigable(sqlException);
        }

        /// <summary>
        /// Genera un mensaje amigable al usuario según el tipo de error SQL
        /// </summary>
        /// <param name="sqlException">Excepción SQL a traducir</param>
        /// <returns>Mensaje amigable para el usuario</returns>
        private string ObtenerMensajeAmigable(SqlException sqlException)
        {
            if (sqlException == null)
                return "Error de base de datos desconocido.";

            switch (sqlException.Number)
            {
                case -1:
                    return "La operación tardó demasiado tiempo. Por favor, intente nuevamente.";
                
                case 2:
                case 53:
                    return "No se pudo conectar al servidor de base de datos. Verifique su conexión de red.";
                
                case 4060:
                    return "La base de datos no está disponible. Contacte al administrador del sistema.";
                
                case 18456:
                    return "Error de autenticación con la base de datos. Contacte al administrador del sistema.";
                
                case 547:
                    return "No se puede completar la operación porque existen registros relacionados.";
                
                case 2627:
                case 2601:
                    return "Ya existe un registro con los mismos datos. Por favor, verifique la información ingresada.";
                
                case 8152:
                    return "Uno de los valores ingresados es demasiado largo.";
                
                default:
                    return "Ha ocurrido un error al acceder a la base de datos. Por favor, contacte al administrador del sistema.";
            }
        }
    }
}
