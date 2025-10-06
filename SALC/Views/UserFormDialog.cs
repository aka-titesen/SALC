// Views/UserFormDialog.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para agregar/editar usuarios internos del sistema SALC
    /// Implementa RF-10: Gestionar usuarios según ERS-SALC_IEEE830
    /// </summary>
    public partial class UserFormDialog : Form
    {
        #region Controles UI

        private TabControl mainTabControl;
        
        // Tab Datos Básicos
        private TabPage datosBasicosTab;
        private TextBox txtDni;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtEmail;
        private TextBox txtTelefono;
        private ComboBox cmbRol;
        private ComboBox cmbEstado;
        private TextBox txtPassword;
        private Label lblPassword;

        // Tab Datos Específicos
        private TabPage datosEspecificosTab;
        private Panel panelDoctor;
        private Panel panelAsistente;
        private TextBox txtMatricula;
        private TextBox txtEspecialidad;
        private TextBox txtLegajo;
        private ComboBox cmbSupervisor;
        private DateTimePicker dtpFechaIngreso;

        // Botones
        private Button btnGuardar;
        private Button btnCancelar;

        #endregion

        #region Propiedades

        public Usuario Usuario { get; set; }
        public bool EsModoEdicion { get; set; }
        public Dictionary<int, string> RolesDisponibles { get; set; }
        public List<Usuario> DoctoresParaSupervisor { get; set; }

        #endregion

        public UserFormDialog()
        {
            InitializeComponent();
            InitializeCustomComponents();
            ConfigurarEventos();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestión de Usuario - SALC";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(248, 249, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeCustomComponents()
        {
            // TabControl principal
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                Padding = new Point(10, 10)
            };

            CreateDatosBasicosTab();
            CreateDatosEspecificosTab();
            CreateBotones();

            this.Controls.Add(mainTabControl);
        }

        private void CreateDatosBasicosTab()
        {
            datosBasicosTab = new TabPage("Datos Básicos")
            {
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(10)
            };

            // Configurar columnas
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            int row = 0;

            // DNI
            layout.Controls.Add(new Label { Text = "DNI:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, row);
            txtDni = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(txtDni, 1, row++);

            // Nombre
            layout.Controls.Add(new Label { Text = "Nombre:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, row);
            txtNombre = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(txtNombre, 1, row++);

            // Apellido
            layout.Controls.Add(new Label { Text = "Apellido:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, row);
            txtApellido = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(txtApellido, 1, row++);

            // Email
            layout.Controls.Add(new Label { Text = "Email:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10) }, 0, row);
            txtEmail = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(txtEmail, 1, row++);

            // Teléfono
            layout.Controls.Add(new Label { Text = "Teléfono:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10) }, 0, row);
            txtTelefono = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };
            layout.Controls.Add(txtTelefono, 1, row++);

            // Rol
            layout.Controls.Add(new Label { Text = "Rol:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, row);
            cmbRol = new ComboBox { 
                Anchor = AnchorStyles.Left | AnchorStyles.Right, 
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            layout.Controls.Add(cmbRol, 1, row++);

            // Estado
            layout.Controls.Add(new Label { Text = "Estado:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, row);
            cmbEstado = new ComboBox { 
                Anchor = AnchorStyles.Left | AnchorStyles.Right, 
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEstado.Items.Add("Activo");
            cmbEstado.Items.Add("Inactivo");
            cmbEstado.SelectedIndex = 0;
            layout.Controls.Add(cmbEstado, 1, row++);

            // Password
            lblPassword = new Label { Text = "Contraseña:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            layout.Controls.Add(lblPassword, 0, row);
            txtPassword = new TextBox { 
                Anchor = AnchorStyles.Left | AnchorStyles.Right, 
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };
            layout.Controls.Add(txtPassword, 1, row++);

            datosBasicosTab.Controls.Add(layout);
            mainTabControl.TabPages.Add(datosBasicosTab);
        }

        private void CreateDatosEspecificosTab()
        {
            datosEspecificosTab = new TabPage("Datos del Rol")
            {
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Panel para Doctor
            panelDoctor = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };

            var layoutDoctor = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(10)
            };

            layoutDoctor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            layoutDoctor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Matrícula
            layoutDoctor.Controls.Add(new Label { Text = "Nro. Matrícula:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, 0);
            txtMatricula = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };
            layoutDoctor.Controls.Add(txtMatricula, 1, 0);

            // Especialidad
            layoutDoctor.Controls.Add(new Label { Text = "Especialidad:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, 1);
            txtEspecialidad = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };
            layoutDoctor.Controls.Add(txtEspecialidad, 1, 1);

            panelDoctor.Controls.Add(layoutDoctor);

            // Panel para Asistente
            panelAsistente = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false
            };

            var layoutAsistente = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(10)
            };

            layoutAsistente.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            layoutAsistente.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Legajo
            layoutAsistente.Controls.Add(new Label { Text = "Nro. Legajo:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, 0);
            txtLegajo = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right, Font = new Font("Segoe UI", 10) };
            layoutAsistente.Controls.Add(txtLegajo, 1, 0);

            // Supervisor
            layoutAsistente.Controls.Add(new Label { Text = "Supervisor:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10, FontStyle.Bold) }, 0, 1);
            cmbSupervisor = new ComboBox { 
                Anchor = AnchorStyles.Left | AnchorStyles.Right, 
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            layoutAsistente.Controls.Add(cmbSupervisor, 1, 1);

            // Fecha de ingreso
            layoutAsistente.Controls.Add(new Label { Text = "Fecha Ingreso:", Anchor = AnchorStyles.Right, Font = new Font("Segoe UI", 10) }, 0, 2);
            dtpFechaIngreso = new DateTimePicker { 
                Anchor = AnchorStyles.Left | AnchorStyles.Right, 
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };
            layoutAsistente.Controls.Add(dtpFechaIngreso, 1, 2);

            panelAsistente.Controls.Add(layoutAsistente);

            datosEspecificosTab.Controls.Add(panelDoctor);
            datosEspecificosTab.Controls.Add(panelAsistente);
            mainTabControl.TabPages.Add(datosEspecificosTab);
        }

        private void CreateBotones()
        {
            var panelBotones = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(240, 242, 247),
                Padding = new Padding(20, 10, 20, 10)
            };

            btnCancelar = new Button
            {
                Text = "&Cancelar",
                Size = new Size(100, 35),
                Location = new Point(panelBotones.Width - 220, 12),
                Anchor = AnchorStyles.Right,
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            btnGuardar = new Button
            {
                Text = "&Guardar",
                Size = new Size(100, 35),
                Location = new Point(panelBotones.Width - 110, 12),
                Anchor = AnchorStyles.Right,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnGuardar.Click += BtnGuardar_Click;

            panelBotones.Controls.Add(btnCancelar);
            panelBotones.Controls.Add(btnGuardar);

            this.Controls.Add(panelBotones);
        }

        private void ConfigurarEventos()
        {
            cmbRol.SelectedIndexChanged += CmbRol_SelectedIndexChanged;
        }

        #region Métodos públicos

        /// <summary>
        /// Configura el formulario para crear un nuevo usuario
        /// </summary>
        public void ConfigurarParaCreacion(Dictionary<int, string> roles, List<Usuario> supervisores)
        {
            EsModoEdicion = false;
            this.Text = "Agregar Usuario - SALC";
            RolesDisponibles = roles;
            DoctoresParaSupervisor = supervisores;

            CargarRoles();
            CargarSupervisores();
            
            txtDni.ReadOnly = false;
            lblPassword.Text = "Contraseña:";
        }

        /// <summary>
        /// Configura el formulario para editar un usuario existente
        /// </summary>
        public void ConfigurarParaEdicion(Usuario usuario, Dictionary<int, string> roles, List<Usuario> supervisores)
        {
            EsModoEdicion = true;
            this.Text = "Editar Usuario - SALC";
            Usuario = usuario;
            RolesDisponibles = roles;
            DoctoresParaSupervisor = supervisores;

            CargarRoles();
            CargarSupervisores();
            CargarDatosUsuario();
            
            txtDni.ReadOnly = true;
            lblPassword.Text = "Nueva Contraseña:";
        }

        #endregion

        #region Métodos privados

        private void CargarRoles()
        {
            cmbRol.Items.Clear();
            if (RolesDisponibles != null)
            {
                foreach (var kvp in RolesDisponibles)
                {
                    cmbRol.Items.Add(new ComboBoxItem { Value = kvp.Key, Text = kvp.Value });
                }
            }
        }

        private void CargarSupervisores()
        {
            cmbSupervisor.Items.Clear();
            if (DoctoresParaSupervisor != null)
            {
                foreach (var doctor in DoctoresParaSupervisor)
                {
                    cmbSupervisor.Items.Add(new ComboBoxItem 
                    { 
                        Value = doctor.Dni, 
                        Text = $"{doctor.NombreCompleto} - {doctor.Especialidad}" 
                    });
                }
            }
        }

        private void CargarDatosUsuario()
        {
            if (Usuario == null) return;

            txtDni.Text = Usuario.Dni.ToString();
            txtNombre.Text = Usuario.Nombre;
            txtApellido.Text = Usuario.Apellido;
            txtEmail.Text = Usuario.Email;
            txtTelefono.Text = Usuario.Telefono;

            // Seleccionar rol
            for (int i = 0; i < cmbRol.Items.Count; i++)
            {
                if (((ComboBoxItem)cmbRol.Items[i]).Value == Usuario.id_rol)
                {
                    cmbRol.SelectedIndex = i;
                    break;
                }
            }

            // Seleccionar estado
            cmbEstado.SelectedIndex = Usuario.EstaActivo ? 0 : 1;

            // Cargar datos específicos del rol
            if (Usuario.EsDoctor)
            {
                txtMatricula.Text = Usuario.NumeroMatricula?.ToString();
                txtEspecialidad.Text = Usuario.Especialidad;
            }
            else if (Usuario.EsAsistente)
            {
                txtLegajo.Text = Usuario.NumeroLegajo?.ToString();
                dtpFechaIngreso.Value = Usuario.FechaIngreso ?? DateTime.Now;

                // Seleccionar supervisor
                if (Usuario.DniSupervisor.HasValue)
                {
                    for (int i = 0; i < cmbSupervisor.Items.Count; i++)
                    {
                        if (((ComboBoxItem)cmbSupervisor.Items[i]).Value == Usuario.DniSupervisor.Value)
                        {
                            cmbSupervisor.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void CmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbRol.SelectedItem == null) return;

            int rolId = ((ComboBoxItem)cmbRol.SelectedItem).Value;

            // Mostrar/ocultar paneles según el rol
            panelDoctor.Visible = (rolId == 2); // Clínico
            panelAsistente.Visible = (rolId == 3); // Asistente

            // Habilitar/deshabilitar tab
            if (rolId == 1) // Administrador
            {
                datosEspecificosTab.Enabled = false;
                datosEspecificosTab.Text = "Datos del Rol (No aplica)";
            }
            else
            {
                datosEspecificosTab.Enabled = true;
                datosEspecificosTab.Text = rolId == 2 ? "Datos del Clínico" : "Datos del Asistente";
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarFormulario()) return;

                Usuario usuario = CrearUsuarioDesdeFormulario();
                Usuario = usuario;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar los datos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarFormulario()
        {
            // Validaciones básicas
            if (!int.TryParse(txtDni.Text, out int dni) || dni <= 0)
            {
                MessageBox.Show("El DNI debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El apellido es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (cmbRol.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un rol.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Focus();
                return false;
            }

            if (!EsModoEdicion && string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("La contraseña es obligatoria para usuarios nuevos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return false;
            }

            // Validar email si se proporciona
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("El formato del email no es válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validaciones específicas por rol
            int rolId = ((ComboBoxItem)cmbRol.SelectedItem).Value;

            if (rolId == 2) // Doctor
            {
                if (!int.TryParse(txtMatricula.Text, out int matricula) || matricula <= 0)
                {
                    MessageBox.Show("El número de matrícula debe ser válido para clínicos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    mainTabControl.SelectedTab = datosEspecificosTab;
                    txtMatricula.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtEspecialidad.Text))
                {
                    MessageBox.Show("La especialidad es obligatoria para clínicos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    mainTabControl.SelectedTab = datosEspecificosTab;
                    txtEspecialidad.Focus();
                    return false;
                }
            }
            else if (rolId == 3) // Asistente
            {
                if (!int.TryParse(txtLegajo.Text, out int legajo) || legajo <= 0)
                {
                    MessageBox.Show("El número de legajo debe ser válido para asistentes.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    mainTabControl.SelectedTab = datosEspecificosTab;
                    txtLegajo.Focus();
                    return false;
                }

                if (cmbSupervisor.SelectedItem == null)
                {
                    MessageBox.Show("Debe seleccionar un supervisor para asistentes.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    mainTabControl.SelectedTab = datosEspecificosTab;
                    cmbSupervisor.Focus();
                    return false;
                }
            }

            return true;
        }

        private Usuario CrearUsuarioDesdeFormulario()
        {
            int rolId = ((ComboBoxItem)cmbRol.SelectedItem).Value;

            var usuario = new Usuario
            {
                Dni = int.Parse(txtDni.Text),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                password = txtPassword.Text, // Se procesará en el servicio
                id_rol = rolId,
                estado_usuario = cmbEstado.SelectedIndex == 0 ? 1 : 2
            };

            // Datos específicos por rol
            if (rolId == 2) // Doctor
            {
                usuario.NumeroMatricula = int.Parse(txtMatricula.Text);
                usuario.Especialidad = txtEspecialidad.Text.Trim();
            }
            else if (rolId == 3) // Asistente
            {
                usuario.NumeroLegajo = int.Parse(txtLegajo.Text);
                usuario.DniSupervisor = ((ComboBoxItem)cmbSupervisor.SelectedItem).Value;
                usuario.FechaIngreso = dtpFechaIngreso.Value;
            }

            return usuario;
        }

        #endregion

        #region Clase auxiliar

        private class ComboBoxItem
        {
            public int Value { get; set; }
            public string Text { get; set; }

            public override string ToString() => Text;
        }

        #endregion
    }
}