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

            // T�tulo de bienvenida
            lblBienvenida = new Label
            {
                Text = "Bienvenido a SALC",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(160, 40),
                Size = new Size(450, 45),
                BackColor = Color.Transparent
            };

            // Descripci�n del sistema
            lblDescripcion = new Label
            {
                Text = "Sistema de Administraci�n de Laboratorio Cl�nico",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(160, 90),
                Size = new Size(500, 25),
                BackColor = Color.Transparent
            };

            // Versi�n
            lblVersion = new Label
            {
                Text = "Versi�n 1.0 - Desarrollado para la excelencia en diagn�stico m�dico",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(160, 120),
                Size = new Size(500, 25),
                BackColor = Color.Transparent
            };

            panelBienvenida.Controls.AddRange(new Control[] {
                picLogo, lblBienvenida, lblDescripcion, lblVersion
            });

            // Informaci�n de la empresa/laboratorio
            groupInfo = new GroupBox
            {
                Text = " Informaci�n del Laboratorio ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(20, 220),
                Size = new Size(420, 220),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(70, 130, 180)
            };

            lblInfoEmpresa = new Label
            {
                Text = "Laboratorio Cl�nico SALC\n\n" +
                       "� An�lisis cl�nicos de alta precisi�n\n" +
                       "� Personal m�dico especializado\n" +
                       "� Tecnolog�a de vanguardia\n" +
                       "� Resultados confiables y oportunos\n" +
                       "� Amplia gama de estudios disponibles\n" +
                       "� Certificaciones de calidad internacionales\n\n" +
                       "Comprometidos con la excelencia en el\n" +
                       "diagn�stico m�dico y la atenci�n al paciente.\n\n" +
                       "SISTEMA COMPLETAMENTE OPERATIVO\n" +
                       "Todas las funcionalidades implementadas\n" +
                       "seg�n ERS-SALC v2.7",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(15, 25),
                Size = new Size(390, 180),
                BackColor = Color.Transparent
            };

            groupInfo.Controls.Add(lblInfoEmpresa);

            // Informaci�n de contacto
            groupContacto = new GroupBox
            {
                Text = " Estado de Funcionalidades ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(460, 220),
                Size = new Size(420, 220),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(70, 130, 180)
            };

            lblContacto = new Label
            {
                Text = "FUNCIONALIDADES IMPLEMENTADAS:\n\n" +
                       "? ADMINISTRADOR\n" +
                       "   � ABM Usuarios (M�dicos, Asistentes)\n" +
                       "   � ABM Pacientes completo\n" +
                       "   � Gesti�n de Cat�logos\n" +
                       "   � Copias de seguridad\n\n" +
                       "? ASISTENTE\n" +
                       "   � Consulta de pacientes\n" +
                       "   � Historial de an�lisis\n" +
                       "   � Generaci�n de informes\n\n" +
                       "? EN DESARROLLO\n" +
                       "   � Panel M�dico (pr�ximamente)\n" +
                       "   � Generaci�n PDF avanzada\n" +
                       "   � Env�o autom�tico de informes",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(15, 25),
                Size = new Size(390, 180),
                BackColor = Color.Transparent
            };

            groupContacto.Controls.Add(lblContacto);

            // Panel de estad�sticas b�sicas
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
                Text = "Sistema Operativo    Base de Datos Conectada    Rendimiento �ptimo    Seguridad Activa\n\n" +
                       "? MVP Arquitectura 3 Capas    ? Autenticaci�n BCrypt    ? Consultas Parametrizadas    ? Baja L�gica Implementada",
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