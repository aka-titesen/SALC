using System;
using System.Windows.Forms;
using System.Drawing;

namespace SALC.Views.Compartidos
{
    public class FrmInicio : Form
    {
        private Panel panelSuperior;
        private Panel panelCentral;
        private Panel panelInferior;
        private Label lblBienvenida;
        private Label lblSubtitulo;
        private PictureBox picLogo;
        private Label lblVersion;
        
        // Información de la clínica
        private GroupBox grpInformacion;
        private Label lblInfoClinica;
        
        private GroupBox grpContacto;
        private Label lblDatosContacto;
        
        private GroupBox grpHorarios;
        private Label lblHorariosAtencion;

        public FrmInicio()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Inicio";
            BackColor = Color.White;
            Dock = DockStyle.Fill;
            FormBorderStyle = FormBorderStyle.None;

            // ============ PANEL SUPERIOR - BIENVENIDA ============
            panelSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.FromArgb(245, 250, 255), // Azul muy claro
                Padding = new Padding(40, 30, 40, 20)
            };

            // Logo en el panel superior
            picLogo = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(40, 50),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Cargar logo
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "icono.png");
                if (System.IO.File.Exists(iconPath))
                {
                    picLogo.Image = Image.FromFile(iconPath);
                }
                else
                {
                    picLogo.BackColor = Color.FromArgb(100, 149, 237);
                    picLogo.Paint += (s, e) => {
                        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
                        using (var brush = new SolidBrush(Color.White))
                        {
                            e.Graphics.DrawString("SALC", font, brush, new PointF(18, 32));
                        }
                    };
                }
            }
            catch
            {
                picLogo.BackColor = Color.FromArgb(100, 149, 237);
                picLogo.Paint += (s, e) => {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (var font = new Font("Segoe UI", 18, FontStyle.Bold))
                    using (var brush = new SolidBrush(Color.White))
                    {
                        e.Graphics.DrawString("SALC", font, brush, new PointF(18, 32));
                    }
                };
            }

            // Título de bienvenida - Tipografía grande
            lblBienvenida = new Label
            {
                Text = "Bienvenido al Sistema de Gestión Clínica",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185), // Azul médico
                Location = new Point(170, 50),
                Size = new Size(700, 40),
                BackColor = Color.Transparent
            };

            // Subtítulo - Tipografía mediana
            lblSubtitulo = new Label
            {
                Text = "Laboratorio Clínico de Análisis y Diagnóstico",
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141), // Gris suave
                Location = new Point(170, 95),
                Size = new Size(650, 30),
                BackColor = Color.Transparent
            };

            // Versión del sistema
            lblVersion = new Label
            {
                Text = "Versión 1.0",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(170, 130),
                Size = new Size(200, 25),
                BackColor = Color.Transparent
            };

            panelSuperior.Controls.AddRange(new Control[] {
                picLogo, lblBienvenida, lblSubtitulo, lblVersion
            });

            // ============ PANEL CENTRAL - INFORMACIÓN ============
            panelCentral = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(40, 20, 40, 20)
            };

            // Crear un TableLayoutPanel para distribuir los elementos uniformemente
            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            // Configurar columnas - distribución equitativa
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            // Configurar fila
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Grupo: Información de la Clínica
            grpInformacion = new GroupBox
            {
                Text = "  Información del Centro Médico  ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 10, 0),
                BackColor = Color.FromArgb(250, 252, 255), // Fondo azul muy claro
                Padding = new Padding(15, 10, 15, 15)
            };

            lblInfoClinica = new Label
            {
                Text = 
                    "Laboratorio Clínico SALC\n\n" +
                    "Somos un centro especializado en análisis\n" +
                    "clínicos y diagnóstico médico de precisión.\n\n" +
                    "Nuestros Servicios:\n\n" +
                    "• Análisis de sangre completos\n" +
                    "• Estudios bioquímicos\n" +
                    "• Hematología clínica\n" +
                    "• Microbiología\n" +
                    "• Inmunología y serología\n" +
                    "• Análisis de orina y fluidos\n\n" +
                    "Contamos con tecnología de última\n" +
                    "generación y personal altamente capacitado.",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            grpInformacion.Controls.Add(lblInfoClinica);

            // Grupo: Datos de Contacto
            grpContacto = new GroupBox
            {
                Text = "  Contacto y Ubicación  ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96), // Verde médico
                Dock = DockStyle.Fill,
                Margin = new Padding(5, 0, 5, 0),
                BackColor = Color.FromArgb(248, 255, 250), // Fondo verde muy claro
                Padding = new Padding(15, 10, 15, 15)
            };

            lblDatosContacto = new Label
            {
                Text = 
                    "Dirección:\n" +
                    "Av. Principal 1234, Ciudad\n" +
                    "Código Postal 2000\n\n" +
                    "Teléfonos:\n" +
                    "341-5123456 (Recepción)\n" +
                    "341-5123457 (Urgencias)\n\n" +
                    "Correo Electrónico:\n" +
                    "contacto@laboratoriosaLc.com\n" +
                    "resultados@laboratoriosaLc.com\n\n" +
                    "Sitio Web:\n" +
                    "www.laboratoriosaLc.com",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            grpContacto.Controls.Add(lblDatosContacto);

            // Grupo: Horarios de Atención
            grpHorarios = new GroupBox
            {
                Text = "  Horarios de Atención  ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34), // Naranja suave
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 0, 0, 0),
                BackColor = Color.FromArgb(255, 250, 245), // Fondo naranja muy claro
                Padding = new Padding(15, 10, 15, 15)
            };

            lblHorariosAtencion = new Label
            {
                Text = 
                    "Lunes a Viernes: 7:00 a 19:00 hs.\n" +
                    "Sábados: 8:00 a 13:00 hs.\n" +
                    "Domingos y feriados: Cerrado",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            grpHorarios.Controls.Add(lblHorariosAtencion);

            // Agregar los controles al TableLayoutPanel
            tableLayout.Controls.Add(grpInformacion, 0, 0);
            tableLayout.Controls.Add(grpContacto, 1, 0);
            tableLayout.Controls.Add(grpHorarios, 2, 0);

            panelCentral.Controls.Add(tableLayout);

            // ============ PANEL INFERIOR - INFORMACIÓN DEL SISTEMA ============
            panelInferior = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.FromArgb(41, 128, 185), // Azul médico
                Padding = new Padding(40, 15, 40, 15)
            };

            var lblInfoSistema = new Label
            {
                Text = 
                    "Sistema en Operación   |   Base de Datos Conectada   |   " +
                    "Seguridad Activa   |   Todas las Funcionalidades Operativas\n" +
                    "Arquitectura MVP de 3 Capas   •   Autenticación BCrypt   •   " +
                    "Consultas Parametrizadas   •   Gestión Integral de Análisis Clínicos",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            panelInferior.Controls.Add(lblInfoSistema);

            // Agregar paneles al formulario
            Controls.AddRange(new Control[] {
                panelCentral, panelSuperior, panelInferior
            });
        }
    }
}