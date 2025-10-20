using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;
using SALC.DAL;
using SALC.Views.PanelMedico;
using SALC.Views.PanelAdministrador;

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
            try
            {
                // Cargar tipos de análisis activos para el flujo de análisis
                var tiposActivos = _catalogoService.ObtenerTiposAnalisis()
                    .Where(t => t.Estado == "Activo").ToList();
                _view.CargarTiposAnalisis(tiposActivos);

                // Cargar pacientes SOLO para la gestión (ABM)
                CargarListadoPacientesGestion();
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al inicializar panel: " + ex.Message, true);
            }
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
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error cargando pacientes: " + ex.Message, true);
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
            var dni = _view.ObtenerPacienteSeleccionadoDni();
            if (dni == null)
            {
                _view.MostrarMensaje("Seleccione un paciente para editar.");
                return;
            }
            
            var existente = _pacientesGestion.FirstOrDefault(p => p.Dni == dni.Value);
            if (existente == null) 
            {
                _view.MostrarMensaje("No se encontró el paciente seleccionado.", true);
                return;
            }

            using (var dlg = new FrmPacienteEdit(existente))
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        var p = dlg.ObtenerPaciente();
                        _pacienteService.ActualizarPaciente(p);
                        CargarListadoPacientesGestion(); // Recargar solo la gestión
                        _view.MostrarMensaje("Paciente actualizado correctamente.");
                    }
                    catch (System.Exception ex)
                    {
                        _view.MostrarMensaje("Error al actualizar paciente: " + ex.Message, true);
                    }
                }
            }
        }

        private void OnPacientesGestionEliminar(object sender, EventArgs e)
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
                _view.MostrarMensaje("No se encontró el paciente seleccionado.", true);
                return;
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
            
            try
            {
                _pacienteService.EliminarPaciente(dni.Value);
                CargarListadoPacientesGestion(); // Recargar solo la gestión
                _view.MostrarMensaje("Paciente dado de baja correctamente.");
            }
            catch (System.Exception ex)
            {
                _view.MostrarMensaje("Error al dar de baja al paciente: " + ex.Message, true);
            }
        }

        #endregion

        #region RF-05: Crear Análisis (FLUJO INDEPENDIENTE - NO RELACIONADO CON ABM DE PACIENTES)

        private void OnBuscarPacienteParaCrearAnalisis(object sender, EventArgs e)
        {
            try
            {
                // Usar diálogo independiente para seleccionar paciente para análisis
                using (var frmSeleccion = new FrmSeleccionPaciente())
                {
                    if (frmSeleccion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _pacienteParaCrearAnalisis = frmSeleccion.PacienteSeleccionado;
                        _view.MostrarPacienteSeleccionado(_pacienteParaCrearAnalisis);
                        _view.MostrarMensaje($"Paciente seleccionado para análisis: {_pacienteParaCrearAnalisis.Nombre} {_pacienteParaCrearAnalisis.Apellido}");
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al seleccionar paciente: " + ex.Message, true);
                _view.LimpiarPacienteSeleccionado();
                _pacienteParaCrearAnalisis = null;
            }
        }

        private void OnCrearAnalisis(object sender, EventArgs e)
        {
            if (_pacienteParaCrearAnalisis == null)
            {
                _view.MostrarMensaje("Primero debe seleccionar un paciente para crear el análisis", true);
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
                    _pacienteParaCrearAnalisis.Dni,
                    idTipo.Value,
                    _dniMedico,
                    _view.CrearAnalisisObservaciones ?? ""
                );

                _view.MostrarMensaje($"✅ Análisis creado correctamente (ID: {analisis.IdAnalisis})\n\n" +
                                   $"📋 Paciente: {_pacienteParaCrearAnalisis.Nombre} {_pacienteParaCrearAnalisis.Apellido}\n\n" +
                                   "🔄 Flujo siguiente:\n" +
                                   "1. Vaya a la pestaña 'Cargar Resultados'\n" +
                                   "2. Seleccione este análisis recién creado\n" +
                                   "3. Complete los valores de las métricas");

                // Limpiar formulario de análisis (NO afecta la gestión de pacientes)
                _view.LimpiarPacienteSeleccionado();
                _pacienteParaCrearAnalisis = null;

                // Navegar automáticamente a la siguiente fase
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

                _view.MostrarMensaje($"✅ Análisis ID {_analisisParaFirmar.IdAnalisis} firmado correctamente.\n\n" +
                                   "El análisis está ahora verificado y disponible para que el Asistente genere el informe PDF.");

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

        #region Helpers

        private void PrepararResultadosParaAnalisis(int idAnalisis)
        {
            try
            {
                // Obtener el análisis para conocer su tipo
                var analisis = _analisisService.ObtenerAnalisisPorId(idAnalisis);
                if (analisis == null)
                {
                    _view.MostrarMensaje("No se encontró el análisis especificado.", true);
                    return;
                }

                // ✅ CORRECCIÓN: Obtener solo las métricas asociadas a este tipo de análisis (según ERS)
                var metricas = _catalogoService.ObtenerMetricasPorTipoAnalisis(analisis.IdTipoAnalisis).ToList();

                if (metricas.Count == 0)
                {
                    _view.MostrarMensaje($"El tipo de análisis seleccionado no tiene métricas asociadas.\n" +
                                       "Contacte al administrador para configurar las relaciones tipo análisis-métricas.", true);
                    return;
                }

                // Obtener resultados existentes
                var existentes = _analisisService.ObtenerResultados(idAnalisis).ToList();

                // Crear filas para el grid usando SOLO las métricas específicas del tipo de análisis
                var filas = new List<MetricaConResultado>();
                foreach (var metrica in metricas)
                {
                    var existente = existentes.FirstOrDefault(x => x.IdMetrica == metrica.IdMetrica);
                    filas.Add(MetricaConResultado.Desde(metrica, existente));
                }

                _view.CargarResultadosParaEdicion(filas);
                _view.MostrarMensaje($"✅ Se cargaron {filas.Count} métricas específicas para este tipo de análisis.\n\n" +
                                   "Complete los valores en la columna 'Resultado' y presione 'Guardar Resultados'.");

            }
            catch (Exception ex)
            {
                _view.MostrarMensaje("Error al cargar métricas: " + ex.Message, true);
            }
        }

        #endregion
    }
}
