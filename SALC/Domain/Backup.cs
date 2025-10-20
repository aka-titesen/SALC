using System;

namespace SALC.Domain
{
    // Entidades relacionadas con el sistema de backups
    
    public class HistorialBackup
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string RutaArchivo { get; set; }
        public long TamanoArchivo { get; set; } // en bytes
        public string Estado { get; set; } // Exitoso, Error
        public string Observaciones { get; set; }
        public string TipoBackup { get; set; } // Manual, Automatico
        public int? DniUsuario { get; set; } // Usuario que ejecutó el backup manual
    }

    public class ConfiguracionBackup
    {
        public int Id { get; set; }
        public bool BackupAutomaticoHabilitado { get; set; }
        public string HoraProgramada { get; set; } // Formato HH:mm
        public string DiasSemana { get; set; } // Días separados por coma: "1,2,3,4,5" (L-V)
        public string RutaDestino { get; set; }
        public int DiasRetencion { get; set; } // Días para conservar backups antiguos
        public DateTime? UltimaEjecucion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int DniUsuarioModificacion { get; set; }
    }

    public class BackupResult
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public string RutaArchivo { get; set; }
        public long TamanoArchivo { get; set; }
        public DateTime FechaEjecucion { get; set; }
    }
}