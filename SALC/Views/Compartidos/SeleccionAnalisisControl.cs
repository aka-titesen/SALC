using System;
using System.Windows.Forms;

namespace SALC.Views.Compartidos
{
    public class SeleccionAnalisisControl : UserControl
    {
        public event EventHandler BuscarClick;
        private TextBox txtIdAnalisis;
        private Button btnBuscar;

        public string IdAnalisisTexto => txtIdAnalisis.Text?.Trim();

        public SeleccionAnalisisControl()
        {
            Height = 32;
            txtIdAnalisis = new TextBox { Left = 0, Top = 5, Width = 120 };
            btnBuscar = new Button { Left = 130, Top = 3, Width = 80, Text = "Buscar" };
            Controls.Add(txtIdAnalisis);
            Controls.Add(btnBuscar);
            btnBuscar.Click += (s, e) => BuscarClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
