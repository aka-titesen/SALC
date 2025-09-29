// Views/AnalysisReportsForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Views.Interfaces;
using SALC.UI;
using SALC.Presenters;

namespace SALC.Views
{
    public partial class AnalysisReportsForm : Form, IReportsView
    {
        #region Eventos de la Interfaz IReportsView
        public event EventHandler GenerateRequested;
        public event EventHandler ExportPdfRequested;
        public event EventHandler CloseRequested;
        #endregion

        #region Eventos personalizados
        public event EventHandler LoadReports;
        public event EventHandler<string> SearchReports;
        public event EventHandler<ReportFilter> FilterReports;
        public event EventHandler<int> ViewFullReport;
        public event EventHandler<int> ExportPdfReport;
        public event EventHandler<int> ExportCsvReport;
        #endregion

        #region Propiedades
        public List<AnalisisReport> Reports { get; set; } = new List<AnalisisReport>();
        public AnalisisReport SelectedReport { get; set; }
        public string CurrentUserRole { get; set; }
        #endregion

        #region Controles de la UI
        private Panel topPanel;
        private Panel contentPanel;
        private Panel listPanel;
        private Panel detailsPanel;
        private DataGridView dgvReports;
        private ReportsPresenter _presenter;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnAdvancedFilters;
        private Button btnExportPdf;
        private Button btnExportCsv;
        private Button btnViewFull;
        
        // Filtros avanzados
        private Panel filtersPanel;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private ComboBox cbAnalysisType;
        private ComboBox cbPatientStatus;
        private Button btnApplyFilters;
        private Button btnClearFilters;
        
        // Detalles del reporte
        private Label lblPatientInfo;
        private Label lblAnalysisInfo;
        private DataGridView dgvResults;
        private TextBox txtObservations;
        private Label lblDoctorInfo;
        private Label lblReportStatus;
        #endregion

        public AnalysisReportsForm(string userRole)
        {
            CurrentUserRole = userRole;
            InitializeComponent();
            InitializeCustomComponents();
            CheckAccessPermissions();
            _presenter = new ReportsPresenter(this);
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Informes de Análisis";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SALCColors.Background;
            this.WindowState = FormWindowState.Maximized;
        }

        private void InitializeCustomComponents()
        {
            CreateTopPanel();
            CreateContentPanel();
            SetupListPanel();
            SetupDetailsPanel();
            SetupFiltersPanel();
        }

        private void CheckAccessPermissions()
        {
            if (CurrentUserRole != "Clínico" && CurrentUserRole != "Asistente")
            {
                MessageBox.Show("Acceso denegado. Solo roles de Clínico y Asistente pueden acceder a esta funcionalidad.", 
                    "Acceso Restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = SALCColors.Primary,
                Padding = new Padding(20)
            };

            var lblTitle = new Label
            {
                Text = "INFORMES DE ANÁLISIS",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            txtSearch = new TextBox
            {
                PlaceholderText = "Buscar por nombre o ID de paciente...",
                Size = new Size(300, 30),
                Location = new Point(400, 20)
            };

            btnSearch = CreateButton("Buscar", SALCColors.Secondary, new Point(710, 20));
            btnSearch.Click += (s, e) => SearchReports?.Invoke(this, txtSearch.Text);

            btnAdvancedFilters = CreateButton("Filtros Avanzados", SALCColors.Info, new Point(800, 20));
            btnAdvancedFilters.Click += (s, e) => ToggleFiltersPanel();

            topPanel.Controls.AddRange(new Control[] { lblTitle, txtSearch, btnSearch, btnAdvancedFilters });
            this.Controls.Add(topPanel);
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SALCColors.Background,
                Padding = new Padding(20)
            };

            // Dividir en listPanel (izquierda) y detailsPanel (derecha)
            listPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = (int)(contentPanel.Width * 0.6),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 20, 0)
            };

            detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            contentPanel.Controls.Add(listPanel);
            contentPanel.Controls.Add(detailsPanel);
            this.Controls.Add(contentPanel);
        }

