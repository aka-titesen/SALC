using System;
using System.Windows.Forms;
using System.Drawing;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelAdministrador
{
    public class FrmPanelAdministrador : Form, IPanelAdministradorView
    {
        private TabControl tabs;
        // Grillas por sección
        private DataGridView gridUsuarios;
        private DataGridView gridObrasSociales;
        private DataGridView gridTiposAnalisis;
        private DataGridView gridMetricas;

        // Controles adicionales
        private ComboBox cboFiltroEstadoUsuarios;
        private ComboBox cboFiltroEstadoObrasSociales;
        private ComboBox cboFiltroEstadoTiposAnalisis;
        private ComboBox cboFiltroEstadoMetricas;

        public FrmPanelAdministrador()
        {
            Text = "Gestión Administrativa del Sistema";
            BackColor = Color.White;
            Width = 1200;
            Height = 700;

            tabs = new TabControl 
            { 
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Padding = new Point(15, 8),
                ItemSize = new Size(120, 35),
                SizeMode = TabSizeMode.Fixed
            };
            
            Controls.Add(tabs);

            AgregarTabUsuarios();
            AgregarTabCatalogos();
            AgregarTabReportes();
            AgregarTabBackups();
        }

        private void AgregarTabUsuarios()
        {
            var tab = new TabPage("Gestión de Usuarios")
            {
                BackColor = Color.White
            };
            
            // Panel principal con padding
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            // Título de la sección - MÁS ALTO
            var lblTitulo = new Label
            {
                Text = "Administración de Personal Médico y Asistentes",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(25, 25),
                Size = new Size(1100, 40),
                BackColor = Color.White,
                AutoSize = false
            };

            // Toolbar - MÁS ALTO Y MÁS ABAJO
            var tool = new ToolStrip
            {
                BackColor = Color.FromArgb(236, 240, 241),
                GripStyle = ToolStripGripStyle.Hidden,
                Padding = new Padding(12, 8, 12, 8),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new Point(25, 80),
                Width = 1100,
                AutoSize = false,
                Height = 45
            };
            
            var btnNuevo = new ToolStripButton("Nuevo Usuario") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96)
            };
            
            var btnEditar = new ToolStripButton("Modificar") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            
            var btnEliminar = new ToolStripButton("Dar de Baja") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(192, 57, 43)
            };
            
            var btnDetalle = new ToolStripButton("Ver Información Completa") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            
            var lblBuscar = new ToolStripLabel("Buscar:") 
            { 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };
            
            var txtBuscar = new ToolStripTextBox 
            { 
                Width = 240, 
                ToolTipText = "DNI, Apellido o Email",
                Font = new Font("Segoe UI", 10)
            };
            
            // Filtro de estado
            var lblFiltroEstado = new ToolStripLabel("Filtrar por Estado:")
            {
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(15, 0, 5, 0)
            };
            
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoUsuarios = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 120,
                Font = new Font("Segoe UI", 10)
            });
            
            cboFiltroEstadoUsuarios.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoUsuarios.SelectedIndex = 0;
            cboFiltroEstadoUsuarios.SelectedIndexChanged += (s, e) => 
                UsuariosFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoUsuarios.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar, btnEliminar, btnDetalle,
                new ToolStripSeparator(), 
                lblBuscar, txtBuscar,
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            // DataGridView - MUCHO MÁS ABAJO CON ENCABEZADOS ALTOS
            gridUsuarios = new DataGridView 
            { 
                Location = new Point(25, 140),
                Size = new Size(1100, 430),
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(52, 152, 219),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(209, 231, 248),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(6)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 252, 255)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 }
            };

            // Eventos
            btnNuevo.Click += (s, e) => UsuariosNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => UsuariosEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => UsuariosEliminarClick?.Invoke(this, EventArgs.Empty);
            btnDetalle.Click += (s, e) => UsuariosDetalleClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => UsuariosBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            // Agregar controles al panel en orden
            panelPrincipal.Controls.Add(gridUsuarios);
            panelPrincipal.Controls.Add(tool);
            panelPrincipal.Controls.Add(lblTitulo);
            
            tab.Controls.Add(panelPrincipal);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabCatalogos()
        {
            var tab = new TabPage("Catálogos Clínicos")
            {
                BackColor = Color.White
            };
            
            var tabsCat = new TabControl 
            { 
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Padding = new Point(12, 6),
                ItemSize = new Size(110, 30)
            };

            // Obras Sociales
            tabsCat.TabPages.Add(CrearTabObrasSociales());

            // Tipos de Análisis
            tabsCat.TabPages.Add(CrearTabTiposAnalisis());

            // Métricas
            tabsCat.TabPages.Add(CrearTabMetricas());

            // Relaciones
            tabsCat.TabPages.Add(CrearTabRelacionesTipoAnalisisMetricas());

            tab.Controls.Add(tabsCat);
            tabs.TabPages.Add(tab);
        }

        private TabPage CrearTabObrasSociales()
        {
            var tab = new TabPage("Obras Sociales")
            {
                BackColor = Color.White,
                Padding = new Padding(25)
            };
            
            // Título - MÁS ALTO
            var lblTitulo = new Label
            {
                Text = "Gestión de Obras Sociales y Mutuales",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                Location = new Point(25, 25),
                Size = new Size(1000, 40),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            // Toolbar - MÁS ALTO
            var tool = new ToolStrip
            {
                BackColor = Color.FromArgb(248, 255, 250),
                GripStyle = ToolStripGripStyle.Hidden,
                Padding = new Padding(10, 8, 10, 8),
                Location = new Point(25, 75),
                Width = 1000,
                AutoSize = false,
                Height = 45
            };
            
            var btnNuevo = new ToolStripButton("Nueva Obra Social") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96)
            };
            
            var btnEditar = new ToolStripButton("Modificar") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            
            var btnEliminar = new ToolStripButton("Dar de Baja") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(192, 57, 43)
            };
            
            var lblBuscar = new ToolStripLabel("Buscar:") 
            { 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };
            
            var txtBuscar = new ToolStripTextBox 
            { 
                Width = 220, 
                ToolTipText = "CUIT o Nombre",
                Font = new Font("Segoe UI", 10)
            };
            
            var lblFiltroEstado = new ToolStripLabel("Estado:")
            {
                Margin = new Padding(12, 0, 5, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoObrasSociales = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 110,
                Font = new Font("Segoe UI", 10)
            });
            
            cboFiltroEstadoObrasSociales.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoObrasSociales.SelectedIndex = 0;
            cboFiltroEstadoObrasSociales.SelectedIndexChanged += (s, e) => 
                ObrasSocialesFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoObrasSociales.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar, btnEliminar, 
                new ToolStripSeparator(), 
                lblBuscar, txtBuscar,
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            gridObrasSociales = new DataGridView 
            { 
                Location = new Point(25, 135),
                Size = new Size(1000, 380),
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(39, 174, 96),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(212, 239, 223),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(6)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 255, 250)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 }
            };

            btnNuevo.Click += (s, e) => ObrasSocialesNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => ObrasSocialesEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => ObrasSocialesEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => ObrasSocialesBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            tab.Controls.Add(gridObrasSociales);
            tab.Controls.Add(tool);
            tab.Controls.Add(lblTitulo);
            
            return tab;
        }

        private TabPage CrearTabTiposAnalisis()
        {
            var tab = new TabPage("Tipos de Análisis")
            {
                BackColor = Color.White,
                Padding = new Padding(25)
            };
            
            // Título - MÁS ALTO
            var lblTitulo = new Label
            {
                Text = "Catálogo de Tipos de Análisis Clínicos",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34),
                Location = new Point(25, 25),
                Size = new Size(1000, 40),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            // Toolbar - MÁS ALTO
            var tool = new ToolStrip
            {
                BackColor = Color.FromArgb(255, 250, 245),
                GripStyle = ToolStripGripStyle.Hidden,
                Padding = new Padding(10, 8, 10, 8),
                Location = new Point(25, 75),
                Width = 1000,
                AutoSize = false,
                Height = 45
            };
            
            var btnNuevo = new ToolStripButton("Nuevo Tipo de Análisis") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34)
            };
            
            var btnEditar = new ToolStripButton("Modificar") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            
            var btnEliminar = new ToolStripButton("Dar de Baja") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(192, 57, 43)
            };
            
            var lblBuscar = new ToolStripLabel("Buscar:") 
            { 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };
            
            var txtBuscar = new ToolStripTextBox 
            { 
                Width = 220, 
                ToolTipText = "Descripción del tipo de análisis",
                Font = new Font("Segoe UI", 10)
            };
            
            var lblFiltroEstado = new ToolStripLabel("Estado:")
            {
                Margin = new Padding(12, 0, 5, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoTiposAnalisis = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 110,
                Font = new Font("Segoe UI", 10)
            });
            
            cboFiltroEstadoTiposAnalisis.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoTiposAnalisis.SelectedIndex = 0;
            cboFiltroEstadoTiposAnalisis.SelectedIndexChanged += (s, e) => 
                TiposAnalisisFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoTiposAnalisis.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar, btnEliminar, 
                new ToolStripSeparator(), 
                lblBuscar, txtBuscar,
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            gridTiposAnalisis = new DataGridView 
            { 
                Location = new Point(25, 135),
                Size = new Size(1000, 380),
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(230, 126, 34),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(255, 235, 205),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(6)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(255, 250, 245)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 }
            };

            btnNuevo.Click += (s, e) => TiposAnalisisNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => TiposAnalisisEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => TiposAnalisisEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => TiposAnalisisBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            tab.Controls.Add(gridTiposAnalisis);
            tab.Controls.Add(tool);
            tab.Controls.Add(lblTitulo);
            
            return tab;
        }

        private TabPage CrearTabMetricas()
        {
            var tab = new TabPage("Métricas")
            {
                BackColor = Color.White,
                Padding = new Padding(25)
            };
            
            // Título - MÁS ALTO
            var lblTitulo = new Label
            {
                Text = "Catálogo de Métricas y Parámetros de Laboratorio",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(142, 68, 173),
                Location = new Point(25, 25),
                Size = new Size(1000, 40),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            // Toolbar - MÁS ALTO
            var tool = new ToolStrip
            {
                BackColor = Color.FromArgb(250, 245, 255),
                GripStyle = ToolStripGripStyle.Hidden,
                Padding = new Padding(10, 8, 10, 8),
                Location = new Point(25, 75),
                Width = 1000,
                AutoSize = false,
                Height = 45
            };
            
            var btnNuevo = new ToolStripButton("Nueva Métrica") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(142, 68, 173)
            };
            
            var btnEditar = new ToolStripButton("Modificar") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            
            var btnEliminar = new ToolStripButton("Dar de Baja") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(192, 57, 43)
            };
            
            var lblBuscar = new ToolStripLabel("Buscar:") 
            { 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };
            
            var txtBuscar = new ToolStripTextBox 
            { 
                Width = 220, 
                ToolTipText = "Nombre o unidad de medida",
                Font = new Font("Segoe UI", 10)
            };
            
            var lblFiltroEstado = new ToolStripLabel("Estado:")
            {
                Margin = new Padding(12, 0, 5, 0),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoMetricas = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 110,
                Font = new Font("Segoe UI", 10)
            });
            
            cboFiltroEstadoMetricas.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoMetricas.SelectedIndex = 0;
            cboFiltroEstadoMetricas.SelectedIndexChanged += (s, e) => 
                MetricasFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoMetricas.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar, btnEliminar, 
                new ToolStripSeparator(), 
                lblBuscar, txtBuscar,
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            gridMetricas = new DataGridView 
            { 
                Location = new Point(25, 135),
                Size = new Size(1000, 380),
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(142, 68, 173),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(235, 222, 240),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(6)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 245, 255)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 38 }
            };

            btnNuevo.Click += (s, e) => MetricasNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => MetricasEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => MetricasEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => MetricasBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            tab.Controls.Add(gridMetricas);
            tab.Controls.Add(tool);
            tab.Controls.Add(lblTitulo);
            
            return tab;
        }

        private TabPage CrearTabRelacionesTipoAnalisisMetricas()
        {
            var tab = new TabPage("Configuración")
            {
                BackColor = Color.White,
                Padding = new Padding(30)
            };
            
            // Título principal
            var lblTitulo = new Label
            {
                Text = "Configuración de Relaciones entre Análisis y Métricas",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(41, 128, 185),
                Location = new Point(30, 30),
                Size = new Size(900, 35)
            };

            // Subtítulo descriptivo
            var lblSubtitulo = new Label
            {
                Text = "Gestione qué métricas específicas componen cada tipo de análisis clínico",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(30, 70),
                Size = new Size(900, 25)
            };

            // Panel informativo
            var panelInfo = new Panel
            {
                Location = new Point(30, 110),
                Size = new Size(900, 150),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblInfo = new Label
            {
                Text = "Importancia de las Relaciones:\n\n" +
                       "• Define qué parámetros se pueden medir en cada tipo de análisis\n" +
                       "• Determina qué campos aparecerán al cargar resultados\n" +
                       "• Garantiza consistencia en los informes generados\n\n" +
                       "Las relaciones establecidas son fundamentales para la integridad de los datos clínicos.",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                Location = new Point(20, 15),
                Size = new Size(850, 120),
                BackColor = Color.Transparent
            };

            panelInfo.Controls.Add(lblInfo);

            // Botón de gestión
            var btnGestionar = new Button
            {
                Text = "Gestionar Relaciones Análisis-Métricas",
                Location = new Point(30, 280),
                Size = new Size(380, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGestionar.FlatAppearance.BorderSize = 0;
            btnGestionar.Click += (s, e) => RelacionesTipoAnalisisMetricaGestionarClick?.Invoke(this, EventArgs.Empty);

            // Nota adicional
            var lblNota = new Label
            {
                Text = "Nota: Los cambios en las relaciones solo afectarán a futuros análisis.\n" +
                       "Los análisis existentes mantendrán su configuración original.",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(30, 350),
                Size = new Size(600, 25)
            };
            
            tab.Controls.AddRange(new Control[] { 
                lblTitulo, lblSubtitulo, panelInfo, btnGestionar, lblNota 
            });
            
            return tab;
        }

        private void AgregarTabReportes()
        {
            var tab = new TabPage("Reportes y Análisis")
            {
                BackColor = Color.White
            };
            
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30),
                BackColor = Color.White
            };
            
            // Título principal
            var lblTitulo = new Label
            {
                Text = "Módulo de Reportes Estadísticos y Visualizaciones",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                Location = new Point(0, 0),
                Size = new Size(900, 35)
            };

            // Información
            var grpInfo = new GroupBox
            {
                Text = "  Información de Reportes Disponibles  ",
                Location = new Point(0, 50),
                Size = new Size(900, 160),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                BackColor = Color.FromArgb(248, 255, 250)
            };
            
            var lblDescripcion = new Label
            {
                Text = "Los reportes proporcionan análisis estadísticos y visualizaciones gráficas\n" +
                       "que facilitan la toma de decisiones basadas en datos del laboratorio.\n\n" +
                       "Reportes Disponibles:\n\n" +
                       "• Productividad de Personal Médico (análisis por profesional)\n" +
                       "• Facturación por Obra Social (distribución económica)\n" +
                       "• Demanda de Análisis (top 10 más solicitados)",
                Location = new Point(20, 30),
                Size = new Size(850, 115),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = false,
                BackColor = Color.Transparent
            };
            
            grpInfo.Controls.Add(lblDescripcion);
            
            // Acciones
            var grpAcciones = new GroupBox
            {
                Text = "  Acceder al Módulo de Reportes  ",
                Location = new Point(0, 230),
                Size = new Size(900, 170),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                BackColor = Color.FromArgb(248, 255, 250)
            };
            
            var btnReportes = new Button 
            { 
                Text = "Abrir Módulo de Reportes", 
                Location = new Point(25, 40), 
                Size = new Size(350, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReportes.FlatAppearance.BorderSize = 0;
            btnReportes.Click += (s, e) => ReportesClick?.Invoke(this, EventArgs.Empty);
            
            var lblBeneficios = new Label
            {
                Text = "Beneficios de usar los reportes:\n" +
                       "• Análisis del rendimiento y productividad del personal médico\n" +
                       "• Comprensión de la distribución del trabajo por obra social\n" +
                       "• Identificación de los tipos de análisis más demandados",
                Location = new Point(25, 100),
                Size = new Size(840, 55),
                AutoSize = false,
                ForeColor = Color.FromArgb(52, 73, 94),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.Transparent
            };
            
            grpAcciones.Controls.AddRange(new Control[] { btnReportes, lblBeneficios });
            
            panelPrincipal.Controls.AddRange(new Control[] { 
                lblTitulo, grpInfo, grpAcciones 
            });
            
            tab.Controls.Add(panelPrincipal);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabBackups()
        {
            var tab = new TabPage("Copias de Seguridad")
            {
                BackColor = Color.White
            };
            
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30),
                BackColor = Color.White
            };
            
            // Título principal
            var lblTitulo = new Label
            {
                Text = "Gestión de Copias de Seguridad del Sistema",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(0, 0),
                Size = new Size(900, 35)
            };

            // Información
            var grpInfo = new GroupBox
            {
                Text = "  Información sobre Copias de Seguridad  ",
                Location = new Point(0, 50),
                Size = new Size(900, 170),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                BackColor = Color.FromArgb(245, 250, 255)
            };
            
            var lblDescripcion = new Label
            {
                Text = "Las copias de seguridad son fundamentales para proteger la información del laboratorio.\n\n" +
                       "Recomendaciones:\n" +
                       "• Ejecute backups regularmente según la política establecida\n" +
                       "• El sistema utiliza la carpeta predeterminada de SQL Server con permisos configurados\n" +
                       "• Puede cambiar la ubicación, asegurándose que SQL Server tenga permisos de escritura\n\n" +
                       "Las copias de seguridad garantizan la recuperación de datos ante cualquier eventualidad.",
                Location = new Point(20, 30),
                Size = new Size(850, 125),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = false,
                BackColor = Color.Transparent
            };
            
            grpInfo.Controls.Add(lblDescripcion);
            
            // Acciones
            var grpAcciones = new GroupBox
            {
                Text = "  Acciones de Respaldo Disponibles  ",
                Location = new Point(0, 240),
                Size = new Size(900, 200),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                BackColor = Color.FromArgb(245, 250, 255)
            };
            
            var btnEjecutar = new Button 
            { 
                Text = "Ejecutar Copia de Seguridad", 
                Location = new Point(25, 40), 
                Size = new Size(400, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEjecutar.FlatAppearance.BorderSize = 0;
            
            var btnProbarConexion = new Button 
            { 
                Text = "Probar Conexión a Base de Datos", 
                Location = new Point(445, 40), 
                Size = new Size(400, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = Color.FromArgb(44, 62, 80),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnProbarConexion.FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199);
            
            var lblUbicacion = new Label
            {
                Text = "La ubicación predeterminada tiene los permisos necesarios para SQL Server.\n" +
                       "Los archivos se guardarán en la carpeta de backups del servidor SQL por defecto.",
                Location = new Point(25, 110),
                Size = new Size(840, 40),
                AutoSize = false,
                ForeColor = Color.FromArgb(39, 174, 96),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                BackColor = Color.Transparent
            };

            var lblNota = new Label
            {
                Text = "Importante: Conserve las copias de seguridad en un lugar seguro y externo al servidor.",
                Location = new Point(25, 155),
                Size = new Size(840, 25),
                ForeColor = Color.FromArgb(192, 57, 43),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                BackColor = Color.Transparent
            };
            
            btnEjecutar.Click += (s, e) => EjecutarBackupClick?.Invoke(this, EventArgs.Empty);
            btnProbarConexion.Click += (s, e) => ProbarConexionClick?.Invoke(this, EventArgs.Empty);
            
            grpAcciones.Controls.AddRange(new Control[] { 
                btnEjecutar, btnProbarConexion, lblUbicacion, lblNota 
            });
            
            panelPrincipal.Controls.AddRange(new Control[] { 
                lblTitulo, grpInfo, grpAcciones 
            });
            
            tab.Controls.Add(panelPrincipal);
            tabs.TabPages.Add(tab);
        }

        // Eventos existentes
        public event EventHandler UsuariosNuevoClick;
        public event EventHandler UsuariosEditarClick;
        public event EventHandler UsuariosEliminarClick;
        public event EventHandler<string> UsuariosBuscarTextoChanged;
        public event EventHandler UsuariosDetalleClick;
        public event EventHandler<string> UsuariosFiltroEstadoChanged;
        
        public event EventHandler ObrasSocialesNuevoClick;
        public event EventHandler ObrasSocialesEditarClick;
        public event EventHandler ObrasSocialesEliminarClick;
        public event EventHandler<string> ObrasSocialesBuscarTextoChanged;
        public event EventHandler<string> ObrasSocialesFiltroEstadoChanged;
        
        public event EventHandler TiposAnalisisNuevoClick;
        public event EventHandler TiposAnalisisEditarClick;
        public event EventHandler TiposAnalisisEliminarClick;
        public event EventHandler<string> TiposAnalisisBuscarTextoChanged;
        public event EventHandler<string> TiposAnalisisFiltroEstadoChanged;
        
        public event EventHandler MetricasNuevoClick;
        public event EventHandler MetricasEditarClick;
        public event EventHandler MetricasEliminarClick;
        public event EventHandler<string> MetricasBuscarTextoChanged;
        public event EventHandler<string> MetricasFiltroEstadoChanged;
        
        public event EventHandler RelacionesTipoAnalisisMetricaGestionarClick;
        public event EventHandler ReportesClick;
        public event EventHandler EjecutarBackupClick;
        public event EventHandler ProbarConexionClick;

        // Implementación de métodos de datos
        public void CargarUsuarios(System.Collections.IEnumerable usuarios)
        {
            if (gridUsuarios != null) gridUsuarios.DataSource = usuarios;
        }

        public int? ObtenerUsuarioSeleccionadoDni()
        {
            if (gridUsuarios?.CurrentRow?.DataBoundItem == null) return null;
            var row = gridUsuarios.CurrentRow.DataBoundItem;
            var dniProp = row.GetType().GetProperty("DNI");  // CAMBIADO de "Dni" a "DNI"
            if (dniProp == null) return null;
            var val = dniProp.GetValue(row);
            return val as int? ?? (val != null ? (int?)System.Convert.ToInt32(val) : null);
        }

        public void CargarObrasSociales(System.Collections.IEnumerable obrasSociales)
        {
            if (gridObrasSociales != null) gridObrasSociales.DataSource = obrasSociales;
        }

        public int? ObtenerObraSocialSeleccionadaId()
        {
            if (gridObrasSociales?.CurrentRow?.DataBoundItem == null) return null;
            var row = gridObrasSociales.CurrentRow.DataBoundItem;
            var idProp = row.GetType().GetProperty("IdObraSocial");
            if (idProp == null) return null;
            var val = idProp.GetValue(row);
            return val as int? ?? (val != null ? (int?)System.Convert.ToInt32(val) : null);
        }

        public void CargarTiposAnalisis(System.Collections.IEnumerable tiposAnalisis)
        {
            if (gridTiposAnalisis != null) gridTiposAnalisis.DataSource = tiposAnalisis;
        }

        public int? ObtenerTipoAnalisisSeleccionadoId()
        {
            if (gridTiposAnalisis?.CurrentRow?.DataBoundItem == null) return null;
            var row = gridTiposAnalisis.CurrentRow.DataBoundItem;
            var idProp = row.GetType().GetProperty("IdTipoAnalisis");
            if (idProp == null) return null;
            var val = idProp.GetValue(row);
            return val as int? ?? (val != null ? (int?)System.Convert.ToInt32(val) : null);
        }

        public void CargarMetricas(System.Collections.IEnumerable metricas)
        {
            if (gridMetricas != null) gridMetricas.DataSource = metricas;
        }

        public int? ObtenerMetricaSeleccionadaId()
        {
            if (gridMetricas?.CurrentRow?.DataBoundItem == null) return null;
            var row = gridMetricas.CurrentRow.DataBoundItem;
            var idProp = row.GetType().GetProperty("IdMetrica");
            if (idProp == null) return null;
            var val = idProp.GetValue(row);
            return val as int? ?? (val != null ? (int?)System.Convert.ToInt32(val) : null);
        }

        public void MostrarMensaje(string texto, string titulo = "SALC - Administración", bool esError = false)
        {
            MessageBox.Show(texto, titulo, MessageBoxButtons.OK, esError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }
    }
}
