using System;
using System.Drawing;
using System.Windows.Forms;
using SALC.Views.Interfaces;
using SALC.Presenters;

namespace SALC
{
    public class LoginFormNew : Form, ILoginView
    {
        private TextBox userTextBox;
        private TextBox passwordTextBox;
        private Button loginButton;
        private LoginPresenter _presenter;

        public event EventHandler LoginRequested;

        public string Username => userTextBox?.Text?.Trim() ?? string.Empty;
        public string Password => passwordTextBox?.Text ?? string.Empty;

        public LoginFormNew()
        {
            InitializeMinimalUI();
            _presenter = new LoginPresenter(this);
        }

        private void InitializeMinimalUI()
        {
            this.Text = "SALC - Acceso";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 420; this.Height = 210;

            var lblUser = new Label { Left = 20, Top = 20, Width = 140, Text = "Usuario (DNI)" };
            var lblPass = new Label { Left = 20, Top = 65, Width = 140, Text = "Contraseña" };

            userTextBox = new TextBox { Left = 160, Top = 18, Width = 220 };
            passwordTextBox = new TextBox { Left = 160, Top = 63, Width = 220, UseSystemPasswordChar = true };
            loginButton = new Button { Left = 160, Top = 110, Width = 220, Text = "Iniciar Sesión" };
            loginButton.Click += (s, e) => LoginRequested?.Invoke(this, EventArgs.Empty);

            Controls.AddRange(new Control[] { lblUser, userTextBox, lblPass, passwordTextBox, loginButton });
        }

        public void ShowValidationMessage(string message)
        {
            MessageBox.Show(message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ShowLoginError(string message)
        {
            MessageBox.Show(message, "Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            passwordTextBox.Clear();
            passwordTextBox.Focus();
        }

        public void NavigateToDashboard()
        {
            var main = new MainDashboardForm();
            main.Show();
            Hide();
        }
    }
}
