using System;
using System.Windows.Forms;
using System.Drawing;

namespace SALC.Views.Compartidos
{
    public class FrmInicio : Form
    {
        private Panel panelBienvenida;
        private Label lblBienvenida;
        private Label lblDescripcion;
        private PictureBox picLogo;
        private Label lblVersion;
        private GroupBox groupInfo;
        private Label lblInfoEmpresa;
        private GroupBox groupContacto;
        private Label lblContacto;
        private GroupBox groupEstadisticas;
        private Label lblEstadisticas;

        public FrmInicio()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "SALC - Inicio";
            Size = new Size(900, 650);
            BackColor = Color.FromArgb(248, 250, 252);
            Dock = DockStyle.Fill;
            FormBorderStyle = FormBorderStyle.None;

            // Panel principal de bienvenida
            panelBienvenida = new Panel
            {
                Size = new Size(860, 180),
                Location = new Point(20, 20),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Logo principal
            picLogo = new PictureBox
            {
                Size = new Size(100, 100),
                Location = new Point(30, 40),
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Cargar logo
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
                        using (var font = new Font("Arial", 16, FontStyle.Bold))
                        {
                            e.Graphics.DrawString("SALC", font, Brushes.White, new PointF(20, 35));
                        }
                    };
                }
            }
            catch
            {
                picLogo.BackColor = Color.FromArgb(70, 130, 180);
                picLogo.Paint += (s, e) => {
                    using (var font = new Font("Arial", 16, FontStyle.Bold))
                    {
                        e.Graphics.DrawString("SALC", font, Brushes.White, new PointF(20, 35));
                    }
                };
            }

            // Título de bienvenida
            lblBienvenida = new Label
            {
                Text = "Bienvenido a SALC",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(160, 40),
                Size = new Size(450, 45),
                BackColor = Color.Transparent
            };

            // Descripción del sistema
            lblDescripcion = new Label
            {
                Text = "Sistema de Administración de Laboratorio Clínico",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(160, 90),
                Size = new Size(500, 25),
                BackColor = Color.Transparent
            };

            // Versión
            lblVersion = new Label
            {
                Text = "Versión 1.0 - Desarrollado para la excelencia en diagnóstico médico",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(160, 120),
                Size = new Size(500, 25),
                BackColor = Color.Transparent
            };

            panelBienvenida.Controls.AddRange(new Control[] {
                picLogo, lblBienvenida, lblDescripcion, lblVersion
            });

            // Información de la empresa/laboratorio
            groupInfo = new GroupBox
            {
                Text = " Información del Laboratorio ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 220),
                Size = new Size(420, 220),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(70, 130, 180)
            };

            lblInfoEmpresa = new Label
            {
                Text = "Laboratorio Clínico SALC\n\n" +
                       "• Análisis clínicos de alta precisión\n" +
                       "• Personal médico especializado\n" +
                       "• Tecnología de vanguardia\n" +
                       "• Resultados confiables y oportunos\n" +
                       "• Amplia gama de estudios disponibles\n" +
                       "• Certificaciones de calidad internacionales\n\n" +
                       "Comprometidos con la excelencia en el\n" +
                       "diagnóstico médico y la atención al paciente.\n\n" +
                       "Misión: Proporcionar resultados de laboratorio\n" +
                       "precisos y confiables para apoyar decisiones\n" +
                       "médicas informadas.",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(15, 25),
                Size = new Size(390, 180),
                BackColor = Color.Transparent
            };

            groupInfo.Controls.Add(lblInfoEmpresa);

            // Información de contacto
            groupContacto = new GroupBox
            {
                Text = " Información de Contacto ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(460, 220),
                Size = new Size(420, 220),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(70, 130, 180)
            };

            lblContacto = new Label
            {
                Text = "Dirección:\n" +
                       "   Av. Salud 123, Centro Médico\n" +
                       "   Ciudad Autónoma de Buenos Aires\n\n" +
                       "Teléfonos:\n" +
                       "   Central: (011) 4567-8900\n" +
                       "   Urgencias: (011) 4567-8901\n\n" +
                       "Contacto Digital:\n" +
                       "   info@laboratorio-salc.com\n" +
                       "   resultados@laboratorio-salc.com\n\n" +
                       "Horarios de Atención:\n" +
                       "   Lunes a Viernes: 07:00 - 18:00\n" +
                       "   Sábados: 08:00 - 13:00\n" +
                       "   Domingos: Solo urgencias",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(15, 25),
                Size = new Size(390, 180),
                BackColor = Color.Transparent
            };

            groupContacto.Controls.Add(lblContacto);

            // Panel de estadísticas básicas
            groupEstadisticas = new GroupBox
            {
                Text = " Panel de Estado del Sistema ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 460),
                Size = new Size(860, 100),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White
            };

            lblEstadisticas = new Label
            {
                Text = "Sistema Operativo    Base de Datos Conectada    Rendimiento Óptimo    Seguridad Activa\n\n" +
                       "Todos los servicios funcionando correctamente. Listo para gestionar análisis clínicos con total confiabilidad.",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 25),
                Size = new Size(820, 60),
                BackColor = Color.Transparent
            };

            groupEstadisticas.Controls.Add(lblEstadisticas);

            // Agregar todos los controles al formulario
            Controls.AddRange(new Control[] {
                panelBienvenida, groupInfo, groupContacto, groupEstadisticas
            });
        }
    }
}