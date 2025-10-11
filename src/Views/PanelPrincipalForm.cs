using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Presenters;
using SALC.Views.Interfaces;

namespace SALC
{
    public partial class PanelPrincipalForm : Form, IVistaPanelPrincipal
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

        // Eventos (en español)
        public event EventHandler CierreSesionSolicitado;
        public event EventHandler PacientesSolicitado;
        public event EventHandler EstudiosSolicitado;
        public event EventHandler ResultadosSolicitado;
        public event EventHandler RecepcionMuestrasSolicitado;
        public event EventHandler ReportesSolicitado;
        public event EventHandler NotificacionesSolicitado;
        public event EventHandler HistorialSolicitado;

        // ADMINISTRACIÓN DE USUARIOS
        public event EventHandler GestionUsuariosSolicitada;
        public event EventHandler PacientesAdminSolicitado;
        public event EventHandler DoctoresExternosSolicitado;

        // ADMINISTRACIÓN DE CATÁLOGOS
        public event EventHandler TiposAnalisisSolicitado;
        public event EventHandler MetricasSolicitado;
        public event EventHandler ObrasSocialesSolicitado;
        public event EventHandler EstadosSolicitado;
        public event EventHandler RolesSolicitado;

        // CONFIGURACIÓN Y SISTEMA
        public event EventHandler CopiasSeguridadSolicitado;

        public PanelPrincipalForm()
        {
            InitializeComponent();
            InitializeLayout();

            // Inicializar presentador del panel principal
            var presentador = new PanelPrincipalPresenter(this);

            // Conectar eventos administrativos si es admin
            if (UserAuthentication.CurrentUser?.Rol?.ToLower() == "admin")
            {
                ConectarEventosAdministrativos();
            }
        }

        private void ConectarEventosAdministrativos()
        {
            this.GestionUsuariosSolicitada += OnGestionUsuariosSolicitada;
            this.PacientesAdminSolicitado += OnPacientesAdminSolicitado;
            this.DoctoresExternosSolicitado += OnDoctoresExternosSolicitado;
            this.TiposAnalisisSolicitado += OnCatalogoSolicitado;
            this.MetricasSolicitado += OnCatalogoSolicitado;
            this.ObrasSocialesSolicitado += OnCatalogoSolicitado;
            this.EstadosSolicitado += OnCatalogoSolicitado;
            this.RolesSolicitado += OnCatalogoSolicitado;
            this.CopiasSeguridadSolicitado += OnCopiasSeguridadSolicitado;
        }

        #region Manejadores Admin

        private void OnGestionUsuariosSolicitada(object sender, EventArgs e)
        {
            try
            {
                var userForm = new GestionUsuariosForm();
                userForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", $"Error al abrir gestión de usuarios: {ex.Message}");
            }
        }

        private void OnPacientesAdminSolicitado(object sender, EventArgs e)
        {
            try
            {
                var patientsForm = new PacientesForm();
                patientsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", $"Error al abrir gestión de pacientes: {ex.Message}");
            }
        }

        private void OnDoctoresExternosSolicitado(object sender, EventArgs e)
        {
            MostrarMensaje("Funcionalidad en Desarrollo",
                "La gestión de doctores externos estará disponible en la próxima versión.");
        }

        private void OnCatalogoSolicitado(object sender, EventArgs e)
        {
            try
            {
                var catalogForm = new AdminCatalogFormStub();
                catalogForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", $"Error al abrir gestión de catálogos: {ex.Message}");
            }
        }

        private void OnCopiasSeguridadSolicitado(object sender, EventArgs e)
        {
            try
            {
                var backupForm = new BackupManagementFormStub();
                backupForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarMensaje("Error", $"Error al abrir gestión de backups: {ex.Message}");
            }
        }

        #endregion

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
                var hasSubs = CierreSesionSolicitado != null;
                CierreSesionSolicitado?.Invoke(this, EventArgs.Empty);
                if (!hasSubs)
                {
                    UserAuthentication.Logout();
                    Hide();
                    var login = new InicioSesionForm();
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
                WrapContents = true,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            ribbonPanel.Controls.Add(ribbonFlow);

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
            mainLayout.Controls.Add(cardsGrid, 0, 2);
        }

        // IVistaPanelPrincipal
        public void EstablecerTituloEncabezado(string titulo)
        {
            headerTitleLabel.Text = string.IsNullOrWhiteSpace(titulo) ? "Panel Principal" : titulo;
        }

        public void EstablecerInformacionUsuario(string nombreCompleto, string rol)
        {
            string name = string.IsNullOrWhiteSpace(nombreCompleto) ? "Usuario" : nombreCompleto;
            string r = string.IsNullOrWhiteSpace(rol) ? "-" : rol;
            userInfoLabel.Text = $"{name} | Rol: {r}";
        }

