using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SALC.BLL;
using SALC.Domain;
using SALC.Presenters.ViewsContracts;
using SALC.Views.PanelAdministrador;

namespace SALC.Presenters
{
    public class GestionPacientesAsistentePresenter
    {
        private readonly IGestionPacientesAsistenteView _view;
        private readonly IPacienteService _pacienteService;
        private readonly ICatalogoService _catalogoService;
        private List<Paciente> _todosLosPacientes;

        public GestionPacientesAsistentePresenter(IGestionPacientesAsistenteView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _pacienteService = new PacienteService();
            _catalogoService = new CatalogoService();
            
            SubscribirEventos();
        }

        private void SubscribirEventos()
        {
            _view.VistaInicializada += OnVistaInicializada;
            _view.BuscarPacientesClick += OnBuscarPacientes;
            _view.NuevoPacienteClick += OnNuevoPaciente;
            _view.EditarPacienteClick += OnEditarPaciente;
            _view.RefrescarClick += OnRefrescar;
        }

        public void InicializarVista()
        {
            try
            {
                _view.MostrarCargando(true);
                CargarPacientes();
                _view.HabilitarAcciones(false); // No habilitar hasta seleccionar un paciente
                _view.MostrarCargando(false);
            }
            catch (Exception ex)
            {
                _view.MostrarCargando(false);
                _view.MostrarMensaje($"Error al inicializar la vista: {ex.Message}", true);
            }
        }

        private void OnVistaInicializada(object sender, EventArgs e)
        {
            InicializarVista();
        }

        private void OnRefrescar(object sender, EventArgs e)
        {
            CargarPacientes();
        }

        private void CargarPacientes()
        {
            try
            {
                _view.MostrarCargando(true);
                
                // Cargar todos los pacientes (el asistente puede ver activos e inactivos)
                _todosLosPacientes = _pacienteService.ObtenerTodos().ToList();
                
                // Aplicar filtro si hay texto de búsqueda
                FiltrarPacientes();
                
                // Actualizar contadores
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

        private void OnBuscarPacientes(object sender, EventArgs e)
        {
            FiltrarPacientes();
        }

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
                
                // Actualizar estado de botones según selección
                _view.HabilitarAcciones(_view.PacienteSeleccionado != null);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al filtrar pacientes: {ex.Message}", true);
            }
        }

