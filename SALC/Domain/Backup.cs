using System;

namespace SALC.Domain
{
    /// <summary>
    /// Entidad que representa el historial de copias de seguridad manuales ejecutadas en el sistema
    /// </summary>
    public class HistorialBackup
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string RutaArchivo { get; set; }
        public long TamanoArchivo { get; set; } // en bytes
        public string Estado { get; set; } // Exitoso, Error
        public string Observaciones { get; set; }
        public int DniUsuario { get; set; } // Usuario (Administrador) que ejecutó el backup manual
    }

    /// <summary>
    /// Resultado de la ejecución de un backup
    /// </summary>
    public class BackupResult
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string RutaArchivo { get; set; }
        public long TamanoArchivo { get; set; }
        public DateTime FechaEjecucion { get; set; }
    }
}