using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    /// <summary>
    /// Interfaz para el servicio de copias de seguridad manuales de la base de datos
    /// </summary>
    public interface IBackupService
    {
        /// <summary>
        /// Ejecuta una copia de seguridad manual de la base de datos
        /// </summary>
        /// <param name="rutaArchivoBak">Ruta completa donde se guardará el archivo .bak</param>
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
        /// Elimina registros de historial y archivos de backups antiguos
        /// </summary>
        /// <param name="diasRetencion">Días de antigüedad para considerar un backup como antiguo</param>
        void LimpiarBackupsAntiguos(int diasRetencion);
        
        /// <summary>
        /// Formatea un tamaño en bytes a una representación legible (KB, MB, GB)
        /// </summary>
        /// <param name="bytes">Tamaño en bytes</param>
        /// <returns>Cadena formateada con la unidad apropiada</returns>
        string FormatearTamanoArchivo(long bytes);
    }
}
