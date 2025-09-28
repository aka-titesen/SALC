using System;

namespace SALC.Views.Interfaces
{
    public interface IAppointmentsView
    {
        event EventHandler CreateRequested;
        event EventHandler EditRequested;
        event EventHandler DeleteRequested;
        event EventHandler CloseRequested;
        void ShowMessage(string title, string message);
    }
}
