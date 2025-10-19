using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Views.Compartidos;
using SALC.Domain;
using System.Linq;

namespace SALC.Views
{
    public class FrmPrincipalConTabs : Form
    {
        private TabControl tabPrincipal;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblUsuario;
        private ToolStripStatusLabel lblRol;
        private ToolStripStatusLabel lblFecha;
        private ToolStripStatusLabel lblEstado;
        
        private Usuario usuarioActual;

        public FrmPrincipalConTabs(Usuario usuario)
        {
            usuarioActual = usuario;
            InitializeComponent();
            ConfigurarPestanasPorRol();
        }

        private void InitializeComponent()
        {
            Text = "SALC - Sistema de Administración de Laboratorio Clínico";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Icon = CargarIcono();

            // Agregar menú principal
            var menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            // Menú Archivo
            var menuArchivo = new ToolStripMenuItem("Archivo")
            {
                ForeColor = Color.White
            };

            var menuCerrarSesion = new ToolStripMenuItem("Cerrar Sesión")
            {
                ForeColor = Color.Black,
                ShortcutKeys = Keys.Control | Keys.Q
            };
            menuCerrarSesion.Click += (s, e) => CerrarSesion();

            var menuSalir = new ToolStripMenuItem("Salir")
            {
                ForeColor = Color.Black,
                ShortcutKeys = Keys.Alt | Keys.F4
            };
            menuSalir.Click += (s, e) => Close();

            menuArchivo.DropDownItems.AddRange(new ToolStripItem[] {
                menuCerrarSesion,
                new ToolStripSeparator(),
                menuSalir
            });

            // Menú Ayuda
            var menuAyuda = new ToolStripMenuItem("Ayuda")
            {
                ForeColor = Color.White
            };

            var menuAcercaDe = new ToolStripMenuItem("Acerca de SALC")
            {
                ForeColor = Color.Black
            };
            menuAcercaDe.Click += (s, e) => MostrarAcercaDe();

            menuAyuda.DropDownItems.Add(menuAcercaDe);

            menuStrip.Items.AddRange(new ToolStripItem[] { menuArchivo, menuAyuda });

            // TabControl principal - ajustado para no solaparse con el menú
            tabPrincipal = new TabControl
            {
                Location = new Point(0, menuStrip.Height),
                Size = new Size(ClientSize.Width, ClientSize.Height - menuStrip.Height - 25), // 25 para la barra de estado
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Padding = new Point(20, 8),
                ItemSize = new Size(120, 35),
                SizeMode = TabSizeMode.Fixed
            };

            // Barra de estado
            statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White
            };

            lblUsuario = new ToolStripStatusLabel
            {
                Text = $"Usuario: {usuarioActual?.Nombre} {usuarioActual?.Apellido}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            lblRol = new ToolStripStatusLabel
            {
                Text = $"Rol: {ObtenerNombreRol(usuarioActual?.IdRol ?? 0)}",
                ForeColor = Color.White,
                Margin = new Padding(20, 0, 0, 0)
            };

            lblFecha = new ToolStripStatusLabel
            {
                Text = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}",
                ForeColor = Color.White,
                Margin = new Padding(20, 0, 0, 0)
            };

            lblEstado = new ToolStripStatusLabel
            {
                Text = "Sistema Operativo",
                ForeColor = Color.White,
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight
            };

            // Botón de cerrar sesión en la barra de estado
            var btnCerrarSesion = new ToolStripDropDownButton
            {
                Text = "Cerrar Sesión",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            
            var itemCerrarSesion = new ToolStripMenuItem("Cerrar Sesión");
            itemCerrarSesion.Click += (s, e) => CerrarSesion();
            
            var itemSalir = new ToolStripMenuItem("Salir del Sistema");
            itemSalir.Click += (s, e) => Close();

            btnCerrarSesion.DropDownItems.AddRange(new ToolStripItem[] {
                itemCerrarSesion,
                new ToolStripSeparator(),
                itemSalir
            });

            statusStrip.Items.AddRange(new ToolStripItem[] {
                lblUsuario, lblRol, lblFecha, lblEstado, btnCerrarSesion
            });

            // Agregar controles en el orden correcto
            Controls.AddRange(new Control[] { tabPrincipal, menuStrip, statusStrip });

            // Ajustar el TabControl cuando la ventana cambie de tamaño
            this.Resize += (s, e) => {
                tabPrincipal.Size = new Size(ClientSize.Width, ClientSize.Height - menuStrip.Height - statusStrip.Height);
            };

            // Timer para actualizar fecha
            var timer = new Timer { Interval = 60000 }; // 1 minuto
            timer.Tick += (s, e) => lblFecha.Text = $"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";
            timer.Start();
        }

