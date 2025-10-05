// Services/AnalysisReportsService.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using SALC.Views.Interfaces;
using SALC.Models;

namespace SALC.Services
{
    public class AnalysisReportsService
    {
        private readonly string _connectionString;

        public AnalysisReportsService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");
        }

        public List<AnalisisReport> GetAllReports()
        {
            var reports = new List<AnalisisReport>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var query = @"
                    SELECT 
                        a.id_analisis,
                        p.dni,
                        p.nombre + ' ' + p.apellido as PatientName,
                        ta.descripcion as AnalysisType,
                        a.fecha_creacion as AnalysisDate,
                        e.tipo_estado as Status,
                        d.nombre + ' ' + d.apellido as DoctorName,
                        a.observaciones
                    FROM analisis a
                    INNER JOIN paciente p ON a.doc_paciente = p.dni
                    INNER JOIN tipo_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                    INNER JOIN estado e ON a.id_estado = e.id_estado
                    INNER JOIN doctor d ON a.doctor_encargado = d.dni
                    ORDER BY a.fecha_creacion DESC";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var report = new AnalisisReport
                        {
                            ReportId = reader.GetInt32(0),
                            PatientId = reader.GetInt32(1).ToString(),
                            PatientName = reader.GetString(2),
                            AnalysisType = reader.GetString(3),
                            AnalysisDate = reader.GetDateTime(4),
                            Status = reader.GetString(5),
                            DoctorName = reader.GetString(6),
                            Observations = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                        };

                        // Cargar resultados del análisis
                        report.Results = GetAnalysisResults(report.ReportId);
                        
                        reports.Add(report);
                    }
                }
            }

            return reports;
        }

        public List<AnalisisReport> SearchReports(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllReports();

            var reports = new List<AnalisisReport>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var query = @"
                    SELECT 
                        a.id_analisis,
                        p.dni,
                        p.nombre + ' ' + p.apellido as PatientName,
                        ta.descripcion as AnalysisType,
                        a.fecha_creacion as AnalysisDate,
                        e.tipo_estado as Status,
                        d.nombre + ' ' + d.apellido as DoctorName,
                        a.observaciones
                    FROM analisis a
                    INNER JOIN paciente p ON a.doc_paciente = p.dni
                    INNER JOIN tipo_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                    INNER JOIN estado e ON a.id_estado = e.id_estado
                    INNER JOIN doctor d ON a.doctor_encargado = d.dni
                    WHERE p.nombre LIKE @search OR p.apellido LIKE @search OR CAST(p.dni AS NVARCHAR) LIKE @search
                    ORDER BY a.fecha_creacion DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@search", $"%{searchText}%");
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var report = new AnalisisReport
                            {
                                ReportId = reader.GetInt32(0),
                                PatientId = reader.GetInt32(1).ToString(),
                                PatientName = reader.GetString(2),
                                AnalysisType = reader.GetString(3),
                                AnalysisDate = reader.GetDateTime(4),
                                Status = reader.GetString(5),
                                DoctorName = reader.GetString(6),
                                Observations = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                            };

                            report.Results = GetAnalysisResults(report.ReportId);
                            reports.Add(report);
                        }
                    }
                }
            }

            return reports;
        }

        public List<AnalisisReport> FilterReports(ReportFilter filter)
        {
            var reports = new List<AnalisisReport>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var query = @"
                    SELECT 
                        a.id_analisis,
                        p.dni,
                        p.nombre + ' ' + p.apellido as PatientName,
                        ta.descripcion as AnalysisType,
                        a.fecha_creacion as AnalysisDate,
                        e.tipo_estado as Status,
                        d.nombre + ' ' + d.apellido as DoctorName,
                        a.observaciones
                    FROM analisis a
                    INNER JOIN paciente p ON a.doc_paciente = p.dni
                    INNER JOIN tipo_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                    INNER JOIN estado e ON a.id_estado = e.id_estado
                    INNER JOIN doctor d ON a.doctor_encargado = d.dni
                    WHERE a.fecha_creacion BETWEEN @fromDate AND @toDate
                    AND (@analysisType = 'Todos' OR ta.descripcion = @analysisType)
                    AND (@patientStatus = 'Todos' OR e.tipo_estado = @patientStatus)
                    ORDER BY a.fecha_creacion DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fromDate", filter.FromDate);
                    command.Parameters.AddWithValue("@toDate", filter.ToDate.AddDays(1)); // Incluir el día completo
                    command.Parameters.AddWithValue("@analysisType", filter.AnalysisType ?? "Todos");
                    command.Parameters.AddWithValue("@patientStatus", filter.PatientStatus ?? "Todos");
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var report = new AnalisisReport
                            {
                                ReportId = reader.GetInt32(0),
                                PatientId = reader.GetInt32(1).ToString(),
                                PatientName = reader.GetString(2),
                                AnalysisType = reader.GetString(3),
                                AnalysisDate = reader.GetDateTime(4),
                                Status = reader.GetString(5),
                                DoctorName = reader.GetString(6),
                                Observations = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                            };

                            report.Results = GetAnalysisResults(report.ReportId);
                            reports.Add(report);
                        }
                    }
                }
            }

            return reports;
        }

        private List<AnalysisResult> GetAnalysisResults(int analysisId)
        {
            var results = new List<AnalysisResult>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var query = @"
                    SELECT 
                        m.nombre as Parameter,
                        am.valor as Value,
                        m.unidad as Unit,
                        ISNULL(CAST(m.valor_minimo AS VARCHAR), 'N/A') + ' - ' + ISNULL(CAST(m.valor_maximo AS VARCHAR), 'N/A') as ReferenceRange
                    FROM analisis_metrica am
                    INNER JOIN metrica m ON am.id_metrica = m.id_metrica
                    WHERE am.id_analisis = @analysisId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@analysisId", analysisId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var result = new AnalysisResult
                            {
                                Parameter = reader.GetString(0),
                                Value = reader.GetDecimal(1).ToString(),
                                Unit = reader.GetString(2),
                                ReferenceRange = $"{reader.GetString(3)} - {reader.GetString(4)}"
                            };

                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }

        public void ExportToPdf(AnalisisReport report)
        {
            // Implementación básica de exportación a PDF
            // En una implementación real, se usaría una librería como iTextSharp o QuestPDF
            Console.WriteLine($"Exportando a PDF: {report.PatientName} - {report.AnalysisDate}");
            // Lógica real de exportación a PDF iría aquí
        }

        public void ExportToCsv(AnalisisReport report)
        {
            // Implementación básica de exportación a CSV
            Console.WriteLine($"Exportando a CSV: {report.PatientName} - {report.AnalysisDate}");
            // Lógica real de exportación a CSV iría aquí
        }

        public AnalisisReport GetAnalysisDetails(int analysisId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                var query = @"
                    SELECT 
                        a.id_analisis,
                        p.dni,
                        p.nombre + ' ' + p.apellido as PatientName,
                        p.telefono as PatientPhone,
                        os.nombre as Insurance,
                        ta.descripcion as AnalysisType,
                        a.fecha_creacion as AnalysisDate,
                        e.tipo_estado as Status,
                        d.nombre + ' ' + d.apellido as DoctorName,
                        a.prioridad as Priority,
                        a.observaciones as Observations
                    FROM analisis a
                    INNER JOIN paciente p ON a.doc_paciente = p.dni
                    INNER JOIN tipo_analisis ta ON a.id_tipo_analisis = ta.id_tipo_analisis
                    INNER JOIN estado e ON a.id_estado = e.id_estado
                    INNER JOIN doctor d ON a.doctor_encargado = d.dni
                    LEFT JOIN obra_social os ON p.id_obra_social = os.id_obra_social
                    WHERE a.id_analisis = @analysisId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@analysisId", analysisId);
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var report = new AnalisisReport
                            {
                                ReportId = reader.GetInt32(0),
                                PatientId = reader.GetInt32(1).ToString(),
                                PatientName = reader.GetString(2),
                                PatientPhone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                Insurance = reader.IsDBNull(4) ? "Sin obra social" : reader.GetString(4),
                                AnalysisType = reader.GetString(5),
                                AnalysisDate = reader.GetDateTime(6),
                                Status = reader.GetString(7),
                                DoctorName = reader.GetString(8),
                                Priority = reader.IsDBNull(9) ? "Normal" : reader.GetString(9),
                                Observations = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)
                            };

                            report.Results = GetAnalysisResults(report.ReportId);
                            return report;
                        }
                    }
                }
            }

            return null;
        }
    }
}