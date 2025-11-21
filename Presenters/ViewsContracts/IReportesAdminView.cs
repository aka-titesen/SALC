using System;

namespace SALC.Presenters.ViewsContracts
{
    /// <summary>
    /// Interfaz de contrato para la vista de reportes del administrador.
    /// Define las propiedades y eventos para generar y visualizar reportes gerenciales.
    /// </summary>
    public interface IReportesAdminView
    {
        /// <summary>
        /// Obtiene la fecha de inicio del período del reporte
        /// </summary>
        DateTime FechaDesde { get; }

        /// <summary>
        /// Obtiene la fecha de fin del período del reporte
        /// </summary>
        DateTime FechaHasta { get; }

        /// <summary>
        /// Evento que se dispara cuando se solicita generar el reporte de productividad
        /// </summary>
        event EventHandler GenerarReporteProductividadClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita generar el reporte de facturación
        /// </summary>
        event EventHandler GenerarReporteFacturacionClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita generar el reporte de demanda
        /// </summary>
        event EventHandler GenerarReporteDemandaClick;

        /// <summary>
        /// Muestra el reporte de productividad de médicos
        /// </summary>
        /// <param name="datos">Datos del reporte a mostrar</param>
        void MostrarReporteProductividad(System.Collections.IEnumerable datos);

        /// <summary>
        /// Muestra el reporte de facturación por obra social
        /// </summary>
        /// <param name="datos">Datos del reporte a mostrar</param>
        void MostrarReporteFacturacion(System.Collections.IEnumerable datos);

        /// <summary>
        /// Muestra el reporte de demanda de tipos de análisis
        /// </summary>
        /// <param name="datos">Datos del reporte a mostrar</param>
        void MostrarReporteDemanda(System.Collections.IEnumerable datos);

        /// <summary>
        /// Muestra un mensaje al usuario
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        /// <param name="esError">Indica si es un mensaje de error</param>
        void MostrarMensaje(string mensaje, bool esError = false);
    }
}
