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
        
        // RF-03: Gestión de Pacientes (Asistente)
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
            BackColor = Color.White;

            tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            CrearTabGestionPacientes();
            
            // Evento cuando se carga el formulario
            Load += (s, e) => VistaInicializada?.Invoke(this, EventArgs.Empty);
        }

        #region RF-03: Gestión de Pacientes (Asistente) - ESTILO IGUAL AL MÉDICO

        private void CrearTabGestionPacientes()
        {
            var tab = new TabPage("Gestión de Pacientes")
            {
                BackColor = Color.White
            };
            
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(25)
            };

            // Título - IGUAL QUE MÉDICO
            var lblTitulo = new Label 
            { 
                Text = "Administración de Información de Pacientes", 
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(25, 25),
                Size = new Size(1100, 40),
                BackColor = Color.White,
                AutoSize = false
            };

            // Toolbar - IGUAL QUE MÉDICO (más alto y más abajo)
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
            
            var btnNuevo = new ToolStripButton("Nuevo Paciente") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96)
            };
            
            var btnEditar = new ToolStripButton("Modificar") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 130, 180)
            };
            
            var lblBuscar = new ToolStripLabel("Buscar:") 
            { 
                Font = new Font("Segoe UI", 10, FontStyle.Bold) 
            };
            
            txtBuscarTool = new ToolStripTextBox 
            { 
                Width = 240, 
                ToolTipText = "DNI, Apellido o Nombre",
                Font = new Font("Segoe UI", 10)
            };
            
            var lblFiltroEstado = new ToolStripLabel("Filtrar por Estado:")
            {
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(15, 0, 5, 0)
            };
            
            var cboFiltroEstadoHost = new ToolStripControlHost(cboFiltroEstadoPacientes = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 120,
                Font = new Font("Segoe UI", 10)
            });
            
            cboFiltroEstadoPacientes.Items.AddRange(new object[] { "Todos", "Activo", "Inactivo" });
            cboFiltroEstadoPacientes.SelectedIndex = 0;
            cboFiltroEstadoPacientes.SelectedIndexChanged += (s, e) => BuscarPacientesClick?.Invoke(this, EventArgs.Empty);

            tool.Items.AddRange(new ToolStripItem[] { 
                btnNuevo,
                new ToolStripSeparator(),
                btnEditar, 
                new ToolStripSeparator(), 
                lblBuscar, 
                txtBuscarTool,
                lblFiltroEstado, 
                cboFiltroEstadoHost
            });
            
            // Grid - IGUAL QUE MÉDICO (posición, altura de encabezados y filas)
            gridPacientes = new DataGridView 
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
                    BackColor = Color.FromArgb(70, 130, 180),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(176, 196, 222),
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
            btnNuevo.Click += (s, e) => NuevoPacienteClick?.Invoke(this, EventArgs.Empty);
            btnEditar.Click += (s, e) => EditarPacienteClick?.Invoke(this, EventArgs.Empty);
            txtBuscarTool.TextChanged += (s, e) => BuscarPacientesClick?.Invoke(this, EventArgs.Empty);

            // Manejo de selección
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

            // Agregar controles al panel en orden (grid, tool, título)
            panelPrincipal.Controls.Add(gridPacientes);
            panelPrincipal.Controls.Add(tool);
            panelPrincipal.Controls.Add(lblTitulo);
            
            tab.Controls.Add(panelPrincipal);
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region Implementación de la interfaz

        // Método para cargar pacientes
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

                // Aplicar filtro de búsqueda
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
            // Los botones se manejan automáticamente con la selección
        }

        public void HabilitarEdicion(bool habilitar)
        {
            // Los botones se manejan automáticamente
        }

        public void MostrarMensaje(string mensaje, bool esError = false)
        {
            MessageBox.Show(this, mensaje, "SALC - Panel Asistente", MessageBoxButtons.OK, 
                esError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        public void MostrarCargando(bool cargando)
        {
            Cursor = cargando ? Cursors.WaitCursor : Cursors.Default;
            Enabled = !cargando;
        }

        public void ActualizarContadores(int totalPacientes, int pacientesActivos)
        {
            // Actualizar el título de la pestaña con información
            var inactivos = totalPacientes - pacientesActivos;
            tabs.TabPages[0].Text = $"Gestión de Pacientes (Total: {totalPacientes}, Activos: {pacientesActivos})";
        }

        #endregion
    }
}