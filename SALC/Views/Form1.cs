using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace SALC
{
    public partial class Form1 : Form
    {
        private Panel sidebar;
        private Panel mainContent;
        private Panel topbar;
        private Panel dashboardGrid;
        private Panel recentOrdersPanel;

        public Form1()
        {
            InitializeComponent();
            InitializeMainForm();
        }

        private void InitializeMainForm()
        {
            this.Text = "SALC - Dashboard Clínico";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(248, 249, 250);

            CreateSidebar();
            CreateMainContent();
        }

        private void CreateSidebar()
        {
            sidebar = new Panel
            {
                Width = 250,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(52, 58, 64),
                Padding = new Padding(20)
            };

            // Logo container
            Panel logoContainer = new Panel
            {
                Size = new Size(210, 120),
                Location = new Point(20, 20),
                BackColor = Color.Transparent
            };

            // Logo
            PictureBox logoBox = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(65, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            // Intentar cargar el icono
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, "..", "..", "..", "icono.png");
                if (File.Exists(iconPath))
                {
                    logoBox.Image = Image.FromFile(iconPath);
                }
            }
            catch { /* Ignorar errores de carga de imagen */ }

            Label logoLabel = new Label
            {
                Text = "SALC",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(210, 30),
                Location = new Point(0, 90)
            };

            logoContainer.Controls.AddRange(new Control[] { logoBox, logoLabel });

            // Navegación
            Panel navigation = new Panel
            {
                Size = new Size(210, 400),
                Location = new Point(20, 160),
                BackColor = Color.Transparent
            };

            var menuItems = new[]
            {
                new { Text = "📊 Dashboard", Action = new EventHandler(ShowDashboard), Active = true },
                new { Text = "🏥 Gestión de Pacientes", Action = new EventHandler(ShowPatients), Active = false },
                new { Text = "🧪 Gestión de Estudios", Action = new EventHandler(ShowStudies), Active = false },
                new { Text = "⚗️ Carga de Resultados", Action = new EventHandler(ShowResults), Active = false },
                new { Text = "📄 Generar Informes", Action = new EventHandler(ShowReports), Active = false },
                new { Text = "🔔 Notificaciones", Action = new EventHandler(ShowNotifications), Active = false },
                new { Text = "📋 Historial de Órdenes", Action = new EventHandler(ShowHistory), Active = false }
            };

            int yPosition = 0;
            foreach (var item in menuItems)
            {
                Button menuButton = new Button
                {
                    Text = item.Text,
                    Size = new Size(210, 45),
                    Location = new Point(0, yPosition),
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    ForeColor = item.Active ? Color.White : Color.FromArgb(173, 181, 189),
                    BackColor = item.Active ? Color.FromArgb(0, 123, 255) : Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor = Cursors.Hand
                };

                menuButton.FlatAppearance.BorderSize = 0;
                menuButton.Click += item.Action;

                // Efectos hover
                menuButton.MouseEnter += (s, e) => 
                {
                    if (menuButton.BackColor == Color.Transparent)
                        menuButton.BackColor = Color.FromArgb(0, 123, 255);
                };
                menuButton.MouseLeave += (s, e) => 
                {
                    if (!item.Active)
                        menuButton.BackColor = Color.Transparent;
                };

                navigation.Controls.Add(menuButton);
                yPosition += 50;
            }

            // User info
            Panel userInfo = new Panel
            {
                Size = new Size(210, 100),
                Location = new Point(20, 600),
                BackColor = Color.Transparent
            };

            Label userLabel = new Label
            {
                Text = $"👤 {UserAuthentication.CurrentUser?.Username ?? "Usuario"}",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(173, 181, 189),
                Size = new Size(210, 20),
                Location = new Point(0, 0)
            };

            Label roleLabel = new Label
            {
                Text = $"Rol: {UserAuthentication.CurrentUser?.Role ?? "No definido"}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(173, 181, 189),
                Size = new Size(210, 20),
                Location = new Point(0, 25)
            };

            Button logoutButton = new Button
            {
                Text = "🚪 Cerrar Sesión",
                Size = new Size(210, 35),
                Location = new Point(0, 55),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            logoutButton.FlatAppearance.BorderSize = 0;
            logoutButton.Click += LogoutButton_Click;

            userInfo.Controls.AddRange(new Control[] { userLabel, roleLabel, logoutButton });

            sidebar.Controls.AddRange(new Control[] { logoContainer, navigation, userInfo });
            this.Controls.Add(sidebar);
        }

        private void CreateMainContent()
        {
            mainContent = new Panel
            {
                Location = new Point(250, 0), // Empezar después del sidebar (250px)
                Size = new Size(this.Width - 250, this.Height), // Ancho total menos el sidebar
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(30),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            CreateTopbar();
            CreateDashboardGrid();
            CreateRecentOrdersPanel();

            this.Controls.Add(mainContent);
        }

        private void CreateTopbar()
        {
            // Usar el ancho del mainContent menos padding
            int topbarWidth = Math.Max(600, (this.Width - 250) - 60); // Ancho disponible menos padding
            
            topbar = new Panel
            {
                Size = new Size(topbarWidth, 80),
                Location = new Point(30, 30),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label titleLabel = new Label
            {
                Text = "Dashboard Clínico",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Size = new Size(300, 30),
                Location = new Point(20, 25)
            };

            // Search bar
            Panel searchContainer = new Panel
            {
                Size = new Size(300, 30),
                Location = new Point(Math.Max(400, topbar.Width - 320), 25),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            TextBox searchBox = new TextBox
            {
                Text = "Buscar paciente o estudio...",
                Size = new Size(250, 30),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Color.Gray
            };

            // Simular placeholder text
            searchBox.GotFocus += (s, e) =>
            {
                if (searchBox.Text == "Buscar paciente o estudio...")
                {
                    searchBox.Text = "";
                    searchBox.ForeColor = Color.Black;
                }
            };

            searchBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(searchBox.Text))
                {
                    searchBox.Text = "Buscar paciente o estudio...";
                    searchBox.ForeColor = Color.Gray;
                }
            };

            Button searchButton = new Button
            {
                Text = "🔍",
                Size = new Size(40, 30),
                Location = new Point(255, 0),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            searchButton.FlatAppearance.BorderSize = 0;

            searchContainer.Controls.AddRange(new Control[] { searchBox, searchButton });
            topbar.Controls.AddRange(new Control[] { titleLabel, searchContainer });

            mainContent.Controls.Add(topbar);
        }

        private void CreateDashboardGrid()
        {
            // Usar el ancho del mainContent menos padding
            int gridWidth = Math.Max(600, (this.Width - 250) - 60); // Ancho disponible menos padding
            
            dashboardGrid = new Panel
            {
                Size = new Size(gridWidth, 300),
                Location = new Point(30, 130),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var cardData = new[]
            {
                new { Title = "🏥 Gestión de Pacientes", Description = "Administra la información completa de los pacientes (CRUD).", ButtonText = "Ver Pacientes", Color = Color.FromArgb(23, 162, 184) },
                new { Title = "🧪 Gestión de Estudios", Description = "Registra nuevas órdenes de análisis y asigna prioridades.", ButtonText = "Nueva Orden", Color = Color.FromArgb(0, 123, 255) },
                new { Title = "⚗️ Carga de Resultados", Description = "Ingresa y valida los resultados de los análisis de laboratorio.", ButtonText = "Cargar Resultados", Color = Color.FromArgb(255, 193, 7) },
                new { Title = "📄 Generación de Informes", Description = "Genera informes PDF de los estudios completados.", ButtonText = "Ver Informes", Color = Color.FromArgb(111, 66, 193) },
                new { Title = "🔔 Notificaciones", Description = "Envía notificaciones automáticas a los pacientes.", ButtonText = "Enviar Notificación", Color = Color.FromArgb(253, 126, 20) },
                new { Title = "📋 Historial de Órdenes", Description = "Consulta el historial completo de todas las órdenes de análisis.", ButtonText = "Ver Historial", Color = Color.FromArgb(40, 167, 69) }
            };

            int cardWidth = 350;
            int cardHeight = 140;
            
            // Asegurar que el ancho del dashboard esté disponible antes de calcular
            int availableWidth = Math.Max(dashboardGrid.Width, 800); // Mínimo 800px
            int cardsPerRow = Math.Max(1, (availableWidth - 40) / (cardWidth + 20)); // Mínimo 1 tarjeta por fila
            
            for (int i = 0; i < cardData.Length; i++)
            {
                int row = i / cardsPerRow;
                int col = i % cardsPerRow;
                
                Panel card = CreateDashboardCard(cardData[i].Title, cardData[i].Description, cardData[i].ButtonText, cardData[i].Color);
                card.Location = new Point(col * (cardWidth + 20), row * (cardHeight + 20));
                dashboardGrid.Controls.Add(card);
            }

            mainContent.Controls.Add(dashboardGrid);
        }

        private Panel CreateDashboardCard(string title, string description, string buttonText, Color color)
        {
            Panel card = new Panel
            {
                Size = new Size(340, 130),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = color,
                Size = new Size(300, 25),
                Location = new Point(15, 15)
            };

            Label descLabel = new Label
            {
                Text = description,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 58, 64),
                Size = new Size(300, 40),
                Location = new Point(15, 45)
            };

            Button actionButton = new Button
            {
                Text = buttonText,
                Size = new Size(300, 30),
                Location = new Point(15, 90),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            actionButton.FlatAppearance.BorderSize = 0;

            card.Controls.AddRange(new Control[] { titleLabel, descLabel, actionButton });
            return card;
        }

        private void CreateRecentOrdersPanel()
        {
            // Usar el ancho del mainContent menos padding
            int panelWidth = Math.Max(600, (this.Width - 250) - 60); // Ancho disponible menos padding
            
            recentOrdersPanel = new Panel
            {
                Size = new Size(panelWidth, 250),
                Location = new Point(30, 450),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            Label ordersTitle = new Label
            {
                Text = "📋 Órdenes Recientes / Pendientes",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 123, 255),
                Size = new Size(400, 30),
                Location = new Point(20, 20)
            };

            // Crear tabla de órdenes (simulada con labels)
            CreateOrdersTable();

            recentOrdersPanel.Controls.Add(ordersTitle);
            mainContent.Controls.Add(recentOrdersPanel);
        }

        private void CreateOrdersTable()
        {
            var headers = new[] { "ID Orden", "Paciente", "Estudio", "Fecha", "Estado" };
            var sampleData = new[,]
            {
                { "#00123", "Ana García", "Hemograma Completo", "2025-09-01", "Pendiente" },
                { "#00122", "Carlos Ruiz", "Glucosa, Colesterol", "2025-08-31", "En Proceso" },
                { "#00121", "María López", "Uroanálisis", "2025-08-30", "Completado" },
                { "#00120", "Pedro Sánchez", "Perfil Hepático", "2025-08-29", "Entregado" }
            };

            // Headers
            for (int i = 0; i < headers.Length; i++)
            {
                Label header = new Label
                {
                    Text = headers[i],
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.FromArgb(233, 236, 239),
                    ForeColor = Color.FromArgb(52, 58, 64),
                    Size = new Size(150, 30),
                    Location = new Point(20 + i * 150, 60),
                    TextAlign = ContentAlignment.MiddleLeft,
                    BorderStyle = BorderStyle.FixedSingle
                };
                recentOrdersPanel.Controls.Add(header);
            }

            // Data rows
            for (int row = 0; row < sampleData.GetLength(0); row++)
            {
                for (int col = 0; col < sampleData.GetLength(1); col++)
                {
                    Label cell = new Label
                    {
                        Text = sampleData[row, col],
                        Font = new Font("Segoe UI", 9, FontStyle.Regular),
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(52, 58, 64),
                        Size = new Size(150, 25),
                        Location = new Point(20 + col * 150, 90 + row * 25),
                        TextAlign = ContentAlignment.MiddleLeft,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    // Colorear estados
                    if (col == 4) // Columna de estado
                    {
                        switch (sampleData[row, col])
                        {
                            case "Pendiente":
                                cell.BackColor = Color.FromArgb(255, 193, 7);
                                break;
                            case "En Proceso":
                                cell.BackColor = Color.FromArgb(23, 162, 184);
                                cell.ForeColor = Color.White;
                                break;
                            case "Completado":
                                cell.BackColor = Color.FromArgb(40, 167, 69);
                                cell.ForeColor = Color.White;
                                break;
                            case "Entregado":
                                cell.BackColor = Color.FromArgb(0, 123, 255);
                                cell.ForeColor = Color.White;
                                break;
                        }
                    }

                    recentOrdersPanel.Controls.Add(cell);
                }
            }
        }

        // Event handlers para los menús
        private void ShowDashboard(object sender, EventArgs e) => MessageBox.Show("Mostrando Dashboard");
        private void ShowPatients(object sender, EventArgs e) => MessageBox.Show("Módulo de Gestión de Pacientes");
        private void ShowStudies(object sender, EventArgs e) => MessageBox.Show("Módulo de Gestión de Estudios");
        private void ShowResults(object sender, EventArgs e) => MessageBox.Show("Módulo de Carga de Resultados");
        private void ShowReports(object sender, EventArgs e) => MessageBox.Show("Módulo de Generación de Informes");
        private void ShowNotifications(object sender, EventArgs e) => MessageBox.Show("Módulo de Notificaciones");
        private void ShowHistory(object sender, EventArgs e) => MessageBox.Show("Módulo de Historial de Órdenes");

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro que desea cerrar sesión?", "Confirmar Cierre de Sesión", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                UserAuthentication.Logout();
                this.Hide();
                
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                loginForm.FormClosed += (s, args) => Application.Exit();
            }
        }
    }
}
