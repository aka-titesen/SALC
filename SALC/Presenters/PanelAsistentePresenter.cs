using System;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;
using SALC.Infraestructura;
using SALC.Infraestructura.Exceptions;

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
            ExceptionHandler.EjecutarConManejo(() =>
            {
                ExceptionHandler.LogInfo("Inicializando vista de asistente", "PanelAsistente");
                CargarTodosLosPacientes();
            }, "InicializarVistaAsistente");
        }

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

        private void OnVerHistorial()
        {
            try
            {
                var pacienteSeleccionado = _view.PacienteSeleccionado;
                if (pacienteSeleccionado == null)
                {
                    throw new SalcValidacionException("Seleccione un paciente para ver su historial.", "paciente");
                }

                // Validar que el paciente tenga datos completos
                if (pacienteSeleccionado.Dni <= 0)
                {
                    throw new SalcValidacionException("El DNI del paciente no es válido.", "dni");
                }

                ExceptionHandler.LogInfo($"Abriendo historial de paciente - DNI: {pacienteSeleccionado.Dni}", "PanelAsistente");

                // Cargar paciente completo desde la base de datos
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
