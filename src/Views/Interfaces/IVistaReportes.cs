using System;
using System.Collections.Generic;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interfaz para la vista de reportes de an�lisis seg�n ERS v2.7
    /// Implementa el patr�n MVP (Model-View-Presenter) para RF-08: Generar y Enviar Informe
    /// </summary>
    public interface IVistaReportes
    {
        #region Eventos de la Vista (en espa�ol)
        event EventHandler GeneracionSolicitada;
        event EventHandler ExportacionPdfSolicitada;
        event EventHandler CierreSolicitado;
        event EventHandler CargarReportes;
        event EventHandler<string> BuscarReportes;
        event EventHandler<FiltroReporte> FiltrarReportes;
        event EventHandler<int> VerReporteCompleto;
        event EventHandler<int> ExportarReportePdf;
        event EventHandler<int> ExportarReporteCsv;
        #endregion

        #region Propiedades de la Vista (en espa�ol)
        List<ReporteAnalisis> Reportes { get; set; }
        ReporteAnalisis ReporteSeleccionado { get; set; }
        string RolUsuarioActual { get; set; }
        #endregion

        #region M�todos de la Vista (en espa�ol)
        void CargarDatosReportes(List<ReporteAnalisis> reportes);
        void MostrarMensaje(string titulo, string mensaje);
        void MostrarError(string mensaje);
        #endregion
    }

    #region Clases de apoyo para la interfaz (en espa�ol)
    /// <summary>
    /// Representa un reporte de an�lisis seg�n la estructura ERS v2.7
    /// </summary>
    public class ReporteAnalisis
    {
        public int IdReporte { get; set; }
        public string NombrePaciente { get; set; }
        public string IdPaciente { get; set; }
        public string TipoAnalisis { get; set; }
        public DateTime FechaAnalisis { get; set; }
        public string Estado { get; set; }
        public string NombreMedico { get; set; }
        public string Observaciones { get; set; }
        public List<ResultadoAnalisis> Resultados { get; set; } = new List<ResultadoAnalisis>();
        
        // Propiedades adicionales
        public string TelefonoPaciente { get; set; }
        public string ObraSocial { get; set; }
        public string Prioridad { get; set; }
    }

    /// <summary>
    /// Representa el resultado de una m�trica espec�fica de un an�lisis
    /// </summary>
    public class ResultadoAnalisis
    {
        public string Parametro { get; set; }
        public string Valor { get; set; }
        public string Unidad { get; set; }
        public string RangoReferencia { get; set; }
    }

    /// <summary>
    /// Filtro para b�squedas en reportes de an�lisis
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