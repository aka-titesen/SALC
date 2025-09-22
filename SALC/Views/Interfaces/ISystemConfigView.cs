using System;

namespace SALC.Views.Interfaces
{
    public interface ISystemConfigView
    {
        event EventHandler SaveRequested;
        event EventHandler CloseRequested;
        void ShowMessage(string title, string message);
    }
}
