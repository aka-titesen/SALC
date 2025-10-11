using System;
using System.Configuration;
using System.IO;

namespace SALC.Services
{
    /// <summary>
    /// Servicio para configuración del sistema con convención 'Service'.
    /// </summary>
    public class ConfiguracionSistemaService
    {
        public class ConfiguracionSistema
        {
            public string RutaBackups { get; set; } = @"C:\\SALC\\Backups";
            public int DiasVencimientoPassword { get; set; } = 90;
            public int LongitudMinimaPassword { get; set; } = 8;
            public bool RequiereComplejidadPassword { get; set; } = true;
            public int IntentosMaximosLogin { get; set; } = 3;
            public int MinutosTimeoutSesion { get; set; } = 30;
        }

        public ConfiguracionSistema ObtenerConfiguracionSistema()
        {
            try
            {
                return new ConfiguracionSistema
                {
                    RutaBackups = ConfigurationManager.AppSettings["BackupPath"] ?? @"C:\\SALC\\Backups",
                    DiasVencimientoPassword = int.Parse(ConfigurationManager.AppSettings["PasswordExpirationDays"] ?? "90"),
                    LongitudMinimaPassword = int.Parse(ConfigurationManager.AppSettings["MinPasswordLength"] ?? "8"),
                    RequiereComplejidadPassword = bool.Parse(ConfigurationManager.AppSettings["RequirePasswordComplexity"] ?? "true"),
                    IntentosMaximosLogin = int.Parse(ConfigurationManager.AppSettings["MaxLoginAttempts"] ?? "3"),
                    MinutosTimeoutSesion = int.Parse(ConfigurationManager.AppSettings["SessionTimeoutMinutes"] ?? "30")
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al leer configuración del sistema: {ex.Message}");
            }
        }

        public bool ActualizarConfiguracionSistema(ConfiguracionSistema config)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(config.RutaBackups))
                    throw new ArgumentException("La ruta de backups es obligatoria");

                if (config.DiasVencimientoPassword < 30 || config.DiasVencimientoPassword > 365)
                    throw new ArgumentException("Los días de vencimiento de contraseña deben estar entre 30 y 365");

                if (config.LongitudMinimaPassword < 6 || config.LongitudMinimaPassword > 20)
                    throw new ArgumentException("La longitud mínima de contraseña debe estar entre 6 y 20 caracteres");

                if (!Directory.Exists(config.RutaBackups))
                {
                    Directory.CreateDirectory(config.RutaBackups);
                }

                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                SetAppSetting(configFile, "BackupPath", config.RutaBackups);
                SetAppSetting(configFile, "PasswordExpirationDays", config.DiasVencimientoPassword.ToString());
                SetAppSetting(configFile, "MinPasswordLength", config.LongitudMinimaPassword.ToString());
                SetAppSetting(configFile, "RequirePasswordComplexity", config.RequiereComplejidadPassword.ToString());
                SetAppSetting(configFile, "MaxLoginAttempts", config.IntentosMaximosLogin.ToString());
                SetAppSetting(configFile, "SessionTimeoutMinutes", config.MinutosTimeoutSesion.ToString());

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar configuración: {ex.Message}");
            }
        }

        private void SetAppSetting(System.Configuration.Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }
    }
}
