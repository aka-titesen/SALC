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
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Configurar el formulario principal
            this.Text = "SALC - Acceso al Sistema";
            this.Size = new Size(500, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(233, 236, 239);

            // Panel principal contenedor
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(233, 236, 239),
                Padding = new Padding(20)
            };

            // Contenedor de login centrado
            Panel loginContainer = new Panel
            {
                Size = new Size(400, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(50, 75)
            };

            // Logo/Icono
            PictureBox logoBox = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(160, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(0, 123, 255)
            };

            // Intentar cargar el icono
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, "..", "..", "..", "icono.png");
                if (File.Exists(iconPath))
                {
                    logoBox.Image = Image.FromFile(iconPath);
                    logoBox.BackColor = Color.Transparent;
                }
            }
            catch { /* Mantener color de fondo azul si no se puede cargar */ }

            // Título principal
            Label titleLabel = new Label
            {
                Text = "SALC",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 35),
                Location = new Point(10, 120)
            };

            // Subtítulo
            Label subtitleLabel = new Label
            {
                Text = "Sistema de Administración de Laboratorio Clínico",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 40),
                Location = new Point(10, 160)
            };

            // Título de sección
            Label loginTitle = new Label
            {
                Text = "Iniciar Sesión",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 30),
                Location = new Point(10, 220)
            };

            // Label Usuario
            Label userLabel = new Label
            {
                Text = "👤 Usuario",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(150, 25),
                Location = new Point(50, 270)
            };

            // TextBox Usuario
            userTextBox = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(50, 300),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Label Contraseña
            Label passwordLabel = new Label
            {
                Text = "🔒 Contraseña",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(150, 25),
                Location = new Point(50, 340)
            };

            // TextBox Contraseña
            passwordTextBox = new TextBox
            {
                Size = new Size(300, 30),
                Location = new Point(50, 370),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
                BackColor = Color.White
            };

            // Botón de Login
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

            // Label de error
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

            // Footer
            Label footerLabel = new Label
            {
                Text = "© 2025 SALC - Desarrollado con .NET Framework",
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = Color.FromArgb(108, 117, 125),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(380, 15),
                Location = new Point(10, 520)
            };

            // Eventos
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
            };

            passwordTextBox.KeyPress += (sender, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    loginButton.PerformClick();
                    e.Handled = true;
                }
            };

            // Agregar todos los controles al contenedor de login
            loginContainer.Controls.AddRange(new Control[] 
            { 
                logoBox, titleLabel, subtitleLabel, loginTitle,
                userLabel, userTextBox, passwordLabel, passwordTextBox, 
                loginButton, errorLabel, footerLabel
            });

            // Agregar el contenedor al panel principal
            mainPanel.Controls.Add(loginContainer);

            // Agregar el panel principal al formulario
            this.Controls.Add(mainPanel);

            // Configurar foco inicial
            userTextBox.Focus();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (ValidateCredentials(userTextBox.Text, passwordTextBox.Text))
            {
                ShowSuccessMessage();
                this.Hide();
                
                Form1 mainForm = new Form1();
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                ShowErrorMessage("Usuario o contraseña incorrectos. Inténtelo de nuevo.");
                passwordTextBox.Text = "";
                passwordTextBox.Focus();
            }
        }

        private void ShowErrorMessage(string message)
        {
            errorLabel.Text = message;
            errorLabel.ForeColor = Color.FromArgb(220, 53, 69);
            errorLabel.Visible = true;
        }

        private void ShowSuccessMessage()
        {
            errorLabel.Text = "¡Inicio de sesión exitoso!";
            errorLabel.ForeColor = Color.FromArgb(40, 167, 69);
            errorLabel.Visible = true;
        }

        private bool ValidateCredentials(string username, string password)
        {
            return UserAuthentication.ValidateCredentials(username, password);
        }
    }
}
