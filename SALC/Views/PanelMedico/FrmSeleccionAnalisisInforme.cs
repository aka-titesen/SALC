using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SALC.Domain;
using SALC.BLL;

namespace SALC.Views.PanelMedico
{
    public class FrmSeleccionAnalisisInforme : Form
    {
        private DataGridView gridAnalisis;
        private TextBox txtBuscar;
        private Button btnSeleccionar, btnCancelar;
        private List<AnalisisInformeViewModel> _analisisViewModel;
        private List<AnalisisInformeViewModel> _analisisFiltrados;
        private readonly int _dniMedico;
        
        public Analisis AnalisisSeleccionado { get; private set; }

        public FrmSeleccionAnalisisInforme(int dniMedico)
        {
            _dniMedico = dniMedico;
            InitializeComponent();
            CargarAnalisis();
        }

        private void InitializeComponent()
        {
            Text = "Seleccionar Análisis para Generar Informe";
            Width = 1000;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Título
            var lblTitulo = new Label
            {
                Text = "Seleccione un análisis 'Verificado' para generar informe PDF",
                Left = 20, Top = 20, Width = 500, Height = 25,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold)
            };

            var lblInfo = new Label
            {
                Text = "Solo se muestran análisis verificados (firmados) de su autoría con métricas cargadas",
                Left = 20, Top = 50, Width = 650, Height = 20,
                ForeColor = System.Drawing.Color.Blue
            };

            // Búsqueda
            var lblBuscar = new Label { Text = "Buscar:", Left = 20, Top = 80, Width = 60 };
            txtBuscar = new TextBox 
            { 
                Left = 80, Top = 78, Width = 200
            };
            txtBuscar.TextChanged += OnBuscarTextChanged;

            // Grid
            gridAnalisis = new DataGridView
            {
                Left = 20, Top = 110, Width = 940, Height = 380,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Configurar columnas
            gridAnalisis.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "IdAnalisis", HeaderText = "ID", DataPropertyName = "IdAnalisis", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "PacienteNombre", HeaderText = "Paciente", DataPropertyName = "PacienteNombre", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo Análisis", DataPropertyName = "TipoAnalisis", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "Fecha Creación", DataPropertyName = "FechaCreacion", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "FechaFirma", HeaderText = "Fecha Firma", DataPropertyName = "FechaFirma", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "CantidadMetricas", HeaderText = "Métricas", DataPropertyName = "CantidadMetricas", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", DataPropertyName = "Estado", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Observaciones", HeaderText = "Observaciones", DataPropertyName = "Observaciones", Width = 150 }
            });

            gridAnalisis.CellDoubleClick += OnCellDoubleClick;

            // Botones
            btnSeleccionar = new Button
            {
                Text = "Generar Informe",
                Left = 770, Top = 510, Width = 120, Height = 35,
                DialogResult = DialogResult.OK,
                Enabled = false,
                BackColor = System.Drawing.Color.LightCoral
            };
            btnSeleccionar.Click += OnSeleccionar;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Left = 900, Top = 510, Width = 80, Height = 35,
                DialogResult = DialogResult.Cancel
            };

            gridAnalisis.SelectionChanged += (s, e) => 
            {
                btnSeleccionar.Enabled = gridAnalisis.CurrentRow != null;
            };

            // Agregar controles
            Controls.AddRange(new Control[] 
            { 
                lblTitulo, lblInfo, lblBuscar, txtBuscar, gridAnalisis, btnSeleccionar, btnCancelar 
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

                // Obtener análisis del médico en estado "Verificado" (id_estado = 2)
                var analisisDelMedico = analisisService.ObtenerAnalisisPorMedicoCarga(_dniMedico)
                    .Where(a => a.IdEstado == 2) // Solo "Verificado"
                    .OrderByDescending(a => a.FechaFirma ?? a.FechaCreacion)
                    .ToList();

                _analisisViewModel = new List<AnalisisInformeViewModel>();

                foreach (var analisis in analisisDelMedico)
                {
                    try
                    {
                        // Verificar que tenga métricas cargadas
                        var resultados = analisisService.ObtenerResultados(analisis.IdAnalisis).ToList();
                        if (!resultados.Any())
                        {
                            continue; // Saltar análisis sin resultados (aunque no debería pasar en verificados)
                        }

                        var paciente = pacienteRepo.ObtenerPorId(analisis.DniPaciente);
                        var tipoAnalisis = catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);

                        _analisisViewModel.Add(new AnalisisInformeViewModel
                        {
                            Analisis = analisis,
                            IdAnalisis = analisis.IdAnalisis,
                            PacienteNombre = paciente != null ? $"{paciente.Nombre} {paciente.Apellido} (DNI: {paciente.Dni})" : "Paciente no encontrado",
                            TipoAnalisis = tipoAnalisis?.Descripcion ?? "Tipo no encontrado",
                            FechaCreacion = analisis.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                            FechaFirma = analisis.FechaFirma?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                            CantidadMetricas = $"{resultados.Count} métricas",
                            Estado = "Verificado",
                            Observaciones = string.IsNullOrWhiteSpace(analisis.Observaciones) ? "-" : analisis.Observaciones
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue
                        System.Diagnostics.Debug.WriteLine($"Error procesando análisis {analisis.IdAnalisis}: {ex.Message}");
                    }
                }

                _analisisFiltrados = _analisisViewModel.ToList();
                gridAnalisis.DataSource = _analisisViewModel;

                if (!_analisisViewModel.Any())
                {
                    MessageBox.Show("No se encontraron análisis 'Verificados' de su autoría.\n\n" +
                                  "Para generar informes, primero debe crear análisis, cargar resultados y firmarlos.", 
                                  "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var lblConteo = new Label
                    {
                        Text = $"Se encontraron {_analisisViewModel.Count} análisis verificados listos para informe",
                        Left = 300, Top = 80, Width = 400, Height = 20,
                        ForeColor = System.Drawing.Color.Green
                    };
                    Controls.Add(lblConteo);
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
                    AnalisisSeleccionado = _analisisFiltrados[rowIndex].Analisis;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private class AnalisisInformeViewModel
        {
            public Analisis Analisis { get; set; }
            public int IdAnalisis { get; set; }
            public string PacienteNombre { get; set; }
            public string TipoAnalisis { get; set; }
            public string FechaCreacion { get; set; }
            public string FechaFirma { get; set; }
            public string CantidadMetricas { get; set; }
            public string Estado { get; set; }
            public string Observaciones { get; set; }
        }
    }
}