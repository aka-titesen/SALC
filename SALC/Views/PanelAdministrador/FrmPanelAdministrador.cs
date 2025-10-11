using System;
using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelAdministrador
{
    public class FrmPanelAdministrador : Form, IPanelAdministradorView
    {
        private TabControl tabs;

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
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };

            btnNuevo.Click += (s, e) => UsuariosNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => UsuariosEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => UsuariosEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => UsuariosBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(grid);
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
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };

            btnNuevo.Click += (s, e) => PacientesNuevoClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => PacientesEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => PacientesEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => PacientesBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            panel.Controls.Add(grid);
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
            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };

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
            btnEjecutar.Click += (s, e) => EjecutarBackupClick?.Invoke(this, EventArgs.Empty);
            btnProgramar.Click += (s, e) => ProgramarBackupClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(btnEjecutar);
            tab.Controls.Add(btnProgramar);
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
    }
}
