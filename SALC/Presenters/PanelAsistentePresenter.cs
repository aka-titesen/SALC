using System;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;

namespace SALC.Presenters
{
    public class PanelAsistentePresenter
    {
        private readonly IPanelAsistenteView _view;
        private readonly IAnalisisService _analisisService = new AnalisisService();

        public PanelAsistentePresenter(IPanelAsistenteView view)
        {
            _view = view;
            _view.BuscarHistorialClick += (s, e) => OnBuscarHistorial();
            _view.GenerarInformeClick += (s, e) => OnGenerarInforme();
        }

        private void OnBuscarHistorial()
        {
            if (!int.TryParse(_view.HistorialDniPacienteTexto, out var dniPaciente))
            {
                _view.MostrarMensaje("DNI de paciente inválido", true);
                return;
            }
            var lista = _analisisService.ObtenerAnalisisPorPaciente(dniPaciente);
            _view.CargarHistorialAnalisis(lista);
        }

        private void OnGenerarInforme()
        {
            if (!int.TryParse(_view.AnalisisIdParaInformeTexto, out var idAnalisis))
            {
                _view.MostrarMensaje("ID de análisis inválido", true);
                return;
            }
            var a = _analisisService.ObtenerAnalisisPorId(idAnalisis);
            if (a == null)
            {
                _view.MostrarMensaje("Análisis no encontrado", true);
                return;
            }
            if (a.IdEstado != 2) // 2 = Verificado
            {
                _view.MostrarMensaje("Solo se puede generar informe para análisis Verificado", true);
                return;
            }
            // Stub de informe: se implementará en Sprint 9
            _view.MostrarMensaje("Generación de informe (PDF) pendiente para Sprint 9");
        }
    }
}
