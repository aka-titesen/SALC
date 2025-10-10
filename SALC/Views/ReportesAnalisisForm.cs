using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Views.Interfaces;
using SALC.Common;
using SALC.Services;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para gesti�n de reportes de an�lisis seg�n ERS v2.7
    /// Implementa RF-08: Generar y Enviar Informe y RF-09: Visualizar Historial
    /// </summary>
    public partial class ReportesAnalisisForm : Form, IReportsView
    {
        #region Eventos de la Interfaz IReportsView
        public event EventHandler GenerateRequested;
        public event EventHandler ExportPdfRequested;
        public event EventHandler CloseRequested;
        #endregion

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
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Reportes de An�lisis";
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
            if (RolUsuarioActual != "M�dico" && RolUsuarioActual != "Asistente")
            {
                MessageBox.Show("Acceso denegado. Solo roles de M�dico y Asistente pueden acceder a esta funcionalidad.", 
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
                Text = "REPORTES DE AN�LISIS",
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

            btnBuscar = CrearBoton("?? Buscar", SALCColors.Secondary, new Point(710, 20));
            btnBuscar.Click += BtnBuscar_Click;

            btnFiltrosAvanzados = CrearBoton("?? Filtros", SALCColors.Info, new Point(840, 20));
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

            // Dividir en panelLista (izquierda) y panelDetalles (derecha)
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

            // Configurar columnas
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
                HeaderText = "Tipo de An�lisis",
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
                HeaderText = "M�dico",
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

            // T�tulo del panel de detalles
            var lblTituloDetalles = new Label
            {
                Text = "DETALLES DEL REPORTE",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.Primary,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            // Informaci�n del paciente
            lblInfoPaciente = new Label
            {
                Text = "Paciente: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 40)
            };

            // Informaci�n del an�lisis
            lblInfoAnalisis = new Label
            {
                Text = "An�lisis: ",
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

            // Informaci�n del m�dico
            lblInfoMedico = new Label
            {
                Text = "M�dico: ",
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

            // Botones de acci�n
            btnVerCompleto = CrearBoton("??? Ver Completo", SALCColors.Primary, new Point(0, 470));
            btnVerCompleto.Click += BtnVerCompleto_Click;

            btnExportarPdf = CrearBoton("?? Exportar PDF", SALCColors.Success, new Point(130, 470));
            btnExportarPdf.Click += BtnExportarPdf_Click;

            btnExportarCsv = CrearBoton("?? Exportar CSV", SALCColors.Info, new Point(260, 470));
            btnExportarCsv.Click += BtnExportarCsv_Click;

            // Agregar controles al panel
            contenedorDetalles.Controls.AddRange(new Control[] {
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

            // Filtro por tipo de an�lisis
            var lblTipoAnalisis = new Label
            {
                Text = "Tipo An�lisis:",
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

            // Filtro por estado del an�lisis
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
            btnAplicarFiltros = CrearBoton("? Aplicar", SALCColors.Success, new Point(800, 10));
            btnAplicarFiltros.Click += BtnAplicarFiltros_Click;

            btnLimpiarFiltros = CrearBoton("?? Limpiar", SALCColors.Warning, new Point(930, 10));
            btnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;

            // Agregar controles al panel de filtros
            panelFiltros.Controls.AddRange(new Control[] {
                lblFechaDesde, dtpFechaDesde, lblFechaHasta, dtpFechaHasta,
                lblTipoAnalisis, cmbTipoAnalisis, lblEstadoAnalisis, cmbEstadoAnalisis,
                btnAplicarFiltros, btnLimpiarFiltros
            });

            // Insertar el panel de filtros despu�s del panelSuperior
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

        #region M�todos Privados

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
            cmbTipoAnalisis.Items.AddRange(new object[] { "Todos", "Hemograma Completo", "Glucosa en Ayunas", "Perfil Lip�dico Completo", "An�lisis de Orina Completo" });
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
            lblInfoAnalisis.Text = $"An�lisis: {ReporteSeleccionado.TipoAnalisis} - {ReporteSeleccionado.FechaAnalisis:dd/MM/yyyy}";
            lblInfoMedico.Text = $"M�dico: {ReporteSeleccionado.NombreMedico}";
            lblEstadoReporte.Text = $"Estado: {ReporteSeleccionado.Estado}";
            txtObservaciones.Text = ReporteSeleccionado.Observaciones;

            // Cargar resultados en el DataGridView
            dgvResultados.DataSource = ReporteSeleccionado.Resultados;
        }

        #endregion

        #region Implementaci�n de IReportsView

        public void CargarDatosReportes(List<ReporteAnalisis> reportes)
        {
            Reportes = reportes;
            dgvReportes.DataSource = reportes;
        }

        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}