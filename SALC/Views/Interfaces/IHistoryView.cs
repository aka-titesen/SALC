using System;

namespace SALC.Views.Interfaces
{
    public interface IHistoryView
    {
        event EventHandler RefreshRequested;
        event EventHandler CloseRequested;
        void ShowMessage(string title, string message);
    }
}
