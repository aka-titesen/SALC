using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Views.Interfaces;
using SALC.Common;
using SALC.Presenters;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para gestión de informes de análisis según ERS v2.7
    /// Implementa RF-08 y RF-09.
    /// </summary>
    public class InformesAnalisisForm : Form, IVistaInformes
    {
        // Eventos
        public event EventHandler CargarInformes;
        public event EventHandler<string> BuscarInformes;
        public event EventHandler<FiltroInforme> FiltrarInformes;
        public event EventHandler<int> VerInformeCompleto;
        public event EventHandler<int> ExportarInformePdf;
        public event EventHandler<int> ExportarInformeCsv;

        // Propiedades
        public List<InformeAnalisis> Informes { get; set; } = new List<InformeAnalisis>();
        public InformeAnalisis InformeSeleccionado { get; set; }
        public string RolUsuarioActual { get; set; }

        // Controles UI
        private Panel panelSuperior;
        private Panel panelContenido;
        private Panel panelLista;
        private Panel panelDetalles;
        private DataGridView dgvInformes;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnFiltrosAvanzados;
        private Button btnExportarPdf;
        private Button btnExportarCsv;
        private Button btnVerCompleto;
        private Panel panelFiltros;
        private DateTimePicker dtpFechaDesde;
        private DateTimePicker dtpFechaHasta;
        private ComboBox cmbTipoAnalisis;
        private ComboBox cmbEstadoAnalisis;
        private Button btnAplicarFiltros;
        private Button btnLimpiarFiltros;
        private Label lblInfoPaciente;
        private Label lblInfoAnalisis;
        private DataGridView dgvResultados;
        private TextBox txtObservaciones;
        private Label lblInfoMedico;
        private Label lblEstadoInforme;

        private PresentadorInformes _presentador;

        public InformesAnalisisForm(string rolUsuario)
        {
            RolUsuarioActual = rolUsuario;
            InitializeComponent();
            InicializarComponentesPersonalizados();
            VerificarPermisosAcceso();

            _presentador = new PresentadorInformes(this);
            CargarInformes?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeComponent()
        {
            Text = "SALC - Informes de Análisis";
            Size = new Size(1400, 900);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = SALCColors.Background;
            WindowState = FormWindowState.Maximized;
        }

        private void InicializarComponentesPersonalizados()
        {
            CrearPanelSuperior();
            CrearPanelContenido();
            ConfigurarPanelLista();
            ConfigurarPanelDetalles();
            ConfigurarPanelFiltros();
        }

        private void VerificarPermisosAcceso()
        {
            if (RolUsuarioActual != "Médico" && RolUsuarioActual != "Asistente")
            {
                MessageBox.Show("Acceso denegado. Solo roles de Médico y Asistente pueden acceder a esta funcionalidad.",
                    "Acceso Restringido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Close();
            }
        }

        private void CrearPanelSuperior()
        {
            panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = SALCColors.Primary,
                Padding = new Padding(20)
            };

            var lblTitulo = new Label
            {
                Text = "INFORMES DE ANÁLISIS",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            txtBuscar = new TextBox
            {
                Text = "Buscar por nombre o DNI de paciente...",
                ForeColor = Color.Gray,
                Size = new Size(300, 30),
                Location = new Point(400, 20),
                Font = new Font("Segoe UI", 10)
            };
            txtBuscar.GotFocus += (s, e) =>
            {
                if (txtBuscar.Text == "Buscar por nombre o DNI de paciente...")
                {
                    txtBuscar.Text = string.Empty;
                    txtBuscar.ForeColor = Color.Black;
                }
            };
            txtBuscar.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtBuscar.Text))
                {
                    txtBuscar.Text = "Buscar por nombre o DNI de paciente...";
                    txtBuscar.ForeColor = Color.Gray;
                }
            };

            btnBuscar = CrearBoton("Buscar", SALCColors.Secondary, new Point(710, 20));
            btnBuscar.Click += (s, e) =>
            {
                var texto = txtBuscar.Text == "Buscar por nombre o DNI de paciente..." ? string.Empty : txtBuscar.Text;
                BuscarInformes?.Invoke(this, texto);
            };

            btnFiltrosAvanzados = CrearBoton("Filtros", SALCColors.Info, new Point(840, 20));
            btnFiltrosAvanzados.Click += (s, e) => AlternarPanelFiltros();

            panelSuperior.Controls.AddRange(new Control[] { lblTitulo, txtBuscar, btnBuscar, btnFiltrosAvanzados });
            Controls.Add(panelSuperior);
        }

        private void CrearPanelContenido()
        {
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SALCColors.Background,
                Padding = new Padding(20)
            };

            panelLista = new Panel
            {
                Dock = DockStyle.Left,
                Width = 800,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 20, 0)
            };

            panelDetalles = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            panelContenido.Controls.Add(panelLista);
            panelContenido.Controls.Add(panelDetalles);
            Controls.Add(panelContenido);
        }

        private Button CrearBoton(string texto, Color colorFondo, Point ubicacion)
        {
            return new Button
            {
                Text = texto,
                BackColor = colorFondo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 30),
                Location = ubicacion,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        private void ConfigurarPanelLista()
        {
            dgvInformes = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false
            };

            dgvInformes.Columns.Clear();
            dgvInformes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Paciente", HeaderText = "Paciente", DataPropertyName = "NombrePaciente", Width = 150 });
            dgvInformes.Columns.Add(new DataGridViewTextBoxColumn { Name = "DNI", HeaderText = "DNI Paciente", DataPropertyName = "IdPaciente", Width = 100 });
            dgvInformes.Columns.Add(new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo de Análisis", DataPropertyName = "TipoAnalisis", Width = 120 });
            dgvInformes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Fecha", DataPropertyName = "FechaAnalisis", Width = 100 });
            dgvInformes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", DataPropertyName = "Estado", Width = 100 });
            dgvInformes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Medico", HeaderText = "Médico", DataPropertyName = "NombreMedico", Width = 120 });

            dgvInformes.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvInformes.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvInformes.RowsDefaultCellStyle.SelectionBackColor = SALCColors.PrimaryHover;
            dgvInformes.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvInformes.SelectionChanged += (s, e) =>
            {
                if (dgvInformes.CurrentRow?.DataBoundItem is InformeAnalisis sel)
                {
                    InformeSeleccionado = sel;
                    CargarDetallesInformeSeleccionado();
                }
            };
            dgvInformes.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    VerInformeCompleto?.Invoke(this, e.RowIndex);
                }
            };

            panelLista.Controls.Add(dgvInformes);
        }

        private void ConfigurarPanelDetalles()
        {
            var contenedorDetalles = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var lblTituloDetalles = new Label
            {
                Text = "DETALLES DEL INFORME",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.Primary,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            lblInfoPaciente = new Label { Text = "Paciente: ", AutoSize = true, Location = new Point(0, 40) };
            lblInfoAnalisis = new Label { Text = "Análisis: ", Font = new Font("Segoe UI", 10), AutoSize = true, Location = new Point(0, 70) };

            dgvResultados = new DataGridView
            {
                Location = new Point(0, 100),
                Size = new Size(600, 200),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                RowHeadersVisible = false
            };

            var lblObservaciones = new Label { Text = "Observaciones:", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Location = new Point(0, 310) };
            txtObservaciones = new TextBox { Location = new Point(0, 330), Size = new Size(600, 60), Multiline = true, ReadOnly = true, BorderStyle = BorderStyle.FixedSingle };
            lblInfoMedico = new Label { Text = "Médico: ", Font = new Font("Segoe UI", 10), AutoSize = true, Location = new Point(0, 400) };
            lblEstadoInforme = new Label { Text = "Estado: ", Font = new Font("Segoe UI", 10), AutoSize = true, Location = new Point(0, 430) };

            btnVerCompleto = CrearBoton("Ver Completo", SALCColors.Primary, new Point(0, 470));
            btnVerCompleto.Click += (s, e) => { if (dgvInformes.CurrentRow?.Index >= 0) VerInformeCompleto?.Invoke(this, dgvInformes.CurrentRow.Index); };

            btnExportarPdf = CrearBoton("Exportar PDF", SALCColors.Success, new Point(130, 470));
            btnExportarPdf.Click += (s, e) => { if (dgvInformes.CurrentRow?.Index >= 0) ExportarInformePdf?.Invoke(this, dgvInformes.CurrentRow.Index); };

            btnExportarCsv = CrearBoton("Exportar CSV", SALCColors.Info, new Point(260, 470));
            btnExportarCsv.Click += (s, e) => { if (dgvInformes.CurrentRow?.Index >= 0) ExportarInformeCsv?.Invoke(this, dgvInformes.CurrentRow.Index); };

            contenedorDetalles.Controls.AddRange(new Control[]
            {
                lblTituloDetalles, lblInfoPaciente, lblInfoAnalisis, dgvResultados,
                lblObservaciones, txtObservaciones, lblInfoMedico, lblEstadoInforme,
                btnVerCompleto, btnExportarPdf, btnExportarCsv
            });

            panelDetalles.Controls.Add(contenedorDetalles);
        }

        private void ConfigurarPanelFiltros()
        {
            panelFiltros = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = SALCColors.BackgroundLight,
                Padding = new Padding(20, 10, 20, 10),
                Visible = false
            };

            var lblFechaDesde = new Label { Text = "Desde:", AutoSize = true, Location = new Point(0, 10), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            dtpFechaDesde = new DateTimePicker { Location = new Point(60, 10), Size = new Size(120, 25) };

            var lblFechaHasta = new Label { Text = "Hasta:", AutoSize = true, Location = new Point(190, 10), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            dtpFechaHasta = new DateTimePicker { Location = new Point(240, 10), Size = new Size(120, 25) };

            var lblTipoAnalisis = new Label { Text = "Tipo Análisis:", AutoSize = true, Location = new Point(370, 10), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            cmbTipoAnalisis = new ComboBox { Location = new Point(460, 10), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            var lblEstadoAnalisis = new Label { Text = "Estado:", AutoSize = true, Location = new Point(620, 10), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            cmbEstadoAnalisis = new ComboBox { Location = new Point(670, 10), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            btnAplicarFiltros = CrearBoton("Aplicar", SALCColors.Success, new Point(800, 10));
            btnAplicarFiltros.Click += (s, e) =>
            {
                var filtro = new FiltroInforme
                {
                    FechaDesde = dtpFechaDesde.Value,
                    FechaHasta = dtpFechaHasta.Value,
                    TipoAnalisis = cmbTipoAnalisis.SelectedItem?.ToString(),
                    EstadoAnalisis = cmbEstadoAnalisis.SelectedItem?.ToString()
                };
                FiltrarInformes?.Invoke(this, filtro);
            };

            btnLimpiarFiltros = CrearBoton("Limpiar", SALCColors.Warning, new Point(930, 10));
            btnLimpiarFiltros.Click += (s, e) =>
            {
                dtpFechaDesde.Value = DateTime.Today.AddMonths(-1);
                dtpFechaHasta.Value = DateTime.Today;
                cmbTipoAnalisis.SelectedIndex = -1;
                cmbEstadoAnalisis.SelectedIndex = -1;
            };

            panelFiltros.Controls.AddRange(new Control[]
            {
                lblFechaDesde, dtpFechaDesde, lblFechaHasta, dtpFechaHasta,
                lblTipoAnalisis, cmbTipoAnalisis, lblEstadoAnalisis, cmbEstadoAnalisis,
                btnAplicarFiltros, btnLimpiarFiltros
            });

            Controls.Add(panelFiltros);
            Controls.SetChildIndex(panelFiltros, 1);
        }

        private void AlternarPanelFiltros()
        {
            panelFiltros.Visible = !panelFiltros.Visible;
            if (panelFiltros.Visible)
            {
                cmbTipoAnalisis.Items.Clear();
                cmbTipoAnalisis.Items.AddRange(new object[] { "Todos", "Hemograma Completo", "Glucosa en Ayunas", "Perfil Lipídico Completo", "Análisis de Orina Completo" });
                cmbTipoAnalisis.SelectedIndex = 0;

                cmbEstadoAnalisis.Items.Clear();
                cmbEstadoAnalisis.Items.AddRange(new object[] { "Todos", "Sin verificar", "Verificado" });
                cmbEstadoAnalisis.SelectedIndex = 0;
            }
        }

        private void CargarDetallesInformeSeleccionado()
        {
            if (InformeSeleccionado == null) return;

            lblInfoPaciente.Text = $"Paciente: {InformeSeleccionado.NombrePaciente} (DNI: {InformeSeleccionado.IdPaciente})";
            lblInfoAnalisis.Text = $"Análisis: {InformeSeleccionado.TipoAnalisis} - {InformeSeleccionado.FechaAnalisis:dd/MM/yyyy}";
            lblInfoMedico.Text = $"Médico: {InformeSeleccionado.NombreMedico}";
            lblEstadoInforme.Text = $"Estado: {InformeSeleccionado.Estado}";
            txtObservaciones.Text = InformeSeleccionado.Observaciones;
            dgvResultados.DataSource = InformeSeleccionado.Resultados;
        }

        // Implementación IVistaInformes
        public void CargarDatosInformes(List<InformeAnalisis> informes)
        {
            Informes = informes;
            dgvInformes.DataSource = informes;
        }

        public void MostrarMensaje(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
