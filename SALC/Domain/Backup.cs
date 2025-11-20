using System;

namespace SALC.Domain
{
    /// <summary>
    /// Representa el historial de copias de seguridad manuales ejecutadas en el sistema.
    /// Registra información sobre cada backup realizado.
    /// </summary>
    public class HistorialBackup
    {
        /// <summary>
        /// Identificador único del registro de backup
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha y hora en que se ejecutó el backup
        /// </summary>
        public DateTime FechaHora { get; set; }

        /// <summary>
        /// Ruta completa del archivo de backup generado
        /// </summary>
        public string RutaArchivo { get; set; }

        /// <summary>
        /// Tamaño del archivo de backup en bytes
        /// </summary>
        public long TamanoArchivo { get; set; }

        /// <summary>
        /// Estado del backup. Valores posibles: Exitoso, Error
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Observaciones o detalles adicionales sobre el backup
        /// </summary>
        public string Observaciones { get; set; }

        /// <summary>
        /// DNI del usuario administrador que ejecutó el backup manual
        /// </summary>
        public int DniUsuario { get; set; }
    }

    /// <summary>
    /// Representa el resultado de la ejecución de un proceso de backup.
    /// Contiene información sobre el éxito o fracaso de la operación.
    /// </summary>
    public class BackupResult
    {
        /// <summary>
        /// Indica si el backup se ejecutó exitosamente
        /// </summary>
        public bool Exitoso { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado de la operación
        /// </summary>
        public string Mensaje { get; set; }

        /// <summary>
        /// Ruta completa del archivo de backup generado
        /// </summary>
        public string RutaArchivo { get; set; }

        /// <summary>
        /// Tamaño del archivo de backup en bytes
        /// </summary>
        public long TamanoArchivo { get; set; }

        /// <summary>
        /// Fecha y hora en que se ejecutó el backup
        /// </summary>
        public DateTime FechaEjecucion { get; set; }
    }
}