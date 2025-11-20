using System;
using System.Data.SqlClient;
using System.Configuration;

namespace SALC.Infraestructura
{
    /// <summary>
    /// Resultado de una prueba de salud de la conexión a la base de datos
    /// </summary>
    public class DbHealthResult
    {
        /// <summary>
        /// Indica si la prueba de conexión fue exitosa
        /// </summary>
        public bool Exito { get; set; }

        /// <summary>
        /// Detalle del resultado o mensaje de error
        /// </summary>
        public string Detalle { get; set; }
    }

    /// <summary>
    /// Clase estática para verificar la salud de la conexión a la base de datos.
    /// Permite probar la conectividad antes de realizar operaciones.
    /// </summary>
    public static class DbHealth
    {
        /// <summary>
        /// Prueba la conexión a la base de datos ejecutando una consulta simple
        /// </summary>
        /// <param name="timeoutSegundos">Tiempo máximo de espera en segundos para la conexión</param>
        /// <returns>Resultado de la prueba de conexión con el detalle del estado</returns>
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
