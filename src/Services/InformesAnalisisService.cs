using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using SALC.Views.Interfaces;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para gestión de informes de análisis según ERS v2.7
    /// Implementa RF-08: Generar y Enviar Informe y RF-09: Visualizar Historial
    /// </summary>
    public class InformesAnalisisService
    {
        private readonly string _cadenaConexion;

        public InformesAnalisisService()
        {
            _cadenaConexion = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_cadenaConexion))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");
        }

        #region Operaciones de Consulta

        public List<InformeAnalisis> ObtenerTodosLosInformes()
        {
            var informes = new List<InformeAnalisis>();

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
                        var informe = new InformeAnalisis
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

                        informe.Resultados = ObtenerResultadosAnalisis(informe.IdReporte);
                        informes.Add(informe);
                    }
                }
            }

            return informes;
        }

        public List<InformeAnalisis> BuscarInformes(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
                return ObtenerTodosLosInformes();

            var informes = new List<InformeAnalisis>();

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
                            var informe = new InformeAnalisis
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

                            informe.Resultados = ObtenerResultadosAnalisis(informe.IdReporte);
                            informes.Add(informe);
                        }
                    }
                }
            }

            return informes;
        }

        public List<InformeAnalisis> FiltrarInformes(FiltroInforme filtro)
        {
            var informes = new List<InformeAnalisis>();

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
                    comando.Parameters.AddWithValue("@fechaHasta", filtro.FechaHasta.AddDays(1));
                    comando.Parameters.AddWithValue("@tipoAnalisis", filtro.TipoAnalisis ?? "Todos");
                    comando.Parameters.AddWithValue("@estadoPaciente", filtro.EstadoAnalisis ?? "Todos");

                    using (var lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var informe = new InformeAnalisis
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

                            informe.Resultados = ObtenerResultadosAnalisis(informe.IdReporte);
                            informes.Add(informe);
                        }
                    }
                }
            }

            return informes;
        }

        public InformeAnalisis ObtenerDetallesAnalisis(int idAnalisis)
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
                            var informe = new InformeAnalisis
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

                            informe.Resultados = ObtenerResultadosAnalisis(informe.IdReporte);
                            return informe;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Operaciones de Exportación

        public void ExportarAPdf(InformeAnalisis informe)
        {
            Console.WriteLine($"Exportando a PDF: {informe.NombrePaciente} - {informe.FechaAnalisis}");
        }

        public void ExportarACsv(InformeAnalisis informe)
        {
            Console.WriteLine($"Exportando a CSV: {informe.NombrePaciente} - {informe.FechaAnalisis}");
        }

        public void EnviarPorEmail(InformeAnalisis informe, string emailDestino)
        {
            Console.WriteLine($"Enviando informe por email a {emailDestino}: {informe.NombrePaciente}");
        }

        #endregion

        #region Métodos Privados

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

        public bool PuedeGenerarInforme(int idAnalisis, int dniUsuario, string rolUsuario)
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
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al verificar permisos de informe: {ex.Message}", ex);
            }

            return false;
        }

        #endregion
    }
}
