using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelMedico
{
    public class FrmPanelMedico : Form, IPanelMedicoView
    {
        public FrmPanelMedico()
        {
            Text = "Panel de Médico";
            Width = 1000;
            Height = 700;
        }
    }
}
