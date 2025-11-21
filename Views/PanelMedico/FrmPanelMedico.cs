using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using SALC.Presenters.ViewsContracts;
using SALC.Presenters;
using SALC.Views.Compartidos;
using SALC.Domain;

namespace SALC.Views.PanelMedico
{
    public class FrmPanelMedico : Form, IPanelMedicoView
    {
        private TabControl tabs;
        
        // RF-05: Crear análisis
        private Button btnSeleccionarPaciente;
        private Label lblPacienteSeleccionado;
        private ComboBox cboTipoAnalisis;
        private TextBox txtObservacionesCrear;
        private Button btnCrearAnalisis;

        // RF-06: Cargar resultados
        private Button btnSeleccionarAnalisisResultados;
        private Label lblAnalisisResultadosSeleccionado;
        private Button btnCargarMetricas;
        private DataGridView gridResultados;
        private Button btnGuardarResultados;

        // RF-07: Validar/Firmar
        private Button btnSeleccionarAnalisisFirmar;
        private Label lblAnalisisFirmarSeleccionado;
        private DataGridView gridValidacion;
        private Button btnFirmarAnalisis;

        // RF-03: Gestión de Pacientes (Médico)
        private DataGridView gridPacientes;
        private ComboBox cboFiltroEstadoPacientes;

        public FrmPanelMedico()
        {
            Text = "Gestión Médica Clínica";
            Width = 1200;
            Height = 800;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            tabs = new TabControl 
            { 
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Padding = new Point(15, 8),
                ItemSize = new Size(140, 35),
                SizeMode = TabSizeMode.Fixed
            };
            
            Controls.Add(tabs);

            CrearTabGestionPacientes();
            CrearTabCrearAnalisis();
            CrearTabCargarResultados();
            CrearTabValidarFirmar();
        }

        #region Gestión de Pacientes

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

            // Título - IGUAL QUE ADMINISTRADOR
            var lblTitulo = new Label 
            { 
                Text = "Administración de Información de Pacientes", 
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 136),
                Location = new Point(25, 25),
                Size = new Size(1100, 40),
                BackColor = Color.White,
                AutoSize = false
            };

            // Toolbar - IGUAL QUE ADMINISTRADOR (más alto y más abajo)
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
            
