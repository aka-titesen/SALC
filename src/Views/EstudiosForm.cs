using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Views.Interfaces;
using SALC.Services;
using SALC.Models;
using SALC.Common;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para gestión de estudios/análisis según ERS v2.7
    /// Implementa RF-05: Crear Análisis, RF-06: Cargar Resultados, RF-07: Validar Análisis
    /// </summary>
    public partial class EstudiosForm : Form, IVistaEstudios
    {
        #region Eventos de la Interfaz IVistaEstudios
        public event EventHandler CreacionSolicitada;
        public event EventHandler EdicionSolicitada;
        public event EventHandler EliminacionSolicitada;
        public event EventHandler BusquedaSolicitada;
        public event EventHandler CierreSolicitado;
        #endregion

        #region Propiedades de la Interfaz IVistaEstudios
        public string TextoBusqueda => txtBuscar.Text;
        #endregion

        #region Eventos personalizados
        public event EventHandler CargarEstudios;
        public event EventHandler<int> EliminarEstudio;
        public event EventHandler<Analisis> GuardarEstudio;
        public event EventHandler<int> EditarEstudio;
        public event EventHandler<string> BuscarEstudio;
        public event EventHandler<int> VerDetallesEstudio;
        public event EventHandler<int> ValidarEstudio;
        #endregion

        #region Propiedades
        private readonly AnalisisService _servicioAnalisis;
        private readonly PacienteService _servicioPacientes;
        public List<Analisis> Estudios { get; set; } = new List<Analisis>();
        public Analisis EstudioSeleccionado { get; set; }
        public bool EstaEditando { get; set; }
        public List<Paciente> Pacientes { get; set; } = new List<Paciente>();
        public List<TipoAnalisis> TiposAnalisis { get; set; } = new List<TipoAnalisis>();
        #endregion

        #region Controles de la interfaz
        private Panel panelSuperior;
        private Panel panelContenido;
        private Panel panelFormulario;
        private Panel panelLista;
        private DataGridView dgvEstudios;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnNuevo;
        private Button btnEditar;
        private Button btnEliminar;
        private Button btnDetalles;
        private Button btnValidar;

        // Formulario de estudio
        private ComboBox cmbPaciente;
        private ComboBox cmbTipoAnalisis;
        private ComboBox cmbEstado;
        private TextBox txtObservaciones;
        private DateTimePicker dtpFechaCreacion;
        private Button btnGuardar;
        private Button btnCancelar;
        #endregion

        public EstudiosForm()
        {
            _servicioAnalisis = new AnalisisService();
            _servicioPacientes = new PacienteService();
            InitializeComponent();
            InicializarComponentesPersonalizados();
            CargarDatosIniciales();
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Gestión de Estudios";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SALCColors.Background;
            this.WindowState = FormWindowState.Maximized;
        }

        private void InicializarComponentesPersonalizados()
        {
            CrearPanelSuperior();
            CrearPanelContenido();
            ConfigurarDataGridView();
            ConfigurarPanelFormulario();
        }

        private void CrearPanelSuperior()
        {
            panelSuperior = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = SALCColors.Primary,
                Padding = new Padding(20)
            };

            Label lblTitulo = new Label
            {
                Text = "GESTIÓN DE ESTUDIOS",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(300, 40),
                Location = new Point(20, 20)
            };

            Panel panelBusqueda = new Panel
            {
                Size = new Size(400, 40),
                Location = new Point(this.Width - 440, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            txtBuscar = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(0, 5),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Buscar por paciente o tipo de análisis..."
            };
            txtBuscar.GotFocus += TxtBuscar_GotFocus;
            txtBuscar.LostFocus += TxtBuscar_LostFocus;

            btnBuscar = new Button
            {
                Text = "Buscar",
                Size = new Size(80, 30),
                Location = new Point(310, 5),
                BackColor = SALCColors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += BtnBuscar_Click;

            panelBusqueda.Controls.AddRange(new Control[] { txtBuscar, btnBuscar });
            panelSuperior.Controls.AddRange(new Control[] { lblTitulo, panelBusqueda });
            this.Controls.Add(panelSuperior);
        }

        private void CrearPanelContenido()
        {
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SALCColors.Background,
                Padding = new Padding(20)
            };

            // Panel izquierdo para la lista
            panelLista = new Panel
            {
                Width = 700,
                Dock = DockStyle.Left,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // Panel derecho para el formulario
            panelFormulario = new Panel
            {
                Width = 450,
                Dock = DockStyle.Right,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            panelContenido.Controls.AddRange(new Control[] { panelLista, panelFormulario });
            this.Controls.Add(panelContenido);
        }

        private void ConfigurarDataGridView()
        {
            // Botones de acción
            Panel panelBotones = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            btnNuevo = CrearBotonAccion("Nuevo", SALCColors.Success, 0);
            btnEditar = CrearBotonAccion("Editar", SALCColors.Primary, 90);
            btnEliminar = CrearBotonAccion("Eliminar", SALCColors.Danger, 180);
            btnDetalles = CrearBotonAccion("Detalles", SALCColors.Info, 270);
            btnValidar = CrearBotonAccion("Validar", SALCColors.Warning, 360);

            btnNuevo.Click += BtnNuevo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnDetalles.Click += BtnDetalles_Click;
            btnValidar.Click += BtnValidar_Click;

            panelBotones.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnEliminar, btnDetalles, btnValidar });

            // DataGridView
            dgvEstudios = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9)
            };

            // Configurar columnas
            ConfigurarColumnas();

            // Estilo del encabezado
            dgvEstudios.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvEstudios.RowsDefaultCellStyle.SelectionBackColor = SALCColors.PrimaryHover;
            dgvEstudios.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvEstudios.SelectionChanged += DgvEstudios_SelectionChanged;

            panelLista.Controls.AddRange(new Control[] { panelBotones, dgvEstudios });
        }

        private void ConfigurarColumnas()
        {
            dgvEstudios.Columns.Clear();
            dgvEstudios.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "Paciente", HeaderText = "Paciente", Width = 180 },
                new DataGridViewTextBoxColumn { Name = "TipoAnalisis", HeaderText = "Tipo de Análisis", Width = 150 },
                new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "FechaCreacion", HeaderText = "Fecha Creación", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "Medico", HeaderText = "Médico", Width = 150 }
            });
        }

        private Button CrearBotonAccion(string texto, Color colorFondo, int x)
        {
            return new Button
            {
                Text = texto,
                Size = new Size(80, 30),
                Location = new Point(x, 10),
                BackColor = colorFondo,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
        }

        private void ConfigurarPanelFormulario()
        {
            Label lblTituloFormulario = new Label
            {
                Text = "DATOS DEL ESTUDIO",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.Primary,
                Size = new Size(400, 25),
                Location = new Point(20, 20)
            };

            int posicionY = 60;
            int espaciado = 40;

            // Paciente
            Label lblPaciente = CrearEtiqueta("Paciente:", posicionY);
            cmbPaciente = new ComboBox
            {
                Size = new Size(250, 25),
                Location = new Point(150, posicionY),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            posicionY += espaciado;

            // Tipo de Análisis
            Label lblTipoAnalisis = CrearEtiqueta("Tipo de Análisis:", posicionY);
            cmbTipoAnalisis = new ComboBox
            {
                Size = new Size(250, 25),
                Location = new Point(150, posicionY),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            posicionY += espaciado;

            // Estado
            Label lblEstado = CrearEtiqueta("Estado:", posicionY);
            cmbEstado = new ComboBox
            {
                Size = new Size(250, 25),
                Location = new Point(150, posicionY),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbEstado.Items.AddRange(new string[] { "Sin verificar", "Verificado" });
            cmbEstado.SelectedIndex = 0;
            posicionY += espaciado;

            // Fecha de Creación
            Label lblFecha = CrearEtiqueta("Fecha:", posicionY);
            dtpFechaCreacion = new DateTimePicker
            {
                Size = new Size(250, 25),
                Location = new Point(150, posicionY),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short
            };
            posicionY += espaciado;

            // Observaciones
            Label lblObservaciones = CrearEtiqueta("Observaciones:", posicionY);
            txtObservaciones = new TextBox
            {
                Size = new Size(250, 60),
                Location = new Point(150, posicionY),
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            posicionY += 80;

            // Botones
            btnGuardar = new Button
            {
                Text = "Guardar",
                Size = new Size(90, 35),
                Location = new Point(150, posicionY),
                BackColor = SALCColors.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(90, 35),
                Location = new Point(250, posicionY),
                BackColor = SALCColors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;

            panelFormulario.Controls.AddRange(new Control[] {
                lblTituloFormulario, lblPaciente, cmbPaciente, lblTipoAnalisis, cmbTipoAnalisis,
                lblEstado, cmbEstado, lblFecha, dtpFechaCreacion,
                lblObservaciones, txtObservaciones, btnGuardar, btnCancelar
            });

            LimpiarFormulario();
        }

        private Label CrearEtiqueta(string texto, int posicionY)
        {
            return new Label
            {
                Text = texto,
                Size = new Size(140, 20),
                Location = new Point(20, posicionY + 3),
                Font = new Font("Segoe UI", 9),
                ForeColor = SALCColors.TextPrimary
            };
        }

        private void CargarDatosIniciales()
        {
            try
            {
                // Cargar pacientes
                Pacientes = _servicioPacientes.ObtenerPacientes();
                cmbPaciente.Items.Clear();
                cmbPaciente.Items.Add(new ElementoComboBox("Seleccionar paciente...", 0));
                foreach (var paciente in Pacientes)
                {
                    cmbPaciente.Items.Add(new ElementoComboBox($"{paciente.Nombre} {paciente.Apellido} - DNI: {paciente.Dni}", paciente.Dni));
                }
                cmbPaciente.SelectedIndex = 0;

                // Cargar tipos de análisis
                // TODO: Implementar servicio de tipos de análisis
                cmbTipoAnalisis.Items.Clear();
                cmbTipoAnalisis.Items.AddRange(new string[] { "Hemograma Completo", "Glucosa en Ayunas", "Perfil Lipídico Completo", "Análisis de Orina Completo" });

                ActualizarListaEstudios();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar datos iniciales: {ex.Message}");
            }
        }

        #region Manejadores de Eventos

        private void TxtBuscar_GotFocus(object sender, EventArgs e)
        {
            if (txtBuscar.Text == "Buscar por paciente o tipo de análisis...")
            {
                txtBuscar.Text = "";
                txtBuscar.ForeColor = Color.Black;
            }
        }

        private void TxtBuscar_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar por paciente o tipo de análisis...";
                txtBuscar.ForeColor = Color.Gray;
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string textoBusqueda = txtBuscar.Text == "Buscar por paciente o tipo de análisis..." ? "" : txtBuscar.Text;
            BuscarEstudio?.Invoke(this, textoBusqueda);
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            EstaEditando = false;
            LimpiarFormulario();
            HabilitarFormulario(true);
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (EstudioSeleccionado != null)
            {
                EstaEditando = true;
                CargarDatosEstudio(EstudioSeleccionado);
                HabilitarFormulario(true);
                EditarEstudio?.Invoke(this, EstudioSeleccionado.IdAnalisis);
            }
            else
            {
                MostrarMensaje("Seleccione un estudio para editar.");
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (EstudioSeleccionado != null)
            {
                var resultado = MessageBox.Show(
                    $"¿Está seguro que desea eliminar el estudio ID {EstudioSeleccionado.IdAnalisis}?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    EliminarEstudio?.Invoke(this, EstudioSeleccionado.IdAnalisis);
                }
            }
            else
            {
                MostrarMensaje("Seleccione un estudio para eliminar.");
            }
        }

        private void BtnDetalles_Click(object sender, EventArgs e)
        {
            if (EstudioSeleccionado != null)
            {
                VerDetallesEstudio?.Invoke(this, EstudioSeleccionado.IdAnalisis);
            }
            else
            {
                MostrarMensaje("Seleccione un estudio para ver sus detalles.");
            }
        }

        private void BtnValidar_Click(object sender, EventArgs e)
        {
            if (EstudioSeleccionado != null)
            {
                if (EstudioSeleccionado.Estado == "Sin verificar")
                {
                    var resultado = MessageBox.Show(
                        $"¿Está seguro que desea validar el estudio ID {EstudioSeleccionado.IdAnalisis}?",
                        "Confirmar Validación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (resultado == DialogResult.Yes)
                    {
                        ValidarEstudio?.Invoke(this, EstudioSeleccionado.IdAnalisis);
                    }
                }
                else
                {
                    MostrarMensaje("Este estudio ya ha sido validado.");
                }
            }
            else
            {
                MostrarMensaje("Seleccione un estudio para validar.");
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarFormulario())
            {
                try
                {
                    var analisis = new Analisis
                    {
                        DniPaciente = ((ElementoComboBox)cmbPaciente.SelectedItem).Valor,
                        // TODO: Implementar selección de tipo de análisis
                        // IdTipoAnalisis = ...,
                        FechaCreacion = dtpFechaCreacion.Value,
                        Observaciones = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? null : txtObservaciones.Text.Trim()
                    };

                    GuardarEstudio?.Invoke(this, analisis);
                    LimpiarFormulario();
                    HabilitarFormulario(false);
                    ActualizarListaEstudios();
                }
                catch (Exception ex)
                {
                    MostrarError($"Error al guardar estudio: {ex.Message}");
                }
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            HabilitarFormulario(false);
        }

        private void DgvEstudios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEstudios.SelectedRows.Count > 0)
            {
                var fila = dgvEstudios.SelectedRows[0];
                int idAnalisis = (int)fila.Cells["Id"].Value;
                EstudioSeleccionado = Estudios.Find(e => e.IdAnalisis == idAnalisis);
            }
        }

        #endregion

        #region Implementación de la Interfaz

        public void MostrarMensaje(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void MostrarMensaje(string mensaje)
        {
            MostrarMensaje("Información", mensaje);
        }

        public void MostrarError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void LimpiarFormulario()
        {
            cmbPaciente.SelectedIndex = 0;
            cmbTipoAnalisis.SelectedIndex = -1;
            cmbEstado.SelectedIndex = 0;
            dtpFechaCreacion.Value = DateTime.Now;
            txtObservaciones.Clear();
            HabilitarFormulario(false);
        }

        public void CargarDatosEstudio(Analisis analisis)
        {
            // Seleccionar paciente
            for (int i = 0; i < cmbPaciente.Items.Count; i++)
            {
                if (cmbPaciente.Items[i] is ElementoComboBox elemento &&
                    elemento.Valor == analisis.DniPaciente)
                {
                    cmbPaciente.SelectedIndex = i;
                    break;
                }
            }

            // TODO: Implementar carga de tipo de análisis
            cmbEstado.SelectedItem = analisis.Estado;
            dtpFechaCreacion.Value = analisis.FechaCreacion;
            txtObservaciones.Text = analisis.Observaciones ?? "";
        }

        public void ActualizarListaEstudios()
        {
            try
            {
                // TODO: Implementar obtención de estudios
                dgvEstudios.Rows.Clear();

                // Datos de ejemplo
                dgvEstudios.Rows.Add(1, "Juan Pérez", "Hemograma Completo", "Sin verificar", DateTime.Now.AddDays(-2).ToShortDateString(), "Dr. González");
                dgvEstudios.Rows.Add(2, "María López", "Glucosa en Ayunas", "Verificado", DateTime.Now.AddDays(-1).ToShortDateString(), "Dr. Martínez");
            }
            catch (Exception ex)
            {
                MostrarError($"Error al actualizar lista de estudios: {ex.Message}");
            }
        }

        #endregion

        #region Métodos Privados

        private bool ValidarFormulario()
        {
            if (cmbPaciente.SelectedIndex <= 0)
            {
                MostrarError("Debe seleccionar un paciente.");
                cmbPaciente.Focus();
                return false;
            }

            if (cmbTipoAnalisis.SelectedIndex < 0)
            {
                MostrarError("Debe seleccionar un tipo de análisis.");
                cmbTipoAnalisis.Focus();
                return false;
            }

            return true;
        }

        private void HabilitarFormulario(bool habilitado)
        {
            cmbPaciente.Enabled = habilitado;
            cmbTipoAnalisis.Enabled = habilitado;
            cmbEstado.Enabled = habilitado;
            dtpFechaCreacion.Enabled = habilitado;
            txtObservaciones.Enabled = habilitado;
            btnGuardar.Enabled = habilitado;
            btnCancelar.Enabled = habilitado;
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ActualizarListaEstudios();
        }

        #region Clase Helper

        private class ElementoComboBox
        {
            public string Texto { get; set; }
            public int Valor { get; set; }

            public ElementoComboBox(string texto, int valor)
            {
                Texto = texto;
                Valor = valor;
            }

            public override string ToString()
            {
                return Texto;
            }
        }

        #endregion
    }
}
