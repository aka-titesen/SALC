using System;

namespace SALC.Views.Interfaces
{
    public interface IReportsView
    {
        event EventHandler GenerateRequested;
        event EventHandler ExportPdfRequested;
        event EventHandler CloseRequested;
        void ShowMessage(string title, string message);
    }
}
