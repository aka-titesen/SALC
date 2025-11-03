using SALC.BLL;
using SALC.DAL;
using SALC.Domain;
using SALC.Presenters.ViewsContracts;
using System;
using SALC.EmailService;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SALC.Presenters
{
    public class InformesVerificadosPresenter
    {
        private readonly IInformesVerificadosView _view;
        private readonly EmailServicio _emailService;
        private readonly IAnalisisService _analisisService = new AnalisisService();
        private readonly IPacienteService _pacienteService = new PacienteService();
        private readonly ICatalogoService _catalogoService = new CatalogoService();
        private readonly UsuarioRepositorio _usuarioRepo = new UsuarioRepositorio();
        private readonly IInformeService _informeService = new InformeService();

        public InformesVerificadosPresenter(IInformesVerificadosView view)
        {
            _view = view;
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
                CargarMedicosConAnalisis();
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
                _view.MostrarMensaje($"Error al cargar médicos: {ex.Message}", true);
            }
        }

        private void OnBuscarAnalisis(object sender, EventArgs e)
        {
            try
            {
                var fechaDesde = _view.FechaDesde;
                var fechaHasta = _view.FechaHasta.AddDays(1);
                var medicoId = _view.MedicoSeleccionadoId;
                var textoBusqueda = _view.TextoBusquedaPaciente;

                var analisisVerificados = _analisisService.ObtenerAnalisisActivos()
                    .Where(a => a.IdEstado == 2)
                    .Where(a => a.FechaFirma.HasValue)
                    .Where(a => a.FechaFirma.Value >= fechaDesde && a.FechaFirma.Value < fechaHasta)
                    .ToList();

                if (medicoId.HasValue)
                {
                    analisisVerificados = analisisVerificados.Where(a => a.DniFirma == medicoId.Value).ToList();
                }

                var analisisCompletos = new List<object>();

                foreach (var analisis in analisisVerificados)
                {
                    try
                    {
                        var paciente = _pacienteService.ObtenerPorDni(analisis.DniPaciente);
                        var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);
                        var medicoFirma = _usuarioRepo.ObtenerPorId(analisis.DniFirma ?? 0);

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
                            MedicoFirma = medicoFirma != null ? $"Dr. {medicoFirma.Nombre} {medicoFirma.Apellido}" : "Médico no encontrado",
                            EmailPaciente = paciente.Email ?? "Sin email",
                            TelefonoPaciente = paciente.Telefono ?? "Sin teléfono",
                            AnalisisCompleto = analisis,
                            PacienteCompleto = paciente
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error procesando análisis {analisis.IdAnalisis}: {ex.Message}");
                    }
                }

                analisisCompletos = analisisCompletos
                    .OrderByDescending(a => ((dynamic)a).AnalisisCompleto.FechaFirma)
                    .ToList();

                _view.CargarAnalisisVerificados(analisisCompletos);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al buscar análisis: {ex.Message}", true);
            }
        }

        private void OnLimpiarFiltros(object sender, EventArgs e)
        {
            _view.LimpiarFiltros();
            OnBuscarAnalisis(sender, e);
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
                System.Diagnostics.Debug.WriteLine($"Error en selección de análisis: {ex.Message}");
            }
        }

        private void OnGenerarPdf(object sender, EventArgs e)
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                if (analisisSeleccionado == null)
                {
                    _view.MostrarMensaje("Seleccione un análisis para generar el PDF.", true);
                    return;
                }

                var analisisCompleto = ((dynamic)analisisSeleccionado).AnalisisCompleto as Analisis;
                var pacienteCompleto = ((dynamic)analisisSeleccionado).PacienteCompleto as Paciente;
                var idAnalisis = analisisCompleto.IdAnalisis;

                if (analisisCompleto.IdEstado != 2)
                {
                    _view.MostrarMensaje("Solo se puede generar PDF para análisis verificados.", true);
                    return;
                }

                string rutaArchivo = _informeService.GenerarPdfDeAnalisis(idAnalisis);

                if (rutaArchivo != null)
                {
                    var tipoAnalisis = _catalogoService.ObtenerTiposAnalisis()
                        .FirstOrDefault(t => t.IdTipoAnalisis == analisisCompleto.IdTipoAnalisis);

                    _view.MostrarMensaje(
                        $"? Informe PDF generado exitosamente\n\n" +
                        $"Análisis ID: {idAnalisis}\n" +
                        $"Paciente: {pacienteCompleto.Nombre} {pacienteCompleto.Apellido}\n" +
                        $"Tipo: {tipoAnalisis?.Descripcion}\n" +
                        $"Fecha verificación: {analisisCompleto.FechaFirma:dd/MM/yyyy HH:mm}\n\n" +
                        $"?? Archivo guardado en:\n{rutaArchivo}");
                }
                else
                {
                    _view.MostrarMensaje("Generación de informe cancelada por el usuario.");
                }
            }
            catch (InvalidOperationException ex)
            {
                _view.MostrarMensaje($"? Error de validación: {ex.Message}", true);
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"? Error al generar PDF: {ex.Message}", true);
            }
        }

        private void OnEnviarEmail(object sender, EventArgs e)
        {
            try
            {
                var analisisSeleccionado = _view.AnalisisSeleccionado;
                if (analisisSeleccionado == null)
                {
                    _view.MostrarMensaje("Seleccione un análisis para enviar por email.", true);
                    return;
                }

                var pacienteCompleto = ((dynamic)analisisSeleccionado).PacienteCompleto as Paciente;
                var analisisCompleto = ((dynamic)analisisSeleccionado).AnalisisCompleto as Analisis;

                if (string.IsNullOrWhiteSpace(pacienteCompleto.Email))
                {
                    _view.MostrarMensaje("El paciente no tiene email registrado.", true);
                    return;
                }

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
                    openFileDialog.Title = "Seleccionar el informe PDF para enviar";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string rutaArchivoSeleccionado = openFileDialog.FileName;

                        // 2. Extraer el Email del Paciente del análisis seleccionado
                        var tipoAnalisis = analisisSeleccionado.GetType();
                        string emailPaciente = tipoAnalisis.GetProperty("EmailPaciente")?.GetValue(analisisSeleccionado) as string;

                        // Opcional: Obtener el ID para el mensaje de éxito
                        var idAnalisis = tipoAnalisis.GetProperty("IdAnalisis")?.GetValue(analisisSeleccionado);

                        if (string.IsNullOrWhiteSpace(emailPaciente))
                        {
                            _view.MostrarMensaje("El paciente seleccionado no tiene un correo electrónico válido.", true);
                            return;
                        }

                        // 3. Llamar al servicio de envío de correo
                        // Nota: La lógica del EmailService que proporcionaste ya maneja la adjunción.
                        bool exito = _emailService.EnviarInformePorCorreo(emailPaciente, rutaArchivoSeleccionado);

                        if (exito)
                        {
                            _view.MostrarMensaje($"Informe {idAnalisis} ({System.IO.Path.GetFileName(rutaArchivoSeleccionado)}) enviado con éxito a: {emailPaciente}", false);
                        }
                        // Si hay error, el EmailService debe mostrar el MessageBox.
                    }
                    else
                    {
                        // El usuario canceló la selección del archivo
                        _view.MostrarMensaje("Envío de correo cancelado por el usuario.", false);
                    }

                    

                    var observaciones = _view.ObservacionesEnvio;

                    _view.MostrarMensaje(
                        $"Informe enviado por email:\n\n" +
                        $"Destinatario: {pacienteCompleto.Nombre} {pacienteCompleto.Apellido}\n" +
                        $"Email: {pacienteCompleto.Email}\n" +
                        $"Análisis ID: {analisisCompleto.IdAnalisis}\n" +
                        $"Mensaje: {(string.IsNullOrWhiteSpace(observaciones) ? "Sin mensaje adicional" : observaciones)}\n\n" +
                        $"El email ha sido enviado exitosamente.\n" +
                        $"(Funcionalidad completa pendiente de implementación)");
                }
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
                    _view.MostrarMensaje("Seleccione un análisis para enviar por WhatsApp.", true);
                    return;
                }

                var pacienteCompleto = ((dynamic)analisisSeleccionado).PacienteCompleto as Paciente;
                var analisisCompleto = ((dynamic)analisisSeleccionado).AnalisisCompleto as Analisis;

                if (string.IsNullOrWhiteSpace(pacienteCompleto.Telefono))
                {
                    _view.MostrarMensaje("El paciente no tiene teléfono registrado.", true);
                    return;
                }

                var observaciones = _view.ObservacionesEnvio;

                _view.MostrarMensaje(
                    $"Informe enviado por WhatsApp:\n\n" +
                    $"Destinatario: {pacienteCompleto.Nombre} {pacienteCompleto.Apellido}\n" +
                    $"Teléfono: {pacienteCompleto.Telefono}\n" +
                    $"Análisis ID: {analisisCompleto.IdAnalisis}\n" +
                    $"Mensaje: {(string.IsNullOrWhiteSpace(observaciones) ? "Sin mensaje adicional" : observaciones)}\n\n" +
                    $"El mensaje ha sido enviado exitosamente.\n" +
                    $"(Funcionalidad completa pendiente de implementación)");
            }
            catch (Exception ex)
            {
                _view.MostrarMensaje($"Error al enviar WhatsApp: {ex.Message}", true);
            }
        }
    }
}
