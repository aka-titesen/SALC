using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SALC.Domain;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelAsistente
{
    public partial class FrmGestionPacientes : Form, IGestionPacientesAsistenteView
    {
        private TabControl tabs;
        
        // RF-03: Gesti�n de Pacientes (Asistente)
        private DataGridView gridPacientes;
        private ComboBox cboFiltroEstadoPacientes;
        private ToolStripTextBox txtBuscarTool;

        #region Eventos de la interfaz
        public event EventHandler BuscarPacientesClick;
        public event EventHandler NuevoPacienteClick;
        public event EventHandler EditarPacienteClick;
        public event EventHandler RefrescarClick;
        public event EventHandler VistaInicializada;
        #endregion

        #region Propiedades de la interfaz
        public string TextoBusqueda => txtBuscarTool?.Text?.Trim() ?? "";

        public Paciente PacienteSeleccionado
        {
            get
            {
                if (gridPacientes?.CurrentRow?.DataBoundItem == null)
                    return null;

                var row = gridPacientes.CurrentRow.DataBoundItem;
                var dniProp = row.GetType().GetProperty("Dni");
                if (dniProp == null) return null;
                var val = dniProp.GetValue(row);
                var dni = val as int? ?? (val != null ? (int?)Convert.ToInt32(val) : null);
                
                if (!dni.HasValue) return null;
                
                // Buscar el paciente original en la fuente de datos
                var dataSource = gridPacientes.Tag as List<Paciente>;
                return dataSource?.FirstOrDefault(p => p.Dni == dni.Value);
            }
        }
        #endregion

        public FrmGestionPacientes()
        {
            Text = "Panel de Asistente - Sistema SALC";
            Width = 1200;
            Height = 800;
            StartPosition = FormStartPosition.CenterScreen;

            tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            CrearTabGestionPacientes(); // EXACTAMENTE IGUAL AL M�DICO
            
            // Evento cuando se carga el formulario
            Load += (s, e) => VistaInicializada?.Invoke(this, EventArgs.Empty);
        }

        #region RF-03: Gesti�n de Pacientes (Asistente) - COPIA EXACTA DEL M�DICO

        private void CrearTabGestionPacientes()
        {
            var tab = new TabPage("Gesti�n de Pacientes");
            
            // T�tulo y descripci�n - EXACTO AL M�DICO
            var lblTitulo = new Label { 
                Text = "Gesti�n de Pacientes (RF-03) - Rol: Asistente", 
                Left = 20, Top = 20, Width = 500, Height = 25,
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold)
            };
            
            var lblDescripcion = new Label {
                Text = "Como asistente, puede crear nuevos pacientes y modificar datos de pacientes existentes. No puede dar de baja pacientes.",
                Left = 20, Top = 50, Width = 800, Height = 40,
                ForeColor = Color.Blue
            };

            // Toolbar - EXACTO AL M�DICO
            var tool = new ToolStrip();
            var btnNuevo = new ToolStripButton("Nuevo Paciente") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            var btnEditar = new ToolStripButton("Modificar Paciente") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            txtBuscarTool = new ToolStripTextBox { Width = 200, ToolTipText = "Buscar por DNI/Apellido/Nombre" };
            
            // Filtro de estado para pacientes - EXACTO AL M�DICO
            var lblFiltroEstado = new ToolStripLabel("Estado:");
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoPacientes = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 100
            });
            cboFiltroEstadoPacientes.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoPacientes.SelectedIndex = 0;
            cboFiltroEstadoPacientes.SelectedIndexChanged += (s, e) => BuscarPacientesClick?.Invoke(this, EventArgs.Empty);

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo, btnEditar,
                new ToolStripSeparator(), 
                new ToolStripLabel("Buscar:"), txtBuscarTool,
                new ToolStripSeparator(),
                lblFiltroEstado, cboFiltroEstadoHost
            });
            
            // Grid de pacientes - EXACTO AL M�DICO
            gridPacientes = new DataGridView { 
                Left = 20, Top = 120, Width = 1120, Height = 480,
                ReadOnly = true, 
                AllowUserToAddRows = false, 
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, 
                MultiSelect = false, 
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Informaci�n adicional - ADAPTADO PARA ASISTENTE
            var lblInfo = new Label {
                Text = "?? Permisos de Asistente:\n" +
                       "� ? Crear nuevos pacientes (autom�ticamente en estado 'Activo')\n" +
                       "� ? Modificar datos de pacientes existentes\n" +
                       "� ? No puede dar de baja l�gica (cambiar estado a 'Inactivo')\n" +
                       "� Para dar de baja pacientes, consulte a un M�dico",
                Left = 20, Top = 620, Width = 500, Height = 80,
                ForeColor = Color.DarkGreen
            };

            // Eventos - EXACTO AL M�DICO
            btnNuevo.Click += (s, e) => NuevoPacienteClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => EditarPacienteClick?.Invoke(this, EventArgs.Empty);
            txtBuscarTool.TextChanged += (s, e) => BuscarPacientesClick?.Invoke(this, EventArgs.Empty);

            // Manejo de selecci�n
            gridPacientes.SelectionChanged += (s, e) => {
                var tieneSeleccion = PacienteSeleccionado != null;
                btnEditar.Enabled = tieneSeleccion;
            };

            // Doble click para editar
            gridPacientes.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0 && PacienteSeleccionado != null)
                {
                    EditarPacienteClick?.Invoke(this, EventArgs.Empty);
                }
            };

            // Layout - EXACTO AL M�DICO
            var panel = new Panel { Dock = DockStyle.Fill };
            tool.Dock = DockStyle.Top;
            tool.Top = 100;
            panel.Controls.Add(gridPacientes);
            panel.Controls.Add(tool);
            
            tab.Controls.AddRange(new Control[] { 
                lblTitulo, lblDescripcion, panel, lblInfo
            });
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region Implementaci�n de la interfaz

        // M�todo para cargar pacientes - IGUAL AL M�DICO
        public void CargarPacientes(System.Collections.IEnumerable pacientes)
        {
            if (gridPacientes != null) 
            {
                var listaPacientes = pacientes?.Cast<Paciente>()?.ToList() ?? new List<Paciente>();
                
                // Aplicar filtro de estado
                var estadoFiltro = cboFiltroEstadoPacientes?.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(estadoFiltro) && estadoFiltro != "Todos")
                {
                    listaPacientes = listaPacientes.Where(p => p.Estado == estadoFiltro).ToList();
                }

                // Aplicar filtro de b�squeda
                if (!string.IsNullOrWhiteSpace(TextoBusqueda))
                {
                    var filtro = TextoBusqueda.ToLowerInvariant();
                    listaPacientes = listaPacientes.Where(p =>
                        p.Dni.ToString().Contains(filtro) ||
                        p.Nombre.ToLowerInvariant().Contains(filtro) ||
                        p.Apellido.ToLowerInvariant().Contains(filtro) ||
                        (p.Email?.ToLowerInvariant().Contains(filtro) ?? false)
                    ).ToList();
                }

                gridPacientes.DataSource = listaPacientes;
                gridPacientes.Tag = listaPacientes; // Guardar para obtener objetos completos
            }
        }

        public void CargarListaPacientes(IEnumerable<Paciente> pacientes)
        {
            CargarPacientes(pacientes);
        }

        // M�todo para obtener DNI del paciente seleccionado - IGUAL AL M�DICO
        public int? ObtenerPacienteSeleccionadoDni()
        {
            if (gridPacientes?.CurrentRow?.DataBoundItem == null) return null;
            var row = gridPacientes.CurrentRow.DataBoundItem;
            var dniProp = row.GetType().GetProperty("Dni");
            if (dniProp == null) return null;
            var val = dniProp.GetValue(row);
            return val as int? ?? (val != null ? (int?)Convert.ToInt32(val) : null);
        }

        public void HabilitarAcciones(bool habilitar)
        {
            // Los botones se manejan autom�ticamente con la selecci�n
        }

        public void HabilitarEdicion(bool habilitar)
        {
            // Los botones se manejan autom�ticamente
        }

        public void MostrarMensaje(string mensaje, bool esError = false)
        {
            MessageBox.Show(this, mensaje, "Panel Asistente - SALC", MessageBoxButtons.OK, 
                esError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        public void MostrarCargando(bool cargando)
        {
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
            Enabled = !cargando;
        }

        public void ActualizarContadores(int totalPacientes, int pacientesActivos)
        {
            // Actualizar el t�tulo de la pesta�a con informaci�n
            var inactivos = totalPacientes - pacientesActivos;
            tabs.TabPages[0].Text = $"Gesti�n de Pacientes (Total: {totalPacientes}, Activos: {pacientesActivos})";
        }

        #endregion
    }
}