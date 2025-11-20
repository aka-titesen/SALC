using System;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para el panel del asistente de laboratorio.
    /// Coordina la vista del asistente con los servicios de negocio para la búsqueda de pacientes
    /// y visualización de historial de análisis.
    /// </summary>
    public class PanelAsistentePresenter
    {
        private readonly IPanelAsistenteView _view;
        private readonly IAnalisisService _analisisService = new AnalisisService();
        private readonly IPacienteService _pacienteService = new PacienteService();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly IInformeService _informeService = new InformeService();
        private readonly IEmailService _emailService = new EmailService();

        /// <summary>
        /// Constructor del presenter
        /// </summary>
        /// <param name="view">Vista del panel de asistente</param>
        public PanelAsistentePresenter(IPanelAsistenteView view)
        {
            _view = view;
            _view.BuscarPacientesClick += (s, e) => OnBuscarPacientes();
            _view.VerHistorialClick += (s, e) => OnVerHistorial();
        }

        /// <summary>
        /// Inicializa la vista cargando los datos iniciales
        /// </summary>
        public void InicializarVista()
        {
            ExceptionHandler.EjecutarConManejo(() =>
            {
                ExceptionHandler.LogInfo("Inicializando vista de asistente", "PanelAsistente");
                CargarTodosLosPacientes();
            }, "InicializarVistaAsistente");
        }

        /// <summary>
        /// Carga todos los pacientes activos en la vista
        /// </summary>
        private void CargarTodosLosPacientes()
        {
            try
            {
                var pacientes = _pacienteService.ObtenerActivos();
                _view.CargarListaPacientes(pacientes);
            }
            catch (SalcDatabaseException dbEx)
            {
                _view.MostrarMensaje(dbEx.UserFriendlyMessage, true);
                ExceptionHandler.LogWarning($"Error de BD al cargar pacientes: {dbEx.Message}", "PanelAsistente");
            }
            catch (SalcException salcEx)
            {
                _view.MostrarMensaje(salcEx.UserFriendlyMessage, true);
            }
            catch (Exception ex)
            {
                var mensaje = ExceptionHandler.ManejarExcepcion(ex, "CargarPacientesAsistente", mostrarAlUsuario: false);
                _view.MostrarMensaje(mensaje, true);
            }
        }

        /// <summary>
        /// Maneja el evento de búsqueda de pacientes
        /// </summary>
        private void OnBuscarPacientes()
        {
            try
            {
                var textoBusqueda = _view.BusquedaPacienteTexto?.Trim();
                
                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    CargarTodosLosPacientes();
                    return;
                }

                var pacientes = _pacienteService.ObtenerActivos();
                
                // Buscar por DNI o apellido
                if (int.TryParse(textoBusqueda, out int dni))
                {
                    pacientes = pacientes.Where(p => p.Dni.ToString().Contains(textoBusqueda));
                }
                else
                {
                    pacientes = pacientes.Where(p => p.Apellido.ToLower().Contains(textoBusqueda.ToLower()));
                }

                _view.CargarListaPacientes(pacientes);
                ExceptionHandler.LogInfo($"Búsqueda de pacientes - Criterio: '{textoBusqueda}'", "PanelAsistente");
            }
            catch (SalcException salcEx)
            {
                _view.MostrarMensaje(salcEx.UserFriendlyMessage, true);
            }
            catch (Exception ex)
            {
                var mensaje = ExceptionHandler.ManejarExcepcion(ex, "BuscarPacientesAsistente", mostrarAlUsuario: false);
                _view.MostrarMensaje(mensaje, true);
            }
        }

        /// <summary>
        /// Maneja el evento de visualización de historial de un paciente
        /// </summary>
        private void OnVerHistorial()
        {
            try
            {
                var pacienteSeleccionado = _view.PacienteSeleccionado;
                if (pacienteSeleccionado == null)
                {
                    throw new SalcValidacionException("Seleccione un paciente para ver su historial.", "paciente");
                }

                if (pacienteSeleccionado.Dni <= 0)
                {
                    throw new SalcValidacionException("El DNI del paciente no es válido.", "dni");
                }

                ExceptionHandler.LogInfo($"Abriendo historial de paciente - DNI: {pacienteSeleccionado.Dni}", "PanelAsistente");

                var pacienteCompleto = _pacienteService.ObtenerPorDni(pacienteSeleccionado.Dni);
                if (pacienteCompleto == null)
                {
                    throw new SalcBusinessException("No se encontró información completa del paciente.");
                }

                // Abrir ventana modal con historial completo
                using (var frmHistorial = new SALC.Views.PanelAsistente.FrmHistorialPaciente(pacienteCompleto))
                {
                    frmHistorial.ShowDialog();
                }

                ExceptionHandler.LogInfo($"Historial consultado - DNI: {pacienteSeleccionado.Dni}", "PanelAsistente");
            }
            catch (SalcValidacionException valEx)
            {
                _view.MostrarMensaje(valEx.UserFriendlyMessage, true);
            }
            catch (SalcBusinessException bizEx)
            {
                _view.MostrarMensaje(bizEx.UserFriendlyMessage, true);
            }
            catch (SalcException salcEx)
            {
                _view.MostrarMensaje(salcEx.UserFriendlyMessage, true);
            }
            catch (Exception ex)
            {
                var mensaje = ExceptionHandler.ManejarExcepcion(ex, "VerHistorialAsistente", mostrarAlUsuario: false);
                _view.MostrarMensaje(mensaje, true);
            }
        }
    }
}
