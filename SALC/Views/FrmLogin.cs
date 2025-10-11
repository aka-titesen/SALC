using System;
using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views
{
    public class FrmLogin : Form, ILoginView
    {
        private TextBox txtDni;
        private TextBox txtContrasenia;
        private Button btnAcceder;
        private ErrorProvider errorProvider;

        public FrmLogin()
        {
            Text = "SALC - Inicio de Sesión";
            Width = 350;
            Height = 180;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            txtDni = new TextBox { Left = 130, Top = 20, Width = 200, TabIndex = 0 };
            txtContrasenia = new TextBox { Left = 130, Top = 60, Width = 200, PasswordChar = '•', TabIndex = 1 };
            btnAcceder = new Button { Left = 230, Top = 100, Width = 100, Text = "Acceder", TabIndex = 2 };
            errorProvider = new ErrorProvider();

            Controls.Add(new Label { Left = 20, Top = 23, Width = 100, Text = "DNI:" });
            Controls.Add(new Label { Left = 20, Top = 63, Width = 100, Text = "Contraseña:" });
            Controls.Add(txtDni);
            Controls.Add(txtContrasenia);
            Controls.Add(btnAcceder);

            // Hacer que Enter en los TextBox active el botón Acceder
            txtDni.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnAcceder.PerformClick(); } };
            txtContrasenia.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnAcceder.PerformClick(); } };
            
            btnAcceder.Click += (s, e) => AccederClick?.Invoke(this, EventArgs.Empty);

            // Hacer que el botón sea el botón por defecto
            AcceptButton = btnAcceder;
        }

        public string DniTexto => txtDni.Text?.Trim();
        public string Contrasenia => txtContrasenia.Text;

        public event EventHandler AccederClick;

        public void MostrarError(string mensaje)
        {
            errorProvider.Clear();
            MessageBox.Show(this, mensaje, "Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void Cerrar()
        {
            Close();
        }

        public void LimpiarCampos()
        {
            txtDni.Clear();
            txtContrasenia.Clear();
            txtDni.Focus();
            errorProvider.Clear();
        }
    }
}
