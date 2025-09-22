using System;

namespace SALC.Views.Interfaces
{
    public interface ISecurityView
    {
        event EventHandler RefreshRequested;
        event EventHandler CloseRequested;
        void ShowMessage(string title, string message);
    }
}
