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
            this.Text = "Reportes de Calidad y Desempeño - SALC (Médico)";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(1000, 600);

            // Panel de filtros (superior)
            panelFiltros = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(10)
            };

            // Título informativo
            var lblInfoRol = new Label
            {
                Text = "?? Reportes para Médicos - Calidad y Desempeño Personal",
                Location = new Point(20, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204)
            };

            // Filtros de fechas
            var lblDesde = new Label
            {
                Text = "Fecha Desde:",
                Location = new Point(20, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            dtpDesde = new DateTimePicker
            {
                Location = new Point(120, 42),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now.AddDays(-7) // Última semana por defecto
            };

            var lblHasta = new Label
            {
                Text = "Fecha Hasta:",
                Location = new Point(350, 45),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            dtpHasta = new DateTimePicker
            {
                Location = new Point(450, 42),
                Width = 200,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Now
            };

            // Botones de reportes
            btnAlertas = new Button
            {
                Text = "?? Reporte de Alertas\n(Valores Críticos)",
                Location = new Point(20, 75),
                Size = new Size(200, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BackColor = Color.FromArgb(255, 87, 34), // Naranja rojizo
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnAlertas.FlatAppearance.BorderSize = 0;
            btnAlertas.Click += (s, e) => GenerarReporteAlertasClick?.Invoke(s, e);

            btnCargaTrabajo = new Button
            {
                Text = "?? Mi Carga de Trabajo\n(Pendientes y Verificados)",
                Location = new Point(240, 75),
                Size = new Size(220, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                BackColor = Color.FromArgb(0, 150, 136), // Verde azulado
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnCargaTrabajo.FlatAppearance.BorderSize = 0;
            btnCargaTrabajo.Click += (s, e) => GenerarReporteCargaTrabajoClick?.Invoke(s, e);

            panelFiltros.Controls.AddRange(new Control[] 
            { 
                lblInfoRol, lblDesde, dtpDesde, lblHasta, dtpHasta, 
                btnAlertas, btnCargaTrabajo 
            });

            // Panel de contenido (centro)
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            // Título del contenido
            lblTitulo = new Label
            {
                Text = "Seleccione un reporte para visualizar",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(64, 64, 64)
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
                Visible = false
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
            panelContenido.Controls.Add(lblTitulo);

            // Agregar paneles al formulario
            this.Controls.Add(panelContenido);
            this.Controls.Add(panelFiltros);
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

            lblTitulo.Text = "?? Reporte de Alertas - Valores Fuera de Rango";

            var lista = datos.Cast<BLL.ReporteAlerta>().ToList();

            if (lista.Count == 0)
            {
                lblTitulo.Text = "? No se encontraron valores críticos en el período seleccionado";
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
                if (estado == "?? Crítico Alto")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                else if (estado == "?? Crítico Bajo")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
            }
        }

        private string DeterminarEstadoValor(BLL.ReporteAlerta alerta)
        {
            if (alerta.ValorMinimo.HasValue && alerta.Resultado < alerta.ValorMinimo.Value)
                return "?? Crítico Bajo";
            if (alerta.ValorMaximo.HasValue && alerta.Resultado > alerta.ValorMaximo.Value)
                return "?? Crítico Alto";
            return "Normal";
        }

        public void MostrarReporteCargaTrabajo(BLL.ReporteCargaTrabajo datos)
        {
            // Ocultar otros controles
            gridAlertas.Visible = false;
            panelResumen.Visible = true;
            panelResumen.Controls.Clear();

            lblTitulo.Text = "?? Mi Carga de Trabajo Personal";

            // Crear tarjetas de resumen visual
            var lblPendientes = new Label
            {
                Text = $"{datos.AnalisisPendientes}\nAnálisis Pendientes\nde Verificar",
                Location = new Point(100, 100),
                Size = new Size(300, 150),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(255, 243, 224),
                ForeColor = Color.FromArgb(230, 81, 0),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblVerificados = new Label
            {
                Text = $"{datos.AnalisisVerificadosMes}\nAnálisis Verificados\neste Mes",
                Location = new Point(450, 100),
                Size = new Size(300, 150),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(232, 245, 233),
                ForeColor = Color.FromArgb(56, 142, 60),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Información adicional
            var lblInfo = new Label
            {
                Text = "?? Información:\n\n" +
                       "• Los análisis pendientes requieren su verificación y firma\n" +
                       "• Los análisis verificados este mes reflejan su productividad reciente\n" +
                       "• Revise regularmente los valores críticos en el reporte de alertas",
                Location = new Point(100, 280),
                Size = new Size(650, 120),
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            panelResumen.Controls.AddRange(new Control[] { lblPendientes, lblVerificados, lblInfo });
        }

        public void MostrarMensaje(string mensaje, bool esError = false)
        {
            MessageBox.Show(
                mensaje,
                esError ? "Error" : "Reportes - Médico",
                MessageBoxButtons.OK,
                esError ? MessageBoxIcon.Error : MessageBoxIcon.Information
            );
        }

        #endregion
    }
}
