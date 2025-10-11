using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using SALC.Models;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gestión de reportes de análisis según ERS v2.7
    /// Implementa RF-08: Generar y Enviar Informe y RF-09: Visualizar Historial
    /// </summary>
    public class ServicioReportesAnalisis
    {
        private readonly string _cadenaConexion;

        public ServicioReportesAnalisis()
        {
            _cadenaConexion = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_cadenaConexion))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");
        }

        #region Operaciones de Consulta

        /// <summary>
        /// Obtiene todos los reportes de análisis
        /// </summary>
        public List<ReporteAnalisis> ObtenerTodosLosReportes()
        {
            var reportes = new List<ReporteAnalisis>();

            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                
                var consulta = @"
                    SELECT 
                        a.id_analisis,
                        p.dni,
                        p.nombre + ' ' + p.apellido as NombrePaciente,
                        ta.descripcion as TipoAnalisis,
                        a.fecha_creacion as FechaAnalisis,
                        ea.descripcion as Estado,
                        COALESCE(uc.nombre + ' ' + uc.apellido, 'No asignado') as NombreMedico,
                        a.observaciones
                    FROM analisis a
                    INNER JOIN pacientes p ON a.dni_paciente = p.dni
                    INNER JOIN tipos_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                    INNER JOIN estados_analisis ea ON a.id_estado = ea.id_estado
                    LEFT JOIN usuarios uc ON a.dni_carga = uc.dni
                    ORDER BY a.fecha_creacion DESC";

                using (var comando = new SqlCommand(consulta, conexion))
                using (var lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        var reporte = new ReporteAnalisis
                        {
                            IdReporte = lector.GetInt32("id_analisis"),
                            IdPaciente = lector.GetInt32("dni").ToString(),
                            NombrePaciente = lector.GetString("NombrePaciente"),
                            TipoAnalisis = lector.GetString("TipoAnalisis"),
                            FechaAnalisis = lector.GetDateTime("FechaAnalisis"),
                            Estado = lector.GetString("Estado"),
                            NombreMedico = lector.GetString("NombreMedico"),
                            Observaciones = lector.IsDBNull("observaciones") ? string.Empty : lector.GetString("observaciones")
                        };

                        // Cargar resultados del análisis
                        reporte.Resultados = ObtenerResultadosAnalisis(reporte.IdReporte);
                        
                        reportes.Add(reporte);
                    }
                }
            }

            return reportes;
        }

        /// <summary>
        /// Busca reportes por texto
        /// </summary>
        public List<ReporteAnalisis> BuscarReportes(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
                return ObtenerTodosLosReportes();

            var reportes = new List<ReporteAnalisis>();

            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                
                var consulta = @"
                    SELECT 
                        a.id_analisis,
                        p.dni,
                        p.nombre + ' ' + p.apellido as NombrePaciente,
                        ta.descripcion as TipoAnalisis,
                        a.fecha_creacion as FechaAnalisis,
                        ea.descripcion as Estado,
                        COALESCE(uc.nombre + ' ' + uc.apellido, 'No asignado') as NombreMedico,
                        a.observaciones
                    FROM analisis a
                    INNER JOIN pacientes p ON a.dni_paciente = p.dni
                    INNER JOIN tipos_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                    INNER JOIN estados_analisis ea ON a.id_estado = ea.id_estado
                    LEFT JOIN usuarios uc ON a.dni_carga = uc.dni
                    WHERE p.nombre LIKE @busqueda OR p.apellido LIKE @busqueda OR CAST(p.dni AS NVARCHAR) LIKE @busqueda
                    ORDER BY a.fecha_creacion DESC";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@busqueda", $"%{textoBusqueda}%");
                    
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var reporte = new ReporteAnalisis
                            {
                                IdReporte = lector.GetInt32("id_analisis"),
                                IdPaciente = lector.GetInt32("dni").ToString(),
                                NombrePaciente = lector.GetString("NombrePaciente"),
                                TipoAnalisis = lector.GetString("TipoAnalisis"),
                                FechaAnalisis = lector.GetDateTime("FechaAnalisis"),
                                Estado = lector.GetString("Estado"),
                                NombreMedico = lector.GetString("NombreMedico"),
                                Observaciones = lector.IsDBNull("observaciones") ? string.Empty : lector.GetString("observaciones")
                            };

                            reporte.Resultados = ObtenerResultadosAnalisis(reporte.IdReporte);
                            reportes.Add(reporte);
                        }
                    }
                }
            }

            return reportes;
        }

        /// <summary>
        /// Filtra reportes por criterios específicos
        /// </summary>
        public List<ReporteAnalisis> FiltrarReportes(FiltroReporte filtro)
        {
            var reportes = new List<ReporteAnalisis>();

            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                
                var consulta = @"
                    SELECT 
                        a.id_analisis,
                        p.dni,
                        p.nombre + ' ' + p.apellido as NombrePaciente,
                        ta.descripcion as TipoAnalisis,
                        a.fecha_creacion as FechaAnalisis,
                        ea.descripcion as Estado,
                        COALESCE(uc.nombre + ' ' + uc.apellido, 'No asignado') as NombreMedico,
                        a.observaciones
                    FROM analisis a
                    INNER JOIN pacientes p ON a.dni_paciente = p.dni
                    INNER JOIN tipos_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                    INNER JOIN estados_analisis ea ON a.id_estado = ea.id_estado
                    LEFT JOIN usuarios uc ON a.dni_carga = uc.dni
                    WHERE a.fecha_creacion BETWEEN @fechaDesde AND @fechaHasta
                    AND (@tipoAnalisis = 'Todos' OR ta.descripcion = @tipoAnalisis)
                    AND (@estadoPaciente = 'Todos' OR ea.descripcion = @estadoPaciente)
                    ORDER BY a.fecha_creacion DESC";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaDesde", filtro.FechaDesde);
                    comando.Parameters.AddWithValue("@fechaHasta", filtro.FechaHasta.AddDays(1)); // Incluir el día completo
                    comando.Parameters.AddWithValue("@tipoAnalisis", filtro.TipoAnalisis ?? "Todos");
                    comando.Parameters.AddWithValue("@estadoPaciente", filtro.EstadoAnalisis ?? "Todos");
                    
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var reporte = new ReporteAnalisis
                            {
                                IdReporte = lector.GetInt32("id_analisis"),
                                IdPaciente = lector.GetInt32("dni").ToString(),
                                NombrePaciente = lector.GetString("NombrePaciente"),
                                TipoAnalisis = lector.GetString("TipoAnalisis"),
                                FechaAnalisis = lector.GetDateTime("FechaAnalisis"),
                                Estado = lector.GetString("Estado"),
                                NombreMedico = lector.GetString("NombreMedico"),
                                Observaciones = lector.IsDBNull("observaciones") ? string.Empty : lector.GetString("observaciones")
                            };

                            reporte.Resultados = ObtenerResultadosAnalisis(reporte.IdReporte);
                            reportes.Add(reporte);
                        }
                    }
                }
            }

            return reportes;
        }

        /// <summary>
        /// Obtiene los detalles completos de un análisis específico
        /// </summary>
        public ReporteAnalisis ObtenerDetallesAnalisis(int idAnalisis)
        {
            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                
                var consulta = @"
                    SELECT 
                        a.id_analisis,
                        p.dni,
                        p.nombre + ' ' + p.apellido as NombrePaciente,
                        p.telefono as TelefonoPaciente,
                        os.nombre as ObraSocial,
                        ta.descripcion as TipoAnalisis,
                        a.fecha_creacion as FechaAnalisis,
                        ea.descripcion as Estado,
                        COALESCE(uc.nombre + ' ' + uc.apellido, 'No asignado') as NombreMedico,
                        a.observaciones as Observaciones
                    FROM analisis a
                    INNER JOIN pacientes p ON a.dni_paciente = p.dni
                    INNER JOIN tipos_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                    INNER JOIN estados_analisis ea ON a.id_estado = ea.id_estado
                    LEFT JOIN usuarios uc ON a.dni_carga = uc.dni
                    LEFT JOIN obras_sociales os ON p.id_obra_social = os.id_obra_social
                    WHERE a.id_analisis = @idAnalisis";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@idAnalisis", idAnalisis);
                    
                    using (var lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            var reporte = new ReporteAnalisis
                            {
                                IdReporte = lector.GetInt32("id_analisis"),
                                IdPaciente = lector.GetInt32("dni").ToString(),
                                NombrePaciente = lector.GetString("NombrePaciente"),
                                TelefonoPaciente = lector.IsDBNull("TelefonoPaciente") ? string.Empty : lector.GetString("TelefonoPaciente"),
                                ObraSocial = lector.IsDBNull("ObraSocial") ? "Sin obra social" : lector.GetString("ObraSocial"),
                                TipoAnalisis = lector.GetString("TipoAnalisis"),
                                FechaAnalisis = lector.GetDateTime("FechaAnalisis"),
                                Estado = lector.GetString("Estado"),
                                NombreMedico = lector.GetString("NombreMedico"),
                                Observaciones = lector.IsDBNull("Observaciones") ? string.Empty : lector.GetString("Observaciones")
                            };

                            reporte.Resultados = ObtenerResultadosAnalisis(reporte.IdReporte);
                            return reporte;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Operaciones de Exportación

        /// <summary>
        /// Exporta un reporte a formato PDF (RF-08)
        /// </summary>
        public void ExportarAPdf(ReporteAnalisis reporte)
        {
            // Implementación básica de exportación a PDF
            // En una implementación real, se usaría una librería como iTextSharp o QuestPDF
            Console.WriteLine($"Exportando a PDF: {reporte.NombrePaciente} - {reporte.FechaAnalisis}");
            // Lógica real de exportación a PDF iría aquí
        }

        /// <summary>
        /// Exporta un reporte a formato CSV
        /// </summary>
        public void ExportarACsv(ReporteAnalisis reporte)
        {
            // Implementación básica de exportación a CSV
            Console.WriteLine($"Exportando a CSV: {reporte.NombrePaciente} - {reporte.FechaAnalisis}");
            // Lógica real de exportación a CSV iría aquí
        }

        /// <summary>
        /// Envía reporte por email (RF-08)
        /// </summary>
        public void EnviarPorEmail(ReporteAnalisis reporte, string emailDestino)
        {
            // Implementación básica de envío por email
            Console.WriteLine($"Enviando reporte por email a {emailDestino}: {reporte.NombrePaciente}");
            // Lógica real de envío por email iría aquí
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Obtiene los resultados de métricas de un análisis específico
        /// </summary>
        private List<ResultadoAnalisis> ObtenerResultadosAnalisis(int idAnalisis)
        {
            var resultados = new List<ResultadoAnalisis>();

            using (var conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                
                var consulta = @"
                    SELECT 
                        m.nombre as Parametro,
                        am.resultado as Valor,
                        m.unidad_medida as Unidad,
                        COALESCE(CAST(m.valor_minimo AS VARCHAR), 'N/A') + ' - ' + COALESCE(CAST(m.valor_maximo AS VARCHAR), 'N/A') as RangoReferencia
                    FROM analisis_metrica am
                    INNER JOIN metricas m ON am.id_metrica = m.id_metrica
                    WHERE am.id_analisis = @idAnalisis
                    ORDER BY m.nombre";

                using (var comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@idAnalisis", idAnalisis);
                    
                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var resultado = new ResultadoAnalisis
                            {
                                Parametro = lector.GetString("Parametro"),
                                Valor = lector.GetDecimal("Valor").ToString(),
                                Unidad = lector.GetString("Unidad"),
                                RangoReferencia = lector.GetString("RangoReferencia")
                            };

                            resultados.Add(resultado);
                        }
                    }
                }
            }

            return resultados;
        }

        #endregion

        #region Métodos de Validación

        /// <summary>
        /// Verifica si un usuario puede generar reportes para un análisis específico (RF-08, RF-09)
        /// </summary>
        public bool PuedeGenerarReporte(int idAnalisis, int dniUsuario, string rolUsuario)
        {
            try
            {
                using (var conexion = new SqlConnection(_cadenaConexion))
                {
                    conexion.Open();
                    
                    var consulta = @"
                        SELECT a.dni_carga, ea.descripcion as estado
                        FROM analisis a
                        INNER JOIN estados_analisis ea ON a.id_estado = ea.id_estado
                        WHERE a.id_analisis = @idAnalisis";

                    using (var comando = new SqlCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@idAnalisis", idAnalisis);
                        
                        using (var lector = comando.ExecuteReader())
                        {
                            if (lector.Read())
                            {
                                int dniMedicoCarga = lector.GetInt32("dni_carga");
                                string estadoAnalisis = lector.GetString("estado");

                                // Reglas según ERS v2.7:
                                // - Médico: puede generar reportes de análisis que él cargó
                                // - Asistente: solo puede generar reportes de análisis verificados
                                if (rolUsuario == "Médico")
                                {
                                    return dniMedicoCarga == dniUsuario;
                                }
                                else if (rolUsuario == "Asistente")
                                {
                                    return estadoAnalisis == "Verificado";
                                }
                                else if (rolUsuario == "Administrador")
                                {
                                    return true; // Administrador puede generar cualquier reporte
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al verificar permisos de reporte: {ex.Message}", ex);
            }

            return false;
        }

        #endregion
    }

    #region Modelos de Soporte

    /// <summary>
    /// Representa un reporte de análisis según ERS v2.7
    /// </summary>
    public class ReporteAnalisis
    {
        public int IdReporte { get; set; }
        public string IdPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string TelefonoPaciente { get; set; }
        public string ObraSocial { get; set; }
        public string TipoAnalisis { get; set; }
        public DateTime FechaAnalisis { get; set; }
        public string Estado { get; set; }
        public string NombreMedico { get; set; }
        public string Observaciones { get; set; }
        public List<ResultadoAnalisis> Resultados { get; set; } = new List<ResultadoAnalisis>();
    }

    /// <summary>
    /// Representa el resultado de una métrica en un análisis
    /// </summary>
    public class ResultadoAnalisis
    {
        public string Parametro { get; set; }
        public string Valor { get; set; }
        public string Unidad { get; set; }
        public string RangoReferencia { get; set; }
    }

    /// <summary>
    /// Filtro para búsqueda de reportes
    /// </summary>
    public class FiltroReporte
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string TipoAnalisis { get; set; }
        public string EstadoAnalisis { get; set; }
    }

    #endregion
}