using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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

        // RF-08: Generar informe
        private Button btnSeleccionarAnalisisInforme;
        private Label lblAnalisisInformeSeleccionado;
        private Button btnGenerarInforme;

        public FrmPanelMedico()
        {
            Text = "Panel de Médico - Sistema SALC";
            Width = 1200;
            Height = 800;
            StartPosition = FormStartPosition.CenterScreen;

            tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            CrearTabCrearAnalisis();
            CrearTabCargarResultados();
            CrearTabValidarFirmar();
            CrearTabGenerarInforme();
        }

        #region RF-05: Crear Análisis

        private void CrearTabCrearAnalisis()
        {
            var tab = new TabPage("1. Crear Análisis");
            
            // Título y descripción
            var lblTitulo = new Label { 
                Text = "Paso 1: Creación del Análisis (RF-05)", 
                Left = 20, Top = 20, Width = 400, Height = 25,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold)
            };
            
            var lblDescripcion = new Label {
                Text = "Seleccione un paciente y tipo de análisis para crear un nuevo análisis en estado 'Sin verificar'",
                Left = 20, Top = 50, Width = 800, Height = 40,
                ForeColor = System.Drawing.Color.Blue
            };

            // Selección de paciente
            var gbPaciente = new GroupBox { 
                Text = "Seleccionar Paciente", 
                Left = 20, Top = 100, Width = 500, Height = 120 
            };
            
            btnSeleccionarPaciente = new Button { 
                Text = "Seleccionar Paciente...", 
                Left = 20, Top = 30, Width = 160, Height = 35,
                BackColor = System.Drawing.Color.LightBlue
            };
            
            lblPacienteSeleccionado = new Label { 
                Text = "Ningún paciente seleccionado", 
                Left = 20, Top = 75, Width = 450, Height = 35,
                ForeColor = System.Drawing.Color.Gray,
                BorderStyle = BorderStyle.FixedSingle
            };

            gbPaciente.Controls.AddRange(new Control[] { btnSeleccionarPaciente, lblPacienteSeleccionado });

            // Selección de tipo de análisis
            var gbTipo = new GroupBox { 
                Text = "Tipo de Análisis", 
                Left = 540, Top = 100, Width = 400, Height = 120 
            };
            
            var lblTipo = new Label { Text = "Tipo:", Left = 20, Top = 40, Width = 80 };
            cboTipoAnalisis = new ComboBox { 
                Left = 100, Top = 38, Width = 280, 
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Descripcion", 
                ValueMember = "IdTipoAnalisis" 
            };

            gbTipo.Controls.AddRange(new Control[] { lblTipo, cboTipoAnalisis });

            // Observaciones
            var gbObservaciones = new GroupBox { 
                Text = "Observaciones Generales", 
                Left = 20, Top = 240, Width = 920, Height = 100 
            };
            
            txtObservacionesCrear = new TextBox { 
                Left = 20, Top = 30, Width = 880, Height = 50, 
                Multiline = true, ScrollBars = ScrollBars.Vertical 
            };

            gbObservaciones.Controls.Add(txtObservacionesCrear);

            // Botón crear
            btnCrearAnalisis = new Button { 
                Text = "Crear Análisis", 
                Left = 820, Top = 360, Width = 120, Height = 40,
                BackColor = System.Drawing.Color.LightGreen,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold),
                Enabled = false
            };

            // Eventos
            btnSeleccionarPaciente.Click += (s, e) => BuscarPacienteCrearClick?.Invoke(this, EventArgs.Empty);
            btnCrearAnalisis.Click += (s, e) => CrearAnalisisClick?.Invoke(this, EventArgs.Empty);

            tab.Controls.AddRange(new Control[] { 
                lblTitulo, lblDescripcion, gbPaciente, gbTipo, gbObservaciones, btnCrearAnalisis 
            });
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region RF-06: Cargar Resultados

        private void CrearTabCargarResultados()
        {
            var tab = new TabPage("2. Cargar Resultados");
            
            // Título y descripción
            var lblTitulo = new Label { 
                Text = "Paso 2: Carga de Resultados (RF-06)", 
                Left = 20, Top = 20, Width = 400, Height = 25,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold)
            };
            
            var lblDescripcion = new Label {
                Text = "Seleccione un análisis 'Sin verificar' de su autoría y cargue los valores de las métricas",
                Left = 20, Top = 50, Width = 800, Height = 40,
                ForeColor = System.Drawing.Color.Blue
            };

            // Selección de análisis
            var gbAnalisis = new GroupBox { 
                Text = "Seleccionar Análisis", 
                Left = 20, Top = 100, Width = 600, Height = 120 
            };
            
            btnSeleccionarAnalisisResultados = new Button { 
                Text = "Seleccionar Análisis...", 
                Left = 20, Top = 30, Width = 180, Height = 35,
                BackColor = System.Drawing.Color.LightBlue
            };
            
            lblAnalisisResultadosSeleccionado = new Label { 
                Text = "Ningún análisis seleccionado", 
                Left = 20, Top = 75, Width = 550, Height = 35,
                ForeColor = System.Drawing.Color.Gray,
                BorderStyle = BorderStyle.FixedSingle
            };

            gbAnalisis.Controls.AddRange(new Control[] { btnSeleccionarAnalisisResultados, lblAnalisisResultadosSeleccionado });

            // Botón cargar métricas
            btnCargarMetricas = new Button { 
                Text = "Cargar Métricas", 
                Left = 640, Top = 130, Width = 120, Height = 35,
                BackColor = System.Drawing.Color.LightBlue,
                Enabled = false
            };

            // Grid de resultados
            var lblGrid = new Label { 
                Text = "Resultados del Análisis:", 
                Left = 20, Top = 240, Width = 200,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold)
            };

            gridResultados = new DataGridView { 
                Left = 20, Top = 270, Width = 1120, Height = 350,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };

            // Botón guardar
            btnGuardarResultados = new Button { 
                Text = "Guardar Resultados", 
                Left = 1020, Top = 635, Width = 120, Height = 35,
                BackColor = System.Drawing.Color.LightGreen,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold),
                Enabled = false
            };

            // Eventos
            btnSeleccionarAnalisisResultados.Click += (s, e) => BuscarAnalisisResultadosClick?.Invoke(this, EventArgs.Empty);
            btnCargarMetricas.Click += (s, e) => CargarMetricasAnalisisClick?.Invoke(this, EventArgs.Empty);
            btnGuardarResultados.Click += (s, e) => CargarResultadosGuardarClick?.Invoke(this, EventArgs.Empty);

            tab.Controls.AddRange(new Control[] { 
                lblTitulo, lblDescripcion, gbAnalisis, btnCargarMetricas, lblGrid, gridResultados, btnGuardarResultados 
            });
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region RF-07: Validar/Firmar

        private void CrearTabValidarFirmar()
        {
            var tab = new TabPage("3. Validar / Firmar");
            
            // Título y descripción
            var lblTitulo = new Label { 
                Text = "Paso 3: Validación y Firma (RF-07)", 
                Left = 20, Top = 20, Width = 400, Height = 25,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold)
            };
            
            var lblDescripcion = new Label {
                Text = "Seleccione un análisis con resultados cargados y proceda a firmarlo para darle validez clínica",
                Left = 20, Top = 50, Width = 800, Height = 40,
                ForeColor = System.Drawing.Color.Blue
            };

            var lblAdvertencia = new Label {
                Text = "⚠️ ATENCIÓN: Una vez firmado, el análisis no podrá modificarse",
                Left = 20, Top = 90, Width = 800, Height = 20,
                ForeColor = System.Drawing.Color.Red,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold)
            };

            // Selección de análisis
            var gbAnalisis = new GroupBox { 
                Text = "Seleccionar Análisis para Firmar", 
                Left = 20, Top = 130, Width = 600, Height = 120 
            };
            
            btnSeleccionarAnalisisFirmar = new Button { 
                Text = "Seleccionar Análisis...", 
                Left = 20, Top = 30, Width = 180, Height = 35,
                BackColor = System.Drawing.Color.Orange
            };
            
            lblAnalisisFirmarSeleccionado = new Label { 
                Text = "Ningún análisis seleccionado", 
                Left = 20, Top = 75, Width = 550, Height = 35,
                ForeColor = System.Drawing.Color.Gray,
                BorderStyle = BorderStyle.FixedSingle
            };

            gbAnalisis.Controls.AddRange(new Control[] { btnSeleccionarAnalisisFirmar, lblAnalisisFirmarSeleccionado });

            // Grid de validación (solo lectura)
            var lblValidacion = new Label { 
                Text = "Revisión de Resultados:", 
                Left = 20, Top = 270, Width = 200,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold)
            };

            gridValidacion = new DataGridView { 
                Left = 20, Top = 300, Width = 1120, Height = 300,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            // Botón firmar
            btnFirmarAnalisis = new Button { 
                Text = "FIRMAR ANÁLISIS", 
                Left = 1000, Top = 620, Width = 140, Height = 40,
                BackColor = System.Drawing.Color.Orange,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold),
                Enabled = false
            };

            // Eventos
            btnSeleccionarAnalisisFirmar.Click += (s, e) => BuscarAnalisisFirmarClick?.Invoke(this, EventArgs.Empty);
            btnFirmarAnalisis.Click += (s, e) => FirmarAnalisisClick?.Invoke(this, EventArgs.Empty);

            tab.Controls.AddRange(new Control[] { 
                lblTitulo, lblDescripcion, lblAdvertencia, gbAnalisis, lblValidacion, gridValidacion, btnFirmarAnalisis 
            });
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region RF-08: Generar Informe

        private void CrearTabGenerarInforme()
        {
            var tab = new TabPage("4. Generar Informe");
            
            // Título y descripción
            var lblTitulo = new Label { 
                Text = "Paso 4: Generación de Informe (RF-08)", 
                Left = 20, Top = 20, Width = 400, Height = 25,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold)
            };
            
            var lblDescripcion = new Label {
                Text = "Seleccione un análisis verificado (firmado) para generar informe PDF",
                Left = 20, Top = 50, Width = 800, Height = 40,
                ForeColor = System.Drawing.Color.Blue
            };

            // Selección de análisis
            var gbAnalisis = new GroupBox { 
                Text = "Seleccionar Análisis Verificado", 
                Left = 20, Top = 120, Width = 600, Height = 120 
            };
            
            btnSeleccionarAnalisisInforme = new Button { 
                Text = "Seleccionar Análisis...", 
                Left = 20, Top = 30, Width = 180, Height = 35,
                BackColor = System.Drawing.Color.LightCoral
            };
            
            lblAnalisisInformeSeleccionado = new Label { 
                Text = "Ningún análisis seleccionado", 
                Left = 20, Top = 75, Width = 550, Height = 35,
                ForeColor = System.Drawing.Color.Gray,
                BorderStyle = BorderStyle.FixedSingle
            };

            gbAnalisis.Controls.AddRange(new Control[] { btnSeleccionarAnalisisInforme, lblAnalisisInformeSeleccionado });

            // Información del estado
            var lblEstado = new Label {
                Text = "ℹ️ Solo se mostrarán análisis en estado 'Verificado' que usted haya creado",
                Left = 20, Top = 260, Width = 800, Height = 20,
                ForeColor = System.Drawing.Color.Green
            };

            // Botón generar informe
            btnGenerarInforme = new Button { 
                Text = "Generar Informe PDF", 
                Left = 640, Top = 150, Width = 140, Height = 40,
                BackColor = System.Drawing.Color.LightCoral,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold),
                Enabled = false
            };

            // Eventos
            btnSeleccionarAnalisisInforme.Click += (s, e) => BuscarAnalisisInformeClick?.Invoke(this, EventArgs.Empty);
            btnGenerarInforme.Click += (s, e) => GenerarInformeClick?.Invoke(this, EventArgs.Empty);

            tab.Controls.AddRange(new Control[] { 
                lblTitulo, lblDescripcion, gbAnalisis, lblEstado, btnGenerarInforme 
            });
            tabs.TabPages.Add(tab);
        }

        #endregion

        #region Eventos e Implementación de IPanelMedicoView

        // Eventos
        public event EventHandler CrearAnalisisClick;
        public event EventHandler BuscarPacienteCrearClick;
        public event EventHandler CargarResultadosGuardarClick;
        public event EventHandler BuscarAnalisisResultadosClick;
        public event EventHandler CargarMetricasAnalisisClick;
        public event EventHandler FirmarAnalisisClick;
        public event EventHandler BuscarAnalisisFirmarClick;
        public event EventHandler GenerarInformeClick;
        public event EventHandler BuscarAnalisisInformeClick;

        // RF-05: Crear análisis
        public string CrearAnalisisDniPacienteTexto => "";  // Ya no se usa
        public int? TipoAnalisisSeleccionadoId => cboTipoAnalisis?.SelectedValue as int? ?? (cboTipoAnalisis?.SelectedValue != null ? (int?)Convert.ToInt32(cboTipoAnalisis.SelectedValue) : null);
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
                lblPacienteSeleccionado.Text = $"✓ {paciente.Nombre} {paciente.Apellido} (DNI: {paciente.Dni})";
                lblPacienteSeleccionado.ForeColor = System.Drawing.Color.Green;
                btnCrearAnalisis.Enabled = true;
            }
        }

        public void LimpiarPacienteSeleccionado()
        {
            lblPacienteSeleccionado.Text = "Ningún paciente seleccionado";
            lblPacienteSeleccionado.ForeColor = System.Drawing.Color.Gray;
            btnCrearAnalisis.Enabled = false;
            txtObservacionesCrear.Text = "";
        }

        // RF-06: Cargar resultados
        public string AnalisisIdParaResultadosTexto => "";  // Ya no se usa

        public void CargarResultadosParaEdicion(IList<ResultadoEdicionDto> filas)
        {
            gridResultados.DataSource = null;
            gridResultados.DataSource = filas;
            
            // Configurar columnas editables
            if (gridResultados.Columns["Resultado"] != null)
                gridResultados.Columns["Resultado"].ReadOnly = false;
            if (gridResultados.Columns["Observaciones"] != null)
                gridResultados.Columns["Observaciones"].ReadOnly = false;
                
            // Hacer otras columnas solo lectura
            foreach (DataGridViewColumn col in gridResultados.Columns)
            {
                if (col.Name != "Resultado" && col.Name != "Observaciones")
                    col.ReadOnly = true;
            }

            btnGuardarResultados.Enabled = true;
        }

        public IList<ResultadoEdicionDto> LeerResultadosEditados()
        {
            var lista = new List<ResultadoEdicionDto>();
            foreach (DataGridViewRow row in gridResultados.Rows)
            {
                if (row.DataBoundItem is ResultadoEdicionDto dto)
                {
                    lista.Add(dto);
                }
            }
            return lista;
        }

        public void MostrarAnalisisParaResultados(Analisis analisis, Paciente paciente, TipoAnalisis tipo)
        {
            if (analisis != null && paciente != null && tipo != null)
            {
                lblAnalisisResultadosSeleccionado.Text = $"✓ ID: {analisis.IdAnalisis} | Paciente: {paciente.Nombre} {paciente.Apellido} | Tipo: {tipo.Descripcion}";
                lblAnalisisResultadosSeleccionado.ForeColor = System.Drawing.Color.Green;
                btnCargarMetricas.Enabled = true;
            }
        }

        public void LimpiarAnalisisParaResultados()
        {
            lblAnalisisResultadosSeleccionado.Text = "Ningún análisis seleccionado";
            lblAnalisisResultadosSeleccionado.ForeColor = System.Drawing.Color.Gray;
            btnCargarMetricas.Enabled = false;
            btnGuardarResultados.Enabled = false;
            gridResultados.DataSource = null;
        }

        // RF-07: Validar/Firmar
        public string AnalisisIdParaFirmaTexto => "";  // Ya no se usa

        public void MostrarAnalisisParaFirmar(Analisis analisis, Paciente paciente, TipoAnalisis tipo)
        {
            if (analisis != null && paciente != null && tipo != null)
            {
                lblAnalisisFirmarSeleccionado.Text = $"✓ ID: {analisis.IdAnalisis} | Paciente: {paciente.Nombre} {paciente.Apellido} | Tipo: {tipo.Descripcion}";
                lblAnalisisFirmarSeleccionado.ForeColor = System.Drawing.Color.Green;
                btnFirmarAnalisis.Enabled = true;
            }
        }

        public void LimpiarAnalisisParaFirmar()
        {
            lblAnalisisFirmarSeleccionado.Text = "Ningún análisis seleccionado";
            lblAnalisisFirmarSeleccionado.ForeColor = System.Drawing.Color.Gray;
            btnFirmarAnalisis.Enabled = false;
            gridValidacion.DataSource = null;
        }

        public void MostrarResultadosParaValidacion(IList<AnalisisMetrica> resultados)
        {
            // Preparar datos para mostrar en grid de validación
            var datosValidacion = resultados.Select(r => new {
                IdMetrica = r.IdMetrica,
                Resultado = r.Resultado,
                Observaciones = r.Observaciones ?? "-"
            }).ToList();

            gridValidacion.DataSource = datosValidacion;
        }

        // RF-08: Generar informe
        public string AnalisisIdParaInformeTexto => "";  // Ya no se usa

        public void MostrarAnalisisParaInforme(Analisis analisis, Paciente paciente, TipoAnalisis tipo)
        {
            if (analisis != null && paciente != null && tipo != null)
            {
                lblAnalisisInformeSeleccionado.Text = $"✓ ID: {analisis.IdAnalisis} | Paciente: {paciente.Nombre} {paciente.Apellido} | Tipo: {tipo.Descripcion} | VERIFICADO";
                lblAnalisisInformeSeleccionado.ForeColor = System.Drawing.Color.Green;
                btnGenerarInforme.Enabled = true;
            }
        }

        public void LimpiarAnalisisParaInforme()
        {
            lblAnalisisInformeSeleccionado.Text = "Ningún análisis seleccionado";
            lblAnalisisInformeSeleccionado.ForeColor = System.Drawing.Color.Gray;
            btnGenerarInforme.Enabled = false;
        }

        // Navegación
        public void ActivarTabResultados()
        {
            tabs.SelectedIndex = 1; // Tab "Cargar Resultados"
        }

        public void ActivarTabValidacion()
        {
            tabs.SelectedIndex = 2; // Tab "Validar / Firmar"
        }

        // Mensajes
        public void MostrarMensaje(string texto, bool esError = false)
        {
            MessageBox.Show(this, texto, "Panel Médico - SALC", MessageBoxButtons.OK, 
                esError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }

        #endregion
    }
}
