using System;
using System.Windows.Forms;

namespace SALC.Views.Compartidos
{
    public class SeleccionPacienteControl : UserControl
    {
        public event EventHandler BuscarClick;
        private TextBox txtDni;
        private Button btnBuscar;

        public string DniTexto => txtDni.Text?.Trim();

        public SeleccionPacienteControl()
        {
            Height = 32;
            txtDni = new TextBox { Left = 0, Top = 5, Width = 120 };
            btnBuscar = new Button { Left = 130, Top = 3, Width = 80, Text = "Buscar" };
            Controls.Add(txtDni);
            Controls.Add(btnBuscar);
            btnBuscar.Click += (s, e) => BuscarClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
