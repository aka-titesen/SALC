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

        public PanelAsistentePresenter(IPanelAsistenteView view)
        {
            _view = view;
            _view.BuscarPacientesClick += (s, e) => OnBuscarPacientes();
            _view.VerHistorialClick += (s, e) => OnVerHistorial();
            _view.GenerarPdfClick += (s, e) => OnGenerarPdf();
            _view.EnviarInformeClick += (s, e) => OnEnviarInforme();
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
                _view.MostrarMensaje($"Error al cargar pacientes: {ex.Message}", true);
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
                _view.MostrarMensaje($"Error en la búsqueda: {ex.Message}", true);
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

                // Obtener TODOS los análisis del paciente (incluyendo anulados)
                var analisis = _analisisService.ObtenerAnalisisPorPaciente(pacienteSeleccionado.Dni);
                
                // Crear objetos con información completa para mostrar en el grid
                var analisisCompletos = analisis.Select(a => new
                {
                    IdAnalisis = a.IdAnalisis,
                    TipoAnalisis = ObtenerDescripcionTipoAnalisis(a.IdTipoAnalisis),
                    Estado = ObtenerDescripcionEstado(a.IdEstado),
                    FechaCreacion = a.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                    FechaFirma = a.FechaFirma?.ToString("dd/MM/yyyy HH:mm") ?? "No firmado",
                    MedicoCarga = ObtenerNombreMedico(a.DniCarga),
                    MedicoFirma = a.DniFirma.HasValue ? ObtenerNombreMedico(a.DniFirma.Value) : "Sin firmar",
                    Observaciones = a.Observaciones ?? ""
                }).ToList();

                _view.CargarHistorialAnalisis(analisisCompletos);
                _view.HabilitarAccionesAnalisis(false); // Inicialmente deshabilitado
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al obtener historial: {ex.Message}", true);
            }
        }

        private void OnGenerarPdf()
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                if (analisisSeleccionado == null)
                {
                    _view.MostrarMensaje("Seleccione un análisis para generar el PDF.", true);
                    return;
                }

                // Obtener el ID del análisis desde el objeto seleccionado
                var idAnalisis = (int)analisisSeleccionado.GetType().GetProperty("IdAnalisis").GetValue(analisisSeleccionado);
                
                var analisis = _analisisService.ObtenerAnalisisPorId(idAnalisis);
                if (analisis == null)
                {
                    _view.MostrarMensaje("Análisis no encontrado.", true);
                    return;
                }

                if (analisis.IdEstado != 2) // 2 = Verificado
                {
                    _view.MostrarMensaje("Solo se puede generar PDF para análisis verificados.", true);
                    return;
                }

                // TODO: Implementar generación real de PDF
                _view.MostrarMensaje($"PDF generado exitosamente para el análisis {idAnalisis}.\n(Funcionalidad completa pendiente de implementación)");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al generar PDF: {ex.Message}", true);
            }
        }

        private void OnEnviarInforme()
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                if (analisisSeleccionado == null)
                {
                    _view.MostrarMensaje("Seleccione un análisis para enviar.", true);
                    return;
                }

                var idAnalisis = (int)analisisSeleccionado.GetType().GetProperty("IdAnalisis").GetValue(analisisSeleccionado);
                
                var analisis = _analisisService.ObtenerAnalisisPorId(idAnalisis);
                if (analisis == null)
                {
                    _view.MostrarMensaje("Análisis no encontrado.", true);
                    return;
                }

                if (analisis.IdEstado != 2) // 2 = Verificado
                {
                    _view.MostrarMensaje("Solo se puede enviar informes de análisis verificados.", true);
                    return;
                }

                // TODO: Implementar envío real por email/teléfono
                var paciente = _pacienteService.ObtenerPorDni(analisis.DniPaciente);
                _view.MostrarMensaje($"Informe enviado exitosamente a {paciente.Nombre} {paciente.Apellido}.\nEmail: {paciente.Email}\nTeléfono: {paciente.Telefono}\n(Funcionalidad completa pendiente de implementación)");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al enviar informe: {ex.Message}", true);
            }
        }

        // Métodos auxiliares para obtener descripciones
        private string ObtenerDescripcionTipoAnalisis(int idTipo)
        {
            try
            {
                var tipo = _catalogoService.ObtenerTiposAnalisis()
                    .FirstOrDefault(t => t.IdTipoAnalisis == idTipo);
                return tipo?.Descripcion ?? $"Tipo {idTipo}";
            }
            catch
            {
                return $"Tipo {idTipo}";
            }
        }

        private string ObtenerDescripcionEstado(int idEstado)
        {
            switch (idEstado)
            {
                case 1: return "Sin verificar";
                case 2: return "Verificado";
                case 3: return "Anulado";
                default: return $"Estado {idEstado}";
            }
        }

        private string ObtenerNombreMedico(int dniMedico)
        {
            try
            {
                var usuarioRepo = new SALC.DAL.UsuarioRepositorio();
                var medico = usuarioRepo.ObtenerPorId(dniMedico);
                return medico != null ? $"Dr. {medico.Nombre} {medico.Apellido}" : $"Dr. {dniMedico}";
            }
            catch
            {
                return $"Dr. {dniMedico}";
            }
        }

        public void SeleccionAnalisisCambiada()
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                if (analisisSeleccionado == null)
                {
                    _view.HabilitarAccionesAnalisis(false);
                    return;
                }

                var estado = analisisSeleccionado.GetType().GetProperty("Estado").GetValue(analisisSeleccionado).ToString();
                
                // Solo habilitar acciones para análisis verificados
                bool esVerificado = estado == "Verificado";
                _view.HabilitarAccionesAnalisis(esVerificado);
            }
            catch
            {
                _view.HabilitarAccionesAnalisis(false);
            }
        }
    }
}
