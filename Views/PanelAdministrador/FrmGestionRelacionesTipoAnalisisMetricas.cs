using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using SALC.BLL;
using SALC.Domain;

namespace SALC.Views.PanelAdministrador
{
    public partial class FrmGestionRelacionesTipoAnalisisMetricas : Form
    {
        private readonly ICatalogoService _catalogoService;
        private ComboBox cboTiposAnalisis;
        private CheckedListBox clbMetricas;
        private DataGridView gridRelaciones;
        private Button btnGuardar, btnCancelar, btnEliminarRelacion, btnActualizar;

        private List<TipoAnalisis> _tiposAnalisis;
        private List<Metrica> _metricas;
        private List<TipoAnalisisMetrica> _relaciones;

        public FrmGestionRelacionesTipoAnalisisMetricas()
        {
            _catalogoService = new CatalogoService();
            InitializeComponent();
            CargarDatos();
        }

        private void InitializeComponent()
        {
            Text = "Gestión de Relaciones";
            Size = new Size(1100, 750);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;

            // Título principal
            var lblTitulo = new Label
            {
                Text = "Configuración de Tipos de Análisis y Métricas",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 180),
                Location = new Point(20, 15),
                Size = new Size(900, 30),
                BackColor = Color.Transparent
            };

            var lblSubtitulo = new Label
            {
                Text = "Defina qué métricas componen cada tipo de análisis clínico",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(20, 45),
                Size = new Size(900, 20),
                BackColor = Color.Transparent
            };

