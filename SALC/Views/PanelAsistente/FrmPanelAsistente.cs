using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Presenters.ViewsContracts;
using System.Collections.Generic;
using SALC.Domain;
using System.Linq;

namespace SALC.Views.PanelAsistente
{
    public class FrmPanelAsistente : Form, IPanelAsistenteView
    {
        // Controles principales
        private Panel panelSuperior;
        private Panel panelInferior;
        
        private GroupBox groupPacientes;
        private TextBox txtBusqueda;
        private Button btnBuscar;
        private DataGridView gridPacientes;
        private Button btnVerHistorial;
        
        private GroupBox groupHistorial;
        private DataGridView gridHistorial;
        
        private GroupBox groupAcciones;
        private Button btnGenerarPdf;
        private Button btnEnviarInforme;
        private Label lblInstrucciones;

        public FrmPanelAsistente()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            
            var presenter = Tag as Presenters.PanelAsistentePresenter;
            presenter?.InicializarVista();
        }

        private void InitializeComponent()
        {
            Text = "Consulta y Gestión de Historiales Clínicos";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // ============ PANEL SUPERIOR - BÚSQUEDA Y PACIENTES ============
            panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 420,
                BackColor = Color.White,
                Padding = new Padding(20, 20, 20, 10)
            };

            // Título principal
            var lblTitulo = new Label
            {
                Text = "Consulta de Pacientes y Historiales Médicos",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(0, 0),
                Size = new Size(800, 35),
                BackColor = Color.Transparent
            };

            // Subtítulo
            var lblSubtitulo = new Label
            {
                Text = "Acceda al historial completo de análisis clínicos de cualquier paciente del sistema",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(0, 35),
                Size = new Size(900, 25),
                BackColor = Color.Transparent
            };

            // GroupBox para búsqueda y lista de pacientes
            groupPacientes = new GroupBox
            {
                Text = "  Búsqueda y Selección de Pacientes  ",
                Location = new Point(0, 75),
                Size = new Size(1140, 325),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                BackColor = Color.FromArgb(245, 250, 255)
            };

