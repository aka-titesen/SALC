using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SALC.Domain;
using SALC.BLL;

namespace SALC.Views.PanelAsistente
{
    /// <summary>
    /// Ventana modal que muestra el detalle completo de un análisis clínico
    /// </summary>
    public class FrmDetalleAnalisis : Form
    {
        private readonly Analisis _analisis;
        private readonly Paciente _paciente;
        private readonly IAnalisisService _analisisService;
        private readonly ICatalogoService _catalogoService;

        private DataGridView gridResultados;

        public FrmDetalleAnalisis(Analisis analisis, Paciente paciente)
        {
            _analisis = analisis ?? throw new ArgumentNullException(nameof(analisis));
            _paciente = paciente ?? throw new ArgumentNullException(nameof(paciente));
            _analisisService = new AnalisisService();
            _catalogoService = new CatalogoService();

            InitializeComponent();
            CargarDetalle();
        }

        private void InitializeComponent()
        {
            Text = string.Format("Detalle del Análisis #{0}", _analisis.IdAnalisis);
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(30)
            };

            // Título
            var lblTitulo = new Label
            {
                Text = "Información Completa del Análisis Clínico",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219),
                Location = new Point(0, 0),
                Size = new Size(900, 30)
            };

            // Panel de información general
            var panelInfo = new Panel
            {
                Location = new Point(0, 45),
                Size = new Size(920, 180),
                BackColor = Color.FromArgb(245, 250, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblInfoGeneral = new Label
            {
                Text = ObtenerInfoGeneral(),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(15, 15),
                Size = new Size(890, 150),
                BackColor = Color.Transparent
            };

            panelInfo.Controls.Add(lblInfoGeneral);

            // Título de resultados
            var lblTituloResultados = new Label
            {
                Text = "Resultados de Laboratorio",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(39, 174, 96),
                Location = new Point(0, 240),
                Size = new Size(400, 30)
            };

            // Grid de resultados
            gridResultados = new DataGridView
            {
                Location = new Point(0, 280),
                Size = new Size(920, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersHeight = 45,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(39, 174, 96),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Padding = new Padding(10),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10),
                    Padding = new Padding(8)
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 255, 250)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                RowTemplate = { Height = 40 }
            };

            // Botón cerrar
            var btnCerrar = new Button
            {
                Text = "Cerrar",
                Location = new Point(800, 595),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;

            panelPrincipal.Controls.AddRange(new Control[] {
                lblTitulo, panelInfo, lblTituloResultados, gridResultados, btnCerrar
            });

            Controls.Add(panelPrincipal);
        }

        private string ObtenerInfoGeneral()
        {
            var tipo = _catalogoService.ObtenerTiposAnalisis()
                .FirstOrDefault(t => t.IdTipoAnalisis == _analisis.IdTipoAnalisis);
            var tipoDesc = tipo != null ? tipo.Descripcion : string.Format("Tipo {0}", _analisis.IdTipoAnalisis);

            var estado = ObtenerDescripcionEstado(_analisis.IdEstado);
            var medicoCarga = ObtenerNombreMedico(_analisis.DniCarga);
            var medicoFirma = _analisis.DniFirma.HasValue ? ObtenerNombreMedico(_analisis.DniFirma.Value) : "-";

            return string.Format(
                "ID del Análisis: {0}\n" +
                "Paciente: {1} {2} (DNI: {3})\n" +
                "Tipo de Análisis: {4}\n" +
                "Estado: {5}\n" +
                "Fecha de Creación: {6}\n" +
                "Fecha de Firma: {7}\n" +
                "Médico que Solicitó: {8}\n" +
                "Médico que Firmó: {9}\n" +
                "Observaciones: {10}",
                _analisis.IdAnalisis,
                _paciente.Nombre,
                _paciente.Apellido,
                _paciente.Dni,
                tipoDesc,
                estado,
                _analisis.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                _analisis.FechaFirma.HasValue ? _analisis.FechaFirma.Value.ToString("dd/MM/yyyy HH:mm") : "No firmado",
                medicoCarga,
                medicoFirma,
                string.IsNullOrWhiteSpace(_analisis.Observaciones) ? "Sin observaciones" : _analisis.Observaciones
            );
        }

        private void CargarDetalle()
        {
            try
            {
                var resultados = _analisisService.ObtenerResultados(_analisis.IdAnalisis).ToList();

                if (resultados.Count == 0)
                {
                    var lblSinResultados = new Label
                    {
                        Text = "Este análisis aún no tiene resultados de laboratorio cargados",
                        Font = new Font("Segoe UI", 11, FontStyle.Italic),
                        ForeColor = Color.FromArgb(149, 165, 166),
                        Location = new Point(200, 350),
                        Size = new Size(520, 30),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    gridResultados.Parent.Controls.Add(lblSinResultados);
                    gridResultados.Visible = false;
                    return;
                }

                var datosGrid = resultados.Select(r => new
                {
                    Métrica = ObtenerNombreMetrica(r.IdMetrica),
                    Resultado = r.Resultado,
                    Unidad = ObtenerUnidadMetrica(r.IdMetrica),
                    RangoMinimo = ObtenerRangoMinimo(r.IdMetrica),
                    RangoMaximo = ObtenerRangoMaximo(r.IdMetrica),
                    Observaciones = string.IsNullOrWhiteSpace(r.Observaciones) ? "-" : r.Observaciones
                }).ToList();

                gridResultados.DataSource = datosGrid;

                if (gridResultados.Columns.Count > 0)
                {
                    gridResultados.Columns["Métrica"].Width = 200;
                    gridResultados.Columns["Resultado"].Width = 100;
                    gridResultados.Columns["Unidad"].Width = 100;
                    gridResultados.Columns["RangoMinimo"].Width = 100;
                    gridResultados.Columns["RangoMaximo"].Width = 100;
                    gridResultados.Columns["Observaciones"].Width = 300;

                    // Colorear resultados fuera de rango
                    foreach (DataGridViewRow row in gridResultados.Rows)
                    {
                        var resultado = Convert.ToDecimal(row.Cells["Resultado"].Value);
                        var minimo = row.Cells["RangoMinimo"].Value as decimal?;
                        var maximo = row.Cells["RangoMaximo"].Value as decimal?;

                        if ((minimo.HasValue && resultado < minimo.Value) || 
                            (maximo.HasValue && resultado > maximo.Value))
                        {
                            row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                            row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                            row.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al cargar resultados: {0}", ex.Message),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerDescripcionEstado(int idEstado)
        {
            switch (idEstado)
            {
                case 1: return "Sin verificar";
                case 2: return "Verificado";
                case 3: return "Anulado";
                default: return string.Format("Estado {0}", idEstado);
            }
        }

        private string ObtenerNombreMedico(int dniMedico)
        {
            try
            {
                var usuarioRepo = new SALC.DAL.UsuarioRepositorio();
                var medico = usuarioRepo.ObtenerPorId(dniMedico);
                return medico != null ? string.Format("Dr. {0} {1}", medico.Nombre, medico.Apellido) : string.Format("Dr. {0}", dniMedico);
            }
            catch
            {
                return string.Format("Dr. {0}", dniMedico);
            }
        }

        private string ObtenerNombreMetrica(int idMetrica)
        {
            try
            {
                var metrica = _catalogoService.ObtenerMetricas()
                    .FirstOrDefault(m => m.IdMetrica == idMetrica);
                return metrica != null ? metrica.Nombre : string.Format("Métrica {0}", idMetrica);
            }
            catch
            {
                return string.Format("Métrica {0}", idMetrica);
            }
        }

        private string ObtenerUnidadMetrica(int idMetrica)
        {
            try
            {
                var metrica = _catalogoService.ObtenerMetricas()
                    .FirstOrDefault(m => m.IdMetrica == idMetrica);
                return metrica != null ? metrica.UnidadMedida : "";
            }
            catch
            {
                return "";
            }
        }

        private decimal? ObtenerRangoMinimo(int idMetrica)
        {
            try
            {
                var metrica = _catalogoService.ObtenerMetricas()
                    .FirstOrDefault(m => m.IdMetrica == idMetrica);
                return metrica?.ValorMinimo;
            }
            catch
            {
                return null;
            }
        }

        private decimal? ObtenerRangoMaximo(int idMetrica)
        {
            try
            {
                var metrica = _catalogoService.ObtenerMetricas()
                    .FirstOrDefault(m => m.IdMetrica == idMetrica);
                return metrica?.ValorMaximo;
            }
            catch
            {
                return null;
            }
        }
    }
}
