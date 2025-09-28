using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Presenters;
using SALC.Views.Interfaces;

namespace SALC
{
    public partial class MainDashboardForm : Form, IMainDashboardView
    {
        // Layout raíz
        private TableLayoutPanel mainLayout;

        // Header
        private Panel headerPanel;
        private Label headerTitleLabel;
        private Label userInfoLabel;
        private Button logoutButton;
        private FlowLayoutPanel rightHeaderFlow;

        // Ribbon
        private Panel ribbonPanel;
        private FlowLayoutPanel ribbonFlow;

        // Cards grid
        private FlowLayoutPanel cardsGrid;

        // Estado
        private IReadOnlyCollection<AppFeature> currentFeatures = Array.Empty<AppFeature>();

        // Eventos
        public event EventHandler LogoutRequested;
        public event EventHandler PatientsRequested;
        public event EventHandler StudiesRequested;
        public event EventHandler ResultsRequested;
        public event EventHandler ReportsRequested;
        public event EventHandler NotificationsRequested;
        public event EventHandler HistoryRequested;
        public event EventHandler AppointmentsRequested;
        public event EventHandler UserManagementRequested;
        public event EventHandler SystemConfigRequested;
        public event EventHandler BackupsRequested;
        public event EventHandler SecurityRequested;

        public MainDashboardForm()
        {
            InitializeComponent();
            InitializeLayout();

            var _ = new MainDashboardPresenter(this);
        }

        private void InitializeLayout()
        {
            Text = "SALC - Inicio";
            BackColor = Color.FromArgb(248, 249, 250);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Padding = new Padding(0);

            // Layout raíz: Header (auto), Ribbon (auto), Contenido (fill)
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = this.BackColor,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(mainLayout);

            BuildHeader();
            BuildRibbon();
            BuildCardsGrid();
        }

        private void BuildHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 56,
                BackColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            // Título (ocupa el espacio restante)
            headerTitleLabel = new Label
            {
                AutoSize = false,
                Text = "Panel Principal",
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 0, 0),
                Margin = new Padding(0)
            };

