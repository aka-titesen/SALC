using SALC.BLL;
using SALC.Presenters.ViewsContracts;

namespace SALC.Presenters
{
    public class LoginPresenter
    {
        private readonly ILoginView _view;
        private readonly IAutenticacionService _authService;

        public LoginPresenter(ILoginView view, IAutenticacionService authService)
        {
            _view = view;
            _authService = authService;
        }
    }
}
