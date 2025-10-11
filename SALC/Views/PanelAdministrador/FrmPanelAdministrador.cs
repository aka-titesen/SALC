using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelAdministrador
{
    public class FrmPanelAdministrador : Form, IPanelAdministradorView
    {
        public FrmPanelAdministrador()
        {
            Text = "Panel de Administrador";
            Width = 1000;
            Height = 700;
        }
    }
}
