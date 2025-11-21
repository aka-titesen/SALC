using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
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
            Text = "Seleccionar Paciente para Análisis";
            Width = 900;
            Height = 650;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            ShowIcon = false;

            // Título principal
            var lblTitulo = new Label
            {
                Text = "Selección de Paciente Activo",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(20, 15),
                Size = new Size(850, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = "Seleccione el paciente para el que desea crear un nuevo análisis clínico",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(850, 20),
                BackColor = Color.Transparent
            };

            // Panel informativo
            var panelInfo = new Panel
            {
                Location = new Point(20, 75),
                Size = new Size(840, 40),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblInfo = new Label
            {
                Text = "Solo se muestran pacientes con estado 'Activo'.\n" +
                       "Puede buscar por DNI, nombre o apellido del paciente.",
                Location = new Point(15, 8),
                Size = new Size(810, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                BackColor = Color.Transparent
            };
            panelInfo.Controls.Add(lblInfo);

            // Búsqueda
            var lblBuscar = new Label 
            { 
                Text = "Buscar:", 
                Left = 20, 
                Top = 130, 
                Width = 80,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            
            txtBuscar = new TextBox 
            { 
                Left = 110, 
                Top = 128, 
                Width = 300,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtBuscar.TextChanged += OnBuscarTextChanged;

            var lblAyuda = new Label
            {
                Text = "Escriba DNI, nombre o apellido para filtrar",
                Left = 420,
                Top = 130,
                Width = 300,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166)
            };

            // Grid de pacientes
            gridPacientes = new DataGridView
            {
                Left = 20, 
                Top = 170, 
                Width = 840, 
                Height = 380,
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
                    BackColor = Color.FromArgb(52, 152, 219),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Padding = new Padding(8),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(209, 231, 248),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(5)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 252, 255),
                    Padding = new Padding(5)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 35 }
            };

            // Configurar columnas
            gridPacientes.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Dni", HeaderText = "DNI", DataPropertyName = "Dni", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Apellido", HeaderText = "Apellido", DataPropertyName = "Apellido", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", DataPropertyName = "Nombre", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "FechaNac", HeaderText = "Fecha Nac.", DataPropertyName = "FechaNac", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Sexo", HeaderText = "Sexo", DataPropertyName = "Sexo", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", DataPropertyName = "Telefono", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "ObraSocial", HeaderText = "Obra Social", DataPropertyName = "ObraSocial", Width = 140 }
            });

            gridPacientes.CellDoubleClick += OnCellDoubleClick;

            // Botones
            btnSeleccionar = new Button
            {
                Text = "Seleccionar Paciente",
                Left = 630, 
                Top = 570, 
                Width = 150, 
                Height = 40,
                DialogResult = DialogResult.OK,
                Enabled = false,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSeleccionar.FlatAppearance.BorderSize = 0;
            btnSeleccionar.Click += OnSeleccionar;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Left = 790, 
                Top = 570, 
                Width = 90, 
                Height = 40,
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            gridPacientes.SelectionChanged += (s, e) => 
            {
                btnSeleccionar.Enabled = gridPacientes.CurrentRow != null;
            };

            // Agregar controles
            Controls.AddRange(new Control[] 
            { 
                lblTitulo, lblSubtitulo, panelInfo,
                lblBuscar, txtBuscar, lblAyuda,
                gridPacientes, 
                btnSeleccionar, btnCancelar 
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
                        Apellido = paciente.Apellido,
                        Nombre = paciente.Nombre,
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
                    Apellido = paciente.Apellido,
                    Nombre = paciente.Nombre,
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