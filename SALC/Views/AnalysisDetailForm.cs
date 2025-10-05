using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.UI;
using SALC.Views.Interfaces;

namespace SALC.Views
{
    public partial class AnalysisDetailForm : Form
    {
        private AnalisisReport _analysisReport;
        private List<AnalysisResult> _analysisResults;

        // Controles de la UI
        private Panel headerPanel;
        private Label titleLabel;
        private Button closeButton;
        private Panel contentPanel;
        private Panel patientInfoPanel;
        private Panel analysisInfoPanel;
        private Panel resultsPanel;
        private DataGridView dgvResults;
        private Button btnPrint;
        private Button btnExportPdf;

        public AnalysisDetailForm(AnalisisReport analysisReport, List<AnalysisResult> analysisResults)
        {
            _analysisReport = analysisReport;
            _analysisResults = analysisResults;
            
            InitializeComponent();
            InitializeCustomComponents();
            LoadAnalysisData();
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Detalle de Análisis";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SALCColors.Background;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeCustomComponents()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = SALCColors.Primary
            };

            titleLabel = new Label
            {
                Text = "Detalle de Análisis",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            closeButton = new Button
            {
                Text = "X",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(40, 40),
                Location = new Point(940, 10),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(closeButton);

            // Content Panel
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Patient Info Panel
            patientInfoPanel = CreateInfoPanel("Información del Paciente", 0);
            
            // Analysis Info Panel
            analysisInfoPanel = CreateInfoPanel("Información del Análisis", 200);
            
            // Results Panel
            resultsPanel = CreateInfoPanel("Resultados", 400);
            resultsPanel.Height = 200;

            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            resultsPanel.Controls.Add(dgvResults);

            // Action Buttons
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.Transparent
            };

            btnPrint = new Button
            {
                Text = "Imprimir",
                Size = new Size(120, 40),
                Location = new Point(650, 10),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPrint.FlatAppearance.BorderSize = 0;

            btnExportPdf = new Button
            {
                Text = "Exportar PDF",
                Size = new Size(120, 40),
                Location = new Point(780, 10),
                BackColor = SALCColors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExportPdf.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.Add(btnPrint);
            buttonPanel.Controls.Add(btnExportPdf);

            contentPanel.Controls.Add(patientInfoPanel);
            contentPanel.Controls.Add(analysisInfoPanel);
            contentPanel.Controls.Add(resultsPanel);
            contentPanel.Controls.Add(buttonPanel);

            this.Controls.Add(headerPanel);
            this.Controls.Add(contentPanel);
        }

        private Panel CreateInfoPanel(string title, int top)
        {
            var panel = new Panel
            {
                Location = new Point(0, top),
                Size = new Size(940, 180),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            panel.Controls.Add(titleLabel);
            return panel;
        }

        private void LoadAnalysisData()
        {
            // Cargar datos del paciente
            AddInfoToPanel(patientInfoPanel, "DNI:", _analysisReport.PacienteDNI);
            AddInfoToPanel(patientInfoPanel, "Nombre:", _analysisReport.PacienteNombre);
            AddInfoToPanel(patientInfoPanel, "Obra Social:", _analysisReport.ObraSocial);
            AddInfoToPanel(patientInfoPanel, "Teléfono:", _analysisReport.PacienteTelefono);

            // Cargar datos del análisis
            AddInfoToPanel(analysisInfoPanel, "ID Análisis:", _analysisReport.ReportId.ToString());
            AddInfoToPanel(analysisInfoPanel, "Tipo de Análisis:", _analysisReport.TipoAnalisis);
            AddInfoToPanel(analysisInfoPanel, "Estado:", _analysisReport.Estado);
            AddInfoToPanel(analysisInfoPanel, "Prioridad:", _analysisReport.Prioridad);
            AddInfoToPanel(analysisInfoPanel, "Fecha Creación:", _analysisReport.FechaCreacion.ToString("dd/MM/yyyy HH:mm"));
            AddInfoToPanel(analysisInfoPanel, "Doctor:", _analysisReport.DoctorNombre);
            AddInfoToPanel(analysisInfoPanel, "Observaciones:", _analysisReport.Observations);

            // Configurar DataGridView para resultados
            dgvResults.Columns.Clear();
            dgvResults.Columns.Add("Metrica", "Métrica");
            dgvResults.Columns.Add("Valor", "Valor");
            dgvResults.Columns.Add("Unidad", "Unidad");
            dgvResults.Columns.Add("FechaCarga", "Fecha Carga");

            foreach (var result in _analysisResults)
            {
                dgvResults.Rows.Add(
                    result.Parameter,
                    result.Value,
                    result.Unit,
                    result.ReferenceRange
                );
            }
        }

        private void AddInfoToPanel(Panel panel, string label, string value)
        {
            var yPos = 40 + (panel.Controls.Count - 1) * 25;
            
            var labelControl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                AutoSize = true,
                Location = new Point(20, yPos)
            };

            var valueControl = new Label
            {
                Text = value ?? "N/A",
                Font = new Font("Segoe UI", 9),
                ForeColor = SALCColors.TextPrimary,
                AutoSize = true,
                Location = new Point(150, yPos)
            };

            panel.Controls.Add(labelControl);
            panel.Controls.Add(valueControl);
        }
    }
}