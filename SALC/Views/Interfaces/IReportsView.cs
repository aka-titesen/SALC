using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    public interface IReportsView
    {
        // Eventos principales
        event EventHandler GenerateRequested;
        event EventHandler ExportPdfRequested;
        event EventHandler CloseRequested;
        
        // Eventos personalizados para análisis de reportes
        event EventHandler LoadReports;
        event EventHandler<string> SearchReports;
        event EventHandler<ReportFilter> FilterReports;
        event EventHandler<int> ViewFullReport;
        event EventHandler<int> ExportPdfReport;
        event EventHandler<int> ExportCsvReport;
        
        // Propiedades
        List<AnalisisReport> Reports { get; set; }
        AnalisisReport SelectedReport { get; set; }
        string CurrentUserRole { get; set; }
        
        // Métodos
        void LoadReportsData(List<AnalisisReport> reports);
        void ShowMessage(string message, string title, System.Windows.Forms.MessageBoxIcon icon);
    }

    #region Clases de apoyo para la interfaz
    public class AnalisisReport
    {
        public int ReportId { get; set; }
        public string PatientName { get; set; }
        public string PatientId { get; set; }
        public string AnalysisType { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string Status { get; set; }
        public string DoctorName { get; set; }
        public string Observations { get; set; }
        public List<AnalysisResult> Results { get; set; } = new List<AnalysisResult>();
    }

    public class AnalysisResult
    {
        public string Parameter { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string ReferenceRange { get; set; }
    }

    public class ReportFilter
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string AnalysisType { get; set; }
        public string PatientStatus { get; set; }
    }
    #endregion
}
