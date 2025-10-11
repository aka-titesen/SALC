using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Views.Interfaces;
using SALC.Common;
using SALC.Presenters;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para gestión de reportes de análisis según ERS v2.7
    /// Implementa RF-08: Generar y Enviar Informe y RF-09: Visualizar Historial
    /// </summary>
    public partial class ReportesAnalisisForm : Form, IVistaReportes
    {
        // Eventos de IVistaReportes (español)
        public event EventHandler CargarReportes;
        public event EventHandler<string> BuscarReportes;
        public event EventHandler<FiltroReporte> FiltrarReportes;
        public event EventHandler<int> VerReporteCompleto;
        public event EventHandler<int> ExportarReportePdf;
        public event EventHandler<int> ExportarReporteCsv;

        // Propiedades de IVistaReportes
        public List<ReporteAnalisis> Reportes { get; set; } = new List<ReporteAnalisis>();
        public ReporteAnalisis ReporteSeleccionado { get; set; }
        public string RolUsuarioActual { get; set; }

        // Controles de la interfaz
        private Panel panelSuperior;
        private Panel panelContenido;
        private Panel panelLista;
        private Panel panelDetalles;
        private DataGridView dgvReportes;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnFiltrosAvanzados;
        private Button btnExportarPdf;
        private Button btnExportarCsv;
        private Button btnVerCompleto;

        // Filtros avanzados
        private Panel panelFiltros;
        private DateTimePicker dtpFechaDesde;
        private DateTimePicker dtpFechaHasta;
        private ComboBox cmbTipoAnalisis;
        private ComboBox cmbEstadoAnalisis;
        private Button btnAplicarFiltros;
        private Button btnLimpiarFiltros;

        // Detalles del reporte
        private Label lblInfoPaciente;
        private Label lblInfoAnalisis;
        private DataGridView dgvResultados;
        private TextBox txtObservaciones;
        private Label lblInfoMedico;
        private Label lblEstadoReporte;

        private PresentadorReportes _presentador;

        public ReportesAnalisisForm(string rolUsuario)
        {
            RolUsuarioActual = rolUsuario;
            InitializeComponent();
            InicializarComponentesPersonalizados();
            VerificarPermisosAcceso();

            _presentador = new PresentadorReportes(this);
            // Cargar datos iniciales
            CargarReportes?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeComponent()
        {
            Text = "SALC - Reportes de Análisis";
            Size = new Size(1400, 900);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = SALCColors.Background;
            WindowState = FormWindowState.Maximized;
        }

        private void InicializarComponentesPersonalizados()
        {
            CrearPanelSuperior();
            CrearPanelContenido();
            ConfigurarPanelLista();
            ConfigurarPanelDetalles();
            ConfigurarPanelFiltros();
        }

        private void VerificarPermisosAcceso()
        {
            if (RolUsuarioActual != "Médico" && RolUsuarioActual != "Asistente")
            {
                MessageBox.Show("Acceso denegado. Solo roles de Médico y Asistente pueden acceder a esta funcionalidad.",
                    "Acceso Restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void CrearPanelSuperior()
        {
            panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = SALCColors.Primary,
                Padding = new Padding(20)
            };

            var lblTitulo = new Label
            {
                Text = "REPORTES DE ANÁLISIS",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            txtBuscar = new TextBox
            {
                Text = "Buscar por nombre o DNI de paciente...",
                ForeColor = Color.Gray,
                Size = new Size(300, 30),
                Location = new Point(400, 20),
                Font = new Font("Segoe UI", 10)
            };
            txtBuscar.GotFocus += TxtBuscar_GotFocus;
            txtBuscar.LostFocus += TxtBuscar_LostFocus;

            btnBuscar = CrearBoton("Buscar", SALCColors.Secondary, new Point(710, 20));
            btnBuscar.Click += BtnBuscar_Click;

            btnFiltrosAvanzados = CrearBoton("Filtros", SALCColors.Info, new Point(840, 20));
            btnFiltrosAvanzados.Click += BtnFiltrosAvanzados_Click;

            panelSuperior.Controls.AddRange(new Control[] { lblTitulo, txtBuscar, btnBuscar, btnFiltrosAvanzados });
            Controls.Add(panelSuperior);
        }

        private void CrearPanelContenido()
        {
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SALCColors.Background,
                Padding = new Padding(20)
            };

            panelLista = new Panel
            {
                Dock = DockStyle.Left,
                Width = 800,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 20, 0)
            };

            panelDetalles = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            panelContenido.Controls.Add(panelLista);
            panelContenido.Controls.Add(panelDetalles);
            Controls.Add(panelContenido);
        }

        private Button CrearBoton(string texto, Color colorFondo, Point ubicacion)
        {
            return new Button
            {
                Text = texto,
                BackColor = colorFondo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = ubicacion,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        private void ConfigurarPanelLista()
        {
            dgvReportes = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            ConfigurarColumnasDataGridView();

            dgvReportes.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvReportes.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvReportes.RowsDefaultCellStyle.SelectionBackColor = SALCColors.PrimaryHover;
            dgvReportes.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvReportes.SelectionChanged += DgvReportes_SelectionChanged;
            dgvReportes.CellDoubleClick += DgvReportes_CellDoubleClick;

            panelLista.Controls.Add(dgvReportes);
        }

        private void ConfigurarColumnasDataGridView()
        {
            dgvReportes.Columns.Clear();

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Paciente",
                HeaderText = "Paciente",
                DataPropertyName = "NombrePaciente",
                Width = 150
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DNI",
                HeaderText = "DNI Paciente",
                DataPropertyName = "IdPaciente",
                Width = 100
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TipoAnalisis",
                HeaderText = "Tipo de Análisis",
                DataPropertyName = "TipoAnalisis",
                Width = 120
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                HeaderText = "Fecha",
                DataPropertyName = "FechaAnalisis",
                Width = 100
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                Width = 100
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Medico",
                HeaderText = "Médico",
                DataPropertyName = "NombreMedico",
                Width = 120
            });
        }

        private void ConfigurarPanelDetalles()
        {
            var contenedorDetalles = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var lblTituloDetalles = new Label
            {
                Text = "DETALLES DEL REPORTE",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.Primary,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            lblInfoPaciente = new Label
            {
                Text = "Paciente: ",
                AutoSize = true,
                Location = new Point(0, 40)
            };

            lblInfoAnalisis = new Label
            {
                Text = "Análisis: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 70)
            };

            dgvResultados = new DataGridView
            {
                Location = new Point(0, 100),
                Size = new Size(600, 200),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                RowHeadersVisible = false
            };

            var lblObservaciones = new Label
            {
                Text = "Observaciones:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 310)
            };

            txtObservaciones = new TextBox
            {
                Location = new Point(0, 330),
                Size = new Size(600, 60),
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblInfoMedico = new Label
            {
                Text = "Médico: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 400)
            };

            lblEstadoReporte = new Label
            {
                Text = "Estado: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 430)
            };

            btnVerCompleto = CrearBoton("Ver Completo", SALCColors.Primary, new Point(0, 470));
            btnVerCompleto.Click += BtnVerCompleto_Click;

            btnExportarPdf = CrearBoton("Exportar PDF", SALCColors.Success, new Point(130, 470));
            btnExportarPdf.Click += BtnExportarPdf_Click;

            btnExportarCsv = CrearBoton("Exportar CSV", SALCColors.Info, new Point(260, 470));
            btnExportarCsv.Click += BtnExportarCsv_Click;

            contenedorDetalles.Controls.AddRange(new Control[]
            {
                lblTituloDetalles, lblInfoPaciente, lblInfoAnalisis, dgvResultados,
                lblObservaciones, txtObservaciones, lblInfoMedico, lblEstadoReporte,
                btnVerCompleto, btnExportarPdf, btnExportarCsv
            });

            panelDetalles.Controls.Add(contenedorDetalles);
        }

        private void ConfigurarPanelFiltros()
        {
            panelFiltros = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = SALCColors.BackgroundLight,
                Padding = new Padding(20, 10, 20, 10),
                Visible = false
            };

            var lblFechaDesde = new Label
            {
                Text = "Desde:",
                AutoSize = true,
                Location = new Point(0, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            dtpFechaDesde = new DateTimePicker { Location = new Point(60, 10), Size = new Size(120, 25) };

            var lblFechaHasta = new Label
            {
                Text = "Hasta:",
                AutoSize = true,
                Location = new Point(190, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            dtpFechaHasta = new DateTimePicker { Location = new Point(240, 10), Size = new Size(120, 25) };

            var lblTipoAnalisis = new Label
            {
                Text = "Tipo Análisis:",
                AutoSize = true,
                Location = new Point(370, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            cmbTipoAnalisis = new ComboBox { Location = new Point(460, 10), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            var lblEstadoAnalisis = new Label
            {
                Text = "Estado:",
                AutoSize = true,
                Location = new Point(620, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            cmbEstadoAnalisis = new ComboBox { Location = new Point(670, 10), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            btnAplicarFiltros = CrearBoton("Aplicar", SALCColors.Success, new Point(800, 10));
            btnAplicarFiltros.Click += BtnAplicarFiltros_Click;

            btnLimpiarFiltros = CrearBoton("Limpiar", SALCColors.Warning, new Point(930, 10));
            btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;

            panelFiltros.Controls.AddRange(new Control[]
            {
                lblFechaDesde, dtpFechaDesde, lblFechaHasta, dtpFechaHasta,
                lblTipoAnalisis, cmbTipoAnalisis, lblEstadoAnalisis, cmbEstadoAnalisis,
                btnAplicarFiltros, btnLimpiarFiltros
            });

            Controls.Add(panelFiltros);
            Controls.SetChildIndex(panelFiltros, 1);
        }

        // Manejadores de eventos
        private void TxtBuscar_GotFocus(object sender, EventArgs e)
        {
            if (txtBuscar.Text == "Buscar por nombre o DNI de paciente...")
            {
                txtBuscar.Text = string.Empty;
                txtBuscar.ForeColor = Color.Black;
            }
        }

        private void TxtBuscar_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar por nombre o DNI de paciente...";
                txtBuscar.ForeColor = Color.Gray;
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string texto = txtBuscar.Text == "Buscar por nombre o DNI de paciente..." ? string.Empty : txtBuscar.Text;
            BuscarReportes?.Invoke(this, texto);
        }

        private void BtnFiltrosAvanzados_Click(object sender, EventArgs e)
        {
            AlternarPanelFiltros();
        }

        private void DgvReportes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow?.DataBoundItem is ReporteAnalisis sel)
            {
                ReporteSeleccionado = sel;
                CargarDetallesReporteSeleccionado();
            }
        }

        private void DgvReportes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                VerReporteCompleto?.Invoke(this, e.RowIndex);
            }
        }

        private void BtnVerCompleto_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow?.Index >= 0)
            {
                VerReporteCompleto?.Invoke(this, dgvReportes.CurrentRow.Index);
            }
        }

        private void BtnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow?.Index >= 0)
            {
                ExportarReportePdf?.Invoke(this, dgvReportes.CurrentRow.Index);
            }
        }

        private void BtnExportarCsv_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow?.Index >= 0)
            {
                ExportarReporteCsv?.Invoke(this, dgvReportes.CurrentRow.Index);
            }
        }

        private void BtnAplicarFiltros_Click(object sender, EventArgs e)
        {
            var filtro = new FiltroReporte
            {
                FechaDesde = dtpFechaDesde.Value,
                FechaHasta = dtpFechaHasta.Value,
                TipoAnalisis = cmbTipoAnalisis.SelectedItem?.ToString(),
                EstadoAnalisis = cmbEstadoAnalisis.SelectedItem?.ToString()
            };
            FiltrarReportes?.Invoke(this, filtro);
        }

        private void BtnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            dtpFechaDesde.Value = DateTime.Today.AddMonths(-1);
            dtpFechaHasta.Value = DateTime.Today;
            cmbTipoAnalisis.SelectedIndex = -1;
            cmbEstadoAnalisis.SelectedIndex = -1;
        }

        // Métodos privados
        private void AlternarPanelFiltros()
        {
            panelFiltros.Visible = !panelFiltros.Visible;
            if (panelFiltros.Visible)
            {
                CargarOpcionesFiltros();
            }
        }

        private void CargarOpcionesFiltros()
        {
            cmbTipoAnalisis.Items.Clear();
            cmbTipoAnalisis.Items.AddRange(new object[] { "Todos", "Hemograma Completo", "Glucosa en Ayunas", "Perfil Lipídico Completo", "Análisis de Orina Completo" });
            cmbTipoAnalisis.SelectedIndex = 0;

            cmbEstadoAnalisis.Items.Clear();
            cmbEstadoAnalisis.Items.AddRange(new object[] { "Todos", "Sin verificar", "Verificado" });
            cmbEstadoAnalisis.SelectedIndex = 0;
        }

        private void CargarDetallesReporteSeleccionado()
        {
            if (ReporteSeleccionado == null)
                return;

            lblInfoPaciente.Text = $"Paciente: {ReporteSeleccionado.NombrePaciente} (DNI: {ReporteSeleccionado.IdPaciente})";
            lblInfoAnalisis.Text = $"Análisis: {ReporteSeleccionado.TipoAnalisis} - {ReporteSeleccionado.FechaAnalisis:dd/MM/yyyy}";
            lblInfoMedico.Text = $"Médico: {ReporteSeleccionado.NombreMedico}";
            lblEstadoReporte.Text = $"Estado: {ReporteSeleccionado.Estado}";
            txtObservaciones.Text = ReporteSeleccionado.Observaciones;

            dgvResultados.DataSource = ReporteSeleccionado.Resultados;
        }

        // Implementación IVistaReportes
        public void CargarDatosReportes(List<ReporteAnalisis> reportes)
        {
            Reportes = reportes;
            dgvReportes.DataSource = reportes;
        }

        public void MostrarMensaje(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Views.Interfaces;
using SALC.Common;
using SALC.Services;
using SALC.Presenters;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para gestión de reportes de análisis según ERS v2.7
    /// Implementa RF-08: Generar y Enviar Informe y RF-09: Visualizar Historial
    /// </summary>
    public partial class ReportesAnalisisForm : Form, SALC.Views.Interfaces.IVistaReportes
    {
    // Eventos legacy no usados (conservados para compatibilidad si alguien los referencia)
    // public event EventHandler GenerateRequested;
    // public event EventHandler ExportPdfRequested;
    // public event EventHandler CloseRequested;

        #region Eventos personalizados
        public event EventHandler CargarReportes;
        public event EventHandler<string> BuscarReportes;
        public event EventHandler<FiltroReporte> FiltrarReportes;
        public event EventHandler<int> VerReporteCompleto;
        public event EventHandler<int> ExportarReportePdf;
        public event EventHandler<int> ExportarReporteCsv;
        #endregion

        #region Propiedades
    private readonly ServicioReportesAnalisis _servicioReportes;
    private PresentadorReportes _presentador;
        public List<ReporteAnalisis> Reportes { get; set; } = new List<ReporteAnalisis>();
        public ReporteAnalisis ReporteSeleccionado { get; set; }
        public string RolUsuarioActual { get; set; }
        #endregion

        #region Controles de la interfaz
        private Panel panelSuperior;
        private Panel panelContenido;
        private Panel panelLista;
        private Panel panelDetalles;
        private DataGridView dgvReportes;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnFiltrosAvanzados;
        private Button btnExportarPdf;
        private Button btnExportarCsv;
        private Button btnVerCompleto;

        // Filtros avanzados
        private Panel panelFiltros;
        private DateTimePicker dtpFechaDesde;
        private DateTimePicker dtpFechaHasta;
        private ComboBox cmbTipoAnalisis;
        private ComboBox cmbEstadoAnalisis;
        private Button btnAplicarFiltros;
        private Button btnLimpiarFiltros;

        // Detalles del reporte
        private Label lblInfoPaciente;
        private Label lblInfoAnalisis;
        private DataGridView dgvResultados;
        private TextBox txtObservaciones;
        private Label lblInfoMedico;
        private Label lblEstadoReporte;
        #endregion

        public ReportesAnalisisForm(string rolUsuario)
        {
            RolUsuarioActual = rolUsuario;
            _servicioReportes = new ServicioReportesAnalisis();
            InitializeComponent();
            InicializarComponentesPersonalizados();
            VerificarPermisosAcceso();
            _presentador = new PresentadorReportes(this);
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Reportes de Análisis";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SALCColors.Background;
            this.WindowState = FormWindowState.Maximized;
        }

        private void InicializarComponentesPersonalizados()
        {
            CrearPanelSuperior();
            CrearPanelContenido();
            ConfigurarPanelLista();
            ConfigurarPanelDetalles();
            ConfigurarPanelFiltros();
        }

        private void VerificarPermisosAcceso()
        {
            if (RolUsuarioActual != "Médico" && RolUsuarioActual != "Asistente")
            {
                MessageBox.Show("Acceso denegado. Solo roles de Médico y Asistente pueden acceder a esta funcionalidad.",
                    "Acceso Restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void CrearPanelSuperior()
        {
            panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = SALCColors.Primary,
                Padding = new Padding(20)
            };

            var lblTitulo = new Label
            {
                Text = "REPORTES DE ANÁLISIS",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            txtBuscar = new TextBox
            {
                Text = "Buscar por nombre o DNI de paciente...",
                ForeColor = Color.Gray,
                Size = new Size(300, 30),
                Location = new Point(400, 20),
                Font = new Font("Segoe UI", 10)
            };
            txtBuscar.GotFocus += TxtBuscar_GotFocus;
            txtBuscar.LostFocus += TxtBuscar_LostFocus;

            btnBuscar = CrearBoton("Buscar", SALCColors.Secondary, new Point(710, 20));
            btnBuscar.Click += BtnBuscar_Click;

            btnFiltrosAvanzados = CrearBoton("Filtros", SALCColors.Info, new Point(840, 20));
            btnFiltrosAvanzados.Click += BtnFiltrosAvanzados_Click;

            panelSuperior.Controls.AddRange(new Control[] { lblTitulo, txtBuscar, btnBuscar, btnFiltrosAvanzados });
            this.Controls.Add(panelSuperior);
        }

        private void CrearPanelContenido()
        {
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SALCColors.Background,
                Padding = new Padding(20)
            };

        // Panel de lista y detalles
            panelLista = new Panel
            {
                Dock = DockStyle.Left,
                Width = (int)(panelContenido.Width * 0.6),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 20, 0)
            };

            panelDetalles = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            panelContenido.Controls.Add(panelLista);
            panelContenido.Controls.Add(panelDetalles);
            this.Controls.Add(panelContenido);
        }

        private Button CrearBoton(string texto, Color colorFondo, Point ubicacion)
        {
            return new Button
            {
                Text = texto,
                BackColor = colorFondo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = ubicacion,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        private void ConfigurarPanelLista()
        {
            // Configurar DataGridView para mostrar los reportes
            dgvReportes = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            ConfigurarColumnasDataGridView();

            // Configurar estilo
            dgvReportes.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvReportes.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvReportes.RowsDefaultCellStyle.SelectionBackColor = SALCColors.PrimaryHover;
            dgvReportes.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Eventos
            dgvReportes.SelectionChanged += DgvReportes_SelectionChanged;
            dgvReportes.CellDoubleClick += DgvReportes_CellDoubleClick;

            panelLista.Controls.Add(dgvReportes);
        }

        private void ConfigurarColumnasDataGridView()
        {
            dgvReportes.Columns.Clear();

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Paciente",
                HeaderText = "Paciente",
                DataPropertyName = "NombrePaciente",
                Width = 150
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DNI",
                HeaderText = "DNI Paciente",
                DataPropertyName = "IdPaciente",
                Width = 100
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TipoAnalisis",
                HeaderText = "Tipo de Análisis",
                DataPropertyName = "TipoAnalisis",
                Width = 120
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                HeaderText = "Fecha",
                DataPropertyName = "FechaAnalisis",
                Width = 100
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                Width = 100
            });

            dgvReportes.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Medico",
                HeaderText = "Médico",
                DataPropertyName = "NombreMedico",
                Width = 120
            });
        }

        private void ConfigurarPanelDetalles()
        {
            // Panel principal para detalles
            var contenedorDetalles = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Título del panel de detalles
            var lblTituloDetalles = new Label
            {
                Text = "DETALLES DEL REPORTE",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.Primary,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            // Información del paciente
            lblInfoPaciente = new Label
            {
                Text = "Paciente: ",
                AutoSize = true,
                Location = new Point(0, 40)
            };

            // Información del análisis
            lblInfoAnalisis = new Label
            {
                Text = "Análisis: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 70)
            };

            // DataGridView para resultados
            dgvResultados = new DataGridView
            {
                Location = new Point(0, 100),
                Size = new Size(contenedorDetalles.Width - 40, 200),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                RowHeadersVisible = false
            };

            // Observaciones
            var lblObservaciones = new Label
            {
                Text = "Observaciones:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 310)
            };

            txtObservaciones = new TextBox
            {
                Location = new Point(0, 330),
                Size = new Size(contenedorDetalles.Width - 40, 60),
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Información del médico
            lblInfoMedico = new Label
            {
                Text = "Médico: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 400)
            };

            // Estado del reporte
            lblEstadoReporte = new Label
            {
                Text = "Estado: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 430)
            };

            // Botones de acción
            btnVerCompleto = CrearBoton("Ver Completo", SALCColors.Primary, new Point(0, 470));
            btnVerCompleto.Click += BtnVerCompleto_Click;

            btnExportarPdf = CrearBoton("Exportar PDF", SALCColors.Success, new Point(130, 470));
            btnExportarPdf.Click += BtnExportarPdf_Click;

            btnExportarCsv = CrearBoton("Exportar CSV", SALCColors.Info, new Point(260, 470));
            btnExportarCsv.Click += BtnExportarCsv_Click;

            // Agregar controles al panel
            contenedorDetalles.Controls.AddRange(new Control[] {
                lblTituloDetalles, lblInfoPaciente, lblInfoAnalisis, dgvResultados,
                lblObservaciones, txtObservaciones, lblInfoMedico, lblEstadoReporte,
                btnVerCompleto, btnExportarPdf, btnExportarCsv
            });

        panelDetalles.Controls.Add(contenedorDetalles);

        private void ConfigurarPanelFiltros()
        {
            panelFiltros = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = SALCColors.BackgroundLight,
                Padding = new Padding(20, 10, 20, 10),
                Visible = false
            };

            // Filtro por fecha desde
            var lblFechaDesde = new Label
            {
                Text = "Desde:",
                AutoSize = true,
                Location = new Point(0, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            dtpFechaDesde = new DateTimePicker
            {
                Location = new Point(60, 10),
                Size = new Size(120, 25)
            };

            // Filtro por fecha hasta
            var lblFechaHasta = new Label
            {
                Text = "Hasta:",
                AutoSize = true,
                Location = new Point(190, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            dtpFechaHasta = new DateTimePicker
            {
                Location = new Point(240, 10),
                Size = new Size(120, 25)
            };

            // Filtro por tipo de análisis
            var lblTipoAnalisis = new Label
            {
                Text = "Tipo Análisis:",
                AutoSize = true,
                Location = new Point(370, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            cmbTipoAnalisis = new ComboBox
            {
                Location = new Point(460, 10),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Filtro por estado del análisis
            var lblEstadoAnalisis = new Label
            {
                Text = "Estado:",
                AutoSize = true,
                Location = new Point(620, 10),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            cmbEstadoAnalisis = new ComboBox
            {
                Location = new Point(670, 10),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Botones de filtros
            btnAplicarFiltros = CrearBoton("Aplicar", SALCColors.Success, new Point(800, 10));
            btnAplicarFiltros.Click += BtnAplicarFiltros_Click;

            btnLimpiarFiltros = CrearBoton("Limpiar", SALCColors.Warning, new Point(930, 10));
            btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;

            // Agregar controles al panel de filtros
            panelFiltros.Controls.AddRange(new Control[] {
                lblFechaDesde, dtpFechaDesde, lblFechaHasta, dtpFechaHasta,
                lblTipoAnalisis, cmbTipoAnalisis, lblEstadoAnalisis, cmbEstadoAnalisis,
                btnAplicarFiltros, btnLimpiarFiltros
            });

            // Insertar el panel de filtros después del panelSuperior
            this.Controls.Add(panelFiltros);
            this.Controls.SetChildIndex(panelFiltros, 1);
        }

        #region Manejadores de Eventos

        private void TxtBuscar_GotFocus(object sender, EventArgs e)
        {
            if (txtBuscar.Text == "Buscar por nombre o DNI de paciente...")
            {
                txtBuscar.Text = "";
                txtBuscar.ForeColor = Color.Black;
            }
        }

        private void TxtBuscar_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar por nombre o DNI de paciente...";
                txtBuscar.ForeColor = Color.Gray;
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string textoBusqueda = txtBuscar.Text == "Buscar por nombre o DNI de paciente..." ? "" : txtBuscar.Text;
            BuscarReportes?.Invoke(this, textoBusqueda);
        }

        private void BtnFiltrosAvanzados_Click(object sender, EventArgs e)
        {
            AlternarPanelFiltros();
        }

        private void DgvReportes_SelectionChanged(object sender, EventArgs e)
        {
            CargarDetallesReporteSeleccionado();
        }

        private void DgvReportes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                VerReporteCompleto?.Invoke(this, e.RowIndex);
            }
        }

        private void BtnVerCompleto_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow?.Index >= 0)
            {
                VerReporteCompleto?.Invoke(this, dgvReportes.CurrentRow.Index);
            }
        }

        private void BtnExportarPdf_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow?.Index >= 0)
            {
                ExportarReportePdf?.Invoke(this, dgvReportes.CurrentRow.Index);
            }
        }

        private void BtnExportarCsv_Click(object sender, EventArgs e)
        {
            if (dgvReportes.CurrentRow?.Index >= 0)
            {
                ExportarReporteCsv?.Invoke(this, dgvReportes.CurrentRow.Index);
            }
        }

        private void BtnAplicarFiltros_Click(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void BtnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            LimpiarFiltros();
        }

        #endregion

    #region Métodos Privados

        private void AlternarPanelFiltros()
        {
            panelFiltros.Visible = !panelFiltros.Visible;
            if (panelFiltros.Visible)
            {
                CargarOpcionesFiltros();
            }
        }

        private void CargarOpcionesFiltros()
        {
            // Cargar opciones de filtros
            cmbTipoAnalisis.Items.Clear();
            cmbTipoAnalisis.Items.AddRange(new object[] { "Todos", "Hemograma Completo", "Glucosa en Ayunas", "Perfil Lipídico Completo", "Análisis de Orina Completo" });
            cmbTipoAnalisis.SelectedIndex = 0;

            cmbEstadoAnalisis.Items.Clear();
            cmbEstadoAnalisis.Items.AddRange(new object[] { "Todos", "Sin verificar", "Verificado" });
            cmbEstadoAnalisis.SelectedIndex = 0;
        }

        private void AplicarFiltros()
        {
            var filtro = new FiltroReporte
            {
                FechaDesde = dtpFechaDesde.Value,
                FechaHasta = dtpFechaHasta.Value,
                TipoAnalisis = cmbTipoAnalisis.SelectedItem?.ToString(),
                EstadoAnalisis = cmbEstadoAnalisis.SelectedItem?.ToString()
            };

            FiltrarReportes?.Invoke(this, filtro);
        }

        private void LimpiarFiltros()
        {
            dtpFechaDesde.Value = DateTime.Today.AddMonths(-1);
            dtpFechaHasta.Value = DateTime.Today;
            cmbTipoAnalisis.SelectedIndex = 0;
            cmbEstadoAnalisis.SelectedIndex = 0;
        }

        private void CargarDetallesReporteSeleccionado()
        {
            if (dgvReportes.CurrentRow == null || ReporteSeleccionado == null)
                return;

            lblInfoPaciente.Text = $"Paciente: {ReporteSeleccionado.NombrePaciente} (DNI: {ReporteSeleccionado.IdPaciente})";
            lblInfoAnalisis.Text = $"Análisis: {ReporteSeleccionado.TipoAnalisis} - {ReporteSeleccionado.FechaAnalisis:dd/MM/yyyy}";
            lblInfoMedico.Text = $"Médico: {ReporteSeleccionado.NombreMedico}";
            lblEstadoReporte.Text = $"Estado: {ReporteSeleccionado.Estado}";
            txtObservaciones.Text = ReporteSeleccionado.Observaciones;

            // Cargar resultados en el DataGridView
            dgvResultados.DataSource = ReporteSeleccionado.Resultados;
        }

        #endregion

    #region Implementación de IVistaReportes

        public void CargarDatosReportes(List<ReporteAnalisis> reportes)
        {
            Reportes = reportes;
            dgvReportes.DataSource = reportes;
        }

        // Implementación interfaz IVistaReportes (español)
        public void MostrarMensaje(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
