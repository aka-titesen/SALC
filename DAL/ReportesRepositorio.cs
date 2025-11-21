using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SALC.BLL;
using SALC.Infraestructura;

namespace SALC.DAL
{
    /// <summary>
    /// Repositorio para el acceso a datos de reportes y estadísticas del sistema.
    /// Proporciona consultas especializadas para generación de reportes gerenciales y médicos.
    /// </summary>
    public class ReportesRepositorio
    {
        /// <summary>
        /// Obtiene un reporte de productividad de médicos en un rango de fechas.
        /// Incluye cantidad de análisis creados y verificados por cada médico.
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista de reportes de productividad por médico</returns>
        public List<ReporteProductividad> ObtenerProductividadMedicos(DateTime desde, DateTime hasta)
        {
            var resultado = new List<ReporteProductividad>();
            
            var sql = @"
                SELECT 
                    u.dni,
                    u.nombre + ' ' + u.apellido AS NombreMedico,
                    COUNT(a.id_analisis) AS TotalCreados,
                    SUM(CASE WHEN a.id_estado = 2 THEN 1 ELSE 0 END) AS TotalVerificados
                FROM 
                    usuarios u
                INNER JOIN 
                    medicos m ON u.dni = m.dni
                LEFT JOIN 
                    analisis a ON u.dni = a.dni_carga 
                    AND a.fecha_creacion BETWEEN @desde AND @hasta
                    AND a.id_estado != 3
                WHERE 
                    u.id_rol = 2 AND u.estado = 'Activo'
                GROUP BY 
                    u.dni, u.nombre, u.apellido
                ORDER BY 
                    TotalVerificados DESC, TotalCreados DESC";
            
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        resultado.Add(new ReporteProductividad
                        {
                            DniMedico = rd.GetInt32(0),
                            NombreMedico = rd.GetString(1),
                            CantidadCreados = rd.GetInt32(2),
                            CantidadVerificados = rd.GetInt32(3)
                        });
                    }
                }
            }
            
            return resultado;
        }

        /// <summary>
        /// Obtiene un reporte de facturación agrupado por obra social en un rango de fechas.
        /// Incluye cantidad de análisis y porcentaje respecto al total.
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista de reportes de facturación por obra social</returns>
        public List<ReporteFacturacion> ObtenerFacturacionPorObraSocial(DateTime desde, DateTime hasta)
        {
            var resultado = new List<ReporteFacturacion>();
            
            var sql = @"
                SELECT 
                    ISNULL(os.nombre, 'Privado') AS NombreObraSocial,
                    COUNT(a.id_analisis) AS Cantidad,
                    CAST(COUNT(a.id_analisis) * 100.0 / SUM(COUNT(a.id_analisis)) OVER() AS DECIMAL(5,2)) AS Porcentaje
                FROM 
                    analisis a
                INNER JOIN 
                    pacientes p ON a.dni_paciente = p.dni
                LEFT JOIN 
                    obras_sociales os ON p.id_obra_social = os.id_obra_social
                WHERE 
                    a.fecha_creacion BETWEEN @desde AND @hasta
                    AND a.id_estado != 3
                GROUP BY 
                    os.nombre
                ORDER BY 
                    Cantidad DESC";
            
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        resultado.Add(new ReporteFacturacion
                        {
                            NombreObraSocial = rd.GetString(0),
                            CantidadAnalisis = rd.GetInt32(1),
                            Porcentaje = rd.GetDecimal(2)
                        });
                    }
                }
            }
            
            return resultado;
        }

        /// <summary>
        /// Obtiene los tipos de análisis más solicitados en un rango de fechas
        /// </summary>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <param name="top">Cantidad de tipos de análisis a retornar</param>
        /// <returns>Lista de tipos de análisis más demandados</returns>
        public List<ReporteDemanda> ObtenerTopAnalisis(DateTime desde, DateTime hasta, int top)
        {
            var resultado = new List<ReporteDemanda>();
            
            var sql = @"
                SELECT TOP (@top)
                    ta.descripcion AS TipoAnalisis,
                    COUNT(a.id_analisis) AS Cantidad
                FROM 
                    analisis a
                INNER JOIN 
                    tipos_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                WHERE 
                    a.fecha_creacion BETWEEN @desde AND @hasta
                    AND a.id_estado != 3
                GROUP BY 
                    ta.descripcion
                ORDER BY 
                    Cantidad DESC";
            
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@top", top);
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        resultado.Add(new ReporteDemanda
                        {
                            TipoAnalisis = rd.GetString(0),
                            CantidadSolicitados = rd.GetInt32(1)
                        });
                    }
                }
            }
            
            return resultado;
        }

        /// <summary>
        /// Obtiene un reporte de valores críticos (fuera de rango) para un médico específico.
        /// Lista todos los resultados que están fuera de los valores de referencia.
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <param name="desde">Fecha de inicio del período</param>
        /// <param name="hasta">Fecha de fin del período</param>
        /// <returns>Lista de alertas de valores críticos</returns>
        public List<ReporteAlerta> ObtenerValoresCriticos(int dniMedico, DateTime desde, DateTime hasta)
        {
            var resultado = new List<ReporteAlerta>();
            
            var sql = @"
                SELECT 
                    a.id_analisis,
                    p.nombre + ' ' + p.apellido AS NombrePaciente,
                    m.nombre AS NombreMetrica,
                    am.resultado,
                    m.valor_minimo,
                    m.valor_maximo,
                    a.fecha_creacion
                FROM 
                    analisis a
                INNER JOIN 
                    pacientes p ON a.dni_paciente = p.dni
                INNER JOIN 
                    analisis_metrica am ON a.id_analisis = am.id_analisis
                INNER JOIN 
                    metricas m ON am.id_metrica = m.id_metrica
                WHERE 
                    a.dni_carga = @dniMedico
                    AND a.fecha_creacion BETWEEN @desde AND @hasta
                    AND a.id_estado != 3
                    AND (
                        (m.valor_minimo IS NOT NULL AND am.resultado < m.valor_minimo)
                        OR
                        (m.valor_maximo IS NOT NULL AND am.resultado > m.valor_maximo)
                    )
                ORDER BY 
                    a.fecha_creacion DESC";
            
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@dniMedico", dniMedico);
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        resultado.Add(new ReporteAlerta
                        {
                            IdAnalisis = rd.GetInt32(0),
                            NombrePaciente = rd.GetString(1),
                            NombreMetrica = rd.GetString(2),
                            Resultado = rd.GetDecimal(3),
                            ValorMinimo = rd.IsDBNull(4) ? (decimal?)null : rd.GetDecimal(4),
                            ValorMaximo = rd.IsDBNull(5) ? (decimal?)null : rd.GetDecimal(5),
                            FechaAnalisis = rd.GetDateTime(6)
                        });
                    }
                }
            }
            
            return resultado;
        }

        /// <summary>
        /// Obtiene un reporte de carga de trabajo de un médico específico.
        /// Incluye análisis pendientes y análisis verificados en el mes actual.
        /// </summary>
        /// <param name="dniMedico">DNI del médico</param>
        /// <returns>Información sobre la carga de trabajo del médico</returns>
        public ReporteCargaTrabajo ObtenerCargaTrabajo(int dniMedico)
        {
            var resultado = new ReporteCargaTrabajo();
            
            var sql = @"
                SELECT 
                    SUM(CASE WHEN id_estado = 1 THEN 1 ELSE 0 END) AS Pendientes,
                    SUM(CASE WHEN id_estado = 2 AND MONTH(fecha_firma) = MONTH(GETDATE()) AND YEAR(fecha_firma) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS VerificadosMes
                FROM 
                    analisis
                WHERE 
                    dni_carga = @dniMedico";
            
            using (var cn = DbConexion.CrearConexion())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@dniMedico", dniMedico);
                
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        resultado.AnalisisPendientes = rd.IsDBNull(0) ? 0 : rd.GetInt32(0);
                        resultado.AnalisisVerificadosMes = rd.IsDBNull(1) ? 0 : rd.GetInt32(1);
                    }
                }
            }
            
            return resultado;
        }
    }
}