            var btnEditar = new ToolStripButton("Modificar") 
            { 
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(0, 150, 136)
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
            cboFiltroEstadoPacientes.SelectedIndexChanged += (s, e) => 
                PacientesFiltroEstadoChanged?.Invoke(this, cboFiltroEstadoPacientes.SelectedItem.ToString());

            tool.Items.AddRange(new ToolStripItem[] { 
                btnEditar, 
                btnEliminar, 
                new ToolStripSeparator(), 
                lblBuscar, 
                txtBuscar,
                lblFiltroEstado, 
                cboFiltroEstadoHost
            });
            
            // Grid - IGUAL QUE ADMINISTRADOR (posición, altura de encabezados y filas)
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
                    BackColor = Color.FromArgb(0, 150, 136),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    WrapMode = DataGridViewTriState.False
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    SelectionBackColor = Color.FromArgb(178, 223, 219),
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
            btnEditar.Click += (s, e) => PacientesEditarClick?.Invoke(this, EventArgs.Empty);
            btnEliminar.Click += (s, e) => PacientesEliminarClick?.Invoke(this, EventArgs.Empty);
            txtBuscar.TextChanged += (s, e) => PacientesBuscarTextoChanged?.Invoke(this, txtBuscar.Text);

            // Agregar controles al panel en orden (grid, tool, título)
            panelPrincipal.Controls.Add(gridPacientes);
            panelPrincipal.Controls.Add(tool);
            panelPrincipal.Controls.Add(lblTitulo);
            
            tab.Controls.Add(panelPrincipal);
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region Crear Análisis

        private void CrearTabCrearAnalisis()
        {
            var tab = new TabPage("Crear Análisis Clínico")
            {
                BackColor = Color.White
            };
            
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.White
            };

            // Título principal
            var lblTitulo = new Label 
            { 
                Text = "Solicitud de Nuevo Análisis Clínico", 
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                Location = new Point(0, 0),
                Size = new Size(900, 35)
            };
            
            // Descripción del proceso
            var lblDescripcion = new Label 
            {
                Text = "Seleccione el paciente y el tipo de análisis para crear una nueva solicitud en estado inicial",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(0, 40),
                Size = new Size(1000, 25)
            };

            // Indicador de flujo con mejor visibilidad
            var panelFlujo = new Panel
            {
                Location = new Point(0, 75),
                Size = new Size(1080, 80),
                BackColor = Color.FromArgb(232, 245, 233),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblFlujoTitulo = new Label
            {
                Text = "Flujo Completo del Proceso Médico:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(27, 94, 32),
                Location = new Point(15, 10),
                Size = new Size(1050, 22),
                BackColor = Color.Transparent
            };

            var lblFlujoDetalle = new Label 
            {
                Text = "1. Crear Análisis  →  2. Cargar Resultados  →  3. Validar y Firmar\n" +
                       "Nota: El personal asistente se encargará posteriormente de generar el informe PDF para el paciente",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(27, 94, 32),
                Location = new Point(15, 35),
                Size = new Size(1050, 40),
                BackColor = Color.Transparent
            };

            panelFlujo.Controls.AddRange(new Control[] { lblFlujoTitulo, lblFlujoDetalle });

            // Paso 1: Selección de paciente
            var gbPaciente = new GroupBox 
            { 
                Text = "  Paso 1: Selección del Paciente  ", 
                Location = new Point(0, 175), 
                Size = new Size(530, 130),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                BackColor = Color.FromArgb(245, 250, 255)
            };
            
            btnSeleccionarPaciente = new Button 
            { 
                Text = "Buscar y Seleccionar Paciente", 
                Location = new Point(20, 35), 
                Size = new Size(240, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSeleccionarPaciente.FlatAppearance.BorderSize = 0;
            
            lblPacienteSeleccionado = new Label 
            { 
                Text = "Ningún paciente seleccionado", 
                Location = new Point(20, 85), 
                Size = new Size(490, 30),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(149, 165, 166),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            gbPaciente.Controls.AddRange(new Control[] { btnSeleccionarPaciente, lblPacienteSeleccionado });

            // Paso 2: Tipo de análisis
            var gbTipo = new GroupBox 
            { 
                Text = "  Paso 2: Tipo de Análisis  ", 
                Location = new Point(550, 175), 
                Size = new Size(530, 130),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34),
                BackColor = Color.FromArgb(255, 250, 245)
            };
            
            var lblTipo = new Label 
            { 
                Text = "Seleccione el tipo:", 
                Location = new Point(20, 40), 
                Size = new Size(140, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };
            
            cboTipoAnalisis = new ComboBox 
            { 
                Location = new Point(20, 70), 
                Size = new Size(490, 30), 
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                DisplayMember = "Descripcion", 
                ValueMember = "IdTipoAnalisis"
            };

            gbTipo.Controls.AddRange(new Control[] { lblTipo, cboTipoAnalisis });

            // Paso 3: Observaciones
            var gbObservaciones = new GroupBox 
            { 
                Text = "  Paso 3: Observaciones Iniciales (Opcional)  ", 
                Location = new Point(0, 325), 
                Size = new Size(1080, 120),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(142, 68, 173),
                BackColor = Color.FromArgb(250, 245, 255)
            };
            
            txtObservacionesCrear = new TextBox 
            { 
                Location = new Point(20, 35), 
                Size = new Size(1040, 70), 
                Multiline = true, 
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            gbObservaciones.Controls.Add(txtObservacionesCrear);

            // Botón crear
            btnCrearAnalisis = new Button 
            { 
                Text = "Crear Análisis Clínico", 
                Location = new Point(880, 465), 
                Size = new Size(200, 50),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnCrearAnalisis.FlatAppearance.BorderSize = 0;

            // Eventos
            btnSeleccionarPaciente.Click += (s, e) => BuscarPacienteCrearClick?.Invoke(this, EventArgs.Empty);
            btnCrearAnalisis.Click += (s, e) => CrearAnalisisClick?.Invoke(this, EventArgs.Empty);

            panelPrincipal.Controls.AddRange(new Control[] { 
                lblTitulo, lblDescripcion, panelFlujo, gbPaciente, gbTipo, gbObservaciones, 
                btnCrearAnalisis
            });
            
            tab.Controls.Add(panelPrincipal);
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region Cargar Resultados

        private void CrearTabCargarResultados()
        {
            var tab = new TabPage("Cargar Resultados")
            {
                BackColor = Color.White
            };
            
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.White,
                AutoScroll = true
            };

            // Título
            var lblTitulo = new Label 
            { 
                Text = "Carga de Resultados de Laboratorio", 
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34),
                Location = new Point(0, 0),
                Size = new Size(700, 35)
            };
            
            var lblDescripcion = new Label 
            {
                Text = "Seleccione un análisis pendiente e ingrese los valores medidos para cada métrica asociada",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(0, 40),
                Size = new Size(900, 25)
            };

            // Nota importante
            var panelNota = new Panel
            {
                Location = new Point(0, 75),
                Size = new Size(1080, 45),
                BackColor = Color.FromArgb(255, 243, 224),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblNota = new Label 
            {
                Text = "Importante: Solo se mostrarán las métricas específicas configuradas para el tipo de análisis seleccionado.\n" +
                       "Los valores deben estar dentro de los rangos establecidos para cada parámetro de laboratorio.",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 81, 0),
                Location = new Point(15, 8),
                Size = new Size(1050, 30),
                BackColor = Color.Transparent
            };

            panelNota.Controls.Add(lblNota);

            // Selección de análisis
            var gbAnalisis = new GroupBox 
            { 
                Text = "  Selección del Análisis  ", 
                Location = new Point(0, 135), 
                Size = new Size(520, 110),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34),
                BackColor = Color.FromArgb(255, 250, 245)
            };
            
            btnSeleccionarAnalisisResultados = new Button 
            { 
                Text = "Buscar Análisis Pendiente", 
                Location = new Point(20, 30), 
                Size = new Size(220, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSeleccionarAnalisisResultados.FlatAppearance.BorderSize = 0;
            
            lblAnalisisResultadosSeleccionado = new Label 
            { 
                Text = "Ningún análisis seleccionado", 
                Location = new Point(20, 73), 
                Size = new Size(480, 27),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(149, 165, 166),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            gbAnalisis.Controls.AddRange(new Control[] { btnSeleccionarAnalisisResultados, lblAnalisisResultadosSeleccionado });

            // Botón cargar métricas
            btnCargarMetricas = new Button 
            { 
                Text = "Cargar Métricas", 
                Location = new Point(550, 165), 
                Size = new Size(240, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnCargarMetricas.FlatAppearance.BorderSize = 0;

            // Botón guardar - POSICIONADO AL LADO DEL BOTÓN CARGAR MÉTRICAS
            btnGuardarResultados = new Button 
            { 
                Text = "Guardar Resultados", 
                Location = new Point(810, 165), 
                Size = new Size(240, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnGuardarResultados.FlatAppearance.BorderSize = 0;

            // Grid de resultados - ALTURA OPTIMIZADA
            var lblGrid = new Label 
            { 
                Text = "Métricas Específicas del Tipo de Análisis:", 
                Location = new Point(0, 220), 
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            gridResultados = new DataGridView 
            { 
                Location = new Point(0, 250), 
                Size = new Size(1080, 320),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(230, 126, 34),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Padding = new Padding(5)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(255, 224, 178),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(5)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(255, 250, 245),
                    Padding = new Padding(5)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 35 }
            };

            // Eventos
            btnSeleccionarAnalisisResultados.Click += (s, e) => BuscarAnalisisResultadosClick?.Invoke(this, EventArgs.Empty);
            btnCargarMetricas.Click += (s, e) => CargarMetricasAnalisisClick?.Invoke(this, EventArgs.Empty);
            btnGuardarResultados.Click += (s, e) => CargarResultadosGuardarClick?.Invoke(this, EventArgs.Empty);

            panelPrincipal.Controls.AddRange(new Control[] { 
                lblTitulo, lblDescripcion, panelNota, gbAnalisis, btnCargarMetricas, 
                btnGuardarResultados, lblGrid, gridResultados
            });
            
            tab.Controls.Add(panelPrincipal);
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region Validar/Firmar

        private void CrearTabValidarFirmar()
        {
            var tab = new TabPage("Validar y Firmar")
            {
                BackColor = Color.White
            };
            
            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.White
            };

            // Título
            var lblTitulo = new Label 
            { 
                Text = "Validación y Firma Profesional del Análisis", 
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(142, 68, 173),
                Location = new Point(0, 0),
                Size = new Size(900, 35)
            };
            
            var lblDescripcion = new Label 
            {
                Text = "Revise cuidadosamente los resultados y proceda a firmar digitalmente el análisis clínico",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(0, 40),
                Size = new Size(1000, 25)
            };

            // Advertencia simplificada
            var panelAdvertencia = new Panel
            {
                Location = new Point(0, 75),
                Size = new Size(1080, 55),
                BackColor = Color.FromArgb(255, 235, 238),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblAdvertencia = new Label 
            {
                Text = "Atención: Una vez firmado digitalmente, el análisis NO podrá modificarse",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(183, 28, 28),
                Location = new Point(15, 15),
                Size = new Size(1050, 25),
                BackColor = Color.Transparent
            };

            panelAdvertencia.Controls.Add(lblAdvertencia);

            // Selección de análisis
            var gbAnalisis = new GroupBox 
            { 
                Text = "  Selección del Análisis para Firmar  ", 
                Location = new Point(0, 145), 
                Size = new Size(720, 100),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(142, 68, 173),
                BackColor = Color.FromArgb(250, 245, 255)
            };
            
            btnSeleccionarAnalisisFirmar = new Button 
            { 
                Text = "Buscar Análisis para Firmar", 
                Location = new Point(20, 30), 
                Size = new Size(240, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(142, 68, 173),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSeleccionarAnalisisFirmar.FlatAppearance.BorderSize = 0;
            
            lblAnalisisFirmarSeleccionado = new Label 
            { 
                Text = "Ningún análisis seleccionado", 
                Location = new Point(270, 30), 
                Size = new Size(430, 35),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(149, 165, 166),
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            gbAnalisis.Controls.AddRange(new Control[] { btnSeleccionarAnalisisFirmar, lblAnalisisFirmarSeleccionado });

            // Botón firmar - posicionado al lado del grupo
            btnFirmarAnalisis = new Button 
            { 
                Text = "Firmar Digitalmente", 
                Location = new Point(740, 160), 
                Size = new Size(340, 70),
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                BackColor = Color.FromArgb(142, 68, 173),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnFirmarAnalisis.FlatAppearance.BorderSize = 0;

            // Grid de validación - ALTURA REDUCIDA PARA NO CORTARSE
            var lblValidacion = new Label 
            { 
                Text = "Revisión de Resultados de Laboratorio:", 
                Location = new Point(0, 255), 
                Size = new Size(500, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            gridValidacion = new DataGridView 
            { 
                Location = new Point(0, 285), 
                Size = new Size(1080, 280),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(142, 68, 173),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Padding = new Padding(5)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(225, 190, 231),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(3)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(250, 245, 255),
                    Padding = new Padding(3)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false
            };

            // Eventos
            btnSeleccionarAnalisisFirmar.Click += (s, e) => BuscarAnalisisFirmarClick?.Invoke(this, EventArgs.Empty);
            btnFirmarAnalisis.Click += (s, e) => FirmarAnalisisClick?.Invoke(this, EventArgs.Empty);

            panelPrincipal.Controls.AddRange(new Control[] { 
                lblTitulo, lblDescripcion, panelAdvertencia, gbAnalisis, btnFirmarAnalisis, lblValidacion, gridValidacion
            });
            
            tab.Controls.Add(panelPrincipal);
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region Eventos e Implementación

        // Eventos - Análisis
        public event EventHandler CrearAnalisisClick;
        public event EventHandler BuscarPacienteCrearClick;
        public event EventHandler CargarResultadosGuardarClick;
        public event EventHandler BuscarAnalisisResultadosClick;
        public event EventHandler CargarMetricasAnalisisClick;
        public event EventHandler FirmarAnalisisClick;
        public event EventHandler BuscarAnalisisFirmarClick;

        // Eventos - Gestión de Pacientes
        public event EventHandler PacientesEditarClick;
        public event EventHandler PacientesEliminarClick;
        public event EventHandler<string> PacientesBuscarTextoChanged;
        public event EventHandler<string> PacientesFiltroEstadoChanged;

        // RF-05: Crear análisis
        public string CrearAnalisisDniPacienteTexto => "";
        public int? TipoAnalisisSeleccionadoId => cboTipoAnalisis?.SelectedValue as int? ?? 
            (cboTipoAnalisis?.SelectedValue != null ? (int?)Convert.ToInt32(cboTipoAnalisis.SelectedValue) : null);
        public string CrearAnalisisObservaciones => txtObservacionesCrear?.Text?.Trim();

        public void CargarTiposAnalisis(IEnumerable<TipoAnalisis> tipos)
        {
            cboTipoAnalisis.DataSource = null;
            cboTipoAnalisis.DataSource = tipos.ToList();
        }

        public void MostrarPacienteSeleccionado(Paciente paciente)
        {
            if (paciente != null)
            {
                lblPacienteSeleccionado.Text = string.Format("Seleccionado: {0} {1} (DNI: {2})", 
                    paciente.Nombre, paciente.Apellido, paciente.Dni);
                lblPacienteSeleccionado.ForeColor = Color.FromArgb(56, 142, 60);
                lblPacienteSeleccionado.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnCrearAnalisis.Enabled = true;
            }
        }

        public void LimpiarPacienteSeleccionado()
        {
            lblPacienteSeleccionado.Text = "Ningún paciente seleccionado";
            lblPacienteSeleccionado.ForeColor = Color.FromArgb(149, 165, 166);
            lblPacienteSeleccionado.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            btnCrearAnalisis.Enabled = false;
            txtObservacionesCrear.Text = "";
        }

        // RF-06: Cargar resultados
        public string AnalisisIdParaResultadosTexto => "";

        public void CargarResultadosParaEdicion(IList<MetricaConResultado> filas)
        {
            gridResultados.DataSource = null;
            gridResultados.DataSource = filas;
            
            if (gridResultados.Columns["Resultado"] != null)
                gridResultados.Columns["Resultado"].ReadOnly = false;
            if (gridResultados.Columns["Observaciones"] != null)
                gridResultados.Columns["Observaciones"].ReadOnly = false;
                
            foreach (DataGridViewColumn col in gridResultados.Columns)
            {
                if (col.Name != "Resultado" && col.Name != "Observaciones")
                    col.ReadOnly = true;
            }

            btnGuardarResultados.Enabled = true;
        }

        public IList<MetricaConResultado> LeerResultadosEditados()
        {
            var lista = new List<MetricaConResultado>();
            foreach (DataGridViewRow row in gridResultados.Rows)
            {
                if (row.DataBoundItem is MetricaConResultado metrica)
                {
                    lista.Add(metrica);
                }
            }
            return lista;
        }

        public void MostrarAnalisisParaResultados(Analisis analisis, Paciente paciente, TipoAnalisis tipo)
        {
            if (analisis != null && paciente != null && tipo != null)
            {
                lblAnalisisResultadosSeleccionado.Text = string.Format(
                    "Seleccionado: ID {0} | Paciente: {1} {2} | Tipo: {3}",
                    analisis.IdAnalisis, paciente.Nombre, paciente.Apellido, tipo.Descripcion);
                lblAnalisisResultadosSeleccionado.ForeColor = Color.FromArgb(230, 81, 0);
                lblAnalisisResultadosSeleccionado.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btnCargarMetricas.Enabled = true;
            }
        }

        public void LimpiarAnalisisParaResultados()
        {
            lblAnalisisResultadosSeleccionado.Text = "Ningún análisis seleccionado";
            lblAnalisisResultadosSeleccionado.ForeColor = Color.FromArgb(149, 165, 166);
            lblAnalisisResultadosSeleccionado.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            btnCargarMetricas.Enabled = false;
            btnGuardarResultados.Enabled = false;
            gridResultados.DataSource = null;
        }

        // RF-07: Validar/Firmar
        public string AnalisisIdParaFirmaTexto => "";

        public void MostrarAnalisisParaFirmar(Analisis analisis, Paciente paciente, TipoAnalisis tipo)
        {
            if (analisis != null && paciente != null && tipo != null)
            {
                lblAnalisisFirmarSeleccionado.Text = string.Format(
                    "Seleccionado: ID {0} | Paciente: {1} {2} | Tipo: {3}",
                    analisis.IdAnalisis, paciente.Nombre, paciente.Apellido, tipo.Descripcion);
                lblAnalisisFirmarSeleccionado.ForeColor = Color.FromArgb(106, 27, 154);
                lblAnalisisFirmarSeleccionado.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                btnFirmarAnalisis.Enabled = true;
            }
        }

        public void LimpiarAnalisisParaFirmar()
        {
            lblAnalisisFirmarSeleccionado.Text = "Ningún análisis seleccionado";
            lblAnalisisFirmarSeleccionado.ForeColor = Color.FromArgb(149, 165, 166);
            lblAnalisisFirmarSeleccionado.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            btnFirmarAnalisis.Enabled = false;
            gridValidacion.DataSource = null;
        }

        public void MostrarResultadosParaValidacion(IList<AnalisisMetrica> resultados)
        {
            var datosValidacion = resultados.Select(r => new {
                IdMetrica = r.IdMetrica,
                Resultado = r.Resultado,
                Observaciones = r.Observaciones ?? "-"
            }).ToList();

            gridValidacion.DataSource = datosValidacion;
        }

        // RF-03: Gestión de Pacientes
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

        // Navegación
        public void ActivarTabResultados()
        {
            tabs.SelectedIndex = 2;
        }

        public void ActivarTabValidacion()
        {
            tabs.SelectedIndex = 3;
        }

        // Mensajes
        public void MostrarMensaje(string texto, bool esError = false)
        {
            MessageBox.Show(this, texto, "SALC - Panel Médico", MessageBoxButtons.OK, 
                esError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        #endregion
    }
}
