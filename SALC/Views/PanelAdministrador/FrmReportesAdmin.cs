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
        private Panel panelFiltros;
        private Panel panelGrafico;

        public FrmReportesAdmin()
        {
            InitializeComponent();
            _presenter = new ReportesAdminPresenter(this);
        }

        private void InitializeComponent()
        {
            this.Text = "Reportes Administrativos - SALC";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Panel de filtros (superior)
            panelFiltros = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(10)
            };

            // Filtros de fechas
            var lblDesde = new Label
            {
                Text = "Fecha Desde:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            dtpDesde = new DateTimePicker
            {
                Location = new Point(120, 17),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddMonths(-1)
            };

            var lblHasta = new Label
            {
                Text = "Fecha Hasta:",
                Location = new Point(350, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            dtpHasta = new DateTimePicker
            {
                Location = new Point(450, 17),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Botones de reportes
            btnProductividad = new Button
            {
                Text = "?? Productividad Médicos",
                Location = new Point(20, 60),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnProductividad.FlatAppearance.BorderSize = 0;
            btnProductividad.Click += (s, e) => GenerarReporteProductividadClick?.Invoke(s, e);

            btnFacturacion = new Button
            {
                Text = "?? Facturación por Obra Social",
                Location = new Point(240, 60),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BackColor = Color.FromArgb(0, 153, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnFacturacion.FlatAppearance.BorderSize = 0;
            btnFacturacion.Click += (s, e) => GenerarReporteFacturacionClick?.Invoke(s, e);

            btnDemanda = new Button
            {
                Text = "?? Top Análisis Solicitados",
                Location = new Point(460, 60),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BackColor = Color.FromArgb(255, 140, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDemanda.FlatAppearance.BorderSize = 0;
            btnDemanda.Click += (s, e) => GenerarReporteDemandaClick?.Invoke(s, e);

            panelFiltros.Controls.AddRange(new Control[] 
            { 
                lblDesde, dtpDesde, lblHasta, dtpHasta, 
                btnProductividad, btnFacturacion, btnDemanda 
            });

            // Panel de gráfico (centro)
            panelGrafico = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Título del gráfico
            lblTitulo = new Label
            {
                Text = "Seleccione un reporte para visualizar",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            // Chart principal
            chartPrincipal = new Chart
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            var chartArea = new ChartArea("MainArea");
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.BackColor = Color.White;
            chartPrincipal.ChartAreas.Add(chartArea);

            panelGrafico.Controls.Add(chartPrincipal);
            panelGrafico.Controls.Add(lblTitulo);

            // Agregar paneles al formulario
            this.Controls.Add(panelGrafico);
            this.Controls.Add(panelFiltros);

            // Configurar leyenda global del chart
            var legend = new Legend("MainLegend");
            legend.Docking = Docking.Right;
            legend.Font = new Font("Segoe UI", 9F);
            chartPrincipal.Legends.Add(legend);
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

            lblTitulo.Text = "Productividad de Médicos";

            var series = new Series("Análisis Creados");
            series.ChartType = SeriesChartType.Column;
            series.Color = Color.FromArgb(0, 120, 215);
            series["PointWidth"] = "0.8";

            var seriesVerificados = new Series("Análisis Verificados");
            seriesVerificados.ChartType = SeriesChartType.Column;
            seriesVerificados.Color = Color.FromArgb(0, 153, 51);
            seriesVerificados["PointWidth"] = "0.8";

            var lista = datos.Cast<BLL.ReporteProductividad>().ToList();

            foreach (var item in lista)
            {
                series.Points.AddXY(item.NombreMedico, item.CantidadCreados);
                seriesVerificados.Points.AddXY(item.NombreMedico, item.CantidadVerificados);
            }

            chartPrincipal.Series.Add(series);
            chartPrincipal.Series.Add(seriesVerificados);

            chartPrincipal.ChartAreas[0].AxisX.Title = "Médico";
            chartPrincipal.ChartAreas[0].AxisY.Title = "Cantidad de Análisis";
            chartPrincipal.ChartAreas[0].AxisX.Interval = 1;
            chartPrincipal.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartPrincipal.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 8F);

            // Agregar título al gráfico
            var title = new Title($"Período: {FechaDesde:dd/MM/yyyy} - {FechaHasta:dd/MM/yyyy}");
            title.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            title.ForeColor = Color.Gray;
            chartPrincipal.Titles.Add(title);
        }

        public void MostrarReporteFacturacion(System.Collections.IEnumerable datos)
        {
            chartPrincipal.Series.Clear();
            chartPrincipal.Titles.Clear();

            lblTitulo.Text = "Facturación por Obra Social";

            var series = new Series("Obras Sociales");
            series.ChartType = SeriesChartType.Pie;
            series["PieLabelStyle"] = "Outside";
            series.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            var lista = datos.Cast<BLL.ReporteFacturacion>().ToList();

            // Colores personalizados para el gráfico de torta
            var colores = new Color[]
            {
                Color.FromArgb(0, 120, 215),
                Color.FromArgb(0, 153, 51),
                Color.FromArgb(255, 140, 0),
                Color.FromArgb(153, 51, 255),
                Color.FromArgb(255, 51, 51),
                Color.FromArgb(0, 204, 204),
                Color.FromArgb(255, 204, 0),
                Color.FromArgb(102, 102, 102)
            };

            int colorIndex = 0;
            foreach (var item in lista)
            {
                var punto = series.Points.Add(item.CantidadAnalisis);
                punto.Label = $"{item.NombreObraSocial}\n{item.Porcentaje:F1}%";
                punto.LegendText = $"{item.NombreObraSocial} ({item.CantidadAnalisis})";
                punto.Color = colores[colorIndex % colores.Length];
                colorIndex++;
            }

            chartPrincipal.Series.Add(series);

            // Agregar título al gráfico
            var title = new Title($"Período: {FechaDesde:dd/MM/yyyy} - {FechaHasta:dd/MM/yyyy}");
            title.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            title.ForeColor = Color.Gray;
            chartPrincipal.Titles.Add(title);
        }

        public void MostrarReporteDemanda(System.Collections.IEnumerable datos)
        {
            chartPrincipal.Series.Clear();
            chartPrincipal.Titles.Clear();

            lblTitulo.Text = "Top 10 Análisis Más Solicitados";

            var series = new Series("Demanda");
            series.ChartType = SeriesChartType.Bar;
            series.Color = Color.FromArgb(255, 140, 0);
            series["PointWidth"] = "0.7";

            var lista = datos.Cast<BLL.ReporteDemanda>().ToList();

            foreach (var item in lista)
            {
                var punto = series.Points.AddXY(item.TipoAnalisis, item.CantidadSolicitados);
                series.Points[series.Points.Count - 1].Label = item.CantidadSolicitados.ToString();
            }

            chartPrincipal.Series.Add(series);

            chartPrincipal.ChartAreas[0].AxisX.Title = "Cantidad Solicitada";
            chartPrincipal.ChartAreas[0].AxisY.Title = "Tipo de Análisis";
            chartPrincipal.ChartAreas[0].AxisY.Interval = 1;
            chartPrincipal.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 8F);

            // Agregar título al gráfico
            var title = new Title($"Período: {FechaDesde:dd/MM/yyyy} - {FechaHasta:dd/MM/yyyy}");
            title.Font = new Font("Segoe UI", 10F, FontStyle.Italic);
            title.ForeColor = Color.Gray;
            chartPrincipal.Titles.Add(title);
        }

        public void MostrarMensaje(string mensaje, bool esError = false)
        {
            MessageBox.Show(
                mensaje,
                esError ? "Error" : "Reportes",
                MessageBoxButtons.OK,
                esError ? MessageBoxIcon.Error : MessageBoxIcon.Information
            );
        }

        #endregion
    }
}