        private void ConfigurarPestanasPorRol()
        {
            // Pestaña de Inicio (común para todos)
            AgregarPestanaInicio();

            // Pestañas específicas por rol
            switch (usuarioActual?.IdRol)
            {
                case 1: // Administrador
                    AgregarPestanasAdministrador();
                    break;
                case 2: // Médico
                    AgregarPestanasMedico();
                    break;
                case 3: // Asistente
                    AgregarPestanasAsistente();
                    break;
            }
        }

        private void AgregarPestanaInicio()
        {
            var tabInicio = new TabPage("Inicio")
            {
                BackColor = Color.FromArgb(248, 250, 252),
                UseVisualStyleBackColor = false
            };

            var frmInicio = new FrmInicio
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };

            tabInicio.Controls.Add(frmInicio);
            frmInicio.Show();

            tabPrincipal.TabPages.Add(tabInicio);
        }

        private void AgregarPestanasAdministrador()
        {
            // Pestaña Usuarios - Solo gestión de usuarios
            var tabUsuarios = new TabPage("Usuarios")
            {
                BackColor = Color.White
            };

            try
            {
                // Crear una instancia del panel completo para obtener solo la funcionalidad de usuarios
                var frmPanelAdmin = new Views.PanelAdministrador.FrmPanelAdministrador
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                // Crear el presenter para el panel de administrador
                var presenter = new SALC.Presenters.PanelAdministradorPresenter(frmPanelAdmin);
                frmPanelAdmin.Tag = presenter;

                // Seleccionar la pestaña de usuarios en el panel
                var tabControl = frmPanelAdmin.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControl != null && tabControl.TabPages.Count > 0)
                {
                    tabControl.SelectedIndex = 0; // Pestaña usuarios
                    // Ocultar las otras pestañas para mostrar solo usuarios
                    for (int i = tabControl.TabPages.Count - 1; i > 0; i--)
                    {
                        tabControl.TabPages.RemoveAt(i);
                    }
                }

                tabUsuarios.Controls.Add(frmPanelAdmin);
                frmPanelAdmin.Show();
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Error al cargar panel de usuarios:\n{ex.Message}",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tabUsuarios.Controls.Add(lblError);
            }

            tabPrincipal.TabPages.Add(tabUsuarios);

            // Pestaña Pacientes - Solo gestión de pacientes
            var tabPacientes = new TabPage("Pacientes")
            {
                BackColor = Color.White
            };
            
            try
            {
                var frmPanelPacientes = new Views.PanelAdministrador.FrmPanelAdministrador
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                var presenterPacientes = new SALC.Presenters.PanelAdministradorPresenter(frmPanelPacientes);
                frmPanelPacientes.Tag = presenterPacientes;

                // Seleccionar la pestaña de pacientes en el panel
                var tabControlPacientes = frmPanelPacientes.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlPacientes != null && tabControlPacientes.TabPages.Count > 1)
                {
                    tabControlPacientes.SelectedIndex = 1; // Pestaña pacientes
                    // Remover todas las pestañas excepto pacientes
                    var pacientesTab = tabControlPacientes.TabPages[1];
                    tabControlPacientes.TabPages.Clear();
                    tabControlPacientes.TabPages.Add(pacientesTab);
                }

