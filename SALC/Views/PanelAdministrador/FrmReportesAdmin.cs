using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SALC.Presenters;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelAdministrador
{
    public partial class FrmReportesAdmin : Form, IReportesAdminView
    {
        private ReportesAdminPresenter _presenter;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Button btnProductividad;
        private Button btnFacturacion;
        private Button btnDemanda;
        private Chart chartPrincipal;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel panelHeader;
        private Panel panelFiltros;
        private Panel panelGrafico;

        public FrmReportesAdmin()
        {
            InitializeComponent();
            _presenter = new ReportesAdminPresenter(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Módulo de Reportes Estadísticos y Análisis";
            this.Size = new Size(1300, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1200, 700);
            this.BackColor = Color.White;

            // ============ PANEL HEADER ============
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 10)
            };

            lblTitulo = new Label
            {
                Text = "Reportes Administrativos del Laboratorio",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                Location = new Point(0, 0),
                Size = new Size(900, 35),
                BackColor = Color.Transparent
            };

            lblSubtitulo = new Label
            {
                Text = "Análisis estadísticos y visualizaciones para la toma de decisiones",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(0, 35),
                Size = new Size(900, 25),
                BackColor = Color.Transparent
            };

            panelHeader.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo });

            // ============ PANEL DE FILTROS Y BOTONES ============
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
                ForeColor = Color.FromArgb(52, 152, 219),
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
                Value = DateTime.Now.AddMonths(-1),
                Font = new Font("Segoe UI", 10),
                CalendarForeColor = Color.FromArgb(52, 152, 219)
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
                CalendarForeColor = Color.FromArgb(52, 152, 219)
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
            btnProductividad = new Button
            {
                Text = "Productividad de Médicos",
                Location = new Point(570, 10),
                Size = new Size(220, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnProductividad.FlatAppearance.BorderSize = 0;
            btnProductividad.Click += (s, e) => GenerarReporteProductividadClick?.Invoke(s, e);

            btnFacturacion = new Button
            {
                Text = "Distribución por Obra Social",
                Location = new Point(810, 10),
                Size = new Size(220, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnFacturacion.FlatAppearance.BorderSize = 0;
            btnFacturacion.Click += (s, e) => GenerarReporteFacturacionClick?.Invoke(s, e);

            btnDemanda = new Button
            {
                Text = "Análisis Más Solicitados",
                Location = new Point(1050, 10),
                Size = new Size(200, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnDemanda.FlatAppearance.BorderSize = 0;
            btnDemanda.Click += (s, e) => GenerarReporteDemandaClick?.Invoke(s, e);

            // Panel de ayuda
            var panelAyuda = new Panel
            {
                Location = new Point(570, 70),
                Size = new Size(680, 40),
                BackColor = Color.FromArgb(255, 250, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblAyuda = new Label
            {
                Text = "Haga clic en cualquier botón de reporte para generar la visualización correspondiente",
                Location = new Point(10, 10),
                Size = new Size(650, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(230, 126, 34),
                BackColor = Color.Transparent
            };

            panelAyuda.Controls.Add(lblAyuda);

            panelFiltros.Controls.AddRange(new Control[] { 
                grpFechas, btnProductividad, btnFacturacion, btnDemanda, panelAyuda 
            });

            // ============ PANEL DEL GRÁFICO ============
            panelGrafico = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(30, 20, 30, 30)
            };

            // Título del gráfico actual
            var lblTituloGrafico = new Label
            {
                Text = "Seleccione un reporte para visualizar las estadísticas",
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(127, 140, 141),
                BackColor = Color.Transparent
            };

            // Chart principal con diseño mejorado
            chartPrincipal = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderlineColor = Color.FromArgb(189, 195, 199),
                BorderlineDashStyle = ChartDashStyle.Solid,
                BorderlineWidth = 1
            };

            // Configurar área del gráfico
            var chartArea = new ChartArea("MainArea");
            chartArea.BackColor = Color.FromArgb(250, 252, 255);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(220, 220, 220);
            chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(220, 220, 220);
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
            chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
            chartArea.AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            chartPrincipal.ChartAreas.Add(chartArea);

            // Configurar leyenda
            var legend = new Legend("MainLegend");
            legend.Docking = Docking.Right;
            legend.Font = new Font("Segoe UI", 9);
            legend.BackColor = Color.FromArgb(250, 252, 255);
            legend.BorderColor = Color.FromArgb(189, 195, 199);
            chartPrincipal.Legends.Add(legend);

            panelGrafico.Controls.Add(chartPrincipal);
            panelGrafico.Controls.Add(lblTituloGrafico);

            // Agregar todos los paneles al formulario
            this.Controls.Add(panelGrafico);
            this.Controls.Add(panelFiltros);
            this.Controls.Add(panelHeader);
        }

        #region Implementación de IReportesAdminView

        public DateTime FechaDesde => dtpDesde.Value.Date;
        public DateTime FechaHasta => dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);

        public event EventHandler GenerarReporteProductividadClick;
        public event EventHandler GenerarReporteFacturacionClick;
        public event EventHandler GenerarReporteDemandaClick;

        public void MostrarReporteProductividad(System.Collections.IEnumerable datos)
        {
            chartPrincipal.Series.Clear();
            chartPrincipal.Titles.Clear();

            // Actualizar título del panel
            var lblTitulo = panelGrafico.Controls.OfType<Label>().FirstOrDefault();
            if (lblTitulo != null)
                lblTitulo.Text = "Productividad del Personal Médico";

            // Serie de análisis creados
            var series = new Series("Análisis Creados");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.FromArgb(52, 152, 219);
            series["PointWidth"] = "0.7";
            series.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Serie de análisis verificados
            var seriesVerificados = new Series("Análisis Verificados");
            seriesVerificados.ChartType = SeriesChartType.Column;
            seriesVerificados.Color = Color.FromArgb(39, 174, 96);
            seriesVerificados["PointWidth"] = "0.7";
            seriesVerificados.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var lista = datos.Cast<BLL.ReporteProductividad>().ToList();

            foreach (var item in lista)
            {
                var punto1 = series.Points.AddXY(item.NombreMedico, item.CantidadCreados);
                var punto2 = seriesVerificados.Points.AddXY(item.NombreMedico, item.CantidadVerificados);
                
                series.Points[series.Points.Count - 1].Label = item.CantidadCreados.ToString();
                seriesVerificados.Points[seriesVerificados.Points.Count - 1].Label = item.CantidadVerificados.ToString();
            }

            chartPrincipal.Series.Add(series);
            chartPrincipal.Series.Add(seriesVerificados);

            chartPrincipal.ChartAreas[0].AxisX.Title = "Médico";
            chartPrincipal.ChartAreas[0].AxisY.Title = "Cantidad de Análisis";
            chartPrincipal.ChartAreas[0].AxisX.Interval = 1;
            chartPrincipal.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartPrincipal.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 8);

            // Título del gráfico
            var title = new Title($"Período: {FechaDesde:dd/MM/yyyy} - {FechaHasta:dd/MM/yyyy}");
            title.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            title.ForeColor = Color.FromArgb(127, 140, 141);
            chartPrincipal.Titles.Add(title);
        }

        public void MostrarReporteFacturacion(System.Collections.IEnumerable datos)
        {
            chartPrincipal.Series.Clear();
            chartPrincipal.Titles.Clear();

            // Actualizar título del panel
            var lblTitulo = panelGrafico.Controls.OfType<Label>().FirstOrDefault();
            if (lblTitulo != null)
                lblTitulo.Text = "Distribución de Análisis por Obra Social";

            var series = new Series("Obras Sociales");
            series.ChartType = SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";
            series["PieLineColor"] = "Black";
            series.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var lista = datos.Cast<BLL.ReporteFacturacion>().ToList();

            // Colores pasteles profesionales
            var colores = new Color[]
            {
                Color.FromArgb(52, 152, 219),   // Azul
                Color.FromArgb(39, 174, 96),    // Verde
                Color.FromArgb(230, 126, 34),   // Naranja
                Color.FromArgb(142, 68, 173),   // Púrpura
                Color.FromArgb(231, 76, 60),    // Rojo
                Color.FromArgb(26, 188, 156),   // Turquesa
                Color.FromArgb(241, 196, 15),   // Amarillo
                Color.FromArgb(149, 165, 166)   // Gris
            };

            int colorIndex = 0;
            foreach (var item in lista)
            {
                var punto = series.Points.Add(item.CantidadAnalisis);
                punto.Label = $"{item.NombreObraSocial}\n{item.Porcentaje:F1}%";
                punto.LegendText = $"{item.NombreObraSocial}: {item.CantidadAnalisis} ({item.Porcentaje:F1}%)";
                punto.Color = colores[colorIndex % colores.Length];
                colorIndex++;
            }

            chartPrincipal.Series.Add(series);

            // Título del gráfico
            var title = new Title($"Período: {FechaDesde:dd/MM/yyyy} - {FechaHasta:dd/MM/yyyy}");
            title.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            title.ForeColor = Color.FromArgb(127, 140, 141);
            chartPrincipal.Titles.Add(title);
        }

        public void MostrarReporteDemanda(System.Collections.IEnumerable datos)
        {
            chartPrincipal.Series.Clear();
            chartPrincipal.Titles.Clear();

            // Actualizar título del panel
            var lblTitulo = panelGrafico.Controls.OfType<Label>().FirstOrDefault();
            if (lblTitulo != null)
                lblTitulo.Text = "Top 10 Análisis Más Solicitados";

            var series = new Series("Demanda");
            series.ChartType = SeriesChartType.Bar;
            series.Color = Color.FromArgb(230, 126, 34);
            series["PointWidth"] = "0.6";
            series.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            var lista = datos.Cast<BLL.ReporteDemanda>().ToList();

            foreach (var item in lista)
            {
                series.Points.AddXY(item.TipoAnalisis, item.CantidadSolicitados);
                series.Points[series.Points.Count - 1].Label = $" {item.CantidadSolicitados}";
            }

            chartPrincipal.Series.Add(series);

            chartPrincipal.ChartAreas[0].AxisX.Title = "Cantidad Solicitada";
            chartPrincipal.ChartAreas[0].AxisY.Title = "Tipo de Análisis";
            chartPrincipal.ChartAreas[0].AxisY.Interval = 1;
            chartPrincipal.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 9);

            // Título del gráfico
            var title = new Title($"Período: {FechaDesde:dd/MM/yyyy} - {FechaHasta:dd/MM/yyyy}");
            title.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            title.ForeColor = Color.FromArgb(127, 140, 141);
            chartPrincipal.Titles.Add(title);
        }

        public void MostrarMensaje(string mensaje, bool esError = false)
        {
            MessageBox.Show(
                mensaje,
                esError ? "SALC - Error en Reportes" : "SALC - Reportes",
                MessageBoxButtons.OK,
                esError ? MessageBoxIcon.Error : MessageBoxIcon.Information
            );
        }

        #endregion
    }
}
