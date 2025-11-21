using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using SALC.Domain;
using SALC.BLL;

namespace SALC.Views.PanelMedico
{
    public class FrmSeleccionAnalisisFirma : Form
    {
        private DataGridView gridAnalisis;
        private TextBox txtBuscar;
        private Button btnSeleccionar, btnCancelar;
        private List<AnalisisFirmaViewModel> _analisisViewModel;
        private List<AnalisisFirmaViewModel> _analisisFiltrados;
        private readonly int _dniMedico;
        
        public Analisis AnalisisSeleccionado { get; private set; }

        public FrmSeleccionAnalisisFirma(int dniMedico)
        {
            _dniMedico = dniMedico;
            InitializeComponent();
            CargarAnalisis();
        }

        private void InitializeComponent()
        {
            Text = "Seleccionar Análisis para Validar y Firmar";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            ShowIcon = false;

            // Título principal
            var lblTitulo = new Label
            {
                Text = "Selección de Análisis para Firma Digital",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(142, 68, 173),
                Location = new Point(20, 15),
                Size = new Size(950, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = "Seleccione el análisis que desea revisar y firmar digitalmente",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(950, 20),
                BackColor = Color.Transparent
            };

            // Panel informativo
            var panelInfo = new Panel
            {
                Location = new Point(20, 75),
                Size = new Size(940, 50),
                BackColor = Color.FromArgb(250, 245, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblInfo = new Label
            {
                Text = "Solo se muestran análisis en estado 'Sin verificar' con resultados cargados.\n" +
                       "Revise cuidadosamente los resultados antes de firmar.",
                Location = new Point(15, 10),
                Size = new Size(910, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                BackColor = Color.Transparent
            };
            panelInfo.Controls.Add(lblInfo);

            // Panel de advertencia
            var panelAdvertencia = new Panel
            {
                Location = new Point(20, 135),
                Size = new Size(940, 40),
                BackColor = Color.FromArgb(255, 235, 238),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblAdvertencia = new Label
            {
                Text = "IMPORTANTE: Una vez firmado digitalmente, el análisis NO podrá modificarse",
                Location = new Point(15, 10),
                Size = new Size(910, 22),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(183, 28, 28),
                BackColor = Color.Transparent
            };
            panelAdvertencia.Controls.Add(lblAdvertencia);

            // Búsqueda
            var lblBuscar = new Label 
            { 
                Text = "Buscar:", 
                Left = 20, 
                Top = 195, 
                Width = 80,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            
            txtBuscar = new TextBox 
            { 
                Left = 110, 
                Top = 193, 
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBuscar.TextChanged += OnBuscarTextChanged;

            var lblAyuda = new Label
            {
                Text = "Escriba ID, nombre de paciente o tipo de análisis",
                Left = 420,
                Top = 195,
                Width = 350,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166)
            };

            // Grid de análisis
            gridAnalisis = new DataGridView
            {
                Left = 20, 
                Top = 235, 
                Width = 940, 
                Height = 360,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 40,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(142, 68, 173),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Padding = new Padding(8),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(235, 222, 240),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(5)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 245, 255),
                    Padding = new Padding(5)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 35 }
            };

            // Configurar columnas
            gridAnalisis.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "IdAnalisis", HeaderText = "ID", DataPropertyName = "IdAnalisis", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "PacienteNombre", HeaderText = "Paciente", DataPropertyName = "PacienteNombre", Width = 220 },
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo Análisis", DataPropertyName = "TipoAnalisis", Width = 170 },
                new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "Fecha Creación", DataPropertyName = "FechaCreacion", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "CantidadMetricas", HeaderText = "Métricas", DataPropertyName = "CantidadMetricas", Width = 90 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", DataPropertyName = "Estado", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Observaciones", HeaderText = "Observaciones", DataPropertyName = "Observaciones", Width = 150 }
            });

            gridAnalisis.CellDoubleClick += OnCellDoubleClick;

            // Botones
            btnSeleccionar = new Button
            {
                Text = "Seleccionar para Firmar",
                Left = 710, 
                Top = 620, 
                Width = 170, 
                Height = 45,
                DialogResult = DialogResult.OK,
                Enabled = false,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(142, 68, 173),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSeleccionar.FlatAppearance.BorderSize = 0;
            btnSeleccionar.Click += OnSeleccionar;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Left = 890, 
                Top = 620, 
                Width = 90, 
                Height = 45,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            gridAnalisis.SelectionChanged += (s, e) => 
            {
                btnSeleccionar.Enabled = gridAnalisis.CurrentRow != null;
            };

            // Agregar controles
            Controls.AddRange(new Control[] 
            { 
                lblTitulo, lblSubtitulo, panelInfo, panelAdvertencia,
                lblBuscar, txtBuscar, lblAyuda,
                gridAnalisis, 
                btnSeleccionar, btnCancelar 
            });

            AcceptButton = btnSeleccionar;
            CancelButton = btnCancelar;
        }

        private void CargarAnalisis()
        {
            try
            {
                var analisisService = new AnalisisService();
                var pacienteRepo = new SALC.DAL.PacienteRepositorio();
                var catalogoService = new CatalogoService();

                // Obtener análisis del médico en estado "Sin verificar" (id_estado = 1)
                var analisisDelMedico = analisisService.ObtenerAnalisisPorMedicoCarga(_dniMedico)
                    .Where(a => a.IdEstado == 1) // Solo "Sin verificar"
                    .OrderByDescending(a => a.FechaCreacion)
                    .ToList();

                _analisisViewModel = new List<AnalisisFirmaViewModel>();

                foreach (var analisis in analisisDelMedico)
                {
                    try
                    {
                        // Verificar que tenga métricas cargadas
                        var resultados = analisisService.ObtenerResultados(analisis.IdAnalisis).ToList();
                        if (!resultados.Any())
                        {
                            continue; // Saltar análisis sin resultados
                        }

                        var paciente = pacienteRepo.ObtenerPorId(analisis.DniPaciente);
                        var tipoAnalisis = catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);

                        _analisisViewModel.Add(new AnalisisFirmaViewModel
                        {
                            Analisis = analisis,
                            IdAnalisis = analisis.IdAnalisis,
                            PacienteNombre = paciente != null ? $"{paciente.Apellido}, {paciente.Nombre} (DNI: {paciente.Dni})" : "Paciente no encontrado",
                            TipoAnalisis = tipoAnalisis?.Descripcion ?? "Tipo no encontrado",
                            FechaCreacion = analisis.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                            CantidadMetricas = $"{resultados.Count} métricas",
                            Estado = "Sin verificar",
                            Observaciones = string.IsNullOrWhiteSpace(analisis.Observaciones) ? "-" : analisis.Observaciones
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error procesando análisis {analisis.IdAnalisis}: {ex.Message}");
                    }
                }

                _analisisFiltrados = _analisisViewModel.ToList();
                gridAnalisis.DataSource = _analisisViewModel;

                if (!_analisisViewModel.Any())
                {
                    MessageBox.Show(
                        "No se encontraron análisis 'Sin verificar' con resultados cargados.\n\n" +
                        "Para firmar un análisis, primero debe:\n" +
                        "1. Crear el análisis en 'Crear Análisis Clínico'\n" +
                        "2. Cargar las métricas en 'Cargar Resultados'", 
                        "Información", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar análisis: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnBuscarTextChanged(object sender, EventArgs e)
        {
            var filtro = txtBuscar.Text.Trim().ToLowerInvariant();
            
            if (string.IsNullOrEmpty(filtro))
            {
                _analisisFiltrados = _analisisViewModel.ToList();
            }
            else
            {
                _analisisFiltrados = _analisisViewModel.Where(a =>
                    a.IdAnalisis.ToString().Contains(filtro) ||
                    a.PacienteNombre.ToLowerInvariant().Contains(filtro) ||
                    a.TipoAnalisis.ToLowerInvariant().Contains(filtro)
                ).ToList();
            }

            gridAnalisis.DataSource = _analisisFiltrados;
        }

        private void OnCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                OnSeleccionar(sender, e);
            }
        }

        private void OnSeleccionar(object sender, EventArgs e)
        {
            if (gridAnalisis.CurrentRow != null)
            {
                var rowIndex = gridAnalisis.CurrentRow.Index;
                if (rowIndex < _analisisFiltrados.Count)
                {
                    var analisisViewModel = _analisisFiltrados[rowIndex];
                    
                    // Confirmación adicional con diseño consistente
                    var confirmacion = MessageBox.Show(
                        $"¿Está seguro de que desea seleccionar este análisis para firmar?\n\n" +
                        $"ID: {analisisViewModel.IdAnalisis}\n" +
                        $"Paciente: {analisisViewModel.PacienteNombre}\n" +
                        $"Tipo: {analisisViewModel.TipoAnalisis}\n" +
                        $"Métricas: {analisisViewModel.CantidadMetricas}\n\n" +
                        "ATENCIÓN: Una vez firmado, el análisis NO podrá modificarse.",
                        "Confirmar Selección para Firma Digital",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (confirmacion == DialogResult.Yes)
                    {
                        AnalisisSeleccionado = analisisViewModel.Analisis;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
            }
        }

        private class AnalisisFirmaViewModel
        {
            public Analisis Analisis { get; set; }
            public int IdAnalisis { get; set; }
            public string PacienteNombre { get; set; }
            public string TipoAnalisis { get; set; }
            public string FechaCreacion { get; set; }
            public string CantidadMetricas { get; set; }
            public string Estado { get; set; }
            public string Observaciones { get; set; }
        }
    }
}