using System;

namespace SALC.Views.Interfaces
{
    public interface ILoginView
    {
        string Username { get; }
        string Password { get; }

        event EventHandler LoginRequested;

        void ShowValidationMessage(string message);
        void ShowLoginError(string message);
        void NavigateToDashboard();
    }
}
