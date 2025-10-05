// Views/Interfaces/IBackupManagementView.cs
using System;
using System.Collections.Generic;
using SALC.Services;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interface para gestión de backups (solo Administrador)
    /// </summary>
    public interface IBackupManagementView
    {
        // Eventos de backup
        event EventHandler ExecuteBackup;
        event EventHandler<string> RestoreFromBackup;
        event EventHandler LoadBackupList;
        event EventHandler<int> DeleteBackup;
        event EventHandler CleanupOldBackups;
        
        // Eventos de programación
        event EventHandler<TimeSpan> ScheduleDailyBackup;
        event EventHandler DisableScheduledBackup;
        
        // Eventos de navegación
        event EventHandler BrowseBackupPath;
        event EventHandler CloseRequested;

        // Propiedades
        List<BackupService.BackupInfo> AvailableBackups { get; set; }
        string BackupPath { get; set; }
        bool BackupInProgress { get; set; }
        bool RestoreInProgress { get; set; }
        
        // Métodos
        void LoadBackupsData(List<BackupService.BackupInfo> backups);
        void ShowBackupResult(BackupService.BackupResult result);
        void ShowRestoreResult(BackupService.BackupResult result);
        void ShowMessage(string title, string message, System.Windows.Forms.MessageBoxIcon icon);
        void UpdateProgress(string status, int percentage);
        void SetControlsEnabled(bool enabled);
        
        // Propiedades de configuración
        TimeSpan ScheduledBackupTime { get; set; }
        bool AutoBackupEnabled { get; set; }
        int RetentionDays { get; set; }
    }
}