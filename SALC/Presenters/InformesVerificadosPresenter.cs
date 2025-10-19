using System;
using System.Collections.Generic;
using System.Linq;
using SALC.Presenters.ViewsContracts;
using SALC.BLL;
using SALC.Domain;
using SALC.DAL;

namespace SALC.Presenters
{
    public class InformesVerificadosPresenter
    {
        private readonly IInformesVerificadosView _view;
        private readonly IAnalisisService _analisisService = new AnalisisService();
        private readonly IPacienteService _pacienteService = new PacienteService();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly UsuarioRepositorio _usuarioRepo = new UsuarioRepositorio();

        public InformesVerificadosPresenter(IInformesVerificadosView view)
        {
            _view = view;

            // Conectar eventos
            _view.BuscarAnalisisClick += OnBuscarAnalisis;
            _view.LimpiarFiltrosClick += OnLimpiarFiltros;
            _view.GenerarPdfClick += OnGenerarPdf;
            _view.EnviarEmailClick += OnEnviarEmail;
            _view.EnviarWhatsAppClick += OnEnviarWhatsApp;
            _view.AnalisisSeleccionCambiada += OnAnalisisSeleccionCambiada;
        }

        public void InicializarVista()
        {
            try
            {
                // Cargar m�dicos que han firmado an�lisis
                CargarMedicosConAnalisis();
                
                // Cargar an�lisis verificados del �ltimo mes por defecto
                OnBuscarAnalisis(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al inicializar vista: {ex.Message}", true);
            }
        }

        private void CargarMedicosConAnalisis()
        {
            try
            {
                // Obtener todos los m�dicos que han firmado al menos un an�lisis
                var medicosConAnalisis = _usuarioRepo.ObtenerMedicosConAnalisisFirmados()
                    .Select(m => new
                    {
                        Dni = (int?)m.Dni,
                        NombreCompleto = $"Dr. {m.Nombre} {m.Apellido}"
                    })
                    .ToList();

                _view.CargarMedicos(medicosConAnalisis);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al cargar m�dicos: {ex.Message}", true);
            }
        }

        private void OnBuscarAnalisis(object sender, EventArgs e)
        {
            try
            {
                // Obtener filtros de la vista
                var fechaDesde = _view.FechaDesde;
                var fechaHasta = _view.FechaHasta.AddDays(1); // Incluir todo el d�a
                var medicoId = _view.MedicoSeleccionadoId;
                var textoBusqueda = _view.TextoBusquedaPaciente;

                // Obtener todos los an�lisis verificados
                var analisisVerificados = _analisisService.ObtenerAnalisisActivos()
                    .Where(a => a.IdEstado == 2) // Solo verificados
                    .Where(a => a.FechaFirma.HasValue) // Que tengan fecha de firma
                    .Where(a => a.FechaFirma.Value >= fechaDesde && a.FechaFirma.Value < fechaHasta)
                    .ToList();

                // Filtrar por m�dico si se seleccion� uno espec�fico
                if (medicoId.HasValue)
                {
                    analisisVerificados = analisisVerificados.Where(a => a.DniFirma == medicoId.Value).ToList();
                }

                // Crear objetos completos para mostrar en el grid
                var analisisCompletos = new List<object>();

                foreach (var analisis in analisisVerificados)
                {
                    try
                    {
                        var paciente = _pacienteService.ObtenerPorDni(analisis.DniPaciente);
                        var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);
                        var medicoFirma = _usuarioRepo.ObtenerPorId(analisis.DniFirma ?? 0);

                        // Filtrar por texto de b�squeda de paciente si se especific�
                        if (!string.IsNullOrEmpty(textoBusqueda))
                        {
                            bool coincide = false;
                            if (int.TryParse(textoBusqueda, out int dni))
                            {
                                coincide = paciente.Dni.ToString().Contains(textoBusqueda);
                            }
                            else
                            {
                                coincide = paciente.Apellido.ToLower().Contains(textoBusqueda.ToLower()) ||
                                          paciente.Nombre.ToLower().Contains(textoBusqueda.ToLower());
                            }

                            if (!coincide) continue;
                        }

                        analisisCompletos.Add(new
                        {
                            IdAnalisis = analisis.IdAnalisis,
                            DniPaciente = paciente.Dni,
                            PacienteNombre = $"{paciente.Nombre} {paciente.Apellido}",
                            TipoAnalisis = tipoAnalisis?.Descripcion ?? "Tipo no encontrado",
                            FechaCreacion = analisis.FechaCreacion.ToString("dd/MM/yyyy"),
                            FechaFirma = analisis.FechaFirma?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                            MedicoFirma = medicoFirma != null ? $"Dr. {medicoFirma.Nombre} {medicoFirma.Apellido}" : "M�dico no encontrado",
                            EmailPaciente = paciente.Email ?? "Sin email",
                            TelefonoPaciente = paciente.Telefono ?? "Sin tel�fono",
                            // Objetos completos para uso posterior
                            AnalisisCompleto = analisis,
                            PacienteCompleto = paciente
                        });
                    }
                    catch (Exception ex)
                    {
                        // Si hay error con un an�lisis espec�fico, continuar con los dem�s
                        System.Diagnostics.Debug.WriteLine($"Error procesando an�lisis {analisis.IdAnalisis}: {ex.Message}");
                    }
                }

                // Ordenar por fecha de firma descendente (m�s recientes primero)
                analisisCompletos = analisisCompletos
                    .OrderByDescending(a => ((dynamic)a).AnalisisCompleto.FechaFirma)
                    .ToList();

                _view.CargarAnalisisVerificados(analisisCompletos);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al buscar an�lisis: {ex.Message}", true);
            }
        }

