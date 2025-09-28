using System;

namespace SALC.Views.Interfaces
{
    public interface IStudiesView
    {
        event EventHandler CreateRequested;
        event EventHandler EditRequested;
        event EventHandler DeleteRequested;
        event EventHandler SearchRequested;
        event EventHandler CloseRequested;
        string SearchText { get; }
        void ShowMessage(string title, string message);
    }
}
