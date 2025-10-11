using System;
using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;
using SALC.Views.Compartidos;

namespace SALC.Views.PanelMedico
{
    public class FrmPanelMedico : Form, IPanelMedicoView
    {
        private TabControl tabs;

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
            var selPaciente = new SeleccionPacienteControl { Left = 20, Top = 20 };
            var cboTipoAnalisis = new ComboBox { Left = 20, Top = 60, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            var txtObservaciones = new TextBox { Left = 20, Top = 100, Width = 600, Height = 80, Multiline = true };
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
            var selAnalisis = new SeleccionAnalisisControl { Left = 20, Top = 20 };
            var grid = new DataGridView { Left = 20, Top = 60, Width = 900, Height = 500, AllowUserToAddRows = false };
            var btnGuardar = new Button { Left = 820, Top = 570, Width = 100, Text = "Guardar" };
            btnGuardar.Click += (s, e) => CargarResultadosGuardarClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selAnalisis);
            tab.Controls.Add(grid);
            tab.Controls.Add(btnGuardar);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabValidarFirmar()
        {
            var tab = new TabPage("Validar / Firmar");
            var selAnalisis = new SeleccionAnalisisControl { Left = 20, Top = 20 };
            var btnFirmar = new Button { Left = 20, Top = 60, Width = 160, Text = "Firmar análisis" };
            btnFirmar.Click += (s, e) => FirmarAnalisisClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selAnalisis);
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
    }
}
