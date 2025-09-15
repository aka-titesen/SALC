// Views/LoginForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SALC
{
    public partial class LoginForm : Form
    {
        private TextBox userTextBox;
        private TextBox passwordTextBox;
        private Label errorLabel;
        private Button loginButton;

        public LoginForm()
        {
            InitializeComponent(); // Asegúrate de que LoginForm.Designer.cs esté sincronizado
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "SALC - Acceso al Sistema";
            this.Size = new Size(500, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(233, 236, 239);

            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(233, 236, 239),
                Padding = new Padding(20)
            };

            Panel loginContainer = new Panel
            {
                Size = new Size(400, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(50, 75)
            };

            PictureBox logoBox = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(160, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(0, 123, 255)
            };

            try
            {
                // Ajusta la ruta si es necesario
                string iconPath = Path.Combine(Application.StartupPath, "..", "..", "..", "icono.png");
                if (File.Exists(iconPath))
                {
                    logoBox.Image = Image.FromFile(iconPath);
                    logoBox.BackColor = Color.Transparent;
                }
            }
            catch { /* Mantener color de fondo azul si no se puede cargar */ }

            Label titleLabel = new Label
            {
                Text = "SALC",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 35),
                Location = new Point(10, 120)
            };

            Label subtitleLabel = new Label
            {
                Text = "Sistema de Administración de Laboratorio Clínico",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 40),
                Location = new Point(10, 160)
            };

            Label loginTitle = new Label
            {
                Text = "Iniciar Sesión",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 30),
                Location = new Point(10, 220)
            };

            Label userLabel = new Label
            {
                Text = "👤 Usuario (DNI)",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(150, 25),
                Location = new Point(50, 270)
            };

            userTextBox = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(50, 300),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            Label passwordLabel = new Label
            {
                Text = "🔒 Contraseña",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(150, 25),
                Location = new Point(50, 340)
            };

            passwordTextBox = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(50, 370),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
                BackColor = Color.White
            };

            loginButton = new Button
            {
                Text = "Acceder al Sistema",
                Size = new Size(300, 45),
                Location = new Point(50, 420),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            loginButton.FlatAppearance.BorderSize = 0;

            errorLabel = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(300, 20),
                Location = new Point(50, 475),
                Visible = false
            };

            Label footerLabel = new Label
            {
                Text = "© 2025 SALC - Desarrollado con .NET Framework",
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 15),
                Location = new Point(10, 520)
            };

            loginButton.Click += LoginButton_Click;
            loginButton.MouseEnter += (s, e) => loginButton.BackColor = Color.FromArgb(0, 86, 179);
            loginButton.MouseLeave += (s, e) => loginButton.BackColor = Color.FromArgb(0, 123, 255);

            userTextBox.KeyPress += (sender, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    passwordTextBox.Focus();
                    e.Handled = true;
                }
                else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    // Bloquear cualquier cosa que no sea número ni tecla de control (ej: borrar)
                    e.Handled = true;
                }
            };

            passwordTextBox.KeyPress += (sender, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    loginButton.PerformClick();
                    e.Handled = true;
                }
            };

            loginContainer.Controls.AddRange(new Control[]
            {
                logoBox, titleLabel, subtitleLabel, loginTitle,
                userLabel, userTextBox, passwordLabel, passwordTextBox,
                loginButton, errorLabel, footerLabel
            });

            mainPanel.Controls.Add(loginContainer);
            this.Controls.Add(mainPanel);
            userTextBox.Focus();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            // Usar los TextBox definidos en esta clase
            string username = userTextBox.Text.Trim();
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, ingrese nombre de usuario (DNI) y contraseña.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool loginSuccessful = UserAuthentication.Login(username, password);

            if (loginSuccessful)
            {
                MessageBox.Show($"¡Bienvenido, {UserAuthentication.CurrentUser.Nombre}!", "Inicio de Sesión Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                MainDashboardForm mainForm = new MainDashboardForm();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Nombre de usuario (DNI) o contraseña incorrectos.", "Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                passwordTextBox.Clear();
                passwordTextBox.Focus();
            }
        }
        // Fin del método LoginButton_Click
        // Fin de la clase LoginForm
    }
    // Fin del namespace SALC
}