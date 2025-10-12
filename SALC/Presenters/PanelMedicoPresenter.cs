using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;
using SALC.DAL;
using SALC.Views.PanelMedico;

namespace SALC.Presenters
{
    public class PanelMedicoPresenter
    {
        private readonly IPanelMedicoView _view;
        private readonly IAnalisisService _analisisService = new AnalisisService();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly IPacienteService _pacienteService = new PacienteService();
        private readonly PacienteRepositorio _pacienteRepo = new PacienteRepositorio();
        private readonly int _dniMedico;

        // Estado interno para seguimiento
        private Paciente _pacienteSeleccionado;
        private Analisis _analisisParaResultados;
        private Analisis _analisisParaFirmar;
        private Analisis _analisisParaInforme;

        public PanelMedicoPresenter(IPanelMedicoView view, int dniMedico)
        {
            _view = view;
            _dniMedico = dniMedico;

            // RF-05: Crear Análisis
            _view.CrearAnalisisClick += OnCrearAnalisis;
            _view.BuscarPacienteCrearClick += OnBuscarPacienteCrear;

            // RF-06: Cargar Resultados  
            _view.CargarResultadosGuardarClick += OnGuardarResultados;
            _view.BuscarAnalisisResultadosClick += OnBuscarAnalisisResultados;
            _view.CargarMetricasAnalisisClick += OnCargarMetricasAnalisis;

            // RF-07: Validar/Firmar
            _view.FirmarAnalisisClick += OnFirmarAnalisis;
            _view.BuscarAnalisisFirmarClick += OnBuscarAnalisisFirmar;

            // RF-08: Generar Informe
            _view.GenerarInformeClick += OnGenerarInforme;
            _view.BuscarAnalisisInformeClick += OnBuscarAnalisisInforme;

            InicializarPanel();
        }

        private void InicializarPanel()
        {
            try
            {
                // Cargar tipos de análisis activos
                var tiposActivos = _catalogoService.ObtenerTiposAnalisis()
                    .Where(t => t.Estado == "Activo").ToList();
                _view.CargarTiposAnalisis(tiposActivos);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al inicializar panel: " + ex.Message, true);
            }
        }

        #region RF-05: Crear Análisis

