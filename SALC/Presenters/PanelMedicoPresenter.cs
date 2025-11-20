using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;
using SALC.DAL;
using SALC.Views.PanelMedico;
using SALC.Views.PanelAdministrador;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

namespace SALC.Presenters
{
    // ViewModel para mostrar información enriquecida de pacientes en la grilla del médico
    public class PacienteViewModelMedico
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNac { get; set; }
        public string Sexo { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string ObraSocial { get; set; }
        public string Estado { get; set; }

        public static PacienteViewModelMedico FromPaciente(Paciente paciente, string nombreObraSocial = null)
        {
            return new PacienteViewModelMedico
            {
                Dni = paciente.Dni,
                Nombre = paciente.Nombre,
                Apellido = paciente.Apellido,
                FechaNac = paciente.FechaNac,
                Sexo = ObtenerDescripcionSexo(paciente.Sexo),
                Email = string.IsNullOrWhiteSpace(paciente.Email) ? "-" : paciente.Email,
                Telefono = string.IsNullOrWhiteSpace(paciente.Telefono) ? "-" : paciente.Telefono,
                ObraSocial = nombreObraSocial ?? "Sin obra social",
                Estado = paciente.Estado
            };
        }

        private static string ObtenerDescripcionSexo(char sexo)
        {
            switch (sexo)
            {
                case 'M': return "Masculino";
                case 'F': return "Femenino";
                case 'X': return "Otro";
                default: return "No especificado";
            }
        }
    }

    public class PanelMedicoPresenter
    {
        private readonly IPanelMedicoView _view;
        private readonly IAnalisisService _analisisService = new AnalisisService();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly IPacienteService _pacienteService = new PacienteService();
        private readonly PacienteRepositorio _pacienteRepo = new PacienteRepositorio();
        private readonly ObraSocialRepositorio _obraSocialRepo = new ObraSocialRepositorio();
        private readonly int _dniMedico;

        // ===== ESTADO PARA FLUJO DE ANÁLISIS (SEPARADO) =====
        private Paciente _pacienteParaCrearAnalisis; // Solo para flujo de análisis
        private Analisis _analisisParaResultados;
        private Analisis _analisisParaFirmar;

        // ===== ESTADO PARA GESTIÓN DE PACIENTES (SEPARADO) =====
        private List<Paciente> _pacientesGestion = new List<Paciente>();
        private List<PacienteViewModelMedico> _pacientesGestionViewModel = new List<PacienteViewModelMedico>();
        private string _filtroEstadoPacientesGestion = "Todos";

        public PanelMedicoPresenter(IPanelMedicoView view, int dniMedico)
        {
            _view = view;
            _dniMedico = dniMedico;

            // RF-05: Crear Análisis (FLUJO INDEPENDIENTE)
            _view.CrearAnalisisClick += OnCrearAnalisis;
            _view.BuscarPacienteCrearClick += OnBuscarPacienteParaCrearAnalisis;

            // RF-06: Cargar Resultados  
            _view.CargarResultadosGuardarClick += OnGuardarResultados;
            _view.BuscarAnalisisResultadosClick += OnBuscarAnalisisResultados;
            _view.CargarMetricasAnalisisClick += OnCargarMetricasAnalisis;

            // RF-07: Validar/Firmar
            _view.FirmarAnalisisClick += OnFirmarAnalisis;
            _view.BuscarAnalisisFirmarClick += OnBuscarAnalisisFirmar;

            // RF-03: Gestión de Pacientes (ABM INDEPENDIENTE)
            _view.PacientesEditarClick += OnPacientesGestionEditar;
            _view.PacientesEliminarClick += OnPacientesGestionEliminar;
            _view.PacientesBuscarTextoChanged += OnPacientesGestionBuscar;
            _view.PacientesFiltroEstadoChanged += OnPacientesGestionFiltroEstado;

            InicializarPanel();
        }

        private void InicializarPanel()
        {
            ExceptionHandler.EjecutarConManejo(() =>
            {
                ExceptionHandler.LogInfo($"Inicializando panel médico - DNI: {_dniMedico}", "PanelMedico");

                // Cargar tipos de análisis activos para el flujo de análisis
                var tiposActivos = _catalogoService.ObtenerTiposAnalisis()
                    .Where(t => t.Estado == "Activo").ToList();
                _view.CargarTiposAnalisis(tiposActivos);

                // Cargar pacientes SOLO para la gestión (ABM)
                CargarListadoPacientesGestion();
            }, "InicializarPanelMedico");
        }

