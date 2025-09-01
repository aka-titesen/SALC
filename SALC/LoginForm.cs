using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private void InitializeCustomComponents()
        {
            // Configurar el formulario
            this.Text = "SALC - Sistema de Administración de Laboratorio Clínico";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Panel principal
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Logo/Icono (placeholder)
            PictureBox logoBox = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(160, 30),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            // Título principal
            Label titleLabel = new Label
            {
                Text = "SALC",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(200, 40),
                Location = new Point(100, 120)
            };

            // Subtítulo
            Label subtitleLabel = new Label
            {
                Text = "Sistema de Administración de Laboratorio Clínico",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(128, 128, 128),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(300, 20),
                Location = new Point(50, 160)
            };

            // Panel de credenciales
            Panel credentialsPanel = new Panel
            {
                Size = new Size(320, 200),
                Location = new Point(40, 200),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Label Usuario
            Label userLabel = new Label
            {
                Text = "Usuario:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64),
                Size = new Size(80, 20),
                Location = new Point(20, 20)
            };

            // TextBox Usuario
            TextBox userTextBox = new TextBox
            {
                Size = new Size(250, 25),
                Location = new Point(20, 45),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Label Contraseña
            Label passwordLabel = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64),
                Size = new Size(80, 20),
                Location = new Point(20, 80)
            };

            // TextBox Contraseña
            TextBox passwordTextBox = new TextBox
            {
                Size = new Size(250, 25),
                Location = new Point(20, 105),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };

            // Botón de Login
            Button loginButton = new Button
            {
                Text = "Iniciar Sesión",
                Size = new Size(250, 40),
                Location = new Point(20, 150),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Evento del botón de login
            loginButton.Click += (sender, e) =>
            {
                if (ValidateCredentials(userTextBox.Text, passwordTextBox.Text))
                {
                    this.Hide();
                    
                    Form1 mainForm = new Form1();
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error de Autenticación", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    passwordTextBox.Text = "";
                    passwordTextBox.Focus();
                }
            };

            // Evento Enter en los TextBox
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

            // Agregar controles al panel de credenciales
            credentialsPanel.Controls.AddRange(new Control[] 
            { 
                userLabel, userTextBox, passwordLabel, passwordTextBox, loginButton 
            });

            // Agregar controles al panel principal
            mainPanel.Controls.AddRange(new Control[] 
            { 
                logoBox, titleLabel, subtitleLabel, credentialsPanel 
            });

            // Agregar panel principal al formulario
            this.Controls.Add(mainPanel);

            // Configurar foco inicial
            userTextBox.Focus();
        }

        private bool ValidateCredentials(string username, string password)
        {
            return UserAuthentication.ValidateCredentials(username, password);
        }
    }
}
