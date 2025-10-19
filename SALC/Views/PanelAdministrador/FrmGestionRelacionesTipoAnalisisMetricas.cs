using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
        private Button btnGuardar;
        private Button btnCancelar;
        private Button btnEliminarRelacion;
        
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
            Text = "Gestión de Relaciones Tipo Análisis - Métricas";
            Size = new System.Drawing.Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // Panel izquierdo para configurar relaciones
            var pnlConfiguracion = new Panel
            {
                Dock = DockStyle.Left,
                Width = 400,
                Padding = new Padding(10)
            };

            var lblTipoAnalisis = new Label
            {
                Text = "Seleccionar Tipo de Análisis:",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(180, 20)
            };

            cboTiposAnalisis = new ComboBox
            {
                Location = new System.Drawing.Point(10, 35),
                Size = new System.Drawing.Size(350, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Descripcion",
                ValueMember = "IdTipoAnalisis"
            };
            cboTiposAnalisis.SelectedIndexChanged += CboTiposAnalisis_SelectedIndexChanged;

            var lblMetricas = new Label
            {
                Text = "Métricas disponibles (marcar las que componen el análisis):",
                Location = new System.Drawing.Point(10, 70),
                Size = new System.Drawing.Size(350, 20)
            };

            clbMetricas = new CheckedListBox
            {
                Location = new System.Drawing.Point(10, 95),
                Size = new System.Drawing.Size(350, 300),
                DisplayMember = "NombreCompleto",
                ValueMember = "IdMetrica",
                CheckOnClick = true
            };

            btnGuardar = new Button
            {
                Text = "Guardar Relaciones",
                Location = new System.Drawing.Point(10, 410),
                Size = new System.Drawing.Size(150, 30)
            };
            btnGuardar.Click += BtnGuardar_Click;

            var btnActualizar = new Button
            {
                Text = "Actualizar Vista",
                Location = new System.Drawing.Point(170, 410),
                Size = new System.Drawing.Size(120, 30)
            };
            btnActualizar.Click += (s, e) => CargarRelacionesExistentes();

            pnlConfiguracion.Controls.AddRange(new Control[] { 
                lblTipoAnalisis, cboTiposAnalisis, lblMetricas, clbMetricas, btnGuardar, btnActualizar 
            });

            // Panel derecho para mostrar relaciones existentes
            var pnlRelaciones = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var lblRelacionesExistentes = new Label
            {
                Text = "Relaciones Existentes:",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(200, 20)
            };

            gridRelaciones = new DataGridView
            {
                Location = new System.Drawing.Point(10, 35),
                Size = new System.Drawing.Size(460, 350),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnEliminarRelacion = new Button
            {
                Text = "Eliminar Relación Seleccionada",
                Location = new System.Drawing.Point(10, 395),
                Size = new System.Drawing.Size(200, 30)
            };
            btnEliminarRelacion.Click += BtnEliminarRelacion_Click;

            btnCancelar = new Button
            {
                Text = "Cerrar",
                Location = new System.Drawing.Point(350, 395),
                Size = new System.Drawing.Size(100, 30),
                DialogResult = DialogResult.Cancel
            };

            pnlRelaciones.Controls.AddRange(new Control[] { 
                lblRelacionesExistentes, gridRelaciones, btnEliminarRelacion, btnCancelar 
            });

            // Añadir paneles al formulario
            Controls.Add(pnlRelaciones);
            Controls.Add(pnlConfiguracion);

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
                    NombreCompleto = $"{m.Nombre} ({m.UnidadMedida})",
                    Metrica = m
                }).ToList();

                clbMetricas.DataSource = metricasConNombreCompleto;
                clbMetricas.DisplayMember = "NombreCompleto";
                clbMetricas.ValueMember = "IdMetrica";

                CargarRelacionesExistentes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", 
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar relaciones: {ex.Message}", "Error", 
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
                MessageBox.Show($"Error al cargar métricas del tipo de análisis: {ex.Message}", "Error", 
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
                MessageBox.Show($"Error al guardar relaciones: {ex.Message}", "Error", 
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
                MessageBox.Show($"Error al eliminar relación: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}