        #region RF-03: Gestión de Pacientes (ABM - SEPARADO DEL FLUJO DE ANÁLISIS)

        private void CargarListadoPacientesGestion()
        {
            try
            {
                _pacientesGestion = _pacienteService.ObtenerTodos().OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToList();
                GenerarViewModelsPacientesGestion();
                AplicarFiltrosPacientesGestion();
            }
            catch (SalcException salcEx)
            {
                _view.MostrarMensaje(salcEx.UserFriendlyMessage, true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.ManejarExcepcion(ex, "CargarListadoPacientesGestion");
            }
        }

        private void GenerarViewModelsPacientesGestion()
        {
            _pacientesGestionViewModel = new List<PacienteViewModelMedico>();
            foreach (var paciente in _pacientesGestion)
            {
                string nombreObraSocial = null;
                try
                {
                    if (paciente.IdObraSocial.HasValue)
                    {
                        var obraSocial = _obraSocialRepo.ObtenerPorId(paciente.IdObraSocial.Value);
                        nombreObraSocial = obraSocial?.Nombre;
                    }
                }
                catch (Exception)
                {
                    nombreObraSocial = "Error al cargar obra social";
                }

                _pacientesGestionViewModel.Add(PacienteViewModelMedico.FromPaciente(paciente, nombreObraSocial));
            }
        }

        private void AplicarFiltrosPacientesGestion()
        {
            IEnumerable<PacienteViewModelMedico> pacientesFiltrados = _pacientesGestionViewModel;

            // Filtro por estado
            if (_filtroEstadoPacientesGestion != "Todos")
            {
                pacientesFiltrados = pacientesFiltrados.Where(p => p.Estado == _filtroEstadoPacientesGestion);
            }

            _view.CargarPacientes(pacientesFiltrados.ToList());
        }

        private void OnPacientesGestionFiltroEstado(object sender, string filtro)
        {
            _filtroEstadoPacientesGestion = filtro ?? "Todos";
            AplicarFiltrosPacientesGestion();
        }

        private void OnPacientesGestionBuscar(object sender, string txt)
        {
            var q = txt?.Trim().ToLowerInvariant();
            IEnumerable<PacienteViewModelMedico> src = _pacientesGestionViewModel;

            // Aplicar filtro de estado
            if (_filtroEstadoPacientesGestion != "Todos")
            {
                src = src.Where(p => p.Estado == _filtroEstadoPacientesGestion);
            }

            // Aplicar filtro de búsqueda
            if (!string.IsNullOrEmpty(q))
            {
                src = src.Where(p => p.Apellido.ToLowerInvariant().Contains(q)
                    || p.Nombre.ToLowerInvariant().Contains(q)
                    || p.Dni.ToString().Contains(q));
            }
            
            _view.CargarPacientes(src.ToList());
        }

        private void OnPacientesGestionEditar(object sender, EventArgs e)
        {
            ExceptionHandler.EjecutarConManejo(() =>
            {
                var dni = _view.ObtenerPacienteSeleccionadoDni();
                if (dni == null)
                {
                    _view.MostrarMensaje("Seleccione un paciente para editar.");
                    return;
                }
                
                var existente = _pacientesGestion.FirstOrDefault(p => p.Dni == dni.Value);
                if (existente == null) 
                {
                    throw new SalcBusinessException("No se encontró el paciente seleccionado.");
                }

                using (var dlg = new FrmPacienteEdit(existente))
                {
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var p = dlg.ObtenerPaciente();
                        _pacienteService.ActualizarPaciente(p);
                        CargarListadoPacientesGestion();
                        _view.MostrarMensaje("Paciente actualizado correctamente.");
                        ExceptionHandler.LogInfo($"Paciente actualizado - DNI: {dni.Value}", "PanelMedico");
                    }
                }
            }, "EditarPacienteMedico");
        }

