using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SALC.Domain;
using SALC.BLL;

namespace SALC.Views.PanelAsistente
{
    /// <summary>
    /// Ventana modal que muestra el historial completo de análisis de un paciente
    /// </summary>
    public class FrmHistorialPaciente : Form
    {
        private readonly Paciente _paciente;
        private readonly IAnalisisService _analisisService;
        private readonly ICatalogoService _catalogoService;
        private readonly IInformeService _informeService;
        private readonly IEmailService _emailService;

        private Label lblInfoPaciente;
        private DataGridView gridHistorial;
        private Button btnVerDetalle;
        private Button btnGenerarPdf;
        private Button btnEnviarEmail;
        private Button btnCerrar;

        private List<AnalisisViewModel> _analisisCompletos;

        public FrmHistorialPaciente(Paciente paciente)
        {
            _paciente = paciente ?? throw new ArgumentNullException(nameof(paciente));
            _analisisService = new AnalisisService();
            _catalogoService = new CatalogoService();
            _informeService = new InformeService();
            _emailService = new EmailService();

            InitializeComponent();
            CargarHistorial();
        }

        private void InitializeComponent()
        {
            Text = string.Format("Historial Clínico - {0} {1}", _paciente.Nombre, _paciente.Apellido);
            Size = new Size(1400, 800);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = false;

            // Panel principal
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(30)
            };

            // Título
            var lblTitulo = new Label
            {
                Text = "Historial Clínico Completo",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(0, 0),
                Size = new Size(500, 35)
            };

            // Información del paciente
            lblInfoPaciente = new Label
            {
                Text = ObtenerInfoPaciente(),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(0, 45),
                Size = new Size(1320, 45),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15, 10, 15, 10)
            };