        public void EstablecerFuncionalidadesDisponibles(IReadOnlyCollection<AppFeature> funcionalidades)
        {
            currentFeatures = funcionalidades ?? Array.Empty<AppFeature>();
            ConstruirBotonesRibbon(currentFeatures);
            ConstruirTarjetasFunciones(currentFeatures);
        }

        public void MostrarMensaje(string titulo, string mensaje)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ActualizarEstadoUsuario(string estado)
        {
            // Hook opcional para estado
        }

        public void HabilitarOpcion(AppFeature funcionalidad, bool habilitada)
        {
            // Hook opcional para habilitar/deshabilitar botones por feature
        }

        private void ConstruirBotonesRibbon(IReadOnlyCollection<AppFeature> features)
        {
            ribbonFlow.SuspendLayout();
            ribbonFlow.Controls.Clear();

            foreach (var feature in features)
            {
                var btn = CrearBotonRibbon(feature);
                ribbonFlow.Controls.Add(btn);
            }
            ribbonFlow.ResumeLayout();
        }

        private Button CrearBotonRibbon(AppFeature feature)
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
                Text = ObtenerTituloFeature(feature)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => ElevarEventoFeature(feature);
            return btn;
        }

        private void ConstruirTarjetasFunciones(IReadOnlyCollection<AppFeature> features)
        {
            cardsGrid.SuspendLayout();
            cardsGrid.Controls.Clear();

            foreach (var feature in features)
            {
                var card = CrearTarjeta(feature);
                cardsGrid.Controls.Add(card);
            }

            cardsGrid.ResumeLayout();
        }

        private Control CrearTarjeta(AppFeature feature)
        {
            var colors = ObtenerColorFeature(feature);

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
                Text = ObtenerTituloFeature(feature),
                Font = new Font("Microsoft Sans Serif", 11, FontStyle.Bold),
                ForeColor = colors.title,
                Location = new Point(12, 12),
                Size = new Size(296, 22)
            };

            var descLbl = new Label
            {
                Text = ObtenerDescripcionFeature(feature),
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
            btn.Click += (s, e) => ElevarEventoFeature(feature);

            panel.Controls.Add(titleLbl);
            panel.Controls.Add(descLbl);
            panel.Controls.Add(btn);
            return panel;
        }

        private string ObtenerTituloFeature(AppFeature f) => f switch
        {
            AppFeature.GestionPacientes => "Pacientes",
            AppFeature.GestionEstudios => "Análisis",
            AppFeature.CargaResultados => "Resultados",
            AppFeature.RecepcionMuestras => "Recepción de Muestras",
            AppFeature.GenerarInformes => "Informes",
            AppFeature.Notificaciones => "Notificaciones",
            AppFeature.HistorialOrdenes => "Historial",

            // ADMINISTRACIÓN DE USUARIOS
            AppFeature.GestionUsuarios => "Usuarios del Sistema",
            AppFeature.GestionPacientesAdmin => "Gestión de Pacientes",
            AppFeature.GestionDoctoresExternos => "Doctores Externos",

            // ADMINISTRACIÓN DE CATÁLOGOS
            AppFeature.GestionTiposAnalisis => "Tipos de Análisis",
            AppFeature.GestionMetricas => "Métricas",
            AppFeature.GestionObrasSociales => "Obras Sociales",
            AppFeature.GestionEstados => "Estados",
            AppFeature.GestionRoles => "Roles",

            // CONFIGURACIÓN Y SISTEMA
            AppFeature.CopiasSeguridad => "Backups",

            _ => f.ToString()
        };

        private string ObtenerDescripcionFeature(AppFeature f) => f switch
        {
            AppFeature.GestionPacientes => "Administrar la información de pacientes (CRUD).",
            AppFeature.GestionEstudios => "Crear y gestionar órdenes de análisis.",
            AppFeature.CargaResultados => "Cargar y validar resultados de estudios.",
            AppFeature.RecepcionMuestras => "Recepcionar muestras de análisis (RF-17).",
            AppFeature.GenerarInformes => "Generar y consultar informes PDF.",
            AppFeature.Notificaciones => "Enviar notificaciones a pacientes.",
            AppFeature.HistorialOrdenes => "Consultar historial de órdenes.",

            // ADMINISTRACIÓN DE USUARIOS
            AppFeature.GestionUsuarios => "ABM usuarios internos: administradores, clínicos, asistentes.",
            AppFeature.GestionPacientesAdmin => "ABM completo de pacientes con vista administrativa.",
            AppFeature.GestionDoctoresExternos => "ABM doctores externos que solicitan análisis.",

            // ADMINISTRACIÓN DE CATÁLOGOS
            AppFeature.GestionTiposAnalisis => "ABM tipos de análisis disponibles en el laboratorio.",
            AppFeature.GestionMetricas => "ABM métricas y parámetros de análisis.",
            AppFeature.GestionObrasSociales => "ABM obras sociales y prepagas.",
            AppFeature.GestionEstados => "ABM estados de análisis y usuarios.",
            AppFeature.GestionRoles => "ABM roles del sistema.",

            // CONFIGURACIÓN Y SISTEMA
            AppFeature.CopiasSeguridad => "Configurar y ejecutar backups.",

            _ => string.Empty
        };

        private (Color title, Color button) ObtenerColorFeature(AppFeature f)
        {
            return f switch
            {
                AppFeature.GestionPacientes => (Color.FromArgb(23, 162, 184), Color.FromArgb(23, 162, 184)),
                AppFeature.GestionEstudios => (Color.FromArgb(0, 120, 215), Color.FromArgb(0, 120, 215)),
                AppFeature.CargaResultados => (Color.FromArgb(255, 193, 7), Color.FromArgb(255, 193, 7)),
                AppFeature.RecepcionMuestras => (Color.FromArgb(32, 201, 151), Color.FromArgb(32, 201, 151)),
                AppFeature.GenerarInformes => (Color.FromArgb(111, 66, 193), Color.FromArgb(111, 66, 193)),
                AppFeature.Notificaciones => (Color.FromArgb(253, 126, 20), Color.FromArgb(253, 126, 20)),
                AppFeature.HistorialOrdenes => (Color.FromArgb(40, 167, 69), Color.FromArgb(40, 167, 69)),

                // ADMINISTRACIÓN DE USUARIOS
                AppFeature.GestionUsuarios => (Color.FromArgb(32, 201, 151), Color.FromArgb(32, 201, 151)),
                AppFeature.GestionPacientesAdmin => (Color.FromArgb(0, 123, 255), Color.FromArgb(0, 123, 255)),
                AppFeature.GestionDoctoresExternos => (Color.FromArgb(23, 162, 184), Color.FromArgb(23, 162, 184)),

                // ADMINISTRACIÓN DE CATÁLOGOS
                AppFeature.GestionTiposAnalisis => (Color.FromArgb(111, 66, 193), Color.FromArgb(111, 66, 193)),
                AppFeature.GestionMetricas => (Color.FromArgb(102, 16, 242), Color.FromArgb(102, 16, 242)),
                AppFeature.GestionObrasSociales => (Color.FromArgb(156, 39, 176), Color.FromArgb(156, 39, 176)),
                AppFeature.GestionEstados => (Color.FromArgb(123, 31, 162), Color.FromArgb(123, 31, 162)),
                AppFeature.GestionRoles => (Color.FromArgb(142, 36, 170), Color.FromArgb(142, 36, 170)),

                // CONFIGURACIÓN Y SISTEMA
                AppFeature.CopiasSeguridad => (Color.FromArgb(52, 58, 64), Color.FromArgb(52, 58, 64)),

                _ => (Color.FromArgb(0, 120, 215), Color.FromArgb(0, 120, 215))
            };
        }

        private void ElevarEventoFeature(AppFeature feature)
        {
            switch (feature)
            {
                case AppFeature.GestionPacientes: PacientesSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionEstudios: EstudiosSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.CargaResultados: ResultadosSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.RecepcionMuestras: RecepcionMuestrasSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GenerarInformes: ReportesSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.Notificaciones: NotificacionesSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.HistorialOrdenes: HistorialSolicitado?.Invoke(this, EventArgs.Empty); break;

                // ADMINISTRACIÓN DE USUARIOS
                case AppFeature.GestionUsuarios: GestionUsuariosSolicitada?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionPacientesAdmin: PacientesAdminSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionDoctoresExternos: DoctoresExternosSolicitado?.Invoke(this, EventArgs.Empty); break;

                // ADMINISTRACIÓN DE CATÁLOGOS
                case AppFeature.GestionTiposAnalisis: TiposAnalisisSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionMetricas: MetricasSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionObrasSociales: ObrasSocialesSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionEstados: EstadosSolicitado?.Invoke(this, EventArgs.Empty); break;
                case AppFeature.GestionRoles: RolesSolicitado?.Invoke(this, EventArgs.Empty); break;

                case AppFeature.CopiasSeguridad: CopiasSeguridadSolicitado?.Invoke(this, EventArgs.Empty); break;
            }
        }
    }
}