        private void OnBuscarPacienteCrear(object sender, EventArgs e)
        {
            try
            {
                using (var frmSeleccion = new FrmSeleccionPaciente())
                {
                    if (frmSeleccion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _pacienteSeleccionado = frmSeleccion.PacienteSeleccionado;
                        _view.MostrarPacienteSeleccionado(_pacienteSeleccionado);
                        _view.MostrarMensaje($"Paciente seleccionado: {_pacienteSeleccionado.Nombre} {_pacienteSeleccionado.Apellido}");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al seleccionar paciente: " + ex.Message, true);
                _view.LimpiarPacienteSeleccionado();
                _pacienteSeleccionado = null;
            }
        }

        private void OnCrearAnalisis(object sender, EventArgs e)
        {
            if (_pacienteSeleccionado == null)
            {
                _view.MostrarMensaje("Primero debe seleccionar un paciente", true);
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
                var analisis = _analisisService.CrearAnalisis(
                    _pacienteSeleccionado.Dni,
                    idTipo.Value,
                    _dniMedico,
                    _view.CrearAnalisisObservaciones ?? ""
                );

                _view.MostrarMensaje($"Análisis creado correctamente (ID: {analisis.IdAnalisis})\n" +
                                   "Proceda a la pestaña 'Cargar Resultados' para ingresar los valores.");

                // Limpiar formulario
                _view.LimpiarPacienteSeleccionado();
                _pacienteSeleccionado = null;

                // Sugerir ir a la siguiente fase
                _view.ActivarTabResultados();
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al crear análisis: " + ex.Message, true);
            }
        }

        #endregion

        #region RF-06: Cargar Resultados

        private void OnBuscarAnalisisResultados(object sender, EventArgs e)
        {
            try
            {
                using (var frmSeleccion = new FrmSeleccionAnalisisResultados(_dniMedico))
                {
                    if (frmSeleccion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _analisisParaResultados = frmSeleccion.AnalisisSeleccionado;
                        
                        // Obtener datos completos para mostrar
                        var paciente = _pacienteRepo.ObtenerPorId(_analisisParaResultados.DniPaciente);
                        var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == _analisisParaResultados.IdTipoAnalisis);

                        _view.MostrarAnalisisParaResultados(_analisisParaResultados, paciente, tipoAnalisis);
                        _view.MostrarMensaje($"Análisis seleccionado. Use 'Cargar Métricas' para comenzar a ingresar resultados.");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al seleccionar análisis: " + ex.Message, true);
                _view.LimpiarAnalisisParaResultados();
                _analisisParaResultados = null;
            }
        }

        private void OnCargarMetricasAnalisis(object sender, EventArgs e)
        {
            if (_analisisParaResultados == null)
            {
                _view.MostrarMensaje("Primero debe seleccionar un análisis válido", true);
                return;
            }

            PrepararResultadosParaAnalisis(_analisisParaResultados.IdAnalisis);
        }

        private void OnGuardarResultados(object sender, EventArgs e)
        {
            if (_analisisParaResultados == null)
            {
                _view.MostrarMensaje("Primero debe seleccionar un análisis válido", true);
                return;
            }

            try
            {
                var filas = _view.LeerResultadosEditados();
                if (filas == null || filas.Count == 0)
                {
                    _view.MostrarMensaje("No hay resultados para guardar", true);
                    return;
                }

                int resultadosGuardados = 0;
                foreach (var fila in filas)
                {
                    if (fila.Resultado.HasValue)
                    {
                        _analisisService.CargarResultado(
                            _analisisParaResultados.IdAnalisis,
                            fila.IdMetrica,
                            fila.Resultado.Value,
                            fila.Observaciones
                        );
                        resultadosGuardados++;
                    }
                }

                if (resultadosGuardados > 0)
                {
                    _view.MostrarMensaje($"Se guardaron {resultadosGuardados} resultados.\n" +
                                       "Cuando termine de cargar todos los resultados, puede proceder a validar el análisis.");
                }
                else
                {
                    _view.MostrarMensaje("No se guardó ningún resultado. Ingrese valores en la columna 'Resultado'", true);
                }

            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al guardar resultados: " + ex.Message, true);
            }
        }

        #endregion

        #region RF-07: Validar/Firmar

        private void OnBuscarAnalisisFirmar(object sender, EventArgs e)
        {
            try
            {
                using (var frmSeleccion = new FrmSeleccionAnalisisFirma(_dniMedico))
                {
                    if (frmSeleccion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _analisisParaFirmar = frmSeleccion.AnalisisSeleccionado;
                        
                        // Obtener datos completos para mostrar
                        var paciente = _pacienteRepo.ObtenerPorId(_analisisParaFirmar.DniPaciente);
                        var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == _analisisParaFirmar.IdTipoAnalisis);

                        var resultados = _analisisService.ObtenerResultados(_analisisParaFirmar.IdAnalisis).ToList();

                        _view.MostrarAnalisisParaFirmar(_analisisParaFirmar, paciente, tipoAnalisis);
                        _view.MostrarResultadosParaValidacion(resultados);
                        _view.MostrarMensaje($"Análisis seleccionado. Revise los resultados y presione 'Firmar Análisis'.");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al seleccionar análisis: " + ex.Message, true);
                _view.LimpiarAnalisisParaFirmar();
                _analisisParaFirmar = null;
            }
        }

        private void OnFirmarAnalisis(object sender, EventArgs e)
        {
            if (_analisisParaFirmar == null)
            {
                _view.MostrarMensaje("Primero debe seleccionar un análisis válido para firmar", true);
                return;
            }

            try
            {
                // Confirmar la acción
                var confirmacion = System.Windows.Forms.MessageBox.Show(
                    "¿Está seguro de que desea firmar este análisis?\n\n" +
                    "Una vez firmado, el análisis quedará verificado y no podrá modificarse.",
                    "Confirmar Firma",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question
                );

                if (confirmacion != System.Windows.Forms.DialogResult.Yes)
                    return;

                _analisisService.ValidarAnalisis(_analisisParaFirmar.IdAnalisis, _dniMedico);

                _view.MostrarMensaje($"Análisis ID {_analisisParaFirmar.IdAnalisis} firmado correctamente.\n" +
                                   "El análisis está ahora verificado y puede generar el informe.");

                // Limpiar estado
                _view.LimpiarAnalisisParaFirmar();
                _analisisParaFirmar = null;

            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al firmar análisis: " + ex.Message, true);
            }
        }

        #endregion

        #region RF-08: Generar Informe

        private void OnBuscarAnalisisInforme(object sender, EventArgs e)
        {
            try
            {
                using (var frmSeleccion = new FrmSeleccionAnalisisInforme(_dniMedico))
                {
                    if (frmSeleccion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _analisisParaInforme = frmSeleccion.AnalisisSeleccionado;
                        
                        // Obtener datos completos para mostrar
                        var paciente = _pacienteRepo.ObtenerPorId(_analisisParaInforme.DniPaciente);
                        var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == _analisisParaInforme.IdTipoAnalisis);

                        _view.MostrarAnalisisParaInforme(_analisisParaInforme, paciente, tipoAnalisis);
                        _view.MostrarMensaje($"Análisis verificado seleccionado. Listo para generar informe PDF.");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al seleccionar análisis: " + ex.Message, true);
                _view.LimpiarAnalisisParaInforme();
                _analisisParaInforme = null;
            }
        }

        private void OnGenerarInforme(object sender, EventArgs e)
        {
            if (_analisisParaInforme == null)
            {
                _view.MostrarMensaje("Primero debe seleccionar un análisis verificado", true);
                return;
            }

            // TODO: Implementar generación de PDF en sprint posterior
            _view.MostrarMensaje($"Funcionalidad de generación de informe PDF pendiente de implementación.\n" +
                               $"Análisis ID: {_analisisParaInforme.IdAnalisis} listo para generar.");
        }

        #endregion

        #region Helpers

        private void PrepararResultadosParaAnalisis(int idAnalisis)
        {
            try
            {
                // Obtener todas las métricas activas
                var metricas = _catalogoService.ObtenerMetricas()
                    .Where(m => m.Estado == "Activo").ToList();

                // Obtener resultados existentes
                var existentes = _analisisService.ObtenerResultados(idAnalisis).ToList();

                // Crear filas para el grid
                var filas = new List<ResultadoEdicionDto>();
                foreach (var metrica in metricas)
                {
                    var existente = existentes.FirstOrDefault(x => x.IdMetrica == metrica.IdMetrica);
                    filas.Add(new ResultadoEdicionDto
                    {
                        IdMetrica = metrica.IdMetrica,
                        Nombre = metrica.Nombre,
                        Unidad = metrica.UnidadMedida,
                        Min = metrica.ValorMinimo,
                        Max = metrica.ValorMaximo,
                        Resultado = existente?.Resultado,
                        Observaciones = existente?.Observaciones
                    });
                }

                _view.CargarResultadosParaEdicion(filas);
                _view.MostrarMensaje($"Se cargaron {filas.Count} métricas. Complete los valores en la columna 'Resultado'.");

            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al cargar métricas: " + ex.Message, true);
            }
        }

        #endregion
    }
}
