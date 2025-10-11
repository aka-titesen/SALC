// Services/BackupService.cs
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para backup y restore de la base de datos (solo Administrador)
    /// Utiliza SQL Server Agent o PowerShell según ERS-SALC_IEEE830
    /// </summary>
    public class BackupService
    {
        private readonly string _connectionString;
        private readonly SystemConfigService _configService;

        public BackupService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");
            
            _configService = new SystemConfigService();
        }

        /// <summary>
        /// Información de un backup
        /// </summary>
        public class BackupInfo
        {
            public string FileName { get; set; }
            public DateTime CreationDate { get; set; }
            public long SizeBytes { get; set; }
            public string SizeMB => $"{SizeBytes / 1024.0 / 1024.0:F2} MB";
            public string FullPath { get; set; }
        }

        /// <summary>
        /// Resultado de operación de backup
        /// </summary>
        public class BackupResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string BackupFilePath { get; set; }
            public TimeSpan Duration { get; set; }
        }

        /// <summary>
        /// Ejecuta un backup completo de la base de datos SALC
        /// Guarda log con Serilog según ERS
        /// </summary>
        public BackupResult ExecuteFullBackup(string customPath = null)
        {
            var startTime = DateTime.Now;
            var result = new BackupResult { Success = false };

            try
            {
                // Obtener configuración
                var config = _configService.GetSystemConfig();
                string backupDir = customPath ?? config.BackupPath;

                // Crear directorio si no existe
                if (!Directory.Exists(backupDir))
                    Directory.CreateDirectory(backupDir);

                // Nombre del archivo de backup
                string fileName = $"SALC_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string backupPath = Path.Combine(backupDir, fileName);

                // SQL para backup
                string backupSql = $@"
                    BACKUP DATABASE [SALC] 
                    TO DISK = @BackupPath
                    WITH FORMAT, 
                         INIT, 
                         NAME = 'SALC-Backup Completo', 
                         DESCRIPTION = 'Backup automático generado por SALC',
                         SKIP, 
                         NOREWIND, 
                         NOUNLOAD, 
                         COMPRESSION,
                         STATS = 10";

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(backupSql, connection))
                    {
                        command.CommandTimeout = 300; // 5 minutos timeout
                        command.Parameters.AddWithValue("@BackupPath", backupPath);
                        
                        command.ExecuteNonQuery();
                    }
                }

                result.Success = true;
                result.BackupFilePath = backupPath;
                result.Message = $"Backup completado exitosamente en: {backupPath}";
                result.Duration = DateTime.Now - startTime;

                // Log con Serilog (se implementará cuando se agregue Serilog)
                LogBackupEvent($"Backup ejecutado exitosamente: {fileName}", true);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error durante el backup: {ex.Message}";
                result.Duration = DateTime.Now - startTime;

                // Log error
                LogBackupEvent($"Error en backup: {ex.Message}", false);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Obtiene la lista de backups disponibles
        /// </summary>
        public List<BackupInfo> GetAvailableBackups()
        {
            var backups = new List<BackupInfo>();
            
            try
            {
                var config = _configService.GetSystemConfig();
                var backupDir = new DirectoryInfo(config.BackupPath);

                if (!backupDir.Exists)
                    return backups;

                var backupFiles = backupDir.GetFiles("SALC_*.bak")
                    .OrderByDescending(f => f.CreationTime);

                foreach (var file in backupFiles)
                {
                    backups.Add(new BackupInfo
                    {
                        FileName = file.Name,
                        CreationDate = file.CreationTime,
                        SizeBytes = file.Length,
                        FullPath = file.FullName
                    });
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener lista de backups: {ex.Message}");
            }

            return backups;
        }

        /// <summary>
        /// Restaura la base de datos desde un backup
        /// ADVERTENCIA: Esta operación es destructiva
        /// </summary>
        public BackupResult RestoreFromBackup(string backupFilePath, bool forceReplace = false)
        {
            var startTime = DateTime.Now;
            var result = new BackupResult { Success = false };

            try
            {
                if (!File.Exists(backupFilePath))
                    throw new FileNotFoundException($"El archivo de backup no existe: {backupFilePath}");

                // Validar que no hay conexiones activas (simplificado para v1.0)
                if (!forceReplace)
                {
                    int activeConnections = GetActiveConnections();
                    if (activeConnections > 1) // > 1 porque la nuestra cuenta
                    {
                        throw new InvalidOperationException($"Hay {activeConnections - 1} conexiones activas. Cierre todas las sesiones antes de restaurar o use forceReplace=true");
                    }
                }

                // SQL para restore
                string restoreSql = $@"
                    RESTORE DATABASE [SALC] 
                    FROM DISK = @BackupPath
                    WITH FILE = 1, 
                         NOUNLOAD, 
                         REPLACE, 
                         STATS = 10";

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    // Poner en modo SINGLE_USER para forzar desconexión
                    if (forceReplace)
                    {
                        using (var singleUserCmd = new SqlCommand("ALTER DATABASE [SALC] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", connection))
                        {
                            singleUserCmd.ExecuteNonQuery();
                        }
                    }

                    // Ejecutar restore
                    using (var command = new SqlCommand(restoreSql, connection))
                    {
                        command.CommandTimeout = 600; // 10 minutos timeout
                        command.Parameters.AddWithValue("@BackupPath", backupFilePath);
                        
                        command.ExecuteNonQuery();
                    }

                    // Volver a MULTI_USER
                    if (forceReplace)
                    {
                        using (var multiUserCmd = new SqlCommand("ALTER DATABASE [SALC] SET MULTI_USER", connection))
                        {
                            multiUserCmd.ExecuteNonQuery();
                        }
                    }
                }

                result.Success = true;
                result.Message = $"Restore completado exitosamente desde: {Path.GetFileName(backupFilePath)}";
                result.Duration = DateTime.Now - startTime;

                LogBackupEvent($"Restore ejecutado exitosamente desde: {Path.GetFileName(backupFilePath)}", true);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error durante el restore: {ex.Message}";
                result.Duration = DateTime.Now - startTime;

                LogBackupEvent($"Error en restore: {ex.Message}", false);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Elimina backups antiguos según política de retención (7 días según ERS)
        /// </summary>
        public int CleanupOldBackups(int retentionDays = 7)
        {
            int deletedCount = 0;

            try
            {
                var config = _configService.GetSystemConfig();
                var backupDir = new DirectoryInfo(config.BackupPath);

                if (!backupDir.Exists)
                    return 0;

                var cutoffDate = DateTime.Now.AddDays(-retentionDays);
                var oldBackups = backupDir.GetFiles("SALC_*.bak")
                    .Where(f => f.CreationTime < cutoffDate);

                foreach (var file in oldBackups)
                {
                    try
                    {
                        file.Delete();
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        LogBackupEvent($"Error al eliminar backup antiguo {file.Name}: {ex.Message}", false);
                    }
                }

                if (deletedCount > 0)
                {
                    LogBackupEvent($"Limpieza de backups: {deletedCount} archivos eliminados (retención: {retentionDays} días)", true);
                }

            }
            catch (Exception ex)
            {
                LogBackupEvent($"Error durante limpieza de backups: {ex.Message}", false);
                throw;
            }

            return deletedCount;
        }

        /// <summary>
        /// Programa un backup automático diario (usando Task Scheduler de Windows)
        /// Implementación simplificada para v1.0
        /// </summary>
        public bool ScheduleDailyBackup(TimeSpan scheduleTime)
        {
            try
            {
                // En v1.0 solo guardamos la configuración
                // Una implementación completa usaría Task Scheduler API
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                
                if (config.AppSettings.Settings["AutoBackupEnabled"] == null)
                    config.AppSettings.Settings.Add("AutoBackupEnabled", "true");
                else
                    config.AppSettings.Settings["AutoBackupEnabled"].Value = "true";

                if (config.AppSettings.Settings["AutoBackupTime"] == null)
                    config.AppSettings.Settings.Add("AutoBackupTime", scheduleTime.ToString());
                else
                    config.AppSettings.Settings["AutoBackupTime"].Value = scheduleTime.ToString();

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                LogBackupEvent($"Backup automático programado para las {scheduleTime:hh\\:mm}", true);
                return true;
            }
            catch (Exception ex)
            {
                LogBackupEvent($"Error al programar backup automático: {ex.Message}", false);
                return false;
            }
        }

        private int GetActiveConnections()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = "SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE database_id = DB_ID('SALC')";
                    using (var command = new SqlCommand(query, connection))
                    {
                        return (int)command.ExecuteScalar();
                    }
                }
            }
            catch
            {
                return 1; // Asumir solo nuestra conexión si hay error
            }
        }

        private void LogBackupEvent(string message, bool success)
        {
            try
            {
                // Por ahora log simple a archivo, luego se integrará Serilog
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                    "SALC", 
                    "Logs", 
                    "backup.log"
                );

                Directory.CreateDirectory(Path.GetDirectoryName(logPath));

                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{(success ? "INFO" : "ERROR")}] {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logEntry, Encoding.UTF8);
            }
            catch
            {
                // Silently ignore log errors
            }
        }
    }
}