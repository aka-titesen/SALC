using System.Configuration;
using System.Data.SqlClient;

namespace SALC.Infraestructura
{
    public static class DbConexion
    {
        private const string NombreConexion = "SALC";

        public static SqlConnection CrearConexion()
        {
            var cadena = ConfigurationManager.ConnectionStrings[NombreConexion]?.ConnectionString;
            return new SqlConnection(cadena);
        }
    }
}
