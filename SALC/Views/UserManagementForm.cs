using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SALC.Views
{
    public partial class UserManagementForm : Form
    {
        private DataGridView usersDataGridView;
        private TextBox searchTextBox;
        private ComboBox roleFilterComboBox;
        private Button addUserButton;
        private Button editUserButton;
        private Button deleteUserButton;
        private Button activateUserButton;
        private Button refreshButton;
        private Label searchLabel;
        private Label roleFilterLabel;
        private Panel filterPanel;

        private Dictionary<int, string> rolesDictionary = new Dictionary<int, string>();

        public UserManagementForm()
        {
            InitializeComponent();
            LoadRoles();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestión de Usuarios - SALC";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Filter Panel
            filterPanel = new Panel
            {
                Size = new Size(960, 80),
                Location = new Point(20, 20),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            searchLabel = new Label
            {
                Text = "Buscar por nombre o DNI:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(20, 15),
                Size = new Size(180, 25)
            };

            searchTextBox = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(200, 10),
                Font = new Font("Segoe UI", 10)
            };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            roleFilterLabel = new Label
            {
                Text = "Filtrar por rol:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(420, 15),
                Size = new Size(100, 25)
            };

            roleFilterComboBox = new ComboBox
            {
                Size = new Size(150, 30),
                Location = new Point(520, 10),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            roleFilterComboBox.Items.Add("Todos los roles");
            roleFilterComboBox.SelectedIndex = 0;
            roleFilterComboBox.SelectedIndexChanged += RoleFilterComboBox_SelectedIndexChanged;

            refreshButton = new Button
            {
                Text = "🔄 Actualizar",
                Size = new Size(120, 35),
                Location = new Point(700, 8),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            refreshButton.Click += RefreshButton_Click;

            filterPanel.Controls.AddRange(new Control[] {
                searchLabel, searchTextBox, roleFilterLabel, roleFilterComboBox, refreshButton
            });

            // DataGridView
            usersDataGridView = new DataGridView
            {
                Size = new Size(960, 500),
                Location = new Point(20, 110),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            usersDataGridView.Columns.Add("DNI", "DNI");
            usersDataGridView.Columns.Add("Nombre", "Nombre");
            usersDataGridView.Columns.Add("Apellido", "Apellido");
            usersDataGridView.Columns.Add("Email", "Email");
            usersDataGridView.Columns.Add("Telefono", "Teléfono");
            usersDataGridView.Columns.Add("Rol", "Rol");
            usersDataGridView.Columns.Add("Estado", "Estado");

            usersDataGridView.Columns["DNI"].Width = 100;
            usersDataGridView.Columns["Nombre"].Width = 120;
            usersDataGridView.Columns["Apellido"].Width = 120;
            usersDataGridView.Columns["Email"].Width = 150;
            usersDataGridView.Columns["Telefono"].Width = 100;
            usersDataGridView.Columns["Rol"].Width = 100;
            usersDataGridView.Columns["Estado"].Width = 100;

            // Buttons Panel
            Panel buttonsPanel = new Panel
            {
                Size = new Size(960, 50),
                Location = new Point(20, 620),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var colEstadoId = new DataGridViewTextBoxColumn
            {
                Name = "ID_estado",
                HeaderText = "ID_estado",
                Visible = false,
                ValueType = typeof(int)
            };
            usersDataGridView.Columns.Add(colEstadoId);

            addUserButton = new Button
            {
                Text = "➕ Agregar Usuario",
                Size = new Size(150, 35),
                Location = new Point(20, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            addUserButton.Click += AddUserButton_Click;

            editUserButton = new Button
            {
                Text = "✏️ Editar Usuario",
                Size = new Size(150, 35),
                Location = new Point(190, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            editUserButton.Click += EditUserButton_Click;

            deleteUserButton = new Button
            {
                Text = "🗑️ Desactivar Usuario",
                Size = new Size(150, 35),
                Location = new Point(360, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            deleteUserButton.Click += DeleteUserButton_Click;

            activateUserButton = new Button
            {
                Text = "✅ Activar Usuario",
                Size = new Size(150, 35),
                Location = new Point(530, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            activateUserButton.Click += ACtivateUserButton_Click;

            buttonsPanel.Controls.AddRange(new Control[] {
                addUserButton, editUserButton, deleteUserButton, activateUserButton
            });

            this.Controls.AddRange(new Control[] {
                filterPanel, usersDataGridView, buttonsPanel
            });
        }

        private void LoadRoles()
        {
            try
            {
                rolesDictionary = UserData.LoadRoles();
                roleFilterComboBox.Items.Clear();
                roleFilterComboBox.Items.Add("Todos los roles");
                foreach (var kv in rolesDictionary)
                {
                    roleFilterComboBox.Items.Add(kv.Value);
                }
                roleFilterComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUsers(string searchFilter = "", string roleFilter = "")
        {
            try
            {
                var usuarios = UserData.GetUsers(searchFilter, roleFilter);
                usersDataGridView.Rows.Clear();
                foreach (var u in usuarios)
                {
                    // Cambiado ID_estado a estado_usuario
                    usersDataGridView.Rows.Add(u.Dni, u.Nombre, u.Apellido, u.Email ?? "", u.Telefono ?? "", u.Rol, u.Estado, u.estado_usuario);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e) => ApplyFilters();

        private void RoleFilterComboBox_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            string searchFilter = searchTextBox.Text.Trim();
            string roleFilter = roleFilterComboBox.SelectedItem?.ToString() ?? "";
            LoadUsers(searchFilter, roleFilter);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadRoles();
            LoadUsers();
            searchTextBox.Clear();
            roleFilterComboBox.SelectedIndex = 0;
        }

        private void AddUserButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Usar el nuevo servicio de gestión de usuarios
                var userService = new SALC.Services.UserManagementService();
                var roles = userService.GetRoles();
                var supervisores = userService.GetDoctoresParaSupervisor();

                // Crear y configurar el diálogo para creación
                var dialog = new UserFormDialog();
                dialog.ConfigurarParaCreacion(roles, supervisores);

                // Mostrar el diálogo y procesar resultado
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var usuario = dialog.Usuario;
                    userService.CrearUsuario(usuario);

                    MessageBox.Show("Usuario creado exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Actualizar la lista de usuarios
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear usuario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditUserButton_Click(object sender, EventArgs e)
        {
            if (usersDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para editar.", "Selección requerida", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Obtener el DNI del usuario seleccionado
                DataGridViewRow selectedRow = usersDataGridView.SelectedRows[0];
                int dni = Convert.ToInt32(selectedRow.Cells["DNI"].Value);

                // Usar el nuevo servicio para obtener datos completos del usuario
                var userService = new SALC.Services.UserManagementService();
                var usuario = userService.GetUsuario(dni);
                
                if (usuario == null)
                {
                    MessageBox.Show("No se encontró el usuario seleccionado.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtener roles y supervisores disponibles
                var roles = userService.GetRoles();
                var supervisores = userService.GetDoctoresParaSupervisor();

                // Crear y configurar el diálogo para edición
                var dialog = new UserFormDialog();
                dialog.ConfigurarParaEdicion(usuario, roles, supervisores);

                // Mostrar el diálogo y procesar resultado
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var usuarioEditado = dialog.Usuario;
                    bool cambiarPassword = !string.IsNullOrEmpty(usuarioEditado.password);
                    
                    userService.ActualizarUsuario(usuarioEditado, cambiarPassword);

                    MessageBox.Show("Usuario actualizado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Actualizar la lista de usuarios
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar usuario: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUserButton_Click(object sender, EventArgs e)
        {
            if (usersDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para desactivar.", "Selección requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = usersDataGridView.SelectedRows[0];
            int dni = Convert.ToInt32(selectedRow.Cells["DNI"].Value);
            string nombre = selectedRow.Cells["Nombre"].Value.ToString();
            string apellido = selectedRow.Cells["Apellido"].Value.ToString();

            var result = MessageBox.Show(
                $"¿Está seguro que desea desactivar al usuario {nombre} {apellido} (DNI: {dni})?",
                "Confirmar desactivacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool ok = UserData.DeleteUser(dni);
                    if (ok)
                        MessageBox.Show("Usuario desactivado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No se pudo desactivar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desactivar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ACtivateUserButton_Click(object sender, EventArgs e)
        {
            if (usersDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para activar.", "Selección requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = usersDataGridView.SelectedRows[0];
            int dni = Convert.ToInt32(selectedRow.Cells["DNI"].Value);
            string nombre = selectedRow.Cells["Nombre"].Value.ToString();
            string apellido = selectedRow.Cells["Apellido"].Value.ToString();

            var result = MessageBox.Show(
                $"¿Está seguro que desea activar al usuario {nombre} {apellido} (DNI: {dni})?",
                "Confirmar activacion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool ok = UserData.ActivateUser(dni);
                    if (ok)
                        MessageBox.Show("Usuario activado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No se pudo activado el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al activar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
