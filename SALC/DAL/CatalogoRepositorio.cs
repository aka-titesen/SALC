using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class CatalogoRepositorio
    {
        public IEnumerable<ObraSocial> ObtenerObrasSociales()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_obra_social, cuit, nombre FROM obras_sociales", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        yield return new ObraSocial
                        {
                            IdObraSocial = rd.GetInt32(0),
                            Cuit = rd.GetString(1),
                            Nombre = rd.GetString(2)
                        };
                    }
                }
            }
        }

        public IEnumerable<TipoAnalisis> ObtenerTiposAnalisis()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_tipo_analisis, descripcion FROM tipos_analisis", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        yield return new TipoAnalisis
                        {
                            IdTipoAnalisis = rd.GetInt32(0),
                            Descripcion = rd.GetString(1)
                        };
                    }
                }
            }
        }

        public IEnumerable<Metrica> ObtenerMetricas()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_metrica, nombre, unidad_medida, valor_minimo, valor_maximo FROM metricas", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        yield return new Metrica
                        {
                            IdMetrica = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            UnidadMedida = rd.GetString(2),
                            ValorMinimo = rd.IsDBNull(3) ? (decimal?)null : rd.GetDecimal(3),
                            ValorMaximo = rd.IsDBNull(4) ? (decimal?)null : rd.GetDecimal(4)
                        };
                    }
                }
            }
        }
    }
}
