using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class TipoAnalisisMetricaRepositorio
    {
        public IEnumerable<TipoAnalisisMetrica> ObtenerRelaciones()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"
                SELECT tam.id_tipo_analisis, tam.id_metrica, 
                       ta.descripcion as descripcion_tipo_analisis,
                       m.nombre as nombre_metrica, m.unidad_medida
                FROM tipo_analisis_metrica tam
                INNER JOIN tipos_analisis ta ON tam.id_tipo_analisis = ta.id_tipo_analisis
                INNER JOIN metricas m ON tam.id_metrica = m.id_metrica
                WHERE ta.estado = 'Activo' AND m.estado = 'Activo'
                ORDER BY ta.descripcion, m.nombre", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        yield return new TipoAnalisisMetrica
                        {
                            IdTipoAnalisis = rd.GetInt32(0), // id_tipo_analisis
                            IdMetrica = rd.GetInt32(1), // id_metrica
                            DescripcionTipoAnalisis = rd.GetString(2), // descripcion_tipo_analisis
                            NombreMetrica = rd.GetString(3), // nombre_metrica
                            UnidadMedidaMetrica = rd.GetString(4) // unidad_medida
                        };
                    }
                }
            }
        }

        public IEnumerable<Metrica> ObtenerMetricasPorTipoAnalisis(int idTipoAnalisis)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(@"
                SELECT m.id_metrica, m.nombre, m.unidad_medida, m.valor_minimo, m.valor_maximo, m.estado
                FROM metricas m
                INNER JOIN tipo_analisis_metrica tam ON m.id_metrica = tam.id_metrica
                WHERE tam.id_tipo_analisis = @idTipoAnalisis AND m.estado = 'Activo'
                ORDER BY m.nombre", cn))
            {
                cmd.Parameters.AddWithValue("@idTipoAnalisis", idTipoAnalisis);
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        yield return new Metrica
                        {
                            IdMetrica = rd.GetInt32(0), // id_metrica
                            Nombre = rd.GetString(1), // nombre
                            UnidadMedida = rd.GetString(2), // unidad_medida
                            ValorMinimo = rd.IsDBNull(3) ? (decimal?)null : rd.GetDecimal(3), // valor_minimo
                            ValorMaximo = rd.IsDBNull(4) ? (decimal?)null : rd.GetDecimal(4), // valor_maximo
                            Estado = rd.GetString(5) // estado
                        };
                    }
                }
            }
        }

        public void CrearRelacion(int idTipoAnalisis, int idMetrica)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("INSERT INTO tipo_analisis_metrica (id_tipo_analisis, id_metrica) VALUES (@idTipoAnalisis, @idMetrica)", cn))
            {
                cmd.Parameters.AddWithValue("@idTipoAnalisis", idTipoAnalisis);
                cmd.Parameters.AddWithValue("@idMetrica", idMetrica);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarRelacion(int idTipoAnalisis, int idMetrica)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("DELETE FROM tipo_analisis_metrica WHERE id_tipo_analisis = @idTipoAnalisis AND id_metrica = @idMetrica", cn))
            {
                cmd.Parameters.AddWithValue("@idTipoAnalisis", idTipoAnalisis);
                cmd.Parameters.AddWithValue("@idMetrica", idMetrica);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarTodasLasRelacionesDeTipoAnalisis(int idTipoAnalisis)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("DELETE FROM tipo_analisis_metrica WHERE id_tipo_analisis = @idTipoAnalisis", cn))
            {
                cmd.Parameters.AddWithValue("@idTipoAnalisis", idTipoAnalisis);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool ExisteRelacion(int idTipoAnalisis, int idMetrica)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM tipo_analisis_metrica WHERE id_tipo_analisis = @idTipoAnalisis AND id_metrica = @idMetrica", cn))
            {
                cmd.Parameters.AddWithValue("@idTipoAnalisis", idTipoAnalisis);
                cmd.Parameters.AddWithValue("@idMetrica", idMetrica);
                cn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}