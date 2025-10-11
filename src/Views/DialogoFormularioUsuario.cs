using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SALC.Models;

namespace SALC.Views
{
    /// <summary>
    /// Formulario de diálogo para crear y editar usuarios según ERS v2.7
    /// Soporta estructura normalizada con tablas usuarios, medicos, asistentes
    /// </summary>
    public partial class DialogoFormularioUsuario : Form
    {
        #region Campos Privados
        private TableLayoutPanel layoutPrincipal;
        private GroupBox grupoInformacionBasica;
        private GroupBox grupoInformacionRol;
        private Panel panelBotones;

        // Controles información básica
        private Label lblDni;
        private TextBox txtDni;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblApellido;
        private TextBox txtApellido;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblContrasena;
        private TextBox txtContrasena;
        private Label lblRol;
        private ComboBox cmbRol;
        private Label lblEstado;
        private ComboBox cmbEstado;

        // Controles específicos por rol
        private Label lblMatricula;
        private TextBox txtMatricula;
        private Label lblEspecialidad;
        private TextBox txtEspecialidad;
        private Label lblSupervisor;
        private ComboBox cmbSupervisor;
        private Label lblFechaIngreso;
        private DateTimePicker dtpFechaIngreso;

        // Botones
        private Button btnGuardar;
        private Button btnCancelar;

        // Datos
        private Dictionary<int, string> _roles;
        private List<Medico> _supervisores;
        private bool _esModoEdicion = false;

        public Usuario Usuario { get; private set; }
        #endregion

        #region Constructor
        public DialogoFormularioUsuario()
        {
            InitializeComponent();
        }
        #endregion

        #region Métodos Públicos de Configuración
        /// <summary>
        /// Configura el diálogo para crear un nuevo usuario
        /// </summary>
        public void ConfigurarParaCreacion(Dictionary<int, string> roles, List<Medico> supervisores)
        {
            this.Text = "Agregar Nuevo Usuario - SALC";
            _roles = roles;
            _supervisores = supervisores;
            _esModoEdicion = false;
            
            ConfigurarControles();
            CargarRoles();
            CargarSupervisores();
            
            // Establecer valores por defecto
            cmbEstado.SelectedIndex = 0; // Activo por defecto
            cmbRol.SelectedIndex = 0;
            dtpFechaIngreso.Value = DateTime.Now;
        }

        /// <summary>
        /// Configura el diálogo para editar un usuario existente
        /// </summary>
        public void ConfigurarParaEdicion(Usuario usuario, Dictionary<int, string> roles, List<Medico> supervisores)
        {
            this.Text = "Editar Usuario - SALC";
            _roles = roles;
            _supervisores = supervisores;
            _esModoEdicion = true;
            
            ConfigurarControles();
            CargarRoles();
            CargarSupervisores();
            CargarDatosUsuario(usuario);
            
            // DNI no debe ser editable en modo edición
            txtDni.ReadOnly = true;
            txtDni.BackColor = SystemColors.Control;
        }
        #endregion

        #region Métodos Privados
        private void InitializeComponent()
        {
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            CrearControles();
            OrganizarControles();
            AsignarEventos();
        }

        private void CrearControles()
        {
            // Layout principal
            layoutPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };

            // Grupo información básica
            grupoInformacionBasica = new GroupBox
            {
                Text = "Información Básica",
                Padding = new Padding(10),
                Dock = DockStyle.Fill
            };

            // Grupo información específica del rol
            grupoInformacionRol = new GroupBox
            {
                Text = "Información del Rol",
                Padding = new Padding(10),
                Dock = DockStyle.Fill
            };

            // Panel de botones
            panelBotones = new Panel
            {
                Height = 50,
                Dock = DockStyle.Fill
            };

            // Controles información básica
            lblDni = new Label { Text = "DNI:", AutoSize = true };
            txtDni = new TextBox { Width = 200 };
            
            lblNombre = new Label { Text = "Nombre:", AutoSize = true };
            txtNombre = new TextBox { Width = 200 };
            
