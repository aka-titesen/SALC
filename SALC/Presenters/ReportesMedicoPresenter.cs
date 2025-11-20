using System;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;

namespace SALC.Presenters
{
    public class ReportesMedicoPresenter
    {
        private readonly IReportesMedicoView _view;
        private readonly IReportesService _reportesService = new ReportesService();
        private readonly int _dniMedico;

        public ReportesMedicoPresenter(IReportesMedicoView view, int dniMedico)
        {
            _view = view;
            _dniMedico = dniMedico;
            _view.GenerarReporteAlertasClick += (s, e) => OnGenerarReporteAlertas();
            _view.GenerarReporteCargaTrabajoClick += (s, e) => OnGenerarReporteCargaTrabajo();
        }

        private void OnGenerarReporteAlertas()
        {
            if (!ValidarFechas())
                return;

            try
            {
                var datos = _reportesService.ObtenerValoresCriticos(_dniMedico, _view.FechaDesde, _view.FechaHasta);
                
                if (datos.Count == 0)
                {
                    _view.MostrarMensaje(
                        "Buenas noticias!\n\n" +
                        "No se encontraron valores fuera de los rangos de referencia en el período seleccionado.\n\n" +
                        "Todos los resultados de sus análisis están dentro de los parámetros normales."
                    );
                }

                _view.MostrarReporteAlertas(datos);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al generar reporte de alertas: " + ex.Message, true);
            }
        }

        private void OnGenerarReporteCargaTrabajo()
        {
            try
            {
                var datos = _reportesService.ObtenerCargaTrabajo(_dniMedico);
                
                _view.MostrarReporteCargaTrabajo(datos);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al generar reporte de carga de trabajo: " + ex.Message, true);
            }
        }

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
