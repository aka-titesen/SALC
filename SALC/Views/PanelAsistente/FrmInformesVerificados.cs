using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SALC.Domain;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelAsistente
{
    public class FrmInformesVerificados : Form, IInformesVerificadosView
    {
        // Controles principales
        private GroupBox groupFiltros;
        private Label lblFechaDesde, lblFechaHasta;
        private DateTimePicker dtpFechaDesde, dtpFechaHasta;
        private ComboBox cboMedicoFirma;
        private TextBox txtBuscarPaciente;
        private Button btnBuscar, btnLimpiarFiltros;

        private GroupBox groupAnalisis;
        private DataGridView gridAnalisisVerificados;
        private Label lblConteoAnalisis;

        private GroupBox groupAcciones;
        private Button btnGenerarPdf, btnEnviarEmail, btnEnviarWhatsApp;
        private Label lblAnalisisSeleccionado;
        private RichTextBox txtObservacionesEnvio;

        public FrmInformesVerificados()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Informes Verificados - Asistente";
            Size = new System.Drawing.Size(1200, 700);
            StartPosition = FormStartPosition.CenterScreen;

            // GroupBox de Filtros
            groupFiltros = new GroupBox
            {
                Text = "Filtros de Búsqueda",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(1160, 100)
            };

            // Filtros de fecha
            lblFechaDesde = new Label
            {
                Text = "Desde:",
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(50, 20)
            };

            dtpFechaDesde = new DateTimePicker
            {
                Location = new System.Drawing.Point(65, 23),
                Size = new System.Drawing.Size(120, 20),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1) // Por defecto último mes
            };

            lblFechaHasta = new Label
            {
                Text = "Hasta:",
                Location = new System.Drawing.Point(200, 25),
                Size = new System.Drawing.Size(50, 20)
            };

            dtpFechaHasta = new DateTimePicker
            {
                Location = new System.Drawing.Point(255, 23),
                Size = new System.Drawing.Size(120, 20),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            // Filtro por médico que firmó
            var lblMedico = new Label
            {
                Text = "Médico:",
                Location = new System.Drawing.Point(390, 25),
                Size = new System.Drawing.Size(50, 20)
            };

            cboMedicoFirma = new ComboBox
            {
                Location = new System.Drawing.Point(445, 23),
                Size = new System.Drawing.Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Búsqueda por paciente
            var lblPaciente = new Label
            {
                Text = "Paciente:",
                Location = new System.Drawing.Point(660, 25),
                Size = new System.Drawing.Size(60, 20)
            };

            txtBuscarPaciente = new TextBox
            {
                Location = new System.Drawing.Point(720, 23),
                Size = new System.Drawing.Size(150, 20)
                // PlaceholderText no disponible en .NET Framework 4.7.2
            };

            // Simulación de placeholder text
            txtBuscarPaciente.Text = "DNI o Apellido";
            txtBuscarPaciente.ForeColor = System.Drawing.Color.Gray;
            txtBuscarPaciente.Enter += (s, e) => {
                if (txtBuscarPaciente.Text == "DNI o Apellido")
                {
                    txtBuscarPaciente.Text = "";
                    txtBuscarPaciente.ForeColor = System.Drawing.Color.Black;
                }
            };
            txtBuscarPaciente.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtBuscarPaciente.Text))
                {
                    txtBuscarPaciente.Text = "DNI o Apellido";
                    txtBuscarPaciente.ForeColor = System.Drawing.Color.Gray;
                }
            };

            // Botones de filtros
            btnBuscar = new Button
            {
                Text = "Buscar",
                Location = new System.Drawing.Point(880, 21),
                Size = new System.Drawing.Size(80, 25),
                BackColor = System.Drawing.Color.LightBlue
            };

            btnLimpiarFiltros = new Button
            {
                Text = "Limpiar",
                Location = new System.Drawing.Point(970, 21),
                Size = new System.Drawing.Size(80, 25),
                BackColor = System.Drawing.Color.LightGray
            };

            groupFiltros.Controls.AddRange(new Control[] {
                lblFechaDesde, dtpFechaDesde, lblFechaHasta, dtpFechaHasta,
                lblMedico, cboMedicoFirma, lblPaciente, txtBuscarPaciente,
                btnBuscar, btnLimpiarFiltros
            });

            // GroupBox de Análisis
            groupAnalisis = new GroupBox
            {
                Text = "Análisis Verificados",
                Location = new System.Drawing.Point(10, 120),
                Size = new System.Drawing.Size(1160, 350)
            };

            lblConteoAnalisis = new Label
            {
                Text = "0 análisis encontrados",
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(200, 20),
                ForeColor = System.Drawing.Color.Blue
            };

            gridAnalisisVerificados = new DataGridView
            {
                Location = new System.Drawing.Point(10, 50),
                Size = new System.Drawing.Size(1140, 290),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Configurar columnas del grid
            gridAnalisisVerificados.AutoGenerateColumns = false;
            gridAnalisisVerificados.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "IdAnalisis", HeaderText = "ID", DataPropertyName = "IdAnalisis", Width = 60 },
                new DataGridViewTextBoxColumn { Name = "PacienteNombre", HeaderText = "Paciente", DataPropertyName = "PacienteNombre", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "DniPaciente", HeaderText = "DNI Pac.", DataPropertyName = "DniPaciente", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo Análisis", DataPropertyName = "TipoAnalisis", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "F. Creación", DataPropertyName = "FechaCreacion", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "FechaFirma", HeaderText = "F. Verificación", DataPropertyName = "FechaFirma", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "MedicoFirma", HeaderText = "Médico Verificador", DataPropertyName = "MedicoFirma", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "EmailPaciente", HeaderText = "Email", DataPropertyName = "EmailPaciente", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "TelefonoPaciente", HeaderText = "Teléfono", DataPropertyName = "TelefonoPaciente", Width = 100 }
            });

            groupAnalisis.Controls.AddRange(new Control[] { lblConteoAnalisis, gridAnalisisVerificados });

            // GroupBox de Acciones
            groupAcciones = new GroupBox
            {
                Text = "Acciones sobre Análisis Seleccionado",
                Location = new System.Drawing.Point(10, 480),
                Size = new System.Drawing.Size(1160, 180)
            };

            lblAnalisisSeleccionado = new Label
            {
                Text = "Seleccione un análisis para habilitar las acciones",
                Location = new System.Drawing.Point(10, 25),
                Size = new System.Drawing.Size(800, 20),
                ForeColor = System.Drawing.Color.Gray
            };

            // Botones de acción
            btnGenerarPdf = new Button
            {
                Text = "?? Generar PDF",
                Location = new System.Drawing.Point(10, 55),
                Size = new System.Drawing.Size(150, 35),
                BackColor = System.Drawing.Color.LightCoral,
                Enabled = false
            };

            btnEnviarEmail = new Button
            {
                Text = "?? Enviar por Email",
                Location = new System.Drawing.Point(170, 55),
                Size = new System.Drawing.Size(150, 35),
                BackColor = System.Drawing.Color.LightGreen,
                Enabled = false
            };

            btnEnviarWhatsApp = new Button
            {
                Text = "?? Enviar por WhatsApp",
                Location = new System.Drawing.Point(330, 55),
                Size = new System.Drawing.Size(150, 35),
                BackColor = System.Drawing.Color.LightBlue,
                Enabled = false
            };

            // Observaciones para el envío
            var lblObservaciones = new Label
            {
                Text = "Observaciones del envío:",
                Location = new System.Drawing.Point(10, 105),
                Size = new System.Drawing.Size(150, 20)
            };

            txtObservacionesEnvio = new RichTextBox
            {
                Location = new System.Drawing.Point(170, 105),
                Size = new System.Drawing.Size(600, 60)
                // PlaceholderText no disponible en .NET Framework 4.7.2
            };

            // Simulación de placeholder text
            txtObservacionesEnvio.Text = "Mensaje adicional para el paciente (opcional)";
            txtObservacionesEnvio.ForeColor = System.Drawing.Color.Gray;
            txtObservacionesEnvio.Enter += (s, e) => {
                if (txtObservacionesEnvio.Text == "Mensaje adicional para el paciente (opcional)")
                {
                    txtObservacionesEnvio.Text = "";
                    txtObservacionesEnvio.ForeColor = System.Drawing.Color.Black;
                }
            };
            txtObservacionesEnvio.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtObservacionesEnvio.Text))
                {
                    txtObservacionesEnvio.Text = "Mensaje adicional para el paciente (opcional)";
                    txtObservacionesEnvio.ForeColor = System.Drawing.Color.Gray;
                }
            };

            groupAcciones.Controls.AddRange(new Control[] {
                lblAnalisisSeleccionado, btnGenerarPdf, btnEnviarEmail, btnEnviarWhatsApp,
                lblObservaciones, txtObservacionesEnvio
            });

            // Agregar todos los grupos al formulario
            Controls.AddRange(new Control[] { groupFiltros, groupAnalisis, groupAcciones });

            // Eventos
            btnBuscar.Click += (s, e) => BuscarAnalisisClick?.Invoke(this, EventArgs.Empty);
            btnLimpiarFiltros.Click += (s, e) => LimpiarFiltrosClick?.Invoke(this, EventArgs.Empty);
            btnGenerarPdf.Click += (s, e) => GenerarPdfClick?.Invoke(this, EventArgs.Empty);
            btnEnviarEmail.Click += (s, e) => EnviarEmailClick?.Invoke(this, EventArgs.Empty);
            btnEnviarWhatsApp.Click += (s, e) => EnviarWhatsAppClick?.Invoke(this, EventArgs.Empty);

            // Evento de selección
            gridAnalisisVerificados.SelectionChanged += (s, e) => AnalisisSeleccionCambiada?.Invoke(this, EventArgs.Empty);

            // Permitir búsqueda con Enter
            txtBuscarPaciente.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BuscarAnalisisClick?.Invoke(this, EventArgs.Empty);
                }
            };
        }

        #region Implementación de IInformesVerificadosView

        // Eventos
        public event EventHandler BuscarAnalisisClick;
        public event EventHandler LimpiarFiltrosClick;
        public event EventHandler GenerarPdfClick;
        public event EventHandler EnviarEmailClick;
        public event EventHandler EnviarWhatsAppClick;
        public event EventHandler AnalisisSeleccionCambiada;

        // Propiedades de filtros
        public DateTime FechaDesde => dtpFechaDesde?.Value.Date ?? DateTime.Today.AddMonths(-1);
        public DateTime FechaHasta => dtpFechaHasta?.Value.Date ?? DateTime.Today;
        public int? MedicoSeleccionadoId => cboMedicoFirma?.SelectedValue as int?;
        public string TextoBusquedaPaciente => 
            txtBuscarPaciente?.Text?.Trim() == "DNI o Apellido" ? "" : txtBuscarPaciente?.Text?.Trim();
            
        public string ObservacionesEnvio => 
            txtObservacionesEnvio?.Text?.Trim() == "Mensaje adicional para el paciente (opcional)" ? "" : txtObservacionesEnvio?.Text?.Trim();

        // Métodos de carga de datos
        public void CargarMedicos(IEnumerable<object> medicos)
        {
            if (cboMedicoFirma == null) return;

            cboMedicoFirma.DataSource = null;
            var listaMedicos = new List<object> { new { Dni = (int?)null, NombreCompleto = "Todos los médicos" } };
            listaMedicos.AddRange(medicos);
            
            cboMedicoFirma.DataSource = listaMedicos;
            cboMedicoFirma.DisplayMember = "NombreCompleto";
            cboMedicoFirma.ValueMember = "Dni";
            cboMedicoFirma.SelectedIndex = 0;
        }

        public void CargarAnalisisVerificados(IEnumerable<object> analisis)
        {
            if (gridAnalisisVerificados == null) return;

            var lista = analisis.ToList();
            gridAnalisisVerificados.DataSource = lista;

            if (lblConteoAnalisis != null)
            {
                lblConteoAnalisis.Text = $"{lista.Count} análisis verificados encontrados";
                lblConteoAnalisis.ForeColor = lista.Count > 0 ? System.Drawing.Color.Blue : System.Drawing.Color.Gray;
            }

            // Limpiar selección
            HabilitarAcciones(false);
        }

        public object AnalisisSeleccionado
        {
            get
            {
                if (gridAnalisisVerificados?.SelectedRows.Count == 0)
                    return null;

                return gridAnalisisVerificados.SelectedRows[0].DataBoundItem;
            }
        }

        public void HabilitarAcciones(bool habilitar)
        {
            if (btnGenerarPdf != null) btnGenerarPdf.Enabled = habilitar;
            if (btnEnviarEmail != null) btnEnviarEmail.Enabled = habilitar;
            if (btnEnviarWhatsApp != null) btnEnviarWhatsApp.Enabled = habilitar;

            if (lblAnalisisSeleccionado != null)
            {
                if (habilitar && AnalisisSeleccionado != null)
                {
                    var analisis = AnalisisSeleccionado;
                    var idAnalisis = analisis.GetType().GetProperty("IdAnalisis")?.GetValue(analisis);
                    var paciente = analisis.GetType().GetProperty("PacienteNombre")?.GetValue(analisis);
                    
                    lblAnalisisSeleccionado.Text = $"? Análisis {idAnalisis} seleccionado - Paciente: {paciente}";
                    lblAnalisisSeleccionado.ForeColor = System.Drawing.Color.DarkGreen;
                }
                else
                {
                    lblAnalisisSeleccionado.Text = "Seleccione un análisis para habilitar las acciones";
                    lblAnalisisSeleccionado.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }

        public void LimpiarFiltros()
        {
            if (dtpFechaDesde != null) dtpFechaDesde.Value = DateTime.Today.AddMonths(-1);
            if (dtpFechaHasta != null) dtpFechaHasta.Value = DateTime.Today;
            if (cboMedicoFirma != null) cboMedicoFirma.SelectedIndex = 0;
            if (txtBuscarPaciente != null) txtBuscarPaciente.Clear();
            if (txtObservacionesEnvio != null) txtObservacionesEnvio.Clear();
        }

        public void MostrarMensaje(string texto, bool esError = false)
        {
            var icon = esError ? MessageBoxIcon.Error : MessageBoxIcon.Information;
            var titulo = esError ? "Error - Informes Verificados" : "Información - Informes Verificados";
            MessageBox.Show(this, texto, titulo, MessageBoxButtons.OK, icon);
        }

        #endregion
    }
}