using System;

namespace SALC.Views.Interfaces
{
    public interface IResultsView
    {
        event EventHandler LoadRequested;
        event EventHandler ValidateRequested;
        event EventHandler SaveRequested;
        event EventHandler CloseRequested;
        void ShowMessage(string title, string message);
    }
}
