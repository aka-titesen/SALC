// Views/UserManagementForm.cs
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SALC
{
    public partial class UserManagementForm : Form
    {
        private DataGridView usersDataGridView;
        private TextBox searchTextBox;
        private ComboBox roleFilterComboBox;
        private Button addUserButton;
        private Button editUserButton;
        private Button deleteUserButton;
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

            usersDataGridView.Columns["DNI"].Width = 100;
            usersDataGridView.Columns["Nombre"].Width = 120;
            usersDataGridView.Columns["Apellido"].Width = 120;
            usersDataGridView.Columns["Email"].Width = 150;
            usersDataGridView.Columns["Telefono"].Width = 100;
            usersDataGridView.Columns["Rol"].Width = 100;

            // Buttons Panel
            Panel buttonsPanel = new Panel
            {
                Size = new Size(960, 50),
                Location = new Point(20, 620),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

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
                Text = "🗑️ Eliminar Usuario",
                Size = new Size(150, 35),
                Location = new Point(360, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            deleteUserButton.Click += DeleteUserButton_Click;

            buttonsPanel.Controls.AddRange(new Control[] {
                addUserButton, editUserButton, deleteUserButton
            });

            this.Controls.AddRange(new Control[] {
                filterPanel, usersDataGridView, buttonsPanel
            });
        }

        private void LoadRoles()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Error de configuración: Cadena de conexión no encontrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "SELECT id_rol, rol FROM roles ORDER BY rol";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            rolesDictionary.Clear();
                            roleFilterComboBox.Items.Clear();
                            roleFilterComboBox.Items.Add("Todos los roles");

                            while (reader.Read())
                            {
                                int idRol = reader.GetInt32(0);
                                string rol = reader.GetString(1);
                                rolesDictionary[idRol] = rol;
                                roleFilterComboBox.Items.Add(rol);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUsers(string searchFilter = "", string roleFilter = "")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Error de configuración: Cadena de conexión no encontrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = @"
                SELECT u.dni, u.nombre, u.apellido, u.email, u.telefono, r.rol
                FROM usuario u
                INNER JOIN roles r ON u.id_rol = r.id_rol
                WHERE 1=1";

            if (!string.IsNullOrEmpty(searchFilter))
            {
                query += " AND (u.nombre LIKE @Search OR u.apellido LIKE @Search OR CAST(u.dni AS NVARCHAR) LIKE @Search)";
            }

            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "Todos los roles")
            {
                query += " AND r.rol = @Role";
            }

            query += " ORDER BY u.apellido, u.nombre";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchFilter))
                        {
                            command.Parameters.AddWithValue("@Search", $"%{searchFilter}%");
                        }
                        if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "Todos los roles")
                        {
                            command.Parameters.AddWithValue("@Role", roleFilter);
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            usersDataGridView.Rows.Clear();
                            while (reader.Read())
                            {
                                usersDataGridView.Rows.Add(
                                    reader.GetInt32(0), // DNI
                                    reader.GetString(1), // Nombre
                                    reader.GetString(2), // Apellido
                                    reader.IsDBNull(3) ? "" : reader.GetString(3), // Email
                                    reader.IsDBNull(4) ? "" : reader.GetString(4), // Telefono
                                    reader.GetString(5) // Rol
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void RoleFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

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
            using (UserEditForm editForm = new UserEditForm(null, rolesDictionary))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void EditUserButton_Click(object sender, EventArgs e)
        {
            if (usersDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para editar.", "Selección requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = usersDataGridView.SelectedRows[0];
            Usuario selectedUser = new Usuario
            {
                Dni = Convert.ToInt32(selectedRow.Cells["DNI"].Value),
                Nombre = selectedRow.Cells["Nombre"].Value.ToString(),
                Apellido = selectedRow.Cells["Apellido"].Value.ToString(),
                Email = selectedRow.Cells["Email"].Value.ToString(),
                Telefono = selectedRow.Cells["Telefono"].Value.ToString(),
                Rol = selectedRow.Cells["Rol"].Value.ToString()
            };

            using (UserEditForm editForm = new UserEditForm(selectedUser, rolesDictionary))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void DeleteUserButton_Click(object sender, EventArgs e)
        {
            if (usersDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para eliminar.", "Selección requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = usersDataGridView.SelectedRows[0];
            int dni = Convert.ToInt32(selectedRow.Cells["DNI"].Value);
            string nombre = selectedRow.Cells["Nombre"].Value.ToString();
            string apellido = selectedRow.Cells["Apellido"].Value.ToString();

            var result = MessageBox.Show(
                $"¿Está seguro que desea eliminar al usuario {nombre} {apellido} (DNI: {dni})?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                DeleteUser(dni);
                LoadUsers();
            }
        }

        private void DeleteUser(int dni)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Error de configuración: Cadena de conexión no encontrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string query = "DELETE FROM usuario WHERE dni = @Dni";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Dni", dni);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}