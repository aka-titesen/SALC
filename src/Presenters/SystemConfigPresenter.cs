// Presenters/SystemConfigPresenter.cs
using System;
using SALC.Views.Interfaces;
using SALC.Services;

namespace SALC.Presenters
{
    /// <summary>
    /// Presenter para configuraci�n del sistema (solo Administrador)
    /// Implementa la gesti�n de pol�ticas y par�metros del sistema
    /// </summary>
    public class SystemConfigPresenter
    {
        private readonly ISystemConfigView _view;
        private readonly SystemConfigService _configService;

        public SystemConfigPresenter(ISystemConfigView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _configService = new SystemConfigService();
            
            SubscribeToViewEvents();
            LoadInitialData();
        }

        private void SubscribeToViewEvents()
        {
            _view.LoadConfiguration += OnLoadConfiguration;
            _view.SaveConfiguration += OnSaveConfiguration;
            _view.TestBackupPath += OnTestBackupPath;
            _view.ViewSystemStats += OnViewSystemStats;
        }

        private void LoadInitialData()
        {
            try
            {
                OnLoadConfiguration(null, EventArgs.Empty);
                OnViewSystemStats(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al cargar datos iniciales: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnLoadConfiguration(object sender, EventArgs e)
        {
            try
            {
                var config = _configService.GetSystemConfig();
                _view.LoadConfigurationData(config);
                _view.CurrentConfig = config;
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al cargar configuraci�n: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnSaveConfiguration(object sender, SystemConfigService.SystemConfig config)
        {
            try
            {
                _view.ClearValidationErrors();

                // Validaciones del lado del presenter
                if (!ValidateConfiguration(config))
                    return;

                bool success = _configService.UpdateSystemConfig(config);
                
                if (success)
                {
                    _view.ShowMessage("�xito", 
                        "Configuraci�n guardada correctamente.\n\nNota: Algunos cambios requieren reiniciar la aplicaci�n.", 
                        System.Windows.Forms.MessageBoxIcon.Information);
                    
                    // Recargar configuraci�n
                    OnLoadConfiguration(sender, EventArgs.Empty);
                }
            }
            catch (ArgumentException ex)
            {
                _view.ShowMessage("Error de Validaci�n", ex.Message, System.Windows.Forms.MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al guardar configuraci�n: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private bool ValidateConfiguration(SystemConfigService.SystemConfig config)
        {
            bool isValid = true;

            // Validar ruta de backup
            if (string.IsNullOrWhiteSpace(config.BackupPath))
            {
                _view.ShowValidationError("BackupPath", "La ruta de backup es obligatoria");
                isValid = false;
            }
            else if (config.BackupPath.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
            {
                _view.ShowValidationError("BackupPath", "La ruta de backup contiene caracteres no v�lidos");
                isValid = false;
            }

            // Validar d�as de vencimiento de contrase�a
            if (config.PasswordExpirationDays < 30 || config.PasswordExpirationDays > 365)
            {
                _view.ShowValidationError("PasswordExpirationDays", "Los d�as deben estar entre 30 y 365");
                isValid = false;
            }

            // Validar longitud m�nima de contrase�a
            if (config.MinPasswordLength < 6 || config.MinPasswordLength > 20)
            {
                _view.ShowValidationError("MinPasswordLength", "La longitud debe estar entre 6 y 20 caracteres");
                isValid = false;
            }

            // Validar intentos m�ximos de login
            if (config.MaxLoginAttempts < 3 || config.MaxLoginAttempts > 10)
            {
                _view.ShowValidationError("MaxLoginAttempts", "Los intentos deben estar entre 3 y 10");
                isValid = false;
            }

            // Validar timeout de sesi�n
            if (config.SessionTimeoutMinutes < 5 || config.SessionTimeoutMinutes > 240)
            {
                _view.ShowValidationError("SessionTimeoutMinutes", "El timeout debe estar entre 5 y 240 minutos");
                isValid = false;
            }

            return isValid;
        }

        private void OnTestBackupPath(object sender, EventArgs e)
        {
            try
            {
                string backupPath = _view.CurrentConfig?.BackupPath;
                
                if (string.IsNullOrWhiteSpace(backupPath))
                {
                    _view.ShowMessage("Validaci�n", "Debe especificar una ruta de backup", 
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                // Probar crear el directorio
                if (!System.IO.Directory.Exists(backupPath))
                {
                    System.IO.Directory.CreateDirectory(backupPath);
                }

                // Probar escribir un archivo de prueba
                string testFile = System.IO.Path.Combine(backupPath, "test_salc_backup.tmp");
                System.IO.File.WriteAllText(testFile, "Test file for SALC backup");
                System.IO.File.Delete(testFile);

                _view.ShowMessage("Prueba Exitosa", 
                    $"La ruta de backup es v�lida y accesible:\n{backupPath}", 
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                _view.ShowMessage("Error de Acceso", 
                    "No tiene permisos para escribir en la ruta especificada", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                _view.ShowMessage("Error de Ruta", 
                    "No se pudo crear el directorio en la ruta especificada", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", 
                    $"Error al probar la ruta de backup: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void OnViewSystemStats(object sender, EventArgs e)
        {
            try
            {
                var stats = _configService.GetSystemStats();
                _view.LoadSystemStatsData(stats);
                _view.SystemStats = stats;
            }
            catch (Exception ex)
            {
                _view.ShowMessage("Error", $"Error al cargar estad�sticas del sistema: {ex.Message}", 
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}