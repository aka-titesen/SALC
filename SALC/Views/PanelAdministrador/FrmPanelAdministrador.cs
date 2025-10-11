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
        private DataGridView gridPacientes;
        private DataGridView gridObrasSociales;
        private DataGridView gridTiposAnalisis;
        private DataGridView gridMetricas;

        public FrmPanelAdministrador()
        {
            Text = "Panel de Administrador";
            Width = 1000;
            Height = 700;

            tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            AgregarTabUsuarios();
            AgregarTabPacientes();
            AgregarTabCatalogos();
            AgregarTabBackups();
        }

        private void AgregarTabUsuarios()
        {
            var tab = new TabPage("Usuarios");
            var tool = new ToolStrip();
            var btnNuevo = new ToolStripButton("Nuevo");
            var btnEditar = new ToolStripButton("Editar");
            var btnEliminar = new ToolStripButton("Eliminar");
            var txtBuscar = new ToolStripTextBox { Width = 200, ToolTipText = "Buscar por DNI/Apellido/Email" };
            tool.Items.AddRange(new ToolStripItem[] { btnNuevo, btnEditar, btnEliminar, new ToolStripSeparator(), new ToolStripLabel("Buscar:"), txtBuscar });
            gridUsuarios = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoGenerateColumns = true };

            btnNuevo.Click += (s, e) => UsuariosNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => UsuariosEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => UsuariosEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => UsuariosBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(gridUsuarios);
            panel.Controls.Add(tool);
            tab.Controls.Add(panel);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabPacientes()
        {
            var tab = new TabPage("Pacientes");
            var tool = new ToolStrip();
            var btnNuevo = new ToolStripButton("Nuevo");
            var btnEditar = new ToolStripButton("Editar");
            var btnEliminar = new ToolStripButton("Eliminar");
            var txtBuscar = new ToolStripTextBox { Width = 200, ToolTipText = "Buscar por DNI/Apellido" };
            tool.Items.AddRange(new ToolStripItem[] { btnNuevo, btnEditar, btnEliminar, new ToolStripSeparator(), new ToolStripLabel("Buscar:"), txtBuscar });
            gridPacientes = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoGenerateColumns = true };

            btnNuevo.Click += (s, e) => PacientesNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => PacientesEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => PacientesEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => PacientesBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(gridPacientes);
            panel.Controls.Add(tool);
            tab.Controls.Add(panel);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabCatalogos()
        {
            var tab = new TabPage("Catálogos");
            var tabsCat = new TabControl { Dock = DockStyle.Fill };

            // Obras Sociales
            tabsCat.TabPages.Add(CrearTabCatalogo("Obras Sociales",
                (s, e) => ObrasSocialesNuevoClick?.Invoke(this, EventArgs.Empty),
                (s, e) => ObrasSocialesEditarClick?.Invoke(this, EventArgs.Empty),
                (s, e) => ObrasSocialesEliminarClick?.Invoke(this, EventArgs.Empty),
                t => ObrasSocialesBuscarTextoChanged?.Invoke(this, t)));

            // Tipos de Análisis
            tabsCat.TabPages.Add(CrearTabCatalogo("Tipos de Análisis",
                (s, e) => TiposAnalisisNuevoClick?.Invoke(this, EventArgs.Empty),
                (s, e) => TiposAnalisisEditarClick?.Invoke(this, EventArgs.Empty),
                (s, e) => TiposAnalisisEliminarClick?.Invoke(this, EventArgs.Empty),
                t => TiposAnalisisBuscarTextoChanged?.Invoke(this, t)));

            // Métricas
            tabsCat.TabPages.Add(CrearTabCatalogo("Métricas",
                (s, e) => MetricasNuevoClick?.Invoke(this, EventArgs.Empty),
                (s, e) => MetricasEditarClick?.Invoke(this, EventArgs.Empty),
                (s, e) => MetricasEliminarClick?.Invoke(this, EventArgs.Empty),
                t => MetricasBuscarTextoChanged?.Invoke(this, t)));

            tab.Controls.Add(tabsCat);
            tabs.TabPages.Add(tab);
        }

        private TabPage CrearTabCatalogo(string titulo,
            EventHandler onNuevo, EventHandler onEditar, EventHandler onEliminar, Action<string> onBuscar)
        {
            var tab = new TabPage(titulo);
            var tool = new ToolStrip();
            var btnNuevo = new ToolStripButton("Nuevo");
            var btnEditar = new ToolStripButton("Editar");
            var btnEliminar = new ToolStripButton("Eliminar");
            var txtBuscar = new ToolStripTextBox { Width = 200, ToolTipText = "Buscar" };
            tool.Items.AddRange(new ToolStripItem[] { btnNuevo, btnEditar, btnEliminar, new ToolStripSeparator(), new ToolStripLabel("Buscar:"), txtBuscar });
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, AutoGenerateColumns = true };

            // Guardar referencia según el título
            if (titulo.StartsWith("Obras")) gridObrasSociales = grid;
            else if (titulo.StartsWith("Tipos")) gridTiposAnalisis = grid;
            else if (titulo.StartsWith("Métricas")) gridMetricas = grid;

            btnNuevo.Click += onNuevo;
            btnEditar.Click += onEditar;
            btnEliminar.Click += onEliminar;
            txtBuscar.TextChanged += (s, e) => onBuscar?.Invoke(txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(grid);
            panel.Controls.Add(tool);
            tab.Controls.Add(panel);
            return tab;
        }

        private void AgregarTabBackups()
        {
            var tab = new TabPage("Backups");
            var btnEjecutar = new Button { Text = "Ejecutar backup ahora", Left = 20, Top = 20, Width = 220 };
            var btnProgramar = new Button { Text = "Programar backup...", Left = 20, Top = 60, Width = 220 };
            var btnProbarConexion = new Button { Text = "Probar conexión a BD", Left = 20, Top = 100, Width = 220 };
            btnEjecutar.Click += (s, e) => EjecutarBackupClick?.Invoke(this, EventArgs.Empty);
            btnProgramar.Click += (s, e) => ProgramarBackupClick?.Invoke(this, EventArgs.Empty);
            btnProbarConexion.Click += (s, e) => ProbarConexionClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(btnEjecutar);
            tab.Controls.Add(btnProgramar);
            tab.Controls.Add(btnProbarConexion);
            tabs.TabPages.Add(tab);
        }

        public event EventHandler UsuariosNuevoClick;
        public event EventHandler UsuariosEditarClick;
        public event EventHandler UsuariosEliminarClick;
        public event EventHandler<string> UsuariosBuscarTextoChanged;
        public event EventHandler PacientesNuevoClick;
        public event EventHandler PacientesEditarClick;
        public event EventHandler PacientesEliminarClick;
        public event EventHandler<string> PacientesBuscarTextoChanged;
        public event EventHandler ObrasSocialesNuevoClick;
        public event EventHandler ObrasSocialesEditarClick;
        public event EventHandler ObrasSocialesEliminarClick;
        public event EventHandler<string> ObrasSocialesBuscarTextoChanged;
        public event EventHandler TiposAnalisisNuevoClick;
        public event EventHandler TiposAnalisisEditarClick;
        public event EventHandler TiposAnalisisEliminarClick;
        public event EventHandler<string> TiposAnalisisBuscarTextoChanged;
        public event EventHandler MetricasNuevoClick;
        public event EventHandler MetricasEditarClick;
        public event EventHandler MetricasEliminarClick;
        public event EventHandler<string> MetricasBuscarTextoChanged;
        public event EventHandler EjecutarBackupClick;
        public event EventHandler ProgramarBackupClick;
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

        public void CargarPacientes(System.Collections.IEnumerable pacientes)
        {
            if (gridPacientes != null) gridPacientes.DataSource = pacientes;
        }

        public int? ObtenerPacienteSeleccionadoDni()
        {
            if (gridPacientes?.CurrentRow?.DataBoundItem == null) return null;
            var row = gridPacientes.CurrentRow.DataBoundItem;
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
