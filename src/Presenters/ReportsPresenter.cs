// Presenters/ReportsPresenter.cs
using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Views.Interfaces;
using SALC.Views;
using SALC.Models;
using SALC.Services;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la generación de informes de análisis
    /// Maneja la lógica de presentación entre la vista y los servicios de datos
    /// </summary>
    [Obsolete("Use PresentadorReportes con IVistaReportes")]
    public class ReportsPresenter
    {
        private readonly IVistaReportes _view;
        private readonly ServicioReportesAnalisis _reportsService;

        public ReportsPresenter(IVistaReportes view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _reportsService = new ServicioReportesAnalisis();

            SubscribeToViewEvents();
            InitializeView();
        }

        private void SubscribeToViewEvents()
        {
            _view.CargarReportes += OnLoadReports;
            _view.BuscarReportes += OnSearchReports;
            _view.FiltrarReportes += OnFilterReports;
            _view.VerReporteCompleto += OnViewFullReport;
            _view.ExportarReportePdf += OnExportPdfReport;
            _view.ExportarReporteCsv += OnExportCsvReport;
        }

        private void InitializeView()
        {
            // Cargar datos iniciales
            LoadReportsData();
        }

        private void OnLoadReports(object sender, EventArgs e)
        {
            LoadReportsData();
        }

        private void OnSearchReports(object sender, string searchText)
        {
            try
            {
                var reports = _reportsService.BuscarReportes(searchText);
                _view.CargarDatosReportes(reports);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al buscar reportes: {ex.Message}");
            }
        }

        private void OnFilterReports(object sender, FiltroReporte filter)
        {
            try
            {
                var reports = _reportsService.FiltrarReportes(filter);
                _view.CargarDatosReportes(reports);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al filtrar reportes: {ex.Message}");
            }
        }

        private void OnViewFullReport(object sender, int reportIndex)
        {
            try
            {
                if (reportIndex >= 0 && reportIndex < _view.Reportes.Count)
                {
                    var report = _view.Reportes[reportIndex];
                    var detailedReport = _reportsService.ObtenerDetallesAnalisis(report.IdReporte);

                    if (detailedReport != null)
                    {
                        _view.MostrarMensaje("Detalles", $"Paciente: {detailedReport.NombrePaciente}\nAnálisis: {detailedReport.TipoAnalisis} - {detailedReport.FechaAnalisis:dd/MM/yyyy}");
                    }
                    else
                    {
                        _view.MostrarError("No se pudieron cargar los detalles del análisis");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al ver reporte: {ex.Message}");
            }
        }

        private void OnExportPdfReport(object sender, int reportIndex)
        {
            try
            {
                if (reportIndex >= 0 && reportIndex < _view.Reportes.Count)
                {
                    var report = _view.Reportes[reportIndex];
                    _reportsService.ExportarAPdf(report);
                    _view.MostrarMensaje("Exportación Exitosa", $"Reporte de {report.NombrePaciente} exportado a PDF correctamente");
                }
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al exportar a PDF: {ex.Message}");
            }
        }

        private void OnExportCsvReport(object sender, int reportIndex)
        {
            try
            {
                if (reportIndex >= 0 && reportIndex < _view.Reportes.Count)
                {
                    var report = _view.Reportes[reportIndex];
                    _reportsService.ExportarACsv(report);
                    _view.MostrarMensaje("Exportación Exitosa", $"Reporte de {report.NombrePaciente} exportado a CSV correctamente");
                }
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al exportar a CSV: {ex.Message}");
            }
        }

        private void LoadReportsData()
        {
            try
            {
                var reports = _reportsService.ObtenerTodosLosReportes();
                _view.CargarDatosReportes(reports);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar reportes: {ex.Message}");
            }
        }
    }
}