                tabPacientes.Controls.Add(frmPanelPacientes);
                frmPanelPacientes.Show();
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Error al cargar panel de pacientes:\n{ex.Message}",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tabPacientes.Controls.Add(lblError);
            }

            tabPrincipal.TabPages.Add(tabPacientes);

            // Pestaña Catálogos - Solo gestión de catálogos
            var tabCatalogos = new TabPage("Catálogos")
            {
                BackColor = Color.White
            };
            
            try
            {
                var frmPanelCatalogos = new Views.PanelAdministrador.FrmPanelAdministrador
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                var presenterCatalogos = new SALC.Presenters.PanelAdministradorPresenter(frmPanelCatalogos);
                frmPanelCatalogos.Tag = presenterCatalogos;

                // Seleccionar la pestaña de catálogos en el panel
                var tabControlCatalogos = frmPanelCatalogos.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlCatalogos != null && tabControlCatalogos.TabPages.Count > 2)
                {
                    tabControlCatalogos.SelectedIndex = 2; // Pestaña catálogos
                    // Remover todas las pestañas excepto catálogos
                    var catalogosTab = tabControlCatalogos.TabPages[2];
                    tabControlCatalogos.TabPages.Clear();
                    tabControlCatalogos.TabPages.Add(catalogosTab);
                }

                tabCatalogos.Controls.Add(frmPanelCatalogos);
                frmPanelCatalogos.Show();
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Error al cargar panel de catálogos:\n{ex.Message}",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tabCatalogos.Controls.Add(lblError);
            }

            tabPrincipal.TabPages.Add(tabCatalogos);

            // Pestaña Backups - Solo gestión de backups
            var tabBackups = new TabPage("Backups")
            {
                BackColor = Color.White
            };
            
            try
            {
                var frmPanelBackups = new Views.PanelAdministrador.FrmPanelAdministrador
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                var presenterBackups = new SALC.Presenters.PanelAdministradorPresenter(frmPanelBackups);
                frmPanelBackups.Tag = presenterBackups;

                // Seleccionar la pestaña de backups en el panel
                var tabControlBackups = frmPanelBackups.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlBackups != null && tabControlBackups.TabPages.Count > 3)
                {
                    tabControlBackups.SelectedIndex = 3; // Pestaña backups
                    // Remover todas las pestañas excepto backups
                    var backupsTab = tabControlBackups.TabPages[3];
                    tabControlBackups.TabPages.Clear();
                    tabControlBackups.TabPages.Add(backupsTab);
                }

                tabBackups.Controls.Add(frmPanelBackups);
                frmPanelBackups.Show();
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Error al cargar panel de backups:\n{ex.Message}",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tabBackups.Controls.Add(lblError);
            }

            tabPrincipal.TabPages.Add(tabBackups);
        }

        private void AgregarPestanasMedico()
        {
            // Pestaña Crear Análisis
            var tabCrear = new TabPage("Crear Análisis")
            {
                BackColor = Color.White
            };
            
            var lblCrear = new Label
            {
                Text = "Panel de Creación de Análisis\n\nFuncionalidades:\n• Crear nuevos análisis para pacientes\n• Seleccionar tipos de estudios\n• Asignar análisis a pacientes\n• Configurar parámetros iniciales",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabCrear.Controls.Add(lblCrear);
            tabPrincipal.TabPages.Add(tabCrear);

            // Pestaña Cargar Resultados
            var tabResultados = new TabPage("Resultados")
            {
                BackColor = Color.White
            };
            
            var lblResultados = new Label
            {
                Text = "Panel de Carga de Resultados\n\nFuncionalidades:\n• Cargar valores de métricas\n• Ingresar resultados numéricos\n• Añadir observaciones\n• Modificar análisis pendientes",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabResultados.Controls.Add(lblResultados);
            tabPrincipal.TabPages.Add(tabResultados);

            // Pestaña Firmar
            var tabFirmar = new TabPage("Validar")
            {
                BackColor = Color.White
            };
            
            var lblFirmar = new Label
            {
                Text = "Panel de Validación de Análisis\n\nFuncionalidades:\n• Revisar análisis completados\n• Validar resultados cargados\n• Firmar digitalmente\n• Cambiar estado a 'Verificado'",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabFirmar.Controls.Add(lblFirmar);
            tabPrincipal.TabPages.Add(tabFirmar);

            // Pestaña Informes
            var tabInformes = new TabPage("Informes")
            {
                BackColor = Color.White
            };
            
            var lblInformes = new Label
            {
                Text = "Panel de Generación de Informes\n\nFuncionalidades:\n• Generar informes PDF\n• Enviar resultados a pacientes\n• Consultar análisis propios\n• Gestionar reportes médicos",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabInformes.Controls.Add(lblInformes);
            tabPrincipal.TabPages.Add(tabInformes);
        }

        private void AgregarPestanasAsistente()
        {
            // Pestaña Panel del Asistente (reemplaza la ventana actual)
            var tabAsistente = new TabPage("Consultar Pacientes")
            {
                BackColor = Color.White
            };

            try
            {
                var frmAsistente = new PanelAsistente.FrmPanelAsistente
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                // Crear el presenter para el asistente
                var presenter = new SALC.Presenters.PanelAsistentePresenter(frmAsistente);
                frmAsistente.Tag = presenter;

                tabAsistente.Controls.Add(frmAsistente);
                frmAsistente.Show();

                // Inicializar la vista del asistente
                presenter.InicializarVista();
            }
            catch (Exception ex)
            {
                // Si hay error con el panel del asistente, mostrar mensaje
                var lblError = new Label
                {
                    Text = $"Error al cargar panel del asistente:\n{ex.Message}\n\nFuncionalidad en desarrollo.",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tabAsistente.Controls.Add(lblError);
            }

            tabPrincipal.TabPages.Add(tabAsistente);

            // Pestaña Informes Verificados
            var tabInformesVerificados = new TabPage("Informes")
            {
                BackColor = Color.White
            };
            
            var lblInformesVerificados = new Label
            {
                Text = "Panel de Informes Verificados\n\nFuncionalidades:\n• Consultar análisis verificados\n• Generar PDFs de resultados\n• Enviar informes a pacientes\n• Gestionar comunicaciones",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabInformesVerificados.Controls.Add(lblInformesVerificados);
            tabPrincipal.TabPages.Add(tabInformesVerificados);
        }

        private void CerrarSesion()
        {
            var result = MessageBox.Show(
                "¿Está seguro que desea cerrar sesión?\n\nSe perderán los datos no guardados.",
                "Confirmar Cierre de Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                // Crear nuevo login
                var nuevoLogin = new FrmLogin();
                
                // Inyección manual mínima
                var usuariosRepo = new SALC.DAL.UsuarioRepositorio();
                var hasher = new SALC.BLL.DefaultPasswordHasher();
                var auth = new SALC.BLL.AutenticacionService(usuariosRepo, hasher);
                var presenter = new SALC.Presenters.LoginPresenter(nuevoLogin, auth);
                
                // Ocultar ventana actual
                Hide();
                
                // Mostrar login
                nuevoLogin.Show();
                
                // Configurar cierre cuando se cierre el nuevo login
                nuevoLogin.FormClosed += (s, e) => Close();
            }
        }

        private void MostrarAcercaDe()
        {
            MessageBox.Show(
                "SALC - Sistema de Administración de Laboratorio Clínico\n" +
                "Versión 1.0\n\n" +
                "Desarrollado para optimizar la gestión integral\n" +
                "de laboratorios clínicos.\n\n" +
                "Características:\n" +
                "• Gestión de pacientes y análisis\n" +
                "• Control de usuarios por roles\n" +
                "• Generación de informes PDF\n" +
                "• Seguridad con encriptación BCrypt\n\n" +
                "© 2025 - Todos los derechos reservados",
                "Acerca de SALC",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private string ObtenerNombreRol(int idRol)
        {
            switch (idRol)
            {
                case 1: return "Administrador";
                case 2: return "Médico";
                case 3: return "Asistente";
                default: return "Sin Rol";
            }
        }

        private Icon CargarIcono()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "..", "..", "..", "icono.png");
                if (System.IO.File.Exists(iconPath))
                {
                    using (var bmp = new Bitmap(iconPath))
                    {
                        return Icon.FromHandle(bmp.GetHicon());
                    }
                }
            }
            catch { }
            return null;
        }
    }
}