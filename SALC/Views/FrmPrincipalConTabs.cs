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
            Text = "Sistema de Administración de Laboratorio Clínico - SALC";
            Size = new Size(1280, 850);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Color.White;
            ShowIcon = false; // No mostrar icono personalizado - usar default de Windows

            // ============ MENÚ PRINCIPAL ============
            var menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(46, 134, 193), // Azul médico profesional
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Padding = new Padding(10, 4, 0, 4)
            };

            // Menú Archivo
            var menuArchivo = new ToolStripMenuItem("Archivo")
            {
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var menuCerrarSesion = new ToolStripMenuItem("Cerrar Sesión")
            {
                ForeColor = Color.FromArgb(44, 62, 80),
                ShortcutKeys = Keys.Control | Keys.Q,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            menuCerrarSesion.Click += (s, e) => CerrarSesion();

            var menuSalir = new ToolStripMenuItem("Salir del Sistema")
            {
                ForeColor = Color.FromArgb(44, 62, 80),
                ShortcutKeys = Keys.Alt | Keys.F4,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
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
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var menuAcercaDe = new ToolStripMenuItem("Acerca del Sistema")
            {
                ForeColor = Color.FromArgb(44, 62, 80),
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };
            menuAcercaDe.Click += (s, e) => MostrarAcercaDe();

            menuAyuda.DropDownItems.Add(menuAcercaDe);

            menuStrip.Items.AddRange(new ToolStripItem[] { menuArchivo, menuAyuda });

            // ============ PANEL DE INFORMACIÓN DE USUARIO ============
            var panelInfoUsuario = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(245, 250, 255), // Azul muy claro
                Padding = new Padding(20, 8, 20, 8)
            };

            // Información del usuario en la esquina derecha
            var lblInfoUsuario = new Label
            {
                Text = $"Usuario conectado: {usuarioActual?.Nombre} {usuarioActual?.Apellido}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                AutoSize = true,
                Location = new Point(20, 12)
            };

            var lblRolUsuario = new Label
            {
                Text = $"Rol: {ObtenerNombreRol(usuarioActual?.IdRol ?? 0)}",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true
            };
            // Posicionar a la derecha
            panelInfoUsuario.Resize += (s, e) => {
                lblRolUsuario.Location = new Point(panelInfoUsuario.Width - lblRolUsuario.Width - 20, 14);
            };

            var lblVersionSistema = new Label
            {
                Text = "SALC v1.0",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(149, 165, 166),
                AutoSize = true
            };
            // Posicionar a la derecha, antes del rol
            panelInfoUsuario.Resize += (s, e) => {
                lblVersionSistema.Location = new Point(panelInfoUsuario.Width - lblVersionSistema.Width - lblRolUsuario.Width - 50, 14);
            };

            panelInfoUsuario.Controls.AddRange(new Control[] {
                lblInfoUsuario, lblRolUsuario, lblVersionSistema
            });

            // ============ TAB CONTROL PRINCIPAL - REDISEÑADO ============
            tabPrincipal = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Padding = new Point(25, 10), // Mayor separación entre pestañas
                ItemSize = new Size(140, 40), // Pestañas más grandes
                SizeMode = TabSizeMode.Fixed,
                DrawMode = TabDrawMode.OwnerDrawFixed
            };

            // Dibujar pestañas con color de fondo para la activa
            tabPrincipal.DrawItem += (s, e) => {
                var tab = tabPrincipal.TabPages[e.Index];
                var bounds = e.Bounds;
                var isSelected = (e.Index == tabPrincipal.SelectedIndex);

                // Color de fondo de la pestaña
                Color backColor;
                Color textColor;

                if (isSelected)
                {
                    backColor = Color.FromArgb(209, 231, 248); // Azul pastel claro (pestaña activa)
                    textColor = Color.FromArgb(41, 128, 185); // Azul oscuro
                }
                else
                {
                    backColor = Color.FromArgb(236, 240, 241); // Gris muy claro
                    textColor = Color.FromArgb(127, 140, 141); // Gris
                }

                // Dibujar fondo
                using (var brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle(brush, bounds);
                }

                // Dibujar texto centrado
                var textBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                TextRenderer.DrawText(e.Graphics, tab.Text, tabPrincipal.Font, textBounds, 
                    textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                // Borde inferior para pestaña activa
                if (isSelected)
                {
                    using (var pen = new Pen(Color.FromArgb(52, 152, 219), 3))
                    {
                        e.Graphics.DrawLine(pen, bounds.Left, bounds.Bottom - 1, 
                            bounds.Right, bounds.Bottom - 1);
                    }
                }
            };

            // ============ BARRA DE ESTADO ============
            statusStrip = new StatusStrip
            {
                BackColor = Color.FromArgb(46, 134, 193),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Padding = new Padding(10, 0, 10, 0)
            };

            lblFecha = new ToolStripStatusLabel
            {
                Text = $"{DateTime.Now:dddd, dd 'de' MMMM 'de' yyyy - HH:mm}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Margin = new Padding(5, 0, 20, 0)
            };

            lblEstado = new ToolStripStatusLabel
            {
                Text = "Sistema Operativo  •  Base de Datos Conectada",
                ForeColor = Color.White,
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            // Botón de cerrar sesión en la barra de estado
            var btnCerrarSesion = new ToolStripDropDownButton
            {
                Text = "Cerrar Sesión",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Margin = new Padding(10, 0, 0, 0)
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
                lblFecha, lblEstado, btnCerrarSesion
            });

            // Agregar controles en el orden correcto
            Controls.AddRange(new Control[] { 
                tabPrincipal, panelInfoUsuario, menuStrip, statusStrip 
            });

            // Timer para actualizar fecha
            var timer = new Timer { Interval = 60000 }; // 1 minuto
            timer.Tick += (s, e) => lblFecha.Text = $"{DateTime.Now:dddd, dd 'de' MMMM 'de' yyyy - HH:mm}";
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
                
                // IMPORTANTE: Establecer el DNI del usuario actual para backups
                presenter.EstablecerUsuarioActual(usuarioActual.Dni);

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
                
                // IMPORTANTE: Establecer el DNI del usuario actual
                presenterCatalogos.EstablecerUsuarioActual(usuarioActual.Dni);

                var tabControlCatalogos = frmPanelCatalogos.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlCatalogos != null && tabControlCatalogos.TabPages.Count > 1)
                {
                    // Tomar la pestaña de catálogos (índice 1)
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

            // Pestaña Reportes - NUEVA PESTAÑA AGREGADA
            var tabReportes = new TabPage("Reportes")
            {
                BackColor = Color.White
            };
            
            try
            {
                var frmPanelReportes = new Views.PanelAdministrador.FrmPanelAdministrador
                {
                    TopLevel = false,
                    FormBorderStyle = FormBorderStyle.None,
                    Dock = DockStyle.Fill
                };

                var presenterReportes = new SALC.Presenters.PanelAdministradorPresenter(frmPanelReportes);
                
                // IMPORTANTE: Establecer el DNI del usuario actual
                presenterReportes.EstablecerUsuarioActual(usuarioActual.Dni);

                var tabControlReportes = frmPanelReportes.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlReportes != null && tabControlReportes.TabPages.Count > 2)
                {
                    // Tomar la pestaña de reportes (índice 2)
                    var reportesTabPage = tabControlReportes.TabPages[2];
                    tabControlReportes.TabPages.Remove(reportesTabPage);
                    tabReportes.Controls.Add(reportesTabPage.Controls[0]);
                }
                else
                {
                    tabReportes.Controls.Add(frmPanelReportes);
                    frmPanelReportes.Show();
                }

                tabReportes.Tag = presenterReportes;
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Error al cargar panel de reportes:\n{ex.Message}",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tabReportes.Controls.Add(lblError);
            }

            tabPrincipal.TabPages.Add(tabReportes);

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
                
                // IMPORTANTE: Establecer el DNI del usuario actual para backups
                presenterBackups.EstablecerUsuarioActual(usuarioActual.Dni);

                var tabControlBackups = frmPanelBackups.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlBackups != null && tabControlBackups.TabPages.Count > 3)
                {
                    // Tomar la pestaña de backups (índice 3)
                    var backupsTabPage = tabControlBackups.TabPages[3];
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
                    ExtraerPestanaMedico(tabControlMedico, 0, "Gestión de Pacientes");
                    ExtraerPestanaMedico(tabControlMedico, 0, "Crear Análisis");
                    ExtraerPestanaMedico(tabControlMedico, 0, "Cargar Resultados");
                    ExtraerPestanaMedico(tabControlMedico, 0, "Validar/Firmar");
                    
                    // ⭐ NUEVA: Agregar pestaña de Reportes para Médico
                    AgregarPestanaReportesMedico();
                    
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

        private void AgregarPestanaReportesMedico()
        {
            var tabReportes = new TabPage("Mis Reportes")
            {
                BackColor = Color.White,
                UseVisualStyleBackColor = false
            };

            try
            {
                // Crear un botón para abrir el módulo de reportes (similar al patrón del administrador)
                var panelPrincipal = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(30)
                };

                var lblTitulo = new Label
                {
                    Text = "Reportes de Calidad y Desempeño Personal",
                    Location = new Point(0, 0),
                    Size = new Size(900, 35),
                    Font = new Font("Segoe UI", 16, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 150, 136)
                };

                var lblSubtitulo = new Label
                {
                    Text = "Acceda a sus reportes personalizados para monitorear calidad de trabajo y carga laboral",
                    Location = new Point(0, 40),
                    Size = new Size(900, 25),
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    ForeColor = Color.FromArgb(127, 140, 141)
                };

                var grpInfo = new GroupBox
                {
                    Text = "  Información de Reportes Disponibles  ",
                    Location = new Point(0, 80),
                    Size = new Size(1000, 180),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 150, 136),
                    BackColor = Color.White
                };

                var lblDescripcion = new Label
                {
                    Text = "Los reportes personalizados le ayudan a monitorear su calidad de trabajo\n" +
                           "y a identificar proactivamente situaciones que requieren atención médica.\n\n" +
                           "Reportes disponibles:\n\n" +
                           "• Reporte de Alertas: Identifica valores críticos fuera de rango que requieren seguimiento\n" +
                           "• Mi Carga de Trabajo: Visualiza análisis pendientes y verificados en el período actual",
                    Location = new Point(20, 30),
                    Size = new Size(960, 140),
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.FromArgb(44, 62, 80),
                    BackColor = Color.Transparent
                };

                grpInfo.Controls.Add(lblDescripcion);

                var grpAcciones = new GroupBox
                {
                    Text = "  Acceder a Mis Reportes  ",
                    Location = new Point(0, 280),
                    Size = new Size(1000, 180),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 150, 136),
                    BackColor = Color.White
                };

                var btnReportes = new Button
                {
                    Text = "Abrir Módulo de Reportes Personales",
                    Location = new Point(300, 45),
                    Size = new Size(400, 50),
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    BackColor = Color.FromArgb(0, 150, 136),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnReportes.FlatAppearance.BorderSize = 0;
                btnReportes.Click += (s, e) => AbrirReportesMedico();

            var lblNota = new Label
            {
                Text = "Beneficios de usar los reportes:\n\n" +
                       "• Seguimiento proactivo de pacientes con valores críticos\n" +
                       "• Visualización rápida de su carga de trabajo actual\n" +
                       "• Mejora continua de la calidad asistencial",
                Location = new Point(250, 110),
                Size = new Size(500, 60),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.TopCenter
            };

                grpAcciones.Controls.AddRange(new Control[] { btnReportes, lblNota });

                panelPrincipal.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, grpInfo, grpAcciones });
                tabReportes.Controls.Add(panelPrincipal);
            }
            catch (Exception ex)
            {
                var lblError = new Label
                {
                    Text = $"Error al configurar la pestaña de reportes:\n{ex.Message}",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Red,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };
                tabReportes.Controls.Add(lblError);
            }

            tabPrincipal.TabPages.Add(tabReportes);
        }

        private void AbrirReportesMedico()
        {
            try
            {
                using (var dlg = new Views.PanelMedico.FrmReportesMedico(usuarioActual.Dni))
                {
                    dlg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al abrir módulo de reportes:\n{ex.Message}",
                    "Error - Reportes Médico",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
                    Text = string.Format("Error al cargar funcionalidades del asistente:\n{0}", ex.Message),
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
    }
}