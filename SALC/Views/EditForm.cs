using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SALC.Views
{
    public partial class EditForm : Form
    {
        private readonly Usuario _original;
        public Usuario EditedUser { get; private set; } = new Usuario();
        public bool ShouldChangePassword { get; private set; } = false;
        public EditForm(Usuario usuarioActual)
        {
            InitializeComponent();
            _original = usuarioActual;
            Load += UserEditDialog_Load;
        }

        private void UserEditDialog_Load(object? sender, EventArgs e)
        {
            var rol = "";
            var Roltext = _original.Rol;
            if (Roltext == "Administrador")
            {
                rol = "1";
            }
            else if (Roltext == "Clinico")
            {
                rol = "2";
            }
            else if (Roltext == "Asistente")
            {
                rol = "5";
            }

            comboBox2.DisplayMember = _original.Rol;
            comboBox2.ValueMember = rol;

            txtDni.Text = _original.Dni.ToString();
            txtDni.ReadOnly = true;
            txtNombre.Text = _original.Nombre ?? "";
            txtApellido.Text = _original.Apellido ?? "";
            txtEmail.Text = _original.Email ?? "";
            txtTelefono.Text = _original.Telefono ?? "";

            txtPass2.Text = "";
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtDni_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNombre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtApellido_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTelefono_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPass2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDni.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                comboBox2.SelectedIndex == -1) 
            {
                MessageBox.Show("Por favor complete todos los campos antes de guardar.",
                    "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var rol = "";
            var Roltext = comboBox2.Text.Trim();
            if(Roltext == "Administrador" )
            {
                rol = "1";
            }else if (Roltext == "Clinico")
            {
                rol = "2";
            }else if ( Roltext == "Asistente")
            {
                rol = "5";
            }

            EditedUser = new Usuario
            {
                Dni = _original.Dni,
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Rol = rol,

            };
            if (!string.IsNullOrWhiteSpace(txtPass2.Text))
            {
                EditedUser.Pass = txtPass2.Text;
                ShouldChangePassword = true;
            }
            else
            {
                ShouldChangePassword = false;
            }
            UserData.UpdateUser(EditedUser, ShouldChangePassword);

        }
    }
}