        private void OnPacientesGestionEliminar(object sender, EventArgs e)
        {
            ExceptionHandler.EjecutarConManejo(() =>
            {
                var dni = _view.ObtenerPacienteSeleccionadoDni();
                if (dni == null)
                {
                    _view.MostrarMensaje("Seleccione un paciente para dar de baja.");
                    return;
                }
                
                var paciente = _pacientesGestion.FirstOrDefault(p => p.Dni == dni.Value);
                if (paciente == null) 
                {
                    throw new SalcBusinessException("No se encontró el paciente seleccionado.");
                }

                if (paciente.Estado == "Inactivo")
                {
                    _view.MostrarMensaje("El paciente ya está dado de baja.");
                    return;
                }
                
                var confirm = System.Windows.Forms.MessageBox.Show(
                    $"¿Desea dar de baja al paciente {paciente.Nombre} {paciente.Apellido} (DNI: {dni})?\n\n" +
                    "El paciente se marcará como inactivo pero se conservarán todos sus datos y análisis.",
                    "Confirmar Baja Lógica", 
                    System.Windows.Forms.MessageBoxButtons.YesNo, 
                    System.Windows.Forms.MessageBoxIcon.Question);
                    
                if (confirm != System.Windows.Forms.DialogResult.Yes) return;
                
                _pacienteService.EliminarPaciente(dni.Value);
                CargarListadoPacientesGestion();
                _view.MostrarMensaje("Paciente dado de baja correctamente.");
                ExceptionHandler.LogInfo($"Paciente dado de baja - DNI: {dni.Value}", "PanelMedico");
            }, "EliminarPacienteMedico");
        }

        #endregion

        #region RF-05: Crear Análisis (FLUJO INDEPENDIENTE - NO RELACIONADO CON ABM DE PACIENTES)

        private void OnBuscarPacienteParaCrearAnalisis(object sender, EventArgs e)
        {
            ExceptionHandler.EjecutarConManejo(() =>
            {
                using (var frmSeleccion = new FrmSeleccionPaciente())
                {
                    if (frmSeleccion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _pacienteParaCrearAnalisis = frmSeleccion.PacienteSeleccionado;
                        _view.MostrarPacienteSeleccionado(_pacienteParaCrearAnalisis);
                        _view.MostrarMensaje($"Paciente seleccionado para análisis: {_pacienteParaCrearAnalisis.Nombre} {_pacienteParaCrearAnalisis.Apellido}");
                    }
                }
            }, "BuscarPacienteParaAnalisis");
        }

        private void OnCrearAnalisis(object sender, EventArgs e)
        {
            try
            {
                if (_pacienteParaCrearAnalisis == null)
                {
                    throw new SalcValidacionException("Primero debe seleccionar un paciente para crear el análisis", "paciente");
                }

                var idTipo = _view.TipoAnalisisSeleccionadoId;
                if (idTipo == null)
                {
                    throw new SalcValidacionException("Seleccione un tipo de análisis", "tipoAnalisis");
                }

                ExceptionHandler.LogInfo($"Creando análisis - Paciente: {_pacienteParaCrearAnalisis.Dni}, Tipo: {idTipo.Value}, Médico: {_dniMedico}", "PanelMedico");

                var analisis = _analisisService.CrearAnalisis(
                    _pacienteParaCrearAnalisis.Dni,
                    idTipo.Value,
                    _dniMedico,
                    _view.CrearAnalisisObservaciones ?? ""
                );

                var mensaje = string.Format(
                    "ANÁLISIS CREADO CORRECTAMENTE\n\n" +
                    "ID del Análisis: {0}\n" +
                    "Paciente: {1} {2}\n\n" +
                    "FLUJO SIGUIENTE:\n\n" +
                    "1. Vaya a la pestaña 'Cargar Resultados'\n" +
                    "2. Seleccione este análisis recién creado\n" +
                    "3. Complete los valores de las métricas",
                    analisis.IdAnalisis,
                    _pacienteParaCrearAnalisis.Nombre,
                    _pacienteParaCrearAnalisis.Apellido
                );

                System.Windows.Forms.MessageBox.Show(
                    mensaje,
                    "Análisis Creado Exitosamente",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information
                );

                _view.LimpiarPacienteSeleccionado();
                _pacienteParaCrearAnalisis = null;
                _view.ActivarTabResultados();

                ExceptionHandler.LogInfo($"Análisis creado exitosamente - ID: {analisis.IdAnalisis}", "PanelMedico");
            }
            catch (SalcValidacionException valEx)
            {
                _view.MostrarMensaje(valEx.UserFriendlyMessage, true);
            }
            catch (SalcException salcEx)
            {
                _view.MostrarMensaje(salcEx.UserFriendlyMessage, true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.ManejarExcepcion(ex, "CrearAnalisisMedico");
            }
        }

        #endregion

        #region RF-06: Cargar Resultados

        private void OnBuscarAnalisisResultados(object sender, EventArgs e)
        {
            ExceptionHandler.EjecutarConManejo(() =>
            {
                using (var frmSeleccion = new FrmSeleccionAnalisisResultados(_dniMedico))
                {
                    if (frmSeleccion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _analisisParaResultados = frmSeleccion.AnalisisSeleccionado;
                        
                        var paciente = _pacienteRepo.ObtenerPorId(_analisisParaResultados.DniPaciente);
                        var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == _analisisParaResultados.IdTipoAnalisis);

                        _view.MostrarAnalisisParaResultados(_analisisParaResultados, paciente, tipoAnalisis);
                        _view.MostrarMensaje("Análisis seleccionado. Use 'Cargar Métricas' para comenzar a ingresar resultados.");
                    }
                }
            }, "BuscarAnalisisResultados");
        }

