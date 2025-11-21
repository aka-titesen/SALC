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
        private Label lblSubtitulo;
        private Label lblVersion;
        private Panel panelPrincipal;
        private Panel panelFormulario;
        private Label lblDni;
        private Label lblContrasenia;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Acceso al Sistema - SALC";
            Size = new Size(600, 550);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Panel principal con gradiente suave
            panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(250, 252, 255) // Azul muy claro
            };

            // Logo centrado con borde redondeado
            picLogo = new PictureBox
            {
                Size = new Size(140, 140),
                Location = new Point((600 - 140) / 2, 40),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.None
            };

            // Cargar icono del proyecto
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "..", "..", "..", "icono.png");
                if (System.IO.File.Exists(iconPath))
                {
                    picLogo.Image = Image.FromFile(iconPath);
                }
                else
                {
                    // Fallback con logo placeholder
                    picLogo.BackColor = Color.FromArgb(100, 149, 237); // Cornflower blue - médico
                    picLogo.Paint += (s, e) => {
                        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        using (var font = new Font("Segoe UI", 24, FontStyle.Bold))
                        using (var brush = new SolidBrush(Color.White))
                        {
                            var text = "SALC";
                            var size = e.Graphics.MeasureString(text, font);
                            e.Graphics.DrawString(text, font, brush, 
                                (picLogo.Width - size.Width) / 2, 
                                (picLogo.Height - size.Height) / 2);
                        }
                    };
                }
            }
            catch
            {
                picLogo.BackColor = Color.FromArgb(100, 149, 237);
                picLogo.Paint += (s, e) => {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (var font = new Font("Segoe UI", 24, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.White))
                    {
                        var text = "SALC";
                        var size = e.Graphics.MeasureString(text, font);
                        e.Graphics.DrawString(text, font, brush, 
                            (picLogo.Width - size.Width) / 2, 
                            (picLogo.Height - size.Height) / 2);
                    }
                };
            }

            // Título principal - Tipografía grande
            lblTitulo = new Label
            {
                Text = "Sistema de Administración",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185), // Azul médico profesional
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 195),
                Size = new Size(500, 35),
                BackColor = Color.Transparent
            };

            // Subtítulo - Tipografía mediana
            lblSubtitulo = new Label
            {
                Text = "de Laboratorio Clínico",
                Font = new Font("Segoe UI", 15, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 152, 219), // Azul claro
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 230),
                Size = new Size(500, 30),
                BackColor = Color.Transparent
            };

            // Panel de formulario con sombra sutil
            panelFormulario = new Panel
            {
                Size = new Size(400, 190),
                Location = new Point(100, 280),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Etiqueta DNI - SOBRE el campo
            lblDni = new Label
            {
                Text = "Documento Nacional de Identidad",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(30, 25),
                Size = new Size(340, 20),
                BackColor = Color.Transparent
            };

            // Campo DNI
            txtDni = new TextBox
            {
                Location = new Point(30, 48),
                Size = new Size(340, 28),
                Font = new Font("Segoe UI", 11),
                TabIndex = 0,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Etiqueta Contraseña - SOBRE el campo
            lblContrasenia = new Label
            {
                Text = "Contraseña de acceso",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(30, 90),
                Size = new Size(340, 20),
                BackColor = Color.Transparent
            };

            // Campo Contraseña
            txtContrasenia = new TextBox
            {
                Location = new Point(30, 113),
                Size = new Size(340, 28),
                Font = new Font("Segoe UI", 11),
                PasswordChar = '•',
                TabIndex = 1,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Botón Acceder - Estilo Microsoft
            btnAcceder = new Button
            {
                Text = "Acceder",
                Location = new Point(230, 150),
                Size = new Size(140, 32),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 134, 193), // Azul médico
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TabIndex = 2,
                Cursor = Cursors.Hand
            };
            btnAcceder.FlatAppearance.BorderSize = 0;
            btnAcceder.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 152, 219);
            btnAcceder.FlatAppearance.MouseDownBackColor = Color.FromArgb(41, 128, 185);

            // Versión del sistema - Esquina inferior
            lblVersion = new Label
            {
                Text = "SALC v1.0",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(149, 165, 166), // Gris suave
                TextAlign = ContentAlignment.BottomRight,
                Location = new Point(450, 485),
                Size = new Size(130, 20),
                BackColor = Color.Transparent
            };

            // Agregar controles al formulario
            panelFormulario.Controls.AddRange(new Control[] {
                lblDni, txtDni, lblContrasenia, txtContrasenia, btnAcceder
            });

            panelPrincipal.Controls.AddRange(new Control[] {
                picLogo, lblTitulo, lblSubtitulo, panelFormulario, lblVersion
            });

            Controls.Add(panelPrincipal);

            // Eventos de teclado
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
            MessageBox.Show(this, mensaje, "SALC - Error de Autenticación", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
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