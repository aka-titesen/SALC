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
    /// Servicio para gestionar copias de seguridad manuales de la base de datos
    /// Implementa el patrón de 3 capas: esta capa contiene la lógica de negocio
    /// </summary>
    public class BackupService : IBackupService
    {
        private readonly BackupRepositorio _backupRepo;

        public BackupService()
        {
            _backupRepo = new BackupRepositorio();
        }

        #region Ejecución de Backup Manual

        /// <summary>
        /// Ejecuta una copia de seguridad manual de la base de datos SQL Server
        /// </summary>
        /// <param name="rutaArchivoBak">Ruta completa donde se guardará el archivo .bak</param>
        /// <param name="dniUsuario">DNI del administrador que ejecuta el backup</param>
        /// <exception cref="ArgumentException">Si la ruta es inválida</exception>
        /// <exception cref="SqlException">Si hay un error al ejecutar el backup en SQL Server</exception>
        public void EjecutarBackupManual(string rutaArchivoBak, int dniUsuario)
        {
            // Validaciones de negocio
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
                // Validar y crear directorio si no existe
                var directorio = Path.GetDirectoryName(rutaArchivoBak);
                if (string.IsNullOrEmpty(directorio))
                    throw new ArgumentException("No se pudo determinar el directorio de la ruta especificada.", nameof(rutaArchivoBak));

                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                // Obtener el nombre de la base de datos del connection string
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SALC"].ConnectionString;
                var builder = new SqlConnectionStringBuilder(connectionString);
                var nombreBaseDatos = builder.InitialCatalog;

                if (string.IsNullOrEmpty(nombreBaseDatos))
                    throw new InvalidOperationException("No se pudo determinar el nombre de la base de datos del connection string.");

                // Ejecutar el comando BACKUP DATABASE en SQL Server
                var sqlBackup = $"BACKUP DATABASE [{nombreBaseDatos}] TO DISK = @ruta WITH INIT, STATS = 5";
                
                using (var conexion = DbConexion.CrearConexion())
                using (var comando = new SqlCommand(sqlBackup, conexion))
                {
                    comando.Parameters.AddWithValue("@ruta", rutaArchivoBak);
                    comando.CommandTimeout = 0; // Sin timeout para permitir backups grandes
                    
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }

                // Obtener información del archivo creado
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
                
                // Registrar en historial antes de relanzar la excepción
                try
                {
                    _backupRepo.InsertarHistorial(historial);
                }
                catch
                {
                    // Ignorar errores al guardar historial si el backup falló
                }
                
                throw; // Relanzar la excepción original
            }
            finally
            {
                // Siempre registrar en el historial si el backup fue exitoso
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
        public List<HistorialBackup> ObtenerHistorialBackups(int limite = 50)
        {
            if (limite <= 0)
                throw new ArgumentException("El límite debe ser mayor a cero.", nameof(limite));

            return _backupRepo.ObtenerHistorial(limite);
        }

        /// <summary>
        /// Obtiene información del último backup exitoso ejecutado
        /// </summary>
        public HistorialBackup ObtenerUltimoBackup()
        {
            return _backupRepo.ObtenerUltimoBackup();
        }

        /// <summary>
        /// Elimina registros de historial y archivos físicos de backups más antiguos que el período especificado
        /// </summary>
        public void LimpiarBackupsAntiguos(int diasRetencion)
        {
            if (diasRetencion <= 0)
                throw new ArgumentException("Los días de retención deben ser mayor a cero.", nameof(diasRetencion));

            // Limpiar registros de historial en la base de datos
            _backupRepo.LimpiarHistorialAntiguo(diasRetencion);
            
            // Intentar eliminar archivos físicos antiguos
            // Nota: Esto es una operación de mejor esfuerzo, no debe fallar si hay problemas con archivos
            try
            {
                EliminarArchivosAntiguos(diasRetencion);
            }
            catch
            {
                // Ignorar errores al eliminar archivos físicos
                // El historial ya fue limpiado en la BD
            }
        }

        /// <summary>
        /// Elimina archivos físicos de backup más antiguos que el período especificado
        /// </summary>
        private void EliminarArchivosAntiguos(int diasRetencion)
        {
            // Obtener todas las rutas únicas del historial
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
                    // Ignorar errores individuales (archivo en uso, sin permisos, etc.)
                    continue;
                }
            }
        }

        #endregion

        #region Utilidades

        /// <summary>
        /// Formatea un tamaño en bytes a una representación legible
        /// </summary>
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
