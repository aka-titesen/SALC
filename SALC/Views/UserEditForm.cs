// Views/UserEditForm.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SALC
{
    public partial class UserEditForm : Form
    {
        private Usuario editingUser;
        private Dictionary<int, string> rolesDictionary;

        private Label titleLabel;
        private Label dniLabel;
        private TextBox dniTextBox;
        private Label nombreLabel;
        private TextBox nombreTextBox;
        private Label apellidoLabel;
        private TextBox apellidoTextBox;
        private Label emailLabel;
        private TextBox emailTextBox;
        private Label telefonoLabel;
        private TextBox telefonoTextBox;
        private Label passwordLabel;
        private TextBox passwordTextBox;
        private Label confirmPasswordLabel;
        private TextBox confirmPasswordTextBox;
        private Label rolLabel;
        private ComboBox rolComboBox;
        private Button saveButton;
        private Button cancelButton;

        public UserEditForm(Usuario user, Dictionary<int, string> roles)
        {
            editingUser = user;
            rolesDictionary = roles;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = editingUser == null ? "Agregar Usuario" : "Editar Usuario";
            this.Size = new Size(450, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Title
            titleLabel = new Label
            {
                Text = editingUser == null ? "Agregar Nuevo Usuario" : "Editar Usuario",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 40),
                Location = new Point(25, 20)
            };

            // DNI
            dniLabel = new Label
            {
                Text = "DNI:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 80),
                Size = new Size(100, 25)
            };

            dniTextBox = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 75),
                Font = new Font("Segoe UI", 10),
                MaxLength = 8
            };

            // Nombre
            nombreLabel = new Label
            {
                Text = "Nombre:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 120),
                Size = new Size(100, 25)
            };

            nombreTextBox = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 115),
                Font = new Font("Segoe UI", 10)
            };

            // Apellido
            apellidoLabel = new Label
            {
                Text = "Apellido:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 160),
                Size = new Size(100, 25)
            };

            apellidoTextBox = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 155),
                Font = new Font("Segoe UI", 10)
            };

            // Email
            emailLabel = new Label
            {
                Text = "Email:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 200),
                Size = new Size(100, 25)
            };

            emailTextBox = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 195),
                Font = new Font("Segoe UI", 10)
            };

            // Telefono
            telefonoLabel = new Label
            {
                Text = "Teléfono:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 235),
                Size = new Size(100, 25)
            };

            telefonoTextBox = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 230),
                Font = new Font("Segoe UI", 10)
            };

            // Password
            passwordLabel = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 280),
                Size = new Size(100, 25)
            };

            passwordTextBox = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 275),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };

            // Confirm Password
            confirmPasswordLabel = new Label
            {
                Text = "Confirmar:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 320),
                Size = new Size(100, 25)
            };

            confirmPasswordTextBox = new TextBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 315),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };

            // Rol
            rolLabel = new Label
            {
                Text = "Rol:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(50, 360),
                Size = new Size(100, 25)
            };

            rolComboBox = new ComboBox
            {
                Size = new Size(250, 30),
                Location = new Point(150, 355),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Buttons
            saveButton = new Button
            {
                Text = "💾 Guardar",
                Size = new Size(120, 40),
                Location = new Point(120, 420),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "❌ Cancelar",
                Size = new Size(120, 40),
                Location = new Point(250, 420),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] {
                titleLabel, dniLabel, dniTextBox, nombreLabel, nombreTextBox,
                apellidoLabel, apellidoTextBox, emailLabel, emailTextBox,
                telefonoLabel, telefonoTextBox, passwordLabel, passwordTextBox,
                confirmPasswordLabel, confirmPasswordTextBox,
                rolLabel, rolComboBox, saveButton, cancelButton
            });

            // Load roles into combo box
            foreach (var role in rolesDictionary)
            {
                rolComboBox.Items.Add(new KeyValuePair<int, string>(role.Key, role.Value));
            }
            rolComboBox.DisplayMember = "Value";
            rolComboBox.ValueMember = "Key";

            // Load user data if editing
            if (editingUser != null)
            {
                LoadUserData();
            }
            else
            {
                // Set default role for new users
                if (rolComboBox.Items.Count > 0)
                {
                    rolComboBox.SelectedIndex = 0;
                }
            }

            // DNI validation
            dniTextBox.KeyPress += (sender, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
        }

        private void LoadUserData()
        {
            dniTextBox.Text = editingUser.Dni.ToString();
            dniTextBox.ReadOnly = true; // DNI shouldn't be editable
            nombreTextBox.Text = editingUser.Nombre;
            apellidoTextBox.Text = editingUser.Apellido;
            emailTextBox.Text = editingUser.Email;
            telefonoTextBox.Text = editingUser.Telefono;

            // Find and select the current role
            for (int i = 0; i < rolComboBox.Items.Count; i++)
            {
                var item = (KeyValuePair<int, string>)rolComboBox.Items[i];
                if (item.Value.ToLower() == editingUser.Rol.ToLower())
                {
                    rolComboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            try
            {
                if (editingUser == null)
                {
                    CreateUser();
                }
                else
                {
                    UpdateUser();
                }

                MessageBox.Show("Usuario guardado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(dniTextBox.Text))
            {
                MessageBox.Show("El DNI es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dniTextBox.Focus();
                return false;
            }

            if (!int.TryParse(dniTextBox.Text, out int dni) || dni <= 0)
            {
                MessageBox.Show("El DNI debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dniTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(nombreTextBox.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nombreTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(apellidoTextBox.Text))
            {
                MessageBox.Show("El apellido es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                apellidoTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                MessageBox.Show("El email es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                emailTextBox.Focus();
                return false;
            }

            if (editingUser == null && string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("La contraseña es obligatoria para nuevos usuarios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                passwordTextBox.Focus();
                return false;
            }

            if (passwordTextBox.Text != confirmPasswordTextBox.Text)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                confirmPasswordTextBox.Focus();
                return false;
            }

            if (rolComboBox.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un rol.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rolComboBox.Focus();
                return false;
            }

            return true;
        }

        private void CreateUser()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Cadena de conexión no encontrada.");
            }

            int dni = int.Parse(dniTextBox.Text);
            var selectedRole = (KeyValuePair<int, string>)rolComboBox.SelectedItem;

            // Cambiado 'contraseña' a 'password' para coincidir con la BD
            string query = @"
                INSERT INTO usuario (dni, nombre, apellido, email, telefono, password, id_rol)
                VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono, @Password, @IdRol)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Dni", dni);
                    command.Parameters.AddWithValue("@Nombre", nombreTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Apellido", apellidoTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Email", emailTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Telefono", telefonoTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Password", passwordTextBox.Text);
                    command.Parameters.AddWithValue("@IdRol", selectedRole.Key);

                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateUser()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Cadena de conexión no encontrada.");
            }

            int dni = int.Parse(dniTextBox.Text);
            var selectedRole = (KeyValuePair<int, string>)rolComboBox.SelectedItem;

            string query = @"
                UPDATE usuario
                SET nombre = @Nombre, apellido = @Apellido, email = @Email, telefono = @Telefono, id_rol = @IdRol
                WHERE dni = @Dni";

            // Update password only if provided
            if (!string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                // Cambiado 'contraseña' a 'password' para coincidir con la BD
                query = @"
                    UPDATE usuario
                    SET nombre = @Nombre, apellido = @Apellido, email = @Email, telefono = @Telefono, password = @Password, id_rol = @IdRol
                    WHERE dni = @Dni";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Dni", dni);
                    command.Parameters.AddWithValue("@Nombre", nombreTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Apellido", apellidoTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Email", emailTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Telefono", telefonoTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@IdRol", selectedRole.Key);

                    if (!string.IsNullOrWhiteSpace(passwordTextBox.Text))
                    {
                        command.Parameters.AddWithValue("@Password", passwordTextBox.Text);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}