            lblApellido = new Label { Text = "Apellido:", AutoSize = true };
            txtApellido = new TextBox { Width = 200 };
            
            lblEmail = new Label { Text = "Email:", AutoSize = true };
            txtEmail = new TextBox { Width = 200 };
            
            lblContrasena = new Label { Text = "Contraseña:", AutoSize = true };
            txtContrasena = new TextBox { Width = 200, UseSystemPasswordChar = true };
            
            lblRol = new Label { Text = "Rol:", AutoSize = true };
            cmbRol = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            
            lblEstado = new Label { Text = "Estado:", AutoSize = true };
            cmbEstado = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEstado.Items.AddRange(new[] { "Activo", "Inactivo" });

            // Controles específicos por rol
            lblMatricula = new Label { Text = "Nº Matrícula:", AutoSize = true };
            txtMatricula = new TextBox { Width = 200 };
            
            lblEspecialidad = new Label { Text = "Especialidad:", AutoSize = true };
            txtEspecialidad = new TextBox { Width = 200 };
            
            lblSupervisor = new Label { Text = "Supervisor:", AutoSize = true };
            cmbSupervisor = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            
            lblFechaIngreso = new Label { Text = "Fecha Ingreso:", AutoSize = true };
            dtpFechaIngreso = new DateTimePicker { Width = 200, Format = DateTimePickerFormat.Short };

            // Botones
            btnGuardar = new Button
            {
                Text = "Guardar",
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
        }

        private void OrganizarControles()
        {
            // Configurar layout principal
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Layout información básica
            var layoutInformacionBasica = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                AutoSize = true
            };
            
            layoutInformacionBasica.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layoutInformacionBasica.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Agregar controles información básica
            layoutInformacionBasica.Controls.Add(lblDni, 0, 0);
            layoutInformacionBasica.Controls.Add(txtDni, 1, 0);
            layoutInformacionBasica.Controls.Add(lblNombre, 0, 1);
            layoutInformacionBasica.Controls.Add(txtNombre, 1, 1);
            layoutInformacionBasica.Controls.Add(lblApellido, 0, 2);
            layoutInformacionBasica.Controls.Add(txtApellido, 1, 2);
            layoutInformacionBasica.Controls.Add(lblEmail, 0, 3);
            layoutInformacionBasica.Controls.Add(txtEmail, 1, 3);
            layoutInformacionBasica.Controls.Add(lblContrasena, 0, 4);
            layoutInformacionBasica.Controls.Add(txtContrasena, 1, 4);
            layoutInformacionBasica.Controls.Add(lblRol, 0, 5);
            layoutInformacionBasica.Controls.Add(cmbRol, 1, 5);
            layoutInformacionBasica.Controls.Add(lblEstado, 0, 6);
            layoutInformacionBasica.Controls.Add(cmbEstado, 1, 6);

            grupoInformacionBasica.Controls.Add(layoutInformacionBasica);

            // Layout información del rol
            var layoutInformacionRol = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                AutoSize = true
            };
            
            layoutInformacionRol.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layoutInformacionRol.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Agregar controles específicos del rol
            layoutInformacionRol.Controls.Add(lblMatricula, 0, 0);
            layoutInformacionRol.Controls.Add(txtMatricula, 1, 0);
            layoutInformacionRol.Controls.Add(lblEspecialidad, 0, 1);
            layoutInformacionRol.Controls.Add(txtEspecialidad, 1, 1);
            layoutInformacionRol.Controls.Add(lblSupervisor, 0, 2);
            layoutInformacionRol.Controls.Add(cmbSupervisor, 1, 2);
            layoutInformacionRol.Controls.Add(lblFechaIngreso, 0, 3);
            layoutInformacionRol.Controls.Add(dtpFechaIngreso, 1, 3);

            grupoInformacionRol.Controls.Add(layoutInformacionRol);

            // Organizar botones
            btnGuardar.Location = new Point(panelBotones.Width - 170, 10);
            btnCancelar.Location = new Point(panelBotones.Width - 80, 10);
            btnGuardar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            
            panelBotones.Controls.Add(btnGuardar);
            panelBotones.Controls.Add(btnCancelar);

