using System;

namespace SALC.Views.Interfaces
{
    public interface IBackupsView
    {
        event EventHandler RunBackupRequested;
        event EventHandler RestoreRequested;
        event EventHandler CloseRequested;
        void ShowMessage(string title, string message);
    }
}