            // Panel derecho con flujo RTL para que el botón quede más a la derecha
            rightHeaderFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft,
                BackColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0),
                WrapContents = false
            };

            logoutButton = new Button
            {
                Text = "Cerrar sesión",
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 8, 8, 8),
                Padding = new Padding(10, 5, 10, 5)
            };
            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.Click += (s, e) =>
            {
                var hasSubs = LogoutRequested != null;
                LogoutRequested?.Invoke(this, EventArgs.Empty);
                if (!hasSubs)
                {
                    UserAuthentication.Logout();
                    Hide();
                    var login = new LoginForm();
                    login.Show();
                    login.FormClosed += (ss, ee) => Application.Exit();
                }
            };

            userInfoLabel = new Label
            {
                AutoSize = true,
                Text = "Usuario",
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(73, 80, 87),
                TextAlign = ContentAlignment.MiddleRight,
                Margin = new Padding(0, 12, 12, 12)
            };

            rightHeaderFlow.Controls.Add(logoutButton);
            rightHeaderFlow.Controls.Add(userInfoLabel);

            headerPanel.Controls.Add(rightHeaderFlow);
            headerPanel.Controls.Add(headerTitleLabel);

            // Añadir a fila 0
            mainLayout.Controls.Add(headerPanel, 0, 0);
        }

        private void BuildRibbon()
        {
            ribbonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 56,
                BackColor = Color.FromArgb(241, 243, 245),
                Padding = new Padding(8, 8, 8, 8),
                Margin = new Padding(0)
            };
            ribbonFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true, // saltar a nueva línea cuando no entra
                AutoScroll = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            ribbonPanel.Controls.Add(ribbonFlow);

            // Añadir a fila 1
            mainLayout.Controls.Add(ribbonPanel, 0, 1);
        }

        private void BuildCardsGrid()
        {
            cardsGrid = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(16),
                AutoScroll = true,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = new Padding(0)
            };
            // Añadir a fila 2
            mainLayout.Controls.Add(cardsGrid, 0, 2);
        }

        // IMainDashboardView
        public void SetHeaderTitle(string title)
        {
            headerTitleLabel.Text = string.IsNullOrWhiteSpace(title) ? "Panel Principal" : title;
        }

        public void SetUserInfo(string displayName, string role)
        {
            string name = string.IsNullOrWhiteSpace(displayName) ? "Usuario" : displayName;
            string r = string.IsNullOrWhiteSpace(role) ? "-" : role;
            userInfoLabel.Text = $"{name} | Rol: {r}";
        }

        public void SetAvailableFeatures(IReadOnlyCollection<AppFeature> features)
        {
            currentFeatures = features ?? Array.Empty<AppFeature>();
            BuildRibbonButtons(currentFeatures);
            BuildFeatureCards(currentFeatures);
        }

        public void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BuildRibbonButtons(IReadOnlyCollection<AppFeature> features)
        {
            ribbonFlow.SuspendLayout();
            ribbonFlow.Controls.Clear();

            foreach (var feature in features)
            {
                var btn = CreateRibbonButton(feature);
                ribbonFlow.Controls.Add(btn);
            }
            ribbonFlow.ResumeLayout();
        }

        private Button CreateRibbonButton(AppFeature feature)
        {
            var btn = new Button
            {
                Height = 32,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(4),
                Padding = new Padding(10, 4, 10, 4),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold),
                Text = GetFeatureTitle(feature)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => RaiseFeatureEvent(feature);
            return btn;
        }

        private void BuildFeatureCards(IReadOnlyCollection<AppFeature> features)
        {
            cardsGrid.SuspendLayout();
            cardsGrid.Controls.Clear();

            foreach (var feature in features)
            {
                var card = CreateCard(feature);
                cardsGrid.Controls.Add(card);
            }

            cardsGrid.ResumeLayout();
        }

        private Control CreateCard(AppFeature feature)
        {
            var colors = GetFeatureColor(feature);

            var panel = new Panel
            {
                Width = 320,
                Height = 120,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(8)
            };

            var titleLbl = new Label
            {
                Text = GetFeatureTitle(feature),
                Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold),
                ForeColor = colors.title,
                Location = new Point(12, 12),
                Size = new Size(296, 22)
            };

            var descLbl = new Label
            {
                Text = GetFeatureDescription(feature),
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 58, 64),
                Location = new Point(12, 38),
                Size = new Size(296, 36)
            };

            var btn = new Button
            {
                Text = "Abrir",
                Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold),
                BackColor = colors.button,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(12, 82),
                Size = new Size(296, 26)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => RaiseFeatureEvent(feature);

            panel.Controls.Add(titleLbl);
            panel.Controls.Add(descLbl);
            panel.Controls.Add(btn);
            return panel;
        }

        private string GetFeatureTitle(AppFeature f) => f switch
        {
            AppFeature.Dashboard => "Dashboard",
            AppFeature.GestionPacientes => "Pacientes",
            AppFeature.GestionEstudios => "Análisis",
            AppFeature.CargaResultados => "Resultados",
            AppFeature.GenerarInformes => "Informes",
            AppFeature.Notificaciones => "Notificaciones",
            AppFeature.HistorialOrdenes => "Historial",
            AppFeature.Turnos => "Turnos",
            AppFeature.GestionUsuarios => "Usuarios",
            AppFeature.ConfigSistema => "Configuración",
            AppFeature.CopiasSeguridad => "Backups",
            AppFeature.Seguridad => "Seguridad",
            _ => f.ToString()
        };

        private string GetFeatureDescription(AppFeature f) => f switch
        {
            AppFeature.GestionPacientes => "Administrar la información de pacientes (CRUD).",
            AppFeature.GestionEstudios => "Crear y gestionar órdenes de análisis.",
            AppFeature.CargaResultados => "Cargar y validar resultados de estudios.",
            AppFeature.GenerarInformes => "Generar y consultar informes PDF.",
            AppFeature.Notificaciones => "Enviar notificaciones a pacientes.",
            AppFeature.HistorialOrdenes => "Consultar historial de órdenes.",
            AppFeature.Turnos => "Gestionar agenda y turnos.",
            AppFeature.GestionUsuarios => "Administrar usuarios y roles.",
            AppFeature.ConfigSistema => "Configurar parámetros del sistema.",
            AppFeature.CopiasSeguridad => "Configurar y ejecutar backups.",
            AppFeature.Seguridad => "Auditar accesos y permisos.",
            _ => string.Empty
        };

        private (Color title, Color button) GetFeatureColor(AppFeature f)
        {
            return f switch
            {
                AppFeature.GestionPacientes => (Color.FromArgb(23, 162, 184), Color.FromArgb(23, 162, 184)),
                AppFeature.GestionEstudios => (Color.FromArgb(0, 120, 215), Color.FromArgb(0, 120, 215)),
                AppFeature.CargaResultados => (Color.FromArgb(255, 193, 7), Color.FromArgb(255, 193, 7)),
                AppFeature.GenerarInformes => (Color.FromArgb(111, 66, 193), Color.FromArgb(111, 66, 193)),
                AppFeature.Notificaciones => (Color.FromArgb(253, 126, 20), Color.FromArgb(253, 126, 20)),
                AppFeature.HistorialOrdenes => (Color.FromArgb(40, 167, 69), Color.FromArgb(40, 167, 69)),
                AppFeature.Turnos => (Color.FromArgb(102, 16, 242), Color.FromArgb(102, 16, 242)),
                AppFeature.GestionUsuarios => (Color.FromArgb(32, 201, 151), Color.FromArgb(32, 201, 151)),
                AppFeature.ConfigSistema => (Color.FromArgb(108, 117, 125), Color.FromArgb(108, 117, 125)),
                AppFeature.CopiasSeguridad => (Color.FromArgb(52, 58, 64), Color.FromArgb(52, 58, 64)),
                AppFeature.Seguridad => (Color.FromArgb(220, 53, 69), Color.FromArgb(220, 53, 69)),
                _ => (Color.FromArgb(0, 120, 215), Color.FromArgb(0, 120, 215))
            };
        }

        private void RaiseFeatureEvent(AppFeature feature)
        {
            switch (feature)
            {
                case AppFeature.GestionPacientes: PatientsRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionEstudios: StudiesRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.CargaResultados: ResultsRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GenerarInformes: ReportsRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.Notificaciones: NotificationsRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.HistorialOrdenes: HistoryRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.Turnos: AppointmentsRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionUsuarios: UserManagementRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.ConfigSistema: SystemConfigRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.CopiasSeguridad: BackupsRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.Seguridad: SecurityRequested?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.Dashboard: break;
            }
        }
    }
}