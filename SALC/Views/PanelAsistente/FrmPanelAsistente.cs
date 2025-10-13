using System;
using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;
using System.Collections.Generic;
using SALC.Domain;
using System.Linq;

namespace SALC.Views.PanelAsistente
{
    public class FrmPanelAsistente : Form, IPanelAsistenteView
    {
        // Controles principales
        private GroupBox groupPacientes;
        private TextBox txtBusqueda;
        private Button btnBuscar;
        private DataGridView gridPacientes;
        private Button btnVerHistorial;
        
        private GroupBox groupHistorial;
        private DataGridView gridHistorial;
        
        private GroupBox groupAcciones;
        private Button btnGenerarPdf;
        private Button btnEnviarInforme;
        private Label lblInstrucciones;

        public FrmPanelAsistente()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            
            // Inicializar el presenter después de que la vista esté completamente cargada
            var presenter = Tag as Presenters.PanelAsistentePresenter;
            presenter?.InicializarVista();
        }

        private void InitializeComponent()
        {
            Text = "Panel de Asistente - SALC";
            Size = new System.Drawing.Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;

            // GroupBox para pacientes
            groupPacientes = new GroupBox
            {
                Text = "Lista de Pacientes",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(580, 350)
            };

            // Búsqueda de pacientes
            var lblBusqueda = new Label
            {
                Text = "Buscar por DNI o Apellido:",
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(150, 20)
            };

            txtBusqueda = new TextBox
            {
                Location = new System.Drawing.Point(165, 23),
                Size = new System.Drawing.Size(200, 20)
            };

            btnBuscar = new Button
            {
                Text = "Buscar",
                Location = new System.Drawing.Point(375, 21),
                Size = new System.Drawing.Size(80, 25)
            };

            // Grid de pacientes
            gridPacientes = new DataGridView
            {
                Location = new System.Drawing.Point(10, 55),
                Size = new System.Drawing.Size(560, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnVerHistorial = new Button
            {
                Text = "Ver Historial del Paciente Seleccionado",
                Location = new System.Drawing.Point(10, 315),
                Size = new System.Drawing.Size(250, 25)
            };

            groupPacientes.Controls.AddRange(new Control[] { lblBusqueda, txtBusqueda, btnBuscar, gridPacientes, btnVerHistorial });

            // GroupBox para historial
            groupHistorial = new GroupBox
            {
                Text = "Historial de Análisis",
                Location = new System.Drawing.Point(600, 10),
                Size = new System.Drawing.Size(580, 350)
            };

            gridHistorial = new DataGridView
            {
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(560, 315),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            groupHistorial.Controls.Add(gridHistorial);

            // GroupBox para acciones
            groupAcciones = new GroupBox
            {
                Text = "Acciones sobre Análisis Verificados",
                Location = new System.Drawing.Point(10, 370),
                Size = new System.Drawing.Size(1170, 120)
            };

            lblInstrucciones = new Label
            {
                Text = "Seleccione un análisis en estado 'Verificado' para habilitar las acciones de generación y envío de informes.",
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(800, 20),
                ForeColor = System.Drawing.Color.DarkBlue
            };

            btnGenerarPdf = new Button
            {
                Text = "Generar PDF del Análisis",
                Location = new System.Drawing.Point(10, 55),
                Size = new System.Drawing.Size(200, 30),
                Enabled = false
            };

            btnEnviarInforme = new Button
            {
                Text = "Enviar Informe al Paciente",
                Location = new System.Drawing.Point(220, 55),
                Size = new System.Drawing.Size(200, 30),
                Enabled = false
            };

            groupAcciones.Controls.AddRange(new Control[] { lblInstrucciones, btnGenerarPdf, btnEnviarInforme });

            // Agregar todos los grupos al formulario
            Controls.AddRange(new Control[] { groupPacientes, groupHistorial, groupAcciones });

            // Eventos
            btnBuscar.Click += (s, e) => BuscarPacientesClick?.Invoke(this, EventArgs.Empty);
            btnVerHistorial.Click += (s, e) => VerHistorialClick?.Invoke(this, EventArgs.Empty);
            btnGenerarPdf.Click += (s, e) => GenerarPdfClick?.Invoke(this, EventArgs.Empty);
            btnEnviarInforme.Click += (s, e) => EnviarInformeClick?.Invoke(this, EventArgs.Empty);
            
            // Evento cuando cambia la selección en el grid de historial
            gridHistorial.SelectionChanged += (s, e) => {
                if (gridHistorial.SelectedRows.Count > 0)
                {
                    var presenter = Tag as Presenters.PanelAsistentePresenter;
                    presenter?.SeleccionAnalisisCambiada();
                }
            };

            // Permitir búsqueda con Enter
            txtBusqueda.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BuscarPacientesClick?.Invoke(this, EventArgs.Empty);
                }
            };
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
                ObraSocial = p.IdObraSocial?.ToString() ?? "Sin obra social"
            }).ToList();

