using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Models;
using SALC.Services;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para editar usuarios existentes según ERS v2.7
    /// Implementa RF-02: ABM de Usuarios
    /// </summary>
    public partial class EditarUsuarioForm : Form
    {
    private readonly SALC.Services.UsuariosService _servicioUsuarios;
        private Usuario _usuarioEnEdicion;
        private Dictionary<int, string> _roles;
        private List<Medico> _supervisores;

        // Controles de la interfaz
        private Label lblTitulo;
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
        private Label lblConfirmarContrasena;
        private TextBox txtConfirmarContrasena;
        private Label lblRol;
        private ComboBox cmbRol;
        private Button btnGuardar;
        private Button btnCancelar;

        public EditarUsuarioForm(Usuario usuario)
        {
            _servicioUsuarios = new SALC.Services.UsuariosService();
            _usuarioEnEdicion = usuario;
            InitializeComponent();
            CargarDatos();
        }

        private void InitializeComponent()
        {
            this.Text = _usuarioEnEdicion == null ? "Agregar Usuario - SALC" : "Editar Usuario - SALC";
            this.Size = new Size(450, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Título
            lblTitulo = new Label
            {
                Text = _usuarioEnEdicion == null ? "Agregar Nuevo Usuario" : "Editar Usuario",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 40),
                Location = new Point(25, 20)
            };

            // DNI
            lblDni = new Label
            {
                Text = "DNI:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 80),
                Size = new Size(100, 25)
            };

            txtDni = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 75),
                Font = new Font("Segoe UI", 10),
                MaxLength = 8
            };

            // Nombre
            lblNombre = new Label
            {
                Text = "Nombre:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 120),
                Size = new Size(100, 25)
            };

            txtNombre = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 115),
                Font = new Font("Segoe UI", 10)
            };

            // Apellido
            lblApellido = new Label
            {
                Text = "Apellido:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 160),
                Size = new Size(100, 25)
            };

            txtApellido = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 155),
                Font = new Font("Segoe UI", 10)
            };

            // Email
            lblEmail = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 200),
                Size = new Size(100, 25)
            };

            txtEmail = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 195),
                Font = new Font("Segoe UI", 10)
            };

            // Contraseña
            lblContrasena = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 280),
                Size = new Size(100, 25)
            };

            txtContrasena = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 275),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };

            // Confirmar Contraseña
            lblConfirmarContrasena = new Label
            {
                Text = "Confirmar:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 320),
                Size = new Size(100, 25)
            };

            txtConfirmarContrasena = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 315),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };

            // Rol
            lblRol = new Label
            {
                Text = "Rol:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 360),
                Size = new Size(100, 25)
            };

            cmbRol = new ComboBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 355),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Botones
            btnGuardar = new Button
            {
                Text = "Guardar",
                Size = new Size(120, 40),
                Location = new Point(120, 420),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnGuardar.Click += BtnGuardar_Click;

            btnCancelar = new Button
            {
                 Text = "Cancelar",
                Size = new Size(120, 40),
                Location = new Point(250, 420),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] {
                lblTitulo, lblDni, txtDni, lblNombre, txtNombre,
                lblApellido, txtApellido, lblEmail, txtEmail,
                lblContrasena, txtContrasena, lblConfirmarContrasena, txtConfirmarContrasena,
                lblRol, cmbRol, btnGuardar, btnCancelar
            });

            // Validación de DNI solo números
            txtDni.KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
        }

        private void CargarDatos()
        {
            try
            {
                // Cargar roles
                _roles = _servicioUsuarios.ObtenerRoles();
                cmbRol.Items.Clear();
                foreach (var rol in _roles)
                {
                    cmbRol.Items.Add(new ElementoComboBox(rol.Value, rol.Key));
                }

                // Cargar supervisores
                _supervisores = _servicioUsuarios.ObtenerMedicosParaSupervisor();

                // Cargar datos del usuario si está en modo edición
                if (_usuarioEnEdicion != null)
                {
                    CargarDatosUsuario();
                }
                else
                {
                    // Establecer valores por defecto para nuevo usuario
                    if (cmbRol.Items.Count > 0)
                        cmbRol.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDatosUsuario()
        {
            txtDni.Text = _usuarioEnEdicion.Dni.ToString();
            txtDni.ReadOnly = true; // DNI no se puede editar
            txtDni.BackColor = SystemColors.Control;

            txtNombre.Text = _usuarioEnEdicion.Nombre;
            txtApellido.Text = _usuarioEnEdicion.Apellido;
            txtEmail.Text = _usuarioEnEdicion.Email ?? "";

            // Seleccionar rol actual
            var elementoRol = FindRolInComboBox(_usuarioEnEdicion.IdRol);
            if (elementoRol != null)
                cmbRol.SelectedItem = elementoRol;
        }

        private ElementoComboBox FindRolInComboBox(int idRol)
        {
            foreach (ElementoComboBox item in cmbRol.Items)
            {
                if (item.Valor.Equals(idRol))
                    return item;
            }
            return null;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatos())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                var usuario = CrearUsuarioDesdeFormulario();
                bool cambiarContrasena = !string.IsNullOrWhiteSpace(txtContrasena.Text);

                if (_usuarioEnEdicion == null)
                {
                    _servicioUsuarios.CrearUsuario(usuario);
                    MessageBox.Show("Usuario creado exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    usuario.Dni = _usuarioEnEdicion.Dni; // Preservar DNI original
                    _servicioUsuarios.ActualizarUsuario(usuario, cambiarContrasena);
                    MessageBox.Show("Usuario actualizado exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar usuario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }

        private bool ValidarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtDni.Text))
            {
                MessageBox.Show("El DNI es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }

            if (!int.TryParse(txtDni.Text, out int dni) || dni <= 0)
            {
                MessageBox.Show("El DNI debe ser un número válido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDni.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                    MessageBox.Show("El nombre es obligatorio.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                    MessageBox.Show("El apellido es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApellido.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                    MessageBox.Show("El email es obligatorio.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            if (_usuarioEnEdicion == null && string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                    MessageBox.Show("La contraseña es obligatoria para nuevos usuarios.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContrasena.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtContrasena.Text) &&
                txtContrasena.Text != txtConfirmarContrasena.Text)
            {
                    MessageBox.Show("Las contraseñas no coinciden.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmarContrasena.Focus();
                return false;
            }

            if (cmbRol.SelectedItem == null)
            {
                    MessageBox.Show("Debe seleccionar un rol.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRol.Focus();
                return false;
            }

            return true;
        }

        private Usuario CrearUsuarioDesdeFormulario()
        {
            var rolSeleccionado = (ElementoComboBox)cmbRol.SelectedItem;

            var usuario = new Usuario
            {
                Dni = int.Parse(txtDni.Text),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                IdRol = (int)rolSeleccionado.Valor,
                Estado = "Activo"
            };

            if (!string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                 usuario.PasswordHash = txtContrasena.Text; // Será hasheada por el servicio
            }

            return usuario;
        }

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
    }
}
