using System;

namespace SALC.Presenters.ViewsContracts
{
    public interface IReportesAdminView
    {
        // Propiedades para filtros comunes
        DateTime FechaDesde { get; }
        DateTime FechaHasta { get; }

        // Eventos
        event EventHandler GenerarReporteProductividadClick;
        event EventHandler GenerarReporteFacturacionClick;
        event EventHandler GenerarReporteDemandaClick;

        // Métodos para mostrar reportes
        void MostrarReporteProductividad(System.Collections.IEnumerable datos);
        void MostrarReporteFacturacion(System.Collections.IEnumerable datos);
        void MostrarReporteDemanda(System.Collections.IEnumerable datos);

        // Mensajes
        void MostrarMensaje(string mensaje, bool esError = false);
    }
}
