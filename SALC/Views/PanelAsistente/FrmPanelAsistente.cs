using System;
using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;
using SALC.Views.Compartidos;
using System.Collections.Generic;
using SALC.Domain;

namespace SALC.Views.PanelAsistente
{
    public class FrmPanelAsistente : Form, IPanelAsistenteView
    {
        private TabControl tabs;
        private SeleccionPacienteControl selPacienteHist;
        private DataGridView gridHistorial;
        private SeleccionAnalisisControl selAnalisisInforme;
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
            selPacienteHist = new SeleccionPacienteControl { Left = 20, Top = 20 };
            var btnBuscar = new Button { Left = 240, Top = 18, Width = 120, Text = "Buscar historial" };
            gridHistorial = new DataGridView { Left = 20, Top = 60, Width = 900, Height = 520, ReadOnly = true, AllowUserToAddRows = false, AutoGenerateColumns = true };
            btnBuscar.Click += (s, e) => BuscarHistorialClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selPacienteHist);
            tab.Controls.Add(btnBuscar);
            tab.Controls.Add(gridHistorial);
            tabs.TabPages.Add(tab);
        }

        private void AgregarTabGenerarInforme()
        {
            var tab = new TabPage("Generar Informe Verificado");
            selAnalisisInforme = new SeleccionAnalisisControl { Left = 20, Top = 20 };
            var btnGenerar = new Button { Left = 20, Top = 60, Width = 200, Text = "Generar informe" };
            btnGenerar.Click += (s, e) => GenerarInformeClick?.Invoke(this, EventArgs.Empty);
            tab.Controls.Add(selAnalisisInforme);
            tab.Controls.Add(btnGenerar);
            tabs.TabPages.Add(tab);
        }

        public event EventHandler BuscarHistorialClick;
        public event EventHandler GenerarInformeClick;

        // IPanelAsistenteView
        public string HistorialDniPacienteTexto => selPacienteHist?.DniTexto;
        public void CargarHistorialAnalisis(IEnumerable<Analisis> analisis)
        {
            gridHistorial.DataSource = null;
            gridHistorial.DataSource = new List<Analisis>(analisis);
        }
        public string AnalisisIdParaInformeTexto => selAnalisisInforme?.IdAnalisisTexto;
        public void MostrarMensaje(string texto, bool esError = false)
        {
            MessageBox.Show(this, texto, "Asistente", MessageBoxButtons.OK, esError ? MessageBoxIcon.Error : MessageBoxIcon.Information);
        }
    }
}
