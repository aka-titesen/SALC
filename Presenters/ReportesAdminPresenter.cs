using System;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la gestión de reportes del panel de administrador.
    /// Coordina la vista de reportes con los servicios de negocio para generar
    /// reportes de productividad, facturación y demanda.
    /// </summary>
    public class ReportesAdminPresenter
    {
        private readonly IReportesAdminView _view;
        private readonly IReportesService _reportesService = new ReportesService();

        /// <summary>
        /// Constructor del presenter
        /// </summary>
        /// <param name="view">Vista de reportes del administrador</param>
        public ReportesAdminPresenter(IReportesAdminView view)
        {
            _view = view;
            _view.GenerarReporteProductividadClick += (s, e) => OnGenerarReporteProductividad();
            _view.GenerarReporteFacturacionClick += (s, e) => OnGenerarReporteFacturacion();
            _view.GenerarReporteDemandaClick += (s, e) => OnGenerarReporteDemanda();
        }

        /// <summary>
        /// Genera el reporte de productividad de médicos
        /// </summary>
        private void OnGenerarReporteProductividad()
        {
            if (!ValidarFechas())
                return;

            try
            {
                var datos = _reportesService.ObtenerProductividadMedicos(_view.FechaDesde, _view.FechaHasta);
                
                if (datos.Count == 0)
                {
                    _view.MostrarMensaje("No hay datos disponibles para el período seleccionado.");
                    return;
                }

                _view.MostrarReporteProductividad(datos);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al generar reporte de productividad: " + ex.Message, true);
            }
        }

        /// <summary>
        /// Genera el reporte de facturación por obra social
        /// </summary>
        private void OnGenerarReporteFacturacion()
        {
            if (!ValidarFechas())
                return;

            try
            {
                var datos = _reportesService.ObtenerFacturacionPorObraSocial(_view.FechaDesde, _view.FechaHasta);
                
                if (datos.Count == 0)
                {
                    _view.MostrarMensaje("No hay datos disponibles para el período seleccionado.");
                    return;
                }

                _view.MostrarReporteFacturacion(datos);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al generar reporte de facturación: " + ex.Message, true);
            }
        }

        /// <summary>
        /// Genera el reporte de demanda de tipos de análisis
        /// </summary>
        private void OnGenerarReporteDemanda()
        {
            if (!ValidarFechas())
                return;

            try
            {
                var datos = _reportesService.ObtenerTopAnalisis(_view.FechaDesde, _view.FechaHasta, 10);
                
                if (datos.Count == 0)
                {
                    _view.MostrarMensaje("No hay datos disponibles para el período seleccionado.");
                    return;
                }

                _view.MostrarReporteDemanda(datos);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al generar reporte de demanda: " + ex.Message, true);
            }
        }

        /// <summary>
        /// Valida que el rango de fechas sea correcto
        /// </summary>
        /// <returns>True si las fechas son válidas, false en caso contrario</returns>
        private bool ValidarFechas()
        {
            if (_view.FechaDesde > _view.FechaHasta)
            {
                _view.MostrarMensaje("La fecha 'Desde' no puede ser mayor a la fecha 'Hasta'", true);
                return false;
            }

            return true;
        }
    }
}
