using System;
using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;
using SALC.Views.Compartidos;

namespace SALC.Views.PanelAsistente
{
    public class FrmPanelAsistente : Form, IPanelAsistenteView
    {
        private TabControl tabs;
        public FrmPanelAsistente()
        {
            Text = "Panel de Asistente";
            Width = 1000;
            Height = 700;
            tabs = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(tabs);
            AgregarTabHistorial();
            AgregarTabGenerarInforme();
        }

        private void AgregarTabHistorial()
        {
            var tab = new TabPage("Historial de Pacientes");
            var selPaciente = new SeleccionPacienteControl { Left = 20, Top = 20 };
            var btnBuscar = new Button { Left = 240, Top = 18, Width = 120, Text = "Buscar historial" };
            var grid = new DataGridView { Left = 20, Top = 60, Width = 900, Height = 520, ReadOnly = true, AllowUserToAddRows = false };
            btnBuscar.Click += (s, e) => BuscarHistorialClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selPaciente);
            tab.Controls.Add(btnBuscar);
            tab.Controls.Add(grid);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabGenerarInforme()
        {
            var tab = new TabPage("Generar Informe Verificado");
            var selAnalisis = new SeleccionAnalisisControl { Left = 20, Top = 20 };
            var btnGenerar = new Button { Left = 20, Top = 60, Width = 200, Text = "Generar informe" };
            btnGenerar.Click += (s, e) => GenerarInformeClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selAnalisis);
            tab.Controls.Add(btnGenerar);
            tabs.TabPages.Add(tab);
        }

        public event EventHandler BuscarHistorialClick;
        public event EventHandler GenerarInformeClick;
    }
}
