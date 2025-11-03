using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Views.Compartidos;
using SALC.Domain;
using System.Linq;
using SALC.Views.PanelAsistente;
using SALC.Presenters;

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
            // Pestaña Usuarios - Crear directamente los controles sin el panel completo
            var tabUsuarios = new TabPage("Usuarios")
            {
                BackColor = Color.White
            };

            try
            {
                // Crear el panel de administrador para obtener la funcionalidad
                var frmPanelAdmin = new Views.PanelAdministrador.FrmPanelAdministrador
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                // Crear el presenter
                var presenter = new SALC.Presenters.PanelAdministradorPresenter(frmPanelAdmin);

                // Obtener el TabControl interno y extraer solo la pestaña de usuarios
                var tabControl = frmPanelAdmin.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControl != null && tabControl.TabPages.Count > 0)
                {
                    // Tomar la pestaña de usuarios (índice 0)
                    var usuariosTabPage = tabControl.TabPages[0];
                    
                    // Remover la pestaña del control original
                    tabControl.TabPages.Remove(usuariosTabPage);
                    
                    // Agregar directamente el contenido de la pestaña
                    tabUsuarios.Controls.Add(usuariosTabPage.Controls[0]); // El panel con el contenido
                }
                else
                {
                    // Fallback: agregar todo el formulario
                    tabUsuarios.Controls.Add(frmPanelAdmin);
                    frmPanelAdmin.Show();
                }

                // Guardar referencia del presenter
                tabUsuarios.Tag = presenter;
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

            // Pestaña Catálogos - Usar la misma lógica
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

                var tabControlCatalogos = frmPanelCatalogos.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlCatalogos != null && tabControlCatalogos.TabPages.Count > 1)
                {
                    // Tomar la pestaña de catálogos (ahora índice 1, ya que se eliminó la de pacientes)
                    var catalogosTabPage = tabControlCatalogos.TabPages[1];
                    tabControlCatalogos.TabPages.Remove(catalogosTabPage);
                    tabCatalogos.Controls.Add(catalogosTabPage.Controls[0]);
                }
                else
                {
                    tabCatalogos.Controls.Add(frmPanelCatalogos);
                    frmPanelCatalogos.Show();
                }

                tabCatalogos.Tag = presenterCatalogos;
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

            // Pestaña Backups - Usar la misma lógica
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

                var tabControlBackups = frmPanelBackups.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlBackups != null && tabControlBackups.TabPages.Count > 2)
                {
                    // Tomar la pestaña de backups (ahora índice 2, ya que se eliminó la de pacientes)
                    var backupsTabPage = tabControlBackups.TabPages[2];
                    tabControlBackups.TabPages.Remove(backupsTabPage);
                    tabBackups.Controls.Add(backupsTabPage.Controls[0]);
                }
                else
                {
                    tabBackups.Controls.Add(frmPanelBackups);
                    frmPanelBackups.Show();
                }

                tabBackups.Tag = presenterBackups;
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
            try
            {
                // Crear una sola instancia del panel médico
                var frmPanelMedico = new Views.PanelMedico.FrmPanelMedico
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                // Crear el presenter una sola vez
                var presenterMedico = new SALC.Presenters.PanelMedicoPresenter(frmPanelMedico, usuarioActual.Dni);

                // Mostrar el formulario para inicializarlo completamente
                frmPanelMedico.Show();

                // Obtener el TabControl interno del panel médico
                var tabControlMedico = frmPanelMedico.Controls.OfType<TabControl>().FirstOrDefault();
                
                if (tabControlMedico != null && tabControlMedico.TabPages.Count >= 4)
                {
                    // Extraer pestañas en el orden correcto:
                    // 0 = Gestión de Pacientes, 1 = Crear Análisis, 2 = Cargar Resultados, 3 = Validar/Firmar
                    ExtraerPestanaMedico(tabControlMedico, 0, "Gestión de Pacientes"); // NUEVA: Primera pestaña
                    ExtraerPestanaMedico(tabControlMedico, 0, "Crear Análisis");       // Ahora índice 0 porque se removió la anterior
                    ExtraerPestanaMedico(tabControlMedico, 0, "Cargar Resultados");    // Ahora índice 0 porque se removió la anterior
                    ExtraerPestanaMedico(tabControlMedico, 0, "Validar/Firmar");       // Ahora índice 0 porque se removió la anterior
                    
                    // Guardar referencia del presenter para que no se pierda
                    tabPrincipal.Tag = presenterMedico;
                }
                else
                {
                    // Fallback: mostrar mensaje de error con info de debug
                    var tabError = new TabPage("Error")
                    {
                        BackColor = Color.White
                    };
                    
                    var lblError = new Label
                    {
                        Text = $"Error: Panel médico tiene {tabControlMedico?.TabPages.Count ?? 0} pestañas.\n" +
                               "Se esperaban 4 pestañas (Gestión Pacientes, Crear, Cargar, Validar).\n" +
                               "Contacte al administrador del sistema.",
                        Font = new Font("Segoe UI", 12),
                        ForeColor = Color.Red,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Fill
                    };
                    
                    tabError.Controls.Add(lblError);
                    tabPrincipal.TabPages.Add(tabError);
                }
            }
            catch (Exception ex)
            {
                var tabError = new TabPage("Error - Funcionalidades Médico")
                {
                    BackColor = Color.White
                };
                
                var lblError = new Label
                {
                    Text = $"Error al cargar funcionalidades del médico:\n{ex.Message}\n\n" +
                           "Verifique que el Panel de Médico tenga las 4 pestañas correctas.",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                
                tabError.Controls.Add(lblError);
                tabPrincipal.TabPages.Add(tabError);
            }
        }

        private void ExtraerPestanaMedico(TabControl tabControlOriginal, int indice, string nuevoNombre)
        {
            if (indice >= 0 && indice < tabControlOriginal.TabPages.Count)
            {
                // Tomar la pestaña del control original
                var pestanaOriginal = tabControlOriginal.TabPages[indice];
                
                // Remover del control original
                tabControlOriginal.TabPages.RemoveAt(indice);
                
                // Crear nueva pestaña con el nombre apropiado
                var nuevaPestana = new TabPage(nuevoNombre)
                {
                    BackColor = Color.White,
                    UseVisualStyleBackColor = false
                };
                
                // Transferir todos los controles
                var controles = new Control[pestanaOriginal.Controls.Count];
                pestanaOriginal.Controls.CopyTo(controles, 0);
                
                foreach (var control in controles)
                {
                    pestanaOriginal.Controls.Remove(control);
                    nuevaPestana.Controls.Add(control);
                }
                
                // Agregar la nueva pestaña al TabControl principal
                tabPrincipal.TabPages.Add(nuevaPestana);
            }
        }

        private void AgregarPestanasAsistente()
        {
            try
            {
                // Pestaña 1: Gestión de Pacientes (RF-03) - NUEVA FUNCIONALIDAD
                AgregarPestanaGestionPacientesAsistente();

                // Pestaña 2: Consultar Pacientes (RF-09)
                var tabConsultarPacientes = new TabPage("Consultar Pacientes")
                {
                    BackColor = Color.White,
                    UseVisualStyleBackColor = false
                };

                // Crear una sola instancia del panel asistente para consultas
                var frmPanelAsistente = new PanelAsistente.FrmPanelAsistente
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                // Crear el presenter una sola vez
                var presenterAsistente = new SALC.Presenters.PanelAsistentePresenter(frmPanelAsistente);
                frmPanelAsistente.Tag = presenterAsistente;

                // Mostrar el formulario para inicializarlo completamente
                frmPanelAsistente.Show();

                tabConsultarPacientes.Controls.Add(frmPanelAsistente);
                tabPrincipal.TabPages.Add(tabConsultarPacientes);

                // Inicializar la vista del asistente
                presenterAsistente.InicializarVista();

                // Pestaña 3: Informes Verificados (RF-08)
                AgregarPestanaInformesVerificados();

                // Guardar referencia del presenter para que no se pierda
                tabPrincipal.Tag = presenterAsistente;
            }
            catch (Exception ex)
            {
                var tabError = new TabPage("Error - Funcionalidades Asistente")
                {
                    BackColor = Color.White
                };

                var lblError = new Label
                {
                    Text = $"Error al cargar funcionalidades del asistente:\n{ex.Message}",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };

                tabError.Controls.Add(lblError);
                tabPrincipal.TabPages.Add(tabError);
            }
        }

        private void AgregarPestanaGestionPacientesAsistente()
        {
            var tabGestionPacientes = new TabPage("Gestión de Pacientes")
            {
                BackColor = Color.White,
                UseVisualStyleBackColor = false
            };

            try
            {
                // Crear formulario de gestión de pacientes para asistente
                var frmGestionPacientes = new SALC.Views.PanelAsistente.FrmGestionPacientes
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                // Crear presenter para gestión de pacientes del asistente
                var presenterGestionPacientes = new SALC.Presenters.GestionPacientesAsistentePresenter(frmGestionPacientes);
                frmGestionPacientes.Tag = presenterGestionPacientes;

                tabGestionPacientes.Controls.Add(frmGestionPacientes);
                frmGestionPacientes.Show();

                // Inicializar la vista
                presenterGestionPacientes.InicializarVista();
            }
            catch (Exception ex)
            {
                // Si hay error, mostrar placeholder descriptivo
                var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

                var lblTitulo = new Label
                {
                    Text = "👥 Gestión de Pacientes - Asistente",
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.FromArgb(70, 130, 180),
                    Location = new Point(50, 50),
                    Size = new Size(500, 35)
                };

                var lblDescripcion = new Label
                {
                    Text = "RF-03: Administración de Pacientes por Asistente\n\n" +
                           "Funcionalidades habilitadas para Asistente:\n" +
                           "• ✅ Alta de nuevos pacientes\n" +
                           "• ✅ Modificación de datos de pacientes existentes\n" +
                           "• 📋 Listado y búsqueda de pacientes\n" +
                           "• 👁️ Visualización de información completa\n\n" +
                           "Restricciones:\n" +
                           "• ❌ No puede realizar baja de pacientes\n" +
                           "• ⚠️ Supervisado por médico asignado\n\n" +
                           "Nota: Esta funcionalidad complementa la consulta de historiales (RF-09)",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(70, 130, 180),
                    Location = new Point(50, 100),
                    Size = new Size(650, 250)
                };

                var lblEstado = new Label
                {
                    Text = $"⚠️ Error al cargar componente: {ex.Message}",
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.Orange,
                    Location = new Point(50, 370),
                    Size = new Size(600, 30)
                };

                var lblImplementacion = new Label
                {
                    Text = "💡 Componentes implementados: FrmGestionPacientes y GestionPacientesAsistentePresenter",
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.Green,
                    Location = new Point(50, 400),
                    Size = new Size(600, 30)
                };

                panel.Controls.AddRange(new Control[] { 
                    lblTitulo, lblDescripcion, lblEstado, lblImplementacion 
                });
                tabGestionPacientes.Controls.Add(panel);
            }

            tabPrincipal.TabPages.Add(tabGestionPacientes);
        }

        private void AgregarPestanaInformesVerificados()
        {
            var tabInformesVerificados = new TabPage("Informes")
            {
                BackColor = Color.White,
                UseVisualStyleBackColor = false
            };

            try
            {
                // Crear formulario de informes verificados para asistente
                var frmInformesAsistente = new SALC.Views.PanelAsistente.FrmInformesVerificados
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                // Crear presenter para informes verificados
                var presenterInformes = new SALC.Presenters.InformesVerificadosPresenter(frmInformesAsistente);
                frmInformesAsistente.Tag = presenterInformes;

                tabInformesVerificados.Controls.Add(frmInformesAsistente);
                frmInformesAsistente.Show();

                // Inicializar la vista
                presenterInformes.InicializarVista();
            }
            catch (Exception ex)
            {
                // Si hay error, mostrar placeholder mejorado
                var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

                var lblTitulo = new Label
                {
                    Text = "📋 Informes Verificados",
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.FromArgb(70, 130, 180),
                    Location = new Point(50, 50),
                    Size = new Size(400, 35)
                };

                var lblDescripcion = new Label
                {
                    Text = "RF-08: Generar y Enviar Informes de Análisis Verificados\n\n" +
                           "Funcionalidades para Asistente:\n" +
                           "• Consultar análisis en estado 'Verificado'\n" +
                           "• Generar informes PDF de resultados\n" +
                           "• Enviar informes a pacientes por email\n" +
                           "• Gestionar comunicaciones con pacientes\n\n" +
                           "Restricción: Solo análisis previamente verificados por un Médico",
                    Font = new Font("Segoe UI", 11),
                    ForeColor = Color.FromArgb(70, 130, 180),
                    Location = new Point(50, 100),
                    Size = new Size(600, 200)
                };

                var lblEstado = new Label
                {
                    Text = $"⚠️ Error al cargar componente: {ex.Message}",
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.Orange,
                    Location = new Point(50, 320),
                    Size = new Size(600, 30)
                };

                var lblImplementacion = new Label
                {
                    Text = $"💡 Error técnico: {ex.StackTrace}",
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    Location = new Point(50, 350),
                    Size = new Size(600, 30)
                };

                panel.Controls.AddRange(new Control[] { lblTitulo, lblDescripcion, lblEstado, lblImplementacion });
                tabInformesVerificados.Controls.Add(panel);
            }

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