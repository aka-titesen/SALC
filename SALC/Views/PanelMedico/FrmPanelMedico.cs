using System;
using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;
using SALC.Presenters;
using SALC.Views.Compartidos;

namespace SALC.Views.PanelMedico
{
    public class FrmPanelMedico : Form, IPanelMedicoView
    {
        private TabControl tabs;
        // Crear análisis
        private Compartidos.SeleccionPacienteControl selPaciente;
        private ComboBox cboTipoAnalisis;
        private TextBox txtObservaciones;

        // Cargar resultados
        private Compartidos.SeleccionAnalisisControl selAnalisisResultados;
        private DataGridView gridResultados;

        // Validar
        private Compartidos.SeleccionAnalisisControl selAnalisisFirmar;

        public FrmPanelMedico()
        {
            Text = "Panel de Médico";
            Width = 1000;
            Height = 700;
            tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);

            AgregarTabCrearAnalisis();
            AgregarTabCargarResultados();
            AgregarTabValidarFirmar();
            AgregarTabGenerarInforme();
        }

        private void AgregarTabCrearAnalisis()
        {
            var tab = new TabPage("Crear Análisis");
            selPaciente = new SeleccionPacienteControl { Left = 20, Top = 20 };
            cboTipoAnalisis = new ComboBox { Left = 20, Top = 60, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = "Descripcion", ValueMember = "IdTipoAnalisis" };
            txtObservaciones = new TextBox { Left = 20, Top = 100, Width = 600, Height = 80, Multiline = true };
            var btnCrear = new Button { Left = 520, Top = 200, Width = 100, Text = "Crear" };
            btnCrear.Click += (s, e) => CrearAnalisisClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selPaciente);
            tab.Controls.Add(new Label { Left = 20, Top = 45, Text = "Tipo de análisis:" });
            tab.Controls.Add(cboTipoAnalisis);
            tab.Controls.Add(new Label { Left = 20, Top = 85, Text = "Observaciones:" });
            tab.Controls.Add(txtObservaciones);
            tab.Controls.Add(btnCrear);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabCargarResultados()
        {
            var tab = new TabPage("Cargar Resultados");
            selAnalisisResultados = new SeleccionAnalisisControl { Left = 20, Top = 20 };
            gridResultados = new DataGridView { Left = 20, Top = 60, Width = 900, Height = 500, AllowUserToAddRows = false, AutoGenerateColumns = true };
            var btnGuardar = new Button { Left = 820, Top = 570, Width = 100, Text = "Guardar" };
            btnGuardar.Click += (s, e) => CargarResultadosGuardarClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selAnalisisResultados);
            tab.Controls.Add(gridResultados);
            tab.Controls.Add(btnGuardar);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabValidarFirmar()
        {
            var tab = new TabPage("Validar / Firmar");
            selAnalisisFirmar = new SeleccionAnalisisControl { Left = 20, Top = 20 };
            var btnFirmar = new Button { Left = 20, Top = 60, Width = 160, Text = "Firmar análisis" };
            btnFirmar.Click += (s, e) => FirmarAnalisisClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selAnalisisFirmar);
            tab.Controls.Add(btnFirmar);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabGenerarInforme()
        {
            var tab = new TabPage("Generar Informe");
            var selAnalisis = new SeleccionAnalisisControl { Left = 20, Top = 20 };
            var btnGenerar = new Button { Left = 20, Top = 60, Width = 160, Text = "Generar informe" };
            btnGenerar.Click += (s, e) => GenerarInformeClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selAnalisis);
            tab.Controls.Add(btnGenerar);
            tabs.TabPages.Add(tab);
        }

        public event EventHandler CrearAnalisisClick;
        public event EventHandler CargarResultadosGuardarClick;
        public event EventHandler FirmarAnalisisClick;
        public event EventHandler GenerarInformeClick;

        // IPanelMedicoView impl
        public string CrearAnalisisDniPacienteTexto => selPaciente?.DniTexto;
        public int? TipoAnalisisSeleccionadoId => cboTipoAnalisis?.SelectedValue as int? ?? (cboTipoAnalisis?.SelectedValue != null ? (int?)Convert.ToInt32(cboTipoAnalisis.SelectedValue) : null);
        public string CrearAnalisisObservaciones => txtObservaciones?.Text;
        public void CargarTiposAnalisis(System.Collections.Generic.IEnumerable<SALC.Domain.TipoAnalisis> tipos)
        {
            cboTipoAnalisis.DataSource = null;
            cboTipoAnalisis.DataSource = new System.Collections.Generic.List<SALC.Domain.TipoAnalisis>(tipos);
        }
        public string AnalisisIdParaResultadosTexto => selAnalisisResultados?.IdAnalisisTexto;
        public void CargarResultadosParaEdicion(System.Collections.Generic.IList<SALC.Presenters.ResultadoEdicionDto> filas)
        {
            gridResultados.DataSource = null;
            gridResultados.DataSource = filas;
        }
        public System.Collections.Generic.IList<SALC.Presenters.ResultadoEdicionDto> LeerResultadosEditados()
        {
            var list = new System.Collections.Generic.List<SALC.Presenters.ResultadoEdicionDto>();
            foreach (DataGridViewRow row in gridResultados.Rows)
            {
                if (row.DataBoundItem is SALC.Presenters.ResultadoEdicionDto dto)
                {
                    list.Add(dto);
                }
                else
                {
                    var dto2 = new SALC.Presenters.ResultadoEdicionDto
                    {
                        IdMetrica = Convert.ToInt32(row.Cells[nameof(ResultadoEdicionDto.IdMetrica)].Value),
                        Nombre = Convert.ToString(row.Cells[nameof(ResultadoEdicionDto.Nombre)].Value),
                        Unidad = Convert.ToString(row.Cells[nameof(ResultadoEdicionDto.Unidad)].Value),
                        Min = row.Cells[nameof(ResultadoEdicionDto.Min)].Value as decimal?,
                        Max = row.Cells[nameof(ResultadoEdicionDto.Max)].Value as decimal?,
                        Resultado = row.Cells[nameof(ResultadoEdicionDto.Resultado)].Value as decimal?,
                        Observaciones = Convert.ToString(row.Cells[nameof(ResultadoEdicionDto.Observaciones)].Value)
                    };
                    list.Add(dto2);
                }
            }
            return list;
        }
        public string AnalisisIdParaFirmaTexto => selAnalisisFirmar?.IdAnalisisTexto;
        public void MostrarMensaje(string texto, bool esError = false)
        {
            MessageBox.Show(this, texto, "Médico", MessageBoxButtons.OK, esError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }
    }
}
