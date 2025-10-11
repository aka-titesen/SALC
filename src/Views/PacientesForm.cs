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
    /// Formulario para gestión de pacientes según ERS v2.7
    /// Implementa RF-03: ABM de Pacientes
    /// </summary>
    public partial class PacientesForm : Form, IVistaPacientes
    {
        #region Eventos de la Interfaz IVistaPacientes (CORREGIDO AL ESPAÑOL)
        public event EventHandler CreacionSolicitada;
        public event EventHandler EdicionSolicitada;
        public event EventHandler EliminacionSolicitada;
        public event EventHandler BusquedaSolicitada;
        public event EventHandler CierreSolicitado;
        #endregion

        #region Propiedades de la Interfaz IVistaPacientes (CORREGIDO AL ESPAÑOL)
        public string TextoBusqueda => txtBuscar.Text == "Buscar por DNI, nombre o apellido..." ? "" : txtBuscar.Text;
        #endregion

        #region Eventos personalizados para funcionalidad extendida
        public event EventHandler CargarPacientes;
        public event EventHandler<int> EliminarPaciente;
        public event EventHandler<Paciente> GuardarPaciente;
        public event EventHandler<int> EditarPaciente;
        public event EventHandler<string> BuscarPaciente;
        public event EventHandler<int> VerHistorialPaciente;
        #endregion

        #region Propiedades de la Interfaz
    private readonly PacienteService _serviciosPacientes;
        public List<Paciente> Pacientes { get; set; } = new List<Paciente>();
        public Paciente PacienteSeleccionado { get; set; }
        public bool EstaEditando { get; set; }
        public List<ObraSocial> ObrasSociales { get; set; } = new List<ObraSocial>();
        #endregion

        #region Controles de la interfaz
        private Panel panelSuperior;
        private Panel panelContenido;
        private Panel panelFormulario;
        private Panel panelLista;
        private DataGridView dgvPacientes;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Button btnNuevo;
        private Button btnEditar;
        private Button btnEliminar;
        private Button btnHistorial;

        // Formulario de paciente
        private TextBox txtDni;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private ComboBox cmbSexo;
        private DateTimePicker dtpFechaNacimiento;
        private TextBox txtTelefono;
        private TextBox txtEmail;
        private ComboBox cmbObraSocial;
        private Button btnGuardar;
        private Button btnCancelar;
        #endregion

        public PacientesForm()
        {
            _serviciosPacientes = new PacienteService();
            InitializeComponent();
            InicializarComponentesPersonalizados();
            CargarObrasSociales();
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Gestión de Pacientes";
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
                Text = "GESTIÓN DE PACIENTES",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(300, 40),
                Location = new Point(20, 20)
            };

            Panel panelBusqueda = new Panel
            {
                Size = new Size(400, 40),
                Location = new Point(this.Width - 440, 20)
            };

            txtBuscar = new TextBox
            {
                Text = "Buscar por DNI, nombre o apellido...",
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill
            };

            btnBuscar = new Button
            {
                Text = "Buscar",
                Width = 80,
                Dock = DockStyle.Right
            };

            panelBusqueda.Controls.Add(txtBuscar);
            panelBusqueda.Controls.Add(btnBuscar);

            panelSuperior.Controls.Add(lblTitulo);
            panelSuperior.Controls.Add(panelBusqueda);

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
            btnHistorial = CrearBotonAccion("Historial", SALCColors.Info, 270);

            btnNuevo.Click += BtnNuevo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnHistorial.Click += BtnHistorial_Click;

            panelBotones.Controls.AddRange(new Control[] { btnNuevo, btnEditar, btnEliminar, btnHistorial });

            // DataGridView
            dgvPacientes = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            ConfigurarColumnas();

            // Encabezados de DataGridView
            dgvPacientes.EnableHeadersVisualStyles = false;
            dgvPacientes.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvPacientes.RowsDefaultCellStyle.SelectionBackColor = SALCColors.PrimaryHover;
            dgvPacientes.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgvPacientes.SelectionChanged += DgvPacientes_SelectionChanged;

            panelLista.Controls.AddRange(new Control[] { panelBotones, dgvPacientes });
        }

        private void ConfigurarColumnas()
        {
            dgvPacientes.Columns.Clear();
            dgvPacientes.Columns.AddRange(new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "Dni", HeaderText = "DNI", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "NombreCompleto", HeaderText = "Nombre Completo", Width = 200 },
                new DataGridViewTextBoxColumn { Name = "FechaNacimiento", HeaderText = "Fecha Nac.", Width = 100 },
                new DataGridViewTextBoxColumn { Name = "Sexo", HeaderText = "Sexo", Width = 60 },
                    new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "ObraSocial", HeaderText = "Obra Social", Width = 150 }
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
                Text = "DATOS DEL PACIENTE",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.Primary,
                Size = new Size(400, 25),
                Location = new Point(20, 20)
            };

            int posicionY = 60;
            int espaciado = 35;

            // DNI
            CrearCampoFormulario("DNI:", ref txtDni, posicionY);
            txtDni.KeyPress += ValidarSoloNumeros;
            posicionY += espaciado;

            // Nombre
            CrearCampoFormulario("Nombre:", ref txtNombre, posicionY);
            posicionY += espaciado;

            // Apellido
            CrearCampoFormulario("Apellido:", ref txtApellido, posicionY);
            posicionY += espaciado;

            // Sexo
            Label lblSexo = CrearEtiqueta("Sexo:", posicionY);
            cmbSexo = new ComboBox
            {
                Size = new Size(200, 25),
                Location = new Point(200, posicionY),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbSexo.Items.AddRange(new string[] { "M", "F", "X" });
            posicionY += espaciado;

            // Fecha de nacimiento
            Label lblFechaNac = CrearEtiqueta("Fecha de Nacimiento:", posicionY);
            // Teléfono
            {
                Size = new Size(200, 25),
                Location = new Point(200, posicionY),
                Font = new Font("Segoe UI", 9),
                Format = DateTimePickerFormat.Short
            };
            posicionY += espaciado;

            // Teléfono
            CrearCampoFormulario("Teléfono:", ref txtTelefono, posicionY);
            posicionY += espaciado;

            // Email
            CrearCampoFormulario("Email:", ref txtEmail, posicionY);
            posicionY += espaciado;

            // Obra Social
            Label lblObraSocial = CrearEtiqueta("Obra Social:", posicionY);
            cmbObraSocial = new ComboBox
            {
                Size = new Size(200, 25),
                Text = "Guardar",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            posicionY += espaciado + 20;

            // Botones
            btnGuardar = new Button
            {
                Text = "Guardar",
                Size = new Size(90, 35),
                Location = new Point(200, posicionY),
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
                Location = new Point(300, posicionY),
                BackColor = SALCColors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;

            panelFormulario.Controls.AddRange(new Control[] {
                lblTituloFormulario, lblSexo, cmbSexo,
                lblFechaNac, dtpFechaNacimiento, lblObraSocial, cmbObraSocial,
                btnGuardar, btnCancelar
            });

            LimpiarFormulario();
        }

        private void CrearCampoFormulario(string textoEtiqueta, ref TextBox cajaTexto, int posicionY)
        {
            Label etiqueta = CrearEtiqueta(textoEtiqueta, posicionY);
            cajaTexto = new TextBox
            {
                Size = new Size(200, 25),
                Location = new Point(200, posicionY),
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            panelFormulario.Controls.AddRange(new Control[] { etiqueta, cajaTexto });
        }

        private Label CrearEtiqueta(string texto, int posicionY)
        {
            return new Label
            {
                Text = texto,
                Size = new Size(180, 20),
                Location = new Point(20, posicionY + 3),
                Font = new Font("Segoe UI", 9),
                ForeColor = SALCColors.TextPrimary
            };
        }

        private void CargarObrasSociales()
        {
            try
            {
                var obrasSociales = _serviciosPacientes.ObtenerObrasSociales();
                cmbObraSocial.Items.Clear();
                cmbObraSocial.Items.Add(new ElementoComboBox("Seleccionar obra social...", 0));

                foreach (var obraSocial in obrasSociales)
                {
                    cmbObraSocial.Items.Add(new ElementoComboBox($"{obraSocial.Nombre} - {obraSocial.Cuit}", obraSocial.IdObraSocial));
                }

                cmbObraSocial.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar obras sociales: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Manejadores de Eventos

        private void TxtBuscar_GotFocus(object sender, EventArgs e)
        {
            if (txtBuscar.Text == "Buscar por DNI, nombre o apellido...")
            {
                txtBuscar.Text = "";
                txtBuscar.ForeColor = Color.Black;
            }
        }

        private void TxtBuscar_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Buscar por DNI, nombre o apellido...";
                txtBuscar.ForeColor = Color.Gray;
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            BusquedaSolicitada?.Invoke(this, EventArgs.Empty);
            string textoBusqueda = TextoBusqueda;
            BuscarPaciente?.Invoke(this, textoBusqueda);
        }

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            CreacionSolicitada?.Invoke(this, EventArgs.Empty);
            EstaEditando = false;
            LimpiarFormulario();
            HabilitarFormulario(true);
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (PacienteSeleccionado != null)
            {
                EdicionSolicitada?.Invoke(this, EventArgs.Empty);
                EstaEditando = true;
                CargarDatosPaciente(PacienteSeleccionado);
                HabilitarFormulario(true);
                EditarPaciente?.Invoke(this, PacienteSeleccionado.Dni);
            }
            else
            {
                MostrarMensaje("Información", "Seleccione un paciente para editar.");
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (PacienteSeleccionado != null)
            {
                EliminacionSolicitada?.Invoke(this, EventArgs.Empty);
                var resultado = MessageBox.Show(
                    $"¿Está seguro que desea eliminar al paciente {PacienteSeleccionado.Nombre} {PacienteSeleccionado.Apellido}?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    EliminarPaciente?.Invoke(this, PacienteSeleccionado.Dni);
                }
            }
            else
            {
                MostrarMensaje("Información", "Seleccione un paciente para eliminar.");
            }
        }

        private void BtnHistorial_Click(object sender, EventArgs e)
        {
            if (PacienteSeleccionado != null)
            {
                VerHistorialPaciente?.Invoke(this, PacienteSeleccionado.Dni);
            }
            else
            {
                MostrarMensaje("Seleccione un paciente para ver su historial.");
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarFormulario())
            {
                try
                {
                    var paciente = new Paciente
                    {
                        Dni = int.Parse(txtDni.Text),
                        Nombre = txtNombre.Text.Trim(),
                        Apellido = txtApellido.Text.Trim(),
                        Sexo = cmbSexo.SelectedItem?.ToString(),
                        FechaNacimiento = dtpFechaNacimiento.Value,
                        Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                        IdObraSocial = cmbObraSocial.SelectedIndex > 0 ?
                                      ((ElementoComboBox)cmbObraSocial.SelectedItem).Valor :
                                      (int?)null
                    };

                    if (EstaEditando)
                    {
                        _serviciosPacientes.ActualizarPaciente(paciente);
                        MostrarMensaje("Paciente actualizado exitosamente.");
                    }
                    else
                    {
                        _serviciosPacientes.CrearPaciente(paciente);
                        MostrarMensaje("Paciente creado exitosamente.");
                    }

                    LimpiarFormulario();
                    HabilitarFormulario(false);
                    ActualizarListaPacientes();
                }
                catch (Exception ex)
                {
                    MostrarError($"Error al guardar paciente: {ex.Message}");
                }
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            HabilitarFormulario(false);
        }

        private void DgvPacientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPacientes.SelectedRows.Count > 0)
            {
                var fila = dgvPacientes.SelectedRows[0];
                int dni = (int)fila.Cells["Dni"].Value;
                PacienteSeleccionado = Pacientes.Find(p => p.Dni == dni);
            }
        }

        private void ValidarSoloNumeros(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

    #region Implementación de la Interfaz IVistaPacientes (CORREGIDO AL ESPAÑOL)

        public void MostrarMensaje(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void LimpiarFormulario()
                MostrarError("El formato del email no es válido.");
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            cmbSexo.SelectedIndex = -1;
            dtpFechaNacimiento.Value = DateTime.Now;
            txtTelefono.Clear();
            txtEmail.Clear();
            cmbObraSocial.SelectedIndex = 0;
            HabilitarFormulario(false);
        }

        public void HabilitarFormulario(bool habilitado)
        {
            txtDni.Enabled = habilitado && !EstaEditando; // El DNI no se puede editar
            txtNombre.Enabled = habilitado;
            txtApellido.Enabled = habilitado;
            cmbSexo.Enabled = habilitado;
            dtpFechaNacimiento.Enabled = habilitado;
            txtTelefono.Enabled = habilitado;
            txtEmail.Enabled = habilitado;
            cmbObraSocial.Enabled = habilitado;
            btnGuardar.Enabled = habilitado;
            btnCancelar.Enabled = habilitado;

            if (!habilitado && EstaEditando)
            {
                txtDni.BackColor = SystemColors.Control;
            }
            else
            {
                txtDni.BackColor = SystemColors.Window;
            }
        }

        public void CargarDatosPaciente(Paciente paciente)
        {
            txtDni.Text = paciente.Dni.ToString();
            txtNombre.Text = paciente.Nombre;
            txtApellido.Text = paciente.Apellido;
            cmbSexo.SelectedItem = paciente.Sexo;
            dtpFechaNacimiento.Value = paciente.FechaNacimiento;
            txtTelefono.Text = paciente.Telefono ?? "";
            txtEmail.Text = paciente.Email ?? "";

            // Seleccionar obra social
            if (paciente.IdObraSocial.HasValue)
            {
                for (int i = 0; i < cmbObraSocial.Items.Count; i++)
                {
                    if (cmbObraSocial.Items[i] is ElementoComboBox elemento &&
                        elemento.Valor == paciente.IdObraSocial.Value)
                    {
                        cmbObraSocial.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        public void ActualizarListaPacientes()
        {
            try
            {
                Pacientes = _serviciosPacientes.ObtenerPacientes();
                dgvPacientes.Rows.Clear();

                foreach (var paciente in Pacientes)
                {
                    dgvPacientes.Rows.Add(
                        paciente.Dni,
                        $"{paciente.Nombre} {paciente.Apellido}",
                        paciente.FechaNacimiento.ToShortDateString(),
                        paciente.Sexo,
                        paciente.Telefono ?? "No disponible",
                        paciente.NombreObraSocial ?? "Sin obra social"
                    );
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al actualizar lista de pacientes: {ex.Message}");
            }
        }

        #endregion

    #region Métodos Privados

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtDni.Text) || !int.TryParse(txtDni.Text, out _))
            {
                MostrarError("El DNI es requerido y debe ser numérico.");
                txtDni.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarError("El nombre es requerido.");
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MostrarError("El apellido es requerido.");
                txtApellido.Focus();
                return false;
            }

            if (cmbSexo.SelectedIndex == -1)
            {
                MostrarError("Debe seleccionar el sexo.");
                cmbSexo.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !EsEmailValido(txtEmail.Text))
            {
                    MostrarError("El formato del email no es válido.");
                txtEmail.Focus();
                return false;
            }

            return true;
        }

        private bool EsEmailValido(string email)
        {
            try
            {
                var direccion = new System.Net.Mail.MailAddress(email);
                return direccion.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void HabilitarFormulario(bool habilitado)
        {
            txtDni.Enabled = habilitado && !EstaEditando; // El DNI no se puede editar
            txtNombre.Enabled = habilitado;
            txtApellido.Enabled = habilitado;
            cmbSexo.Enabled = habilitado;
            dtpFechaNacimiento.Enabled = habilitado;
            txtTelefono.Enabled = habilitado;
            txtEmail.Enabled = habilitado;
            cmbObraSocial.Enabled = habilitado;
            btnGuardar.Enabled = habilitado;
            btnCancelar.Enabled = habilitado;

            if (!habilitado && EstaEditando)
            {
                txtDni.BackColor = SystemColors.Control;
            }
            else
            {
                txtDni.BackColor = SystemColors.Window;
            }
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ActualizarListaPacientes();
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
