using System;
using System.Windows.Forms;
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

        // Controles adicionales para usuarios, obras sociales, tipos de análisis y métricas
        private ComboBox cboFiltroEstadoUsuarios;
        private ComboBox cboFiltroEstadoObrasSociales;
        private ComboBox cboFiltroEstadoTiposAnalisis;
        private ComboBox cboFiltroEstadoMetricas;

        public FrmPanelAdministrador()
        {
            Text = "Panel de Administrador";
            Width = 1200;
            Height = 700;

            tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            AgregarTabUsuarios();
            AgregarTabCatalogos();
            AgregarTabReportes();
            AgregarTabBackups();
        }

        private void AgregarTabUsuarios()
        {
            var tab = new TabPage("Usuarios");
            var tool = new ToolStrip();
            var btnNuevo = new ToolStripButton("Nuevo") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEditar = new ToolStripButton("Editar") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEliminar = new ToolStripButton("Eliminar") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnDetalle = new ToolStripButton("Ver Detalle") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var txtBuscar = new ToolStripTextBox { Width = 200, ToolTipText = "Buscar por DNI/Apellido/Email" };
            
            // Filtro de estado
            var lblFiltroEstado = new ToolStripLabel("Estado:");
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoUsuarios = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            });
            cboFiltroEstadoUsuarios.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoUsuarios.SelectedIndex = 0;
            cboFiltroEstadoUsuarios.SelectedIndexChanged += (s, e) => UsuariosFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoUsuarios.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar, btnEliminar, btnDetalle,
                new ToolStripSeparator(), 
                new ToolStripLabel("Buscar:"), txtBuscar,
                new ToolStripSeparator(),
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            gridUsuarios = new DataGridView { 
                Dock = DockStyle.Fill, 
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnNuevo.Click += (s, e) => UsuariosNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => UsuariosEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => UsuariosEliminarClick?.Invoke(this, EventArgs.Empty);
            btnDetalle.Click += (s, e) => UsuariosDetalleClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => UsuariosBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(gridUsuarios);
            panel.Controls.Add(tool);
            tab.Controls.Add(panel);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabCatalogos()
        {
            var tab = new TabPage("Catálogos");
            var tabsCat = new TabControl { Dock = DockStyle.Fill };

            // Obras Sociales - con filtro de estado personalizado
            tabsCat.TabPages.Add(CrearTabObrasSociales());

            // Tipos de Análisis - con filtro de estado personalizado
            tabsCat.TabPages.Add(CrearTabTiposAnalisis());

            // Métricas - con filtro de estado personalizado
            tabsCat.TabPages.Add(CrearTabMetricas());

            // Relaciones Tipo Análisis - Métricas
            tabsCat.TabPages.Add(CrearTabRelacionesTipoAnalisisMetricas());

            tab.Controls.Add(tabsCat);
            tabs.TabPages.Add(tab);
        }

        private TabPage CrearTabObrasSociales()
        {
            var tab = new TabPage("Obras Sociales");
            var tool = new ToolStrip();
            var btnNuevo = new ToolStripButton("Nuevo") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEditar = new ToolStripButton("Editar") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEliminar = new ToolStripButton("Eliminar") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var txtBuscar = new ToolStripTextBox { Width = 200, ToolTipText = "Buscar por CUIT/Nombre" };
            
            // Filtro de estado para obras sociales
            var lblFiltroEstado = new ToolStripLabel("Estado:");
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoObrasSociales = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            });
            cboFiltroEstadoObrasSociales.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoObrasSociales.SelectedIndex = 0;
            cboFiltroEstadoObrasSociales.SelectedIndexChanged += (s, e) => ObrasSocialesFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoObrasSociales.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar, btnEliminar, 
                new ToolStripSeparator(), 
                new ToolStripLabel("Buscar:"), txtBuscar,
                new ToolStripSeparator(),
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            gridObrasSociales = new DataGridView { 
                Dock = DockStyle.Fill, 
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnNuevo.Click += (s, e) => ObrasSocialesNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => ObrasSocialesEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => ObrasSocialesEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => ObrasSocialesBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(gridObrasSociales);
            panel.Controls.Add(tool);
            tab.Controls.Add(panel);
            return tab;
        }

        private TabPage CrearTabTiposAnalisis()
        {
            var tab = new TabPage("Tipos de Análisis");
            var tool = new ToolStrip();
            var btnNuevo = new ToolStripButton("Nuevo") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEditar = new ToolStripButton("Editar") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEliminar = new ToolStripButton("Eliminar") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var txtBuscar = new ToolStripTextBox { Width = 200, ToolTipText = "Buscar por descripción" };
            
            // Filtro de estado para tipos de análisis
            var lblFiltroEstado = new ToolStripLabel("Estado:");
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoTiposAnalisis = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            });
            cboFiltroEstadoTiposAnalisis.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoTiposAnalisis.SelectedIndex = 0;
            cboFiltroEstadoTiposAnalisis.SelectedIndexChanged += (s, e) => TiposAnalisisFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoTiposAnalisis.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar, btnEliminar, 
                new ToolStripSeparator(), 
                new ToolStripLabel("Buscar:"), txtBuscar,
                new ToolStripSeparator(),
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            gridTiposAnalisis = new DataGridView { 
                Dock = DockStyle.Fill, 
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnNuevo.Click += (s, e) => TiposAnalisisNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => TiposAnalisisEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => TiposAnalisisEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => TiposAnalisisBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(gridTiposAnalisis);
            panel.Controls.Add(tool);
            tab.Controls.Add(panel);
            return tab;
        }

        private TabPage CrearTabMetricas()
        {
            var tab = new TabPage("Métricas");
            var tool = new ToolStrip();
            var btnNuevo = new ToolStripButton("Nuevo") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEditar = new ToolStripButton("Editar") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEliminar = new ToolStripButton("Eliminar") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var txtBuscar = new ToolStripTextBox { Width = 200, ToolTipText = "Buscar por nombre/unidad" };
            
            // Filtro de estado para métricas
            var lblFiltroEstado = new ToolStripLabel("Estado:");
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoMetricas = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            });
            cboFiltroEstadoMetricas.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoMetricas.SelectedIndex = 0;
            cboFiltroEstadoMetricas.SelectedIndexChanged += (s, e) => MetricasFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoMetricas.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar, btnEliminar, 
                new ToolStripSeparator(), 
                new ToolStripLabel("Buscar:"), txtBuscar,
                new ToolStripSeparator(),
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            gridMetricas = new DataGridView { 
                Dock = DockStyle.Fill, 
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnNuevo.Click += (s, e) => MetricasNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => MetricasEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => MetricasEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => MetricasBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(gridMetricas);
            panel.Controls.Add(tool);
            tab.Controls.Add(panel);
            return tab;
        }

        private TabPage CrearTabRelacionesTipoAnalisisMetricas()
        {
            var tab = new TabPage("Relaciones Análisis-Métricas");
            
            var lblDescripcion = new Label
            {
                Text = "Gestione qué métricas componen cada tipo de análisis.\nLas relaciones definen los valores que se pueden cargar para cada tipo de análisis.",
                Left = 20,
                Top = 20,
                Width = 600,
                Height = 50,
                AutoSize = false
            };

            var btnGestionarRelaciones = new Button
            {
                Text = "Gestionar Relaciones Tipo Análisis - Métricas",
                Left = 20,
                Top = 80,
                Width = 350,
                Height = 30
            };

            btnGestionarRelaciones.Click += (s, e) => RelacionesTipoAnalisisMetricaGestionarClick?.Invoke(this, EventArgs.Empty);

            tab.Controls.Add(lblDescripcion);
            tab.Controls.Add(btnGestionarRelaciones);
            
            return tab;
        }

        private void AgregarTabReportes()
        {
            var tab = new TabPage("Reportes");
            
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new System.Windows.Forms.Padding(20)
            };
            
            var grpInfo = new GroupBox
            {
                Text = "Información de Reportes",
                Left = 20,
                Top = 20,
                Width = 750,
                Height = 130
            };
            
            var lblDescripcion = new Label
            {
                Text = "Los reportes proporcionan análisis estadísticos y visualizaciones gráficas\n" +
                       "para ayudar en la toma de decisiones basadas en datos.\n\n" +
                       "📊 Reportes disponibles:\n" +
                       "   • Productividad de Médicos\n" +
                       "   • Facturación por Obra Social\n" +
                       "   • Demanda de Análisis (Top 10)",
                Left = 15,
                Top = 25,
                Width = 710,
                Height = 90,
                AutoSize = false
            };
            
            grpInfo.Controls.Add(lblDescripcion);
            
            var grpAcciones = new GroupBox
            {
                Text = "Acceder a Reportes",
                Left = 20,
                Top = 170,
                Width = 750,
                Height = 150
            };
            
            var btnReportes = new Button 
            { 
                Text = "📊 Abrir Módulo de Reportes", 
                Left = 20, 
                Top = 35, 
                Width = 320,
                Height = 45,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(0, 153, 51),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReportes.FlatAppearance.BorderSize = 0;
            btnReportes.Click += (s, e) => ReportesClick?.Invoke(this, EventArgs.Empty);
            
            var lblNota = new Label
            {
                Text = "💡 Los reportes permiten analizar:\n" +
                       "   • Rendimiento y productividad del personal médico\n" +
                       "   • Distribución de trabajo por obra social\n" +
                       "   • Tipos de análisis más demandados",
                Left = 20,
                Top = 90,
                Width = 660,
                Height = 50,
                AutoSize = false,
                ForeColor = System.Drawing.Color.FromArgb(64, 64, 64),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            
            grpAcciones.Controls.Add(btnReportes);
            grpAcciones.Controls.Add(lblNota);
            
            panelPrincipal.Controls.Add(grpInfo);
            panelPrincipal.Controls.Add(grpAcciones);
            
            tab.Controls.Add(panelPrincipal);
            
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabBackups()
        {
            var tab = new TabPage("Backups");
            
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new System.Windows.Forms.Padding(20)
            };
            
            var grpInfo = new GroupBox
            {
                Text = "Información de Copias de Seguridad",
                Left = 20,
                Top = 20,
                Width = 750,
                Height = 130
            };
            
            var lblDescripcion = new Label
            {
                Text = "Las copias de seguridad protegen los datos del sistema.\n" +
                       "Se recomienda ejecutar backups regularmente según la política del laboratorio.\n\n" +
                       "💡 El sistema usa la carpeta predeterminada de SQL Server que ya tiene los permisos configurados.\n" +
                       "   Puede cambiar la ubicación, pero asegúrese de que SQL Server tenga permisos de escritura.",
                Left = 15,
                Top = 25,
                Width = 710,
                Height = 90,
                AutoSize = false
            };
            
            grpInfo.Controls.Add(lblDescripcion);
            
            var grpAcciones = new GroupBox
            {
                Text = "Acciones Disponibles",
                Left = 20,
                Top = 170,
                Width = 750,
                Height = 150
            };
            
            var btnEjecutar = new Button 
            { 
                Text = "🔄 Ejecutar Copia de Seguridad Ahora", 
                Left = 20, 
                Top = 35, 
                Width = 320,
                Height = 45,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(0, 122, 204),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEjecutar.FlatAppearance.BorderSize = 0;
            
            var btnProbarConexion = new Button 
            { 
                Text = "🔌 Probar Conexión a Base de Datos", 
                Left = 360, 
                Top = 35, 
                Width = 320,
                Height = 45,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F),
                BackColor = System.Drawing.Color.FromArgb(243, 243, 243),
                ForeColor = System.Drawing.Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnProbarConexion.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(204, 204, 204);
            
            var lblNota = new Label
            {
                Text = "✅ La ubicación predeterminada tiene los permisos necesarios para SQL Server.\n" +
                       "📁 Se guardará en la carpeta de backups de SQL Server por defecto.",
                Left = 20,
                Top = 90,
                Width = 660,
                Height = 45,
                AutoSize = false,
                ForeColor = System.Drawing.Color.FromArgb(0, 100, 0),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F)
            };
            
            btnEjecutar.Click += (s, e) => EjecutarBackupClick?.Invoke(this, EventArgs.Empty);
            btnProbarConexion.Click += (s, e) => ProbarConexionClick?.Invoke(this, EventArgs.Empty);
            
            grpAcciones.Controls.Add(btnEjecutar);
            grpAcciones.Controls.Add(btnProbarConexion);
            grpAcciones.Controls.Add(lblNota);
            
            panelPrincipal.Controls.Add(grpInfo);
            panelPrincipal.Controls.Add(grpAcciones);
            
            tab.Controls.Add(panelPrincipal);
            
            tabs.TabPages.Add(tab);
        }

        // Eventos existentes
        public event EventHandler UsuariosNuevoClick;
        public event EventHandler UsuariosEditarClick;
        public event EventHandler UsuariosEliminarClick;
        public event EventHandler<string> UsuariosBuscarTextoChanged;
        
        // Nuevos eventos para usuarios
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
        
        // Evento para relaciones Tipo Análisis - Métricas
        public event EventHandler RelacionesTipoAnalisisMetricaGestionarClick;
        
        // Evento para reportes
        public event EventHandler ReportesClick;
        
        public event EventHandler EjecutarBackupClick;
        public event EventHandler ProbarConexionClick;

        // Implementación de métodos de datos/selección
        public void CargarUsuarios(System.Collections.IEnumerable usuarios)
        {
            if (gridUsuarios != null) gridUsuarios.DataSource = usuarios;
        }

        public int? ObtenerUsuarioSeleccionadoDni()
        {
            if (gridUsuarios?.CurrentRow?.DataBoundItem == null) return null;
            var row = gridUsuarios.CurrentRow.DataBoundItem;
            var dniProp = row.GetType().GetProperty("Dni");
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

        public void MostrarMensaje(string texto, string titulo = "SALC", bool esError = false)
        {
            MessageBox.Show(texto, titulo, MessageBoxButtons.OK, esError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }
    }
}
