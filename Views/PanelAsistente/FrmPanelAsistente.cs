using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Presenters.ViewsContracts;
using System.Collections.Generic;
using SALC.Domain;
using System.Linq;

namespace SALC.Views.PanelAsistente
{
    public class FrmPanelAsistente : Form, IPanelAsistenteView
    {
        private Panel panelPrincipal;
        private DataGridView gridPacientes;
        private TextBox txtBusqueda;
        private Button btnVerHistorial;

        public FrmPanelAsistente()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            
            var presenter = Tag as Presenters.PanelAsistentePresenter;
            presenter?.InicializarVista();
        }

        private void InitializeComponent()
        {
            Text = "Consulta de Historiales Clínicos";
            Size = new Size(1300, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Panel principal
            panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(30)
            };

            // Título principal
            var lblTitulo = new Label
            {
                Text = "Consulta de Pacientes y Historiales Médicos",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(0, 0),
                Size = new Size(900, 35),
                BackColor = Color.Transparent
            };

            // Subtítulo
            var lblSubtitulo = new Label
            {
                Text = "Seleccione un paciente para acceder a su historial clínico completo",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(0, 40),
                Size = new Size(1000, 25),
                BackColor = Color.Transparent
            };

            // Panel de búsqueda
            var panelBusqueda = new Panel
            {
                Location = new Point(0, 75),
                Size = new Size(1220, 50),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblBusqueda = new Label
            {
                Text = "Buscar Paciente:",
                Location = new Point(15, 14),
                Size = new Size(130, 22),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BackColor = Color.Transparent
            };

            txtBusqueda = new TextBox
            {
                Location = new Point(150, 12),
                Size = new Size(400, 26),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblInfoBusqueda = new Label
            {
                Text = "Búsqueda en tiempo real - Ingrese DNI o Apellido del paciente",
                Location = new Point(565, 14),
                Size = new Size(450, 22),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166),
                BackColor = Color.Transparent
            };

            btnVerHistorial = new Button
            {
                Text = "Ver Historial Completo",
                Location = new Point(1020, 10),
                Size = new Size(180, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnVerHistorial.FlatAppearance.BorderSize = 0;

            panelBusqueda.Controls.AddRange(new Control[] { 
                lblBusqueda, txtBusqueda, lblInfoBusqueda, btnVerHistorial 
            });

            // Grid de pacientes 
            gridPacientes = new DataGridView
            {
                Location = new Point(0, 140),
                Size = new Size(1220, 480),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(176, 196, 222),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(6)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 250, 255)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 }
            };

            panelPrincipal.Controls.AddRange(new Control[] { 
                lblTitulo, lblSubtitulo, panelBusqueda, gridPacientes 
            });

            Controls.Add(panelPrincipal);

            // Eventos 
            txtBusqueda.TextChanged += (s, e) => BuscarPacientesClick?.Invoke(this, EventArgs.Empty);
            btnVerHistorial.Click += (s, e) => VerHistorialClick?.Invoke(this, EventArgs.Empty);
        }

        // Implementación de IPanelAsistenteView
        public event EventHandler BuscarPacientesClick;
        public event EventHandler VerHistorialClick;
        public event EventHandler GenerarPdfClick;
        public event EventHandler EnviarInformeClick;

        public string BusquedaPacienteTexto => txtBusqueda?.Text?.Trim();

        public void CargarListaPacientes(IEnumerable<Paciente> pacientes)
        {
            if (gridPacientes == null) return;

            var lista = pacientes.Select(p => new
            {
                DNI = p.Dni,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                FechaNacimiento = p.FechaNac.ToString("dd/MM/yyyy"),
                Sexo = p.Sexo,
                Email = p.Email ?? "Sin email",
                Telefono = p.Telefono ?? "Sin teléfono",
                Estado = p.Estado
            }).ToList();

            gridPacientes.DataSource = lista;
            
            if (gridPacientes.Columns.Count > 0)
            {
                gridPacientes.Columns["DNI"].Width = 90;
                gridPacientes.Columns["Nombre"].Width = 150;
                gridPacientes.Columns["Apellido"].Width = 150;
                gridPacientes.Columns["FechaNacimiento"].Width = 120;
                gridPacientes.Columns["Sexo"].Width = 60;
                gridPacientes.Columns["Estado"].Width = 90;
            }
        }

        public Paciente PacienteSeleccionado
        {
            get
            {
                if (gridPacientes?.SelectedRows.Count == 0)
                    return null;

                var row = gridPacientes.SelectedRows[0];
                return new Paciente
                {
                    Dni = (int)row.Cells["DNI"].Value,
                    Nombre = row.Cells["Nombre"].Value.ToString(),
                    Apellido = row.Cells["Apellido"].Value.ToString(),
                    Email = row.Cells["Email"].Value.ToString(),
                    Telefono = row.Cells["Telefono"].Value.ToString()
                };
            }
        }

        public void MostrarMensaje(string texto, bool esError = false)
        {
            var icon = esError ? MessageBoxIcon.Error : MessageBoxIcon.Information;
            var titulo = esError ? "SALC - Error en Panel Asistente" : "SALC - Panel Asistente";
            MessageBox.Show(this, texto, titulo, MessageBoxButtons.OK, icon);
        }
    }
}
