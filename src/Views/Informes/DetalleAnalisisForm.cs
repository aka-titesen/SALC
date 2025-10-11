using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SALC.Common;
using SALC.Services;
using SALC.Views.Interfaces;

namespace SALC.Views
{
    /// <summary>
    /// Formulario para mostrar detalles completos de un análisis según ERS v2.7
    /// Implementa RF-08: Generar y Enviar Informe
    /// </summary>
    public partial class DetalleAnalisisForm : Form
    {
    private readonly InformesAnalisisService _servicioInformes;
        private InformeAnalisis _informeAnalisis;
        private List<ResultadoAnalisis> _resultadosAnalisis;

        // Controles de la interfaz
        private Panel panelEncabezado;
        private Label lblTitulo;
        private Button btnCerrar;
        private Panel panelContenido;
        private Panel panelInfoPaciente;
        private Panel panelInfoAnalisis;
        private Panel panelResultados;
        private DataGridView dgvResultados;
        private Button btnImprimir;
        private Button btnExportarPdf;

        public DetalleAnalisisForm(InformeAnalisis informeAnalisis, List<ResultadoAnalisis> resultadosAnalisis)
        {
            _servicioInformes = new InformesAnalisisService();
            _informeAnalisis = informeAnalisis;
            _resultadosAnalisis = resultadosAnalisis;

            InitializeComponent();
            InicializarComponentesPersonalizados();
            CargarDatosAnalisis();
        }

        private void InitializeComponent()
        {
            this.Text = "SALC - Detalle de Análisis";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = SALCColors.Background;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InicializarComponentesPersonalizados()
        {
            // Panel de encabezado
            panelEncabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = SALCColors.Primary
            };

            lblTitulo = new Label
            {
                Text = "Detalle de Análisis",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            btnCerrar = new Button
            {
                Text = "?",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(40, 40),
                Location = new Point(940, 10),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();

            panelEncabezado.Controls.Add(lblTitulo);
            panelEncabezado.Controls.Add(btnCerrar);

            // Panel de contenido
            panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            // Panel información del paciente
            panelInfoPaciente = CrearPanelInformacion("Información del Paciente", 0);

            // Panel información del análisis
            panelInfoAnalisis = CrearPanelInformacion("Información del Análisis", 200);

            // Panel de resultados
            panelResultados = CrearPanelInformacion("Resultados", 400);
            panelResultados.Height = 200;

            dgvResultados = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            panelResultados.Controls.Add(dgvResultados);

            // Panel de botones de acción
            var panelBotones = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.Transparent
            };

            btnImprimir = new Button
            {
                Text = "Imprimir",
                Size = new Size(120, 40),
                Location = new Point(650, 10),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnImprimir.FlatAppearance.BorderSize = 0;
            btnImprimir.Click += BtnImprimir_Click;

            btnExportarPdf = new Button
            {
                Text = "Exportar PDF",
                Size = new Size(120, 40),
                Location = new Point(780, 10),
                BackColor = SALCColors.Secondary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnExportarPdf.FlatAppearance.BorderSize = 0;
            btnExportarPdf.Click += BtnExportarPdf_Click;

            panelBotones.Controls.Add(btnImprimir);
            panelBotones.Controls.Add(btnExportarPdf);

            panelContenido.Controls.Add(panelInfoPaciente);
            panelContenido.Controls.Add(panelInfoAnalisis);
            panelContenido.Controls.Add(panelResultados);
            panelContenido.Controls.Add(panelBotones);

            this.Controls.Add(panelEncabezado);
            this.Controls.Add(panelContenido);
        }

        private Panel CrearPanelInformacion(string titulo, int posicionSuperior)
        {
            var panel = new Panel
            {
                Location = new Point(0, posicionSuperior),
                Size = new Size(940, 180),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            var lblTituloPanel = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            panel.Controls.Add(lblTituloPanel);
            return panel;
        }

        private void CargarDatosAnalisis()
        {
            // Cargar datos del paciente
            AgregarInfoAPanel(panelInfoPaciente, "DNI:", _informeAnalisis.IdPaciente);
            AgregarInfoAPanel(panelInfoPaciente, "Nombre:", _informeAnalisis.NombrePaciente);
            AgregarInfoAPanel(panelInfoPaciente, "Obra Social:", _informeAnalisis.ObraSocial ?? "Sin obra social");
            AgregarInfoAPanel(panelInfoPaciente, "Teléfono:", _informeAnalisis.TelefonoPaciente ?? "No disponible");

            // Cargar datos del análisis
            AgregarInfoAPanel(panelInfoAnalisis, "ID Análisis:", _informeAnalisis.IdReporte.ToString());
            AgregarInfoAPanel(panelInfoAnalisis, "Tipo de Análisis:", _informeAnalisis.TipoAnalisis);
            AgregarInfoAPanel(panelInfoAnalisis, "Estado:", _informeAnalisis.Estado);
            AgregarInfoAPanel(panelInfoAnalisis, "Fecha Creación:", _informeAnalisis.FechaAnalisis.ToString("dd/MM/yyyy HH:mm"));
            AgregarInfoAPanel(panelInfoAnalisis, "Médico:", _informeAnalisis.NombreMedico);
            AgregarInfoAPanel(panelInfoAnalisis, "Observaciones:", _informeAnalisis.Observaciones ?? "Sin observaciones");

            // Configurar DataGridView para resultados
            ConfigurarDataGridViewResultados();
            CargarResultados();
        }

        private void ConfigurarDataGridViewResultados()
        {
            dgvResultados.Columns.Clear();
            dgvResultados.Columns.Add("Parametro", "Parámetro");
            dgvResultados.Columns.Add("Valor", "Valor");
            dgvResultados.Columns.Add("Unidad", "Unidad");
            dgvResultados.Columns.Add("RangoReferencia", "Rango de Referencia");

            dgvResultados.Columns["Parametro"].Width = 200;
            dgvResultados.Columns["Valor"].Width = 100;
            dgvResultados.Columns["Unidad"].Width = 100;
            dgvResultados.Columns["RangoReferencia"].Width = 150;

            // Estilo del encabezado
            dgvResultados.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = SALCColors.Primary,
                ForeColor = Color.White,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            dgvResultados.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvResultados.RowsDefaultCellStyle.SelectionBackColor = SALCColors.PrimaryHover;
            dgvResultados.RowsDefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void CargarResultados()
        {
            foreach (var resultado in _resultadosAnalisis)
            {
                dgvResultados.Rows.Add(
                    resultado.Parametro,
                    resultado.Valor,
                    resultado.Unidad,
                    resultado.RangoReferencia
                );
            }
        }

        private void AgregarInfoAPanel(Panel panel, string etiqueta, string valor)
        {
            var posicionY = 40 + (panel.Controls.Count - 1) * 25;

            var controlEtiqueta = new Label
            {
                Text = etiqueta,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = SALCColors.TextPrimary,
                AutoSize = true,
                Location = new Point(20, posicionY)
            };

            var controlValor = new Label
            {
                Text = valor ?? "N/A",
                Font = new Font("Segoe UI", 9),
                ForeColor = SALCColors.TextPrimary,
                AutoSize = true,
                Location = new Point(150, posicionY)
            };

            panel.Controls.Add(controlEtiqueta);
            panel.Controls.Add(controlValor);
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                // Implementar funcionalidad de impresión
                MessageBox.Show("Funcionalidad de impresión en desarrollo.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExportarPdf_Click(object sender, EventArgs e)
        {
            try
            {
                _servicioInformes.ExportarAPdf(_informeAnalisis);
                MessageBox.Show("Reporte exportado a PDF exitosamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar a PDF: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
