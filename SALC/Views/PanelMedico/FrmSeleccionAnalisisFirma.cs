using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
            Text = "Seleccionar An�lisis para Firmar";
            Width = 950;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // T�tulo
            var lblTitulo = new Label
            {
                Text = "Seleccione un an�lisis 'Sin verificar' con resultados cargados",
                Left = 20, Top = 20, Width = 500, Height = 25,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold)
            };

            var lblInfo = new Label
            {
                Text = "Solo se muestran an�lisis de su autor�a con m�tricas cargadas, listos para firmar",
                Left = 20, Top = 50, Width = 600, Height = 20,
                ForeColor = System.Drawing.Color.Blue
            };

            var lblAdvertencia = new Label
            {
                Text = "?? Una vez firmado, el an�lisis no podr� modificarse",
                Left = 20, Top = 75, Width = 400, Height = 20,
                ForeColor = System.Drawing.Color.Red,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold)
            };

            // B�squeda
            var lblBuscar = new Label { Text = "Buscar:", Left = 20, Top = 105, Width = 60 };
            txtBuscar = new TextBox 
            { 
                Left = 80, Top = 103, Width = 200
            };
            txtBuscar.TextChanged += OnBuscarTextChanged;

            // Grid
            gridAnalisis = new DataGridView
            {
                Left = 20, Top = 140, Width = 890, Height = 360,
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
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo An�lisis", DataPropertyName = "TipoAnalisis", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "Fecha Creaci�n", DataPropertyName = "FechaCreacion", Width = 110 },
                new DataGridViewTextBoxColumn { Name = "CantidadMetricas", HeaderText = "M�tricas", DataPropertyName = "CantidadMetricas", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", DataPropertyName = "Estado", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Observaciones", HeaderText = "Observaciones", DataPropertyName = "Observaciones", Width = 150 }
            });

            gridAnalisis.CellDoubleClick += OnCellDoubleClick;

            // Botones
            btnSeleccionar = new Button
            {
                Text = "Seleccionar para Firmar",
                Left = 720, Top = 520, Width = 130, Height = 35,
                DialogResult = DialogResult.OK,
                Enabled = false,
                BackColor = System.Drawing.Color.Orange
            };
            btnSeleccionar.Click += OnSeleccionar;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Left = 860, Top = 520, Width = 80, Height = 35,
                DialogResult = DialogResult.Cancel
            };

            gridAnalisis.SelectionChanged += (s, e) => 
            {
                btnSeleccionar.Enabled = gridAnalisis.CurrentRow != null;
            };

            // Agregar controles
            Controls.AddRange(new Control[] 
            { 
                lblTitulo, lblInfo, lblAdvertencia, lblBuscar, txtBuscar, gridAnalisis, btnSeleccionar, btnCancelar 
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

                _analisisViewModel = new List<AnalisisFirmaViewModel>();

                foreach (var analisis in analisisDelMedico)
                {
                    try
                    {
                        // Verificar que tenga m�tricas cargadas
                        var resultados = analisisService.ObtenerResultados(analisis.IdAnalisis).ToList();
                        if (!resultados.Any())
                        {
                            continue; // Saltar an�lisis sin resultados
                        }

                        var paciente = pacienteRepo.ObtenerPorId(analisis.DniPaciente);
                        var tipoAnalisis = catalogoService.ObtenerTiposAnalisis()
                            .FirstOrDefault(t => t.IdTipoAnalisis == analisis.IdTipoAnalisis);

                        _analisisViewModel.Add(new AnalisisFirmaViewModel
                        {
                            Analisis = analisis,
                            IdAnalisis = analisis.IdAnalisis,
                            PacienteNombre = paciente != null ? $"{paciente.Nombre} {paciente.Apellido} (DNI: {paciente.Dni})" : "Paciente no encontrado",
                            TipoAnalisis = tipoAnalisis?.Descripcion ?? "Tipo no encontrado",
                            FechaCreacion = analisis.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                            CantidadMetricas = $"{resultados.Count} m�tricas",
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
                    MessageBox.Show("No se encontraron an�lisis 'Sin verificar' con resultados cargados.\n\n" +
                                  "Para firmar un an�lisis, primero debe cargar las m�tricas en la pesta�a 'Cargar Resultados'.", 
                                  "Informaci�n", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var lblConteo = new Label
                    {
                        Text = $"Se encontraron {_analisisViewModel.Count} an�lisis listos para firmar",
                        Left = 300, Top = 105, Width = 300, Height = 20,
                        ForeColor = System.Drawing.Color.Green
                    };
                    Controls.Add(lblConteo);
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
                    var analisisViewModel = _analisisFiltrados[rowIndex];
                    
                    // Confirmaci�n adicional
                    var confirmacion = MessageBox.Show(
                        $"�Est� seguro de que desea seleccionar este an�lisis para firmar?\n\n" +
                        $"ID: {analisisViewModel.IdAnalisis}\n" +
                        $"Paciente: {analisisViewModel.PacienteNombre}\n" +
                        $"Tipo: {analisisViewModel.TipoAnalisis}\n" +
                        $"M�tricas: {analisisViewModel.CantidadMetricas}\n\n" +
                        "Una vez firmado, no podr� modificarse.",
                        "Confirmar Selecci�n para Firma",
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