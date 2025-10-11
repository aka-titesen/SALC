using System.Windows.Forms;
using SALC.Presenters.ViewsContracts;

namespace SALC.Views.PanelAsistente
{
    public class FrmPanelAsistente : Form, IPanelAsistenteView
    {
        public FrmPanelAsistente()
        {
            Text = "Panel de Asistente";
            Width = 1000;
            Height = 700;
        }
    }
}
