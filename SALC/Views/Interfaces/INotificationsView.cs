using System;

namespace SALC.Views.Interfaces
{
    public interface INotificationsView
    {
        event EventHandler SendRequested;
        event EventHandler CloseRequested;
        void ShowMessage(string title, string message);
    }
}
