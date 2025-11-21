using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SALC.BLL;
using SALC.Domain;
using SALC.Presenters.ViewsContracts;
using SALC.Views.PanelAsistente;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para la gestión de pacientes desde el panel de asistente.
    /// Coordina la vista de gestión de pacientes con los servicios de negocio, limitando las operaciones
    /// según los permisos del rol de asistente (solo puede crear y editar datos básicos).
    /// </summary>
    public class GestionPacientesAsistentePresenter
    {
        private readonly IGestionPacientesAsistenteView _view;
        private readonly IPacienteService _pacienteService;
        private readonly ICatalogoService _catalogoService;
        private List<Paciente> _todosLosPacientes;

        /// <summary>
        /// Constructor del presenter
        /// </summary>
        /// <param name="view">Vista de gestión de pacientes para asistentes</param>
        public GestionPacientesAsistentePresenter(IGestionPacientesAsistenteView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _pacienteService = new PacienteService();
            _catalogoService = new CatalogoService();
            
            SubscribirEventos();
        }

        /// <summary>
        /// Suscribe a los eventos de la vista
        /// </summary>
        private void SubscribirEventos()
        {
            _view.VistaInicializada += OnVistaInicializada;
            _view.BuscarPacientesClick += OnBuscarPacientes;
            _view.NuevoPacienteClick += OnNuevoPaciente;
            _view.EditarPacienteClick += OnEditarPaciente;
            _view.RefrescarClick += OnRefrescar;
        }

        /// <summary>
        /// Inicializa la vista cargando los datos iniciales
        /// </summary>
        public void InicializarVista()
        {
            try
            {
                _view.MostrarCargando(true);
                CargarPacientes();
                _view.HabilitarAcciones(false);
                _view.MostrarCargando(false);
            }
            catch (Exception ex)
            {
                _view.MostrarCargando(false);
                _view.MostrarMensaje($"Error al inicializar la vista: {ex.Message}", true);
            }
        }

        /// <summary>
        /// Maneja el evento de vista inicializada
        /// </summary>
        private void OnVistaInicializada(object sender, EventArgs e)
        {
            InicializarVista();
        }

        /// <summary>
        /// Maneja el evento de refrescar la lista de pacientes
        /// </summary>
        private void OnRefrescar(object sender, EventArgs e)
        {
            CargarPacientes();
        }

        /// <summary>
        /// Carga todos los pacientes del sistema y actualiza la vista
        /// </summary>
        private void CargarPacientes()
        {
            try
            {
                _view.MostrarCargando(true);
                
                _todosLosPacientes = _pacienteService.ObtenerTodos().ToList();
                
                FiltrarPacientes();
                
                var pacientesActivos = _todosLosPacientes.Count(p => p.Estado == "Activo");
                _view.ActualizarContadores(_todosLosPacientes.Count, pacientesActivos);
                
                _view.MostrarCargando(false);
            }
            catch (Exception ex)
            {
                _view.MostrarCargando(false);
                _view.MostrarMensaje($"Error al cargar pacientes: {ex.Message}", true);
            }
        }

        /// <summary>
        /// Maneja el evento de búsqueda de pacientes
        /// </summary>
        private void OnBuscarPacientes(object sender, EventArgs e)
        {
            FiltrarPacientes();
        }

        /// <summary>
        /// Filtra la lista de pacientes según el texto de búsqueda ingresado
        /// </summary>
        private void FiltrarPacientes()
        {
            try
            {
                var pacientesFiltrados = _todosLosPacientes;

                if (!string.IsNullOrWhiteSpace(_view.TextoBusqueda))
                {
                    var filtro = _view.TextoBusqueda.Trim().ToLowerInvariant();
                    pacientesFiltrados = _todosLosPacientes.Where(p =>
                        p.Dni.ToString().Contains(filtro) ||
                        p.Nombre.ToLowerInvariant().Contains(filtro) ||
                        p.Apellido.ToLowerInvariant().Contains(filtro) ||
                        (p.Email?.ToLowerInvariant().Contains(filtro) ?? false)
                    ).ToList();
                }

                _view.CargarListaPacientes(pacientesFiltrados);
                
                _view.HabilitarAcciones(_view.PacienteSeleccionado != null);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al filtrar pacientes: {ex.Message}", true);
            }
        }

        /// <summary>
        /// Maneja el evento de crear un nuevo paciente
        /// </summary>
        private void OnNuevoPaciente(object sender, EventArgs e)
        {
            try
            {
                using (var frmEdit = new FrmPacienteEditAsistente())
                {
                    if (frmEdit.ShowDialog() == DialogResult.OK)
                    {
                        var nuevoPaciente = frmEdit.ObtenerPaciente();
                        
                        _pacienteService.CrearPaciente(nuevoPaciente);
                        
                        _view.MostrarMensaje(
                            $"Paciente creado exitosamente\n\n" +
                            $"DNI: {nuevoPaciente.Dni}\n" +
                            $"Nombre: {nuevoPaciente.Nombre} {nuevoPaciente.Apellido}\n" +
                            $"Estado: {nuevoPaciente.Estado}\n" +
                            $"Fecha de Nacimiento: {nuevoPaciente.FechaNac:dd/MM/yyyy}\n" +
                            $"Email: {nuevoPaciente.Email ?? "No especificado"}\n" +
                            $"Teléfono: {nuevoPaciente.Telefono ?? "No especificado"}");
                        
                        CargarPacientes();
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al crear paciente: {ex.Message}", true);
            }
        }

        /// <summary>
        /// Maneja el evento de editar un paciente existente
        /// </summary>
        private void OnEditarPaciente(object sender, EventArgs e)
        {
            var pacienteSeleccionado = _view.PacienteSeleccionado;
            if (pacienteSeleccionado == null)
            {
                _view.MostrarMensaje("Seleccione un paciente para editar.", true);
                return;
            }

            try
            {
                using (var frmEdit = new FrmPacienteEditAsistente(pacienteSeleccionado))
                {
                    if (frmEdit.ShowDialog() == DialogResult.OK)
                    {
                        var pacienteEditado = frmEdit.ObtenerPaciente();
                        
                        _pacienteService.ActualizarPaciente(pacienteEditado);
                        
                        var cambios = DetectarCambios(pacienteSeleccionado, pacienteEditado);
                        _view.MostrarMensaje($"Paciente actualizado exitosamente.\n\nCambios realizados:\n{cambios}");
                        
                        CargarPacientes();
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al editar paciente: {ex.Message}", true);
            }
        }

        /// <summary>
        /// Detecta y lista los cambios realizados en un paciente
        /// </summary>
        /// <param name="original">Paciente con datos originales</param>
        /// <param name="editado">Paciente con datos editados</param>
        /// <returns>Texto con la lista de cambios realizados</returns>
        private string DetectarCambios(Paciente original, Paciente editado)
        {
            var cambios = new List<string>();

            if (original.Nombre != editado.Nombre)
                cambios.Add($"- Nombre: '{original.Nombre}' -> '{editado.Nombre}'");
            
            if (original.Apellido != editado.Apellido)
                cambios.Add($"- Apellido: '{original.Apellido}' -> '{editado.Apellido}'");
            
            if (original.Email != editado.Email)
                cambios.Add($"- Email: '{original.Email ?? "Sin email"}' -> '{editado.Email ?? "Sin email"}'");
            
            if (original.Telefono != editado.Telefono)
                cambios.Add($"- Teléfono: '{original.Telefono ?? "Sin teléfono"}' -> '{editado.Telefono ?? "Sin teléfono"}'");
            
            if (original.FechaNac != editado.FechaNac)
                cambios.Add($"- Fecha Nacimiento: {original.FechaNac:dd/MM/yyyy} -> {editado.FechaNac:dd/MM/yyyy}");
            
            if (original.Sexo != editado.Sexo)
                cambios.Add($"- Sexo: '{original.Sexo}' -> '{editado.Sexo}'");
            
            if (original.IdObraSocial != editado.IdObraSocial)
                cambios.Add($"- Obra Social: {(original.IdObraSocial?.ToString() ?? "Sin obra social")} -> {(editado.IdObraSocial?.ToString() ?? "Sin obra social")}");

            return cambios.Count > 0 ? string.Join("\n", cambios) : "- Ningún cambio detectado";
        }
    }
}