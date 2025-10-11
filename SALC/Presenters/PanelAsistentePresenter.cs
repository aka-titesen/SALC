using SALC.Presenters.ViewsContracts;

namespace SALC.Presenters
{
    public class PanelAsistentePresenter
    {
        private readonly IPanelAsistenteView _view;

        public PanelAsistentePresenter(IPanelAsistenteView view)
        {
            _view = view;
        }
    }
}
