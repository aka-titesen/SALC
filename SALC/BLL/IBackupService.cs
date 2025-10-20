using System.Collections.Generic;
using SALC.Domain;

namespace SALC.BLL
{
    public interface IBackupService
    {
        // Ejecución de backups
        void EjecutarBackup(string rutaArchivoBak, int? dniUsuario = null, string tipoBackup = "Manual");
        void EjecutarBackupAutomatico();
        
        // Gestión del historial
        List<HistorialBackup> ObtenerHistorialBackups(int limite = 50);
        HistorialBackup ObtenerUltimoBackup();
        void LimpiarBackupsAntiguos(int diasRetencion);
        
        // Configuración
        ConfiguracionBackup ObtenerConfiguracion();
        void ActualizarConfiguracion(ConfiguracionBackup config);
        
        // Programación automática
        void ProgramarBackup(string expresionHorario, string rutaArchivoBak);
        void ProgramarBackupAutomatico(ConfiguracionBackup config);
        void EliminarTareaProgramada();
        bool ExisteTareaProgramada();
        string ObtenerEstadoTarea();
        
        // Utilidades
        string FormatearTamanoArchivo(long bytes);
    }
}
