using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class AnalisisMetricaRepositorio
    {
        public void UpsertResultado(AnalisisMetrica am)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"MERGE analisis_metrica AS target
USING (SELECT @id_analisis id_analisis, @id_metrica id_metrica) AS src
ON (target.id_analisis = src.id_analisis AND target.id_metrica = src.id_metrica)
WHEN MATCHED THEN UPDATE SET resultado=@resultado, observaciones=@obs
WHEN NOT MATCHED THEN INSERT (id_analisis, id_metrica, resultado, observaciones) VALUES (@id_analisis, @id_metrica, @resultado, @obs);", cn))
            {
                cmd.Parameters.AddWithValue("@id_analisis", am.IdAnalisis);
                cmd.Parameters.AddWithValue("@id_metrica", am.IdMetrica);
                cmd.Parameters.AddWithValue("@resultado", am.Resultado);
                cmd.Parameters.AddWithValue("@obs", (object)am.Observaciones ?? (object)System.DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<AnalisisMetrica> ObtenerResultados(int idAnalisis)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"SELECT id_analisis, id_metrica, resultado, observaciones
                FROM analisis_metrica WHERE id_analisis=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", idAnalisis);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        yield return new AnalisisMetrica
                        {
                            IdAnalisis = rd.GetInt32(0),
                            IdMetrica = rd.GetInt32(1),
                            Resultado = rd.GetDecimal(2),
                            Observaciones = rd.IsDBNull(3) ? null : rd.GetString(3)
                        };
                    }
                }
            }
        }
    }
}
