using System.Configuration;
using System.Data.SqlClient;

namespace SALC.Infraestructura
{
    /// <summary>
    /// Clase estática para la gestión de conexiones a la base de datos.
    /// Centraliza la creación de conexiones SQL Server usando la cadena de conexión del archivo de configuración.
    /// </summary>
    public static class DbConexion
    {
        private const string NombreConexion = "SALC";

        /// <summary>
        /// Crea una nueva conexión a la base de datos SQL Server
        /// </summary>
        /// <returns>Nueva instancia de SqlConnection configurada con la cadena de conexión del sistema</returns>
        public static SqlConnection CrearConexion()
        {
            var cadena = ConfigurationManager.ConnectionStrings[NombreConexion]?.ConnectionString;
            return new SqlConnection(cadena);
        }
    }
}
