using System;

namespace SALC.Presenters.ViewsContracts
{
    public interface IReportesMedicoView
    {
        // Propiedades para filtros
        DateTime FechaDesde { get; }
        DateTime FechaHasta { get; }

        // Eventos
        event EventHandler GenerarReporteAlertasClick;
        event EventHandler GenerarReporteCargaTrabajoClick;

        // Métodos para mostrar reportes
        void MostrarReporteAlertas(System.Collections.IEnumerable datos);
        void MostrarReporteCargaTrabajo(BLL.ReporteCargaTrabajo datos);

        // Mensajes
        void MostrarMensaje(string mensaje, bool esError = false);
    }
}
