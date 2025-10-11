// Services/SystemConfigService.cs
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para configuración del sistema (solo Administrador)
    /// Gestiona rutas de backup, políticas de contraseñas, etc.
    /// </summary>
    public class SystemConfigService
    {
        private readonly string _connectionString;

        public SystemConfigService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SALCConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Error de configuración: Cadena de conexión 'SALCConnection' no encontrada.");
        }

        /// <summary>
        /// Configuración del sistema
        /// </summary>
        public class SystemConfig
        {
            public string BackupPath { get; set; } = @"C:\SALC\Backups";
            public int PasswordExpirationDays { get; set; } = 90;
            public int MinPasswordLength { get; set; } = 8;
            public bool RequirePasswordComplexity { get; set; } = true;
            public int MaxLoginAttempts { get; set; } = 3;
            public int SessionTimeoutMinutes { get; set; } = 30;
        }

        /// <summary>
        /// Obtiene la configuración actual del sistema
        /// Nota: En v1.0 se almacenan en App.config, en futuras versiones podría ser BD
        /// </summary>
        public SystemConfig GetSystemConfig()
        {
            try
            {
                return new SystemConfig
                {
                    BackupPath = ConfigurationManager.AppSettings["BackupPath"] ?? @"C:\SALC\Backups",
                    PasswordExpirationDays = int.Parse(ConfigurationManager.AppSettings["PasswordExpirationDays"] ?? "90"),
                    MinPasswordLength = int.Parse(ConfigurationManager.AppSettings["MinPasswordLength"] ?? "8"),
                    RequirePasswordComplexity = bool.Parse(ConfigurationManager.AppSettings["RequirePasswordComplexity"] ?? "true"),
                    MaxLoginAttempts = int.Parse(ConfigurationManager.AppSettings["MaxLoginAttempts"] ?? "3"),
                    SessionTimeoutMinutes = int.Parse(ConfigurationManager.AppSettings["SessionTimeoutMinutes"] ?? "30")
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al leer configuración del sistema: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza la configuración del sistema
        /// Nota: En v1.0 actualiza App.config, requiere reinicio de aplicación
        /// </summary>
        public bool UpdateSystemConfig(SystemConfig config)
        {
            try
            {
                // Validaciones
                if (string.IsNullOrWhiteSpace(config.BackupPath))
                    throw new ArgumentException("La ruta de backup es obligatoria");

                if (config.PasswordExpirationDays < 30 || config.PasswordExpirationDays > 365)
                    throw new ArgumentException("Los días de vencimiento de contraseña deben estar entre 30 y 365");

                if (config.MinPasswordLength < 6 || config.MinPasswordLength > 20)
                    throw new ArgumentException("La longitud mínima de contraseña debe estar entre 6 y 20 caracteres");

                // Crear directorio de backup si no existe
                if (!Directory.Exists(config.BackupPath))
                {
                    Directory.CreateDirectory(config.BackupPath);
                }

                // Actualizar App.config
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                
                UpdateAppSetting(configFile, "BackupPath", config.BackupPath);
                UpdateAppSetting(configFile, "PasswordExpirationDays", config.PasswordExpirationDays.ToString());
                UpdateAppSetting(configFile, "MinPasswordLength", config.MinPasswordLength.ToString());
                UpdateAppSetting(configFile, "RequirePasswordComplexity", config.RequirePasswordComplexity.ToString());
                UpdateAppSetting(configFile, "MaxLoginAttempts", config.MaxLoginAttempts.ToString());
                UpdateAppSetting(configFile, "SessionTimeoutMinutes", config.SessionTimeoutMinutes.ToString());

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar configuración: {ex.Message}");
            }
        }

        private void UpdateAppSetting(System.Configuration.Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }

        /// <summary>
        /// Valida si una contraseña cumple con las políticas del sistema
        /// </summary>
        public ValidationResult ValidatePassword(string password)
        {
            var config = GetSystemConfig();
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(password))
            {
                result.IsValid = false;
                result.ErrorMessage = "La contraseña no puede estar vacía";
                return result;
            }

            if (password.Length < config.MinPasswordLength)
            {
                result.IsValid = false;
                result.ErrorMessage = $"La contraseña debe tener al menos {config.MinPasswordLength} caracteres";
                return result;
            }

            if (config.RequirePasswordComplexity)
            {
                bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;

                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpper = true;
                    else if (char.IsLower(c)) hasLower = true;
                    else if (char.IsDigit(c)) hasDigit = true;
                    else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
                }

                if (!hasUpper || !hasLower || !hasDigit || !hasSpecial)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "La contraseña debe contener al menos: una mayúscula, una minúscula, un número y un carácter especial";
                    return result;
                }
            }

            return result;
        }

        /// <summary>
        /// Resultado de validación
        /// </summary>
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
        }

        /// <summary>
        /// Obtiene estadísticas del sistema (para dashboard de admin)
        /// </summary>
        public SystemStats GetSystemStats()
        {
            var stats = new SystemStats();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Contar usuarios por rol
                    const string usersQuery = @"
                        SELECT r.rol, COUNT(u.dni) as cantidad
                        FROM rol r
                        LEFT JOIN usuario u ON r.id_rol = u.id_rol AND u.estado_usuario = 1
                        GROUP BY r.id_rol, r.rol
                        ORDER BY r.rol";

                    using (var command = new SqlCommand(usersQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string rol = reader.GetString(0);
                            int cantidad = reader.GetInt32(1);

                            switch (rol.ToLower())
                            {
                                case "administrador":
                                    stats.TotalAdmins = cantidad;
                                    break;
                                case "clinico":
                                    stats.TotalClinicos = cantidad;
                                    break;
                                case "asistente":
                                    stats.TotalAsistentes = cantidad;
                                    break;
                            }
                        }
                    }

                    // Otros stats
                    stats.TotalPacientes = ExecuteScalarQuery(connection, "SELECT COUNT(*) FROM paciente");
                    stats.TotalAnalisis = ExecuteScalarQuery(connection, "SELECT COUNT(*) FROM analisis");
                    stats.AnalisisPendientes = ExecuteScalarQuery(connection, "SELECT COUNT(*) FROM analisis WHERE id_estado = 1"); // Sin verificar
                    stats.AnalisisVerificados = ExecuteScalarQuery(connection, "SELECT COUNT(*) FROM analisis WHERE id_estado = 2"); // Verificados
                }

                // Información del sistema
                stats.DatabaseSize = GetDatabaseSize();
                stats.LastBackupDate = GetLastBackupDate();
                
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener estadísticas del sistema: {ex.Message}");
            }

            return stats;
        }

        private int ExecuteScalarQuery(SqlConnection connection, string query)
        {
            using (var command = new SqlCommand(query, connection))
            {
                var result = command.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        private string GetDatabaseSize()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    const string query = @"
                        SELECT CAST(SUM(size) * 8.0 / 1024 AS DECIMAL(10,2)) as SizeMB
                        FROM sys.master_files 
                        WHERE database_id = DB_ID()";

                    using (var command = new SqlCommand(query, connection))
                    {
                        var result = command.ExecuteScalar();
                        return result?.ToString() + " MB" ?? "N/A";
                    }
                }
            }
            catch
            {
                return "N/A";
            }
        }

        private DateTime? GetLastBackupDate()
        {
            try
            {
                var config = GetSystemConfig();
                var backupDir = new DirectoryInfo(config.BackupPath);
                if (!backupDir.Exists) return null;

                var latestBackup = backupDir.GetFiles("SALC_*.bak")
                    .OrderByDescending(f => f.CreationTime)
                    .FirstOrDefault();

                return latestBackup?.CreationTime;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Estadísticas del sistema
        /// </summary>
        public class SystemStats
        {
            public int TotalAdmins { get; set; }
            public int TotalClinicos { get; set; }
            public int TotalAsistentes { get; set; }
            public int TotalPacientes { get; set; }
            public int TotalAnalisis { get; set; }
            public int AnalisisPendientes { get; set; }
            public int AnalisisVerificados { get; set; }
            public string DatabaseSize { get; set; }
            public DateTime? LastBackupDate { get; set; }
        }
    }
}