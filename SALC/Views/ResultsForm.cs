// Views/ResultsForm.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.UI;

namespace SALC.Views
{
    public class ResultsForm : Form
    {
        // Datos locales de UI (placeholders)
        private List<Analisis> PendingStudies { get; set; } = new List<Analisis>();
        private Analisis SelectedStudy { get; set; }
        private List<Metrica> AvailableMetrics { get; set; } = new List<Metrica>();
        private List<ResultadoAnalisis> StudyResults { get; set; } = new List<ResultadoAnalisis>();

        // Controles de la UI
        private Panel topPanel;
        private Panel contentPanel;
        private Panel studiesPanel;
        private Panel resultsPanel;
        private DataGridView dgvPendingStudies;
        private DataGridView dgvResults;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnLoadResults;
        private Button btnSaveResult;
        private Button btnCompleteStudy;
        private Label lblSelectedStudy;
        private Panel metricsPanel;

        public ResultsForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Carga de Resultados";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SALCColors.Background;
            this.WindowState = FormWindowState.Maximized;
        }

        private void InitializeCustomComponents()
        {
            CreateTopPanel();
            CreateContentPanel();
            SetupStudiesPanel();
            SetupResultsPanel();
        }

        private void CreateTopPanel()
        {
            topPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(20)
            };

            Label titleLabel = new Label
            {
                Text = "Carga de Resultados de Análisis",
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                ForeColor = SALCColors.Results,
                Size = new Size(400, 40),
                Location = new Point(20, 20)
            };

            Panel searchPanel = new Panel
            {
                Size = new Size(400, 40),
                Location = new Point(this.Width - 440, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            txtSearch = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(0, 5),
                Font = new Font("Microsoft Sans Serif", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            // Placeholder manual para .NET Framework
            txtSearch.Text = "Buscar por paciente o ID...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == "Buscar por paciente o ID...")
                {
                    txtSearch.Text = string.Empty;
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Buscar por paciente o ID...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            btnSearch = new Button
            {
                Text = "Buscar",
                Size = new Size(80, 30),
                Location = new Point(310, 5),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => { /* Solo UI */ };

            searchPanel.Controls.AddRange(new Control[] { txtSearch, btnSearch });
            topPanel.Controls.AddRange(new Control[] { titleLabel, searchPanel });
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

            // Panel izquierdo para estudios pendientes
            studiesPanel = new Panel
            {
                Width = 500,
                Dock = DockStyle.Left,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(10)
            };

            // Panel derecho para carga de resultados
            resultsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(20),
                Margin = new Padding(20, 0, 0, 0)
            };

            contentPanel.Controls.AddRange(new Control[] { studiesPanel, resultsPanel });
            this.Controls.Add(contentPanel);
        }

        private void SetupStudiesPanel()
        {
            Label studiesTitle = new Label
            {
                Text = "Estudios Pendientes",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                Size = new Size(480, 25),
                Location = new Point(10, 10),
                Dock = DockStyle.Top
            };

            // Botones de acción para estudios
            Panel buttonPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(0, 10, 0, 0)
            };

            btnLoadResults = new Button
            {
                Text = "Cargar Resultados",
                Size = new Size(150, 30),
                Location = new Point(10, 5),
                BackColor = SALCColors.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            btnLoadResults.FlatAppearance.BorderSize = 0;
            btnLoadResults.Click += (s, e) =>
            {
                if (SelectedStudy != null)
                {
                    CreateMetricsControls();
                    btnSaveResult.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Seleccione un estudio para cargar resultados.");
                }
            };

            btnCompleteStudy = new Button
            {
                Text = "Completar Estudio",
                Size = new Size(130, 30),
                Location = new Point(170, 5),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 9)
            };
            btnCompleteStudy.FlatAppearance.BorderSize = 0;
            btnCompleteStudy.Click += (s, e) =>
            {
                if (SelectedStudy != null)
                {
                    var result = MessageBox.Show(
                        $"¿Está seguro que desea marcar como completado el estudio ID {SelectedStudy.Id}?",
                        "Confirmar Completar Estudio",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        MessageBox.Show("Estudio marcado como completado (solo UI)");
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un estudio para completar.");
                }
            };

            buttonPanel.Controls.AddRange(new Control[] { btnLoadResults, btnCompleteStudy });

            // DataGridView para estudios pendientes
            dgvPendingStudies = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Microsoft Sans Serif", 9)
            };

            // Configurar columnas para estudios pendientes
            dgvPendingStudies.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "Paciente", HeaderText = "Paciente", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Prioridad", HeaderText = "Prioridad", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Fecha", Width = 90 }
            });

