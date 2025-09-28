using SALC;
using SALC.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
            
            this.Load += AdminForm_Load;

        }

        private int? DniSeleccionado()
        {
            if (dgvUsers.CurrentRow == null) return null;

            var valor = dgvUsers.CurrentRow.Cells["Dni"].Value; // "Dni" = nombre de la columna
            if (valor == null) return null;

            return Convert.ToInt32(valor);
        }
        private void AdminForm_Load(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                // Acá llamás a tu lógica
                var listaUsuarios = UserData.GetUsers();

                // Lo asignás como origen de datos de la grilla
                dgvUsers.DataSource = null;   // limpia cualquier binding anterior
                dgvUsers.DataSource = listaUsuarios;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}");
            }
        }
        private void btnActivar_Click(object sender, EventArgs e)
        {
            int? dni = DniSeleccionado();
            if (dni == null)
            {
                MessageBox.Show("Seleccioná un usuario primero.");
                return;
            }

            UserData.ActivateUser(dni.Value);
            LoadUsers(); 
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            int? dni = DniSeleccionado();
            if (dni == null)
            {
                MessageBox.Show("Seleccioná un usuario primero.");
                return;
            }

            UserData.DeleteUser(dni.Value);
            LoadUsers();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) { MessageBox.Show("Seleccioná un usuario."); return; }

            using var editform = new EditForm();
            editform.ShowDialog();
        }

        private void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            using var addForm = new ADDForm();
            addForm.ShowDialog();
        }
    }
}
