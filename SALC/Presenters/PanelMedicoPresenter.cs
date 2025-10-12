using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;
using SALC.DAL;

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
            var dniTexto = _view.CrearAnalisisDniPacienteTexto;
            if (string.IsNullOrWhiteSpace(dniTexto))
            {
                _view.MostrarMensaje("Ingrese un DNI de paciente", true);
                return;
            }

            if (!int.TryParse(dniTexto, out var dni))
            {
                _view.MostrarMensaje("DNI inválido", true);
                return;
            }

            try
            {
                var paciente = _pacienteRepo.ObtenerPorId(dni);
                if (paciente == null)
                {
                    _view.MostrarMensaje("Paciente no encontrado", true);
                    _view.LimpiarPacienteSeleccionado();
                    _pacienteSeleccionado = null;
                    return;
                }

                if (paciente.Estado != "Activo")
                {
                    _view.MostrarMensaje("El paciente está inactivo", true);
                    _view.LimpiarPacienteSeleccionado();
                    _pacienteSeleccionado = null;
                    return;
                }

                _pacienteSeleccionado = paciente;
                _view.MostrarPacienteSeleccionado(paciente);
                _view.MostrarMensaje($"Paciente: {paciente.Nombre} {paciente.Apellido}");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al buscar paciente: " + ex.Message, true);
                _view.LimpiarPacienteSeleccionado();
                _pacienteSeleccionado = null;
            }
        }

        private void OnCrearAnalisis(object sender, EventArgs e)
        {
            if (_pacienteSeleccionado == null)
            {
                _view.MostrarMensaje("Primero debe buscar y seleccionar un paciente", true);
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
            var idTexto = _view.AnalisisIdParaResultadosTexto;
            if (string.IsNullOrWhiteSpace(idTexto))
            {
                _view.MostrarMensaje("Ingrese un ID de análisis", true);
                return;
            }

            if (!int.TryParse(idTexto, out var idAnalisis))
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
                    _view.LimpiarAnalisisParaResultados();
                    _analisisParaResultados = null;
                    return;
                }

                // Verificar que sea del médico actual
                if (analisis.DniCarga != _dniMedico)
                {
                    _view.MostrarMensaje("Solo puede editar resultados de análisis que usted creó", true);
                    _view.LimpiarAnalisisParaResultados();
                    _analisisParaResultados = null;
                    return;
                }

                // Verificar que esté en estado "Sin verificar"
                if (analisis.IdEstado != 1) // 1 = Sin verificar
                {
                    var estado = analisis.IdEstado == 2 ? "Verificado" : 
                                analisis.IdEstado == 3 ? "Anulado" : "Desconocido";
                    _view.MostrarMensaje($"El análisis está en estado '{estado}'. Solo se pueden editar análisis 'Sin verificar'", true);
                    _view.LimpiarAnalisisParaResultados();
                    _analisisParaResultados = null;
                    return;
                }

                // Obtener datos completos para mostrar
                var paciente = _pacienteRepo.ObtenerPorId(analisis.DniPaciente);
                var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                    .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);

                _analisisParaResultados = analisis;
                _view.MostrarAnalisisParaResultados(analisis, paciente, tipoAnalisis);
                _view.MostrarMensaje($"Análisis listo para carga de resultados. Use 'Cargar Métricas' para comenzar.");

            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al buscar análisis: " + ex.Message, true);
                _view.LimpiarAnalisisParaResultados();
                _analisisParaResultados = null;
            }
        }

        private void OnCargarMetricasAnalisis(object sender, EventArgs e)
        {
            if (_analisisParaResultados == null)
            {
                _view.MostrarMensaje("Primero debe buscar un análisis válido", true);
                return;
            }

            PrepararResultadosParaAnalisis(_analisisParaResultados.IdAnalisis);
        }

        private void OnGuardarResultados(object sender, EventArgs e)
        {
            if (_analisisParaResultados == null)
            {
                _view.MostrarMensaje("Primero debe buscar un análisis válido", true);
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
            var idTexto = _view.AnalisisIdParaFirmaTexto;
            if (string.IsNullOrWhiteSpace(idTexto))
            {
                _view.MostrarMensaje("Ingrese un ID de análisis", true);
                return;
            }

            if (!int.TryParse(idTexto, out var idAnalisis))
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
                    _view.LimpiarAnalisisParaFirmar();
                    _analisisParaFirmar = null;
                    return;
                }

                // Verificar que sea del médico actual
                if (analisis.DniCarga != _dniMedico)
                {
                    _view.MostrarMensaje("Solo puede firmar análisis que usted creó", true);
                    _view.LimpiarAnalisisParaFirmar();
                    _analisisParaFirmar = null;
                    return;
                }

                // Verificar el estado
                if (analisis.IdEstado == 2) // Ya verificado
                {
                    _view.MostrarMensaje("Este análisis ya está firmado y verificado", true);
                    _view.LimpiarAnalisisParaFirmar();
                    _analisisParaFirmar = null;
                    return;
                }

                if (analisis.IdEstado == 3) // Anulado
                {
                    _view.MostrarMensaje("Este análisis está anulado", true);
                    _view.LimpiarAnalisisParaFirmar();
                    _analisisParaFirmar = null;
                    return;
                }

                // Verificar que tenga resultados cargados
                var resultados = _analisisService.ObtenerResultados(idAnalisis).ToList();
                if (!resultados.Any())
                {
                    _view.MostrarMensaje("No se puede firmar un análisis sin resultados. Primero cargue los resultados.", true);
                    _view.LimpiarAnalisisParaFirmar();
                    _analisisParaFirmar = null;
                    return;
                }

                // Obtener datos completos para mostrar
                var paciente = _pacienteRepo.ObtenerPorId(analisis.DniPaciente);
                var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                    .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);

                _analisisParaFirmar = analisis;
                _view.MostrarAnalisisParaFirmar(analisis, paciente, tipoAnalisis);
                _view.MostrarResultadosParaValidacion(resultados);
                _view.MostrarMensaje($"Análisis listo para firma. Revise los resultados y presione 'Firmar Análisis'.");

            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al buscar análisis: " + ex.Message, true);
                _view.LimpiarAnalisisParaFirmar();
                _analisisParaFirmar = null;
            }
        }

        private void OnFirmarAnalisis(object sender, EventArgs e)
        {
            if (_analisisParaFirmar == null)
            {
                _view.MostrarMensaje("Primero debe buscar un análisis válido para firmar", true);
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
            var idTexto = _view.AnalisisIdParaInformeTexto;
            if (string.IsNullOrWhiteSpace(idTexto))
            {
                _view.MostrarMensaje("Ingrese un ID de análisis", true);
                return;
            }

            if (!int.TryParse(idTexto, out var idAnalisis))
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
                    _view.LimpiarAnalisisParaInforme();
                    _analisisParaInforme = null;
                    return;
                }

                // Verificar que sea del médico actual
                if (analisis.DniCarga != _dniMedico)
                {
                    _view.MostrarMensaje("Solo puede generar informes de análisis que usted creó", true);
                    _view.LimpiarAnalisisParaInforme();
                    _analisisParaInforme = null;
                    return;
                }

                // Verificar que esté verificado
                if (analisis.IdEstado != 2) // 2 = Verificado
                {
                    var estado = analisis.IdEstado == 1 ? "Sin verificar" : 
                                analisis.IdEstado == 3 ? "Anulado" : "Desconocido";
                    _view.MostrarMensaje($"Solo se pueden generar informes de análisis verificados. Estado actual: '{estado}'", true);
                    _view.LimpiarAnalisisParaInforme();
                    _analisisParaInforme = null;
                    return;
                }

                // Obtener datos completos para mostrar
                var paciente = _pacienteRepo.ObtenerPorId(analisis.DniPaciente);
                var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                    .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);

                _analisisParaInforme = analisis;
                _view.MostrarAnalisisParaInforme(analisis, paciente, tipoAnalisis);
                _view.MostrarMensaje($"Análisis verificado listo para generar informe.");

            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al buscar análisis: " + ex.Message, true);
                _view.LimpiarAnalisisParaInforme();
                _analisisParaInforme = null;
            }
        }

        private void OnGenerarInforme(object sender, EventArgs e)
        {
            if (_analisisParaInforme == null)
            {
                _view.MostrarMensaje("Primero debe buscar un análisis verificado", true);
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