            gridPacientes.DataSource = lista;
            
            if (gridPacientes.Columns.Count > 0)
            {
                gridPacientes.Columns["DNI"].Width = 80;
                gridPacientes.Columns["Nombre"].Width = 100;
                gridPacientes.Columns["Apellido"].Width = 100;
                gridPacientes.Columns["FechaNacimiento"].Width = 100;
                gridPacientes.Columns["Sexo"].Width = 50;
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

        public void CargarHistorialAnalisis(IEnumerable<object> analisisConEstados)
        {
            if (gridHistorial == null) return;

            gridHistorial.DataSource = analisisConEstados.ToList();
            
            if (gridHistorial.Columns.Count > 0)
            {
                gridHistorial.Columns["IdAnalisis"].Width = 60;
                gridHistorial.Columns["TipoAnalisis"].Width = 150;
                gridHistorial.Columns["Estado"].Width = 80;
                gridHistorial.Columns["FechaCreacion"].Width = 120;
                gridHistorial.Columns["FechaFirma"].Width = 120;
                
                // Colorear filas según el estado
                foreach (DataGridViewRow row in gridHistorial.Rows)
                {
                    var estado = row.Cells["Estado"].Value?.ToString();
                    switch (estado)
                    {
                        case "Verificado":
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "Sin verificar":
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                            break;
                        case "Anulado":
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                            break;
                    }
                }
            }
        }

        public object AnalisisSeleccionado
        {
            get
            {
                if (gridHistorial?.SelectedRows.Count == 0)
                    return null;

                var row = gridHistorial.SelectedRows[0];
                return new
                {
                    IdAnalisis = (int)row.Cells["IdAnalisis"].Value,
                    TipoAnalisis = row.Cells["TipoAnalisis"].Value.ToString(),
                    Estado = row.Cells["Estado"].Value.ToString(),
                    FechaCreacion = row.Cells["FechaCreacion"].Value.ToString(),
                    FechaFirma = row.Cells["FechaFirma"].Value.ToString(),
                    MedicoCarga = row.Cells["MedicoCarga"].Value.ToString(),
                    MedicoFirma = row.Cells["MedicoFirma"].Value.ToString(),
                    Observaciones = row.Cells["Observaciones"].Value.ToString()
                };
            }
        }

        public void HabilitarAccionesAnalisis(bool habilitar)
        {
            if (btnGenerarPdf != null) btnGenerarPdf.Enabled = habilitar;
            if (btnEnviarInforme != null) btnEnviarInforme.Enabled = habilitar;
            
            if (lblInstrucciones != null)
            {
                if (habilitar)
                {
                    lblInstrucciones.Text = "Análisis verificado seleccionado. Puede generar PDF y enviar el informe al paciente.";
                    lblInstrucciones.ForeColor = System.Drawing.Color.DarkGreen;
                }
                else
                {
                    lblInstrucciones.Text = "Seleccione un análisis en estado 'Verificado' para habilitar las acciones de generación y envío de informes.";
                    lblInstrucciones.ForeColor = System.Drawing.Color.DarkBlue;
                }
            }
        }

        public void MostrarMensaje(string texto, bool esError = false)
        {
            var icon = esError ? MessageBoxIcon.Error : MessageBoxIcon.Information;
            var titulo = esError ? "Error - Panel Asistente" : "Información - Panel Asistente";
            MessageBox.Show(this, texto, titulo, MessageBoxButtons.OK, icon);
        }
    }
}
