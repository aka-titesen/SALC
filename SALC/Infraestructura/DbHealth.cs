using System;
using System.Data.SqlClient;
using System.Configuration;

namespace SALC.Infraestructura
{
    public class DbHealthResult
    {
        public bool Exito { get; set; }
        public string Detalle { get; set; }
    }

    public static class DbHealth
    {
        public static DbHealthResult ProbarConexion(int timeoutSegundos = 5)
        {
            try
            {
                // Tomar la cadena actual y forzar un timeout corto para la prueba
                var original = System.Configuration.ConfigurationManager.ConnectionStrings["SALC"].ConnectionString;
                var builder = new SqlConnectionStringBuilder(original) { ConnectTimeout = timeoutSegundos };
                using (var cn = new SqlConnection(builder.ToString()))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", cn))
                    {
                        cmd.ExecuteScalar();
                    }
                }
                return new DbHealthResult { Exito = true, Detalle = "Conexión OK" };
            }
            catch (Exception ex)
            {
                return new DbHealthResult { Exito = false, Detalle = ex.Message };
            }
        }
    }
}
