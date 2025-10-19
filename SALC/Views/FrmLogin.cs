using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views
{
    public class FrmLogin : Form, ILoginView
    {
        private TextBox txtDni;
        private TextBox txtContrasenia;
        private Button btnAcceder;
        private ErrorProvider errorProvider;
        private PictureBox picLogo;
        private Label lblTitulo;
        private Label lblVersion;
        private Panel panelPrincipal;
        private Panel panelFormulario;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "SALC - Acceso al Sistema";
            Size = new Size(520, 480);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 250, 255);

            // Panel principal
            panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40)
            };

            // Logo centrado
            picLogo = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point(200, 30),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Cargar icono
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "..", "..", "..", "icono.png");
                if (System.IO.File.Exists(iconPath))
                {
                    picLogo.Image = Image.FromFile(iconPath);
                }
                else
                {
                    picLogo.BackColor = Color.FromArgb(70, 130, 180);
                    picLogo.Paint += (s, e) => {
                        using (var font = new Font("Arial", 20, FontStyle.Bold))
                        {
                            e.Graphics.DrawString("SALC", font, Brushes.White, new PointF(25, 45));
                        }
                    };
                }
            }
            catch
            {
                picLogo.BackColor = Color.FromArgb(70, 130, 180);
                picLogo.Paint += (s, e) => {
                    using (var font = new Font("Arial", 20, FontStyle.Bold))
                    {
                        e.Graphics.DrawString("SALC", font, Brushes.White, new PointF(25, 45));
                    }
                };
            }

            // Título
            lblTitulo = new Label
            {
                Text = "Sistema de Administración\nde Laboratorio Clínico",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 165),
                Size = new Size(400, 60)
            };

            // Versión
            lblVersion = new Label
            {
                Text = "SALC v1.0",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 230),
                Size = new Size(400, 25)
            };

            // Panel formulario
            panelFormulario = new Panel
            {
                Size = new Size(380, 150),
                Location = new Point(70, 270),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Campos formulario
            var lblDni = new Label
            {
                Text = "DNI:",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Location = new Point(40, 30),
                Size = new Size(60, 25)
            };

            txtDni = new TextBox
            {
                Location = new Point(110, 28),
                Size = new Size(220, 25),
                Font = new Font("Segoe UI", 11),
                TabIndex = 0
            };

            var lblContrasenia = new Label
            {
                Text = "Contraseña:",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Location = new Point(40, 70),
                Size = new Size(80, 25)
            };

            txtContrasenia = new TextBox
            {
                Location = new Point(130, 68),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11),
                PasswordChar = '•',
                TabIndex = 1
            };

            btnAcceder = new Button
            {
                Text = "ACCEDER",
                Location = new Point(230, 110),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TabIndex = 2,
                Cursor = Cursors.Hand
            };
            btnAcceder.FlatAppearance.BorderSize = 0;
            btnAcceder.FlatAppearance.MouseOverBackColor = Color.FromArgb(90, 150, 200);

            // Agregar controles
            panelFormulario.Controls.AddRange(new Control[] {
                lblDni, txtDni, lblContrasenia, txtContrasenia, btnAcceder
            });

            panelPrincipal.Controls.AddRange(new Control[] {
                picLogo, lblTitulo, lblVersion, panelFormulario
            });

            Controls.Add(panelPrincipal);

            // Eventos
            txtDni.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    txtContrasenia.Focus();
                }
            };

            txtContrasenia.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnAcceder.PerformClick();
                }
            };

            btnAcceder.Click += (s, e) => AccederClick?.Invoke(this, EventArgs.Empty);

            AcceptButton = btnAcceder;
            errorProvider = new ErrorProvider();
        }

        public string DniTexto => txtDni.Text?.Trim();
        public string Contrasenia => txtContrasenia.Text;

        public event EventHandler AccederClick;

        public void MostrarError(string mensaje)
        {
            errorProvider.Clear();
            MessageBox.Show(this, mensaje, "SALC - Error de Autenticación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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