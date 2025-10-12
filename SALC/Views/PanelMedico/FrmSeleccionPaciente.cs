using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SALC.Domain;
using SALC.BLL;

namespace SALC.Views.PanelMedico
{
    public class FrmSeleccionPaciente : Form
    {
        private DataGridView gridPacientes;
        private TextBox txtBuscar;
        private Button btnSeleccionar, btnCancelar;
        private List<Paciente> _pacientes;
        private List<Paciente> _pacientesFiltrados;
        
        public Paciente PacienteSeleccionado { get; private set; }

        public FrmSeleccionPaciente()
        {
            InitializeComponent();
            CargarPacientes();
        }

        private void InitializeComponent()
        {
            Text = "Seleccionar Paciente";
            Width = 800;
            Height = 600;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Título
            var lblTitulo = new Label
            {
                Text = "Seleccione un paciente activo",
                Left = 20, Top = 20, Width = 300, Height = 25,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold)
            };

            // Búsqueda
            var lblBuscar = new Label { Text = "Buscar:", Left = 20, Top = 60, Width = 60 };
            txtBuscar = new TextBox 
            { 
                Left = 80, Top = 58, Width = 200
            };
            txtBuscar.TextChanged += OnBuscarTextChanged;

            // Grid
            gridPacientes = new DataGridView
            {
                Left = 20, Top = 100, Width = 740, Height = 400,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Configurar columnas
            gridPacientes.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Dni", HeaderText = "DNI", DataPropertyName = "Dni", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", DataPropertyName = "Nombre", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Apellido", HeaderText = "Apellido", DataPropertyName = "Apellido", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "FechaNac", HeaderText = "Fecha Nac.", DataPropertyName = "FechaNac", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Sexo", HeaderText = "Sexo", DataPropertyName = "Sexo", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", DataPropertyName = "Telefono", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "ObraSocial", HeaderText = "Obra Social", DataPropertyName = "ObraSocial", Width = 150 }
            });

            gridPacientes.CellDoubleClick += OnCellDoubleClick;

            // Botones
            btnSeleccionar = new Button
            {
                Text = "Seleccionar",
                Left = 580, Top = 520, Width = 90, Height = 35,
                DialogResult = DialogResult.OK,
                Enabled = false
            };
            btnSeleccionar.Click += OnSeleccionar;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Left = 680, Top = 520, Width = 90, Height = 35,
                DialogResult = DialogResult.Cancel
            };

            gridPacientes.SelectionChanged += (s, e) => 
            {
                btnSeleccionar.Enabled = gridPacientes.CurrentRow != null;
            };

            // Agregar controles
            Controls.AddRange(new Control[] 
            { 
                lblTitulo, lblBuscar, txtBuscar, gridPacientes, btnSeleccionar, btnCancelar 
            });

            AcceptButton = btnSeleccionar;
            CancelButton = btnCancelar;
        }

        private void CargarPacientes()
        {
            try
            {
                var pacienteService = new PacienteService();
                var obraSocialRepo = new SALC.DAL.ObraSocialRepositorio();
                
                _pacientes = pacienteService.ObtenerTodos()
                    .Where(p => p.Estado == "Activo")
                    .OrderBy(p => p.Apellido)
                    .ThenBy(p => p.Nombre)
                    .ToList();

                // Crear ViewModels con obra social
                var pacientesViewModel = new List<object>();
                foreach (var paciente in _pacientes)
                {
                    string obraSocialNombre = "-";
                    if (paciente.IdObraSocial.HasValue)
                    {
                        try
                        {
                            var obraSocial = obraSocialRepo.ObtenerPorId(paciente.IdObraSocial.Value);
                            obraSocialNombre = obraSocial?.Nombre ?? "-";
                        }
                        catch { }
                    }

                    pacientesViewModel.Add(new
                    {
                        Dni = paciente.Dni,
                        Nombre = paciente.Nombre,
                        Apellido = paciente.Apellido,
                        FechaNac = paciente.FechaNac.ToString("dd/MM/yyyy"),
                        Sexo = ObtenerDescripcionSexo(paciente.Sexo),
                        Telefono = string.IsNullOrWhiteSpace(paciente.Telefono) ? "-" : paciente.Telefono,
                        ObraSocial = obraSocialNombre
                    });
                }

                _pacientesFiltrados = _pacientes.ToList();
                gridPacientes.DataSource = pacientesViewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerDescripcionSexo(char sexo)
        {
            switch (sexo)
            {
                case 'M': return "Masculino";
                case 'F': return "Femenino";
                case 'X': return "Otro";
                default: return "-";
            }
        }

        private void OnBuscarTextChanged(object sender, EventArgs e)
        {
            var filtro = txtBuscar.Text.Trim().ToLowerInvariant();
            
            if (string.IsNullOrEmpty(filtro))
            {
                _pacientesFiltrados = _pacientes.ToList();
            }
            else
            {
                _pacientesFiltrados = _pacientes.Where(p =>
                    p.Dni.ToString().Contains(filtro) ||
                    p.Nombre.ToLowerInvariant().Contains(filtro) ||
                    p.Apellido.ToLowerInvariant().Contains(filtro)
                ).ToList();
            }

            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            var obraSocialRepo = new SALC.DAL.ObraSocialRepositorio();
            var pacientesViewModel = new List<object>();
            
            foreach (var paciente in _pacientesFiltrados)
            {
                string obraSocialNombre = "-";
                if (paciente.IdObraSocial.HasValue)
                {
                    try
                    {
                        var obraSocial = obraSocialRepo.ObtenerPorId(paciente.IdObraSocial.Value);
                        obraSocialNombre = obraSocial?.Nombre ?? "-";
                    }
                    catch { }
                }

                pacientesViewModel.Add(new
                {
                    Dni = paciente.Dni,
                    Nombre = paciente.Nombre,
                    Apellido = paciente.Apellido,
                    FechaNac = paciente.FechaNac.ToString("dd/MM/yyyy"),
                    Sexo = ObtenerDescripcionSexo(paciente.Sexo),
                    Telefono = string.IsNullOrWhiteSpace(paciente.Telefono) ? "-" : paciente.Telefono,
                    ObraSocial = obraSocialNombre
                });
            }

            gridPacientes.DataSource = pacientesViewModel;
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
            if (gridPacientes.CurrentRow != null)
            {
                var rowIndex = gridPacientes.CurrentRow.Index;
                if (rowIndex < _pacientesFiltrados.Count)
                {
                    PacienteSeleccionado = _pacientesFiltrados[rowIndex];
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }
    }
}