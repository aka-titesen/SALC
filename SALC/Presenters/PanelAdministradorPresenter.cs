using SALC.Presenters.ViewsContracts;

namespace SALC.Presenters
{
    public class PanelAdministradorPresenter
    {
        private readonly IPanelAdministradorView _view;

        public PanelAdministradorPresenter(IPanelAdministradorView view)
        {
            _view = view;
        }
    }
}
