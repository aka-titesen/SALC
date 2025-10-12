using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.Domain;
using SALC.Infraestructura;

namespace SALC.DAL
{
    public class CatalogoRepositorio
    {
        // CRUD básico adicional para Sprint 6 (ediciones simples) - Actualizado con baja lógica
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

        public void EliminarObraSocial(int id)
        {
            // Baja lógica en lugar de eliminación física
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE obras_sociales SET estado='Inactivo' WHERE id_obra_social=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

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

        public void EliminarTipoAnalisis(int id)
        {
            // Baja lógica en lugar de eliminación física
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE tipos_analisis SET estado='Inactivo' WHERE id_tipo_analisis=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

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

        public void EliminarMetrica(int id)
        {
            // Baja lógica en lugar de eliminación física
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand("UPDATE metricas SET estado='Inactivo' WHERE id_metrica=@id", cn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open(); cmd.ExecuteNonQuery();
            }
        }

        // Métodos de consulta actualizados para incluir el estado
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

        // Método para obtener solo obras sociales activas
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

        // Método para obtener solo tipos de análisis activos
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

        // Método para obtener solo métricas activas
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
    }
}
