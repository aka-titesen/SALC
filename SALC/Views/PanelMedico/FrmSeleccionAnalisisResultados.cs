using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SALC.Domain;
using SALC.BLL;

namespace SALC.Views.PanelMedico
{
    public class FrmSeleccionAnalisisResultados : Form
    {
        private DataGridView gridAnalisis;
        private TextBox txtBuscar;
        private Button btnSeleccionar, btnCancelar;
        private List<AnalisisViewModel> _analisisViewModel;
        private List<AnalisisViewModel> _analisisFiltrados;
        private readonly int _dniMedico;
        
        public Analisis AnalisisSeleccionado { get; private set; }

        public FrmSeleccionAnalisisResultados(int dniMedico)
        {
            _dniMedico = dniMedico;
            InitializeComponent();
            CargarAnalisis();
        }

        private void InitializeComponent()
        {
            Text = "Seleccionar An�lisis para Cargar Resultados";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // T�tulo
            var lblTitulo = new Label
            {
                Text = "Seleccione un an�lisis 'Sin verificar' de su autor�a",
                Left = 20, Top = 20, Width = 400, Height = 25,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold)
            };

            var lblInfo = new Label
            {
                Text = "Solo se muestran an�lisis en estado 'Sin verificar' que usted cre�",
                Left = 20, Top = 50, Width = 500, Height = 20,
                ForeColor = System.Drawing.Color.Blue
            };

            // B�squeda
            var lblBuscar = new Label { Text = "Buscar:", Left = 20, Top = 80, Width = 60 };
            txtBuscar = new TextBox 
            { 
                Left = 80, Top = 78, Width = 200
            };
            txtBuscar.TextChanged += OnBuscarTextChanged;

            // Grid
            gridAnalisis = new DataGridView
            {
                Left = 20, Top = 120, Width = 840, Height = 380,
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
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo An�lisis", DataPropertyName = "TipoAnalisis", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "Fecha Creaci�n", DataPropertyName = "FechaCreacion", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", DataPropertyName = "Estado", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Observaciones", HeaderText = "Observaciones", DataPropertyName = "Observaciones", Width = 200 }
            });

            gridAnalisis.CellDoubleClick += OnCellDoubleClick;

            // Botones
            btnSeleccionar = new Button
            {
                Text = "Seleccionar",
                Left = 680, Top = 520, Width = 90, Height = 35,
                DialogResult = DialogResult.OK,
                Enabled = false
            };
            btnSeleccionar.Click += OnSeleccionar;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Left = 780, Top = 520, Width = 90, Height = 35,
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

                // Obtener an�lisis del m�dico en estado "Sin verificar" (id_estado = 1)
                var analisisDelMedico = analisisService.ObtenerAnalisisPorMedicoCarga(_dniMedico)
                    .Where(a => a.IdEstado == 1) // Solo "Sin verificar"
                    .OrderByDescending(a => a.FechaCreacion)
                    .ToList();

                _analisisViewModel = new List<AnalisisViewModel>();

                foreach (var analisis in analisisDelMedico)
                {
                    try
                    {
                        var paciente = pacienteRepo.ObtenerPorId(analisis.DniPaciente);
                        var tipoAnalisis = catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);

                        _analisisViewModel.Add(new AnalisisViewModel
                        {
                            Analisis = analisis,
                            IdAnalisis = analisis.IdAnalisis,
                            PacienteNombre = paciente != null ? $"{paciente.Nombre} {paciente.Apellido} (DNI: {paciente.Dni})" : "Paciente no encontrado",
                            TipoAnalisis = tipoAnalisis?.Descripcion ?? "Tipo no encontrado",
                            FechaCreacion = analisis.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                            Estado = "Sin verificar",
                            Observaciones = string.IsNullOrWhiteSpace(analisis.Observaciones) ? "-" : analisis.Observaciones
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue
                        System.Diagnostics.Debug.WriteLine($"Error procesando an�lisis {analisis.IdAnalisis}: {ex.Message}");
                    }
                }

                _analisisFiltrados = _analisisViewModel.ToList();
                gridAnalisis.DataSource = _analisisViewModel;

                if (!_analisisViewModel.Any())
                {
                    MessageBox.Show("No se encontraron an�lisis 'Sin verificar' de su autor�a.", "Informaci�n", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar an�lisis: {ex.Message}", "Error", 
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

        private class AnalisisViewModel
        {
            public Analisis Analisis { get; set; }
            public int IdAnalisis { get; set; }
            public string PacienteNombre { get; set; }
            public string TipoAnalisis { get; set; }
            public string FechaCreacion { get; set; }
            public string Estado { get; set; }
            public string Observaciones { get; set; }
        }
    }
}