            // Panel de búsqueda
            var panelBusqueda = new Panel
            {
                Location = new Point(15, 30),
                Size = new Size(1110, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblBusqueda = new Label
            {
                Text = "Buscar por DNI o Apellido:",
                Location = new Point(10, 14),
                Size = new Size(180, 22),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BackColor = Color.Transparent
            };

            txtBusqueda = new TextBox
            {
                Location = new Point(200, 12),
                Size = new Size(280, 25),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnBuscar = new Button
            {
                Text = "Buscar Paciente",
                Location = new Point(495, 10),
                Size = new Size(150, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;

            var lblInfo = new Label
            {
                Text = "Presione Enter o haga clic en Buscar para filtrar la lista",
                Location = new Point(660, 14),
                Size = new Size(420, 22),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166),
                BackColor = Color.Transparent
            };

            panelBusqueda.Controls.AddRange(new Control[] { lblBusqueda, txtBusqueda, btnBuscar, lblInfo });

            // Grid de pacientes
            gridPacientes = new DataGridView
            {
                Location = new Point(15, 95),
                Size = new Size(1110, 180),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 152, 219),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Padding = new Padding(5)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(209, 231, 248),
                    SelectionForeColor = Color.FromArgb(44, 62, 80)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 250, 255)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false
            };

            btnVerHistorial = new Button
            {
                Text = "Ver Historial del Paciente Seleccionado",
                Location = new Point(15, 285),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVerHistorial.FlatAppearance.BorderSize = 0;

            groupPacientes.Controls.AddRange(new Control[] { panelBusqueda, gridPacientes, btnVerHistorial });

            panelSuperior.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, groupPacientes });

            // ============ PANEL INFERIOR - HISTORIAL Y ACCIONES ============
            panelInferior = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20, 10, 20, 20)
            };

            // GroupBox para historial
            groupHistorial = new GroupBox
            {
                Text = "  Historial de Análisis Clínicos del Paciente  ",
                Location = new Point(0, 0),
                Size = new Size(1140, 230),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                BackColor = Color.FromArgb(248, 255, 250)
            };

            gridHistorial = new DataGridView
            {
                Location = new Point(15, 30),
                Size = new Size(1110, 185),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(39, 174, 96),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(212, 239, 223),
                    SelectionForeColor = Color.FromArgb(44, 62, 80)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 255, 250)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false
            };

            groupHistorial.Controls.Add(gridHistorial);

            // GroupBox para acciones
            groupAcciones = new GroupBox
            {
                Text = "  Generación y Envío de Informes  ",
                Location = new Point(0, 245),
                Size = new Size(1140, 120),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(142, 68, 173),
                BackColor = Color.FromArgb(250, 245, 255)
            };

            // Panel informativo
            var panelInfoAcciones = new Panel
            {
                Location = new Point(15, 30),
                Size = new Size(1110, 35),
                BackColor = Color.FromArgb(209, 231, 248),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblInstrucciones = new Label
            {
                Text = "Seleccione un análisis en estado 'Verificado' para habilitar la generación y envío de informes",
                Location = new Point(10, 8),
                Size = new Size(1080, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(21, 101, 192),
                BackColor = Color.Transparent
            };

            panelInfoAcciones.Controls.Add(lblInstrucciones);

            btnGenerarPdf = new Button
            {
                Text = "Generar Informe PDF",
                Location = new Point(15, 75),
                Size = new Size(240, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnGenerarPdf.FlatAppearance.BorderSize = 0;

            btnEnviarInforme = new Button
            {
                Text = "Enviar Informe al Paciente",
                Location = new Point(270, 75),
                Size = new Size(240, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(142, 68, 173),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnEnviarInforme.FlatAppearance.BorderSize = 0;

            var lblNota = new Label
            {
                Text = "Nota: Solo puede generar y enviar informes de análisis previamente verificados por un médico",
                Location = new Point(525, 82),
                Size = new Size(580, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166),
                BackColor = Color.Transparent
            };

            groupAcciones.Controls.AddRange(new Control[] { 
                panelInfoAcciones, btnGenerarPdf, btnEnviarInforme, lblNota 
            });

            panelInferior.Controls.AddRange(new Control[] { groupHistorial, groupAcciones });

            // Agregar paneles al formulario
            Controls.AddRange(new Control[] { panelInferior, panelSuperior });

            // ============ EVENTOS ============
            btnBuscar.Click += (s, e) => BuscarPacientesClick?.Invoke(this, EventArgs.Empty);
            btnVerHistorial.Click += (s, e) => VerHistorialClick?.Invoke(this, EventArgs.Empty);
            btnGenerarPdf.Click += (s, e) => GenerarPdfClick?.Invoke(this, EventArgs.Empty);
            btnEnviarInforme.Click += (s, e) => EnviarInformeClick?.Invoke(this, EventArgs.Empty);
            
            gridHistorial.SelectionChanged += (s, e) => {
                if (gridHistorial.SelectedRows.Count > 0)
                {
                    var presenter = Tag as Presenters.PanelAsistentePresenter;
                    presenter?.SeleccionAnalisisCambiada();
                }
            };

            txtBusqueda.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BuscarPacientesClick?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        // Implementación de IPanelAsistenteView
        public event EventHandler BuscarPacientesClick;
        public event EventHandler VerHistorialClick;
        public event EventHandler GenerarPdfClick;
        public event EventHandler EnviarInformeClick;

        public string BusquedaPacienteTexto => txtBusqueda?.Text?.Trim();

        public void CargarListaPacientes(IEnumerable<Paciente> pacientes)
        {
            if (gridPacientes == null) return;

            var lista = pacientes.Select(p => new
            {
                DNI = p.Dni,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                FechaNacimiento = p.FechaNac.ToString("dd/MM/yyyy"),
                Sexo = p.Sexo,
                Email = p.Email ?? "Sin email",
                Telefono = p.Telefono ?? "Sin teléfono",
                ObraSocial = p.IdObraSocial?.ToString() ?? "Sin obra social"
            }).ToList();

            gridPacientes.DataSource = lista;
            
            if (gridPacientes.Columns.Count > 0)
            {
                gridPacientes.Columns["DNI"].Width = 80;
                gridPacientes.Columns["Nombre"].Width = 100;
                gridPacientes.Columns["Apellido"].Width = 100;
                gridPacientes.Columns["FechaNacimiento"].Width = 100;
                gridPacientes.Columns["Sexo"].Width = 50;
            }
        }

        public Paciente PacienteSeleccionado
        {
            get
            {
                if (gridPacientes?.SelectedRows.Count == 0)
                    return null;

                var row = gridPacientes.SelectedRows[0];
                return new Paciente
                {
                    Dni = (int)row.Cells["DNI"].Value,
                    Nombre = row.Cells["Nombre"].Value.ToString(),
                    Apellido = row.Cells["Apellido"].Value.ToString(),
                    Email = row.Cells["Email"].Value.ToString(),
                    Telefono = row.Cells["Telefono"].Value.ToString()
                };
            }
        }

        public void CargarHistorialAnalisis(IEnumerable<object> analisisConEstados)
        {
            if (gridHistorial == null) return;

            gridHistorial.DataSource = analisisConEstados.ToList();
            
            if (gridHistorial.Columns.Count > 0)
            {
                gridHistorial.Columns["IdAnalisis"].Width = 60;
                gridHistorial.Columns["TipoAnalisis"].Width = 150;
                gridHistorial.Columns["Estado"].Width = 80;
                gridHistorial.Columns["FechaCreacion"].Width = 120;
                gridHistorial.Columns["FechaFirma"].Width = 120;
                
                // Colorear filas según el estado con colores pasteles
                foreach (DataGridViewRow row in gridHistorial.Rows)
                {
                    var estado = row.Cells["Estado"].Value?.ToString();
                    switch (estado)
                    {
                        case "Verificado":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(212, 239, 223); // Verde pastel
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(27, 94, 32);
                            break;
                        case "Sin verificar":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224); // Naranja pastel
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(230, 81, 0);
                            break;
                        case "Anulado":
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238); // Rojo pastel
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                            break;
                    }
                }
            }
        }

        public object AnalisisSeleccionado
        {
            get
            {
                if (gridHistorial?.SelectedRows.Count == 0)
                    return null;

                var row = gridHistorial.SelectedRows[0];
                return new
                {
                    IdAnalisis = (int)row.Cells["IdAnalisis"].Value,
                    TipoAnalisis = row.Cells["TipoAnalisis"].Value.ToString(),
                    Estado = row.Cells["Estado"].Value.ToString(),
                    FechaCreacion = row.Cells["FechaCreacion"].Value.ToString(),
                    FechaFirma = row.Cells["FechaFirma"].Value.ToString(),
                    MedicoCarga = row.Cells["MedicoCarga"].Value.ToString(),
                    MedicoFirma = row.Cells["MedicoFirma"].Value.ToString(),
                    Observaciones = row.Cells["Observaciones"].Value.ToString()
                };
            }
        }

        public void HabilitarAccionesAnalisis(bool habilitar)
        {
            if (btnGenerarPdf != null) btnGenerarPdf.Enabled = habilitar;
            if (btnEnviarInforme != null) btnEnviarInforme.Enabled = habilitar;
            
            if (lblInstrucciones != null)
            {
                if (habilitar)
                {
                    lblInstrucciones.Text = "Análisis verificado seleccionado - Puede generar el informe PDF y enviarlo al paciente por email";
                    lblInstrucciones.ForeColor = Color.FromArgb(27, 94, 32); // Verde oscuro
                }
                else
                {
                    lblInstrucciones.Text = "Seleccione un análisis en estado 'Verificado' para habilitar la generación y envío de informes";
                    lblInstrucciones.ForeColor = Color.FromArgb(21, 101, 192); // Azul
                }
            }
        }

        public void MostrarMensaje(string texto, bool esError = false)
        {
            var icon = esError ? MessageBoxIcon.Error : MessageBoxIcon.Information;
            var titulo = esError ? "SALC - Error en Panel Asistente" : "SALC - Información";
            MessageBox.Show(this, texto, titulo, MessageBoxButtons.OK, icon);
        }
    }
}
