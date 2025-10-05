// Views/Interfaces/ISystemConfigView.cs
using System;
using SALC.Services;

namespace SALC.Views.Interfaces
{
    /// <summary>
    /// Interface para configuración del sistema (solo Administrador)
    /// </summary>
    public interface ISystemConfigView
    {
        // Eventos
        event EventHandler LoadConfiguration;
        event EventHandler<SystemConfigService.SystemConfig> SaveConfiguration;
        event EventHandler TestBackupPath;
        event EventHandler ViewSystemStats;
        event EventHandler CloseRequested;

        // Propiedades
        SystemConfigService.SystemConfig CurrentConfig { get; set; }
        SystemConfigService.SystemStats SystemStats { get; set; }

        // Métodos
        void LoadConfigurationData(SystemConfigService.SystemConfig config);
        void LoadSystemStatsData(SystemConfigService.SystemStats stats);
        void ShowMessage(string title, string message, System.Windows.Forms.MessageBoxIcon icon);
        void ShowValidationError(string field, string error);
        void ClearValidationErrors();
    }
}