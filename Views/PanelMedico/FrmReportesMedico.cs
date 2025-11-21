using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SALC.Presenters;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelMedico
{
    public partial class FrmReportesMedico : Form, IReportesMedicoView
    {
        private ReportesMedicoPresenter _presenter;
        private readonly int _dniMedico;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Button btnAlertas;
        private Button btnCargaTrabajo;
        private DataGridView gridAlertas;
        private Panel panelResumen;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel panelHeader;
        private Panel panelFiltros;
        private Panel panelContenido;

        public FrmReportesMedico(int dniMedico)
        {
            _dniMedico = dniMedico;
            InitializeComponent();
            _presenter = new ReportesMedicoPresenter(this, dniMedico);
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo de Reportes de Calidad y Desempeño Personal";
            this.Size = new Size(1300, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1200, 700);
            this.BackColor = Color.White;

            // PANEL HEADER
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 10)
            };

            lblTitulo = new Label
            {
                Text = "Reportes de Calidad y Desempeño Médico",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 136),
                Location = new Point(0, 0),
                Size = new Size(900, 35),
                BackColor = Color.Transparent
            };

            lblSubtitulo = new Label
            {
                Text = "Análisis personalizados para monitorear su calidad de trabajo y carga laboral",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(0, 35),
                Size = new Size(900, 25),
                BackColor = Color.Transparent
            };

            panelHeader.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo });

            // PANEL DE FILTROS Y BOTONES
            panelFiltros = new Panel
            {
                Dock = DockStyle.Top,
                Height = 140,
                BackColor = Color.FromArgb(245, 250, 255),
                Padding = new Padding(30, 15, 30, 15),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Grupo de filtros de fechas
            var grpFechas = new GroupBox
            {
                Text = "  Período de Análisis  ",
                Location = new Point(0, 0),
                Size = new Size(550, 110),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 150, 136),
                BackColor = Color.White
            };

            var lblDesde = new Label
            {
                Text = "Desde:",
                Location = new Point(20, 35),
                Size = new Size(70, 22),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            dtpDesde = new DateTimePicker
            {
                Location = new Point(95, 32),
                Width = 180,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-7),
                Font = new Font("Segoe UI", 10),
                CalendarForeColor = Color.FromArgb(0, 150, 136)
            };

            var lblHasta = new Label
            {
                Text = "Hasta:",
                Location = new Point(300, 35),
                Size = new Size(70, 22),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            dtpHasta = new DateTimePicker
            {
                Location = new Point(370, 32),
                Width = 160,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now,
                Font = new Font("Segoe UI", 10),
                CalendarForeColor = Color.FromArgb(0, 150, 136)
            };

            var lblInfoFiltro = new Label
            {
                Text = "Seleccione el rango de fechas para filtrar los datos del reporte",
                Location = new Point(20, 70),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166)
            };

            grpFechas.Controls.AddRange(new Control[] { lblDesde, dtpDesde, lblHasta, dtpHasta, lblInfoFiltro });

            // Botones de reportes
            btnAlertas = new Button
            {
                Text = "Reporte de Alertas\n(Valores Críticos)",
                Location = new Point(570, 10),
                Size = new Size(300, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 87, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnAlertas.FlatAppearance.BorderSize = 0;
            btnAlertas.Click += (s, e) => GenerarReporteAlertasClick?.Invoke(s, e);

            btnCargaTrabajo = new Button
            {
                Text = "Mi Carga de Trabajo\n(Pendientes y Verificados)",
                Location = new Point(890, 10),
                Size = new Size(300, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 150, 136),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnCargaTrabajo.FlatAppearance.BorderSize = 0;
            btnCargaTrabajo.Click += (s, e) => GenerarReporteCargaTrabajoClick?.Invoke(s, e);

            // Panel de ayuda
            var panelAyuda = new Panel
            {
                Location = new Point(570, 70),
                Size = new Size(620, 40),
                BackColor = Color.FromArgb(232, 245, 233),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblAyuda = new Label
            {
                Text = "Haga clic en cualquier botón de reporte para generar la visualización correspondiente",
                Location = new Point(10, 10),
                Size = new Size(590, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(56, 142, 60),
                BackColor = Color.Transparent
            };

            panelAyuda.Controls.Add(lblAyuda);

            panelFiltros.Controls.AddRange(new Control[] { 
                grpFechas, btnAlertas, btnCargaTrabajo, panelAyuda 
            });

            // PANEL DEL CONTENIDO
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 30)
            };

            // Título del contenido actual
            var lblTituloContenido = new Label
            {
                Text = "Seleccione un reporte para visualizar las estadísticas personales",
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(127, 140, 141),
                BackColor = Color.Transparent
            };

            // Grid para alertas
            gridAlertas = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(255, 87, 34),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(255, 224, 178),
                    SelectionForeColor = Color.FromArgb(44, 62, 80)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(255, 250, 245)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false
            };

            // Panel de resumen (para carga de trabajo)
            panelResumen = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };

            panelContenido.Controls.Add(gridAlertas);
            panelContenido.Controls.Add(panelResumen);
            panelContenido.Controls.Add(lblTituloContenido);

            // Agregar paneles al formulario
            this.Controls.Add(panelContenido);
            this.Controls.Add(panelFiltros);
            this.Controls.Add(panelHeader);
        }

        #region Implementación de IReportesMedicoView

        public DateTime FechaDesde => dtpDesde.Value.Date;
        public DateTime FechaHasta => dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);

        public event EventHandler GenerarReporteAlertasClick;
        public event EventHandler GenerarReporteCargaTrabajoClick;

        public void MostrarReporteAlertas(System.Collections.IEnumerable datos)
        {
            // Ocultar otros controles
            panelResumen.Visible = false;
            gridAlertas.Visible = true;

            // Actualizar título del panel
            var lblTituloContenido = panelContenido.Controls.OfType<Label>().FirstOrDefault();
            if (lblTituloContenido != null)
                lblTituloContenido.Text = "Reporte de Alertas - Valores Fuera de Rango Crítico";

            var lista = datos.Cast<BLL.ReporteAlerta>().ToList();

            if (lista.Count == 0)
            {
                if (lblTituloContenido != null)
                    lblTituloContenido.Text = "No se encontraron valores críticos en el período seleccionado";
                gridAlertas.DataSource = null;
                return;
            }

            // Preparar datos para visualización
            var datosGrid = lista.Select(a => new
            {
                IdAnalisis = a.IdAnalisis,
                Paciente = a.NombrePaciente,
                Métrica = a.NombreMetrica,
                Resultado = a.Resultado,
                RangoReferencia = $"{a.ValorMinimo ?? 0} - {a.ValorMaximo ?? 0}",
                Fecha = a.FechaAnalisis.ToString("dd/MM/yyyy"),
                Estado = DeterminarEstadoValor(a)
            }).ToList();

            gridAlertas.DataSource = datosGrid;

            // Colorear filas según criticidad
            foreach (DataGridViewRow row in gridAlertas.Rows)
            {
                var estado = row.Cells["Estado"].Value?.ToString();
                if (estado == "Crítico Alto")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                else if (estado == "Crítico Bajo")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
            }
        }

        private string DeterminarEstadoValor(BLL.ReporteAlerta alerta)
        {
            if (alerta.ValorMinimo.HasValue && alerta.Resultado < alerta.ValorMinimo.Value)
                return "Crítico Bajo";
            if (alerta.ValorMaximo.HasValue && alerta.Resultado > alerta.ValorMaximo.Value)
                return "Crítico Alto";
            return "Normal";
        }

        public void MostrarReporteCargaTrabajo(BLL.ReporteCargaTrabajo datos)
        {
            // Ocultar otros controles
            gridAlertas.Visible = false;
            panelResumen.Visible = true;
            panelResumen.Controls.Clear();

            // Actualizar título del panel
            var lblTituloContenido = panelContenido.Controls.OfType<Label>().FirstOrDefault();
            if (lblTituloContenido != null)
                lblTituloContenido.Text = "Mi Carga de Trabajo Personal - Estado Actual";

            // Crear tarjetas de resumen visual con mejor distribución
            var tarjetaPendientes = new Panel
            {
                Location = new Point(150, 80),
                Size = new Size(400, 200),
                BackColor = Color.FromArgb(255, 243, 224),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblCategoriaPendientes = new Label
            {
                Text = "Análisis Pendientes de Verificar",
                Location = new Point(0, 15),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(230, 81, 0),
                BackColor = Color.Transparent
            };

            var lblValorPendientes = new Label
            {
                Text = datos.AnalisisPendientes.ToString(),
                Location = new Point(0, 60),
                Size = new Size(400, 80),
                Font = new Font("Segoe UI", 48, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(230, 81, 0),
                BackColor = Color.Transparent
            };

            var lblDescPendientes = new Label
            {
                Text = "requieren su atención inmediata",
                Location = new Point(0, 150),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent
            };

            tarjetaPendientes.Controls.AddRange(new Control[] { 
                lblCategoriaPendientes, lblValorPendientes, lblDescPendientes 
            });

            var tarjetaVerificados = new Panel
            {
                Location = new Point(600, 80),
                Size = new Size(400, 200),
                BackColor = Color.FromArgb(232, 245, 233),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblCategoriaVerificados = new Label
            {
                Text = "Análisis Verificados este Mes",
                Location = new Point(0, 15),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(56, 142, 60),
                BackColor = Color.Transparent
            };

            var lblValorVerificados = new Label
            {
                Text = datos.AnalisisVerificadosMes.ToString(),
                Location = new Point(0, 60),
                Size = new Size(400, 80),
                Font = new Font("Segoe UI", 48, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(56, 142, 60),
                BackColor = Color.Transparent
            };

            var lblDescVerificados = new Label
            {
                Text = "completados exitosamente",
                Location = new Point(0, 150),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent
            };

            tarjetaVerificados.Controls.AddRange(new Control[] { 
                lblCategoriaVerificados, lblValorVerificados, lblDescVerificados 
            });

            // Panel de información adicional
            var panelInfo = new Panel
            {
                Location = new Point(150, 300),
                Size = new Size(850, 150),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblInfoTitulo = new Label
            {
                Text = "Información Importante sobre su Desempeño",
                Location = new Point(20, 15),
                Size = new Size(810, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                BackColor = Color.Transparent
            };

            var lblInfoDetalle = new Label
            {
                Text = "• Los análisis pendientes requieren su verificación y firma profesional para ser completados\n\n" +
                       "• Los análisis verificados este mes reflejan su productividad y contribución al laboratorio\n\n" +
                       "• Revise regularmente el Reporte de Alertas para identificar valores críticos que requieran\n" +
                       "  atención médica inmediata o seguimiento especial del paciente",
                Location = new Point(25, 45),
                Size = new Size(800, 90),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                BackColor = Color.Transparent
            };

            panelInfo.Controls.AddRange(new Control[] { lblInfoTitulo, lblInfoDetalle });

            panelResumen.Controls.AddRange(new Control[] { 
                tarjetaPendientes, tarjetaVerificados, panelInfo 
            });
        }

        public void MostrarMensaje(string mensaje, bool esError = false)
        {
            MessageBox.Show(
                mensaje,
                esError ? "SALC - Error en Reportes Médicos" : "SALC - Reportes Médicos",
                MessageBoxButtons.OK,
                esError ? MessageBoxIcon.Error : MessageBoxIcon.Information
            );
        }

        #endregion
    }
}
