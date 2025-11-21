using System;

namespace SALC.Presenters.ViewsContracts
{
    /// <summary>
    /// Interfaz de contrato para la vista de reportes del médico.
    /// Define las propiedades y eventos para generar y visualizar reportes médicos.
    /// </summary>
    public interface IReportesMedicoView
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
        /// Evento que se dispara cuando se solicita generar el reporte de alertas
        /// </summary>
        event EventHandler GenerarReporteAlertasClick;

        /// <summary>
        /// Evento que se dispara cuando se solicita generar el reporte de carga de trabajo
        /// </summary>
        event EventHandler GenerarReporteCargaTrabajoClick;

        /// <summary>
        /// Muestra el reporte de alertas de valores críticos
        /// </summary>
        /// <param name="datos">Datos del reporte a mostrar</param>
        void MostrarReporteAlertas(System.Collections.IEnumerable datos);

        /// <summary>
        /// Muestra el reporte de carga de trabajo del médico
        /// </summary>
        /// <param name="datos">Datos de carga de trabajo</param>
        void MostrarReporteCargaTrabajo(BLL.ReporteCargaTrabajo datos);

        /// <summary>
        /// Muestra un mensaje al usuario
        /// </summary>
        /// <param name="mensaje">Mensaje a mostrar</param>
        /// <param name="esError">Indica si es un mensaje de error</param>
        void MostrarMensaje(string mensaje, bool esError = false);
    }
}
