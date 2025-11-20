using System;
using System.Data.SqlClient;

namespace SALC.Infraestructura.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando ocurre un error en la capa de acceso a datos
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

        public SalcDatabaseException() : base()
        {
            ErrorCode = "DATABASE_ERROR";
            UserFriendlyMessage = "Ha ocurrido un error al acceder a la base de datos. Por favor, contacte al administrador del sistema.";
        }

        public SalcDatabaseException(string message) : base(message)
        {
            ErrorCode = "DATABASE_ERROR";
            UserFriendlyMessage = "Ha ocurrido un error al acceder a la base de datos. Por favor, contacte al administrador del sistema.";
        }

        public SalcDatabaseException(string message, SqlException sqlException) 
            : base(message, sqlException)
        {
            ErrorCode = "DATABASE_ERROR";
            SqlErrorNumber = sqlException?.Number;
            
            // Mensajes específicos según el tipo de error SQL
            UserFriendlyMessage = ObtenerMensajeAmigable(sqlException);
        }

        public SalcDatabaseException(string message, string operacion, SqlException sqlException) 
            : base(message, sqlException)
        {
            Operacion = operacion;
            ErrorCode = "DATABASE_ERROR";
            SqlErrorNumber = sqlException?.Number;
            UserFriendlyMessage = ObtenerMensajeAmigable(sqlException);
        }

        private string ObtenerMensajeAmigable(SqlException sqlException)
        {
            if (sqlException == null)
                return "Error de base de datos desconocido.";

            switch (sqlException.Number)
            {
                case -1: // Timeout
                    return "La operación tardó demasiado tiempo. Por favor, intente nuevamente.";
                
                case 2: // Network error
                case 53:
                    return "No se pudo conectar al servidor de base de datos. Verifique su conexión de red.";
                
                case 4060: // Cannot open database
                    return "La base de datos no está disponible. Contacte al administrador del sistema.";
                
                case 18456: // Login failed
                    return "Error de autenticación con la base de datos. Contacte al administrador del sistema.";
                
                case 547: // Foreign key violation
                    return "No se puede completar la operación porque existen registros relacionados.";
                
                case 2627: // Unique constraint violation
                case 2601:
                    return "Ya existe un registro con los mismos datos. Por favor, verifique la información ingresada.";
                
                case 8152: // String truncation
                    return "Uno de los valores ingresados es demasiado largo.";
                
                default:
                    return "Ha ocurrido un error al acceder a la base de datos. Por favor, contacte al administrador del sistema.";
            }
        }
    }
}
