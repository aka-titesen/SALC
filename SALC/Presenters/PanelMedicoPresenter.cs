using SALC.Presenters.ViewsContracts;

namespace SALC.Presenters
{
    public class PanelMedicoPresenter
    {
        private readonly IPanelMedicoView _view;

        public PanelMedicoPresenter(IPanelMedicoView view)
        {
            _view = view;
        }
    }
}