        private void OnNuevoPaciente(object sender, EventArgs e)
        {
            try
            {
                using (var frmEdit = new FrmPacienteEdit())
                {
                    if (frmEdit.ShowDialog() == DialogResult.OK)
                    {
                        var nuevoPaciente = frmEdit.ObtenerPaciente();
                        
                        // Validación: El asistente solo puede crear pacientes en estado "Activo"
                        // Según ERS v2.9 - RF-03: "El sistema permitirá al Asistente el alta y modificación de Pacientes"
                        if (nuevoPaciente.Estado != "Activo")
                        {
                            _view.MostrarMensaje(
                                "? RESTRICCIÓN DE ASISTENTE ?\n\n" +
                                "Los asistentes solo pueden crear pacientes en estado 'Activo'.\n" +
                                "No es posible crear pacientes inactivos directamente.\n\n" +
                                "El paciente será creado automáticamente como 'Activo'.", 
                                true);
                            
                            // Forzar estado activo
                            nuevoPaciente.Estado = "Activo";
                        }

                        // Crear paciente en la base de datos
                        _pacienteService.CrearPaciente(nuevoPaciente);
                        
                        // Mensaje de confirmación con información del paciente creado
                        _view.MostrarMensaje(
                            $"? Paciente creado exitosamente\n\n" +
                            $"DNI: {nuevoPaciente.Dni}\n" +
                            $"Nombre: {nuevoPaciente.Nombre} {nuevoPaciente.Apellido}\n" +
                            $"Estado: {nuevoPaciente.Estado}\n" +
                            $"Fecha de Nacimiento: {nuevoPaciente.FechaNac:dd/MM/yyyy}\n" +
                            $"Email: {nuevoPaciente.Email ?? "No especificado"}\n" +
                            $"Teléfono: {nuevoPaciente.Telefono ?? "No especificado"}");
                        
                        // Recargar lista para mostrar el nuevo paciente
                        CargarPacientes();
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"? Error al crear paciente: {ex.Message}", true);
            }
        }

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
                using (var frmEdit = new FrmPacienteEdit(pacienteSeleccionado))
                {
                    if (frmEdit.ShowDialog() == DialogResult.OK)
                    {
                        var pacienteEditado = frmEdit.ObtenerPaciente();
                        
                        // Validaciones específicas para el rol Asistente según ERS v2.9 (RF-03)
                        
                        // 1. El asistente NO puede cambiar el estado a "Inactivo" (baja lógica)
                        if (pacienteSeleccionado.Estado == "Activo" && pacienteEditado.Estado == "Inactivo")
                        {
                            _view.MostrarMensaje(
                                "? RESTRICCIÓN DE ASISTENTE ?\n\n" +
                                "Los asistentes no pueden realizar bajas de pacientes (cambio a estado 'Inactivo').\n" +
                                "Esta acción está restringida exclusivamente a médicos.\n\n" +
                                "Puede modificar todos los demás datos del paciente.", 
                                true);
                            return;
                        }

                        // 2. El asistente puede modificar todos los demás campos
                        // (DNI, Nombre, Apellido, Email, Teléfono, Obra Social, etc.)
                        // sin restricciones adicionales

                        // 3. Si el paciente está inactivo, el asistente puede reactivarlo
                        if (pacienteSeleccionado.Estado == "Inactivo" && pacienteEditado.Estado == "Activo")
                        {
                            _view.MostrarMensaje(
                                "?? INFORMACIÓN\n\n" +
                                "El paciente será reactivado (cambiado a estado 'Activo').\n" +
                                "Esta acción es permitida para asistentes.");
                        }

                        // Actualizar en base de datos
                        _pacienteService.ActualizarPaciente(pacienteEditado);
                        
                        // Mensaje de confirmación detallado
                        var cambios = DetectarCambios(pacienteSeleccionado, pacienteEditado);
                        _view.MostrarMensaje($"? Paciente actualizado exitosamente.\n\nCambios realizados:\n{cambios}");
                        
                        // Recargar lista para reflejar cambios
                        CargarPacientes();
                    }
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"? Error al editar paciente: {ex.Message}", true);
            }
        }

        /// <summary>
        /// Detecta los cambios realizados entre el paciente original y el editado
        /// </summary>
        private string DetectarCambios(Paciente original, Paciente editado)
        {
            var cambios = new List<string>();

            if (original.Nombre != editado.Nombre)
                cambios.Add($"• Nombre: '{original.Nombre}' ? '{editado.Nombre}'");
            
            if (original.Apellido != editado.Apellido)
                cambios.Add($"• Apellido: '{original.Apellido}' ? '{editado.Apellido}'");
            
            if (original.Email != editado.Email)
                cambios.Add($"• Email: '{original.Email ?? "Sin email"}' ? '{editado.Email ?? "Sin email"}'");
            
            if (original.Telefono != editado.Telefono)
                cambios.Add($"• Teléfono: '{original.Telefono ?? "Sin teléfono"}' ? '{editado.Telefono ?? "Sin teléfono"}'");
            
            if (original.FechaNac != editado.FechaNac)
                cambios.Add($"• Fecha Nacimiento: {original.FechaNac:dd/MM/yyyy} ? {editado.FechaNac:dd/MM/yyyy}");
            
            if (original.Sexo != editado.Sexo)
                cambios.Add($"• Sexo: '{original.Sexo}' ? '{editado.Sexo}'");
            
            if (original.Estado != editado.Estado)
                cambios.Add($"• Estado: '{original.Estado}' ? '{editado.Estado}'");
            
            if (original.IdObraSocial != editado.IdObraSocial)
                cambios.Add($"• Obra Social: {(original.IdObraSocial?.ToString() ?? "Sin obra social")} ? {(editado.IdObraSocial?.ToString() ?? "Sin obra social")}");

            return cambios.Count > 0 ? string.Join("\n", cambios) : "• Ningún cambio detectado";
        }
    }
}