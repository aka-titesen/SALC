using System;
using System.Drawing;
using System.Windows.Forms;

namespace SALC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeMainForm();
        }

        private void InitializeMainForm()
        {
            this.Text = "SALC - Sistema de Administración de Laboratorio Clínico";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Crear menú principal
            MenuStrip mainMenu = new MenuStrip();
            this.MainMenuStrip = mainMenu;

            // Menú Archivo
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("&Archivo");
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("&Salir", null, (s, e) => Application.Exit());
            fileMenu.DropDownItems.Add(exitMenuItem);

            // Menú Pacientes
            ToolStripMenuItem patientsMenu = new ToolStripMenuItem("&Pacientes");
            ToolStripMenuItem newPatientMenuItem = new ToolStripMenuItem("&Nuevo Paciente");
            ToolStripMenuItem searchPatientMenuItem = new ToolStripMenuItem("&Buscar Paciente");
            patientsMenu.DropDownItems.AddRange(new ToolStripItem[] { newPatientMenuItem, searchPatientMenuItem });

            // Menú Estudios
            ToolStripMenuItem studiesMenu = new ToolStripMenuItem("&Estudios");
            ToolStripMenuItem newStudyMenuItem = new ToolStripMenuItem("&Nueva Orden");
            ToolStripMenuItem pendingStudiesMenuItem = new ToolStripMenuItem("&Estudios Pendientes");
            studiesMenu.DropDownItems.AddRange(new ToolStripItem[] { newStudyMenuItem, pendingStudiesMenuItem });

            // Menú Resultados
            ToolStripMenuItem resultsMenu = new ToolStripMenuItem("&Resultados");
            ToolStripMenuItem loadResultsMenuItem = new ToolStripMenuItem("&Cargar Resultados");
            ToolStripMenuItem generateReportMenuItem = new ToolStripMenuItem("&Generar Informe");
            resultsMenu.DropDownItems.AddRange(new ToolStripItem[] { loadResultsMenuItem, generateReportMenuItem });

            // Agregar menús al menú principal
            mainMenu.Items.AddRange(new ToolStripItem[] { fileMenu, patientsMenu, studiesMenu, resultsMenu });

            // Agregar menú al formulario
            this.Controls.Add(mainMenu);

            // Panel principal
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Título de bienvenida
            Label welcomeLabel = new Label
            {
                Text = "Bienvenido al Sistema SALC",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(64, 64, 64),
                Size = new Size(400, 40),
                Location = new Point(20, 20)
            };

            // Información del usuario
            Label userInfoLabel = new Label
            {
                Text = "Usuario: " + (UserAuthentication.CurrentUser != null ? UserAuthentication.CurrentUser.DisplayName : "No autenticado"),
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(128, 128, 128),
                Size = new Size(400, 25),
                Location = new Point(20, 70)
            };

            // Agregar controles al panel principal
            mainPanel.Controls.AddRange(new Control[] { welcomeLabel, userInfoLabel });

            // Agregar panel principal al formulario
            this.Controls.Add(mainPanel);
        }
    }
}
