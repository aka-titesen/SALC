using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de copias de seguridad de la base de datos.
    /// Define las operaciones para ejecutar, consultar y gestionar backups del sistema.
    /// </summary>
    public interface IBackupService
    {
        /// <summary>
        /// Ejecuta una copia de seguridad manual de la base de datos
        /// </summary>
        /// <param name="rutaArchivoBak">Ruta completa donde se guardará el archivo de backup</param>
        /// <param name="dniUsuario">DNI del administrador que ejecuta el backup</param>
        void EjecutarBackupManual(string rutaArchivoBak, int dniUsuario);
        
        /// <summary>
        /// Obtiene el historial de backups ejecutados
        /// </summary>
        /// <param name="limite">Número máximo de registros a obtener</param>
        /// <returns>Lista de registros de historial de backups</returns>
        List<HistorialBackup> ObtenerHistorialBackups(int limite = 50);
        
        /// <summary>
        /// Obtiene información del último backup exitoso
        /// </summary>
        /// <returns>Registro del último backup exitoso o null si no existe</returns>
        HistorialBackup ObtenerUltimoBackup();
        
        /// <summary>
        /// Elimina registros de historial y archivos de backups más antiguos que el período de retención
        /// </summary>
        /// <param name="diasRetencion">Cantidad de días de retención de backups</param>
        void LimpiarBackupsAntiguos(int diasRetencion);
        
        /// <summary>
        /// Formatea un tamaño en bytes a una representación legible (B, KB, MB, GB)
        /// </summary>
        /// <param name="bytes">Tamaño en bytes</param>
        /// <returns>Cadena formateada con la unidad apropiada</returns>
        string FormatearTamanoArchivo(long bytes);
    }
}