        private Button CreateButton(string text, Color backColor, Point location)
        {
            return new Button
            {
                Text = text,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = location,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        private void SetupListPanel()
        {
            // Configurar DataGridView para mostrar los reportes
            dgvReports = new DataGridView
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
            SetupDataGridViewColumns();

            // Configurar estilo
            dgvReports.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvReports.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvReports.RowsDefaultCellStyle.SelectionBackColor = SALCColors.PrimaryHover;
            dgvReports.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            // Eventos
            dgvReports.SelectionChanged += (s, e) => LoadSelectedReportDetails();
            dgvReports.CellDoubleClick += (s, e) => ViewFullReport?.Invoke(this, e.RowIndex);

            listPanel.Controls.Add(dgvReports);
        }

        private void SetupDataGridViewColumns()
        {
            dgvReports.Columns.Clear();

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Paciente",
                HeaderText = "Paciente",
                DataPropertyName = "PatientName",
                Width = 150
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                HeaderText = "ID Paciente",
                DataPropertyName = "PatientId",
                Width = 80
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TipoAnalisis",
                HeaderText = "Tipo de Análisis",
                DataPropertyName = "AnalysisType",
                Width = 120
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Fecha",
                HeaderText = "Fecha",
                DataPropertyName = "AnalysisDate",
                Width = 100
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                DataPropertyName = "Status",
                Width = 100
            });

            dgvReports.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Doctor",
                HeaderText = "Médico",
                DataPropertyName = "DoctorName",
                Width = 120
            });
        }

        private void SetupDetailsPanel()
        {
            // Panel principal para detalles
            var detailsContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Título del panel de detalles
            var lblDetailsTitle = new Label
            {
                Text = "DETALLES DEL REPORTE",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.Primary,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            // Información del paciente
            lblPatientInfo = new Label
            {
                Text = "Paciente: ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 40)
            };

            // Información del análisis
            lblAnalysisInfo = new Label
            {
                Text = "Análisis: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 70)
            };

            // DataGridView para resultados
            dgvResults = new DataGridView
            {
                Location = new Point(0, 100),
                Size = new Size(detailsContainer.Width - 40, 200),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                RowHeadersVisible = false
            };

            // Observaciones
            var lblObservations = new Label
            {
                Text = "Observaciones:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 310)
            };

            txtObservations = new TextBox
            {
                Location = new Point(0, 330),
                Size = new Size(detailsContainer.Width - 40, 60),
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Información del médico
            lblDoctorInfo = new Label
            {
                Text = "Médico: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 400)
            };

            // Estado del reporte
            lblReportStatus = new Label
            {
                Text = "Estado: ",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(0, 430)
            };

            // Botones de acción
            btnViewFull = CreateButton("Ver Completo", SALCColors.Primary, new Point(0, 470));
            btnViewFull.Click += (s, e) => ViewFullReport?.Invoke(this, dgvReports.CurrentRow?.Index ?? -1);

            btnExportPdf = CreateButton("Exportar PDF", SALCColors.Success, new Point(130, 470));
            btnExportPdf.Click += (s, e) => ExportPdfReport?.Invoke(this, dgvReports.CurrentRow?.Index ?? -1);

            btnExportCsv = CreateButton("Exportar CSV", SALCColors.Info, new Point(260, 470));
            btnExportCsv.Click += (s, e) => ExportCsvReport?.Invoke(this, dgvReports.CurrentRow?.Index ?? -1);

            // Agregar controles al panel
            detailsContainer.Controls.AddRange(new Control[] {
                lblDetailsTitle, lblPatientInfo, lblAnalysisInfo, dgvResults,
                lblObservations, txtObservations, lblDoctorInfo, lblReportStatus,
                btnViewFull, btnExportPdf, btnExportCsv
            });

            detailsPanel.Controls.Add(detailsContainer);
        }

        private void SetupFiltersPanel()
        {
            filtersPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = SALCColors.BackgroundLight,
                Padding = new Padding(20, 10, 20, 10),
                Visible = false
            };

            // Filtro por fecha desde
            var lblFromDate = new Label
            {
                Text = "Desde:",
                AutoSize = true,
                Location = new Point(0, 10)
            };

            dtpFromDate = new DateTimePicker
            {
                Location = new Point(50, 10),
                Size = new Size(120, 25)
            };

            // Filtro por fecha hasta
            var lblToDate = new Label
            {
                Text = "Hasta:",
                AutoSize = true,
                Location = new Point(180, 10)
            };

            dtpToDate = new DateTimePicker
            {
                Location = new Point(230, 10),
                Size = new Size(120, 25)
            };

            // Filtro por tipo de análisis
            var lblAnalysisType = new Label
            {
                Text = "Tipo Análisis:",
                AutoSize = true,
                Location = new Point(360, 10)
            };

            cbAnalysisType = new ComboBox
            {
                Location = new Point(450, 10),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Filtro por estado del paciente
            var lblPatientStatus = new Label
            {
                Text = "Estado:",
                AutoSize = true,
                Location = new Point(610, 10)
            };

            cbPatientStatus = new ComboBox
            {
                Location = new Point(660, 10),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Botones de filtros
            btnApplyFilters = CreateButton("Aplicar", SALCColors.Success, new Point(790, 10));
            btnApplyFilters.Click += (s, e) => ApplyFilters();

            btnClearFilters = CreateButton("Limpiar", SALCColors.Warning, new Point(920, 10));
            btnClearFilters.Click += (s, e) => ClearFilters();

            // Agregar controles al panel de filtros
            filtersPanel.Controls.AddRange(new Control[] {
                lblFromDate, dtpFromDate, lblToDate, dtpToDate,
                lblAnalysisType, cbAnalysisType, lblPatientStatus, cbPatientStatus,
                btnApplyFilters, btnClearFilters
            });

            // Insertar el panel de filtros después del topPanel
            this.Controls.Add(filtersPanel);
            this.Controls.SetChildIndex(filtersPanel, 1);
        }

        private void ToggleFiltersPanel()
        {
            filtersPanel.Visible = !filtersPanel.Visible;
            if (filtersPanel.Visible)
            {
                LoadFilterOptions();
            }
        }

        private void LoadFilterOptions()
        {
            // Cargar opciones de filtros (esto debería venir del controlador)
            cbAnalysisType.Items.AddRange(new object[] { "Todos", "Hematología", "Bioquímica", "Microbiología", "Inmunología" });
            cbAnalysisType.SelectedIndex = 0;

            cbPatientStatus.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo", "Pendiente", "Completado" });
            cbPatientStatus.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var filter = new ReportFilter
            {
                FromDate = dtpFromDate.Value,
                ToDate = dtpToDate.Value,
                AnalysisType = cbAnalysisType.SelectedItem?.ToString(),
                PatientStatus = cbPatientStatus.SelectedItem?.ToString()
            };

            FilterReports?.Invoke(this, filter);
        }

        private void ClearFilters()
        {
            dtpFromDate.Value = DateTime.Today.AddMonths(-1);
            dtpToDate.Value = DateTime.Today;
            cbAnalysisType.SelectedIndex = 0;
            cbPatientStatus.SelectedIndex = 0;
        }

        private void LoadSelectedReportDetails()
        {
            if (dgvReports.CurrentRow == null || SelectedReport == null)
                return;

            lblPatientInfo.Text = $"Paciente: {SelectedReport.PatientName} (ID: {SelectedReport.PatientId})";
            lblAnalysisInfo.Text = $"Análisis: {SelectedReport.AnalysisType} - {SelectedReport.AnalysisDate:dd/MM/yyyy}";
            lblDoctorInfo.Text = $"Médico: {SelectedReport.DoctorName}";
            lblReportStatus.Text = $"Estado: {SelectedReport.Status}";
            txtObservations.Text = SelectedReport.Observations;

            // Cargar resultados en el DataGridView
            dgvResults.DataSource = SelectedReport.Results;
        }

        public void LoadReportsData(List<AnalisisReport> reports)
        {
            Reports = reports;
            dgvReports.DataSource = reports;
        }

        public void ShowMessage(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }
    }

    #region Clases de apoyo
    public class AnalisisReport
    {
        public int ReportId { get; set; }
        public string PatientName { get; set; }
        public string PatientId { get; set; }
        public string AnalysisType { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string Status { get; set; }
        public string DoctorName { get; set; }
        public string Observations { get; set; }
        public List<AnalysisResult> Results { get; set; } = new List<AnalysisResult>();
    }

    public class AnalysisResult
    {
        public string Parameter { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string ReferenceRange { get; set; }
    }

    public class ReportFilter
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string AnalysisType { get; set; }
        public string PatientStatus { get; set; }
    }
    #endregion
}