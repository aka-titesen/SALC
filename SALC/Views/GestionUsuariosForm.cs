using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SALC.Services;
using SALC.Models;

namespace SALC.Views
{
    /// <summary>
    /// Formulario de gestión de usuarios según ERS v2.7
    /// Implementa RF-02: ABM de Usuarios
    /// </summary>
    public partial class GestionUsuariosForm : Form
    {
        private DataGridView dataGridViewUsuarios;
        private TextBox txtBuscar;
        private ComboBox cmbFiltroRol;
        private Button btnAgregar;
        private Button btnEditar;
        private Button btnDesactivar;
        private Button btnActivar;
        private Button btnActualizar;
        private Label lblBuscar;
        private Label lblFiltroRol;
        private Panel panelFiltros;

        private readonly UserDataService _servicioUsuarios;
        private Dictionary<int, string> diccionarioRoles = new Dictionary<int, string>();

        public GestionUsuariosForm()
        {
            _servicioUsuarios = new UserDataService();
            InitializeComponent();
            CargarRoles();
            CargarUsuarios();
        }

        private void InitializeComponent()
        {
            this.Text = "Gestión de Usuarios - SALC";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Panel de filtros
            panelFiltros = new Panel
            {
                Size = new Size(960, 80),
                Location = new Point(20, 20),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblBuscar = new Label
            {
                Text = "Buscar por nombre o DNI:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(20, 15),
                Size = new Size(180, 25)
            };

            txtBuscar = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point(200, 10),
                Font = new Font("Segoe UI", 10)
            };
            txtBuscar.TextChanged += TxtBuscar_TextChanged;

            lblFiltroRol = new Label
            {
                Text = "Filtrar por rol:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(420, 15),
                Size = new Size(100, 25)
            };

            cmbFiltroRol = new ComboBox
            {
                Size = new Size(150, 30),
                Location = new Point(520, 10),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFiltroRol.Items.Add("Todos los roles");
            cmbFiltroRol.SelectedIndex = 0;
            cmbFiltroRol.SelectedIndexChanged += CmbFiltroRol_SelectedIndexChanged;

            btnActualizar = new Button
            {
                Text = "?? Actualizar",
                Size = new Size(120, 35),
                Location = new Point(700, 8),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnActualizar.Click += BtnActualizar_Click;

            panelFiltros.Controls.AddRange(new Control[] {
                lblBuscar, txtBuscar, lblFiltroRol, cmbFiltroRol, btnActualizar
            });

            // DataGridView
            dataGridViewUsuarios = new DataGridView
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

            dataGridViewUsuarios.Columns.Add("DNI", "DNI");
            dataGridViewUsuarios.Columns.Add("Nombre", "Nombre");
            dataGridViewUsuarios.Columns.Add("Apellido", "Apellido");
            dataGridViewUsuarios.Columns.Add("Email", "Email");
            dataGridViewUsuarios.Columns.Add("Rol", "Rol");
            dataGridViewUsuarios.Columns.Add("Estado", "Estado");

            dataGridViewUsuarios.Columns["DNI"].Width = 100;
            dataGridViewUsuarios.Columns["Nombre"].Width = 120;
            dataGridViewUsuarios.Columns["Apellido"].Width = 120;
            dataGridViewUsuarios.Columns["Email"].Width = 200;
            dataGridViewUsuarios.Columns["Rol"].Width = 100;
            dataGridViewUsuarios.Columns["Estado"].Width = 100;

            // Panel de botones
            Panel panelBotones = new Panel
            {
                Size = new Size(960, 50),
                Location = new Point(20, 620),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnAgregar = new Button
            {
                Text = "? Agregar Usuario",
                Size = new Size(150, 35),
                Location = new Point(20, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAgregar.Click += BtnAgregar_Click;

            btnEditar = new Button
            {
                Text = "?? Editar Usuario",
                Size = new Size(150, 35),
                Location = new Point(190, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 193, 7),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnEditar.Click += BtnEditar_Click;

            btnDesactivar = new Button
            {
                Text = "??? Desactivar Usuario",
                Size = new Size(150, 35),
                Location = new Point(360, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDesactivar.Click += BtnDesactivar_Click;

            btnActivar = new Button
            {
                Text = "? Activar Usuario",
                Size = new Size(150, 35),
                Location = new Point(530, 7),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnActivar.Click += BtnActivar_Click;

            panelBotones.Controls.AddRange(new Control[] {
                btnAgregar, btnEditar, btnDesactivar, btnActivar
            });

            this.Controls.AddRange(new Control[] {
                panelFiltros, dataGridViewUsuarios, panelBotones
            });
        }

        private void CargarRoles()
        {
            try
            {
                diccionarioRoles = _servicioUsuarios.ObtenerRoles();
                cmbFiltroRol.Items.Clear();
                cmbFiltroRol.Items.Add("Todos los roles");
                foreach (var kv in diccionarioRoles)
                {
                    cmbFiltroRol.Items.Add(kv.Value);
                }
                cmbFiltroRol.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar roles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarUsuarios(string filtroNombre = "", string filtroRol = "")
        {
            try
            {
                // Convertir "Todos los roles" a cadena vacía para el servicio
                if (filtroRol == "Todos los roles") filtroRol = "";

                var usuarios = _servicioUsuarios.ObtenerUsuarios(filtroNombre, filtroRol);
                dataGridViewUsuarios.Rows.Clear();
                
                foreach (var u in usuarios)
                {
                    dataGridViewUsuarios.Rows.Add(
                        u.Dni, 
                        u.Nombre, 
                        u.Apellido, 
                        u.Email ?? "", 
                        u.NombreRol ?? "Desconocido", 
                        u.Estado ?? "Desconocido"
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtBuscar_TextChanged(object sender, EventArgs e) => AplicarFiltros();

        private void CmbFiltroRol_SelectedIndexChanged(object sender, EventArgs e) => AplicarFiltros();

        private void AplicarFiltros()
        {
            string filtroNombre = txtBuscar.Text.Trim();
            string filtroRol = cmbFiltroRol.SelectedItem?.ToString() ?? "";
            CargarUsuarios(filtroNombre, filtroRol);
        }

        private void BtnActualizar_Click(object sender, EventArgs e)
        {
            CargarRoles();
            CargarUsuarios();
            txtBuscar.Clear();
            cmbFiltroRol.SelectedIndex = 0;
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                var roles = _servicioUsuarios.ObtenerRoles();
                var supervisores = _servicioUsuarios.ObtenerMedicosParaSupervisor();

                // Crear y configurar el diálogo para creación
                var dialogo = new DialogoFormularioUsuario();
                dialogo.ConfigurarParaCreacion(roles, supervisores);

                // Mostrar diálogo y procesar resultado
                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    var usuario = dialogo.Usuario;
                    _servicioUsuarios.CrearUsuario(usuario);

                    MessageBox.Show("Usuario creado exitosamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Actualizar la lista de usuarios
                    CargarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear usuario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para editar.", "Selección requerida", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Obtener el DNI del usuario seleccionado
                DataGridViewRow filaSeleccionada = dataGridViewUsuarios.SelectedRows[0];
                int dni = Convert.ToInt32(filaSeleccionada.Cells["DNI"].Value);

                // Usar el servicio para obtener datos completos del usuario
                var usuario = _servicioUsuarios.ObtenerUsuario(dni);
                
                if (usuario == null)
                {
                    MessageBox.Show("No se encontró el usuario seleccionado.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Obtener roles y supervisores disponibles
                var roles = _servicioUsuarios.ObtenerRoles();
                var supervisores = _servicioUsuarios.ObtenerMedicosParaSupervisor();

                // Crear y configurar el diálogo para edición
                var dialogo = new DialogoFormularioUsuario();
                dialogo.ConfigurarParaEdicion(usuario, roles, supervisores);

                // Mostrar diálogo y procesar resultado
                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    var usuarioEditado = dialogo.Usuario;
                    bool cambiarContrasena = !string.IsNullOrEmpty(usuarioEditado.PasswordHash);
                    
                    _servicioUsuarios.ActualizarUsuario(usuarioEditado, cambiarContrasena);

                    MessageBox.Show("Usuario actualizado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Actualizar la lista de usuarios
                    CargarUsuarios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al editar usuario: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDesactivar_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para desactivar.", "Selección requerida", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow filaSeleccionada = dataGridViewUsuarios.SelectedRows[0];
            int dni = Convert.ToInt32(filaSeleccionada.Cells["DNI"].Value);
            string nombre = filaSeleccionada.Cells["Nombre"].Value.ToString();
            string apellido = filaSeleccionada.Cells["Apellido"].Value.ToString();

            var resultado = MessageBox.Show(
                $"¿Está seguro que desea desactivar al usuario {nombre} {apellido} (DNI: {dni})?",
                "Confirmar desactivación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    bool exito = _servicioUsuarios.DesactivarUsuario(dni);
                    if (exito)
                        MessageBox.Show("Usuario desactivado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No se pudo desactivar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    CargarUsuarios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desactivar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnActivar_Click(object sender, EventArgs e)
        {
            if (dataGridViewUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un usuario para activar.", "Selección requerida", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow filaSeleccionada = dataGridViewUsuarios.SelectedRows[0];
            int dni = Convert.ToInt32(filaSeleccionada.Cells["DNI"].Value);
            string nombre = filaSeleccionada.Cells["Nombre"].Value.ToString();
            string apellido = filaSeleccionada.Cells["Apellido"].Value.ToString();

            var resultado = MessageBox.Show(
                $"¿Está seguro que desea activar al usuario {nombre} {apellido} (DNI: {dni})?",
                "Confirmar activación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    bool exito = _servicioUsuarios.ActivarUsuario(dni);
                    if (exito)
                        MessageBox.Show("Usuario activado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("No se pudo activar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    CargarUsuarios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al activar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}