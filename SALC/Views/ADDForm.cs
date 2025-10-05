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
    public partial class ADDForm : Form
    {
        public ADDForm()
        {
            InitializeComponent();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDni.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                comboBox2.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtPass.Text))
                    {
                        MessageBox.Show("Por favor complete todos los campos antes de guardar.",
                            "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

            var rol = 0;
            var Roltext = comboBox2.Text.Trim();
            if (Roltext == "Administrador")
            {
                rol = 1;
            }
            else if (Roltext == "Clinico")
            {
                rol = 2;
            }
            else if (Roltext == "Asistente")
            {
                rol = 5;
            }

            Usuario NewUser = new Usuario
            {
                Dni = int.Parse(txtDni.Text.Trim()),
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                id_rol = rol,
                // Cambiado Pass a password
                password = txtPass.Text.Trim(),

            };

            UserData.CreateUser(NewUser);
        }

        private void txtDni_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