            // Grid de análisis - EXTENDIDO HORIZONTALMENTE
            gridHistorial = new DataGridView
            {
                Location = new Point(0, 110),
                Size = new Size(1320, 520),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(39, 174, 96),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(212, 239, 223),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(6)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 255, 250)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 }
            };

            gridHistorial.SelectionChanged += OnSeleccionCambiada;
            gridHistorial.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                    btnVerDetalle.PerformClick();
            };

            // Panel de botones
            var panelBotones = new Panel
            {
                Location = new Point(0, 645),
                Size = new Size(1320, 80),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnVerDetalle = new Button
            {
                Text = "Ver Detalle del Análisis",
                Location = new Point(15, 20),
                Size = new Size(220, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnVerDetalle.FlatAppearance.BorderSize = 0;
            btnVerDetalle.Click += OnVerDetalle;

            btnGenerarPdf = new Button
            {
                Text = "Generar Informe PDF",
                Location = new Point(250, 20),
                Size = new Size(220, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnGenerarPdf.FlatAppearance.BorderSize = 0;
            btnGenerarPdf.Click += OnGenerarPdf;

            btnEnviarEmail = new Button
            {
                Text = "Enviar por Email",
                Location = new Point(485, 20),
                Size = new Size(220, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(142, 68, 173),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnEnviarEmail.FlatAppearance.BorderSize = 0;
            btnEnviarEmail.Click += OnEnviarEmail;

            var lblNota = new Label
            {
                Text = "Nota: Solo los análisis verificados permiten generación de informes",
                Location = new Point(720, 28),
                Size = new Size(480, 25),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166),
                BackColor = Color.Transparent
            };

            panelBotones.Controls.AddRange(new Control[] {
                btnVerDetalle, btnGenerarPdf, btnEnviarEmail, lblNota
            });

            btnCerrar = new Button
            {
                Text = "Cerrar",
                Location = new Point(1200, 20),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;

            panelBotones.Controls.Add(btnCerrar);

            panelPrincipal.Controls.AddRange(new Control[] {
                lblTitulo, lblInfoPaciente, gridHistorial, panelBotones
            });

            Controls.Add(panelPrincipal);
        }

        private string ObtenerInfoPaciente()
        {
            var edad = CalcularEdad(_paciente.FechaNac);
            var sexo = _paciente.Sexo == 'M' ? "Masculino" : _paciente.Sexo == 'F' ? "Femenino" : "Otro";
            
            return string.Format("DNI: {0}  |  Paciente: {1} {2}  |  Edad: {3} años (Nacimiento: {4})  |  Sexo: {5}  |  Email: {6}  |  Teléfono: {7}  |  Estado: {8}",
                _paciente.Dni,
                _paciente.Nombre,
                _paciente.Apellido,
                edad,
                _paciente.FechaNac.ToString("dd/MM/yyyy"),
                sexo,
                _paciente.Email ?? "Sin email",
                _paciente.Telefono ?? "Sin teléfono",
                _paciente.Estado);
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        private void CargarHistorial()
        {
            try
            {
                var analisis = _analisisService.ObtenerAnalisisPorPaciente(_paciente.Dni).ToList();
                
                _analisisCompletos = analisis.Select(a => new AnalisisViewModel
                {
                    IdAnalisis = a.IdAnalisis,
                    TipoAnalisis = ObtenerDescripcionTipoAnalisis(a.IdTipoAnalisis),
                    Estado = ObtenerDescripcionEstado(a.IdEstado),
                    FechaCreacion = a.FechaCreacion,
                    FechaFirma = a.FechaFirma,
                    MedicoCarga = ObtenerNombreMedico(a.DniCarga),
                    MedicoFirma = a.DniFirma.HasValue ? ObtenerNombreMedico(a.DniFirma.Value) : "-",
                    Observaciones = string.IsNullOrWhiteSpace(a.Observaciones) ? "-" : a.Observaciones,
                    IdEstado = a.IdEstado,
                    Analisis = a
                }).ToList();

                var datosGrid = _analisisCompletos.Select(a => new
                {
                    ID = a.IdAnalisis,
                    Tipo = a.TipoAnalisis,
                    Estado = a.Estado,
                    Creado = a.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                    Firmado = a.FechaFirma.HasValue ? a.FechaFirma.Value.ToString("dd/MM/yyyy HH:mm") : "-",
                    Médico = a.MedicoCarga,
                    Firmante = a.MedicoFirma,
                    Observaciones = a.Observaciones
                }).ToList();

                gridHistorial.DataSource = datosGrid;
                
                if (gridHistorial.Columns.Count > 0)
                {
                    gridHistorial.Columns["ID"].Width = 60;
                    gridHistorial.Columns["Tipo"].Width = 200;
                    gridHistorial.Columns["Estado"].Width = 100;
                    gridHistorial.Columns["Creado"].Width = 140;
                    gridHistorial.Columns["Firmado"].Width = 140;
                    gridHistorial.Columns["Médico"].Width = 180;
                    gridHistorial.Columns["Firmante"].Width = 180;
                    gridHistorial.Columns["Observaciones"].Width = 280;
                    
                    // Colorear filas según estado
                    foreach (DataGridViewRow row in gridHistorial.Rows)
                    {
                        var estado = row.Cells["Estado"].Value?.ToString();
                        switch (estado)
                        {
                            case "Verificado":
                                row.DefaultCellStyle.BackColor = Color.FromArgb(212, 239, 223);
                                row.DefaultCellStyle.ForeColor = Color.FromArgb(27, 94, 32);
                                break;
                            case "Sin verificar":
                                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
                                row.DefaultCellStyle.ForeColor = Color.FromArgb(230, 81, 0);
                                break;
                            case "Anulado":
                                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                                row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                                break;
                        }
                    }
                }

                lblInfoPaciente.Text = ObtenerInfoPaciente() + 
                    string.Format("  |  Total de análisis: {0}", analisis.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al cargar historial: {0}", ex.Message), 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSeleccionCambiada(object sender, EventArgs e)
        {
            if (gridHistorial.SelectedRows.Count == 0)
            {
                btnVerDetalle.Enabled = false;
                btnGenerarPdf.Enabled = false;
                btnEnviarEmail.Enabled = false;
                return;
            }

            var row = gridHistorial.SelectedRows[0];
            var idAnalisis = (int)row.Cells["ID"].Value;
            var analisis = _analisisCompletos.FirstOrDefault(a => a.IdAnalisis == idAnalisis);

            btnVerDetalle.Enabled = true;
            
            // Solo habilitar acciones para análisis verificados
            bool esVerificado = analisis != null && analisis.IdEstado == 2;
            btnGenerarPdf.Enabled = esVerificado;
            btnEnviarEmail.Enabled = esVerificado && !string.IsNullOrWhiteSpace(_paciente.Email);
        }

        private void OnVerDetalle(object sender, EventArgs e)
        {
            if (gridHistorial.SelectedRows.Count == 0) return;

            try
            {
                var row = gridHistorial.SelectedRows[0];
                var idAnalisis = (int)row.Cells["ID"].Value;
                var analisis = _analisisCompletos.FirstOrDefault(a => a.IdAnalisis == idAnalisis);

                if (analisis != null)
                {
                    using (var frmDetalle = new FrmDetalleAnalisis(analisis.Analisis, _paciente))
                    {
                        frmDetalle.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al abrir detalle: {0}", ex.Message), 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnGenerarPdf(object sender, EventArgs e)
        {
            if (gridHistorial.SelectedRows.Count == 0) return;

            try
            {
                var row = gridHistorial.SelectedRows[0];
                var idAnalisis = (int)row.Cells["ID"].Value;
                
                string rutaArchivo = _informeService.GenerarPdfDeAnalisis(idAnalisis);
                
                if (rutaArchivo != null)
                {
                    MessageBox.Show(
                        string.Format("Informe PDF generado exitosamente.\n\nUbicación:\n{0}", rutaArchivo),
                        "PDF Generado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al generar PDF: {0}", ex.Message), 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnEnviarEmail(object sender, EventArgs e)
        {
            if (gridHistorial.SelectedRows.Count == 0) return;

            try
            {
                var row = gridHistorial.SelectedRows[0];
                var idAnalisis = (int)row.Cells["ID"].Value;
                var analisis = _analisisCompletos.FirstOrDefault(a => a.IdAnalisis == idAnalisis);

                if (analisis == null) return;

                var confirmacion = MessageBox.Show(
                    string.Format("¿Enviar informe al email del paciente?\n\nDestinatario: {0}\nAnálisis: {1}",
                        _paciente.Email,
                        analisis.TipoAnalisis),
                    "Confirmar Envío",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes) return;

                // Generar PDF temporal
                string rutaTemporal = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    string.Format("Informe_{0}_{1}_DNI{2}_{3}.pdf", 
                        _paciente.Apellido, 
                        _paciente.Nombre, 
                        _paciente.Dni, 
                        DateTime.Now.ToString("yyyyMMdd_HHmmss")));

                string rutaPdf = ((InformeService)_informeService).GenerarPdfDeAnalisis(idAnalisis, rutaTemporal);
                
                if (string.IsNullOrEmpty(rutaPdf))
                {
                    MessageBox.Show("Error al generar el archivo PDF.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Enviar por email
                string nombreCompleto = string.Format("{0} {1}", _paciente.Nombre, _paciente.Apellido);
                bool enviado = _emailService.EnviarInformePorCorreo(
                    _paciente.Email, 
                    nombreCompleto, 
                    rutaPdf, 
                    analisis.TipoAnalisis
                );

                if (enviado)
                {
                    MessageBox.Show(
                        string.Format("Informe enviado exitosamente a:\n{0}", _paciente.Email),
                        "Email Enviado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Eliminar temporal
                    try
                    {
                        if (System.IO.File.Exists(rutaPdf))
                            System.IO.File.Delete(rutaPdf);
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo enviar el correo electrónico.\nVerifique la configuración SMTP.",
                        "Error de Envío",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al enviar email: {0}", ex.Message), 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Métodos auxiliares
        private string ObtenerDescripcionTipoAnalisis(int idTipo)
        {
            try
            {
                var tipo = _catalogoService.ObtenerTiposAnalisis()
                    .FirstOrDefault(t => t.IdTipoAnalisis == idTipo);
                return tipo != null ? tipo.Descripcion : string.Format("Tipo {0}", idTipo);
            }
            catch
            {
                return string.Format("Tipo {0}", idTipo);
            }
        }

        private string ObtenerDescripcionEstado(int idEstado)
        {
            switch (idEstado)
            {
                case 1: return "Sin verificar";
                case 2: return "Verificado";
                case 3: return "Anulado";
                default: return string.Format("Estado {0}", idEstado);
            }
        }

        private string ObtenerNombreMedico(int dniMedico)
        {
            try
            {
                var usuarioRepo = new SALC.DAL.UsuarioRepositorio();
                var medico = usuarioRepo.ObtenerPorId(dniMedico);
                return medico != null ? string.Format("Dr. {0} {1}", medico.Nombre, medico.Apellido) : string.Format("Dr. {0}", dniMedico);
            }
            catch
            {
                return string.Format("Dr. {0}", dniMedico);
            }
        }

        // Clase auxiliar para el ViewModel
        private class AnalisisViewModel
        {
            public int IdAnalisis { get; set; }
            public string TipoAnalisis { get; set; }
            public string Estado { get; set; }
            public DateTime FechaCreacion { get; set; }
            public DateTime? FechaFirma { get; set; }
            public string MedicoCarga { get; set; }
            public string MedicoFirma { get; set; }
            public string Observaciones { get; set; }
            public int IdEstado { get; set; }
            public Analisis Analisis { get; set; }
        }
    }
}
