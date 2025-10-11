using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;

namespace SALC.Presenters
{
    public class PanelMedicoPresenter
    {
        private readonly IPanelMedicoView _view;
        private readonly IAnalisisService _analisisService = new AnalisisService();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly int _dniMedico;

        public PanelMedicoPresenter(IPanelMedicoView view, int dniMedico)
        {
            _view = view;
            _dniMedico = dniMedico;
            _view.CrearAnalisisClick += (s, e) => OnCrearAnalisis();
            _view.CargarResultadosGuardarClick += (s, e) => OnGuardarResultados();
            _view.FirmarAnalisisClick += (s, e) => OnFirmarAnalisis();
            _view.GenerarInformeClick += (s, e) => _view.MostrarMensaje("Generación de informe pendiente (Sprint 9)");

            // Cargar tipos de análisis en el combo
            _view.CargarTiposAnalisis(_catalogoService.ObtenerTiposAnalisis());
        }

        private void OnCrearAnalisis()
        {
            if (!int.TryParse(_view.CrearAnalisisDniPacienteTexto, out var dniPaciente))
            {
                _view.MostrarMensaje("DNI de paciente inválido", true);
                return;
            }
            var idTipo = _view.TipoAnalisisSeleccionadoId;
            if (idTipo == null)
            {
                _view.MostrarMensaje("Seleccione un tipo de análisis", true);
                return;
            }
            try
            {
                var a = _analisisService.CrearAnalisis(dniPaciente, idTipo.Value, _dniMedico, _view.CrearAnalisisObservaciones);
                _view.MostrarMensaje($"Análisis creado (ID {a.IdAnalisis})");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al crear análisis: " + ex.Message, true);
            }
        }

        private void OnGuardarResultados()
        {
            if (!int.TryParse(_view.AnalisisIdParaResultadosTexto, out var idAnalisis))
            {
                _view.MostrarMensaje("ID de análisis inválido", true);
                return;
            }
            try
            {
                var analisis = _analisisService.ObtenerAnalisisPorId(idAnalisis);
                if (analisis == null)
                {
                    _view.MostrarMensaje("Análisis no encontrado", true);
                    return;
                }
                if (analisis.DniCarga != _dniMedico)
                {
                    _view.MostrarMensaje("Solo puede editar resultados de análisis que usted creó", true);
                    return;
                }
                if (analisis.IdEstado != 1)
                {
                    _view.MostrarMensaje("El análisis no está 'Sin verificar'. No se puede editar.", true);
                    return;
                }
                var filas = _view.LeerResultadosEditados();
                if (filas == null || filas.Count == 0)
                {
                    // Primer uso: preparar grilla con métricas base y resultados existentes
                    PrepararResultadosParaAnalisis(idAnalisis);
                    _view.MostrarMensaje("Se cargaron las métricas del análisis. Complete los resultados y vuelva a presionar Guardar.");
                    return;
                }
                foreach (var f in filas)
                {
                    if (f.Resultado.HasValue)
                        _analisisService.CargarResultado(idAnalisis, f.IdMetrica, f.Resultado.Value, f.Observaciones);
                }
                _view.MostrarMensaje("Resultados guardados");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al guardar resultados: " + ex.Message, true);
            }
        }

        private void OnFirmarAnalisis()
        {
            if (!int.TryParse(_view.AnalisisIdParaFirmaTexto, out var idAnalisis))
            {
                _view.MostrarMensaje("ID de análisis inválido", true);
                return;
            }
            try
            {
                var analisis = _analisisService.ObtenerAnalisisPorId(idAnalisis);
                if (analisis == null)
                {
                    _view.MostrarMensaje("Análisis no encontrado", true);
                    return;
                }
                if (analisis.DniCarga != _dniMedico)
                {
                    _view.MostrarMensaje("Solo puede firmar análisis que usted creó", true);
                    return;
                }
                _analisisService.ValidarAnalisis(idAnalisis, _dniMedico);
                _view.MostrarMensaje("Análisis firmado correctamente");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al firmar: " + ex.Message, true);
            }
        }

        // Helper para cargar grid de resultados con métricas base (opcional, cuando el usuario empiece a editar)
        public void PrepararResultadosParaAnalisis(int idAnalisis)
        {
            var metricas = _catalogoService.ObtenerMetricas();
            var existentes = _analisisService.ObtenerResultados(idAnalisis).ToList();
            var filas = new List<ResultadoEdicionDto>();
            foreach (var m in metricas)
            {
                var ex = existentes.FirstOrDefault(x => x.IdMetrica == m.IdMetrica);
                filas.Add(new ResultadoEdicionDto
                {
                    IdMetrica = m.IdMetrica,
                    Nombre = m.Nombre,
                    Unidad = m.UnidadMedida,
                    Min = m.ValorMinimo,
                    Max = m.ValorMaximo,
                    Resultado = ex?.Resultado,
                    Observaciones = ex?.Observaciones
                });
            }
            _view.CargarResultadosParaEdicion(filas);
        }
    }
}
