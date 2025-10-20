using System;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using SALC.Infraestructura;
using SALC.Domain;
using SALC.DAL;

namespace SALC.BLL
{
    public class BackupService : IBackupService
    {
        private readonly BackupRepositorio _backupRepo;
        private const string TASK_NAME = "SALC_BackupAutomatico";

        public BackupService()
        {
            _backupRepo = new BackupRepositorio();
        }

        #region Ejecución de Backups

        public void EjecutarBackup(string rutaArchivoBak, int? dniUsuario = null, string tipoBackup = "Manual")
        {
            var historial = new HistorialBackup
            {
                FechaHora = DateTime.Now,
                RutaArchivo = rutaArchivoBak,
                TipoBackup = tipoBackup,
                DniUsuario = dniUsuario
            };

            try
            {
                if (string.IsNullOrWhiteSpace(rutaArchivoBak))
                    throw new ArgumentException("Ruta de backup inválida.", nameof(rutaArchivoBak));

                var dir = Path.GetDirectoryName(rutaArchivoBak);
                if (!Directory.Exists(dir)) 
                    Directory.CreateDirectory(dir);

                // Detectar base de datos del connection string
                var cs = System.Configuration.ConfigurationManager.ConnectionStrings["SALC"].ConnectionString;
                var builder = new SqlConnectionStringBuilder(cs);
                var database = builder.InitialCatalog;

                var tsql = $"BACKUP DATABASE [{database}] TO DISK = @ruta WITH INIT, STATS = 5";
                using (var cn = DbConexion.CrearConexion())
                using (var cmd = new SqlCommand(tsql, cn))
                {
                    cmd.Parameters.AddWithValue("@ruta", rutaArchivoBak);
                    cn.Open();
                    cmd.CommandTimeout = 0; // permitir backups largos
                    cmd.ExecuteNonQuery();
                }

                // Obtener tamaño del archivo creado
                if (File.Exists(rutaArchivoBak))
                {
                    var fileInfo = new FileInfo(rutaArchivoBak);
                    historial.TamanoArchivo = fileInfo.Length;
                }

                historial.Estado = "Exitoso";
                historial.Observaciones = "Backup completado correctamente";

                // Actualizar última ejecución si es automático
                if (tipoBackup == "Automatico")
                {
                    _backupRepo.ActualizarUltimaEjecucion(DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                historial.Estado = "Error";
                historial.Observaciones = ex.Message;
                throw;
            }
            finally
            {
                // Siempre registrar en el historial
                _backupRepo.InsertarHistorial(historial);
            }
        }

        public void EjecutarBackupAutomatico()
        {
            var config = ObtenerConfiguracion();
            
            if (!config.BackupAutomaticoHabilitado)
                return;

            // Generar nombre de archivo con timestamp
            var nombreArchivo = $"SALC_AUTO_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            var rutaCompleta = Path.Combine(config.RutaDestino ?? @"C:\Backups\SALC", nombreArchivo);

            EjecutarBackup(rutaCompleta, null, "Automatico");
            
            // Limpiar backups antiguos según configuración
            LimpiarBackupsAntiguos(config.DiasRetencion);
        }

        #endregion

        #region Gestión del Historial

        public List<HistorialBackup> ObtenerHistorialBackups(int limite = 50)
        {
            return _backupRepo.ObtenerHistorial(limite);
        }

        public HistorialBackup ObtenerUltimoBackup()
        {
            return _backupRepo.ObtenerUltimoBackup();
        }

        public void LimpiarBackupsAntiguos(int diasRetencion)
        {
            _backupRepo.LimpiarHistorialAntiguo(diasRetencion);
            
            // También eliminar archivos físicos antiguos si existen
            var config = ObtenerConfiguracion();
            var rutaBackups = config.RutaDestino ?? @"C:\Backups\SALC";
            
            if (Directory.Exists(rutaBackups))
            {
                var archivos = Directory.GetFiles(rutaBackups, "*.bak");
                var fechaLimite = DateTime.Now.AddDays(-diasRetencion);
                
                foreach (var archivo in archivos)
                {
                    var fileInfo = new FileInfo(archivo);
                    if (fileInfo.CreationTime < fechaLimite)
                    {
                        try
                        {
                            File.Delete(archivo);
                        }
                        catch
                        {
                            // Ignorar errores al eliminar archivos (pueden estar en uso)
                        }
                    }
                }
            }
        }

        #endregion

        #region Configuración

        public ConfiguracionBackup ObtenerConfiguracion()
        {
            return _backupRepo.ObtenerConfiguracion();
        }

        public void ActualizarConfiguracion(ConfiguracionBackup config)
        {
            config.FechaModificacion = DateTime.Now;
            _backupRepo.ActualizarConfiguracion(config);
        }

        #endregion

        #region Programación Automática

        public void ProgramarBackup(string expresionHorario, string rutaArchivoBak)
        {
            // Programar usando la configuración actual
            var config = ObtenerConfiguracion();
            ProgramarBackupAutomatico(config);
        }

        public void ProgramarBackupAutomatico(ConfiguracionBackup config)
        {
            if (!config.BackupAutomaticoHabilitado)
            {
                EliminarTareaProgramada();
                return;
            }

            try
            {
                // Construir comando para ejecutar la aplicación con parámetros de backup
                var rutaEjecutable = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var comandoBackup = $"\"{rutaEjecutable}\" /backup /auto";

                // Crear comando schtasks para programar la tarea
                var comando = BuildScheduleCommand(config, comandoBackup);
                
                // Eliminar tarea existente si existe
                EliminarTareaProgramada();
                
                // Crear nueva tarea
                var resultado = EjecutarComando(comando);
                
                if (resultado.ExitCode != 0)
                {
                    throw new Exception($"Error al programar backup automático: {resultado.Output}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"No se pudo programar el backup automático: {ex.Message}");
            }
        }

        public void EliminarTareaProgramada()
        {
            try
            {
                var comando = $"schtasks /delete /tn \"{TASK_NAME}\" /f";
                EjecutarComando(comando);
            }
            catch
            {
                // Ignorar errores al eliminar (la tarea podría no existir)
            }
        }

        public bool ExisteTareaProgramada()
        {
            try
            {
                var comando = $"schtasks /query /tn \"{TASK_NAME}\"";
                var resultado = EjecutarComando(comando);
                return resultado.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        public string ObtenerEstadoTarea()
        {
            try
            {
                var comando = $"schtasks /query /tn \"{TASK_NAME}\" /fo csv /nh";
                var resultado = EjecutarComando(comando);
                
                if (resultado.ExitCode == 0 && !string.IsNullOrEmpty(resultado.Output))
                {
                    // Parsear CSV básico para obtener el estado
                    var campos = resultado.Output.Split(',');
                    if (campos.Length > 3)
                    {
                        return campos[3].Trim('"');
                    }
                }
                
                return "Desconocido";
            }
            catch
            {
                return "Error";
            }
        }

        #endregion

        #region Utilidades

        public string FormatearTamanoArchivo(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024.0):F1} MB";
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }

        #endregion

        #region Métodos Privados de Programación

        private string BuildScheduleCommand(ConfiguracionBackup config, string comandoBackup)
        {
            var diasSemana = ConvertirDiasSemana(config.DiasSemana);
            
            return $"schtasks /create /tn \"{TASK_NAME}\" " +
                   $"/tr \"{comandoBackup}\" " +
                   $"/sc weekly " +
                   $"/d {diasSemana} " +
                   $"/st {config.HoraProgramada} " +
                   $"/ru SYSTEM " +
                   $"/f " +
                   $"/rl HIGHEST";
        }

        private string ConvertirDiasSemana(string diasConfig)
        {
            if (string.IsNullOrEmpty(diasConfig))
                return "MON,TUE,WED,THU,FRI";

            var dias = diasConfig.Split(',');
            var diasWindows = new List<string>();

            foreach (var dia in dias)
            {
                switch (dia.Trim())
                {
                    case "0": diasWindows.Add("SUN"); break;
                    case "1": diasWindows.Add("MON"); break;
                    case "2": diasWindows.Add("TUE"); break;
                    case "3": diasWindows.Add("WED"); break;
                    case "4": diasWindows.Add("THU"); break;
                    case "5": diasWindows.Add("FRI"); break;
                    case "6": diasWindows.Add("SAT"); break;
                }
            }

            return diasWindows.Count > 0 ? string.Join(",", diasWindows) : "MON,TUE,WED,THU,FRI";
        }

        private CommandResult EjecutarComando(string comando)
        {
            var result = new CommandResult();
            
            using (var process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/C {comando}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                
                result.Output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                
                if (!string.IsNullOrEmpty(error))
                {
                    result.Output += Environment.NewLine + error;
                }
                
                process.WaitForExit();
                result.ExitCode = process.ExitCode;
            }

            return result;
        }

        private class CommandResult
        {
            public int ExitCode { get; set; }
            public string Output { get; set; } = "";
        }

        #endregion
    }
}
