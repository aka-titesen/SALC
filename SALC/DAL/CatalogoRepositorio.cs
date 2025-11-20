using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos de los catálogos del sistema.
    /// Gestiona obras sociales, tipos de análisis y métricas.
    /// </summary>
    public class CatalogoRepositorio
    {
        #region Obras Sociales

        /// <summary>
        /// Crea una nueva obra social en la base de datos
        /// </summary>
        /// <param name="os">Obra social a crear</param>
        public void CrearObraSocial(ObraSocial os)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("INSERT INTO obras_sociales (cuit, nombre, estado) VALUES (@cuit, @nombre, @estado)", cn))
            {
                cmd.Parameters.AddWithValue("@cuit", os.Cuit);
                cmd.Parameters.AddWithValue("@nombre", os.Nombre);
                cmd.Parameters.AddWithValue("@estado", os.Estado ?? "Activo");
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza los datos de una obra social existente
        /// </summary>
        /// <param name="os">Obra social con los datos actualizados</param>
        public void ActualizarObraSocial(ObraSocial os)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE obras_sociales SET cuit=@cuit, nombre=@nombre, estado=@estado WHERE id_obra_social=@id", cn))
            {
                cmd.Parameters.AddWithValue("@cuit", os.Cuit);
                cmd.Parameters.AddWithValue("@nombre", os.Nombre);
                cmd.Parameters.AddWithValue("@estado", os.Estado ?? "Activo");
                cmd.Parameters.AddWithValue("@id", os.IdObraSocial);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina una obra social mediante baja lógica
        /// </summary>
        /// <param name="id">Identificador de la obra social</param>
        public void EliminarObraSocial(int id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE obras_sociales SET estado='Inactivo' WHERE id_obra_social=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene todas las obras sociales del sistema
        /// </summary>
        /// <returns>Colección de todas las obras sociales</returns>
        public IEnumerable<ObraSocial> ObtenerObrasSociales()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_obra_social, cuit, nombre, estado FROM obras_sociales", cn))
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
                            Nombre = rd.GetString(2),
                            Estado = rd.GetString(3)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene solo las obras sociales activas del sistema
        /// </summary>
        /// <returns>Colección de obras sociales activas</returns>
        public IEnumerable<ObraSocial> ObtenerObrasSocialesActivas()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_obra_social, cuit, nombre, estado FROM obras_sociales WHERE estado = 'Activo'", cn))
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
                            Nombre = rd.GetString(2),
                            Estado = rd.GetString(3)
                        };
                    }
                }
            }
        }

        #endregion

        #region Tipos de Análisis

        /// <summary>
        /// Crea un nuevo tipo de análisis en la base de datos
        /// </summary>
        /// <param name="ta">Tipo de análisis a crear</param>
        public void CrearTipoAnalisis(TipoAnalisis ta)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("INSERT INTO tipos_analisis (descripcion, estado) VALUES (@d, @estado)", cn))
            {
                cmd.Parameters.AddWithValue("@d", ta.Descripcion);
                cmd.Parameters.AddWithValue("@estado", ta.Estado ?? "Activo");
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza los datos de un tipo de análisis existente
        /// </summary>
        /// <param name="ta">Tipo de análisis con los datos actualizados</param>
        public void ActualizarTipoAnalisis(TipoAnalisis ta)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE tipos_analisis SET descripcion=@d, estado=@estado WHERE id_tipo_analisis=@id", cn))
            {
                cmd.Parameters.AddWithValue("@d", ta.Descripcion);
                cmd.Parameters.AddWithValue("@estado", ta.Estado ?? "Activo");
                cmd.Parameters.AddWithValue("@id", ta.IdTipoAnalisis);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina un tipo de análisis mediante baja lógica
        /// </summary>
        /// <param name="id">Identificador del tipo de análisis</param>
        public void EliminarTipoAnalisis(int id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE tipos_analisis SET estado='Inactivo' WHERE id_tipo_analisis=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de análisis del sistema
        /// </summary>
        /// <returns>Colección de todos los tipos de análisis</returns>
        public IEnumerable<TipoAnalisis> ObtenerTiposAnalisis()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_tipo_analisis, descripcion, estado FROM tipos_analisis", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        yield return new TipoAnalisis
                        {
                            IdTipoAnalisis = rd.GetInt32(0),
                            Descripcion = rd.GetString(1),
                            Estado = rd.GetString(2)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene solo los tipos de análisis activos del sistema
        /// </summary>
        /// <returns>Colección de tipos de análisis activos</returns>
        public IEnumerable<TipoAnalisis> ObtenerTiposAnalisisActivos()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_tipo_analisis, descripcion, estado FROM tipos_analisis WHERE estado = 'Activo'", cn))
            {
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        yield return new TipoAnalisis
                        {
                            IdTipoAnalisis = rd.GetInt32(0),
                            Descripcion = rd.GetString(1),
                            Estado = rd.GetString(2)
                        };
                    }
                }
            }
        }

        #endregion

        #region Métricas

        /// <summary>
        /// Crea una nueva métrica en la base de datos
        /// </summary>
        /// <param name="m">Métrica a crear</param>
        public void CrearMetrica(Metrica m)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("INSERT INTO metricas (nombre, unidad_medida, valor_minimo, valor_maximo, estado) VALUES (@n, @u, @min, @max, @estado)", cn))
            {
                cmd.Parameters.AddWithValue("@n", m.Nombre);
                cmd.Parameters.AddWithValue("@u", m.UnidadMedida);
                cmd.Parameters.AddWithValue("@min", (object)m.ValorMinimo ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@max", (object)m.ValorMaximo ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", m.Estado ?? "Activo");
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Actualiza los datos de una métrica existente
        /// </summary>
        /// <param name="m">Métrica con los datos actualizados</param>
        public void ActualizarMetrica(Metrica m)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE metricas SET nombre=@n, unidad_medida=@u, valor_minimo=@min, valor_maximo=@max, estado=@estado WHERE id_metrica=@id", cn))
            {
                cmd.Parameters.AddWithValue("@n", m.Nombre);
                cmd.Parameters.AddWithValue("@u", m.UnidadMedida);
                cmd.Parameters.AddWithValue("@min", (object)m.ValorMinimo ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@max", (object)m.ValorMaximo ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@estado", m.Estado ?? "Activo");
                cmd.Parameters.AddWithValue("@id", m.IdMetrica);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina una métrica mediante baja lógica
        /// </summary>
        /// <param name="id">Identificador de la métrica</param>
        public void EliminarMetrica(int id)
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE metricas SET estado='Inactivo' WHERE id_metrica=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene todas las métricas del sistema
        /// </summary>
        /// <returns>Colección de todas las métricas</returns>
        public IEnumerable<Metrica> ObtenerMetricas()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_metrica, nombre, unidad_medida, valor_minimo, valor_maximo, estado FROM metricas", cn))
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
                            ValorMaximo = rd.IsDBNull(4) ? (decimal?)null : rd.GetDecimal(4),
                            Estado = rd.GetString(5)
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene solo las métricas activas del sistema
        /// </summary>
        /// <returns>Colección de métricas activas</returns>
        public IEnumerable<Metrica> ObtenerMetricasActivas()
        {
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("SELECT id_metrica, nombre, unidad_medida, valor_minimo, valor_maximo, estado FROM metricas WHERE estado = 'Activo'", cn))
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
                            ValorMaximo = rd.IsDBNull(4) ? (decimal?)null : rd.GetDecimal(4),
                            Estado = rd.GetString(5)
                        };
                    }
                }
            }
        }

        #endregion
    }
}