            // Agregar al layout principal
            layoutPrincipal.Controls.Add(grupoInformacionBasica, 0, 0);
            layoutPrincipal.Controls.Add(grupoInformacionRol, 0, 1);
            layoutPrincipal.Controls.Add(panelBotones, 0, 2);

            this.Controls.Add(layoutPrincipal);
        }

        private void AsignarEventos()
        {
            cmbRol.SelectedIndexChanged += CmbRol_SelectedIndexChanged;
            btnGuardar.Click += BtnGuardar_Click;
            txtDni.KeyPress += TxtDni_KeyPress;
            txtMatricula.KeyPress += TxtNumerico_KeyPress;
        }

        private void ConfigurarControles()
        {
            // Inicialmente ocultar todos los controles específicos del rol
            EstablecerVisibilidadControlesRol(false, false, false);
        }

        private void CargarRoles()
        {
            cmbRol.Items.Clear();
            foreach (var rol in _roles)
            {
                cmbRol.Items.Add(new ElementoComboBox(rol.Value, rol.Key));
            }
        }

        private void CargarSupervisores()
        {
            cmbSupervisor.Items.Clear();
            cmbSupervisor.Items.Add(new ElementoComboBox("Seleccionar supervisor...", 0));
            
            foreach (var supervisor in _supervisores)
            {
                cmbSupervisor.Items.Add(new ElementoComboBox(supervisor.NombreCompleto, supervisor.Dni));
            }
            
            if (cmbSupervisor.Items.Count > 0)
                cmbSupervisor.SelectedIndex = 0;
        }

        private void CargarDatosUsuario(Usuario usuario)
        {
            txtDni.Text = usuario.Dni.ToString();
            txtNombre.Text = usuario.Nombre;
            txtApellido.Text = usuario.Apellido;
            txtEmail.Text = usuario.Email ?? "";
            txtContrasena.Text = ""; // No mostrar contraseña existente
            
            // Seleccionar rol
            var elementoRol = cmbRol.Items.Cast<ElementoComboBox>()
                .FirstOrDefault(item => item.Valor.Equals(usuario.IdRol));
            if (elementoRol != null)
                cmbRol.SelectedItem = elementoRol;

            // Seleccionar estado
            cmbEstado.SelectedItem = usuario.Estado;

            // Cargar datos específicos del rol
            if (usuario.EsMedico)
            {
                txtMatricula.Text = usuario.NumeroMatricula?.ToString() ?? "";
                txtEspecialidad.Text = usuario.Especialidad ?? "";
            }
            else if (usuario.EsAsistente)
            {
                // Seleccionar supervisor
                if (usuario.DniSupervisor.HasValue)
                {
                    var elementoSupervisor = cmbSupervisor.Items.Cast<ElementoComboBox>()
                        .FirstOrDefault(item => item.Valor.Equals(usuario.DniSupervisor.Value));
                    if (elementoSupervisor != null)
                        cmbSupervisor.SelectedItem = elementoSupervisor;
                }
                
                if (usuario.FechaIngreso.HasValue)
                    dtpFechaIngreso.Value = usuario.FechaIngreso.Value;
            }
        }

        private void EstablecerVisibilidadControlesRol(bool mostrarMedico, bool mostrarAsistente, bool mostrarAdmin)
        {
            // Controles específicos de médico
            lblMatricula.Visible = mostrarMedico;
            txtMatricula.Visible = mostrarMedico;
            lblEspecialidad.Visible = mostrarMedico;
            txtEspecialidad.Visible = mostrarMedico;

            // Controles específicos de asistente
            lblSupervisor.Visible = mostrarAsistente;
            cmbSupervisor.Visible = mostrarAsistente;
            lblFechaIngreso.Visible = mostrarAsistente;
            dtpFechaIngreso.Visible = mostrarAsistente;

            // Ajustar altura del formulario según controles visibles
            int alturaBase = 400;
            if (mostrarMedico) alturaBase += 80;
            if (mostrarAsistente) alturaBase += 120;
            
            this.Height = alturaBase;
        }

        private bool ValidarEntrada()
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(txtDni.Text))
            {
                MessageBox.Show("El DNI es obligatorio.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }

            if (!int.TryParse(txtDni.Text, out int dni) || dni <= 0)
            {
                MessageBox.Show("El DNI debe ser un número válido y positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El apellido es obligatorio.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Por favor ingrese una dirección de email válida.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (cmbRol.SelectedItem == null)
            {
                MessageBox.Show("Por favor seleccione un rol.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Focus();
                return false;
            }

            var rolSeleccionado = (ElementoComboBox)cmbRol.SelectedItem;
            int idRol = (int)rolSeleccionado.Valor;

            // Validación específica por rol
            if (idRol == 2) // Médico
            {
                if (string.IsNullOrWhiteSpace(txtMatricula.Text))
                {
                    MessageBox.Show("El número de matrícula es obligatorio para médicos.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMatricula.Focus();
                    return false;
                }

                if (!int.TryParse(txtMatricula.Text, out int matricula) || matricula <= 0)
                {
                    MessageBox.Show("El número de matrícula debe ser un número válido y positivo.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMatricula.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                {
                    MessageBox.Show("La especialidad es obligatoria para médicos.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEspecialidad.Focus();
                    return false;
                }
            }
            else if (idRol == 3) // Asistente
            {
                if (cmbSupervisor.SelectedItem == null || ((ElementoComboBox)cmbSupervisor.SelectedItem).Valor.Equals(0))
                {
                    MessageBox.Show("Por favor seleccione un supervisor para los asistentes.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbSupervisor.Focus();
                    return false;
                }
            }

            return true;
        }

        private Usuario CrearUsuarioDesdeEntrada()
        {
            var rolSeleccionado = (ElementoComboBox)cmbRol.SelectedItem;
            int idRol = (int)rolSeleccionado.Valor;

            var usuario = new Usuario
            {
                Dni = int.Parse(txtDni.Text),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                IdRol = idRol,
                Estado = cmbEstado.SelectedItem?.ToString() ?? "Activo"
            };

            // Establecer contraseña si se proporciona
            if (!string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                usuario.PasswordHash = txtContrasena.Text; // Será hasheada por el servicio
            }

            // Establecer datos específicos del rol
            if (idRol == 2) // Médico
            {
                usuario.NumeroMatricula = int.Parse(txtMatricula.Text);
                usuario.Especialidad = txtEspecialidad.Text.Trim();
            }
            else if (idRol == 3) // Asistente
            {
                var supervisorSeleccionado = (ElementoComboBox)cmbSupervisor.SelectedItem;
                usuario.DniSupervisor = (int)supervisorSeleccionado.Valor;
                usuario.FechaIngreso = dtpFechaIngreso.Value;
            }

            return usuario;
        }
        #endregion

        #region Manejadores de Eventos
        private void CmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRol.SelectedItem == null) return;

            var rolSeleccionado = (ElementoComboBox)cmbRol.SelectedItem;
            int idRol = (int)rolSeleccionado.Valor;

            switch (idRol)
            {
                case 1: // Administrador
                    EstablecerVisibilidadControlesRol(false, false, true);
                    grupoInformacionRol.Text = "Información de Administrador";
                    break;
                case 2: // Médico
                    EstablecerVisibilidadControlesRol(true, false, false);
                    grupoInformacionRol.Text = "Información de Médico";
                    break;
                case 3: // Asistente
                    EstablecerVisibilidadControlesRol(false, true, false);
                    grupoInformacionRol.Text = "Información de Asistente";
                    break;
                default:
                    EstablecerVisibilidadControlesRol(false, false, false);
                    grupoInformacionRol.Text = "Información del Rol";
                    break;
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarEntrada())
            {
                Usuario = CrearUsuarioDesdeEntrada();
                this.DialogResult = DialogResult.OK;
            }
        }

        private void TxtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TxtNumerico_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region Clases Auxiliares
        private class ElementoComboBox
        {
            public string Texto { get; set; }
            public object Valor { get; set; }

            public ElementoComboBox(string texto, object valor)
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