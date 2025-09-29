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
    public class ReportsPresenter
    {
        private readonly IReportsView _view;
        private readonly AnalysisReportsService _reportsService;

        public ReportsPresenter(IReportsView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _reportsService = new AnalysisReportsService();
            
            SubscribeToViewEvents();
            InitializeView();
        }

        private void SubscribeToViewEvents()
        {
            _view.LoadReports += OnLoadReports;
            _view.SearchReports += OnSearchReports;
            _view.FilterReports += OnFilterReports;
            _view.ViewFullReport += OnViewFullReport;
            _view.ExportPdfReport += OnExportPdfReport;
            _view.ExportCsvReport += OnExportCsvReport;
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
                var reports = _reportsService.SearchReports(searchText);
                _view.LoadReportsData(reports);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error en búsqueda", $"Error al buscar reportes: {ex.Message}", System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnFilterReports(object sender, ReportFilter filter)
        {
            try
            {
                var reports = _reportsService.FilterReports(filter);
                _view.LoadReportsData(reports);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error en filtros", $"Error al filtrar reportes: {ex.Message}", System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnViewFullReport(object sender, int reportIndex)
        {
            try
            {
                if (reportIndex >= 0 && reportIndex < _view.Reports.Count)
                {
                    var report = _view.Reports[reportIndex];
                    
                    // Obtener detalles completos del análisis
                    var detailedReport = _reportsService.GetAnalysisDetails(report.ReportId);
                    
                    if (detailedReport != null)
                    {
                        // Abrir formulario de detalle
                        var detailForm = new AnalysisDetailForm(detailedReport, detailedReport.Results);
                        detailForm.ShowDialog();
                    }
                    else
                    {
                        _view.ShowMessage("Error", "No se pudieron cargar los detalles del análisis", System.Windows.Forms.MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al ver reporte: {ex.Message}", System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnExportPdfReport(object sender, int reportIndex)
        {
            try
            {
                if (reportIndex >= 0 && reportIndex < _view.Reports.Count)
                {
                    var report = _view.Reports[reportIndex];
                    _reportsService.ExportToPdf(report);
                    _view.ShowMessage("Exportación Exitosa", $"Reporte de {report.PatientName} exportado a PDF correctamente", System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error en Exportación", $"Error al exportar a PDF: {ex.Message}", System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnExportCsvReport(object sender, int reportIndex)
        {
            try
            {
                if (reportIndex >= 0 && reportIndex < _view.Reports.Count)
                {
                    var report = _view.Reports[reportIndex];
                    _reportsService.ExportToCsv(report);
                    _view.ShowMessage("Exportación Exitosa", $"Reporte de {report.PatientName} exportado a CSV correctamente", System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error en Exportación", $"Error al exportar a CSV: {ex.Message}", System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void LoadReportsData()
        {
            try
            {
                var reports = _reportsService.GetAllReports();
                _view.LoadReportsData(reports);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al cargar reportes: {ex.Message}", System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}