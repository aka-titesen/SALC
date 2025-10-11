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
        private Button btnEntrarAdmin;
        private Button btnEntrarMedico;
        private Button btnEntrarAsistente;
        private ErrorProvider errorProvider;

        public FrmLogin()
        {
            Text = "Inicio de Sesión";
            Width = 400;
            Height = 220;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            txtDni = new TextBox { Left = 130, Top = 20, Width = 200, TabIndex = 0 };
            txtContrasenia = new TextBox { Left = 130, Top = 60, Width = 200, PasswordChar = '•', TabIndex = 1 };
            btnAcceder = new Button { Left = 230, Top = 110, Width = 100, Text = "Acceder", TabIndex = 2 };
            btnEntrarAdmin = new Button { Left = 20, Top = 150, Width = 150, Text = "Entrar como Admin" };
            btnEntrarMedico = new Button { Left = 175, Top = 150, Width = 150, Text = "Entrar como Médico" };
            btnEntrarAsistente = new Button { Left = 330, Top = 150, Width = 150, Text = "Entrar como Asistente" };
            errorProvider = new ErrorProvider();

            Controls.Add(new Label { Left = 20, Top = 23, Width = 100, Text = "DNI:" });
            Controls.Add(new Label { Left = 20, Top = 63, Width = 100, Text = "Contraseña:" });
            Controls.Add(txtDni);
            Controls.Add(txtContrasenia);
            Controls.Add(btnAcceder);
            Controls.Add(btnEntrarAdmin);
            Controls.Add(btnEntrarMedico);
            Controls.Add(btnEntrarAsistente);

            btnAcceder.Click += (s, e) => AccederClick?.Invoke(this, EventArgs.Empty);
            btnEntrarAdmin.Click += (s, e) => AbrirPanel(new PanelAdministrador.FrmPanelAdministrador());
            btnEntrarMedico.Click += (s, e) => AbrirPanel(new PanelMedico.FrmPanelMedico());
            btnEntrarAsistente.Click += (s, e) => AbrirPanel(new PanelAsistente.FrmPanelAsistente());
        }

        public string DniTexto => txtDni.Text?.Trim();
        public string Contrasenia => txtContrasenia.Text;

        public event EventHandler AccederClick;

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(this, mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void Cerrar()
        {
            Close();
        }

        private void AbrirPanel(Form panel)
        {
            if (this.MdiParent != null)
            {
                panel.MdiParent = this.MdiParent;
                panel.Show();
                this.Close();
            }
            else
            {
                // Fallback por si no hay MDI parent (no debería ocurrir)
                panel.Show();
                this.Close();
            }
        }
    }
}