            // Panel izquierdo - Configuración
            var pnlConfiguracion = new Panel
            {
                Location = new Point(20, 75),
                Size = new Size(450, 590),
                BackColor = Color.FromArgb(250, 252, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblSeccionConfig = new Label
            {
                Text = "Configurar Relaciones",
                Location = new Point(0, 0),
                Size = new Size(450, 35),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(70, 130, 180),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            var lblTipoAnalisis = new Label
            {
                Text = "1. Seleccione el Tipo de Análisis:",
                Location = new Point(15, 50),
                Size = new Size(420, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            cboTiposAnalisis = new ComboBox
            {
                Location = new Point(15, 75),
                Size = new Size(415, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Descripcion",
                ValueMember = "IdTipoAnalisis",
                Font = new Font("Segoe UI", 10)
            };
            cboTiposAnalisis.SelectedIndexChanged += CboTiposAnalisis_SelectedIndexChanged;

            var lblMetricas = new Label
            {
                Text = "2. Marque las Métricas que lo componen:",
                Location = new Point(15, 115),
                Size = new Size(420, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            clbMetricas = new CheckedListBox
            {
                Location = new Point(15, 140),
                Size = new Size(415, 360),
                DisplayMember = "NombreCompleto",
                ValueMember = "IdMetrica",
                CheckOnClick = true,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnGuardar = new Button
            {
                Text = "Guardar Relaciones",
                Location = new Point(15, 515),
                Size = new Size(190, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += BtnGuardar_Click;

            btnActualizar = new Button
            {
                Text = "Actualizar Vista",
                Location = new Point(220, 515),
                Size = new Size(150, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnActualizar.FlatAppearance.BorderSize = 0;
            btnActualizar.Click += (s, e) => CargarRelacionesExistentes();

            pnlConfiguracion.Controls.AddRange(new Control[]
            {
                lblSeccionConfig, lblTipoAnalisis, cboTiposAnalisis,
                lblMetricas, clbMetricas, btnGuardar, btnActualizar
            });

            // Panel derecho - Relaciones Existentes
            var pnlRelaciones = new Panel
            {
                Location = new Point(490, 75),
                Size = new Size(590, 590),
                BackColor = Color.FromArgb(250, 252, 255),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblSeccionRelaciones = new Label
            {
                Text = "Relaciones Existentes",
                Location = new Point(0, 0),
                Size = new Size(590, 35),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(230, 126, 34),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            gridRelaciones = new DataGridView
            {
                Location = new Point(15, 50),
                Size = new Size(560, 470),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(230, 126, 34),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Padding = new Padding(5),
                    WrapMode = DataGridViewTriState.True
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9),
                    SelectionBackColor = Color.FromArgb(255, 224, 178),
                    SelectionForeColor = Color.FromArgb(44, 62, 80),
                    Padding = new Padding(5),
                    WrapMode = DataGridViewTriState.False
                },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(255, 250, 245)
                },
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            };

            btnEliminarRelacion = new Button
            {
                Text = "Eliminar Relación",
                Location = new Point(15, 535),
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(192, 57, 43),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEliminarRelacion.FlatAppearance.BorderSize = 0;
            btnEliminarRelacion.Click += BtnEliminarRelacion_Click;

            pnlRelaciones.Controls.AddRange(new Control[]
            {
                lblSeccionRelaciones, gridRelaciones, btnEliminarRelacion
            });

            // Botón Cerrar (global)
            btnCancelar = new Button
            {
                Text = "Cerrar",
                Location = new Point(990, 680),
                Size = new Size(90, 35),
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            Controls.AddRange(new Control[]
            {
                lblTitulo, lblSubtitulo,
                pnlConfiguracion, pnlRelaciones,
                btnCancelar
            });

            CancelButton = btnCancelar;
        }

        private void CargarDatos()
        {
            try
            {
                // Cargar tipos de análisis activos
                _tiposAnalisis = _catalogoService.ObtenerTiposAnalisisActivos().ToList();
                cboTiposAnalisis.DataSource = _tiposAnalisis;

                // Cargar métricas activas
                _metricas = _catalogoService.ObtenerMetricasActivas().ToList();
                var metricasConNombreCompleto = _metricas.Select(m => new
                {
                    IdMetrica = m.IdMetrica,
                    NombreCompleto = string.Format("{0} ({1})", m.Nombre, m.UnidadMedida),
                    Metrica = m
                }).ToList();

                clbMetricas.DataSource = metricasConNombreCompleto;
                clbMetricas.DisplayMember = "NombreCompleto";
                clbMetricas.ValueMember = "IdMetrica";

                CargarRelacionesExistentes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al cargar datos: {0}", ex.Message), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarRelacionesExistentes()
        {
            try
            {
                _relaciones = _catalogoService.ObtenerRelacionesTipoAnalisisMetricas().ToList();
                gridRelaciones.DataSource = _relaciones.Select(r => new
                {
                    TipoAnalisis = r.DescripcionTipoAnalisis,
                    Metrica = r.NombreMetrica,
                    Unidad = r.UnidadMedidaMetrica,
                    IdTipoAnalisis = r.IdTipoAnalisis,
                    IdMetrica = r.IdMetrica
                }).ToList();

                // Ocultar las columnas de IDs
                if (gridRelaciones.Columns["IdTipoAnalisis"] != null)
                    gridRelaciones.Columns["IdTipoAnalisis"].Visible = false;
                if (gridRelaciones.Columns["IdMetrica"] != null)
                    gridRelaciones.Columns["IdMetrica"].Visible = false;

                // Configurar ancho de columnas visibles
                if (gridRelaciones.Columns["TipoAnalisis"] != null)
                {
                    gridRelaciones.Columns["TipoAnalisis"].HeaderText = "Tipo de Análisis";
                    gridRelaciones.Columns["TipoAnalisis"].Width = 240;
                    gridRelaciones.Columns["TipoAnalisis"].MinimumWidth = 200;
                }

                if (gridRelaciones.Columns["Metrica"] != null)
                {
                    gridRelaciones.Columns["Metrica"].HeaderText = "Métrica";
                    gridRelaciones.Columns["Metrica"].Width = 220;
                    gridRelaciones.Columns["Metrica"].MinimumWidth = 180;
                }

                if (gridRelaciones.Columns["Unidad"] != null)
                {
                    gridRelaciones.Columns["Unidad"].HeaderText = "Unidad";
                    gridRelaciones.Columns["Unidad"].Width = 100;
                    gridRelaciones.Columns["Unidad"].MinimumWidth = 80;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al cargar relaciones: {0}", ex.Message), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboTiposAnalisis_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTiposAnalisis.SelectedItem == null) return;

            var tipoAnalisisSeleccionado = (TipoAnalisis)cboTiposAnalisis.SelectedItem;

            try
            {
                // Obtener métricas asociadas al tipo de análisis seleccionado
                var metricasAsociadas = _catalogoService.ObtenerMetricasPorTipoAnalisis(tipoAnalisisSeleccionado.IdTipoAnalisis)
                    .Select(m => m.IdMetrica).ToList();

                // Marcar/desmarcar las métricas en el CheckedListBox
                for (int i = 0; i < clbMetricas.Items.Count; i++)
                {
                    var item = clbMetricas.Items[i];
                    // Obtener el valor usando reflexión en lugar de GetItemValue que no existe
                    var metricaInfo = item.GetType().GetProperty("IdMetrica");
                    if (metricaInfo != null)
                    {
                        var metricaId = (int)metricaInfo.GetValue(item);
                        clbMetricas.SetItemChecked(i, metricasAsociadas.Contains(metricaId));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al cargar métricas del tipo de análisis: {0}", ex.Message), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (cboTiposAnalisis.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un tipo de análisis.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var tipoAnalisisSeleccionado = (TipoAnalisis)cboTiposAnalisis.SelectedItem;
            var metricasSeleccionadas = new List<int>();

            // Obtener métricas marcadas
            for (int i = 0; i < clbMetricas.Items.Count; i++)
            {
                if (clbMetricas.GetItemChecked(i))
                {
                    var item = clbMetricas.Items[i];
                    var metricaInfo = item.GetType().GetProperty("IdMetrica");
                    if (metricaInfo != null)
                    {
                        metricasSeleccionadas.Add((int)metricaInfo.GetValue(item));
                    }
                }
            }

            if (metricasSeleccionadas.Count == 0)
            {
                var resultado = MessageBox.Show(
                    "No ha seleccionado ninguna métrica. Esto eliminará todas las relaciones existentes para este tipo de análisis. ¿Continuar?",
                    "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado != DialogResult.Yes) return;
            }

            try
            {
                _catalogoService.ActualizarRelacionesTipoAnalisis(tipoAnalisisSeleccionado.IdTipoAnalisis, metricasSeleccionadas);
                CargarRelacionesExistentes();
                MessageBox.Show("Relaciones actualizadas correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al guardar relaciones: {0}", ex.Message), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminarRelacion_Click(object sender, EventArgs e)
        {
            if (gridRelaciones.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Seleccione una relación para eliminar.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var relacionSeleccionada = gridRelaciones.CurrentRow.DataBoundItem;
            var idTipoAnalisisProp = relacionSeleccionada.GetType().GetProperty("IdTipoAnalisis");
            var idMetricaProp = relacionSeleccionada.GetType().GetProperty("IdMetrica");

            if (idTipoAnalisisProp == null || idMetricaProp == null) return;

            var idTipoAnalisis = (int)idTipoAnalisisProp.GetValue(relacionSeleccionada);
            var idMetrica = (int)idMetricaProp.GetValue(relacionSeleccionada);

            var resultado = MessageBox.Show("¿Está seguro que desea eliminar esta relación?",
                "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado != DialogResult.Yes) return;

            try
            {
                _catalogoService.EliminarRelacionTipoAnalisisMetrica(idTipoAnalisis, idMetrica);
                CargarRelacionesExistentes();

                // Actualizar la vista si el tipo de análisis eliminado está seleccionado
                if (cboTiposAnalisis.SelectedItem is TipoAnalisis tipoSelected &&
                    tipoSelected.IdTipoAnalisis == idTipoAnalisis)
                {
                    CboTiposAnalisis_SelectedIndexChanged(cboTiposAnalisis, EventArgs.Empty);
                }

                MessageBox.Show("Relación eliminada correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error al eliminar relación: {0}", ex.Message), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}