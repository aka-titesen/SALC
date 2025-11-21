using System;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using SALC.Infraestructura;
using SALC.Domain;
using SALC.DAL;

namespace SALC.BLL
{
    /// <summary>
    /// Servicio de lógica de negocio para la gestión de copias de seguridad de la base de datos.
    /// Permite ejecutar backups manuales, consultar el historial y limpiar backups antiguos.
    /// </summary>
    public class BackupService : IBackupService
    {
        private readonly BackupRepositorio _backupRepo;

        /// <summary>
        /// Constructor del servicio de backup
        /// </summary>
        public BackupService()
        {
            _backupRepo = new BackupRepositorio();
        }

        #region Ejecución de Backup Manual

        /// <summary>
        /// Ejecuta una copia de seguridad manual de la base de datos
        /// </summary>
        /// <param name="rutaArchivoBak">Ruta completa donde se guardará el archivo de backup</param>
        /// <param name="dniUsuario">DNI del administrador que ejecuta el backup</param>
        public void EjecutarBackupManual(string rutaArchivoBak, int dniUsuario)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivoBak))
                throw new ArgumentException("La ruta del archivo de backup no puede estar vacía.", nameof(rutaArchivoBak));

            if (dniUsuario <= 0)
                throw new ArgumentException("El DNI del usuario debe ser válido.", nameof(dniUsuario));

            var historial = new HistorialBackup
            {
                FechaHora = DateTime.Now,
                RutaArchivo = rutaArchivoBak,
                DniUsuario = dniUsuario
            };

            try
            {
                var directorio = Path.GetDirectoryName(rutaArchivoBak);
                if (string.IsNullOrEmpty(directorio))
                    throw new ArgumentException("No se pudo determinar el directorio de la ruta especificada.", nameof(rutaArchivoBak));

                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SALC"].ConnectionString;
                var builder = new SqlConnectionStringBuilder(connectionString);
                var nombreBaseDatos = builder.InitialCatalog;

                if (string.IsNullOrEmpty(nombreBaseDatos))
                    throw new InvalidOperationException("No se pudo determinar el nombre de la base de datos del connection string.");

                var sqlBackup = $"BACKUP DATABASE [{nombreBaseDatos}] TO DISK = @ruta WITH INIT, STATS = 5";
                
                using (var conexion = DbConexion.CrearConexion())
                using (var comando = new SqlCommand(sqlBackup, conexion))
                {
                    comando.Parameters.AddWithValue("@ruta", rutaArchivoBak);
                    comando.CommandTimeout = 0;
                    
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }

                if (File.Exists(rutaArchivoBak))
                {
                    var archivoInfo = new FileInfo(rutaArchivoBak);
                    historial.TamanoArchivo = archivoInfo.Length;
                }
                else
                {
                    throw new FileNotFoundException("El archivo de backup no se creó correctamente.", rutaArchivoBak);
                }

                historial.Estado = "Exitoso";
                historial.Observaciones = $"Backup manual ejecutado correctamente. Base de datos: {nombreBaseDatos}";
            }
            catch (Exception ex)
            {
                historial.Estado = "Error";
                historial.Observaciones = $"Error al ejecutar backup: {ex.Message}";
                
                try
                {
                    _backupRepo.InsertarHistorial(historial);
                }
                catch
                {
                }
                
                throw;
            }
            finally
            {
                if (historial.Estado == "Exitoso")
                {
                    _backupRepo.InsertarHistorial(historial);
                }
            }
        }

        #endregion

        #region Gestión del Historial

        /// <summary>
        /// Obtiene el historial de backups ordenado por fecha descendente
        /// </summary>
        /// <param name="limite">Cantidad máxima de registros a retornar</param>
        /// <returns>Lista con el historial de backups</returns>
        public List<HistorialBackup> ObtenerHistorialBackups(int limite = 50)
        {
            if (limite <= 0)
                throw new ArgumentException("El límite debe ser mayor a cero.", nameof(limite));

            return _backupRepo.ObtenerHistorial(limite);
        }

        /// <summary>
        /// Obtiene información del último backup exitoso ejecutado
        /// </summary>
        /// <returns>Información del último backup o null si no hay registros</returns>
        public HistorialBackup ObtenerUltimoBackup()
        {
            return _backupRepo.ObtenerUltimoBackup();
        }

        /// <summary>
        /// Elimina registros de historial y archivos físicos de backups más antiguos que el período especificado
        /// </summary>
        /// <param name="diasRetencion">Cantidad de días de retención de backups</param>
        public void LimpiarBackupsAntiguos(int diasRetencion)
        {
            if (diasRetencion <= 0)
                throw new ArgumentException("Los días de retención deben ser mayor a cero.", nameof(diasRetencion));

            _backupRepo.LimpiarHistorialAntiguo(diasRetencion);
            
            try
            {
                EliminarArchivosAntiguos(diasRetencion);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Elimina archivos físicos de backup más antiguos que el período especificado
        /// </summary>
        /// <param name="diasRetencion">Cantidad de días de retención</param>
        private void EliminarArchivosAntiguos(int diasRetencion)
        {
            var rutas = _backupRepo.ObtenerRutasBackups();
            
            var fechaLimite = DateTime.Now.AddDays(-diasRetencion);
            
            foreach (var ruta in rutas)
            {
                try
                {
                    if (File.Exists(ruta))
                    {
                        var archivoInfo = new FileInfo(ruta);
                        if (archivoInfo.CreationTime < fechaLimite)
                        {
                            File.Delete(ruta);
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        #endregion

        #region Utilidades

        /// <summary>
        /// Formatea un tamaño en bytes a una representación legible (B, KB, MB, GB)
        /// </summary>
        /// <param name="bytes">Tamaño en bytes</param>
        /// <returns>Cadena formateada con el tamaño legible</returns>
        public string FormatearTamanoArchivo(long bytes)
        {
            if (bytes < 1024) 
                return $"{bytes} B";
            
            if (bytes < 1024 * 1024) 
                return $"{bytes / 1024.0:F1} KB";
            
            if (bytes < 1024L * 1024L * 1024L) 
                return $"{bytes / (1024.0 * 1024.0):F1} MB";
            
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
        }

        #endregion
    }
}
