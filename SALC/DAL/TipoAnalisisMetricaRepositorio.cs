using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos de la relación entre tipos de análisis y métricas.
    /// Gestiona qué métricas se deben medir para cada tipo de análisis.
    /// </summary>
    public class TipoAnalisisMetricaRepositorio
    {
        /// <summary>
        /// Obtiene todas las relaciones activas entre tipos de análisis y métricas
        /// </summary>
        /// <returns>Colección de relaciones con información completa de tipo y métrica</returns>
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
                            IdTipoAnalisis = rd.GetInt32(0),
                            IdMetrica = rd.GetInt32(1),
                            DescripcionTipoAnalisis = rd.GetString(2),
                            NombreMetrica = rd.GetString(3),
                            UnidadMedidaMetrica = rd.GetString(4)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene todas las métricas activas asociadas a un tipo de análisis específico
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <returns>Colección de métricas del tipo de análisis</returns>
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
                            IdMetrica = rd.GetInt32(0),
                            Nombre = rd.GetString(1),
                            UnidadMedida = rd.GetString(2),
                            ValorMinimo = rd.IsDBNull(3) ? (decimal?)null : rd.GetDecimal(3),
                            ValorMaximo = rd.IsDBNull(4) ? (decimal?)null : rd.GetDecimal(4),
                            Estado = rd.GetString(5)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Crea una nueva relación entre un tipo de análisis y una métrica
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <param name="idMetrica">Identificador de la métrica</param>
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

        /// <summary>
        /// Elimina una relación específica entre un tipo de análisis y una métrica
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <param name="idMetrica">Identificador de la métrica</param>
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

        /// <summary>
        /// Elimina todas las relaciones de un tipo de análisis con todas sus métricas
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
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

        /// <summary>
        /// Verifica si existe una relación entre un tipo de análisis y una métrica
        /// </summary>
        /// <param name="idTipoAnalisis">Identificador del tipo de análisis</param>
        /// <param name="idMetrica">Identificador de la métrica</param>
        /// <returns>True si existe la relación, false en caso contrario</returns>
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