using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos de obras sociales.
    /// Proporciona operaciones de consulta sobre la tabla de obras sociales.
    /// </summary>
    public class ObraSocialRepositorio
    {
        /// <summary>
        /// Obtiene una obra social por su identificador
        /// </summary>
        /// <param name="id">Identificador de la obra social</param>
        /// <returns>La obra social encontrada o null si no existe</returns>
        public ObraSocial ObtenerPorId(int id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_obra_social, cuit, nombre, estado FROM obras_sociales WHERE id_obra_social = @id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return new ObraSocial
                        {
                            IdObraSocial = rd.GetInt32(0),
                            Cuit = rd.GetString(1),
                            Nombre = rd.GetString(2),
                            Estado = rd.GetString(3)
                        };
                    }
                }
            }
            return null;
        }
    }
}