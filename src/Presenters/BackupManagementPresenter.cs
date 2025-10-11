// Presenters/BackupManagementPresenter.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SALC.Views.Interfaces;
using SALC.Services;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para gesti�n de backups (solo Administrador)
    /// Implementa operaciones de backup, restore y programaci�n seg�n ERS-SALC_IEEE830
    /// </summary>
    public class BackupManagementPresenter
    {
        private readonly IBackupManagementView _view;
        private readonly BackupService _backupService;
        private readonly SystemConfigService _configService;

        public BackupManagementPresenter(IBackupManagementView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _backupService = new BackupService();
            _configService = new SystemConfigService();
            
            SubscribeToViewEvents();
            LoadInitialData();
        }

        private void SubscribeToViewEvents()
        {
            _view.ExecuteBackup += OnExecuteBackup;
            _view.RestoreFromBackup += OnRestoreFromBackup;
            _view.LoadBackupList += OnLoadBackupList;
            _view.DeleteBackup += OnDeleteBackup;
            _view.CleanupOldBackups += OnCleanupOldBackups;
            _view.ScheduleDailyBackup += OnScheduleDailyBackup;
            _view.DisableScheduledBackup += OnDisableScheduledBackup;
            _view.BrowseBackupPath += OnBrowseBackupPath;
        }

        private void LoadInitialData()
        {
            try
            {
                // Cargar configuraci�n actual
                var config = _configService.GetSystemConfig();
                _view.BackupPath = config.BackupPath;

                // Cargar lista de backups
                OnLoadBackupList(null, EventArgs.Empty);

                // Cargar configuraci�n de backup autom�tico
                LoadBackupScheduleConfig();
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al cargar datos iniciales: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void LoadBackupScheduleConfig()
        {
            try
            {
                // Leer configuraci�n de backup autom�tico desde App.config
                var autoBackupEnabled = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["AutoBackupEnabled"] ?? "false");
                var autoBackupTime = TimeSpan.Parse(System.Configuration.ConfigurationManager.AppSettings["AutoBackupTime"] ?? "02:00:00");

                _view.AutoBackupEnabled = autoBackupEnabled;
                _view.ScheduledBackupTime = autoBackupTime;
                _view.RetentionDays = 7; // Seg�n ERS: retenci�n 7 d�as
            }
            catch
            {
                // Valores por defecto si hay error
                _view.AutoBackupEnabled = false;
                _view.ScheduledBackupTime = new TimeSpan(2, 0, 0); // 02:00 AM
                _view.RetentionDays = 7;
            }
        }

        private async void OnExecuteBackup(object sender, EventArgs e)
        {
            try
            {
                _view.BackupInProgress = true;
                _view.SetControlsEnabled(false);
                _view.UpdateProgress("Iniciando backup...", 0);

                // Ejecutar backup en background thread
                var result = await Task.Run(() => _backupService.ExecuteFullBackup());

                _view.ShowBackupResult(result);

                if (result.Success)
                {
                    _view.UpdateProgress("Backup completado", 100);
                    // Recargar lista de backups
                    OnLoadBackupList(sender, EventArgs.Empty);
                }
                else
                {
                    _view.UpdateProgress("Backup fall�", 0);
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error durante el backup: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
                _view.UpdateProgress("Error en backup", 0);
            }
            finally
            {
                _view.BackupInProgress = false;
                _view.SetControlsEnabled(true);
            }
        }

        private async void OnRestoreFromBackup(object sender, string backupFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(backupFilePath))
                {
                    _view.ShowMessage("Validaci�n", "Debe seleccionar un archivo de backup", 
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                // Confirmaci�n cr�tica para restore
                var confirmResult = System.Windows.Forms.MessageBox.Show(
                    "?? ADVERTENCIA CR�TICA ??\n\n" +
                    "Esta operaci�n reemplazar� COMPLETAMENTE la base de datos actual con el backup seleccionado.\n\n" +
                    "� Se perder�n TODOS los datos ingresados despu�s de la fecha del backup\n" +
                    "� Esta operaci�n NO se puede deshacer\n" +
                    "� Se recomienda crear un backup actual antes de continuar\n\n" +
                    $"Backup seleccionado: {System.IO.Path.GetFileName(backupFilePath)}\n\n" +
                    "�Est� ABSOLUTAMENTE seguro que desea continuar?",
                    "CONFIRMAR RESTORE - OPERACI�N DESTRUCTIVA",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Warning,
                    System.Windows.Forms.MessageBoxDefaultButton.Button2);

                if (confirmResult != System.Windows.Forms.DialogResult.Yes)
                    return;

                _view.RestoreInProgress = true;
                _view.SetControlsEnabled(false);
                _view.UpdateProgress("Iniciando restore...", 0);

                // Ejecutar restore en background thread
                var result = await Task.Run(() => _backupService.RestoreFromBackup(backupFilePath, forceReplace: true));

                _view.ShowRestoreResult(result);

                if (result.Success)
                {
                    _view.UpdateProgress("Restore completado", 100);
                    _view.ShowMessage("Restore Completado", 
                        "La base de datos ha sido restaurada exitosamente.\n\nLa aplicaci�n se cerrar� para aplicar los cambios.", 
                        System.Windows.Forms.MessageBoxIcon.Information);
                    
                    // Cerrar aplicaci�n para reiniciar conexiones
                    System.Windows.Forms.Application.Exit();
                }
                else
                {
                    _view.UpdateProgress("Restore fall�", 0);
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error durante el restore: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
                _view.UpdateProgress("Error en restore", 0);
            }
            finally
            {
                _view.RestoreInProgress = false;
                _view.SetControlsEnabled(true);
            }
        }

        private void OnLoadBackupList(object sender, EventArgs e)
        {
            try
            {
                var backups = _backupService.GetAvailableBackups();
                _view.LoadBackupsData(backups);
                _view.AvailableBackups = backups;
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al cargar lista de backups: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnDeleteBackup(object sender, int backupIndex)
        {
            try
            {
                if (_view.AvailableBackups == null || backupIndex < 0 || backupIndex >= _view.AvailableBackups.Count)
                    return;

                var backup = _view.AvailableBackups[backupIndex];
                
                var confirmResult = System.Windows.Forms.MessageBox.Show(
                    $"�Est� seguro que desea eliminar el backup '{backup.FileName}'?\n\nEsta acci�n no se puede deshacer.",
                    "Confirmar Eliminaci�n",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question);

                if (confirmResult == System.Windows.Forms.DialogResult.Yes)
                {
                    System.IO.File.Delete(backup.FullPath);
                    
                    _view.ShowMessage("�xito", "Backup eliminado correctamente", 
                        System.Windows.Forms.MessageBoxIcon.Information);
                    
                    // Recargar lista
                    OnLoadBackupList(sender, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al eliminar backup: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnCleanupOldBackups(object sender, EventArgs e)
        {
            try
            {
                int retentionDays = _view.RetentionDays;
                
                var confirmResult = System.Windows.Forms.MessageBox.Show(
                    $"�Desea eliminar todos los backups anteriores a {retentionDays} d�as?\n\n" +
                    "Esta acci�n eliminar� autom�ticamente los backups antiguos para liberar espacio.",
                    "Confirmar Limpieza Autom�tica",
                    System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question);

                if (confirmResult == System.Windows.Forms.DialogResult.Yes)
                {
                    int deletedCount = _backupService.CleanupOldBackups(retentionDays);
                    
                    _view.ShowMessage("Limpieza Completada", 
                        $"Se eliminaron {deletedCount} backup(s) antiguos", 
                        System.Windows.Forms.MessageBoxIcon.Information);
                    
                    // Recargar lista
                    OnLoadBackupList(sender, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error durante la limpieza: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnScheduleDailyBackup(object sender, TimeSpan scheduleTime)
        {
            try
            {
                bool success = _backupService.ScheduleDailyBackup(scheduleTime);
                
                if (success)
                {
                    _view.AutoBackupEnabled = true;
                    _view.ScheduledBackupTime = scheduleTime;
                    
                    _view.ShowMessage("Backup Programado", 
                        $"Backup autom�tico configurado para las {scheduleTime:hh\\:mm} diariamente.\n\n" +
                        "Nota: Esta es una configuraci�n b�sica. En entornos de producci�n se recomienda usar SQL Server Agent.",
                        System.Windows.Forms.MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al programar backup: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnDisableScheduledBackup(object sender, EventArgs e)
        {
            try
            {
                // Deshabilitar en configuraci�n
                var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                
                if (config.AppSettings.Settings["AutoBackupEnabled"] == null)
                    config.AppSettings.Settings.Add("AutoBackupEnabled", "false");
                else
                    config.AppSettings.Settings["AutoBackupEnabled"].Value = "false";

                config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");

                _view.AutoBackupEnabled = false;
                
                _view.ShowMessage("Backup Deshabilitado", 
                    "El backup autom�tico ha sido deshabilitado", 
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al deshabilitar backup autom�tico: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnBrowseBackupPath(object sender, EventArgs e)
        {
            try
            {
                using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    folderDialog.Description = "Seleccione la carpeta para almacenar los backups";
                    folderDialog.SelectedPath = _view.BackupPath ?? @"C:\SALC\Backups";
                    folderDialog.ShowNewFolderButton = true;

                    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        _view.BackupPath = folderDialog.SelectedPath;
                    }
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al seleccionar carpeta: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}