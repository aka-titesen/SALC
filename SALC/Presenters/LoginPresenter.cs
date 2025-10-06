using System;
using SALC.Views.Interfaces;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para manejar la l�gica de login
    /// </summary>
    public class LoginPresenter
    {
        private readonly ILoginView _view;

        public LoginPresenter(ILoginView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.LoginRequested += OnLoginRequested;
        }

        private void OnLoginRequested(object sender, EventArgs e)
        {
            var username = _view.Username?.Trim();
            var password = _view.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _view.ShowValidationMessage("Por favor, ingrese usuario (DNI) y contrase�a.");
                return;
            }

            var ok = UserAuthentication.Login(username, password);
            if (!ok)
            {
                _view.ShowLoginError("Usuario o contrase�a incorrectos.");
                return;
            }

            _view.NavigateToDashboard();
        }
    }
}