        private void OnLimpiarFiltros(object sender, EventArgs e)
        {
            _view.LimpiarFiltros();
            OnBuscarAnalisis(sender, e); // Buscar nuevamente con filtros limpios
        }

        private void OnAnalisisSeleccionCambiada(object sender, EventArgs e)
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                _view.HabilitarAcciones(analisisSeleccionado != null);
            }
            catch (Exception ex)
            {
                _view.HabilitarAcciones(false);
                System.Diagnostics.Debug.WriteLine($"Error en selecci�n de an�lisis: {ex.Message}");
            }
        }

        private void OnGenerarPdf(object sender, EventArgs e)
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                if (analisisSeleccionado == null)
                {
                    _view.MostrarMensaje("Seleccione un an�lisis para generar el PDF.", true);
                    return;
                }

                // Obtener datos del an�lisis
                var analisisCompleto = ((dynamic)analisisSeleccionado).AnalisisCompleto as Analisis;
                var pacienteCompleto = ((dynamic)analisisSeleccionado).PacienteCompleto as Paciente;
                var idAnalisis = analisisCompleto.IdAnalisis;

                // Validar que est� verificado
                if (analisisCompleto.IdEstado != 2)
                {
                    _view.MostrarMensaje("Solo se puede generar PDF para an�lisis verificados.", true);
                    return;
                }

                // TODO: Implementar generaci�n real de PDF
                // Por ahora simulamos la funcionalidad
                var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                    .FirstOrDefault(t => t.IdTipoAnalisis == analisisCompleto.IdTipoAnalisis);

                _view.MostrarMensaje(
                    $"PDF generado exitosamente:\n\n" +
                    $"An�lisis ID: {idAnalisis}\n" +
                    $"Paciente: {pacienteCompleto.Nombre} {pacienteCompleto.Apellido}\n" +
                    $"Tipo: {tipoAnalisis?.Descripcion}\n" +
                    $"Fecha verificaci�n: {analisisCompleto.FechaFirma:dd/MM/yyyy HH:mm}\n\n" +
                    $"El archivo PDF se ha guardado en la carpeta de informes.\n" +
                    $"(Funcionalidad completa pendiente de implementaci�n)");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al generar PDF: {ex.Message}", true);
            }
        }

        private void OnEnviarEmail(object sender, EventArgs e)
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                if (analisisSeleccionado == null)
                {
                    _view.MostrarMensaje("Seleccione un an�lisis para enviar por email.", true);
                    return;
                }

                var pacienteCompleto = ((dynamic)analisisSeleccionado).PacienteCompleto as Paciente;
                var analisisCompleto = ((dynamic)analisisSeleccionado).AnalisisCompleto as Analisis;

                if (string.IsNullOrWhiteSpace(pacienteCompleto.Email))
                {
                    _view.MostrarMensaje("El paciente no tiene email registrado.", true);
                    return;
                }

                var observaciones = _view.ObservacionesEnvio;

                // TODO: Implementar env�o real por email
                _view.MostrarMensaje(
                    $"Informe enviado por email:\n\n" +
                    $"Destinatario: {pacienteCompleto.Nombre} {pacienteCompleto.Apellido}\n" +
                    $"Email: {pacienteCompleto.Email}\n" +
                    $"An�lisis ID: {analisisCompleto.IdAnalisis}\n" +
                    $"Mensaje: {(string.IsNullOrWhiteSpace(observaciones) ? "Sin mensaje adicional" : observaciones)}\n\n" +
                    $"El email ha sido enviado exitosamente.\n" +
                    $"(Funcionalidad completa pendiente de implementaci�n)");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al enviar email: {ex.Message}", true);
            }
        }

        private void OnEnviarWhatsApp(object sender, EventArgs e)
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                if (analisisSeleccionado == null)
                {
                    _view.MostrarMensaje("Seleccione un an�lisis para enviar por WhatsApp.", true);
                    return;
                }

                var pacienteCompleto = ((dynamic)analisisSeleccionado).PacienteCompleto as Paciente;
                var analisisCompleto = ((dynamic)analisisSeleccionado).AnalisisCompleto as Analisis;

                if (string.IsNullOrWhiteSpace(pacienteCompleto.Telefono))
                {
                    _view.MostrarMensaje("El paciente no tiene tel�fono registrado.", true);
                    return;
                }

                var observaciones = _view.ObservacionesEnvio;

                // TODO: Implementar env�o real por WhatsApp
                _view.MostrarMensaje(
                    $"Informe enviado por WhatsApp:\n\n" +
                    $"Destinatario: {pacienteCompleto.Nombre} {pacienteCompleto.Apellido}\n" +
                    $"Tel�fono: {pacienteCompleto.Telefono}\n" +
                    $"An�lisis ID: {analisisCompleto.IdAnalisis}\n" +
                    $"Mensaje: {(string.IsNullOrWhiteSpace(observaciones) ? "Sin mensaje adicional" : observaciones)}\n\n" +
                    $"El mensaje ha sido enviado exitosamente.\n" +
                    $"(Funcionalidad completa pendiente de implementaci�n)");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al enviar WhatsApp: {ex.Message}", true);
            }
        }
    }
}