        private void OnCargarMetricasAnalisis(object sender, EventArgs e)
        {
            try
            {
                if (_analisisParaResultados == null)
                {
                    throw new SalcValidacionException("Primero debe seleccionar un análisis válido", "analisis");
                }

                PrepararResultadosParaAnalisis(_analisisParaResultados.IdAnalisis);
            }
            catch (SalcException salcEx)
            {
                _view.MostrarMensaje(salcEx.UserFriendlyMessage, true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.ManejarExcepcion(ex, "CargarMetricasAnalisis");
            }
        }

        private void OnGuardarResultados(object sender, EventArgs e)
        {
            try
            {
                if (_analisisParaResultados == null)
                {
                    throw new SalcValidacionException("Primero debe seleccionar un análisis válido", "analisis");
                }

                var filas = _view.LeerResultadosEditados();
                if (filas == null || filas.Count == 0)
                {
                    throw new SalcValidacionException("No hay resultados para guardar", "resultados");
                }

                ExceptionHandler.LogInfo($"Guardando resultados - Análisis: {_analisisParaResultados.IdAnalisis}", "PanelMedico");

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
                    var mensaje = string.Format(
                        "RESULTADOS GUARDADOS CORRECTAMENTE\n\n" +
                        "Se guardaron {0} resultado(s) de laboratorio.\n\n" +
                        "PRÓXIMO PASO:\n\n" +
                        "Cuando termine de cargar todos los resultados,\n" +
                        "proceda a la pestaña 'Validar y Firmar' para\n" +
                        "dar validez clínica al análisis.",
                        resultadosGuardados
                    );

                    System.Windows.Forms.MessageBox.Show(mensaje, "Resultados Guardados",
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                    ExceptionHandler.LogInfo($"Resultados guardados - Análisis: {_analisisParaResultados.IdAnalisis}, Cantidad: {resultadosGuardados}", "PanelMedico");
                }
                else
                {
                    _view.MostrarMensaje("No se guardó ningún resultado. Ingrese valores en la columna 'Resultado'", true);
                }
            }
            catch (SalcException salcEx)
            {
                _view.MostrarMensaje(salcEx.UserFriendlyMessage, true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.ManejarExcepcion(ex, "GuardarResultados");
            }
        }

        #endregion

        #region RF-07: Validar/Firmar

        private void OnBuscarAnalisisFirmar(object sender, EventArgs e)
        {
            ExceptionHandler.EjecutarConManejo(() =>
            {
                using (var frmSeleccion = new FrmSeleccionAnalisisFirma(_dniMedico))
                {
                    if (frmSeleccion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _analisisParaFirmar = frmSeleccion.AnalisisSeleccionado;
                        
                        var paciente = _pacienteRepo.ObtenerPorId(_analisisParaFirmar.DniPaciente);
                        var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == _analisisParaFirmar.IdTipoAnalisis);

                        var resultados = _analisisService.ObtenerResultados(_analisisParaFirmar.IdAnalisis).ToList();

                        _view.MostrarAnalisisParaFirmar(_analisisParaFirmar, paciente, tipoAnalisis);
                        _view.MostrarResultadosParaValidacion(resultados);
                        _view.MostrarMensaje("Análisis seleccionado. Revise los resultados y presione 'Firmar Digitalmente'.");
                    }
                }
            }, "BuscarAnalisisFirmar");
        }

        private void OnFirmarAnalisis(object sender, EventArgs e)
        {
            try
            {
                if (_analisisParaFirmar == null)
                {
                    throw new SalcValidacionException("Primero debe seleccionar un análisis válido para firmar", "analisis");
                }

                var confirmacion = System.Windows.Forms.MessageBox.Show(
                    "¿Está seguro de que desea firmar digitalmente este análisis?\n\n" +
                    "Una vez firmado, el análisis quedará VERIFICADO y NO podrá modificarse.\n\n" +
                    "¿Desea continuar con la firma digital?",
                    "Confirmar Firma Digital del Análisis",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question
                );

                if (confirmacion != System.Windows.Forms.DialogResult.Yes)
                    return;

                ExceptionHandler.LogInfo($"Firmando análisis - ID: {_analisisParaFirmar.IdAnalisis}, Médico: {_dniMedico}", "PanelMedico");

                _analisisService.ValidarAnalisis(_analisisParaFirmar.IdAnalisis, _dniMedico);

                var mensaje = string.Format(
                    "ANÁLISIS FIRMADO EXITOSAMENTE\n\n" +
                    "ID del Análisis: {0}\n\n" +
                    "El análisis está ahora VERIFICADO y disponible\n" +
                    "para que el personal asistente genere el informe\n" +
                    "PDF para entrega al paciente.",
                    _analisisParaFirmar.IdAnalisis
                );

                System.Windows.Forms.MessageBox.Show(mensaje, "Análisis Firmado Digitalmente",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                _view.LimpiarAnalisisParaFirmar();
                _analisisParaFirmar = null;

                ExceptionHandler.LogInfo($"Análisis firmado exitosamente", "PanelMedico");
            }
            catch (SalcAuthorizationException authEx)
            {
                _view.MostrarMensaje(authEx.UserFriendlyMessage, true);
            }
            catch (SalcException salcEx)
            {
                _view.MostrarMensaje(salcEx.UserFriendlyMessage, true);
            }
            catch (Exception ex)
            {
                ExceptionHandler.ManejarExcepcion(ex, "FirmarAnalisis");
            }
        }

        #endregion

        #region Helpers

        private void PrepararResultadosParaAnalisis(int idAnalisis)
        {
            try
            {
                var analisis = _analisisService.ObtenerAnalisisPorId(idAnalisis);
                if (analisis == null)
                {
                    throw new SalcBusinessException("No se encontró el análisis especificado.");
                }

                var metricas = _catalogoService.ObtenerMetricasPorTipoAnalisis(analisis.IdTipoAnalisis).ToList();

                if (metricas.Count == 0)
                {
                    throw new SalcBusinessException(
                        "El tipo de análisis seleccionado no tiene métricas asociadas.\n\n" +
                        "Contacte al administrador para configurar las relaciones\n" +
                        "entre tipos de análisis y métricas en el sistema.");
                }

                var existentes = _analisisService.ObtenerResultados(idAnalisis).ToList();

                var filas = new List<MetricaConResultado>();
                foreach (var metrica in metricas)
                {
                    var existente = existentes.FirstOrDefault(x => x.IdMetrica == metrica.IdMetrica);
                    filas.Add(MetricaConResultado.Desde(metrica, existente));
                }

                _view.CargarResultadosParaEdicion(filas);
                
                var mensaje = string.Format(
                    "MÉTRICAS CARGADAS CORRECTAMENTE\n\n" +
                    "Se cargaron {0} métrica(s) específica(s)\n" +
                    "para este tipo de análisis.\n\n" +
                    "INSTRUCCIONES:\n\n" +
                    "1. Complete los valores en la columna 'Resultado'\n" +
                    "2. Presione el botón 'Guardar Resultados'",
                    filas.Count
                );

                System.Windows.Forms.MessageBox.Show(mensaje, "Métricas del Análisis",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (SalcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SalcException("Error al cargar las métricas del análisis",
                    "No se pudieron cargar las métricas. Por favor, intente nuevamente.", "LOAD_METRICS_ERROR");
            }
        }

        #endregion
    }
}
