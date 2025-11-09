using System;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;

namespace SALC.Presenters
{
    public class PanelAsistentePresenter
    {
        private readonly IPanelAsistenteView _view;
        private readonly IAnalisisService _analisisService = new AnalisisService();
        private readonly IPacienteService _pacienteService = new PacienteService();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly IInformeService _informeService = new InformeService();
        private readonly IEmailService _emailService = new EmailService();

        public PanelAsistentePresenter(IPanelAsistenteView view)
        {
            _view = view;
            _view.BuscarPacientesClick += (s, e) => OnBuscarPacientes();
            _view.VerHistorialClick += (s, e) => OnVerHistorial();
        }

        public void InicializarVista()
        {
            // Cargar todos los pacientes activos al inicializar
            CargarTodosLosPacientes();
        }

        private void CargarTodosLosPacientes()
        {
            try
            {
                var pacientes = _pacienteService.ObtenerActivos();
                _view.CargarListaPacientes(pacientes);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje(string.Format("Error al cargar pacientes: {0}", ex.Message), true);
            }
        }

        private void OnBuscarPacientes()
        {
            try
            {
                var textoBusqueda = _view.BusquedaPacienteTexto?.Trim();
                
                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    // Si no hay texto, mostrar todos los pacientes
                    CargarTodosLosPacientes();
                    return;
                }

                var pacientes = _pacienteService.ObtenerActivos();
                
                // Buscar por DNI o apellido
                if (int.TryParse(textoBusqueda, out int dni))
                {
                    // Búsqueda por DNI
                    pacientes = pacientes.Where(p => p.Dni.ToString().Contains(textoBusqueda));
                }
                else
                {
                    // Búsqueda por apellido (case-insensitive)
                    pacientes = pacientes.Where(p => p.Apellido.ToLower().Contains(textoBusqueda.ToLower()));
                }

                _view.CargarListaPacientes(pacientes);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje(string.Format("Error en la búsqueda: {0}", ex.Message), true);
            }
        }

        private void OnVerHistorial()
        {
            try
            {
                var pacienteSeleccionado = _view.PacienteSeleccionado;
                if (pacienteSeleccionado == null)
                {
                    _view.MostrarMensaje("Seleccione un paciente para ver su historial.", true);
                    return;
                }

                // Validar que el paciente tenga datos completos
                if (pacienteSeleccionado.Dni <= 0)
                {
                    _view.MostrarMensaje("El DNI del paciente no es válido.", true);
                    return;
                }

                // Cargar paciente completo desde la base de datos
                var pacienteCompleto = _pacienteService.ObtenerPorDni(pacienteSeleccionado.Dni);
                if (pacienteCompleto == null)
                {
                    _view.MostrarMensaje("No se encontró información completa del paciente.", true);
                    return;
                }

                // Abrir ventana modal con historial completo
                using (var frmHistorial = new SALC.Views.PanelAsistente.FrmHistorialPaciente(pacienteCompleto))
                {
                    frmHistorial.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje(string.Format("Error al abrir historial: {0}", ex.Message), true);
            }
        }
    }
}