            dgvPendingStudies.SelectionChanged += (s, e) =>
            {
                if (dgvPendingStudies.SelectedRows.Count > 0)
                {
                    var row = dgvPendingStudies.SelectedRows[0];
                    int id = (int)row.Cells["Id"].Value;
                    SelectedStudy = PendingStudies.Find(st => st.Id == id);

                    if (SelectedStudy != null)
                    {
                        lblSelectedStudy.Text = $"Estudio ID: {SelectedStudy.Id} - {SelectedStudy.Paciente?.Nombre} {SelectedStudy.Paciente?.Apellido} - {SelectedStudy.TipoAnalisis?.Descripcion}";
                        btnSaveResult.Enabled = false;
                        ClearMetricsPanel();
                    }
                }
            };

            studiesPanel.Controls.AddRange(new Control[] { studiesTitle, buttonPanel, dgvPendingStudies });
        }

        private void SetupResultsPanel()
        {
            lblSelectedStudy = new Label
            {
                Text = "Seleccione un estudio para cargar resultados",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                Size = new Size(600, 25),
                Location = new Point(20, 20),
                Dock = DockStyle.Top
            };

            // Panel para métricas dinámicas
            metricsPanel = new Panel
            {
                Height = 400,
                Dock = DockStyle.Top,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(20),
                AutoScroll = true,
                Margin = new Padding(0, 10, 0, 10)
            };

            // Panel para botones de acción
            Panel actionPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = SALCColors.CardBackground,
                Padding = new Padding(20)
            };

            btnSaveResult = new Button
            {
                Text = "Guardar Resultado",
                Size = new Size(150, 35),
                Location = new Point(20, 10),
                BackColor = SALCColors.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                Enabled = false
            };
            btnSaveResult.FlatAppearance.BorderSize = 0;
            btnSaveResult.Click += (s, e) =>
            {
                if (SelectedStudy != null && ValidateMetrics())
                {
                    var resultados = CollectMetricsValues();
                    StudyResults.AddRange(resultados);
                    RefreshResultsList();
                    MessageBox.Show("Resultados guardados (solo UI)");
                }
            };

            actionPanel.Controls.Add(btnSaveResult);

            // DataGridView para resultados guardados
            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Microsoft Sans Serif", 9)
            };

            // Configurar columnas para resultados
            dgvResults.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Metrica", HeaderText = "Métrica", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "Valor", HeaderText = "Valor", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Unidad", HeaderText = "Unidad", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "RangoReferencia", HeaderText = "Rango Referencia", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", Width = 100 }
            });

            Label resultsTitle = new Label
            {
                Text = "Resultados Ingresados:",
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                Size = new Size(400, 20),
                Location = new Point(20, 10),
                Dock = DockStyle.Top
            };

            resultsPanel.Controls.AddRange(new Control[] { 
                lblSelectedStudy, metricsPanel, actionPanel, resultsTitle, dgvResults 
            });
        }

        private void RefreshResultsList()
        {
            dgvResults.Rows.Clear();
            foreach (var resultado in StudyResults)
            {
                var metrica = AvailableMetrics.Find(m => m.IdMet == resultado.IdMet);
                string estado = ValidateResult(resultado.Valor, metrica) ? "Normal" : "Fuera de rango";
                Color estadoColor = estado == "Normal" ? SALCColors.Success : SALCColors.Danger;

                int rowIndex = dgvResults.Rows.Add(
                    metrica?.Nombre ?? "N/A",
                    resultado.Valor.ToString("F2"),
                    metrica?.Unidad ?? "",
                    GetReferenceRange(metrica),
                    estado
                );

                dgvResults.Rows[rowIndex].Cells["Estado"].Style.BackColor = estadoColor;
                dgvResults.Rows[rowIndex].Cells["Estado"].Style.ForeColor = Color.White;
            }
        }

        private void CreateMetricsControls()
        {
            ClearMetricsPanel();

            if (SelectedStudy == null)
            {
                SelectedStudy = new Analisis { Id = 1, IdTipoAnalisis = 1, Paciente = new Paciente { Nombre = "Ana", Apellido = "García" }, TipoAnalisis = new TipoAnalisis { Descripcion = "Perfil Lipídico" }, FechaCreacion = DateTime.Today };
            }

            var metricas = GetMetricsForAnalysisType(SelectedStudy.IdTipoAnalisis);
            AvailableMetrics = metricas;

            int yPos = 20;
            int spacing = 40;

            foreach (var metrica in metricas)
            {
                CreateMetricControl(metrica, yPos);
                yPos += spacing;
            }
        }

        private void CreateMetricControl(Metrica metrica, int yPos)
        {
            Label lblMetrica = new Label
            {
                Text = $"{metrica.Nombre}:",
                Size = new Size(200, 20),
                Location = new Point(20, yPos + 3),
                Font = new Font("Microsoft Sans Serif", 9),
                ForeColor = SALCColors.TextPrimary
            };

            TextBox txtValor = new TextBox
            {
                Size = new Size(100, 25),
                Location = new Point(230, yPos),
                Font = new Font("Microsoft Sans Serif", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Tag = metrica
            };

            Label lblUnidad = new Label
            {
                Text = metrica.Unidad,
                Size = new Size(50, 20),
                Location = new Point(340, yPos + 3),
                Font = new Font("Microsoft Sans Serif", 9),
                ForeColor = SALCColors.TextSecondary
            };

            Label lblRango = new Label
            {
                Text = GetReferenceRange(metrica),
                Size = new Size(200, 20),
                Location = new Point(400, yPos + 3),
                Font = new Font("Microsoft Sans Serif", 8),
                ForeColor = SALCColors.TextSecondary
            };

            txtValor.TextChanged += (s, e) =>
            {
                if (decimal.TryParse(txtValor.Text, out decimal valor))
                {
                    bool isValid = ValidateResult(valor, metrica);
                    txtValor.BackColor = isValid ? Color.LightGreen : Color.LightCoral;
                }
                else
                {
                    txtValor.BackColor = Color.White;
                }
            };

            metricsPanel.Controls.AddRange(new Control[] { lblMetrica, txtValor, lblUnidad, lblRango });
        }

        private void ClearMetricsPanel()
        {
            metricsPanel.Controls.Clear();
        }

        private List<Metrica> GetMetricsForAnalysisType(int tipoAnalisisId)
        {
            return new List<Metrica>
            {
                new Metrica { IdMet = 1, Nombre = "Glucosa", Unidad = "mg/dL", ValorMinimo = 70, ValorMaximo = 110 },
                new Metrica { IdMet = 2, Nombre = "Colesterol Total", Unidad = "mg/dL", ValorMinimo = 150, ValorMaximo = 200 },
                new Metrica { IdMet = 3, Nombre = "Triglicéridos", Unidad = "mg/dL", ValorMinimo = 50, ValorMaximo = 150 }
            };
        }

        private bool ValidateResult(decimal valor, Metrica metrica)
        {
            if (metrica == null) return true;
            return valor >= metrica.ValorMinimo && valor <= metrica.ValorMaximo;
        }

        private string GetReferenceRange(Metrica metrica)
        {
            if (metrica == null) return "N/A";
            return $"{metrica.ValorMinimo} - {metrica.ValorMaximo}";
        }

        private bool ValidateMetrics()
        {
            foreach (Control control in metricsPanel.Controls)
            {
                if (control is TextBox txtBox)
                {
                    if (string.IsNullOrWhiteSpace(txtBox.Text))
                    {
                        MessageBox.Show("Todos los campos de métricas son requeridos.");
                        return false;
                    }

                    if (!decimal.TryParse(txtBox.Text, out _))
                    {
                        MessageBox.Show("Todos los valores deben ser numéricos.");
                        return false;
                    }
                }
            }
            return true;
        }

        private List<ResultadoAnalisis> CollectMetricsValues()
        {
            var resultados = new List<ResultadoAnalisis>();

            foreach (Control control in metricsPanel.Controls)
            {
                if (control is TextBox txtBox && txtBox.Tag is Metrica metrica)
                {
                    if (decimal.TryParse(txtBox.Text, out decimal valor))
                    {
                        resultados.Add(new ResultadoAnalisis
                        {
                            IdAnalisis = SelectedStudy?.Id ?? 0,
                            IdMet = metrica.IdMet,
                            Valor = valor,
                            FechaCarga = DateTime.Now
                        });
                    }
                }
            }

            return resultados;
        }

        // Clases temporales solo para UI
        private class Metrica
        {
            public int IdMet { get; set; }
            public string Nombre { get; set; }
            public string Unidad { get; set; }
            public decimal ValorMinimo { get; set; }
            public decimal ValorMaximo { get; set; }
        }

        private class ResultadoAnalisis
        {
            public int IdAnalisis { get; set; }
            public int IdMet { get; set; }
            public decimal Valor { get; set; }
            public DateTime FechaCarga { get; set; }
        }

        private class Analisis
        {
            public int Id { get; set; }
            public int IdTipoAnalisis { get; set; }
            public Paciente Paciente { get; set; }
            public TipoAnalisis TipoAnalisis { get; set; }
            public DateTime FechaCreacion { get; set; }
        }

        private class Paciente { public string Nombre { get; set; } public string Apellido { get; set; } }
        private class TipoAnalisis { public string Descripcion { get; set; } }
    }
}