using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Views.Compartidos;
using SALC.Domain;

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
            Text = "SALC - Sistema de Administraci�n de Laboratorio Cl�nico";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Icon = CargarIcono();

            // Agregar men� principal
            var menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            // Men� Archivo
            var menuArchivo = new ToolStripMenuItem("Archivo")
            {
                ForeColor = Color.White
            };

            var menuCerrarSesion = new ToolStripMenuItem("Cerrar Sesi�n")
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

            // Men� Ayuda
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

            // TabControl principal - ajustado para no solaparse con el men�
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

            // Bot�n de cerrar sesi�n en la barra de estado
            var btnCerrarSesion = new ToolStripDropDownButton
            {
                Text = "Cerrar Sesi�n",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                DisplayStyle = ToolStripItemDisplayStyle.Text
            };
            
            var itemCerrarSesion = new ToolStripMenuItem("Cerrar Sesi�n");
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

            // Ajustar el TabControl cuando la ventana cambie de tama�o
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
            // Pesta�a de Inicio (com�n para todos)
            AgregarPestanaInicio();

            // Pesta�as espec�ficas por rol
            switch (usuarioActual?.IdRol)
            {
                case 1: // Administrador
                    AgregarPestanasAdministrador();
                    break;
                case 2: // M�dico
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
            // Pesta�a Usuarios
            var tabUsuarios = new TabPage("Usuarios")
            {
                BackColor = Color.White
            };
            
            var lblUsuarios = new Label
            {
                Text = "Panel de Administraci�n de Usuarios\n\nFuncionalidades:\n� Crear usuarios (M�dicos y Asistentes)\n� Modificar informaci�n de usuarios\n� Gestionar estados de usuarios\n� Asignar roles y permisos",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabUsuarios.Controls.Add(lblUsuarios);
            tabPrincipal.TabPages.Add(tabUsuarios);

            // Pesta�a Pacientes
            var tabPacientes = new TabPage("Pacientes")
            {
                BackColor = Color.White
            };
            
            var lblPacientes = new Label
            {
                Text = "Panel de Administraci�n de Pacientes\n\nFuncionalidades:\n� Registrar nuevos pacientes\n� Actualizar informaci�n personal\n� Gestionar obras sociales\n� Administrar estados de pacientes",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabPacientes.Controls.Add(lblPacientes);
            tabPrincipal.TabPages.Add(tabPacientes);

            // Pesta�a Cat�logos
            var tabCatalogos = new TabPage("Cat�logos")
            {
                BackColor = Color.White
            };
            
            var lblCatalogos = new Label
            {
                Text = "Panel de Administraci�n de Cat�logos\n\nFuncionalidades:\n� Gestionar tipos de an�lisis\n� Administrar m�tricas de laboratorio\n� Configurar obras sociales\n� Mantener par�metros del sistema",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabCatalogos.Controls.Add(lblCatalogos);
            tabPrincipal.TabPages.Add(tabCatalogos);

            // Pesta�a Backups
            var tabBackups = new TabPage("Backups")
            {
                BackColor = Color.White
            };
            
            var lblBackups = new Label
            {
                Text = "Panel de Gesti�n de Copias de Seguridad\n\nFuncionalidades:\n� Crear copias de seguridad\n� Programar backups autom�ticos\n� Restaurar base de datos\n� Verificar integridad de datos",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabBackups.Controls.Add(lblBackups);
            tabPrincipal.TabPages.Add(tabBackups);
        }

        private void AgregarPestanasMedico()
        {
            // Pesta�a Crear An�lisis
            var tabCrear = new TabPage("Crear An�lisis")
            {
                BackColor = Color.White
            };
            
            var lblCrear = new Label
            {
                Text = "Panel de Creaci�n de An�lisis\n\nFuncionalidades:\n� Crear nuevos an�lisis para pacientes\n� Seleccionar tipos de estudios\n� Asignar an�lisis a pacientes\n� Configurar par�metros iniciales",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabCrear.Controls.Add(lblCrear);
            tabPrincipal.TabPages.Add(tabCrear);

            // Pesta�a Cargar Resultados
            var tabResultados = new TabPage("Resultados")
            {
                BackColor = Color.White
            };
            
            var lblResultados = new Label
            {
                Text = "Panel de Carga de Resultados\n\nFuncionalidades:\n� Cargar valores de m�tricas\n� Ingresar resultados num�ricos\n� A�adir observaciones\n� Modificar an�lisis pendientes",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabResultados.Controls.Add(lblResultados);
            tabPrincipal.TabPages.Add(tabResultados);

            // Pesta�a Firmar
            var tabFirmar = new TabPage("Validar")
            {
                BackColor = Color.White
            };
            
            var lblFirmar = new Label
            {
                Text = "Panel de Validaci�n de An�lisis\n\nFuncionalidades:\n� Revisar an�lisis completados\n� Validar resultados cargados\n� Firmar digitalmente\n� Cambiar estado a 'Verificado'",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            
            tabFirmar.Controls.Add(lblFirmar);
            tabPrincipal.TabPages.Add(tabFirmar);

            // Pesta�a Informes
            var tabInformes = new TabPage("Informes")
            {
                BackColor = Color.White
            };
            
            var lblInformes = new Label
            {
                Text = "Panel de Generaci�n de Informes\n\nFuncionalidades:\n� Generar informes PDF\n� Enviar resultados a pacientes\n� Consultar an�lisis propios\n� Gestionar reportes m�dicos",
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
            // Pesta�a Panel del Asistente (reemplaza la ventana actual)
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

            // Pesta�a Informes Verificados
            var tabInformesVerificados = new TabPage("Informes")
            {
                BackColor = Color.White
            };
            
            var lblInformesVerificados = new Label
            {
                Text = "Panel de Informes Verificados\n\nFuncionalidades:\n� Consultar an�lisis verificados\n� Generar PDFs de resultados\n� Enviar informes a pacientes\n� Gestionar comunicaciones",
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
                "�Est� seguro que desea cerrar sesi�n?\n\nSe perder�n los datos no guardados.",
                "Confirmar Cierre de Sesi�n",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                // Crear nuevo login
                var nuevoLogin = new FrmLogin();
                
                // Inyecci�n manual m�nima
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
                "SALC - Sistema de Administraci�n de Laboratorio Cl�nico\n" +
                "Versi�n 1.0\n\n" +
                "Desarrollado para optimizar la gesti�n integral\n" +
                "de laboratorios cl�nicos.\n\n" +
                "Caracter�sticas:\n" +
                "� Gesti�n de pacientes y an�lisis\n" +
                "� Control de usuarios por roles\n" +
                "� Generaci�n de informes PDF\n" +
                "� Seguridad con encriptaci�n BCrypt\n\n" +
                "� 2025 - Todos los derechos reservados",
                "Acerca de SALC",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private string ObtenerNombreRol(int idRol)
        {
            switch (idRol)
            {
                case 1: return "Administrador";
                case 2: return "M�dico";
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