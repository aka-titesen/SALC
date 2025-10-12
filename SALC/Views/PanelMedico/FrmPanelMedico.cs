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
        private TextBox txtDniPaciente;
        private Button btnBuscarPaciente;
        private Label lblPacienteInfo;
        private ComboBox cboTipoAnalisis;
        private TextBox txtObservacionesCrear;
        private Button btnCrearAnalisis;

        // RF-06: Cargar resultados
        private TextBox txtIdAnalisisResultados;
        private Button btnBuscarAnalisisResultados;
        private Label lblAnalisisResultadosInfo;
        private Button btnCargarMetricas;
        private DataGridView gridResultados;
        private Button btnGuardarResultados;

        // RF-07: Validar/Firmar
        private TextBox txtIdAnalisisFirmar;
        private Button btnBuscarAnalisisFirmar;
        private Label lblAnalisisFirmarInfo;
        private DataGridView gridValidacion;
        private Button btnFirmarAnalisis;

        // RF-08: Generar informe
        private TextBox txtIdAnalisisInforme;
        private Button btnBuscarAnalisisInforme;
        private Label lblAnalisisInformeInfo;
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
                Text = "Selecciona un paciente y tipo de análisis para crear un nuevo análisis en estado 'Sin verificar'",
                Left = 20, Top = 50, Width = 800, Height = 40,
                ForeColor = System.Drawing.Color.Blue
            };

            // Selección de paciente
            var gbPaciente = new GroupBox { 
                Text = "Seleccionar Paciente", 
                Left = 20, Top = 100, Width = 400, Height = 120 
            };
            
            var lblDni = new Label { Text = "DNI Paciente:", Left = 20, Top = 30, Width = 100 };
            txtDniPaciente = new TextBox { Left = 120, Top = 28, Width = 120 };
            btnBuscarPaciente = new Button { Text = "Buscar", Left = 250, Top = 26, Width = 80 };
            lblPacienteInfo = new Label { 
                Text = "Busque un paciente por DNI", 
                Left = 20, Top = 60, Width = 350, Height = 40,
                ForeColor = System.Drawing.Color.Gray
            };

            gbPaciente.Controls.AddRange(new Control[] { lblDni, txtDniPaciente, btnBuscarPaciente, lblPacienteInfo });

            // Selección de tipo de análisis
            var gbTipo = new GroupBox { 
                Text = "Tipo de Análisis", 
                Left = 440, Top = 100, Width = 400, Height = 120 
            };
            
            var lblTipo = new Label { Text = "Tipo:", Left = 20, Top = 30, Width = 80 };
            cboTipoAnalisis = new ComboBox { 
                Left = 100, Top = 28, Width = 280, 
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Descripcion", 
                ValueMember = "IdTipoAnalisis" 
            };

            gbTipo.Controls.AddRange(new Control[] { lblTipo, cboTipoAnalisis });

            // Observaciones
            var gbObservaciones = new GroupBox { 
                Text = "Observaciones Generales", 
                Left = 20, Top = 240, Width = 820, Height = 100 
            };
            
            txtObservacionesCrear = new TextBox { 
                Left = 20, Top = 30, Width = 780, Height = 50, 
                Multiline = true, ScrollBars = ScrollBars.Vertical 
            };

            gbObservaciones.Controls.Add(txtObservacionesCrear);

            // Botón crear
            btnCrearAnalisis = new Button { 
                Text = "Crear Análisis", 
                Left = 740, Top = 360, Width = 120, Height = 35,
                BackColor = System.Drawing.Color.LightGreen,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold)
            };

            // Eventos
            btnBuscarPaciente.Click += (s, e) => BuscarPacienteCrearClick?.Invoke(this, EventArgs.Empty);
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
                Text = "Busque un análisis 'Sin verificar' de su autoría y cargue los valores de las métricas",
                Left = 20, Top = 50, Width = 800, Height = 40,
                ForeColor = System.Drawing.Color.Blue
            };

            // Selección de análisis
            var gbAnalisis = new GroupBox { 
                Text = "Buscar Análisis", 
                Left = 20, Top = 100, Width = 500, Height = 120 
            };
            
            var lblId = new Label { Text = "ID Análisis:", Left = 20, Top = 30, Width = 100 };
            txtIdAnalisisResultados = new TextBox { Left = 120, Top = 28, Width = 120 };
            btnBuscarAnalisisResultados = new Button { Text = "Buscar", Left = 250, Top = 26, Width = 80 };
            lblAnalisisResultadosInfo = new Label { 
                Text = "Ingrese el ID del análisis a cargar", 
                Left = 20, Top = 60, Width = 450, Height = 40,
                ForeColor = System.Drawing.Color.Gray
            };

            gbAnalisis.Controls.AddRange(new Control[] { lblId, txtIdAnalisisResultados, btnBuscarAnalisisResultados, lblAnalisisResultadosInfo });

            // Botón cargar métricas
            btnCargarMetricas = new Button { 
                Text = "Cargar Métricas", 
                Left = 540, Top = 130, Width = 120, Height = 35,
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
            btnBuscarAnalisisResultados.Click += (s, e) => BuscarAnalisisResultadosClick?.Invoke(this, EventArgs.Empty);
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
                Text = "Busque un análisis con resultados cargados y proceda a firmarlo para darle validez clínica",
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
                Text = "Buscar Análisis para Firmar", 
                Left = 20, Top = 130, Width = 500, Height = 120 
            };
            
            var lblId = new Label { Text = "ID Análisis:", Left = 20, Top = 30, Width = 100 };
            txtIdAnalisisFirmar = new TextBox { Left = 120, Top = 28, Width = 120 };
            btnBuscarAnalisisFirmar = new Button { Text = "Buscar", Left = 250, Top = 26, Width = 80 };
            lblAnalisisFirmarInfo = new Label { 
                Text = "Ingrese el ID del análisis a firmar", 
                Left = 20, Top = 60, Width = 450, Height = 40,
                ForeColor = System.Drawing.Color.Gray
            };

            gbAnalisis.Controls.AddRange(new Control[] { lblId, txtIdAnalisisFirmar, btnBuscarAnalisisFirmar, lblAnalisisFirmarInfo });

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
            btnBuscarAnalisisFirmar.Click += (s, e) => BuscarAnalisisFirmarClick?.Invoke(this, EventArgs.Empty);
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
                Text = "Solo análisis verificados (firmados) pueden generar informe PDF",
                Left = 20, Top = 50, Width = 800, Height = 40,
                ForeColor = System.Drawing.Color.Blue
            };

            // Selección de análisis
            var gbAnalisis = new GroupBox { 
                Text = "Buscar Análisis Verificado", 
                Left = 20, Top = 120, Width = 500, Height = 120 
            };
            
            var lblId = new Label { Text = "ID Análisis:", Left = 20, Top = 30, Width = 100 };
            txtIdAnalisisInforme = new TextBox { Left = 120, Top = 28, Width = 120 };
            btnBuscarAnalisisInforme = new Button { Text = "Buscar", Left = 250, Top = 26, Width = 80 };
            lblAnalisisInformeInfo = new Label { 
                Text = "Ingrese el ID del análisis verificado", 
                Left = 20, Top = 60, Width = 450, Height = 40,
                ForeColor = System.Drawing.Color.Gray
            };

            gbAnalisis.Controls.AddRange(new Control[] { lblId, txtIdAnalisisInforme, btnBuscarAnalisisInforme, lblAnalisisInformeInfo });

            // Información del estado
            var lblEstado = new Label {
                Text = "ℹ️ Solo se mostrarán análisis en estado 'Verificado' que usted haya creado",
                Left = 20, Top = 260, Width = 800, Height = 20,
                ForeColor = System.Drawing.Color.Green
            };

            // Botón generar informe
            btnGenerarInforme = new Button { 
                Text = "Generar Informe PDF", 
                Left = 540, Top = 150, Width = 140, Height = 40,
                BackColor = System.Drawing.Color.LightCoral,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold),
                Enabled = false
            };

            // Eventos
            btnBuscarAnalisisInforme.Click += (s, e) => BuscarAnalisisInformeClick?.Invoke(this, EventArgs.Empty);
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
        public string CrearAnalisisDniPacienteTexto => txtDniPaciente?.Text?.Trim();
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
                lblPacienteInfo.Text = $"✓ {paciente.Nombre} {paciente.Apellido}\nDNI: {paciente.Dni}";
                lblPacienteInfo.ForeColor = System.Drawing.Color.Green;
                btnCrearAnalisis.Enabled = true;
            }
        }

        public void LimpiarPacienteSeleccionado()
        {
            lblPacienteInfo.Text = "Busque un paciente por DNI";
            lblPacienteInfo.ForeColor = System.Drawing.Color.Gray;
            btnCrearAnalisis.Enabled = false;
            txtDniPaciente.Text = "";
            txtObservacionesCrear.Text = "";
        }

        // RF-06: Cargar resultados
        public string AnalisisIdParaResultadosTexto => txtIdAnalisisResultados?.Text?.Trim();

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
                lblAnalisisResultadosInfo.Text = $"✓ Análisis ID: {analisis.IdAnalisis}\n" +
                                               $"Paciente: {paciente.Nombre} {paciente.Apellido}\n" +
                                               $"Tipo: {tipo.Descripcion}";
                lblAnalisisResultadosInfo.ForeColor = System.Drawing.Color.Green;
                btnCargarMetricas.Enabled = true;
            }
        }

        public void LimpiarAnalisisParaResultados()
        {
            lblAnalisisResultadosInfo.Text = "Ingrese el ID del análisis a cargar";
            lblAnalisisResultadosInfo.ForeColor = System.Drawing.Color.Gray;
            btnCargarMetricas.Enabled = false;
            btnGuardarResultados.Enabled = false;
            gridResultados.DataSource = null;
            txtIdAnalisisResultados.Text = "";
        }

        // RF-07: Validar/Firmar
        public string AnalisisIdParaFirmaTexto => txtIdAnalisisFirmar?.Text?.Trim();

        public void MostrarAnalisisParaFirmar(Analisis analisis, Paciente paciente, TipoAnalisis tipo)
        {
            if (analisis != null && paciente != null && tipo != null)
            {
                lblAnalisisFirmarInfo.Text = $"✓ Análisis ID: {analisis.IdAnalisis}\n" +
                                           $"Paciente: {paciente.Nombre} {paciente.Apellido}\n" +
                                           $"Tipo: {tipo.Descripcion}\n" +
                                           $"Creado: {analisis.FechaCreacion:dd/MM/yyyy HH:mm}";
                lblAnalisisFirmarInfo.ForeColor = System.Drawing.Color.Green;
                btnFirmarAnalisis.Enabled = true;
            }
        }

        public void LimpiarAnalisisParaFirmar()
        {
            lblAnalisisFirmarInfo.Text = "Ingrese el ID del análisis a firmar";
            lblAnalisisFirmarInfo.ForeColor = System.Drawing.Color.Gray;
            btnFirmarAnalisis.Enabled = false;
            gridValidacion.DataSource = null;
            txtIdAnalisisFirmar.Text = "";
        }

        public void MostrarResultadosParaValidacion(IList<AnalisisMetrica> resultados)
        {
            // Preparar datos para mostrar en grid de validación
            var datosValidacion = resultados.Select(r => new {
                Métrica = r.IdMetrica, // TODO: obtener nombre de métrica
                Resultado = r.Resultado,
                Observaciones = r.Observaciones ?? "-"
            }).ToList();

            gridValidacion.DataSource = datosValidacion;
        }

        // RF-08: Generar informe
        public string AnalisisIdParaInformeTexto => txtIdAnalisisInforme?.Text?.Trim();

        public void MostrarAnalisisParaInforme(Analisis analisis, Paciente paciente, TipoAnalisis tipo)
        {
            if (analisis != null && paciente != null && tipo != null)
            {
                lblAnalisisInformeInfo.Text = $"✓ Análisis ID: {analisis.IdAnalisis} - VERIFICADO\n" +
                                            $"Paciente: {paciente.Nombre} {paciente.Apellido}\n" +
                                            $"Tipo: {tipo.Descripcion}\n" +
                                            $"Firmado: {analisis.FechaFirma:dd/MM/yyyy HH:mm}";
                lblAnalisisInformeInfo.ForeColor = System.Drawing.Color.Green;
                btnGenerarInforme.Enabled = true;
            }
        }

        public void LimpiarAnalisisParaInforme()
        {
            lblAnalisisInformeInfo.Text = "Ingrese el ID del análisis verificado";
            lblAnalisisInformeInfo.ForeColor = System.Drawing.Color.Gray;
            btnGenerarInforme.Enabled = false;
            txtIdAnalisisInforme.Text = "";
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
